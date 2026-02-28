using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public class LogicResourceStorageComponent : LogicComponent
	{
		protected LogicArrayList<int> m_resourceCount;
		protected LogicArrayList<int> m_maxResourceCount;
		protected LogicArrayList<int> m_maxPercentageResourceCount;
		protected LogicArrayList<int> m_stealableResourceCount;

		public LogicResourceStorageComponent(LogicGameObject gameObject) : base(gameObject)
		{
			int resourceCount = LogicDataTables.GetTable(LogicDataType.RESOURCE).GetItemCount();

			m_maxResourceCount = new LogicArrayList<int>(resourceCount);
			m_stealableResourceCount = new LogicArrayList<int>(resourceCount);
			m_resourceCount = new LogicArrayList<int>(resourceCount);
			m_maxPercentageResourceCount = new LogicArrayList<int>(resourceCount);

			for (int i = 0; i < resourceCount; i++)
			{
				m_resourceCount.Add(0);
				m_maxResourceCount.Add(0);
				m_stealableResourceCount.Add(0);
				m_maxPercentageResourceCount.Add(100);
			}
		}

		public override void Destruct()
		{
			base.Destruct();

			m_maxResourceCount = null;
			m_stealableResourceCount = null;
			m_resourceCount = null;
			m_maxPercentageResourceCount = null;
		}

		public int GetStealableResourceCount(int idx)
			=> LogicMath.Min(m_resourceCount[idx], m_stealableResourceCount[idx]);

		public virtual void ResourcesStolen(int damage, int hp)
		{
			if (damage > 0 && hp > 0)
			{
				LogicDataTable table = LogicDataTables.GetTable(LogicDataType.RESOURCE);

				for (int i = 0; i < m_stealableResourceCount.Size(); i++)
				{
					LogicResourceData data = (LogicResourceData)table.GetItemAt(i);

					int stealableResource = GetStealableResourceCount(i);

					if (damage < hp)
					{
						stealableResource = damage * stealableResource / hp;
					}

					if (stealableResource > 0)
					{
						m_parent.GetLevel().GetBattleLog().IncreaseStolenResourceCount(data, stealableResource);
						m_resourceCount[i] -= stealableResource;

						LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
						LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();

						homeOwnerAvatar.CommodityCountChangeHelper(0, data, -stealableResource);
						visitorAvatar.CommodityCountChangeHelper(0, data, stealableResource);

						if (homeOwnerAvatar.IsNpcAvatar())
						{
							LogicNpcData npcData = ((LogicNpcAvatar)homeOwnerAvatar).GetNpcData();

							if (data == LogicDataTables.GetGoldData())
							{
								visitorAvatar.CommodityCountChangeHelper(1, npcData, stealableResource);
							}
							else if (data == LogicDataTables.GetElixirData())
							{
								visitorAvatar.CommodityCountChangeHelper(2, npcData, stealableResource);
							}
						}

						m_stealableResourceCount[i] = LogicMath.Max(m_stealableResourceCount[i] - stealableResource, 0);
					}
				}
			}
		}

		public void SetCount(int idx, int count)
		{
			m_resourceCount[idx] = LogicMath.Clamp(count, 0, GetMax(idx));

			if (m_parent.GetListener() != null)
				m_parent.GetListener().RefreshResourceCount();
		}

		public virtual int GetMax(int idx)
		{
			if (m_parent.GetData() != LogicDataTables.GetTownHallData() || !m_parent.GetLevel().IsNpcVillage())
				return m_maxResourceCount[idx];
			return 10000000;
		}

		public int GetCount(int idx)
			=> m_resourceCount[idx];

		public int GetRecommendedMax(int idx)
		{
			int max = GetMax(idx);

			LogicHitpointComponent hitpointComponent = m_parent.GetHitpointComponent();

			if (hitpointComponent != null)
			{
				int hp = hitpointComponent.GetHitpoints();
				int maxHp = hitpointComponent.GetMaxHitpoints();

				if (hp <= 10000)
				{
					if (hp <= 1000)
						return max * hp / maxHp;
					return 100 * (max * (hp / 100) / maxHp);
				}
				return 1000 * (max * (hp / 1000) / maxHp);
			}

			return max;
		}

		public int GetRecommendedMax(int idx, int count)
		{
			int max = GetRecommendedMax(idx);

			if (m_maxPercentageResourceCount[idx] != 0)
				return LogicMath.Min(m_maxPercentageResourceCount[idx] * count / 100, max);
			return max;
		}

		public void SetMaxArray(LogicArrayList<int> max)
		{
			for (int i = 0; i < max.Size(); i++)
			{
				m_maxResourceCount[i] = max[i];
			}

			m_parent.GetLevel().RefreshResourceCaps();
		}

		public void SetMaxPercentageArray(LogicArrayList<int> max)
		{
			for (int i = 0; i < max.Size(); i++)
			{
				m_maxPercentageResourceCount[i] = max[i];
			}
		}

		public virtual void RecalculateAvailableLoot()
		{
			int matchType = m_parent.GetLevel().GetMatchType();

			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();
			LogicDataTable resourceTable = LogicDataTables.GetTable(LogicDataType.RESOURCE);

			for (int i = 0; i < m_resourceCount.Size(); i++)
			{
				LogicResourceData data = (LogicResourceData)resourceTable.GetItemAt(i);
				int resourceCount = m_resourceCount[i];

				if (!homeOwnerAvatar.IsNpcAvatar())
				{
					if (matchType == 5 && m_parent.GetLevel().IsArrangedWar())
					{
						if (resourceCount >= 0)
						{
							resourceCount = 0;
						}
					}
					else if (LogicDataTables.GetGlobals().UseTownHallLootPenaltyInWar() || matchType != 5)
					{
						if (matchType != 8 && matchType != 9)
						{
							int multiplier = 100;
							int calculateAvailableLootCount = 0;

							if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar() &&
								visitorAvatar != null && visitorAvatar.IsClientAvatar())
							{
								multiplier = LogicDataTables.GetGlobals().GetLootMultiplierByTownHallDiff(visitorAvatar.GetTownHallLevel(), homeOwnerAvatar.GetTownHallLevel());
							}

							if (m_parent.GetData() == LogicDataTables.GetTownHallData() && LogicDataTables.GetGlobals().GetTownHallLootPercentage() != -1)
							{
								calculateAvailableLootCount = resourceCount * (multiplier * LogicDataTables.GetGlobals().GetTownHallLootPercentage() / 100) / 100;
							}
							else if (!data.IsPremiumCurrency())
							{
								int townHallLevel = homeOwnerAvatar.GetTownHallLevel();
								int lootableResourceCount = 0;

								if (matchType != 3)
								{
									if (matchType == 5)
									{
										lootableResourceCount = resourceCount;
									}
									else if (matchType != 7)
									{
										lootableResourceCount = (int)((long)resourceCount * LogicDataTables.GetTownHallLevel(townHallLevel).GetStorageLootPercentage(data) / 100);
									}
								}

								int storageLootCap = LogicDataTables.GetTownHallLevel(townHallLevel).GetStorageLootCap(data);
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
												if (storageLootCap < 1000)
												{
													clampedValue = (resourceCount * storageLootCap + (maxResourceCount >> 1)) / maxResourceCount;
												}
												else
												{
													if (!LogicDataTables.GetGlobals().UseMoreAccurateLootCap())
													{
														clampedValue = 100 * ((resourceCount * (storageLootCap / 100) + (maxResourceCount >> 1)) / maxResourceCount);
													}
													else
													{
														if (resourceCount / 100 > maxResourceCount / storageLootCap)
														{
															clampedValue = 100 * ((resourceCount * (storageLootCap / 100) + (maxResourceCount >> 1)) / maxResourceCount);
														}
														else
														{
															clampedValue = (resourceCount * storageLootCap + (maxResourceCount >> 1)) / maxResourceCount;
														}
													}
												}
											}
											else
											{
												if (!LogicDataTables.GetGlobals().UseMoreAccurateLootCap())
												{
													clampedValue = 1000 * ((resourceCount * (storageLootCap / 1000) + (maxResourceCount >> 1)) / maxResourceCount);
												}
												else
												{
													if (resourceCount / 1000 > maxResourceCount / storageLootCap)
													{
														clampedValue = 1000 * ((resourceCount * (storageLootCap / 1000) + (maxResourceCount >> 1)) / maxResourceCount);
													}
													else
													{
														if (resourceCount / 100 > maxResourceCount / storageLootCap)
														{
															clampedValue = 100 * ((resourceCount * (storageLootCap / 100) + (maxResourceCount >> 1)) / maxResourceCount);
														}
														else
														{
															clampedValue = (resourceCount * storageLootCap + (maxResourceCount >> 1)) / maxResourceCount;
														}
													}
												}
											}
										}
										else
										{
											if (!LogicDataTables.GetGlobals().UseMoreAccurateLootCap())
											{
												clampedValue = 10000 * ((resourceCount * (storageLootCap / 10000) + (maxResourceCount >> 1)) / maxResourceCount);
											}
											else
											{
												if (resourceCount / 10000 > maxResourceCount / storageLootCap)
												{
													clampedValue = 10000 * ((resourceCount * (storageLootCap / 10000) + (maxResourceCount >> 1)) / maxResourceCount);
												}
												else
												{
													if (resourceCount / 1000 > maxResourceCount / storageLootCap)
													{
														clampedValue = 1000 * ((resourceCount * (storageLootCap / 1000) + (maxResourceCount >> 1)) / maxResourceCount);
													}
													else
													{
														if (resourceCount / 100 > maxResourceCount / storageLootCap)
														{
															clampedValue = 100 * ((resourceCount * (storageLootCap / 100) + (maxResourceCount >> 1)) / maxResourceCount);
														}
														else
														{
															clampedValue = (resourceCount * storageLootCap + (maxResourceCount >> 1)) / maxResourceCount;
														}
													}
												}
											}
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

								calculateAvailableLootCount = multiplier * lootableResourceCount / 100;
							}

							if (calculateAvailableLootCount <= resourceCount)
							{
								resourceCount = calculateAvailableLootCount;
							}
						}
					}
				}

				m_stealableResourceCount[i] = resourceCount;
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.RESOURCE_STORAGE;
	}
}