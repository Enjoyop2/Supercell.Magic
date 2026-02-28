using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicTownhallLevelData : LogicData
	{
		private LogicArrayList<int> m_buildingCaps;
		private LogicArrayList<int> m_buildingGearupCaps;
		private LogicArrayList<int> m_trapCaps;
		private LogicArrayList<int> m_treasuryCaps;

		private int m_attackCost;
		private int m_attackCostVillage2;
		private int m_maxHousingSpace;
		private int m_friendlyCost;
		private int m_resourceStorageLootPercentage;
		private int m_darkElixirStorageLootPercentage;
		private int m_resourceStorageLootCap;
		private int m_darkElixirStorageLootCap;
		private int m_warPrizeResourceCap;
		private int m_warPrizeDarkElixirCap;
		private int m_warPrizeAllianceExpCap;
		private int m_cartLootCapResource;
		private int m_cartLootReengagementResource;
		private int m_cartLootCapDarkElixir;
		private int m_cartLootReengagementDarkElixir;
		private int m_strengthMaxTroopTypes;
		private int m_strengthMaxSpellTypes;
		private int m_packElixir;
		private int m_packGold;
		private int m_packDarkElixir;
		private int m_packGold2;
		private int m_packElixir2;
		private int m_duelPrizeResourceCap;
		private int m_changeTroopCost;

		public LogicTownhallLevelData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			m_maxHousingSpace = -1;
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_buildingCaps = new LogicArrayList<int>();
			m_buildingGearupCaps = new LogicArrayList<int>();
			m_trapCaps = new LogicArrayList<int>();
			m_treasuryCaps = new LogicArrayList<int>();

			LogicTownhallLevelData previousItem = null;

			if (GetInstanceID() > 0)
			{
				previousItem = (LogicTownhallLevelData)m_table.GetItemAt(GetInstanceID() - 1);
			}

			LogicDataTable buildingTable = LogicDataTables.GetTable(LogicDataType.BUILDING);

			for (int i = 0; i < buildingTable.GetItemCount(); i++)
			{
				LogicData item = buildingTable.GetItemAt(i);

				int cap = GetIntegerValue(item.GetName(), 0);
				int gearup = GetIntegerValue(item.GetName() + "_gearup", 0);

				if (previousItem != null)
				{
					if (cap == 0)
					{
						cap = previousItem.m_buildingCaps[i];
					}

					if (gearup == 0)
					{
						gearup = previousItem.m_buildingGearupCaps[i];
					}
				}

				m_buildingCaps.Add(cap);
				m_buildingGearupCaps.Add(gearup);
			}

			LogicDataTable trapTable = LogicDataTables.GetTable(LogicDataType.TRAP);

			for (int i = 0; i < trapTable.GetItemCount(); i++)
			{
				int cap = GetIntegerValue(trapTable.GetItemAt(i).GetName(), 0);

				if (previousItem != null)
				{
					if (cap == 0)
					{
						cap = previousItem.m_trapCaps[i];
					}
				}

				m_trapCaps.Add(cap);
			}

			LogicDataTable resourceTable = LogicDataTables.GetTable(LogicDataType.RESOURCE);

			for (int i = 0; i < resourceTable.GetItemCount(); i++)
			{
				m_treasuryCaps.Add(GetIntegerValue("Treasury" + resourceTable.GetItemAt(i).GetName(), 0));
			}

			m_attackCost = GetIntegerValue("AttackCost", 0);
			m_attackCostVillage2 = GetIntegerValue("AttackCostVillage2", 0);
			m_resourceStorageLootPercentage = GetIntegerValue("ResourceStorageLootPercentage", 0);
			m_darkElixirStorageLootPercentage = GetIntegerValue("DarkElixirStorageLootPercentage", 0);
			m_resourceStorageLootCap = GetIntegerValue("ResourceStorageLootCap", 0);
			m_darkElixirStorageLootCap = GetIntegerValue("DarkElixirStorageLootCap", 0);
			m_warPrizeResourceCap = GetIntegerValue("WarPrizeResourceCap", 0);
			m_warPrizeDarkElixirCap = GetIntegerValue("WarPrizeDarkElixirCap", 0);
			m_warPrizeAllianceExpCap = GetIntegerValue("WarPrizeAllianceExpCap", 0);
			m_cartLootCapResource = GetIntegerValue("CartLootCapResource", 0);
			m_cartLootReengagementResource = GetIntegerValue("CartLootReengagementResource", 0);
			m_cartLootCapDarkElixir = GetIntegerValue("CartLootCapDarkElixir", 0);
			m_cartLootReengagementDarkElixir = GetIntegerValue("CartLootReengagementDarkElixir", 0);
			m_strengthMaxTroopTypes = GetIntegerValue("StrengthMaxTroopTypes", 0);
			m_strengthMaxSpellTypes = GetIntegerValue("StrengthMaxSpellTypes", 0);
			m_friendlyCost = GetIntegerValue("FriendlyCost", 0);
			m_packElixir = GetIntegerValue("PackElixir", 0);
			m_packGold = GetIntegerValue("PackGold", 0);
			m_packDarkElixir = GetIntegerValue("PackDarkElixir", 0);
			m_packGold2 = GetIntegerValue("PackGold2", 0);
			m_packElixir2 = GetIntegerValue("PackElixir2", 0);
			m_duelPrizeResourceCap = GetIntegerValue("DuelPrizeResourceCap", 0);
			m_changeTroopCost = GetIntegerValue("ChangeTroopCost", 0);

			if ((uint)m_darkElixirStorageLootPercentage > 100 || (uint)m_darkElixirStorageLootPercentage > 100)
			{
				Debugger.Error("townhall_levels.csv: Invalid loot percentage!");
			}
		}

		public int GetStorageLootPercentage(LogicResourceData data)
		{
			if (LogicDataTables.GetDarkElixirData() == data)
			{
				return m_darkElixirStorageLootPercentage;
			}

			return m_resourceStorageLootPercentage;
		}

		public int GetAttackCost()
			=> m_attackCost;

		public int GetAttackCostVillage2()
			=> m_attackCostVillage2;

		public int GetFriendlyCost()
			=> m_friendlyCost;

		public int GetStorageLootCap(LogicResourceData data)
		{
			if (data != null && !data.IsPremiumCurrency())
			{
				if (LogicDataTables.GetDarkElixirData() == data)
				{
					return m_darkElixirStorageLootCap;
				}

				return m_resourceStorageLootCap;
			}

			return 0;
		}

		public int GetCartLootCap(LogicResourceData data)
		{
			if (data != null && !data.IsPremiumCurrency())
			{
				if (LogicDataTables.GetDarkElixirData() == data)
				{
					return m_cartLootCapDarkElixir;
				}

				return m_cartLootCapResource;
			}

			return 0;
		}

		public int GetCartLootReengagement(LogicResourceData data)
		{
			if (data != null && !data.IsPremiumCurrency() && data.GetWarResourceReferenceData() == null && data.GetVillageType() != 1)
			{
				if (LogicDataTables.GetDarkElixirData() == data)
				{
					return m_cartLootReengagementDarkElixir;
				}

				return m_cartLootReengagementResource;
			}

			return 0;
		}

		public int GetMaxHousingSpace()
		{
			if (m_maxHousingSpace == -1)
			{
				CalculateHousingSpaceCap();
			}

			return m_maxHousingSpace;
		}

		public void CalculateHousingSpaceCap()
		{
			m_maxHousingSpace = 0;

			if (GetInstanceID() > 0)
			{
				m_table.GetItemAt(GetInstanceID() - 1); // Thx supercell for the crappy code.
			}

			LogicDataTable buildingTable = LogicDataTables.GetTable(LogicDataType.BUILDING);
			int dataTableCount = m_table.GetItemCount();

			if (dataTableCount > 0)
			{
				int unitHousingCostMultiplierForTotal = LogicDataTables.GetGlobals().GetUnitHousingCostMultiplierForTotal();
				int spellHousingCostMultiplierForTotal = LogicDataTables.GetGlobals().GetSpellHousingCostMultiplierForTotal();
				int heroHousingCostMultiplierForTotal = LogicDataTables.GetGlobals().GetHeroHousingCostMultiplierForTotal();
				int allianceUnitHousingCostMultiplierForTotal = LogicDataTables.GetGlobals().GetAllianceUnitHousingCostMultiplierForTotal();

				int idx = 0;

				do
				{
					LogicBuildingData buildingData = (LogicBuildingData)buildingTable.GetItemAt(idx);
					int count = m_buildingCaps[idx];

					if (count > 0)
					{
						int multiplier = unitHousingCostMultiplierForTotal;
						int maxUpgLevel = buildingData.GetMaxUpgradeLevelForTownHallLevel(GetInstanceID());

						if (maxUpgLevel >= 0)
						{
							int housingSpace = buildingData.GetUnitStorageCapacity(maxUpgLevel);

							if (!buildingData.IsAllianceCastle())
							{
								if (buildingData.IsForgesMiniSpells() || buildingData.IsForgesSpells())
								{
									multiplier = spellHousingCostMultiplierForTotal;
								}
								else if (buildingData.IsHeroBarrack())
								{
									housingSpace = buildingData.GetHeroData().GetHousingSpace();
									multiplier = heroHousingCostMultiplierForTotal;
								}
							}
							else
							{
								multiplier = allianceUnitHousingCostMultiplierForTotal;
							}

							if (housingSpace > 0)
							{
								m_maxHousingSpace += multiplier * count * housingSpace / 100;
							}
						}
					}
				} while (++idx != dataTableCount);
			}
		}

		public int GetUnlockedBuildingCount(LogicBuildingData data)
			=> m_buildingCaps[data.GetInstanceID()];

		public int GetUnlockedBuildingGearupCount(LogicBuildingData data)
			=> m_buildingGearupCaps[data.GetInstanceID()];

		public int GetUnlockedTrapCount(LogicTrapData data)
			=> m_trapCaps[data.GetInstanceID()];

		public LogicArrayList<int> GetTreasuryCaps()
			=> m_treasuryCaps;

		public int GetStrengthMaxTroopTypes()
			=> m_strengthMaxTroopTypes;

		public int GetStrengthMaxSpellTypes()
			=> m_strengthMaxSpellTypes;

		public int GetPackElixir()
			=> m_packElixir;

		public int GetPackGold()
			=> m_packGold;

		public int GetPackDarkElixir()
			=> m_packDarkElixir;

		public int GetPackGold2()
			=> m_packGold2;

		public int GetPackElixir2()
			=> m_packElixir2;

		public int GetDuelPrizeResourceCap()
			=> m_duelPrizeResourceCap;

		public int GetChangeTroopCost()
			=> m_changeTroopCost;
	}
}