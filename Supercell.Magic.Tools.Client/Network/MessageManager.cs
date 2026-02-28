using System;

using Supercell.Magic.Logic.Message.Account;
using Supercell.Magic.Logic.Message.Battle;
using Supercell.Magic.Logic.Message.Home;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;
using Supercell.Magic.Tools.Client.Helper;

namespace Supercell.Magic.Tools.Client.Network
{
	public class MessageManager
	{
		private readonly ServerConnection m_serverConnection;
		private readonly Messaging m_messaging;

		private float m_nextKeepAlive;
		private float m_nextClientTurn;

		private DateTime m_sendKeepAliveTime;

		public MessageManager(ServerConnection serverConnection, Messaging messaging)
		{
			m_serverConnection = serverConnection;
			m_messaging = messaging;
		}

		public void ReceiveMessage(PiranhaMessage piranhaMessage)
		{
			switch (piranhaMessage.GetMessageType())
			{
				case LoginFailedMessage.MESSAGE_TYPE:
					OnLoginFailedMessageReceived((LoginFailedMessage)piranhaMessage);
					break;
				case LoginOkMessage.MESSAGE_TYPE:
					OnLoginOkMessageReceived((LoginOkMessage)piranhaMessage);
					break;
				case KeepAliveServerMessage.MESSAGE_TYPE:
					OnKeepAliveServerMessageReceived((KeepAliveServerMessage)piranhaMessage);
					break;
			}
		}

		private void OnLoginFailedMessageReceived(LoginFailedMessage message)
		{
			m_serverConnection.SetState(ServerConnectionState.LOGIN_FAILED);

			switch (message.GetErrorCode())
			{
				case LoginFailedMessage.ErrorCode.DATA_VERSION:
					ZLibHelper.DecompressInMySQLFormat(message.GetCompressedFingerprint(), out byte[] output);
					ResourceManager.DownloadDataUpdate(LogicStringUtil.CreateString(output, 0, output.Length), message.GetContentUrl());
					break;
				default:
					Debugger.Warning("MessageManager.onLoginFailedMessageReceived: error code: " + message.GetErrorCode());
					break;
			}
		}

		private void OnLoginOkMessageReceived(LoginOkMessage message)
		{
			m_serverConnection.SetState(ServerConnectionState.LOGGED);
			m_serverConnection.SetAccountInfo(message.GetAccountId(), message.GetPassToken());

			Debugger.Print(string.Format("MessageManager.onLoginOkMessageReceived: client logged (account: {0} passtoken: {1} server version: {2}", message.GetAccountId(), message.GetPassToken(),
										 message.GetServerMajorVersion() + "." + message.GetServerBuildVersion() + "." + message.GetContentVersion()));
		}

		private void OnKeepAliveServerMessageReceived(KeepAliveServerMessage message)
		{
			if (m_sendKeepAliveTime != DateTime.MinValue)
			{
				int ping = (int)DateTime.UtcNow.Subtract(m_sendKeepAliveTime).TotalMilliseconds;
				m_sendKeepAliveTime = DateTime.MinValue;

				Debugger.Print("MessageManager.onKeepAliveServerMessageReceived: ping: " + ping + "ms");
			}
		}

		public void SendMessage(PiranhaMessage message)
		{
			if (message.GetMessageType() == EndClientTurnMessage.MESSAGE_TYPE || message.GetMessageType() == BattleEndClientTurnMessage.MESSAGE_TYPE)
			{
				m_nextClientTurn = 5f;
			}
			else if (message.GetMessageType() == KeepAliveMessage.MESSAGE_TYPE)
			{
				m_nextKeepAlive = 0.1f;

				if (m_sendKeepAliveTime == DateTime.MinValue)
					m_sendKeepAliveTime = DateTime.UtcNow;
			}

			m_messaging.Send(message);
		}

		public void Update(float time)
		{
			m_nextClientTurn -= time;
			m_nextKeepAlive -= time;

			if (m_nextKeepAlive <= 0f)
				SendMessage(new KeepAliveMessage());
		}
	}
}