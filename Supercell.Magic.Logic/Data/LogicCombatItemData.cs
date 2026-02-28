using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Data
{
	public class LogicCombatItemData : LogicGameObjectData
	{
		public const int COMBAT_ITEM_TYPE_CHARACTER = 0;
		public const int COMBAT_ITEM_TYPE_SPELL = 1;
		public const int COMBAT_ITEM_TYPE_HERO = 2;

		private LogicResourceData[] m_upgradeResourceData;
		private LogicResourceData m_trainingResourceData;

		protected int m_upgradeLevelCount;

		private int[] m_upgradeTime;
		private int[] m_upgradeCost;
		private int[] m_trainingTime;
		private int[] m_trainingCost;
		private int[] m_laboratoryLevel;
		private int[] m_upgradeLevelByTownHall;

		private int m_housingSpace;
		private int m_unitType;
		private int m_donateCost;

		private bool m_productionEnabled;
		private bool m_enableByCalendar;

		public LogicCombatItemData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicCombatItemData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			int size = m_upgradeLevelCount = m_row.GetBiggestArraySize();

			m_upgradeLevelByTownHall = new int[size];
			m_upgradeTime = new int[size];
			m_upgradeCost = new int[size];
			m_trainingTime = new int[size];
			m_trainingCost = new int[size];
			m_laboratoryLevel = new int[size];

			m_upgradeResourceData = new LogicResourceData[size];

			for (int i = 0; i < size; i++)
			{
				m_upgradeLevelByTownHall[i] = GetClampedIntegerValue("UpgradeLevelByTH", i);
				m_upgradeTime[i] = 3600 * GetClampedIntegerValue("UpgradeTimeH", i) + 60 * GetClampedIntegerValue("UpgradeTimeM", i);
				m_upgradeCost[i] = GetClampedIntegerValue("UpgradeCost", i);
				m_trainingTime[i] = GetClampedIntegerValue("TrainingTime", i);
				m_trainingCost[i] = GetClampedIntegerValue("TrainingCost", i);
				m_laboratoryLevel[i] = GetClampedIntegerValue("LaboratoryLevel", i) - 1;

				m_upgradeResourceData[i] = LogicDataTables.GetResourceByName(GetClampedValue("UpgradeResource", i), this);

				if (m_upgradeResourceData[i] == null && GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_HERO)
				{
					Debugger.Error("UpgradeResource is not defined for " + GetName());
				}
			}

			if (GetName().Equals("Barbarian2"))
			{
				if (m_upgradeTime[0] == 0)
				{
					m_upgradeTime[0] = 30;
				}
			}

			m_trainingResourceData = LogicDataTables.GetResourceByName(GetValue("TrainingResource", 0), this);
			m_housingSpace = GetIntegerValue("HousingSpace", 0);
			m_productionEnabled = !GetBooleanValue("DisableProduction", 0);
			m_enableByCalendar = GetBooleanValue("EnabledByCalendar", 0);
			m_unitType = GetIntegerValue("UnitOfType", 0);
			m_donateCost = GetIntegerValue("DonateCost", 0);

			if (m_trainingResourceData == null && GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_HERO)
			{
				Debugger.Error("TrainingResource is not defined for " + GetName());
			}
		}

		public virtual bool IsDonationDisabled()
			=> false;

		public int GetDonateCost()
			=> m_donateCost;

		public int GetUpgradeLevelCount()
			=> m_upgradeLevelCount;

		public int GetUpgradeTime(int idx)
			=> m_upgradeTime[idx];

		public LogicResourceData GetUpgradeResource(int idx)
			=> m_upgradeResourceData[idx];

		public int GetUpgradeCost(int idx)
			=> m_upgradeCost[idx];

		public LogicResourceData GetTrainingResource()
			=> m_trainingResourceData;

		public int GetTrainingCost(int idx)
			=> m_trainingCost[idx];

		public int GetUnitOfType()
			=> m_unitType;

		public int GetRequiredLaboratoryLevel(int idx)
			=> m_laboratoryLevel[idx];

		public virtual int GetRequiredProductionHouseLevel()
			=> 0;

		public virtual bool IsUnlockedForProductionHouseLevel(int level)
			=> false;

		public virtual LogicBuildingData GetProductionHouseData()
			=> null;

		public virtual bool IsUnderground()
			=> false;

		public int GetHousingSpace()
			=> m_housingSpace;

		public int GetUpgradeLevelByTownHall(int townHallLevel)
		{
			int levelCount = m_upgradeLevelCount;

			if (levelCount >= 2)
			{
				int index = 1;

				while (townHallLevel + 1 >= m_upgradeLevelByTownHall[index])
				{
					if (++index >= levelCount)
					{
						return levelCount - 1;
					}
				}

				levelCount = index;
			}

			return levelCount - 1;
		}

		public bool UseUpgradeLevelByTownHall()
			=> m_upgradeLevelByTownHall[0] > 0;

		public int GetTrainingTime(int index, LogicLevel level, int additionalBarrackCount)
		{
			int trainingTime = m_trainingTime[index];

			if (LogicDataTables.GetGlobals().UseNewTraining() &&
				GetVillageType() != 1 &&
				GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				if (level != null)
				{
					LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);

					switch (m_unitType)
					{
						case 1:
							int barrackCount = gameObjectManager.GetBarrackCount();
							int productionLevel = GetRequiredProductionHouseLevel();
							int barrackFound = 0;

							for (int i = 0; i < barrackCount; i++)
							{
								LogicBuilding barrack = (LogicBuilding)gameObjectManager.GetBarrack(i);

								if (barrack != null)
								{
									if (barrack.GetBuildingData().GetProducesUnitsOfType() == GetUnitOfType())
									{
										if (barrack.GetUpgradeLevel() >= productionLevel)
										{
											if (!barrack.IsConstructing())
											{
												barrackFound += 1;
											}
										}
									}
								}
							}

							if (barrackFound + additionalBarrackCount <= 0)
							{
								return trainingTime;
							}

							int[] barrackDivisor = LogicDataTables.GetGlobals().GetBarrackReduceTrainingDevisor();
							int divisor = barrackDivisor[LogicMath.Min(barrackDivisor.Length - 1, barrackFound + additionalBarrackCount - 1)];

							if (divisor > 0)
							{
								return trainingTime / divisor;
							}

							return trainingTime;
						case 2:
							barrackCount = gameObjectManager.GetDarkBarrackCount();
							productionLevel = GetRequiredProductionHouseLevel();
							barrackFound = 0;

							for (int i = 0; i < barrackCount; i++)
							{
								LogicBuilding barrack = (LogicBuilding)gameObjectManager.GetDarkBarrack(i);

								if (barrack != null)
								{
									if (barrack.GetBuildingData().GetProducesUnitsOfType() == GetUnitOfType())
									{
										if (barrack.GetUpgradeLevel() >= productionLevel)
										{
											if (!barrack.IsConstructing())
											{
												barrackFound += 1;
											}
										}
									}
								}
							}

							if (barrackFound + additionalBarrackCount <= 0)
							{
								return trainingTime;
							}

							barrackDivisor = LogicDataTables.GetGlobals().GetDarkBarrackReduceTrainingDevisor();
							divisor = barrackDivisor[LogicMath.Min(barrackDivisor.Length - 1, barrackFound + additionalBarrackCount - 1)];

							if (divisor > 0)
							{
								return trainingTime / divisor;
							}

							return trainingTime;
						default:
							Debugger.Error("invalid type for unit");
							break;
					}
				}
				else
				{
					Debugger.Error("level was null in getTrainingTime()");
				}
			}

			return trainingTime;
		}

		public bool IsProductionEnabled()
			=> m_productionEnabled;

		public override bool IsEnableByCalendar()
			=> m_enableByCalendar;

		public virtual int GetCombatItemType()
			=> -1;
	}
}