using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicBuildingData : LogicGameObjectData
	{
		private LogicBuildingData m_gearUpBuildingData;
		private LogicBuildingClassData m_buildingClass;
		private LogicBuildingClassData m_secondaryTargetingClass;
		private LogicBuildingClassData m_shopBuildingClass;
		private LogicResourceData[] m_buildResourceData;
		private LogicResourceData[] m_altBuildResourceData;
		private LogicResourceData[] m_ammoResourceData;
		private LogicResourceData m_gearUpResourceData;
		private LogicResourceData m_produceResourceData;
		private LogicHeroData m_heroData;
		private LogicArrayList<int>[] m_storedResourceCounts;
		private LogicArrayList<int>[] m_percentageStoredResourceCounts;
		private LogicArrayList<LogicAttackerItemData> m_attackItemData;
		private LogicSpellData m_areaOfEffectSpellData;
		private LogicSpellData m_alternativeAreaOfEffectSpellData;
		private LogicEffectData m_loadAmmoEffect;
		private LogicEffectData m_noAmmoEffect;
		private LogicEffectData m_toggleAttackModeEffect;
		private LogicEffectData m_pickUpEffect;
		private LogicEffectData m_placingEffect;
		private LogicEffectData m_appearEffect;
		private LogicEffectData m_dieDamageEffect;
		private LogicCharacterData[] m_defenceTroopCharacter;
		private LogicCharacterData[] m_defenceTroopCharacter2;

		private int[] m_buildCost;
		private int[] m_constructionTimes;
		private int[] m_townHallLevel;
		private int[] m_townHallVillage2Level;
		private int[] m_wallBlockX;
		private int[] m_wallBlockY;
		private int[] m_gearUpTime;
		private int[] m_gearUpCost;
		private int[] m_boostCost;
		private int[] m_ammoCost;
		private int[] m_housingSpace;
		private int[] m_housingSpaceAlt;
		private int[] m_resourcePer100Hours;
		private int[] m_resourceMax;
		private int[] m_resourceIconLimit;
		private int[] m_hitpoints;
		private int[] m_regenTime;
		private int[] m_amountCanBeUpgraded;
		private int[] m_unitProduction;
		private int[] m_strengthWeight;
		private int[] m_destructionXP;
		private int[] m_defenceTroopLevel;
		private int[] m_defenceTroopCount;
		private int[] m_dieDamage;

		private int m_width;
		private int m_height;
		private int m_village2Housing;
		private int m_producesUnitsOfType;
		private int m_chainAttackDistance;
		private int m_buildingW;
		private int m_buildingH;
		private int m_baseGfx;
		private int m_startingHomeCount;
		private int m_triggerRadius;
		private int m_aimRotateStep;
		private int m_turnSpeed;
		private int m_dieDamageRadius;
		private int m_dieDamageDelay;
		private int m_redMul;
		private int m_greenMul;
		private int m_blueMul;
		private int m_redAdd;
		private int m_greenAdd;
		private int m_blueAdd;
		private int m_newTargetAttackDelay;
		private int m_gearUpLevelRequirement;
		private int m_upgradeLevelCount;
		private int m_targetingConeAngle;

		private bool m_lootOnDestruction;
		private bool m_bunker;
		private bool m_upgradesUnits;
		private bool m_freeBoost;
		private bool m_randomHitPosition;
		private bool m_canNotSellLast;
		private bool m_locked;
		private bool m_hidden;
		private bool m_forgesSpells;
		private bool m_forgesMiniSpells;
		private bool m_isHeroBarrack;
		private bool m_needsAim;
		private bool m_shareHeroCombatData;
		private bool m_isRed;
		private bool m_selfAsAoeCenter;
		private bool m_isClockTower;
		private bool m_isFlamer;
		private bool m_isBarrackVillage2;

		private string m_exportNameNpc;
		private string m_exportNameConstruction;
		private string m_exportNameLocked;
		private string m_exportNameBeamStart;
		private string m_exportNameBeamEnd;

		public LogicBuildingData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicBuildingData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_upgradeLevelCount = m_row.GetBiggestArraySize();
			m_buildingClass = LogicDataTables.GetBuildingClassByName(GetValue("BuildingClass", 0), this);

			if (m_buildingClass == null)
			{
				Debugger.Error("Building class is not defined for " + GetName());
			}

			m_secondaryTargetingClass = LogicDataTables.GetBuildingClassByName(GetValue("SecondaryTargetingClass", 0), this);
			m_shopBuildingClass = LogicDataTables.GetBuildingClassByName(GetValue("ShopBuildingClass", 0), this);

			if (m_shopBuildingClass == null)
			{
				m_shopBuildingClass = m_buildingClass;
			}

			m_exportNameNpc = GetValue("ExportNameNpc", 0);
			m_exportNameConstruction = GetValue("ExportNameConstruction", 0);
			m_exportNameLocked = GetValue("ExportNameLocked", 0);
			m_width = GetIntegerValue("Width", 0);
			m_height = GetIntegerValue("Height", 0);
			m_lootOnDestruction = GetBooleanValue("LootOnDestruction", 0);
			m_bunker = GetBooleanValue("Bunker", 0);
			m_village2Housing = GetIntegerValue("Village2Housing", 0);
			m_upgradesUnits = GetBooleanValue("UpgradesUnits", 0);
			m_producesUnitsOfType = GetIntegerValue("ProducesUnitsOfType", 0);
			m_freeBoost = GetBooleanValue("FreeBoost", 0);
			m_randomHitPosition = GetBooleanValue("RandomHitPosition", 0);
			m_chainAttackDistance = GetIntegerValue("ChainAttackDistance", 0);
			m_buildingW = GetIntegerValue("BuildingW", 0);
			m_buildingH = GetIntegerValue("BuildingH", 0);

			if (m_buildingW == 0)
			{
				m_buildingW = m_width;
			}

			if (m_buildingH == 0)
			{
				m_buildingH = m_height;
			}

			m_baseGfx = GetIntegerValue("BaseGfx", 0);
			m_loadAmmoEffect = LogicDataTables.GetEffectByName(GetValue("LoadAmmoEffect", 0), this);
			m_noAmmoEffect = LogicDataTables.GetEffectByName(GetValue("NoAmmoEffect", 0), this);
			m_toggleAttackModeEffect = LogicDataTables.GetEffectByName(GetValue("ToggleAttackModeEffect", 0), this);
			m_pickUpEffect = LogicDataTables.GetEffectByName(GetValue("PickUpEffect", 0), this);
			m_placingEffect = LogicDataTables.GetEffectByName(GetValue("PlacingEffect", 0), this);
			m_canNotSellLast = GetBooleanValue("CanNotSellLast", 0);
			m_locked = GetBooleanValue("Locked", 0);
			m_startingHomeCount = GetIntegerValue("StartingHomeCount", 0);
			m_hidden = GetBooleanValue("Hidden", 0);
			m_triggerRadius = (GetIntegerValue("TriggerRadius", 0) << 9) / 100;
			m_appearEffect = LogicDataTables.GetEffectByName(GetValue("AppearEffect", 0), this);
			m_forgesSpells = GetBooleanValue("ForgesSpells", 0);
			m_forgesMiniSpells = GetBooleanValue("ForgesMiniSpells", 0);
			m_isHeroBarrack = GetBooleanValue("IsHeroBarrack", 0);
			m_aimRotateStep = GetIntegerValue("AimRotateStep", 0);
			m_turnSpeed = GetIntegerValue("TurnSpeed", 0);

			if (m_turnSpeed == 0)
			{
				m_turnSpeed = 500;
			}

			m_needsAim = GetBooleanValue("NeedsAim", 0);
			m_exportNameBeamStart = GetValue("ExportNameBeamStart", 0);
			m_exportNameBeamEnd = GetValue("ExportNameBeamEnd", 0);
			m_shareHeroCombatData = GetBooleanValue("ShareHeroCombatData", 0);
			m_dieDamageRadius = (GetIntegerValue("DieDamageRadius", 0) << 9) / 100;
			m_dieDamageEffect = LogicDataTables.GetEffectByName(GetValue("DieDamageEffect", 0), this);
			m_dieDamageDelay = GetIntegerValue("DieDamageDelay", 0);

			if (m_dieDamageDelay > 4000)
			{
				Debugger.Warning("m_dieDamageDelay too big");
				m_dieDamageDelay = 4000;
			}

			m_isRed = GetBooleanValue("IsRed", 0);
			m_redMul = GetIntegerValue("RedMul", 0);
			m_greenMul = GetIntegerValue("GreenMul", 0);
			m_blueMul = GetIntegerValue("BlueMul", 0);
			m_redAdd = GetIntegerValue("RedAdd", 0);
			m_greenAdd = GetIntegerValue("GreenAdd", 0);
			m_blueAdd = GetIntegerValue("BlueAdd", 0);

			m_selfAsAoeCenter = GetBooleanValue("SelfAsAoeCenter", 0);
			m_newTargetAttackDelay = GetIntegerValue("NewTargetAttackDelay", 0);
			m_gearUpLevelRequirement = GetIntegerValue("GearUpLevelRequirement", 0);
			m_bunker = GetBooleanValue("Bunker", 0);

			int longestArraySize = m_row.GetBiggestArraySize();

			m_buildResourceData = new LogicResourceData[longestArraySize];
			m_altBuildResourceData = new LogicResourceData[longestArraySize];
			m_storedResourceCounts = new LogicArrayList<int>[longestArraySize];
			m_percentageStoredResourceCounts = new LogicArrayList<int>[longestArraySize];
			m_ammoResourceData = new LogicResourceData[longestArraySize];
			m_attackItemData = new LogicArrayList<LogicAttackerItemData>(longestArraySize);
			m_defenceTroopCharacter = new LogicCharacterData[longestArraySize];
			m_defenceTroopCharacter2 = new LogicCharacterData[longestArraySize];

			m_buildCost = new int[longestArraySize];
			m_ammoCost = new int[longestArraySize];
			m_townHallLevel = new int[longestArraySize];
			m_townHallVillage2Level = new int[longestArraySize];
			m_constructionTimes = new int[longestArraySize];
			m_gearUpTime = new int[longestArraySize];
			m_gearUpCost = new int[longestArraySize];
			m_boostCost = new int[longestArraySize];
			m_housingSpace = new int[longestArraySize];
			m_housingSpaceAlt = new int[longestArraySize];
			m_resourcePer100Hours = new int[longestArraySize];
			m_resourceMax = new int[longestArraySize];
			m_resourceIconLimit = new int[longestArraySize];
			m_hitpoints = new int[longestArraySize];
			m_regenTime = new int[longestArraySize];
			m_amountCanBeUpgraded = new int[longestArraySize];
			m_unitProduction = new int[longestArraySize];
			m_strengthWeight = new int[longestArraySize];
			m_destructionXP = new int[longestArraySize];
			m_defenceTroopCount = new int[longestArraySize];
			m_defenceTroopLevel = new int[longestArraySize];
			m_dieDamage = new int[longestArraySize];
			m_wallBlockX = new int[0];
			m_wallBlockY = new int[0];

			for (int i = 0; i < longestArraySize; i++)
			{
				LogicAttackerItemData itemData = new LogicAttackerItemData();
				itemData.CreateReferences(m_row, this, i);
				m_attackItemData.Add(itemData);

				m_dieDamage[i] = GetClampedIntegerValue("DieDamage", i);
				m_buildCost[i] = GetClampedIntegerValue("BuildCost", i);
				m_housingSpace[i] = GetClampedIntegerValue("HousingSpace", i);
				m_housingSpaceAlt[i] = GetClampedIntegerValue("HousingSpaceAlt", i);
				m_unitProduction[i] = GetClampedIntegerValue("UnitProduction", i);
				m_gearUpCost[i] = GetClampedIntegerValue("GearUpCost", i);
				m_boostCost[i] = GetClampedIntegerValue("BoostCost", i);
				m_resourcePer100Hours[i] = GetClampedIntegerValue("ResourcePer100Hours", i);
				m_resourceMax[i] = GetClampedIntegerValue("ResourceMax", i);
				m_resourceIconLimit[i] = GetClampedIntegerValue("ResourceIconLimit", i);
				m_hitpoints[i] = GetClampedIntegerValue("Hitpoints", i);
				m_regenTime[i] = GetClampedIntegerValue("RegenTime", i);
				m_amountCanBeUpgraded[i] = GetClampedIntegerValue("AmountCanBeUpgraded", i);
				m_buildResourceData[i] = LogicDataTables.GetResourceByName(GetClampedValue("BuildResource", i), this);
				m_altBuildResourceData[i] = LogicDataTables.GetResourceByName(GetClampedValue("AltBuildResource", i), this);
				m_townHallLevel[i] = LogicMath.Max(GetClampedIntegerValue("TownHallLevel", i) - 1, 0);
				m_townHallVillage2Level[i] = LogicMath.Max(GetClampedIntegerValue("TownHallLevel2", i) - 1, 0);
				m_storedResourceCounts[i] = new LogicArrayList<int>();
				m_percentageStoredResourceCounts[i] = new LogicArrayList<int>();

				LogicDataTable table = LogicDataTables.GetTable(DataType.RESOURCE);

				for (int j = 0; j < table.GetItemCount(); j++)
				{
					m_storedResourceCounts[i].Add(GetClampedIntegerValue("MaxStored" + table.GetItemAt(j).GetName(), i));
					m_percentageStoredResourceCounts[i].Add(GetClampedIntegerValue("PercentageStored" + table.GetItemAt(j).GetName(), i));
				}

				m_gearUpTime[i] = 60 * GetClampedIntegerValue("GearUpTime", i);
				m_constructionTimes[i] = 86400 * GetClampedIntegerValue("BuildTimeD", i) +
											  3600 * GetClampedIntegerValue("BuildTimeH", i) +
											  60 * GetClampedIntegerValue("BuildTimeM", i) +
											  GetIntegerValue("BuildTimeS", i);
				m_destructionXP[i] = GetClampedIntegerValue("DestructionXP", i);
				m_ammoResourceData[i] = LogicDataTables.GetResourceByName(GetClampedValue("AmmoResource", i), this);
				m_ammoCost[i] = GetClampedIntegerValue("AmmoCost", i);
				m_strengthWeight[i] = GetClampedIntegerValue("StrengthWeight", i);

				string defenceTroopCharacter = GetClampedValue("DefenceTroopCharacter", i);

				if (defenceTroopCharacter.Length > 0)
				{
					m_defenceTroopCharacter[i] = LogicDataTables.GetCharacterByName(defenceTroopCharacter, this);
				}

				string defenceTroopCharacter2 = GetClampedValue("DefenceTroopCharacter2", i);

				if (defenceTroopCharacter2.Length > 0)
				{
					m_defenceTroopCharacter2[i] = LogicDataTables.GetCharacterByName(defenceTroopCharacter2, this);
				}

				m_defenceTroopCount[i] = GetIntegerValue("DefenceTroopCount", i);
				m_defenceTroopLevel[i] = GetIntegerValue("DefenceTroopLevel", i);

				if (i > 0 && m_housingSpace[i] < m_housingSpace[i - 1])
					Debugger.Error("Building " + GetName() + " unit storage space decreases by upgrade level!");
				if (m_gearUpCost[i] > 0 && m_gearUpTime[i] <= 0 || m_gearUpCost[i] <= 0 && m_gearUpTime[i] > 0)
					Debugger.Error("invalid gear up settings. gear up time and cost must be set for levels where available");
			}

			m_areaOfEffectSpellData = LogicDataTables.GetSpellByName(GetValue("AOESpell", 0), this);
			m_alternativeAreaOfEffectSpellData = LogicDataTables.GetSpellByName(GetValue("AOESpellAlternate", 0), this);
			m_produceResourceData = LogicDataTables.GetResourceByName(GetValue("ProducesResource", 0), this);
			m_gearUpResourceData = LogicDataTables.GetResourceByName(GetValue("GearUpResource", 0), this);

			string heroType = GetValue("HeroType", 0);

			if (!string.IsNullOrEmpty(heroType))
			{
				m_heroData = LogicDataTables.GetHeroByName(heroType, this);
			}

			string wallBlockX = GetValue("WallBlockX", 0);

			if (wallBlockX.Length > 0)
			{
				LoadWallBlock(wallBlockX, out m_wallBlockX);
				LoadWallBlock(GetValue("WallBlockY", 0), out m_wallBlockY);

				if (m_wallBlockX.Length != m_wallBlockY.Length)
				{
					Debugger.Error("LogicBuildingData: Error parsing wall offsets");
				}

				if (m_wallBlockX.Length > 10)
				{
					Debugger.Error("LogicBuildingData: Too many wall blocks");
				}
			}

			string gearUpBuilding = GetValue("GearUpBuilding", 0);

			if (gearUpBuilding.Length > 0)
			{
				m_gearUpBuildingData = LogicDataTables.GetBuildingByName(gearUpBuilding, this);
			}

			m_isClockTower = GetName().Equals("Clock Tower");
			m_isFlamer = GetName().Equals("Flamer");
			m_isBarrackVillage2 = GetName().Equals("Barrack2");
		}

		public void LoadWallBlock(string value, out int[] wallBlock)
		{
			string[] tmp = value.Split(',');
			wallBlock = new int[tmp.Length];

			for (int i = 0; i < tmp.Length; i++)
			{
				wallBlock[i] = int.Parse(tmp[i]);
			}
		}

		public int GetWallBlockIndex(int x, int y, int idx)
		{
			int wallBlockX = m_wallBlockX[idx];
			int wallBlockY = m_wallBlockY[idx];

			for (int i = 0; i < 4; i++)
			{
				if (x == wallBlockX && wallBlockY == y)
				{
					return i;
				}

				int tmp = x;

				x = -y;
				y = tmp;
			}

			return -1;
		}

		public LogicBuildingClassData GetBuildingClass()
			=> m_buildingClass;

		public LogicAttackerItemData GetAttackerItemData(int idx)
			=> m_attackItemData[idx];

		public int GetUpgradeLevelCount()
			=> m_upgradeLevelCount;

		public int GetConstructionTime(int upgLevel, LogicLevel level, int ignoreBuildingCnt)
		{
			if (GetVillage2Housing() < 1)
			{
				return m_constructionTimes[upgLevel];
			}

			return LogicDataTables.GetGlobals().GetTroopHousingBuildTimeVillage2(level, ignoreBuildingCnt);
		}

		public bool IsTownHall()
			=> m_buildingClass.IsTownHall();

		public bool IsTownHallVillage2()
			=> m_buildingClass.IsTownHall2();

		public bool IsWorkerBuilding()
			=> m_buildingClass.IsWorker();

		public bool IsWall()
			=> m_buildingClass.IsWall();

		public bool IsAllianceCastle()
			=> m_bunker;

		public bool IsLaboratory()
			=> m_upgradesUnits;

		public bool IsLocked()
			=> m_locked;

		public bool IsClockTower()
			=> m_isClockTower;

		public bool IsFlamer()
			=> m_isFlamer;

		public bool IsBarrackVillage2()
			=> m_isBarrackVillage2;

		public int GetUnitStorageCapacity(int level)
			=> m_housingSpace[level];

		public int GetAltUnitStorageCapacity(int level)
			=> m_housingSpaceAlt[level];

		public LogicResourceData GetGearUpResource()
			=> m_gearUpResourceData;

		public LogicResourceData GetBuildResource(int idx)
			=> m_buildResourceData[idx];

		public LogicResourceData GetAltBuildResource(int idx)
			=> m_altBuildResourceData[idx];

		public LogicResourceData GetProduceResource()
			=> m_produceResourceData;

		public LogicHeroData GetHeroData()
			=> m_heroData;

		public LogicBuildingData GetGearUpBuildingData()
			=> m_gearUpBuildingData;

		public LogicSpellData GetAreaOfEffectSpell()
			=> m_areaOfEffectSpellData;

		public LogicSpellData GetAltAreaOfEffectSpell()
			=> m_alternativeAreaOfEffectSpellData;

		public int GetBuildCost(int index, LogicLevel level)
		{
			if (GetVillage2Housing() <= 0)
			{
				if (m_buildingClass.IsWorker())
				{
					return LogicDataTables.GetGlobals().GetWorkerCost(level);
				}

				return m_buildCost[index];
			}

			return LogicDataTables.GetGlobals().GetTroopHousingBuildCostVillage2(level);
		}

		public int GetRequiredTownHallLevel(int index)
		{
			if (index != 0 || LogicDataTables.GetTownHallLevelCount() < 1)
			{
				return m_townHallLevel[index];
			}

			for (int i = 0; i < LogicDataTables.GetTownHallLevelCount(); i++)
			{
				LogicTownhallLevelData townHallLevel = LogicDataTables.GetTownHallLevel(i);

				if (townHallLevel.GetUnlockedBuildingCount(this) > 0)
				{
					return i;
				}
			}

			return m_townHallLevel[index];
		}

		public int GetTownHallLevel2(int index)
			=> m_townHallVillage2Level[index];

		public int GetWidth()
			=> m_width;

		public int GetHeight()
			=> m_height;

		public bool StoresResources()
		{
			LogicArrayList<int> storeCount = m_storedResourceCounts[0];

			for (int i = 0; i < storeCount.Size(); i++)
			{
				if (storeCount[i] > 0)
				{
					return true;
				}
			}

			return false;
		}

		public int GetMaxStoredGold(int upgLevel)
			=> m_storedResourceCounts[upgLevel][LogicDataTables.GetGoldData().GetInstanceID()];

		public int GetMaxStoredElixir(int upgLevel)
			=> m_storedResourceCounts[upgLevel][LogicDataTables.GetElixirData().GetInstanceID()];

		public int GetMaxStoredDarkElixir(int upgLevel)
			=> m_storedResourceCounts[upgLevel][LogicDataTables.GetDarkElixirData().GetInstanceID()];

		public int GetMaxUpgradeLevelForTownHallLevel(int townHallLevel)
		{
			int count = m_upgradeLevelCount;

			while (count > 0)
			{
				if (GetRequiredTownHallLevel(--count) <= townHallLevel)
				{
					return count;
				}
			}

			return -1;
		}

		public int GetMinUpgradeLevelForGearUp()
		{
			int count = m_upgradeLevelCount;

			for (int i = 0; i < count; i++)
			{
				if (m_gearUpCost[i] > 0)
				{
					return i;
				}
			}

			return -1;
		}

		public LogicArrayList<int> GetMaxStoredResourceCounts(int idx)
			=> m_storedResourceCounts[idx];

		public LogicArrayList<int> GetMaxPercentageStoredResourceCounts(int idx)
			=> m_percentageStoredResourceCounts[idx];

		public int GetResourcePer100Hours(int index)
			=> m_resourcePer100Hours[index];

		public int GetResourceMax(int index)
			=> m_resourceMax[index];

		public int GetResourceIconLimit(int index)
			=> m_resourceIconLimit[index];

		public int GetBoostCost(int index)
			=> m_boostCost[index];

		public int GetHitpoints(int index)
			=> m_hitpoints[index];

		public int GetRegenerationTime(int index)
			=> m_regenTime[index];

		public int GetAmmoCost(int index, int count)
		{
			if (count < 1)
			{
				return 0;
			}

			return LogicMath.Max(m_ammoCost[index] * count / m_attackItemData[index].GetAmmoCount(), 1);
		}

		public LogicResourceData GetAmmoResourceData(int idx)
			=> m_ammoResourceData[idx];

		public int GetAmountCanBeUpgraded(int index)
			=> m_amountCanBeUpgraded[index];

		public int GetGearUpCost(int index)
			=> m_gearUpCost[index];

		public int GetGearUpTime(int index)
			=> m_gearUpTime[index];

		public int GetWallBlockX(int index)
			=> m_wallBlockX[index];

		public int GetWallBlockY(int index)
			=> m_wallBlockY[index];

		public int GetWallBlockCount()
			=> m_wallBlockX.Length;

		public string GetExportNameNpc()
			=> m_exportNameNpc;

		public string GetExportNameConstruction()
			=> m_exportNameConstruction;

		public string GetExportNameLocked()
			=> m_exportNameLocked;

		public bool IsLootOnDestruction()
			=> m_lootOnDestruction;

		public int GetVillage2Housing()
			=> m_village2Housing;

		public bool IsFreeBoost()
			=> m_freeBoost;

		public bool IsRandomHitPosition()
			=> m_randomHitPosition;

		public int GetChainAttackDistance()
			=> m_chainAttackDistance;

		public int GetBuildingW()
			=> m_buildingW;

		public int GetBuildingH()
			=> m_buildingH;

		public int GetBaseGfx()
			=> m_baseGfx;

		public LogicEffectData GetLoadAmmoEffect()
			=> m_loadAmmoEffect;

		public LogicEffectData GetNoAmmoEffect()
			=> m_noAmmoEffect;

		public LogicEffectData GetToggleAttackModeEffect()
			=> m_toggleAttackModeEffect;

		public LogicEffectData GetPickUpEffect()
			=> m_pickUpEffect;

		public LogicEffectData GetPlacingEffect()
			=> m_placingEffect;

		public bool IsCanNotSellLast()
			=> m_canNotSellLast;

		public int GetStartingHomeCount()
			=> m_startingHomeCount;

		public bool IsHidden()
			=> m_hidden;

		public int GetTriggerRadius()
			=> m_triggerRadius;

		public LogicEffectData GetAppearEffect()
			=> m_appearEffect;

		public bool IsForgesSpells()
			=> m_forgesSpells;

		public bool IsForgesMiniSpells()
			=> m_forgesMiniSpells;

		public int GetAimRotateStep()
			=> m_aimRotateStep;

		public int GetTurnSpeed()
			=> m_turnSpeed;

		public bool IsNeedsAim()
			=> m_needsAim;

		public string GetExportNameBeamStart()
			=> m_exportNameBeamStart;

		public string GetExportNameBeamEnd()
			=> m_exportNameBeamEnd;

		public bool GetShareHeroCombatData()
			=> m_shareHeroCombatData;

		public int GetDieDamageRadius()
			=> m_dieDamageRadius;

		public LogicEffectData GetDieDamageEffect()
			=> m_dieDamageEffect;

		public int GetDieDamage(int upgLevel)
			=> LogicGamePlayUtil.DPSToSingleHit(m_dieDamage[upgLevel], 1000);

		public int GetDieDamageDelay()
			=> m_dieDamageDelay;

		public int GetRedMul()
			=> m_redMul;

		public int GetGreenMul()
			=> m_greenMul;

		public int GetBlueMul()
			=> m_blueMul;

		public int GetRedAdd()
			=> m_redAdd;

		public int GetGreenAdd()
			=> m_greenAdd;

		public int GetBlueAdd()
			=> m_blueAdd;

		public LogicCharacterData GetDefenceTroopCharacter(int upgLevel)
			=> m_defenceTroopCharacter[upgLevel];

		public LogicCharacterData GetDefenceTroopCharacter2(int upgLevel)
			=> m_defenceTroopCharacter2[upgLevel];

		public int GetDefenceTroopCount(int upgLevel)
			=> m_defenceTroopCount[upgLevel];

		public int GetDefenceTroopLevel(int upgLevel)
			=> m_defenceTroopLevel[upgLevel];

		public bool IsSelfAsAoeCenter()
			=> m_selfAsAoeCenter;

		public int GetNewTargetAttackDelay()
			=> m_newTargetAttackDelay;

		public int GetGearUpLevelRequirement()
			=> m_gearUpLevelRequirement;

		public int GetProducesUnitsOfType()
			=> m_producesUnitsOfType;

		public bool IsHeroBarrack()
			=> m_isHeroBarrack;

		public bool IsRed()
			=> m_isRed;

		public int GetUnitProduction(int index)
			=> m_unitProduction[index];

		public int GetStrengthWeight(int index)
			=> m_strengthWeight[index];

		public int GetDestructionXP(int index)
			=> m_destructionXP[index];

		public LogicBuildingClassData GetSecondaryTargetingClass()
			=> m_secondaryTargetingClass;

		public LogicBuildingClassData GetShopBuildingClass()
			=> m_shopBuildingClass;
	}
}