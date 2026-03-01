
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
using Supercell.Magic.Logic.Message.Home;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Servers.Battle.Cluster;
using Supercell.Magic.Servers.Battle.Logic.Mode.Listener;
using Supercell.Magic.Servers.Battle.Session;
using Supercell.Magic.Servers.Battle.Util;
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
using Supercell.Magic.Titan.Exceptions;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Battle.Logic.Mode
{
	public class GameMode
	{
		public const long MAX_LOGIC_LOOP_TIME = 1000L;

		private readonly BattleSession m_session;
		private readonly LogicGameMode m_logicGameMode;
		private readonly Stopwatch m_logicWatch;
		private readonly ServerCommandStorage m_serverCommandStorage;
		private readonly GameListener m_gameListener;

		private bool m_shouldDestruct;
		private bool m_destructed;

		private LogicLong m_liveReplayId;
		private LogicLong m_challengeStreamId;
		private LogicLong m_challengeAllianceId;
		private AvatarChangeListener m_avatarChangeListener;

		private GameMode(BattleSession session)
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

						if (m_avatarChangeListener != null)
							SaveState();
					}

					if (m_liveReplayId != null)
					{
						ServerMessageManager.SendMessage(new EndLiveReplayMessage
						{
							AccountId = m_liveReplayId
						}, 9);
					}

					if (m_challengeStreamId != null)
						CreateChallengeReplay();
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

							File.WriteAllText(string.Concat("logic-stopped-", m_session.AccountId, ".txt"), ToString());
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

				CheckChecksum(checksum);

				if (m_avatarChangeListener != null)
					SaveState();
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

		private void SimulateEndAttackState()
		{
			LogicLevel level = m_logicGameMode.GetLevel();
			LogicGameObjectManager gameObjectManager = level.GetGameObjectManager();
			LogicArrayList<LogicGameObject> characterList = gameObjectManager.GetGameObjects(LogicGameObjectType.CHARACTER);
			LogicArrayList<LogicGameObject> projectileList = gameObjectManager.GetGameObjects(LogicGameObjectType.PROJECTILE);
			LogicArrayList<LogicGameObject> spellList = gameObjectManager.GetGameObjects(LogicGameObjectType.SPELL);
			LogicArrayList<LogicGameObject> alliancePortalList = gameObjectManager.GetGameObjects(LogicGameObjectType.ALLIANCE_PORTAL);

			try
			{
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
			}
			catch (LogicException)
			{
			}
			catch (Exception exc)
			{
				Logging.Error(string.Concat("GameMode.simulateEndAttackState: exception thrown: ", exc, " (acc id: ", (long)m_session.AccountId, ")"));
			}


			if (!m_logicGameMode.IsBattleOver())
			{

				m_logicGameMode.SetBattleOver();
			}
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
					ExecutedServerCommands = new LogicArrayList<LogicServerCommand>(),
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

		private void CreateChallengeReplay()
		{
			string battleLog = LogicJSONParser.CreateJSONString(m_logicGameMode.GetLevel().GetBattleLog().GenerateAttackerJSON());

			ServerRequestManager.Create(new CreateReplayStreamRequestMessage
			{
				JSON = LogicJSONParser.CreateJSONString(m_logicGameMode.GetReplay().GetJson(), 1536)
			}, ServerManager.GetNextSocket(11)).OnComplete = args =>
			{
				LogicLong replayId = null;

				if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
					replayId = ((CreateReplayStreamResponseMessage)args.ResponseMessage).Id;

				ServerMessageManager.SendMessage(new AllianceChallengeReportMessage
				{
					AccountId = m_challengeAllianceId,
					StreamId = m_challengeStreamId,
					ReplayId = replayId,
					BattleLog = battleLog
				}, 11);
			};
		}

		public void SetShouldDestruct()
		{
			m_shouldDestruct = true;
		}

		public BattleSession GetSession()
			=> m_session;

		public LogicGameMode GetLogicGameMode()
			=> m_logicGameMode;

		public LogicClientAvatar GetPlayerAvatar()
			=> m_logicGameMode.GetLevel().GetPlayerAvatar();

		public AvatarChangeListener GetAvatarChangeListener()
			=> m_avatarChangeListener;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("account: " + m_session.AccountId);
			stringBuilder.AppendLine("state: " + m_logicGameMode.GetState());
			stringBuilder.AppendLine("numGameObjects: " + m_logicGameMode.GetLevel().GetGameObjectManager().GetNumGameObjects());
			stringBuilder.AppendLine("numCharacterCount: " + m_logicGameMode.GetLevel().GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER).Size());

			return stringBuilder.ToString();
		}

		public static GameMode LoadFakeAttackState(BattleSession session, GameFakeAttackState state)
		{
			//LogicClientHome home = state.Home;
			LogicClientHome logicClientHome = SetLogicClientHome(state.HomeId, state.HomeData, 0, 0, 0);

			LogicClientAvatar homeOwnerAvatar = state.HomeOwnerAvatar;
			LogicClientAvatar playerAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;
			int secondsSinceLastMaintenance = state.MaintenanceTime != -1 ? currentTimestamp - state.MaintenanceTime : 0;

			EnemyHomeDataMessage enemyHomeDataMessage = new EnemyHomeDataMessage();

			enemyHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
			enemyHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			enemyHomeDataMessage.SetSecondsSinceLastMaintenance(secondsSinceLastMaintenance);
			enemyHomeDataMessage.SetLogicClientHome(logicClientHome);
			enemyHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
			enemyHomeDataMessage.SetAttackerLogicClientAvatar(playerAvatar);
			enemyHomeDataMessage.SetAttackSource(1);
			enemyHomeDataMessage.Encode();

			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);
				gameMode.m_logicGameMode.LoadDirectAttackState(logicClientHome, homeOwnerAvatar, playerAvatar, secondsSinceLastSave, false, false, currentTimestamp);
				session.SendPiranhaMessage(enemyHomeDataMessage, 1);
				return gameMode;
			}
			catch (Exception exception)
			{
				Logging.Error("GameMode.loadFakeAttackState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		public static GameMode LoadChallengeAttackState(BattleSession session, GameChallengeAttackState state)
		{
			//LogicClientHome home = state.Home;

			LogicClientHome logicClientHome = SetLogicClientHome(state.HomeId, state.HomeData, 0, 0, 0);

			LogicClientAvatar homeOwnerAvatar = GameUtil.LoadHomeOwnerAvatarFromHome(logicClientHome);
			LogicClientAvatar playerAvatar = state.PlayerAvatar;

			int currentTimestamp = TimeUtil.GetTimestamp();
			int secondsSinceLastSave = state.SaveTime != -1 ? currentTimestamp - state.SaveTime : 0;
			int secondsSinceLastMaintenance = 0;

			EnemyHomeDataMessage enemyHomeDataMessage = new EnemyHomeDataMessage();

			enemyHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
			enemyHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
			enemyHomeDataMessage.SetSecondsSinceLastMaintenance(secondsSinceLastMaintenance);
			enemyHomeDataMessage.SetLogicClientHome(logicClientHome);
			enemyHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
			enemyHomeDataMessage.SetAttackerLogicClientAvatar(playerAvatar);
			enemyHomeDataMessage.SetAttackSource(5);
			enemyHomeDataMessage.SetMapId(state.MapId);
			enemyHomeDataMessage.Encode();

			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleHomeJSON());
			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleCalendarJSON());
			CompressibleStringHelper.Uncompress(logicClientHome.GetCompressibleGlobalJSON());

			try
			{
				GameMode gameMode = new GameMode(session);

				gameMode.m_logicGameMode.LoadDirectAttackState(logicClientHome, homeOwnerAvatar, playerAvatar, secondsSinceLastSave, state.MapId == 1, false, currentTimestamp);
				gameMode.m_challengeAllianceId = state.AllianceId;
				gameMode.m_challengeStreamId = state.StreamId;
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
				Logging.Error("GameMode.loadChallengeAttackState: exception while the loading of attack state: " + exception);
			}

			return null;
		}

		private static LogicClientHome SetLogicClientHome(LogicLong homeId, byte[] homeJson, int shieldDurationSeconds, int guardDurationSeconds, int personalBreakSeconds)
		{
			LogicClientHome logicClientHome = new LogicClientHome();

			logicClientHome.SetHomeId(homeId);

			logicClientHome.SetShieldDurationSeconds(shieldDurationSeconds);
			logicClientHome.SetGuardDurationSeconds(guardDurationSeconds);
			logicClientHome.SetPersonalBreakSeconds(personalBreakSeconds);
			logicClientHome.GetCompressibleHomeJSON().Set(homeJson);
			logicClientHome.GetCompressibleGlobalJSON().Set(ResourceManager.SERVER_SAVE_FILE_GLOBAL);
			logicClientHome.GetCompressibleCalendarJSON().Set(ResourceManager.SERVER_SAVE_FILE_CALENDAR);

			return logicClientHome;
		}
	}
}