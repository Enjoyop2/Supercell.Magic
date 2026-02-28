using System;

using Supercell.Magic.Logic.Message.Avatar;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Core.Session;

using Supercell.Magic.Servers.Game.Logic;
using Supercell.Magic.Servers.Game.Session.Message;

namespace Supercell.Magic.Servers.Game.Session
{
	public class GameSession : ServerSession
	{
		public LogicMessageManager LogicMessageManager
		{
			get;
		}
		public GameAvatar GameAvatar
		{
			get;
		}
		public GameState GameState
		{
			get; private set;
		}

		public DateTime LastDbSave
		{
			get; set;
		}

		// State Vars
		public bool InMatchmaking
		{
			get; set;
		}
		public bool InDuelMatchmaking
		{
			get; set;
		}
		public long SpectateLiveReplayId { get; set; } = -1;
		public int SpectateLiveReplaySlotId
		{
			get; set;
		}

		public GameFakeAttackState FakeAttackState
		{
			get; set;
		}

		public GameSession(StartServerSessionMessage message) : base(message)
		{
			LogicMessageManager = new LogicMessageManager(this);
			GameAvatar = GameAvatarManager.TryGet(AccountId, out GameAvatar document) ? document : GameAvatarManager.Create(AccountId);

			if (GameAvatar.CurrentSession != null)
				GameSessionManager.Remove(GameAvatar.CurrentSession.Id);
			GameAvatar.CurrentSession = this;

			GameMatchmakingManager.Dequeue(GameAvatar);
			ServerRequestManager.Create(new BindServerSocketRequestMessage
			{
				SessionId = Id,
				ServerType = 10,
				ServerId = -1
			}, m_sockets[1], 10).OnComplete = OnHomeServerBound;
		}

		private void OnHomeServerBound(ServerRequestArgs args)
		{
			if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
			{
				if (GameAvatar.LogicClientAvatar.IsInAlliance())
					BindAllianceServer();
				SendBookmarksListMessageToClient();
				SendAvatarStreamMessageToClient();

				LoadGameState(new GameHomeState
				{
					Home = GameAvatar.LogicClientHome,
					PlayerAvatar = GameAvatar.LogicClientAvatar,
					SaveTime = GameAvatar.SaveTime,
					MapId = GameAvatar.MaintenanceTime,
					ServerCommands = GameAvatar.ServerCommands
				});
			}
			else
			{
				Logging.Error("GameSession.onHomeServerBound: unable to bind a home server to the session.");
				SendMessage(new StopSessionMessage(), 1);
			}
		}

		public override void Destruct()
		{
			GameAvatarManager.ExecuteServerCommandsInOfflineMode(GameAvatar);
			GameAvatarManager.Save(GameAvatar);
			GameMatchmakingManager.Enqueue(GameAvatar);

			GameAvatar.CurrentSession = null;

			DestructGameState();
			base.Destruct();
		}

		public void BindAllianceServer()
		{
			ServerSocket socket = ServerManager.GetDocumentSocket(11, GameAvatar.LogicClientAvatar.GetAllianceId());

			if (socket != null)
			{
				ServerRequestManager.Create(new BindServerSocketRequestMessage
				{
					ServerType = socket.ServerType,
					ServerId = socket.ServerId,
					SessionId = Id
				}, m_sockets[1]);
			}
		}

		private void SendBookmarksListMessageToClient()
		{
			BookmarksListMessage bookmarksListMessage = new BookmarksListMessage();
			bookmarksListMessage.SetAllianceIds(GameAvatar.AllianceBookmarksList);
			SendPiranhaMessage(bookmarksListMessage, 1);
		}

		private void SendAvatarStreamMessageToClient()
		{
			ServerMessageManager.SendMessage(new SendAvatarStreamsToClientMessage
			{
				StreamIds = GameAvatar.AvatarStreamList,
				SessionId = Id
			}, ServerManager.GetDocumentSocket(11, GameAvatar.Id));
		}

		public void DestructGameState()
		{
			if (GameState != null)
			{
				if (GameState.GetSimulationServiceNodeType() == SimulationServiceNodeType.BATTLE)
				{
					SendMessage(new StopSpecifiedServerSessionMessage
					{
						ServerType = 27,
						ServerId = m_sockets[27].ServerId
					}, 1);
				}
				else
				{
					SendMessage(new GameStateNullDataMessage(), 10);
				}

				GameState = null;
			}

			if (InMatchmaking)
				GameMatchmakingManager.Dequeue(this);
			if (InDuelMatchmaking)
				GameDuelMatchmakingManager.Dequeue(this);

			if (SpectateLiveReplayId != -1)
			{
				ServerMessageManager.SendMessage(new LiveReplayRemoveSpectatorMessage
				{
					AccountId = SpectateLiveReplayId,
					SlotId = SpectateLiveReplaySlotId,
					SessionId = Id
				}, 9);

				SpectateLiveReplayId = -1;
				SpectateLiveReplaySlotId = 0;
			}
		}

		public void LoadGameState(GameState state)
		{
			if (GameState != null)
				throw new Exception("GameSession.loadGameState: current game state should be NULL");

			state.Home.GetCompressibleGlobalJSON().Set(ResourceManager.SERVER_SAVE_FILE_GLOBAL);
			state.Home.GetCompressibleCalendarJSON().Set(ResourceManager.SERVER_SAVE_FILE_CALENDAR);

			GameState = state;

			if (state.GetSimulationServiceNodeType() == SimulationServiceNodeType.BATTLE)
			{
				ServerRequestManager.Create(new BindServerSocketRequestMessage
				{
					SessionId = Id,
					ServerType = 27,
					ServerId = -1
				}, m_sockets[1], 10).OnComplete = args =>
				{
					if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
					{
						BindServerSocketResponseMessage responseMessage = (BindServerSocketResponseMessage)args.ResponseMessage;

						m_sockets[responseMessage.ServerType] = ServerManager.GetSocket(responseMessage.ServerType, responseMessage.ServerId);
						SendMessage(new GameStateDataMessage
						{
							State = state
						}, responseMessage.ServerType);
					}
					else
					{
						Logging.Error("GameSession.loadGameState: unable to bind a battle server to the session.");
						SendMessage(new StopSessionMessage(), 1);
					}
				};
			}
			else
			{
				SendMessage(new GameStateDataMessage
				{
					State = state
				}, 10);
			}
		}
	}
}