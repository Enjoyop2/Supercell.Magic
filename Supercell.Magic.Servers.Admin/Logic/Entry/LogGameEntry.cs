using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Core.Util;

namespace Supercell.Magic.Servers.Admin.Logic.Entry
{
	public class LogGameEntry
	{
		public LogType Type
		{
			get;
		}
		public string Message
		{
			get;
		}
		public int Time
		{
			get;
		}

		public LogGameEntry(LogType type, string message)
		{
			Type = type;
			Message = message;
			Time = TimeUtil.GetTimestamp();
		}

		public JObject Save()
			=> new JObject
			{
				{ "type", (int) Type },
				{ "msg", Message },
				{ "t", Time }
			};
	}
}