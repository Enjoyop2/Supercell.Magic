using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Time
{
	public class LogicTimer
	{
		private int m_remainingTime;
		private int m_endTimestamp;
		private int m_fastForward;

		public LogicTimer()
		{
			m_endTimestamp = -1;
		}

		public void Destruct()
		{
			m_remainingTime = 0;
			m_endTimestamp = -1;
			m_fastForward = 0;
		}

		public int GetRemainingSeconds(LogicTime time)
		{
			int remaining = m_remainingTime - time.GetTick() - m_fastForward;

			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				if (remaining > 0)
				{
					return LogicMath.Max((int)(16L * remaining / 1000 + 1), 1);
				}
			}
			else
			{
				if (remaining > 0)
				{
					return LogicMath.Max((remaining + 59) / 60, 1);
				}
			}

			return 0;
		}

		public int GetRemainingMS(LogicTime time)
		{
			int remaining = m_remainingTime - time.GetTick() - m_fastForward;

			if (LogicDataTables.GetGlobals().MoreAccurateTime())
			{
				return 16 * remaining;
			}

			int ms = 1000 * (remaining / 60);

			if (ms % 60 != 0)
			{
				ms += (2133 * ms) >> 7;
			}

			return ms;
		}

		public void StartTimer(int totalSecs, LogicTime time, bool setEndTimestamp, int currentTimestamp)
		{
			m_remainingTime = LogicTime.GetSecondsInTicks(totalSecs) + time.GetTick();

			if (currentTimestamp != -1 && setEndTimestamp)
			{
				m_endTimestamp = currentTimestamp + totalSecs;
			}
		}

		public void FastForward(int totalSecs)
		{
			m_remainingTime -= LogicTime.GetSecondsInTicks(totalSecs);
		}

		public void FastForwardSubticks(int tick)
		{
			if (tick > 0)
			{
				m_remainingTime -= tick;
			}
		}

		public void AdjustEndSubtick(LogicLevel level)
		{
			if (m_endTimestamp != -1)
			{
				int currentTime = LogicDataTables.GetGlobals().AdjustEndSubtickUseCurrentTime()
					? level.GetGameMode().GetServerTimeInSecondsSince1970()
					: level.GetGameMode().GetStartTime();

				if (currentTime != -1)
				{
					int remainingSecs = m_endTimestamp - currentTime;
					int clamp = LogicDataTables.GetGlobals().GetClampLongTimeStampsToDays();

					if (clamp * 86400 > 0)
					{
						if (remainingSecs > 86400 * clamp)
						{
							remainingSecs = 86400 * clamp;
						}
						else if (remainingSecs < -86400 * clamp)
						{
							remainingSecs = -86400 * clamp;
						}
					}

					m_remainingTime = level.GetLogicTime().GetTick() + LogicTime.GetSecondsInTicks(remainingSecs);
				}
			}
		}

		public int GetEndTimestamp()
			=> m_endTimestamp;

		public void SetEndTimestamp(int endTimestamp)
		{
			m_endTimestamp = endTimestamp;
		}

		public int GetFastForward()
			=> m_fastForward;

		public void SetFastForward(int value)
		{
			m_fastForward = value;
		}

		public static void SetLogicTimer(LogicJSONObject jsonObject, LogicTimer timer, LogicLevel level, string key)
		{
			if (timer != null)
			{
				int remainingSeconds = timer.GetRemainingSeconds(level.GetLogicTime());

				if (remainingSeconds > 0)
				{
					jsonObject.Put(key, new LogicJSONNumber(remainingSeconds));
				}
			}
		}

		public static LogicTimer GetLogicTimer(LogicJSONObject jsonObject, LogicTime time, string key, int maxTime)
		{
			LogicJSONNumber number = (LogicJSONNumber)jsonObject.Get(key);

			if (number != null)
			{
				LogicTimer timer = new LogicTimer();

				int remainingSeconds = LogicMath.Min(number.GetIntValue(), maxTime);
				int tick = time.GetTick();

				timer.m_remainingTime = tick + LogicTime.GetSecondsInTicks(remainingSeconds);

				return timer;
			}

			return null;
		}
	}
}