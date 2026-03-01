using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Core.Util;

namespace Supercell.Magic.Servers.Admin.Logic.Entry
{
	public class LogServerEntry
	{
		public LogType Type
		{
			get;
		}
		public string Message
		{
			get;
		}
		public int ServerType
		{
			get;
		}
		public int ServerId
		{
			get;
		}
		public int Time
		{
			get;
		}

		public LogServerEntry(LogType type, string message, int serverType, int serverId)
		{
			Type = type;
			Message = message;
			ServerType = serverType;
			ServerId = serverId;
			Time = TimeUtil.GetTimestamp();
		}

		public JObject Save()
			=> new JObject
			{
				{ "type", (int) Type },
				{ "msg", Message },
				{ "sT", ServerType },
				{ "sI", ServerId },
				{ "t", Time }
			};
	}
}