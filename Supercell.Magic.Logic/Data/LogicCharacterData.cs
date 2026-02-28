using System;

using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicCharacterData : LogicCombatItemData
	{
		public const int SPECIAL_ABILITY_TYPE_START_RAGE = 0;
		public const int SPECIAL_ABILITY_TYPE_BIG_FIRST_HIT = 1;
		public const int SPECIAL_ABILITY_TYPE_START_CLOAK = 2;
		public const int SPECIAL_ABILITY_TYPE_SPEED_BOOST = 3;
		public const int SPECIAL_ABILITY_TYPE_DIE_DAMAGE = 4;
		public const int SPECIAL_ABILITY_TYPE_SPAWN_UNITS = 5;
		public const int SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE = 6;
		public const int SPECIAL_ABILITY_TYPE_RAGE_ALONE = 7;
		public const int SPECIAL_ABILITY_TYPE_RESPAWN_AS_CANNON = 8;

		private LogicCharacterData m_childTroopData;
		private LogicCharacterData m_secondaryTroopData;
		private LogicSpellData m_specialAbilitySpellData;
		private LogicObstacleData m_tombstoneData;
		private LogicEffectData m_moveStartsEffect;

		private LogicEffectData[] m_specialAbilityEffectData;
		private LogicEffectData[] m_deployEffectData;
		private LogicEffectData[] m_dieEffectData;
		private LogicEffectData[] m_dieEffect2Data;
		private LogicEffectData[] m_dieDamageEffectData;
		private LogicEffectData[] m_moveTrailEffectData;
		private LogicSpellData[] m_auraSpellData;
		private LogicSpellData[] m_retributionSpellData;

		private bool m_flying;
		private bool m_jumper;
		private bool m_isUnderground;
		private bool m_disableDonate;
		private bool m_isSecondaryTroop;
		private bool m_attackMultipleBuildings;
		private bool m_balloonGoblin;
		private bool m_randomizeSecSpawnDist;
		private bool m_attackOverWalls;
		private bool m_smoothJump;
		private bool m_scaleByTH;
		private bool m_triggersTraps;
		private bool m_boostedIfAlone;
		private bool m_pickNewTargetAfterPushback;

		private int m_speed;
		private int m_speedDecreasePerChildTroopLost;
		private int m_pushbackSpeed;
		private int m_hitEffectOffset;
		private int m_targetedEffectOffset;
		private int m_specialAbilityType;
		private int m_unlockedBarrackLevel;
		private int m_childTroopCount;
		private int m_dieDamageRadius;
		private int m_dieDamageDelay;
		private int m_secondarySpawnDistance;
		private int m_secondarySpawnOffset;
		private int m_maxTrainingCount;
		private int m_movementOffsetAmount;
		private int m_movementOffsetSpeed;
		private int m_spawnIdle;
		private int m_autoMergeDistance;
		private int m_autoMergeGroupSize;
		private int m_invisibilityRadius;
		private int m_healthReductionPerSecond;
		private int m_friendlyGroupWeight;
		private int m_enemyGroupWeight;
		private int m_chainShootingDistance;
		private int m_boostRadius;
		private int m_boostDamagePerfect;
		private int m_boostAttackSpeed;
		private int m_loseHpPerTick;
		private int m_loseHpInterval;

		protected int[] m_hitpoints;
		protected int[] m_unitsInCamp;
		protected int[] m_strengthWeight;
		protected int[] m_specialAbilityLevel;
		protected int[] m_specialAbilityAttribute;
		protected int[] m_specialAbilityAttribute2;
		protected int[] m_specialAbilityAttribute3;

		private int[] m_secondaryTroopCount;
		private int[] m_childTroopX;
		private int[] m_childTroopY;
		private int[] m_attackCount;
		private int[] m_abilityAttackCount;
		private int[] m_dieDamage;
		private int[] m_scale;
		private int[] m_auraSpellLevel;
		private int[] m_retributionSpellLevel;
		private int[] m_retributionSpellTriggerHealth;

		private string m_swf;
		private string m_specialAbilityName;
		private string m_specialAbilityInfo;
		private string m_bigPictureSWF;
		private string m_customDefenderIcon;
		private string m_auraTID;
		private string m_auraDescTID;
		private string m_auraBigPictureExportName;

		protected LogicAttackerItemData[] m_attackerItemData;

		public LogicCharacterData(CSVRow row, LogicDataTable table) : base(row, table)
		{
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			int upgradeLevelCount = GetUpgradeLevelCount();

			m_attackerItemData = new LogicAttackerItemData[upgradeLevelCount];
			m_specialAbilityEffectData = new LogicEffectData[upgradeLevelCount];
			m_auraSpellData = new LogicSpellData[upgradeLevelCount];
			m_retributionSpellData = new LogicSpellData[upgradeLevelCount];
			m_deployEffectData = new LogicEffectData[upgradeLevelCount];
			m_dieEffectData = new LogicEffectData[upgradeLevelCount];
			m_dieEffect2Data = new LogicEffectData[upgradeLevelCount];
			m_dieDamageEffectData = new LogicEffectData[upgradeLevelCount];
			m_moveTrailEffectData = new LogicEffectData[upgradeLevelCount];

			m_hitpoints = new int[upgradeLevelCount];
			m_secondaryTroopCount = new int[upgradeLevelCount];
			m_unitsInCamp = new int[upgradeLevelCount];
			m_strengthWeight = new int[upgradeLevelCount];
			m_specialAbilityLevel = new int[upgradeLevelCount];
			m_specialAbilityAttribute = new int[upgradeLevelCount];
			m_specialAbilityAttribute2 = new int[upgradeLevelCount];
			m_specialAbilityAttribute3 = new int[upgradeLevelCount];
			m_scale = new int[upgradeLevelCount];
			m_attackCount = new int[upgradeLevelCount];
			m_abilityAttackCount = new int[upgradeLevelCount];
			m_dieDamage = new int[upgradeLevelCount];
			m_auraSpellLevel = new int[upgradeLevelCount];
			m_retributionSpellLevel = new int[upgradeLevelCount];
			m_retributionSpellTriggerHealth = new int[upgradeLevelCount];

			m_childTroopX = new int[3];
			m_childTroopY = new int[3];

			for (int i = 0; i < m_hitpoints.Length; i++)
			{
				m_attackerItemData[i] = new LogicAttackerItemData();
				m_attackerItemData[i].CreateReferences(m_row, this, i);

				m_hitpoints[i] = GetClampedIntegerValue("Hitpoints", i);
				m_secondaryTroopCount[i] = GetClampedIntegerValue("SecondaryTroopCnt", i);
				m_unitsInCamp[i] = GetClampedIntegerValue("UnitsInCamp", i);
				m_specialAbilityLevel[i] = GetClampedIntegerValue("SpecialAbilityLevel", i);
				m_specialAbilityEffectData[i] = LogicDataTables.GetEffectByName(GetClampedValue("SpecialAbilityEffect", i), this);
				m_deployEffectData[i] = LogicDataTables.GetEffectByName(GetClampedValue("DeployEffect", i), this);
				m_dieEffectData[i] = LogicDataTables.GetEffectByName(GetClampedValue("DieEffect", i), this);
				m_dieEffect2Data[i] = LogicDataTables.GetEffectByName(GetClampedValue("DieEffect2", i), this);
				m_dieDamageEffectData[i] = LogicDataTables.GetEffectByName(GetClampedValue("DieDamageEffect", i), this);
				m_moveTrailEffectData[i] = LogicDataTables.GetEffectByName(GetClampedValue("MoveTrailEffect", i), this);
				m_attackCount[i] = GetClampedIntegerValue("AttackCount", i);
				m_abilityAttackCount[i] = GetClampedIntegerValue("AbilityAttackCount", i);
				m_dieDamage[i] = GetClampedIntegerValue("DieDamage", i);
				m_strengthWeight[i] = GetClampedIntegerValue("StrengthWeight", i);
				m_scale[i] = GetClampedIntegerValue("Scale", i);

				if (m_scale[i] == 0)
				{
					m_scale[i] = 100;
				}

				m_auraSpellData[i] = LogicDataTables.GetSpellByName(GetClampedValue("AuraSpell", i), this);
				m_auraSpellLevel[i] = GetClampedIntegerValue("AuraSpellLevel", i);
				m_retributionSpellData[i] = LogicDataTables.GetSpellByName(GetClampedValue("RetributionSpell", i), this);
				m_retributionSpellLevel[i] = GetClampedIntegerValue("RetributionSpellLevel", i);
				m_retributionSpellTriggerHealth[i] = GetClampedIntegerValue("RetributionSpellTriggerHealth", i);
				m_specialAbilityAttribute[i] = GetClampedIntegerValue("SpecialAbilityAttribute", i);
				m_specialAbilityAttribute2[i] = GetClampedIntegerValue("SpecialAbilityAttribute2", i);
				m_specialAbilityAttribute3[i] = GetClampedIntegerValue("SpecialAbilityAttribute3", i);
			}

			m_moveStartsEffect = LogicDataTables.GetEffectByName(GetValue("MoveStartsEffect", 0), this);
			m_specialAbilityType = GetSpecialAbilityTypeFromCSV();

			string specialAbilitySpell = GetValue("SpecialAbilitySpell", 0);

			if (specialAbilitySpell.Length > 0)
			{
				m_specialAbilitySpellData = LogicDataTables.GetSpellByName(specialAbilitySpell, this);
			}

			m_swf = GetValue("SWF", 0);
			m_specialAbilityName = GetValue("SpecialAbilityName", 0);
			m_specialAbilityInfo = GetValue("SpecialAbilityInfo", 0);
			m_bigPictureSWF = GetValue("BigPictureSWF", 0);
			m_dieDamageRadius = (GetIntegerValue("DieDamageRadius", 0) << 9) / 100;
			m_dieDamageDelay = GetIntegerValue("DieDamageDelay", 0);

			if (m_dieDamageDelay > 4000)
			{
				Debugger.Warning("m_dieDamageDelay too big");
				m_dieDamageDelay = 4000;
			}

			m_secondaryTroopData = LogicDataTables.GetCharacterByName(GetValue("SecondaryTroop", 0), this);
			m_isSecondaryTroop = GetBooleanValue("IsSecondaryTroop", 0);
			m_secondarySpawnDistance = (GetIntegerValue("SecondarySpawnDist", 0) << 9) / 100;
			m_secondarySpawnOffset = (GetIntegerValue("SecondarySpawnOffset", 0) << 9) / 100;
			m_tombstoneData = LogicDataTables.GetObstacleByName(GetValue("TombStone", 0), this);
			m_maxTrainingCount = GetIntegerValue("MaxTrainingCount", 0);
			m_unlockedBarrackLevel = GetIntegerValue("BarrackLevel", 0) - 1;
			m_flying = GetBooleanValue("IsFlying", 0);
			m_jumper = GetBooleanValue("IsJumper", 0);
			m_movementOffsetAmount = GetIntegerValue("MovementOffsetAmount", 0);
			m_movementOffsetSpeed = GetIntegerValue("MovementOffsetSpeed", 0);
			m_balloonGoblin = GetName().Equals("Balloon Goblin");
			m_spawnIdle = GetIntegerValue("SpawnIdle", 0);
			m_childTroopData = LogicDataTables.GetCharacterByName(GetValue("ChildTroop", 0), this);
			m_childTroopCount = GetIntegerValue("ChildTroopCount", 0);

			for (int i = 0; i < 3; i++)
			{
				m_childTroopX[i] = GetIntegerValue(string.Format("ChildTroop{0}_X", i), 0);
				m_childTroopY[i] = GetIntegerValue(string.Format("ChildTroop{0}_Y", i), 0);
			}

			m_attackMultipleBuildings = GetBooleanValue("AttackMultipleBuildings", 0);

			if (!m_attackerItemData[0].IsSelfAsAoeCenter())
			{
				m_attackMultipleBuildings = false;
			}

			m_speed = (GetIntegerValue("Speed", 0) << 9) / 100;
			m_speedDecreasePerChildTroopLost = (GetIntegerValue("SpeedDecreasePerChildTroopLost", 0) << 9) / 100;
			m_pickNewTargetAfterPushback = GetBooleanValue("PickNewTargetAfterPushback", 0);
			m_pushbackSpeed = GetIntegerValue("PushbackSpeed", 0);
			m_hitEffectOffset = GetIntegerValue("HitEffectOffset", 0);
			m_targetedEffectOffset = (GetIntegerValue("TargetedEffectOffset", 0) << 9) / 100;
			m_randomizeSecSpawnDist = GetBooleanValue("RandomizeSecSpawnDist", 0);
			m_customDefenderIcon = GetValue("CustonDefenderIcon", 0);
			m_autoMergeDistance = (GetIntegerValue("AutoMergeDistance", 0) << 9) / 100;
			m_autoMergeGroupSize = GetIntegerValue("AutoMergeGroupSize", 0);
			m_invisibilityRadius = (GetIntegerValue("InvisibilityRadius", 0) << 9) / 100;
			m_healthReductionPerSecond = GetIntegerValue("HealthReductionPerSecond", 0);
			m_isUnderground = GetBooleanValue("IsUnderground", 0);
			m_attackOverWalls = GetBooleanValue("NoAttackOverWalls", 0) ^ true;
			m_smoothJump = GetBooleanValue("SmoothJump", 0);
			m_auraTID = GetValue("AuraTID", 0);
			m_auraDescTID = GetValue("AuraDescTID", 0);
			m_auraBigPictureExportName = GetValue("AuraBigPictureExportName", 0);
			m_friendlyGroupWeight = GetIntegerValue("FriendlyGroupWeight", 0);
			m_enemyGroupWeight = GetIntegerValue("EnemyGroupWeight", 0);
			m_scaleByTH = GetBooleanValue("ScaleByTH", 0);
			m_disableDonate = GetBooleanValue("DisableDonate", 0);
			m_loseHpPerTick = GetIntegerValue("LoseHpPerTick", 0);
			m_loseHpInterval = GetIntegerValue("LoseHpInterval", 0);
			m_triggersTraps = GetBooleanValue("TriggersTraps", 0);
			m_chainShootingDistance = GetIntegerValue("ChainShootingDistance", 0);
			m_boostedIfAlone = GetBooleanValue("BoostedIfAlone", 0);
			m_boostRadius = (GetIntegerValue("BoostRadius", 0) << 9) / 100;
			m_boostDamagePerfect = GetIntegerValue("BoostDmgPerfect", 0);
			m_boostAttackSpeed = GetIntegerValue("BoostAttackSpeed", 0);
		}

		public override void CreateReferences2()
		{
			if (m_tombstoneData != null)
			{
				if (!m_tombstoneData.IsEnabledInVillageType(GetVillageType()))
				{
					Debugger.Error(string.Format("invalid tombstone for unit '{0}' villageType's do not match", GetName()));
				}
			}
		}

		private int GetSpecialAbilityTypeFromCSV()
		{
			string name = GetValue("SpecialAbilityType", 0);

			if (string.Equals(name, "StartRage", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_START_RAGE;
			}

			if (string.Equals(name, "BigFirstHit", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_BIG_FIRST_HIT;
			}

			if (string.Equals(name, "StartCloak", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_START_CLOAK;
			}

			if (string.Equals(name, "SpeedBoost", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_SPEED_BOOST;
			}

			if (string.Equals(name, "DieDamage", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_DIE_DAMAGE;
			}

			if (string.Equals(name, "SpawnUnits", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_SPAWN_UNITS;
			}

			if (string.Equals(name, "SpecialProjectile", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE;
			}

			if (string.Equals(name, "RageAlone", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_RAGE_ALONE;
			}

			if (string.Equals(name, "RespawnAsCannon", StringComparison.InvariantCultureIgnoreCase))
			{
				return LogicCharacterData.SPECIAL_ABILITY_TYPE_RESPAWN_AS_CANNON;
			}

			return -1;
		}

		public int GetSpecialAbilityType()
			=> m_specialAbilityType;

		public override int GetRequiredProductionHouseLevel()
			=> m_unlockedBarrackLevel;

		public override bool IsDonationDisabled()
			=> m_disableDonate;

		public override bool IsUnlockedForProductionHouseLevel(int level)
			=> level >= m_unlockedBarrackLevel;

		public override LogicBuildingData GetProductionHouseData()
		{
			string buildingName = GetVillageType() == 1 ? GetUnitOfType() == 1 ? "Barrack2" : "Dark Elixir Barrack2" :
				GetUnitOfType() == 1 ? "Barrack" : "Dark Elixir Barrack";

			return LogicDataTables.GetBuildingByName(buildingName, null);
		}

		public LogicAttackerItemData GetAttackerItemData(int idx)
			=> m_attackerItemData[idx];

		public override bool IsUnderground()
			=> m_isUnderground;

		public int GetHitpoints(int index)
			=> m_hitpoints[index];

		public int GetUnitsInCamp(int index)
			=> m_unitsInCamp[index];

		public bool IsSecondaryTroop()
			=> m_isSecondaryTroop;

		public int GetSpeed()
			=> m_speed;

		public int GetSpecialAbilityLevel(int upgLevel)
			=> m_specialAbilityLevel[upgLevel];

		public int GetSpecialAbilityAttribute(int upgLevel)
			=> m_specialAbilityAttribute[upgLevel];

		public int GetSpecialAbilityAttribute2(int upgLevel)
			=> m_specialAbilityAttribute2[upgLevel];

		public int GetSpecialAbilityAttribute3(int upgLevel)
			=> m_specialAbilityAttribute3[upgLevel];

		public int GetAbilityAttackCount(int upgLevel)
			=> m_abilityAttackCount[upgLevel];

		public LogicCharacterData GetChildTroop()
			=> m_childTroopData;

		public int GetChildTroopCount()
			=> m_childTroopCount;

		public int GetChildTroopX(int idx)
			=> m_childTroopX[idx];

		public int GetChildTroopY(int idx)
			=> m_childTroopY[idx];

		public bool GetAttackMultipleBuildings()
			=> m_attackMultipleBuildings;

		public LogicCharacterData GetSecondaryTroop()
			=> m_secondaryTroopData;

		public LogicSpellData GetSpecialAbilitySpell()
			=> m_specialAbilitySpellData;

		public LogicObstacleData GetTombstone()
			=> m_tombstoneData;

		public LogicEffectData GetMoveStartsEffect()
			=> m_moveStartsEffect;

		public bool GetRandomizeSecSpawnDist()
			=> m_randomizeSecSpawnDist;

		public bool GetAttackOverWalls()
			=> m_attackOverWalls;

		public bool GetSmoothJump()
			=> m_smoothJump;

		public bool GetScaleByTH()
			=> m_scaleByTH;

		public bool GetTriggersTraps()
			=> m_triggersTraps;

		public bool GetBoostedIfAlone()
			=> m_boostedIfAlone;

		public bool GetPickNewTargetAfterPushback()
			=> m_pickNewTargetAfterPushback;

		public int GetSpeedDecreasePerChildTroopLost()
			=> m_speedDecreasePerChildTroopLost;

		public int GetPushbackSpeed()
			=> m_pushbackSpeed;

		public int GetHitEffectOffset()
			=> m_hitEffectOffset;

		public int GetTargetedEffectOffset()
			=> m_targetedEffectOffset;

		public int GetDieDamageRadius()
			=> m_dieDamageRadius;

		public int GetDieDamage(int upgLevel)
			=> LogicGamePlayUtil.DPSToSingleHit(m_dieDamage[upgLevel], 1000);

		public int GetDieDamageDelay()
			=> m_dieDamageDelay;

		public LogicEffectData GetDieEffect(int upgLevel)
			=> m_dieEffectData[upgLevel];

		public LogicEffectData GetDieEffect2(int upgLevel)
			=> m_dieEffect2Data[upgLevel];

		public LogicEffectData GetSpecialAbilityEffect(int upgLevel)
			=> m_specialAbilityEffectData[upgLevel];

		public int GetSecondarySpawnDistance()
			=> m_secondarySpawnDistance;

		public int GetSecondarySpawnOffset()
			=> m_secondarySpawnOffset;

		public int GetMaxTrainingCount()
			=> m_maxTrainingCount;

		public int GetMovementOffsetAmount()
			=> m_movementOffsetAmount;

		public int GetMovementOffsetSpeed()
			=> m_movementOffsetSpeed;

		public int GetSpawnIdle()
			=> m_spawnIdle;

		public int GetAutoMergeDistance()
			=> m_autoMergeDistance;

		public int GetAutoMergeGroupSize()
			=> m_autoMergeGroupSize;

		public int GetInvisibilityRadius()
			=> m_invisibilityRadius;

		public int GetHealthReductionPerSecond()
			=> m_healthReductionPerSecond;

		public int GetFriendlyGroupWeight()
			=> m_friendlyGroupWeight;

		public int GetEnemyGroupWeight()
			=> m_enemyGroupWeight;

		public int GetChainShootingDistance()
			=> m_chainShootingDistance;

		public int GetBoostRadius()
			=> m_boostRadius;

		public int GetBoostDamagePerfect()
			=> m_boostDamagePerfect;

		public int GetBoostAttackSpeed()
			=> m_boostAttackSpeed;

		public int GetLoseHpPerTick()
			=> m_loseHpPerTick;

		public int GetLoseHpInterval()
			=> m_loseHpInterval;

		public string GetSwf()
			=> m_swf;

		public string GetSpecialAbilityName()
			=> m_specialAbilityName;

		public string GetSpecialAbilityInfo()
			=> m_specialAbilityInfo;

		public string GetBigPictureSWF()
			=> m_bigPictureSWF;

		public string GetCustomDefenderIcon()
			=> m_customDefenderIcon;

		public string GetAuraTID()
			=> m_auraTID;

		public LogicSpellData GetAuraSpell(int upgLevel)
			=> m_auraSpellData[upgLevel];

		public int GetAuraSpellLevel(int upgLevel)
			=> m_auraSpellLevel[upgLevel];

		public LogicSpellData GetRetributionSpell(int upgLevel)
			=> m_retributionSpellData[upgLevel];

		public int GetRetributionSpellLevel(int upgLevel)
			=> m_retributionSpellLevel[upgLevel];

		public int GetRetributionSpellTriggerHealth(int upgLevel)
			=> m_retributionSpellTriggerHealth[upgLevel];

		public string GetAuraDescTID()
			=> m_auraDescTID;

		public string GetAuraBigPictureExportName()
			=> m_auraBigPictureExportName;

		public bool IsFlying()
			=> m_flying;

		public bool IsJumper()
			=> m_jumper;

		public bool IsBalloonGoblin()
			=> m_balloonGoblin;

		public bool IsUnlockedForBarrackLevel(int barrackLevel)
			=> m_unlockedBarrackLevel != -1 && m_unlockedBarrackLevel <= barrackLevel;

		public int GetDonateXP()
			=> GetHousingSpace();

		public int GetStrengthWeight(int upgLevel)
			=> m_strengthWeight[upgLevel];

		public int GetSecondaryTroopCount(int upgLevel)
			=> m_secondaryTroopCount[upgLevel];

		public int GetAttackCount(int upgLevel)
			=> m_attackCount[upgLevel];

		public override int GetCombatItemType()
			=> LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER;
	}
}