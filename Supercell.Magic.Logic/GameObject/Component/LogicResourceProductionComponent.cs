using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicResourceProductionComponent : LogicComponent
	{
		private readonly LogicResourceData m_resourceData;
		private readonly LogicTimer m_resourceTimer;

		private int m_availableLoot;
		private int m_maxResources;
		private int m_productionPer100Hour;

		public LogicResourceProductionComponent(LogicGameObject gameObject, LogicResourceData data) : base(gameObject)
		{
			m_resourceTimer = new LogicTimer();
			m_resourceData = data;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.RESOURCE_PRODUCTION;

		public LogicResourceData GetResourceData()
			=> m_resourceData;

		public void RestartTimer()
		{
			int totalTime = 0;

			if (m_productionPer100Hour >= 1)
			{
				totalTime = (int)(360000L * m_maxResources / m_productionPer100Hour);
			}

			m_resourceTimer.StartTimer(totalTime, m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void SetProduction(int productionPer100Hour, int maxResources)
		{
			m_productionPer100Hour = productionPer100Hour;
			m_maxResources = maxResources;

			RestartTimer();
		}

		public int GetStealableResourceCount()
			=> LogicMath.Min(GetResourceCount(), m_availableLoot);

		public int GetResourceCount()
		{
			if (m_productionPer100Hour > 0)
			{
				int totalTime = (int)(360000L * m_maxResources / m_productionPer100Hour);

				if (totalTime > 0)
				{
					int remainingTime = m_resourceTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

					if (remainingTime != 0)
					{
						return (int)((long)m_productionPer100Hour * (totalTime - remainingTime) / 360000L);
					}

					return m_maxResources;
				}
			}

			return 0;
		}

		public void DecreaseResources(int count)
		{
			int resourceCount = GetResourceCount();
			int removeCount = LogicMath.Min(count, resourceCount);

			if (m_productionPer100Hour != 0)
			{
				int totalTime = (int)(360000L * m_maxResources / m_productionPer100Hour);
				int skipTime = (int)(360000L * (resourceCount - removeCount) / m_productionPer100Hour);

				m_resourceTimer.StartTimer(totalTime - skipTime, m_parent.GetLevel().GetLogicTime(), false, -1);
			}
		}

		public int CollectResources(bool updateListener)
		{
			if (m_parent.GetLevel().GetHomeOwnerAvatar() != null)
			{
				int resourceCount = GetResourceCount();

				if (m_parent.GetLevel().GetHomeOwnerAvatar().IsNpcAvatar())
				{
					Debugger.Error("LogicResourceProductionComponent::collectResources() called for Npc avatar");
				}
				else
				{
					LogicClientAvatar clientAvatar = (LogicClientAvatar)m_parent.GetLevel().GetHomeOwnerAvatar();

					if (resourceCount != 0)
					{
						if (m_resourceData.IsPremiumCurrency())
						{
							DecreaseResources(resourceCount);

							clientAvatar.SetDiamonds(clientAvatar.GetDiamonds() + resourceCount);
							clientAvatar.SetFreeDiamonds(clientAvatar.GetFreeDiamonds() + resourceCount);
							clientAvatar.GetChangeListener().FreeDiamondsAdded(resourceCount, 10);
						}
						else
						{
							int unusedResourceCap = clientAvatar.GetUnusedResourceCap(m_resourceData);

							if (unusedResourceCap != 0)
							{
								if (resourceCount > unusedResourceCap)
								{
									resourceCount = unusedResourceCap;
								}

								DecreaseResources(resourceCount);

								clientAvatar.CommodityCountChangeHelper(0, m_resourceData, resourceCount);
							}
							else
							{
								resourceCount = 0;
							}
						}

						return resourceCount;
					}
				}
			}

			return 0;
		}

		public override void FastForwardTime(int time)
		{
			int remainingSeconds = m_resourceTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			int boostTime = m_parent.GetRemainingBoostTime();

			if (boostTime > 0 && LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier() > 1 && !m_parent.IsBoostPaused())
			{
				time += (LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier() - 1) * LogicMath.Min(time, boostTime);
			}

			int clockBoostTime = m_parent.GetLevel().GetUpdatedClockTowerBoostTime();

			if (clockBoostTime > 0 && !m_parent.GetLevel().IsClockTowerBoostPaused())
			{
				if (m_parent.GetData().GetDataType() == DataType.BUILDING && m_parent.GetData().GetVillageType() == 1)
				{
					time += (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1) * LogicMath.Min(time, clockBoostTime);
				}
			}

			m_resourceTimer.StartTimer(remainingSeconds <= time ? 0 : remainingSeconds - time, m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public override void GetChecksum(ChecksumHelper checksum)
		{
			checksum.StartObject("LogicResourceProductionComponent");
			checksum.WriteValue("resourceTimer", m_resourceTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()));
			checksum.WriteValue("m_availableLoot", m_availableLoot);
			checksum.WriteValue("m_maxResources", m_maxResources);
			checksum.WriteValue("m_productionPer100Hour", m_productionPer100Hour);
			checksum.EndObject();
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONNumber resourceTimeObject = jsonObject.GetJSONNumber("res_time");
			int time = m_productionPer100Hour > 0 ? (int)(360000L * m_maxResources / m_productionPer100Hour) : 0;

			if (resourceTimeObject != null)
			{
				time = LogicMath.Min(resourceTimeObject.GetIntValue(), time);
			}

			m_resourceTimer.StartTimer(time, m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			jsonObject.Put("res_time", new LogicJSONNumber(m_resourceTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));
		}

		public void RecalculateAvailableLoot()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (!homeOwnerAvatar.IsNpcAvatar())
			{
				int matchType = m_parent.GetLevel().GetMatchType();

				if (matchType >= 10 || matchType != 3 && matchType != 5)
				{
					int resourceProductionLootPercentage = LogicDataTables.GetGlobals().GetResourceProductionLootPercentage(m_resourceData);

					if (homeOwnerAvatar.IsClientAvatar())
					{
						LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();

						if (visitorAvatar != null && visitorAvatar.IsClientAvatar())
						{
							resourceProductionLootPercentage = resourceProductionLootPercentage *
															   LogicDataTables.GetGlobals().GetLootMultiplierByTownHallDiff(visitorAvatar.GetTownHallLevel(),
																															homeOwnerAvatar.GetTownHallLevel()) / 100;
						}
					}

					if (resourceProductionLootPercentage > 100)
					{
						resourceProductionLootPercentage = 100;
					}

					m_availableLoot = (int)((long)GetResourceCount() * resourceProductionLootPercentage / 100L);
				}
				else
				{
					m_availableLoot = 0;
				}
			}
			else
			{
				m_availableLoot = 0;
			}
		}

		public void ResourcesStolen(int damage, int hp)
		{
			if (damage > 0 && hp > 0)
			{
				int stealableResource = damage * GetStealableResourceCount() / hp;

				if (stealableResource > 0)
				{
					m_parent.GetLevel().GetBattleLog().IncreaseStolenResourceCount(m_resourceData, stealableResource);
					DecreaseResources(stealableResource);
					m_parent.GetLevel().GetVisitorAvatar().CommodityCountChangeHelper(0, m_resourceData, stealableResource);
					m_availableLoot -= stealableResource;
				}
			}
		}

		public override void Tick()
		{
			if (m_parent.GetRemainingBoostTime() > 0 && !m_parent.IsBoostPaused())
			{
				m_resourceTimer.FastForwardSubticks(4 * LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier() - 4);
			}

			if (m_parent.GetLevel().GetRemainingClockTowerBoostTime() > 0)
			{
				if (m_parent.GetData().GetDataType() == DataType.BUILDING && m_parent.GetData().GetVillageType() == 1)
				{
					m_resourceTimer.FastForwardSubticks(4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
				}
			}
		}
	}
}