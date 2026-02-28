using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Supercell.Magic.Logic;
using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Avatar.Change;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Battle;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Logic.Home.Change;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Logic.Message.Home;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Logic.Time;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Helper;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Request.Stream;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Core.Settings;
using Supercell.Magic.Servers.Core.Util;

using Supercell.Magic.Servers.Home.Cluster;
using Supercell.Magic.Servers.Home.Logic.Mode.Listener;
using Supercell.Magic.Servers.Home.Session;

using Supercell.Magic.Titan.Exceptions;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Home.Logic.Mode
{
	public class GameMode
	{
		public const long MAX_LOGIC_LOOP_TIME = 750L;

		private readonly HomeSession m_session;
		private readonly LogicGameMode m_logicGameMode;
		private readonly Stopwatch m_logicWatch;
		private readonly ServerCommandStorage m_serverCommandStorage;
		private readonly GameListener m_gameListener;

		private bool m_shouldDestruct;
		private bool m_destructed;
		private bool m_gameDefenderLocked;

		private LogicLong m_liveReplayId;
		private AvatarChangeListener m_avatarChangeListener;

		private GameMode(HomeSession session)
		{
			m_session = session;
			m_logicWatch = new Stopwatch();
			m_logicGameMode = new LogicGameMode();
			m_serverCommandStorage = new ServerCommandStorage(this, m_logicGameMode);
			m_gameListener = new GameListener(this);

			m_logicGameMode.GetLevel().SetGameListener(m_gameListener);
			m_logicGameMode.GetCommandManager().SetListener(m_serverCommandStorage);
		}

		public async void Destruct()
		{
			if (!m_destructed)
			{
				LogicLevel logicLevel = m_logicGameMode.GetLevel();

				if (m_logicGameMode.GetState() == 2 && logicLevel.GetBattleLog().GetBattleStarted())
				{
					if (!m_logicGameMode.IsBattleOver())
					{
						await Task.Run(() => SimulateEndAttackState());
						SaveState();
					}

					if (m_logicGameMode.GetLevel().GetMatchType() == 1)
						CreateBattleReport();

					if (m_liveReplayId != null)
					{
						ServerMessageManager.SendMessage(new EndLiveReplayMessage
						{
							AccountId = m_liveReplayId
						}, 9);
					}
				}

				logicLevel.GetHome().SetChangeListener(new LogicHomeChangeListener());
				logicLevel.GetPlayerAvatar().SetChangeListener(new LogicAvatarChangeListener());

				m_logicGameMode.Destruct();
				m_destructed = true;
			}
		}

		public void OnClientTurnReceived(int subTick, int checksum, LogicArrayList<LogicCommand> commands)
		{
			if (m_destructed || m_logicGameMode.GetState() == 4 || m_logicGameMode.GetState() == 5)
				return;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int logicTimestamp = m_logicGameMode.GetStartTime() + LogicTime.GetTicksInSeconds(subTick);

			if (currentTimestamp + 1 >= logicTimestamp)
			{
				if (commands != null)
				{
					m_serverCommandStorage.CheckExecutableServerCommands(subTick, commands);

					for (int i = 0; i < commands.Size(); i++)
					{
						m_logicGameMode.GetCommandManager().AddCommand(commands[i]);
					}
				}

				int previousSubTick = m_logicGameMode.GetLevel().GetLogicTime().GetTick();

				try
				{
					m_logicWatch.Start();

					for (int i = 0, count = subTick - previousSubTick; i < count; i++)
					{
						m_logicGameMode.UpdateOneSubTick();

						if (m_logicWatch.ElapsedMilliseconds >= GameMode.MAX_LOGIC_LOOP_TIME)
						{
							Logging.Error(string.Format("GameMode.onClientTurnReceived: logic update stopped because it took too long. ({0}ms for {1} updates)",
														m_logicWatch.ElapsedMilliseconds, i));
							break;
						}
					}

					GameModeClusterManager.ReportLogicUpdateSpeed(m_logicWatch.ElapsedMilliseconds);
					m_logicWatch.Reset();
				}
				catch (LogicException exception)
				{
					Logging.Error("GameMode.onClientTurnReceived: logic exception thrown: " + exception + " (acc id: " + (long)m_session.AccountId + ")");
					ServerErrorMessage serverErrorMessage = new ServerErrorMessage();
					serverErrorMessage.SetErrorMessage(exception.Message);
					m_session.SendPiranhaMessage(serverErrorMessage, 1);
					m_session.SendMessage(new StopSessionMessage(), 1);
				}
				catch (Exception exception)
				{
					Logging.Error("GameMode.onClientTurnReceived: exception thrown: " + exception + " (acc id: " + (long)m_session.AccountId + ")");
					m_session.SendMessage(new StopSessionMessage(), 1);
				}

				SaveState();
				CheckChecksum(checksum);

				if (m_liveReplayId != null)
					UpdateLiveReplay(subTick, commands);
				if (m_logicGameMode.IsBattleOver())
					m_shouldDestruct = true;
				if (m_shouldDestruct)
					m_session.DestructGameMode();
			}
			else
			{
				m_session.SendMessage(new StopSessionMessage(), 1);
			}
		}

		private void CreateBattleReport()
		{
			LogicClientAvatar attacker = (LogicClientAvatar)m_logicGameMode.GetLevel().GetVisitorAvatar();
			LogicClientAvatar defender = (LogicClientAvatar)m_logicGameMode.GetLevel().GetHomeOwnerAvatar();

			string attackerBattleLogJSON = LogicJSONParser.CreateJSONString(m_logicGameMode.GetLevel().GetBattleLog().GenerateAttackerJSON());
			string defenderBattleLogJSON = LogicJSONParser.CreateJSONString(m_logicGameMode.GetLevel().GetBattleLog().GenerateDefenderJSON());

			ServerRequestManager.Create(new CreateReplayStreamRequestMessage
			{
				JSON = LogicJSONParser.CreateJSONString(m_logicGameMode.GetReplay().GetJson(), 1536)
			}, ServerManager.GetNextSocket(11)).OnComplete = args =>
			{
				LogicLong replayId = null;

				if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
					replayId = ((CreateReplayStreamResponseMessage)args.ResponseMessage).Id;

				BattleReportStreamEntry attackerBattleReportStreamEntry = new BattleReportStreamEntry(AvatarStreamEntryType.ATTACKER_BATTLE_REPORT);

				attackerBattleReportStreamEntry.SetSenderAvatarId(defender.GetId());
				attackerBattleReportStreamEntry.SetSenderLevel(defender.GetExpLevel());
				attackerBattleReportStreamEntry.SetSenderLeagueType(defender.GetLeagueType());
				attackerBattleReportStreamEntry.SetSenderName(defender.GetName());
				attackerBattleReportStreamEntry.SetMajorVersion(LogicVersion.MAJOR_VERSION);
				attackerBattleReportStreamEntry.SetBuildVersion(LogicVersion.BUILD_VERSION);
				attackerBattleReportStreamEntry.SetContentVersion(ResourceManager.GetContentVersion());
				attackerBattleReportStreamEntry.SetBattleLogJSON(attackerBattleLogJSON);
				attackerBattleReportStreamEntry.SetReplayId(replayId);

				ServerRequestManager.Create(new CreateAvatarStreamRequestMessage
				{
					OwnerId = attacker.GetId(),
					Entry = attackerBattleReportStreamEntry
				}, ServerManager.GetDocumentSocket(11, attacker.GetId())).OnComplete = args2 =>
				{
					if (args2.ErrorCode == ServerRequestError.Success && args2.ResponseMessage.Success)
					{
						ServerMessageManager.SendMessage(new CreateAvatarStreamMessage
						{
							AccountId = attacker.GetId(),
							Entry = ((CreateAvatarStreamResponseMessage)args2.ResponseMessage).Entry
						}, 9);
					}
				};

				if (m_gameDefenderLocked)
				{
					BattleReportStreamEntry defenderBattleReportStreamEntry = new BattleReportStreamEntry(AvatarStreamEntryType.DEFENDER_BATTLE_REPORT);

					defenderBattleReportStreamEntry.SetSenderAvatarId(attacker.GetId());
					defenderBattleReportStreamEntry.SetSenderLevel(attacker.GetExpLevel());
					defenderBattleReportStreamEntry.SetSenderLeagueType(attacker.GetLeagueType());
					defenderBattleReportStreamEntry.SetSenderName(attacker.GetName());
					defenderBattleReportStreamEntry.SetMajorVersion(LogicVersion.MAJOR_VERSION);
					defenderBattleReportStreamEntry.SetBuildVersion(LogicVersion.BUILD_VERSION);
					defenderBattleReportStreamEntry.SetContentVersion(ResourceManager.GetContentVersion());
					defenderBattleReportStreamEntry.SetBattleLogJSON(defenderBattleLogJSON);
					defenderBattleReportStreamEntry.SetReplayId(replayId);

					ServerRequestManager.Create(new CreateAvatarStreamRequestMessage
					{
						OwnerId = defender.GetId(),
						Entry = defenderBattleReportStreamEntry
					}, ServerManager.GetDocumentSocket(11, defender.GetId())).OnComplete = args2 =>
					{
						if (args2.ErrorCode == ServerRequestError.Success && args2.ResponseMessage.Success)
						{
							ServerMessageManager.SendMessage(new CreateAvatarStreamMessage
							{
								AccountId = defender.GetId(),
								Entry = ((CreateAvatarStreamResponseMessage)args2.ResponseMessage).Entry
							}, 9);
						}
					};
				}
			};
		}

		private void SimulateEndAttackState()
		{
			LogicLevel level = m_logicGameMode.GetLevel();
			LogicGameObjectManager gameObjectManager = level.GetGameObjectManager();
			LogicArrayList<LogicGameObject> characterList = gameObjectManager.GetGameObjects(LogicGameObjectType.CHARACTER);
			LogicArrayList<LogicGameObject> projectileList = gameObjectManager.GetGameObjects(LogicGameObjectType.PROJECTILE);
			LogicArrayList<LogicGameObject> spellList = gameObjectManager.GetGameObjects(LogicGameObjectType.SPELL);
			LogicArrayList<LogicGameObject> alliancePortalList = gameObjectManager.GetGameObjects(LogicGameObjectType.ALLIANCE_PORTAL);

			m_logicWatch.Start();

			while (!m_logicGameMode.IsBattleOver())
			{
				bool canStopBattle = !m_logicGameMode.GetConfiguration().GetBattleWaitForProjectileDestruction() || projectileList.Size() == 0;

				for (int i = 0; i < characterList.Size(); i++)
				{
					LogicCharacter character = (LogicCharacter)characterList[i];
					LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

					if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
					{
						LogicAttackerItemData data = character.GetAttackerItemData();

						if (data.GetDamage(0, false) > 0 &&
							(hitpointComponent.GetHitpoints() > 0 || m_logicGameMode.GetConfiguration().GetBattleWaitForDieDamage() && character.GetWaitDieDamage()))
						{
							canStopBattle = false;
						}
					}
				}

				for (int i = 0; i < spellList.Size(); i++)
				{
					LogicSpell spell = (LogicSpell)spellList[i];

					if (!spell.GetHitsCompleted() && (spell.GetSpellData().IsDamageSpell() || spell.GetSpellData().GetSummonTroop() != null))
						canStopBattle = false;
				}

				for (int i = 0; i < alliancePortalList.Size(); i++)
				{
					LogicAlliancePortal alliancePortal = (LogicAlliancePortal)alliancePortalList[i];

					if (alliancePortal.GetBunkerComponent().GetTeam() == 0 && !alliancePortal.GetBunkerComponent().IsEmpty())
						canStopBattle = false;
				}

				bool isEnded = canStopBattle || m_logicWatch.ElapsedMilliseconds >= 10000;

				if (isEnded)
				{
					LogicEndCombatCommand logicEndCombatCommand = new LogicEndCombatCommand();
					logicEndCombatCommand.SetExecuteSubTick(m_logicGameMode.GetLevel().GetLogicTime().GetTick());
					m_logicGameMode.GetCommandManager().AddCommand(logicEndCombatCommand);
				}

				m_logicGameMode.UpdateOneSubTick();

				if (isEnded)
					break;
			}

			m_logicWatch.Reset();

			if (!m_logicGameMode.IsBattleOver())
				m_logicGameMode.SetBattleOver();
			if (m_liveReplayId != null)
				UpdateLiveReplay(m_logicGameMode.GetLevel().GetLogicTime().GetTick(), null);
		}

		private void SaveState()
		{
			if (m_logicGameMode.GetState() == 1)
			{
				LogicJSONObject jsonObject = new LogicJSONObject(64);

				m_logicGameMode.SaveToJSON(jsonObject);

				ZLibHelper.CompressInZLibFormat(LogicStringUtil.GetBytes(LogicJSONParser.CreateJSONString(jsonObject, 1536)), out byte[] homeJSON);
				ServerMessageManager.SendMessage(new GameStateCallbackMessage
				{
					AccountId = m_logicGameMode.GetLevel().GetPlayerAvatar().GetId(),
					SessionId = m_session.Id,
					LogicClientAvatar = m_logicGameMode.GetLevel().GetPlayerAvatar(),
					AvatarChanges = m_avatarChangeListener.RemoveAvatarChanges(),
					ExecutedServerCommands = m_serverCommandStorage.RemoveExecutedServerCommands(),
					HomeJSON = homeJSON,
					RemainingShieldTime = m_logicGameMode.GetShieldRemainingSeconds(),
					RemainingGuardTime = m_logicGameMode.GetGuardRemainingSeconds(),
					NextPersonalBreakTime = m_logicGameMode.GetPersonalBreakCooldownSeconds(),
					SaveTime = TimeUtil.GetTimestamp()
				}, 9);
			}
			else
			{
				ServerMessageManager.SendMessage(new GameStateCallbackMessage
				{
					AccountId = m_logicGameMode.GetLevel().GetPlayerAvatar().GetId(),
					SessionId = m_session.Id,
					LogicClientAvatar = m_logicGameMode.GetLevel().GetPlayerAvatar(),
					AvatarChanges = m_avatarChangeListener.RemoveAvatarChanges()
				}, 9);
			}
		}

		private void CheckChecksum(int clientChecksum)
		{
			LogicJSONObject debugJSON = new LogicJSONObject();
			ChecksumHelper checksum = m_logicGameMode.CalculateChecksum(debugJSON, EnvironmentSettings.Settings.ContentValidationModeEnabled);

			if (checksum.GetChecksum() != clientChecksum)
			{
				OutOfSyncMessage outOfSyncMessage = new OutOfSyncMessage();

				outOfSyncMessage.SetSubTick(m_logicGameMode.GetLevel().GetLogicTime().GetTick());
				outOfSyncMessage.SetClientChecksum(clientChecksum);
				outOfSyncMessage.SetServerChecksum(checksum.GetChecksum());
				outOfSyncMessage.SetDebugJSON(debugJSON);

				m_session.SendPiranhaMessage(outOfSyncMessage, 1);
				m_shouldDestruct = true;
			}
		}

		private void UpdateLiveReplay(int clientSubTick, LogicArrayList<LogicCommand> commands)
		{
			ServerMessageManager.SendMessage(new ClientUpdateLiveReplayMessage
			{
				AccountId = m_liveReplayId,
				SubTick = clientSubTick,
				Commands = commands
			}, 9);
		}

		public void SetShouldDestruct()
		{
			m_shouldDestruct = true;
		}

		public HomeSession GetSession()
			=> m_session;

		public LogicGameMode GetLogicGameMode()
			=> m_logicGameMode;

		public LogicClientAvatar GetPlayerAvatar()
			=> m_logicGameMode.GetLevel().GetPlayerAvatar();

		public AvatarChangeListener GetAvatarChangeListener()
			=> m_avatarChangeListener;

		public bool GetAwaitingExecutionOfCommandType(LogicCommandType type)
			=> m_serverCommandStorage.GetAwaitingExecutionOfCommandType(type);

		public void AddServerCommand(LogicServerCommand serverCommand)
		{
			if (m_logicGameMode.GetState() != 1)
				throw new Exception("GameMode.addServerCommand: Method called in invalid game state.");

			m_serverCommandStorage.AddServerCommand(serverCommand);

			AvailableServerCommandMessage availableServerCommandMessage = new AvailableServerCommandMessage();
			availableServerCommandMessage.SetServerCommand(serverCommand);
			m_session.SendPiranhaMessage(availableServerCommandMessage, 1);
		}

		public byte[] CreateChallengeSnapshot()
		{
			if (m_logicGameMode.GetState() != 1)
				throw new Exception("GameMode.createChallengeSnapshot called in invalid logic state.");

			LogicLevel logicLevel = m_logicGameMode.GetLevel();
			LogicJSONObject jsonObject = new LogicJSONObject(64);

			jsonObject.Put("exp_ver", new LogicJSONNumber(logicLevel.GetExperienceVersion()));
			logicLevel.GetGameObjectManagerAt(0).SaveToSnapshot(jsonObject, 6);

			m_logicGameMode.GetLevel().GetHomeOwnerAvatar().SaveToDirect(jsonObject);

			ZLibHelper.CompressInZLibFormat(LogicStringUtil.GetBytes(LogicJSONParser.CreateJSONString(jsonObject, 1536)), out byte[] homeJSON);

			return homeJSON;
		}

		public static GameMode LoadHomeState(HomeSession session, GameHomeState state)
		{
			LogicClientHome home = state.Home;
			LogicClientAvatar homeOwnerAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;
			int secondsSinceLastMaintenance = state.MaintenanceTime != -1 ? currentTimestamp - state.MaintenanceTime : 0;
			int reengagementSeconds = secondsSinceLastSave >= 86400 ? secondsSinceLastSave - 86400 : 0;

			OwnHomeDataMessage ownHomeDataMessage = new OwnHomeDataMessage();

			ownHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
			ownHomeDataMessage.SetReengagementSeconds(reengagementSeconds);
			ownHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			ownHomeDataMessage.SetSecondsSinceLastMaintenance(secondsSinceLastMaintenance);
			ownHomeDataMessage.SetLogicClientHome(home);
			ownHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
			ownHomeDataMessage.SetLayoutId(state.LayoutId);
			ownHomeDataMessage.SetMapId(state.MapId);
			ownHomeDataMessage.Encode();

			CompressibleStringHelper.Uncompress(home.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_avatarChangeListener = new AvatarChangeListener(gameMode, homeOwnerAvatar);

				home.SetChangeListener(new HomeChangeListener(gameMode));
				homeOwnerAvatar.SetChangeListener(gameMode.m_avatarChangeListener);

				gameMode.m_logicGameMode.LoadHomeState(home, homeOwnerAvatar, secondsSinceLastSave, -1, currentTimestamp, secondsSinceLastMaintenance, reengagementSeconds);

				session.SendPiranhaMessage(ownHomeDataMessage, 1);

				for (int i = 0; i < state.ServerCommands.Size(); i++)
				{
					gameMode.AddServerCommand(state.ServerCommands[i]);
				}

				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadHomeState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		public static GameMode LoadNpcAttackState(HomeSession session, GameNpcAttackState state)
		{
			LogicClientHome home = state.Home;
			LogicNpcAvatar homeOwnerAvatar = state.NpcAvatar;
			LogicClientAvatar visitorAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;

			NpcDataMessage npcDataMessage = new NpcDataMessage();

			npcDataMessage.SetCurrentTimestamp(currentTimestamp);
			npcDataMessage.SetLogicClientHome(home);
			npcDataMessage.SetLogicClientAvatar(visitorAvatar);
			npcDataMessage.SetLogicNpcAvatar(homeOwnerAvatar);
			npcDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			npcDataMessage.SetNpcDuel(false);
			npcDataMessage.Encode();

			CompressibleStringHelper.Uncompress(home.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_avatarChangeListener = new AvatarChangeListener(gameMode, visitorAvatar);
				visitorAvatar.SetChangeListener(gameMode.m_avatarChangeListener);

				gameMode.m_logicGameMode.LoadNpcAttackState(home, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave);

				session.SendPiranhaMessage(npcDataMessage, 1);

				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadNpcAttackState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		public static GameMode LoadNpcDuelState(HomeSession session, GameNpcDuelState state)
		{
			LogicClientHome home = state.Home;
			LogicNpcAvatar homeOwnerAvatar = state.NpcAvatar;
			LogicClientAvatar visitorAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;

			NpcDataMessage npcDataMessage = new NpcDataMessage();

			npcDataMessage.SetCurrentTimestamp(currentTimestamp);
			npcDataMessage.SetLogicClientHome(home);
			npcDataMessage.SetLogicClientAvatar(visitorAvatar);
			npcDataMessage.SetLogicNpcAvatar(homeOwnerAvatar);
			npcDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			npcDataMessage.SetNpcDuel(true);
			npcDataMessage.Encode();

			CompressibleStringHelper.Uncompress(home.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_avatarChangeListener = new AvatarChangeListener(gameMode, visitorAvatar);
				visitorAvatar.SetChangeListener(gameMode.m_avatarChangeListener);

				gameMode.m_logicGameMode.LoadNpcDuelState(home, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave);

				session.SendPiranhaMessage(npcDataMessage, 1);

				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadNpcDuelState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		public static GameMode LoadMatchedAttackState(HomeSession session, GameMatchedAttackState state)
		{
			LogicClientHome home = state.Home;
			LogicClientAvatar homeOwnerAvatar = state.HomeOwnerAvatar;
			LogicClientAvatar attackerLogicClientAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;
			int secondsSinceLastMaintenance = state.MaintenanceTime != -1 ? currentTimestamp - state.MaintenanceTime : 0;

			EnemyHomeDataMessage enemyHomeDataMessage = new EnemyHomeDataMessage();

			enemyHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
			enemyHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			enemyHomeDataMessage.SetSecondsSinceLastMaintenance(secondsSinceLastMaintenance);
			enemyHomeDataMessage.SetLogicClientHome(home);
			enemyHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
			enemyHomeDataMessage.SetAttackerLogicClientAvatar(attackerLogicClientAvatar);
			enemyHomeDataMessage.SetAttackSource(3);
			enemyHomeDataMessage.Encode();

			CompressibleStringHelper.Uncompress(home.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_avatarChangeListener = new AvatarChangeListener(gameMode, attackerLogicClientAvatar);
				attackerLogicClientAvatar.SetChangeListener(gameMode.m_avatarChangeListener);

				gameMode.m_logicGameMode.LoadMatchedAttackState(home, homeOwnerAvatar, attackerLogicClientAvatar, currentTimestamp, secondsSinceLastSave, secondsSinceLastMaintenance);
				gameMode.m_gameDefenderLocked = state.GameDefenderLocked;
				gameMode.m_liveReplayId = state.LiveReplayId;

				ZLibHelper.CompressInZLibFormat(LogicStringUtil.GetBytes(LogicJSONParser.CreateJSONString(gameMode.m_logicGameMode.GetReplay().GetJson())), out byte[] streamJSON);
				ServerMessageManager.SendMessage(new InitializeLiveReplayMessage
				{
					AccountId = gameMode.m_liveReplayId,
					StreamData = streamJSON
				}, 9);

				session.SendPiranhaMessage(enemyHomeDataMessage, 1);

				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadMatchedAttackState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		public static GameMode LoadVisitAttackState(HomeSession session, GameVisitState state)
		{
			LogicClientHome home = state.Home;
			LogicClientAvatar homeOwnerAvatar = state.HomeOwnerAvatar;
			LogicClientAvatar visitorAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;
			int visitType = state.VisitType;

			VisitedHomeDataMessage visitedHomeDataMessage = new VisitedHomeDataMessage();

			visitedHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
			visitedHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			visitedHomeDataMessage.SetLogicClientHome(home);
			visitedHomeDataMessage.SetOwnerLogicClientAvatar(homeOwnerAvatar);
			visitedHomeDataMessage.SetVisitorLogicClientAvatar(visitorAvatar);
			visitedHomeDataMessage.Encode();

			CompressibleStringHelper.Uncompress(home.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(home.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_avatarChangeListener = new AvatarChangeListener(gameMode, visitorAvatar);
				visitorAvatar.SetChangeListener(gameMode.m_avatarChangeListener);

				gameMode.m_logicGameMode.LoadVisitState(home, homeOwnerAvatar, visitorAvatar, secondsSinceLastSave, currentTimestamp);

				session.SendPiranhaMessage(visitedHomeDataMessage, 1);

				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadVisitAttackState: exception while the loading of visit state: " + exception);
			}

			return null;
		}
	}
}