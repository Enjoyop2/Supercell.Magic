using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Supercell.Magic.Servers.Admin.Attributes;
using Supercell.Magic.Servers.Admin.Logic.Entry;
using Supercell.Magic.Servers.Core;

namespace Supercell.Magic.Servers.Admin.Logic
{
	public static class AuthManager
	{
		private static ConcurrentDictionary<string, SessionEntry> m_sessions;
		private static Thread m_sessionUpdateThread;
		private static List<UserEntry> m_offlineUsers;

		public static void Init()
		{
			m_offlineUsers = new List<UserEntry>();
			m_offlineUsers.Add(new UserEntry("Mike8169", "babana_selamlar777", UserRole.ADMIN));
			m_offlineUsers.Add(new UserEntry("HORIZON", "anana_selamlar888", UserRole.ADMIN));

			m_sessions = new ConcurrentDictionary<string, SessionEntry>();
			m_sessionUpdateThread = new Thread(Update);
			m_sessionUpdateThread.Start();
		}

		private static void Update()
		{
			while (true)
			{
				DateTime utc = DateTime.UtcNow;

				foreach (SessionEntry entry in m_sessions.Values)
				{
					if (utc.Subtract(entry.UpdateTime).TotalMinutes >= 30d || utc.Subtract(entry.CreateTime).TotalDays >= 1d)
						CloseSession(entry.Token);
				}

				Thread.Sleep(10000);
			}
		}

		public static bool OpenSession(string user, string password, out string token)
		{
			if (LoadUser(user, password, out UserEntry userEntry))
			{
				if (userEntry.CurrentSession != null)
					CloseSession(userEntry.CurrentSession.Token);
				SessionEntry session = new SessionEntry(userEntry, token = GenerateToken());

				bool success = m_sessions.TryAdd(session.Token, session);
				if (success)
					userEntry.CurrentSession = session;
				Logging.Print(string.Format("openSession: user: {0} password: {1} token: {2}", user, password, session.Token));
				return success;
			}

			token = null;
			return false;
		}

		public static bool CloseSession(string token)
		{
			Logging.Print(string.Format("closeSession: token: {0}", token));
			return m_sessions.Remove(token, out _);
		}

		public static bool IsOpenSession(string token)
			=> TryGetSession(token, out _);

		public static bool TryGetSession(string token, out SessionEntry entry)
		{
			bool success = m_sessions.TryGetValue(token, out entry);
			if (success)
				entry.UpdateTime = DateTime.UtcNow;
			return success;
		}

		private static bool LoadUser(string user, string password, out UserEntry entry)
		{
			for (int i = 0; i < m_offlineUsers.Count; i++)
			{
				entry = m_offlineUsers[i];

				if (entry.User == user && entry.Password == password)
					return true;
			}

			entry = null;
			return false;
		}

		private static string GenerateToken()
			=> Guid.NewGuid().ToString("N");
	}

	public enum UserRole
	{
		NULL,
		DEFAULT,
		TESTER,
		MODERATOR,
		ADMIN
	}
}