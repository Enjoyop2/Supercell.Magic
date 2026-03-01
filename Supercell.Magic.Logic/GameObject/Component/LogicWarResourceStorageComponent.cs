using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicWarResourceStorageComponent : LogicResourceStorageComponent
	{
		public LogicWarResourceStorageComponent(LogicGameObject gameObject) : base(gameObject)
		{
			// LogicWarResourceStorageComponent.
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.WAR_RESOURCE_STORAGE;

		public bool IsNotEmpty()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			LogicDataTable table = LogicDataTables.GetTable(DataType.RESOURCE);

			for (int i = 0; i < table.GetItemCount(); i++)
			{
				LogicResourceData data = (LogicResourceData)table.GetItemAt(i);

				if (data.GetWarResourceReferenceData() != null)
				{
					if (homeOwnerAvatar.GetResourceCount(data) > 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public override int GetMax(int idx)
		{
			int multiplierPercent = 100;

			if (m_parent.GetLevel().GetHomeOwnerAvatar() != null)
			{
				if (m_parent.GetLevel().GetHomeOwnerAvatar().IsClientAvatar())
				{
					int allianceExpLevel = ((LogicClientAvatar)m_parent.GetLevel().GetHomeOwnerAvatar()).GetAllianceLevel();

					if (allianceExpLevel > 0)
					{
						multiplierPercent = LogicDataTables.GetAllianceLevel(allianceExpLevel).GetWarLootCapacityPercent();
					}
				}
			}

			return multiplierPercent * m_maxResourceCount[idx] / 100;
		}

		public override void RecalculateAvailableLoot()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			LogicDataTable resourceTable = LogicDataTables.GetTable(DataType.RESOURCE);

			for (int i = 0; i < m_resourceCount.Size(); i++)
			{
				LogicResourceData data = (LogicResourceData)resourceTable.GetItemAt(i);

				if (m_parent.GetData() == LogicDataTables.GetAllianceCastleData())
				{
					LogicResourceData refData = data.GetWarResourceReferenceData();

					int resourceCount = m_resourceCount[i];

					if (refData != null)
					{
						int warLootPercentage = LogicDataTables.GetGlobals().GetWarLootPercentage();
						int lootableResourceCount = 0;

						if ((m_parent.GetLevel().GetMatchType() | 4) != 7 && !m_parent.GetLevel().IsArrangedWar())
						{
							lootableResourceCount = (int)((long)resourceCount * warLootPercentage / 100);
						}

						int storageLootCap = LogicDataTables.GetTownHallLevel(homeOwnerAvatar.GetTownHallLevel()).GetStorageLootCap(data);
						int maxResourceCount = LogicMath.Min(homeOwnerAvatar.GetResourceCount(data), homeOwnerAvatar.GetResourceCap(data));

						if (maxResourceCount > storageLootCap && maxResourceCount > 0)
						{
							int clampedValue;

							if (storageLootCap < 1000000)
							{
								if (storageLootCap < 100000)
								{
									if (storageLootCap < 10000)
									{
										clampedValue = storageLootCap < 1000
											? (resourceCount * storageLootCap + (maxResourceCount >> 1)) / maxResourceCount
											: 10 * ((resourceCount * (storageLootCap / 10) + (maxResourceCount >> 1)) / maxResourceCount);
									}
									else
									{
										clampedValue = 100 * ((resourceCount * (storageLootCap / 100) + (maxResourceCount >> 1)) / maxResourceCount);
									}
								}
								else
								{
									clampedValue = 1000 * ((resourceCount * (storageLootCap / 1000) + (maxResourceCount >> 1)) / maxResourceCount);
								}
							}
							else
							{
								clampedValue = 40000 * ((resourceCount * (storageLootCap / 40000) + (maxResourceCount >> 1)) / maxResourceCount);
							}

							if (lootableResourceCount > clampedValue)
							{
								lootableResourceCount = clampedValue;
							}
						}

						if (lootableResourceCount > resourceCount)
						{
							lootableResourceCount = resourceCount;
						}

						m_stealableResourceCount[i] = lootableResourceCount;
					}
				}
			}
		}

		public override void ResourcesStolen(int damage, int hp)
		{
			if (damage > 0 && hp > 0)
			{
				LogicDataTable table = LogicDataTables.GetTable(DataType.RESOURCE);

				for (int i = 0; i < m_stealableResourceCount.Size(); i++)
				{
					LogicResourceData data = (LogicResourceData)table.GetItemAt(i);

					int stealableResource = GetStealableResourceCount(i);

					if (damage < hp)
					{
						stealableResource = damage * stealableResource / hp;
					}

					if (stealableResource > 0 && data.GetWarResourceReferenceData() != null)
					{
						m_parent.GetLevel().GetBattleLog().IncreaseStolenResourceCount(data.GetWarResourceReferenceData(), stealableResource);
						m_resourceCount[i] -= stealableResource;

						LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
						LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();

						homeOwnerAvatar.CommodityCountChangeHelper(0, data, -stealableResource);
						visitorAvatar.CommodityCountChangeHelper(0, data.GetWarResourceReferenceData(), stealableResource);

						m_stealableResourceCount[i] = LogicMath.Max(m_stealableResourceCount[i] - stealableResource, 0);
					}
				}
			}
		}

		public int CollectResources()
		{
			int collected = -1;

			if (m_parent.GetLevel().GetHomeOwnerAvatar().IsClientAvatar())
			{
				collected = 0;

				LogicClientAvatar playerAvatar = (LogicClientAvatar)m_parent.GetLevel().GetHomeOwnerAvatar();
				LogicDataTable table = LogicDataTables.GetTable(DataType.RESOURCE);

				for (int i = 0; i < table.GetItemCount(); i++)
				{
					LogicResourceData data = (LogicResourceData)table.GetItemAt(i);

					if (data.GetWarResourceReferenceData() != null)
					{
						int count = playerAvatar.GetResourceCount(data);

						if (count > 0)
						{
							int unusedResourceCap = playerAvatar.GetUnusedResourceCap(data.GetWarResourceReferenceData());

							if (unusedResourceCap != 0)
							{
								if (count > unusedResourceCap)
								{
									count = unusedResourceCap;
								}

								if (data.GetName().Equals("WarGold"))
								{
									m_parent.GetLevel().GetAchievementManager().IncreaseWarGoldResourceLoot(count);
								}

								collected = count;

								playerAvatar.CommodityCountChangeHelper(0, data.GetWarResourceReferenceData(), count);
								playerAvatar.CommodityCountChangeHelper(0, data, -count);
							}
						}
					}
				}
			}

			return collected;
		}
	}
}