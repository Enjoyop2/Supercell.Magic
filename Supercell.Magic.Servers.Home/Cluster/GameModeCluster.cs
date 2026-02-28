using System;
using System.Diagnostics;

using Supercell.Magic.Logic.Message;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Cluster;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Core;
using Supercell.Magic.Servers.Core.Network.Message.Session;

using Supercell.Magic.Servers.Home.Logic.Mode;
using Supercell.Magic.Servers.Home.Session;

using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Servers.Home.Cluster
{
	public class GameModeCluster : ClusterInstance
	{
		private readonly HomeSessionManager m_sessionManager;
		private readonly Stopwatch m_watch;

		private long m_messageProcessSpeed;
		private int m_messageProcessCount;

		public GameModeCluster(int id, int logicUpdateFrequency = -1) : base(id, logicUpdateFrequency)
		{
			m_sessionManager = new HomeSessionManager();
			m_watch = new Stopwatch();
		}

		protected override void ReceiveMessage(ServerMessage message)
		{
			m_watch.Restart();

			switch (message.GetMessageType())
			{
				case ServerMessageType.START_SERVER_SESSION:
					OnStartServerSessionMessageReceived((StartServerSessionMessage)message);
					break;
				case ServerMessageType.STOP_SERVER_SESSION:
					OnStopServerSessionMessageReceived((StopServerSessionMessage)message);
					break;
				case ServerMessageType.UPDATE_SOCKET_SERVER_SESSION:
					OnUpdateSocketServerSessionMessageReceived((UpdateSocketServerSessionMessage)message);
					break;
				case ServerMessageType.FORWARD_LOGIC_MESSAGE:
					OnForwardLogicMessageReceived((ForwardLogicMessage)message);
					break;

				case ServerMessageType.HOME_SERVER_COMMAND_ALLOWED:
					OnHomeAllowServerCommandMessageReceived((HomeServerCommandAllowedMessage)message);
					break;
				case ServerMessageType.GAME_STATE_DATA:
					OnLoadGameStateDataMessageReceived((GameStateDataMessage)message);
					break;
				case ServerMessageType.GAME_STATE_NULL_DATA:
					OnGameStateNullDataMessageReceived((GameStateNullDataMessage)message);
					break;
			}

			m_watch.Stop();

			m_messageProcessSpeed += m_watch.ElapsedMilliseconds;
			m_messageProcessCount += 1;

			if (m_messageProcessSpeed > 1000)
			{
				m_messageProcessSpeed = m_messageProcessSpeed / 1000L;
				m_messageProcessCount = 1;
			}
		}

		protected override void OnPingTestCompleted()
		{
			for (int i = 0; i < ServerManager.GetServerCount(0); i++)
			{
				ServerMessageManager.SendMessage(new ClusterPerformanceDataMessage
				{
					Id = m_id,
					SessionCount = m_sessionManager.Count,
					Ping = m_ping
				}, 0, i);
			}
		}

		public long GetAverageMessageProcessSpeed()
		{
			if (m_messageProcessCount != 0)
				return m_messageProcessSpeed / m_messageProcessCount;
			return 0L;
		}

		private void OnStartServerSessionMessageReceived(StartServerSessionMessage message)
		{
			m_sessionManager.OnStartServerSessionMessageReceived(message);
		}

		private void OnStopServerSessionMessageReceived(StopServerSessionMessage message)
		{
			m_sessionManager.OnStopServerSessionMessageReceived(message);
		}

		private void OnUpdateSocketServerSessionMessageReceived(UpdateSocketServerSessionMessage message)
		{
			if (m_sessionManager.TryGet(message.SessionId, out HomeSession session))
			{
				session.UpdateSocketServerSessionMessageReceived(message);
			}
		}

		private void OnForwardLogicMessageReceived(ForwardLogicMessage message)
		{
			if (m_sessionManager.TryGet(message.SessionId, out HomeSession session))
			{
				PiranhaMessage logicMessage = LogicMagicMessageFactory.Instance.CreateMessageByType(message.MessageType);

				if (logicMessage == null)
					throw new Exception("logicMessage should not be NULL!");

				logicMessage.GetByteStream().SetByteArray(message.MessageBytes, message.MessageLength);
				logicMessage.SetMessageVersion(message.MessageVersion);
				logicMessage.Decode();

				if (!logicMessage.IsServerToClientMessage())
				{
					session.LogicMessageManager.ReceiveMessage(logicMessage);
				}
			}
		}

		private void OnHomeAllowServerCommandMessageReceived(HomeServerCommandAllowedMessage message)
		{
			if (m_sessionManager.TryGet(message.SessionId, out HomeSession session))
			{
				if (session.GameMode != null && session.GameMode.GetLogicGameMode().GetState() == 1)
					session.GameMode.AddServerCommand(message.ServerCommand);
			}
		}

		private void OnLoadGameStateDataMessageReceived(GameStateDataMessage message)
		{
			if (m_sessionManager.TryGet(message.SessionId, out HomeSession session))
			{
				switch (message.State.GetGameStateType())
				{
					case GameStateType.HOME:
						session.SetGameMode(GameMode.LoadHomeState(session, (GameHomeState)message.State));
						break;
					case GameStateType.NPC_ATTACK:
						session.SetGameMode(GameMode.LoadNpcAttackState(session, (GameNpcAttackState)message.State));
						break;
					case GameStateType.NPC_DUEL:
						session.SetGameMode(GameMode.LoadNpcDuelState(session, (GameNpcDuelState)message.State));
						break;
					case GameStateType.MATCHED_ATTACK:
						session.SetGameMode(GameMode.LoadMatchedAttackState(session, (GameMatchedAttackState)message.State));
						break;
					case GameStateType.VISIT:
						session.SetGameMode(GameMode.LoadVisitAttackState(session, (GameVisitState)message.State));
						break;
					default:
						Logging.Error("GameModeCluster.onLoadGameStateDataMessageReceived: unknown game state: " + message.State.GetGameStateType());
						break;
				}
			}
		}

		private void OnGameStateNullDataMessageReceived(GameStateNullDataMessage message)
		{
			if (m_sessionManager.TryGet(message.SessionId, out HomeSession session))
			{
				session.DestructGameMode();
			}
		}
	}
}