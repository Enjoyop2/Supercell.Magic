using Supercell.Magic.Logic.Data;

namespace Supercell.Magic.Logic.Time
{
	public class LogicTime
	{
		private int m_tick;
		private int m_fullTick;

		public void IncreaseTick()
		{
			++m_tick;

			if ((m_tick & 3) == 0)
			{
				++m_fullTick;
			}
		}

		public bool IsFullTick()
			=> ((m_tick + 1) & 3) == 0;

		public int GetTick()
			=> m_tick;

		public int GetFullTick()
			=> m_fullTick;

		public int GetTotalMS()
			=> m_fullTick << 6;

		public static int GetMSInTicks(int time)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return time / 16;
			}

			return time * 60 / 1000;
		}

		public static int GetSecondsInTicks(int time)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return (int)(1000L * time / 16L);
			}

			return time * 60;
		}

		public static int GetTicksInSeconds(int tick)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return (int)(16L * tick / 1000);
			}

			return tick / 60;
		}

		public static int GetTicksInMS(int tick)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return (int)(16L * tick);
			}

			int ms = 1000 * (tick / 60);
			int mod = tick % 60;

			if (mod > 0)
			{
				ms += (2133 * mod) >> 7;
			}

			return ms;
		}

		public static int GetCooldownSecondsInTicks(int time)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return (int)(1000L * time / 64);
			}

			return time * 15;
		}

		public static int GetCooldownTicksInSeconds(int time)
		{
			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return (int)(((long)time << 6) / 1000);
			}

			return time / 15;
		}
	}
}