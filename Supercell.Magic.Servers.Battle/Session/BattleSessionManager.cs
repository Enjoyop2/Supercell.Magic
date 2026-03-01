using System;
using System.Collections.Generic;

using Supercell.Magic.Servers.Core.Network.Message.Session;

namespace Supercell.Magic.Servers.Battle.Session
{
	public class BattleSessionManager
	{
		private readonly Dictionary<long, BattleSession> m_sessions;

		public int GetCount()
			=> m_sessions.Count;

		public BattleSessionManager()
		{
			m_sessions = new Dictionary<long, BattleSession>();
		}

		public void OnStartServerSessionMessageReceived(StartServerSessionMessage message)
		{
			if (m_sessions.ContainsKey(message.SessionId))
			{
				throw new Exception("BattleSessionManager.onStartSessionMessageReceived: session already started!");

			}
			m_sessions.Add(message.SessionId, new BattleSession(message));
		}

		public void OnStopServerSessionMessageReceived(StopServerSessionMessage message)
		{
			if (m_sessions.Remove(message.SessionId, out BattleSession session))
			{
				session.Destruct();
			}
		}

		public bool TryGet(long id, out BattleSession session)
			=> m_sessions.TryGetValue(id, out session);
	}
}