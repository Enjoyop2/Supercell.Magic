using System;

using Couchbase;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Database.Document;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Session;
using Supercell.Magic.Servers.Core.Settings;
using Supercell.Magic.Servers.Core.Util;

using Supercell.Magic.Servers.Proxy.Network;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Proxy.Session
{
	public class ProxySession : ServerSession
	{
		public ClientConnection ClientConnection
		{
			get;
		}

		private int m_chatUnbanTime;
		private bool m_started;

		private readonly DateTime m_startSessionTime;

		public ProxySession(long sessionId, ClientConnection clientConnection, AccountDocument account) : base(sessionId, account.Id, clientConnection.Location)
		{
			ClientConnection = clientConnection;
			m_chatUnbanTime = -1; // TODO: Implement this.
			m_startSessionTime = DateTime.UtcNow;
		}

		public override void Destruct()
		{
			for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				if (m_sockets[i] != null)
					SendMessage(new StopServerSessionMessage(), i);
			}

			ServerProxy.SessionDatabase.DeleteIfEquals(AccountId, Id.ToString());

			UpdatePlayTimeSeconds();
			base.Destruct();
		}

		private async void UpdatePlayTimeSeconds()
		{
			IOperationResult<string> getResult = await ServerProxy.AccountDatabase.Get(AccountId);

			if (getResult.Success)
			{
				AccountDocument accountDocument = CouchbaseDocument.Load<AccountDocument>(getResult.Value);

				accountDocument.SessionCount += 1;
				accountDocument.PlayTimeSeconds += (int)DateTime.UtcNow.Subtract(m_startSessionTime).TotalSeconds;

				IOperationResult<string> updateResult = await ServerProxy.AccountDatabase.Update(AccountId, CouchbaseDocument.Save(accountDocument), getResult.Cas);

				if (!updateResult.Success)
				{
					UpdatePlayTimeSeconds();
				}
			}
		}

		public void SetSocket(ServerSocket socket, BindServerSocketRequestMessage requestMessage = null)
		{
			if (m_destructed)
				return;

			int serverType = socket.ServerType;
			int serverId = socket.ServerId;

			if (m_sockets[serverType] != null)
				SendMessage(new StopServerSessionMessage(), serverType);

			m_sockets[serverType] = socket;

			if (serverType == 1)
				return;
			if (!m_started && socket.ServerType != 9)
				Logging.Warning("ProxySession.setSocket called but session did not start.");

			LogicArrayList<int> serverTypeList = new LogicArrayList<int>();
			LogicArrayList<int> serverIdList = new LogicArrayList<int>();

			for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				ServerSocket serverSocket = m_sockets[i];

				if (serverSocket != null)
				{
					serverTypeList.Add(serverSocket.ServerType);
					serverIdList.Add(serverSocket.ServerId);
				}
			}

			SendMessage(new StartServerSessionMessage
			{
				AccountId = AccountId,
				Country = Country,
				ServerSocketTypeList = serverTypeList,
				ServerSocketIdList = serverIdList,
				BindRequestMessage = requestMessage
			}, serverType);

			for (int i = 2; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				if (i != serverType && m_sockets[serverType] != null)
				{
					SendMessage(new UpdateSocketServerSessionMessage
					{
						ServerType = serverType,
						ServerId = serverId
					}, i);
				}
			}
		}

		public void UnbindServer(int serverType)
		{
			if (m_sockets[serverType] != null)
			{
				SendMessage(new StopServerSessionMessage(), serverType);
				m_sockets[serverType] = null;

				for (int i = 2; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
				{
					if (i != serverType && m_sockets[serverType] != null)
					{
						SendMessage(new UpdateSocketServerSessionMessage
						{
							ServerType = serverType,
							ServerId = -1
						}, i);
					}
				}
			}
		}

		public ServerSocket GetServer(int type)
			=> m_sockets[type];

		public void Update()
		{
			if (m_started)
			{
				if (m_sockets[6] == null)
				{
					if (m_chatUnbanTime == -1 || TimeUtil.GetTimestamp() > m_chatUnbanTime)
					{
						ServerSocket chatSocket = ServerManager.GetNextSocket(6);

						if (chatSocket != null)
						{
							SetSocket(chatSocket);
						}
					}
				}
			}
		}

		public void SetStarted()
		{
			m_started = true;
		}
	}
}