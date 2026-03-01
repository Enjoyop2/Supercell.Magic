using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Admin.Controllers;
using Supercell.Magic.Servers.Admin.Logic.Entry;
using Supercell.Magic.Servers.Core.Network.Message.Core;
using Supercell.Magic.Servers.Core.Util;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Admin.Logic
{
	public static class LogManager
	{
		private static LogicArrayList<LogServerEntry> m_serverLogs;
		private static LogicArrayList<LogGameEntry> m_gameLogs;
		private static LogicArrayList<LogEventEntry> m_eventLogs;

		public static void Init()
		{
			LogManager.m_serverLogs = new LogicArrayList<LogServerEntry>();
			LogManager.m_gameLogs = new LogicArrayList<LogGameEntry>();
			LogManager.m_eventLogs = new LogicArrayList<LogEventEntry>();
		}

		public static void OnServerLogMessage(ServerLogMessage message)
		{
			LogManager.m_serverLogs.Add(new LogServerEntry((LogType)message.LogType, message.Message, message.SenderType, message.SenderId));
		}

		public static void OnGameLogMessage(GameLogMessage message)
		{
			LogManager.m_gameLogs.Add(new LogGameEntry((LogType)message.LogType, message.Message));
		}

		public static void AddEventLog(LogEventEntry.EventType type, LogicLong accountId, Dictionary<string, object> args)
		{
			LogManager.m_eventLogs.Add(new LogEventEntry(type, accountId, args));
		}

		public static JObject Save()
		{
			JObject jObject = new JObject();
			JArray serverArray = new JArray();
			JArray gameArray = new JArray();
			JArray eventArray = new JArray();

			for (int i = 0; i < LogManager.m_serverLogs.Size(); i++)
			{
				serverArray.Add(LogManager.m_serverLogs[i].Save());
			}

			for (int i = 0; i < LogManager.m_gameLogs.Size(); i++)
			{
				gameArray.Add(LogManager.m_gameLogs[i].Save());
			}

			for (int i = 0; i < LogManager.m_eventLogs.Size(); i++)
			{
				eventArray.Add(LogManager.m_eventLogs[i].Save());
			}

			jObject.Add("server", serverArray);
			jObject.Add("game", gameArray);
			jObject.Add("event", eventArray);

			return jObject;
		}
	}
}