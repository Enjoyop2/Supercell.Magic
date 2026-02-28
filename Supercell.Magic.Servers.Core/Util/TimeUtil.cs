using System;

namespace Supercell.Magic.Servers.Core.Util
{
	public static class TimeUtil
	{
		private static readonly DateTime m_unix = new DateTime(1970, 1, 1);

		public static int GetTimestamp()
			=> (int)DateTime.UtcNow.Subtract(TimeUtil.m_unix).TotalSeconds;

		public static int GetTimestamp(DateTime utc)
			=> (int)utc.Subtract(TimeUtil.m_unix).TotalSeconds;

		public static long GetTimestampMS()
			=> (int)DateTime.UtcNow.Subtract(TimeUtil.m_unix).TotalMilliseconds;

		public static long GetTimestampMS(DateTime utc)
			=> (int)utc.Subtract(TimeUtil.m_unix).TotalMilliseconds;

		public static DateTime GetDateTimeFromTimestamp(int timestamp)
			=> TimeUtil.m_unix.AddSeconds(timestamp);
	}
}