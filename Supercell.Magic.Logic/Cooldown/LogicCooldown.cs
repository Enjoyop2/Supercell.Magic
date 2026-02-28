using Supercell.Magic.Logic.Time;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Cooldown
{
	public class LogicCooldown
	{
		private int m_targetGlobalId;
		private int m_cooldownTime;

		public LogicCooldown()
		{
			// LogicCooldown.
		}

		public LogicCooldown(int targetGlobalId, int cooldownSecs)
		{
			m_targetGlobalId = targetGlobalId;
			m_cooldownTime = LogicTime.GetCooldownSecondsInTicks(cooldownSecs);
		}

		public void Tick()
		{
			if (m_cooldownTime > 0)
			{
				--m_cooldownTime;
			}
		}

		public void FastForwardTime(int secs)
		{
			m_cooldownTime = LogicMath.Max(m_cooldownTime - LogicTime.GetCooldownSecondsInTicks(secs), 0);
		}

		public void Load(LogicJSONObject jsonObject)
		{
			LogicJSONNumber cooldownNumber = jsonObject.GetJSONNumber("cooldown");
			LogicJSONNumber targetNumber = jsonObject.GetJSONNumber("target");

			if (cooldownNumber == null)
			{
				Debugger.Error("LogicCooldown::load - Cooldown was not found!");
				return;
			}

			if (targetNumber == null)
			{
				Debugger.Error("LogicCooldown::load - Target was not found!");
				return;
			}

			m_cooldownTime = cooldownNumber.GetIntValue();
			m_targetGlobalId = targetNumber.GetIntValue();
		}

		public void Save(LogicJSONObject jsonObject)
		{
			jsonObject.Put("cooldown", new LogicJSONNumber(m_cooldownTime));
			jsonObject.Put("target", new LogicJSONNumber(m_targetGlobalId));
		}

		public int GetCooldownSeconds()
			=> LogicTime.GetCooldownTicksInSeconds(m_cooldownTime);

		public int GetTargetGlobalId()
			=> m_targetGlobalId;
	}
}