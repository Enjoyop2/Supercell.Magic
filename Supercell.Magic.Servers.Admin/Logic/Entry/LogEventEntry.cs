using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Core.Util;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Admin.Logic.Entry
{
	public class LogEventEntry
	{
		public EventType Type
		{
			get;
		}
		public LogicLong AccountId
		{
			get;
		}
		public Dictionary<string, object> Args
		{
			get;
		}
		public int Time
		{
			get;
		}

		public LogEventEntry(EventType type, LogicLong accountId, Dictionary<string, object> args)
		{
			Type = type;
			AccountId = accountId;
			Args = args;
			Time = TimeUtil.GetTimestamp();
		}

		public JObject Save()
		{
			JObject jObject = new JObject();

			jObject.Add("type", (int)Type);
			jObject.Add("accId", (long)AccountId);

			JArray args = new JArray();

			foreach (KeyValuePair<string, object> arg in Args)
			{
				args.Add(new JArray
				{
					arg.Key,
					arg.Value
				});
			}

			jObject.Add("args", args);
			jObject.Add("t", Time);

			return jObject;
		}

		public enum EventType
		{
			OUT_OF_SYNC
		}
	}
}