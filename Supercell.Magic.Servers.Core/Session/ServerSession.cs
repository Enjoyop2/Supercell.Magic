using System;

using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Core.Settings;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Session
{
	public class ServerSession
	{
		public long Id
		{
			get;
		}
		public string Country
		{
			get;
		}
		public LogicLong AccountId
		{
			get;
		}

		protected readonly ServerSocket[] m_sockets;
		protected bool m_destructed;

		public ServerSession(long sessionId, LogicLong accountId, string country)
		{
			Id = sessionId;
			AccountId = accountId;
			Country = country;
			m_sockets = new ServerSocket[EnvironmentSettings.SERVER_TYPE_COUNT];
		}

		public ServerSession(StartServerSessionMessage message) : this(message.SessionId, message.AccountId, message.Country)
		{
			LogicArrayList<int> serverSocketTypeList = message.ServerSocketTypeList;
			LogicArrayList<int> serverSocketIdList = message.ServerSocketIdList;

			for (int i = 0; i < serverSocketTypeList.Size(); i++)
			{
				int type = serverSocketTypeList[i];
				int id = serverSocketIdList[i];

				m_sockets[type] = ServerManager.GetSocket(type, id);
			}

			if (message.BindRequestMessage != null)
			{
				ServerRequestManager.SendResponse(new BindServerSocketResponseMessage
				{
					ServerType = ServerCore.Type,
					ServerId = ServerCore.Id,
					Success = true
				}, message.BindRequestMessage);
			}
		}

		public virtual void Destruct()
		{
			if (!m_destructed)
			{
				Array.Clear(m_sockets, 0, EnvironmentSettings.SERVER_TYPE_COUNT);
				m_destructed = true;
			}
		}

		public bool IsDestructed()
			=> m_destructed;

		public virtual void UpdateSocketServerSessionMessageReceived(UpdateSocketServerSessionMessage message)
		{
			m_sockets[message.ServerType] = message.ServerId != -1 ? ServerManager.GetSocket(message.ServerType, message.ServerId) : null;
		}

		public void SendMessage(ServerSessionMessage message, int serverType)
		{
			message.SessionId = Id;

			if (m_sockets[serverType] != null)
			{
				ServerMessageManager.SendMessage(message, m_sockets[serverType]);
			}
		}

		public void SendPiranhaMessage(PiranhaMessage message, int serverType)
		{
			if (message.GetByteStream().GetLength() == 0)
				message.Encode();
			SendMessage(new ForwardLogicMessage
			{
				MessageType = message.GetMessageType(),
				MessageVersion = (short)message.GetMessageVersion(),
				MessageLength = message.GetEncodingLength(),
				MessageBytes = message.GetByteStream().GetByteArray()
			}, serverType);
		}

		public ServerSocket GetSocket(int type)
			=> m_sockets[type];

		public bool IsBoundToServerType(int type)
			=> m_sockets[type] != null;
	}
}