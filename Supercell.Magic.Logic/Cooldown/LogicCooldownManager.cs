using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Cooldown
{
	public class LogicCooldownManager
	{
		private readonly LogicArrayList<LogicCooldown> m_cooldowns;

		public LogicCooldownManager()
		{
			m_cooldowns = new LogicArrayList<LogicCooldown>();
		}

		public void Destruct()
		{
			DeleteCooldowns();
		}

		public void DeleteCooldowns()
		{
			m_cooldowns.Destruct();
		}

		public void Tick()
		{
			for (int i = 0; i < m_cooldowns.Size(); i++)
			{
				m_cooldowns[i].Tick();

				if (m_cooldowns[i].GetCooldownSeconds() <= 0)
				{
					m_cooldowns.Remove(i);
				}
			}
		}

		public void FastForwardTime(int secs)
		{
			for (int i = 0; i < m_cooldowns.Size(); i++)
			{
				m_cooldowns[i].FastForwardTime(secs);
			}
		}

		public void Load(LogicJSONObject jsonObject)
		{
			LogicJSONArray cooldownArray = jsonObject.GetJSONArray("cooldowns");

			if (cooldownArray != null)
			{
				int size = cooldownArray.Size();

				for (int i = 0; i < size; i++)
				{
					LogicCooldown cooldown = new LogicCooldown();
					cooldown.Load(cooldownArray.GetJSONObject(i));
					m_cooldowns.Add(cooldown);
				}
			}
		}

		public void Save(LogicJSONObject jsonObject)
		{
			LogicJSONArray cooldownArray = new LogicJSONArray();

			for (int i = 0; i < m_cooldowns.Size(); i++)
			{
				LogicJSONObject cooldownObject = new LogicJSONObject();
				m_cooldowns[i].Save(cooldownObject);
				cooldownArray.Add(cooldownObject);
			}

			jsonObject.Put("cooldowns", cooldownArray);
		}

		public void AddCooldown(int targetGlobalId, int cooldownSecs)
		{
			m_cooldowns.Add(new LogicCooldown(targetGlobalId, cooldownSecs));
		}

		public int GetCooldownSeconds(int targetGlobalId)
		{
			for (int i = 0; i < m_cooldowns.Size(); i++)
			{
				if (m_cooldowns[i].GetTargetGlobalId() == targetGlobalId)
				{
					return m_cooldowns[i].GetCooldownSeconds();
				}
			}

			return 0;
		}
	}
}