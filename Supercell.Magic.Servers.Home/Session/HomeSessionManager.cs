using System;
using System.Collections.Generic;

using Supercell.Magic.Servers.Core.Network.Message.Session;

namespace Supercell.Magic.Servers.Home.Session
{
	public class HomeSessionManager
	{
		private readonly Dictionary<long, HomeSession> m_sessions;

		public int Count
		{
			get
			{
				return m_sessions.Count;
			}
		}

		public HomeSessionManager()
		{
			m_sessions = new Dictionary<long, HomeSession>();
		}

		public void OnStartServerSessionMessageReceived(StartServerSessionMessage message)
		{
			if (m_sessions.ContainsKey(message.SessionId))
				throw new Exception("HomeSessionManager.onStartSessionMessageReceived: session already started!");
			m_sessions.Add(message.SessionId, new HomeSession(message));
		}

		public void OnStopServerSessionMessageReceived(StopServerSessionMessage message)
		{
			if (m_sessions.Remove(message.SessionId, out HomeSession session))
				session.Destruct();
		}

		public bool TryGet(long id, out HomeSession session)
			=> m_sessions.TryGetValue(id, out session);
	}
}