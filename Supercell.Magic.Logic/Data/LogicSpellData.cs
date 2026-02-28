using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicSpellData : LogicCombatItemData
	{
		private int[] m_damage;
		private int[] m_troopDamagePermil;
		private int[] m_buildingDamagePermil;
		private int[] m_poisonDamage;
		private int[] m_executeHealthPermil;
		private int[] m_damagePermilMin;
		private int[] m_damageBoostPercent;
		private int[] m_preferredDamagePermilMin;
		private int[] m_boostTimeMS;
		private int[] m_speedBoost;
		private int[] m_speedBoost2;
		private int[] m_duplicateLifetime;
		private int[] m_jumpBoostMS;
		private int[] m_jumpHousingLimit;
		private int[] m_freezeTimeMS;
		private int[] m_duplicateHousing;
		private int[] m_unitsToSpawn;
		private int[] m_strengthWeight;
		private int[] m_radius;
		private int[] m_randomRadius;
		private int[] m_timeBetweenHitsMS;
		private int[] m_buildingDamageBoostPercent;
		private int[] m_shieldProjectileSpeed;
		private int[] m_shieldProjectileDamageMod;
		private int[] m_extraHealthPermil;
		private int[] m_extraHealthMin;
		private int[] m_extraHealthMax;
		private int[] m_attackSpeedBoost;
		private int[] m_invulnerabilityTime;
		private int[] m_maxUnitsHit;
		private int[] m_numberOfHits;
		private int[] m_spawnDuration;

		private bool m_randomRadiusAffectsOnlyGfx;
		private bool m_poisonIncreaseSlowly;
		private bool m_poisonAffectAir;
		private bool m_scaleByTownHall;
		private bool m_troopsOnly;
		private bool m_snapToGrid;
		private bool m_boostDefenders;
		private bool m_boostLinkedToPoison;
		private bool m_scaleDeployEffects;

		private int m_spawnFirstGroupSize;
		private int m_pauseCombatComponentMs;
		private int m_damageTHPercent;
		private int m_shrinkReduceSpeedRatio;
		private int m_shrinkHitpointsRatio;
		private int m_deployEffect2Delay;
		private int m_hitTimeMS;
		private int m_deployTimeMS;
		private int m_chargingTimeMS;
		private int m_spellForgeLevel;
		private int m_numObstacles;
		private int m_preferredTargetDamageMod;
		private int m_heroDamageMultiplier;

		private string m_targetInfoString;

		private LogicObstacleData m_spawnObstacle;
		private LogicData m_preferredTarget;
		private LogicCharacterData m_summonTroop;

		private LogicEffectData[] m_preDeployEffect;
		private LogicEffectData[] m_deployEffect;
		private LogicEffectData[] m_deployEffect2;
		private LogicEffectData[] m_enemyDeployEffect;
		private LogicEffectData[] m_chargingEffect;
		private LogicEffectData[] m_hitEffect;

		public LogicSpellData(CSVRow row, LogicDataTable table) : base(row, table)
		{
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_damage = new int[m_upgradeLevelCount];
			m_troopDamagePermil = new int[m_upgradeLevelCount];
			m_buildingDamagePermil = new int[m_upgradeLevelCount];
			m_executeHealthPermil = new int[m_upgradeLevelCount];
			m_damagePermilMin = new int[m_upgradeLevelCount];
			m_preferredDamagePermilMin = new int[m_upgradeLevelCount];
			m_boostTimeMS = new int[m_upgradeLevelCount];
			m_speedBoost = new int[m_upgradeLevelCount];
			m_speedBoost2 = new int[m_upgradeLevelCount];
			m_damageBoostPercent = new int[m_upgradeLevelCount];
			m_duplicateLifetime = new int[m_upgradeLevelCount];
			m_duplicateHousing = new int[m_upgradeLevelCount];
			m_radius = new int[m_upgradeLevelCount];
			m_numberOfHits = new int[m_upgradeLevelCount];
			m_randomRadius = new int[m_upgradeLevelCount];
			m_timeBetweenHitsMS = new int[m_upgradeLevelCount];
			m_jumpBoostMS = new int[m_upgradeLevelCount];
			m_jumpHousingLimit = new int[m_upgradeLevelCount];
			m_freezeTimeMS = new int[m_upgradeLevelCount];
			m_strengthWeight = new int[m_upgradeLevelCount];
			m_buildingDamageBoostPercent = new int[m_upgradeLevelCount];
			m_shieldProjectileSpeed = new int[m_upgradeLevelCount];
			m_shieldProjectileDamageMod = new int[m_upgradeLevelCount];
			m_extraHealthPermil = new int[m_upgradeLevelCount];
			m_extraHealthMin = new int[m_upgradeLevelCount];
			m_extraHealthMax = new int[m_upgradeLevelCount];
			m_poisonDamage = new int[m_upgradeLevelCount];
			m_attackSpeedBoost = new int[m_upgradeLevelCount];
			m_invulnerabilityTime = new int[m_upgradeLevelCount];
			m_maxUnitsHit = new int[m_upgradeLevelCount];
			m_unitsToSpawn = new int[m_upgradeLevelCount];
			m_spawnDuration = new int[m_upgradeLevelCount];

			m_preDeployEffect = new LogicEffectData[m_upgradeLevelCount];
			m_deployEffect = new LogicEffectData[m_upgradeLevelCount];
			m_deployEffect2 = new LogicEffectData[m_upgradeLevelCount];
			m_enemyDeployEffect = new LogicEffectData[m_upgradeLevelCount];
			m_chargingEffect = new LogicEffectData[m_upgradeLevelCount];
			m_hitEffect = new LogicEffectData[m_upgradeLevelCount];

			for (int i = 0; i < m_upgradeLevelCount; i++)
			{
				m_damage[i] = LogicGamePlayUtil.DPSToSingleHit(GetClampedIntegerValue("Damage", i), 1000);
				m_troopDamagePermil[i] = GetClampedIntegerValue("TroopDamagePermil", i);
				m_buildingDamagePermil[i] = GetClampedIntegerValue("BuildingDamagePermil", i);
				m_executeHealthPermil[i] = GetClampedIntegerValue("ExecuteHealthPermil", i);
				m_damagePermilMin[i] = GetClampedIntegerValue("DamagePermilMin", i);
				m_preferredDamagePermilMin[i] = GetClampedIntegerValue("PreferredDamagePermilMin", i);
				m_boostTimeMS[i] = GetClampedIntegerValue("BoostTimeMS", i);
				m_speedBoost[i] = GetClampedIntegerValue("SpeedBoost", i);
				m_speedBoost2[i] = GetClampedIntegerValue("SpeedBoost2", i);
				m_damageBoostPercent[i] = GetClampedIntegerValue("DamageBoostPercent", i);
				m_duplicateLifetime[i] = GetClampedIntegerValue("DuplicateLifetime", i);
				m_duplicateHousing[i] = GetClampedIntegerValue("DuplicateHousing", i);
				m_radius[i] = (GetClampedIntegerValue("Radius", i) << 9) / 100;
				m_numberOfHits[i] = GetClampedIntegerValue("NumberOfHits", i);
				m_randomRadius[i] = (GetClampedIntegerValue("RandomRadius", i) << 9) / 100;
				m_timeBetweenHitsMS[i] = GetClampedIntegerValue("TimeBetweenHitsMS", i);
				m_jumpBoostMS[i] = GetClampedIntegerValue("JumpBoostMS", i);
				m_jumpHousingLimit[i] = GetClampedIntegerValue("JumpHousingLimit", i);
				m_hitEffect[i] = LogicDataTables.GetEffectByName(GetClampedValue("HitEffect", i), this);
				m_chargingEffect[i] = LogicDataTables.GetEffectByName(GetClampedValue("ChargingEffect", i), this);
				m_preDeployEffect[i] = LogicDataTables.GetEffectByName(GetClampedValue("PreDeployEffect", i), this);
				m_deployEffect[i] = LogicDataTables.GetEffectByName(GetClampedValue("DeployEffect", i), this);
				m_enemyDeployEffect[i] = LogicDataTables.GetEffectByName(GetClampedValue("EnemyDeployEffect", i), this);
				m_deployEffect2[i] = LogicDataTables.GetEffectByName(GetClampedValue("DeployEffect2", i), this);
				m_freezeTimeMS[i] = GetClampedIntegerValue("FreezeTimeMS", i);
				m_strengthWeight[i] = GetClampedIntegerValue("StrengthWeight", i);
				m_buildingDamageBoostPercent[i] = GetClampedIntegerValue("BuildingDamageBoostPercent", i);
				m_shieldProjectileSpeed[i] = GetClampedIntegerValue("ShieldProjectileSpeed", i);
				m_shieldProjectileDamageMod[i] = GetClampedIntegerValue("ShieldProjectileDamageMod", i);
				m_extraHealthPermil[i] = GetClampedIntegerValue("ExtraHealthPermil", i);
				m_extraHealthMin[i] = GetClampedIntegerValue("ExtraHealthMin", i);
				m_extraHealthMax[i] = GetClampedIntegerValue("ExtraHealthMax", i);
				m_poisonDamage[i] = LogicGamePlayUtil.DPSToSingleHit(GetClampedIntegerValue("PoisonDPS", i), 1000);
				m_attackSpeedBoost[i] = GetClampedIntegerValue("AttackSpeedBoost", i);
				m_invulnerabilityTime[i] = GetClampedIntegerValue("InvulnerabilityTime", i);
				m_maxUnitsHit[i] = GetClampedIntegerValue("MaxUnitsHit", i);
				m_unitsToSpawn[i] = GetClampedIntegerValue("UnitsToSpawn", i);
				m_spawnDuration[i] = GetClampedIntegerValue("SpawnDuration", i);
			}

			m_poisonIncreaseSlowly = GetBooleanValue("PoisonIncreaseSlowly", 0);
			m_poisonAffectAir = GetBooleanValue("PoisonAffectAir", 0);
			m_spawnFirstGroupSize = GetIntegerValue("SpawnFirstGroupSize", 0);
			m_scaleByTownHall = GetBooleanValue("ScaleByTH", 0);
			m_pauseCombatComponentMs = GetIntegerValue("PauseCombatComponentsMs", 0);
			m_damageTHPercent = GetIntegerValue("DamageTHPercent", 0);

			if (m_damageTHPercent <= 0)
			{
				m_damageTHPercent = 100;
			}

			m_shrinkReduceSpeedRatio = GetIntegerValue("ShrinkReduceSpeedRatio", 0);
			m_shrinkHitpointsRatio = GetIntegerValue("ShrinkHitpointsRatio", 0);
			m_deployEffect2Delay = GetIntegerValue("DeployEffect2Delay", 0);
			m_hitTimeMS = GetIntegerValue("HitTimeMS", 0);
			m_deployTimeMS = GetIntegerValue("DeployTimeMS", 0);
			m_chargingTimeMS = GetIntegerValue("ChargingTimeMS", 0);
			m_spellForgeLevel = GetIntegerValue("SpellForgeLevel", 0) - 1;
			m_randomRadiusAffectsOnlyGfx = GetBooleanValue("RandomRadiusAffectsOnlyGfx", 0);
			m_spawnObstacle = LogicDataTables.GetObstacleByName(GetValue("SpawnObstacle", 0), this);
			m_numObstacles = GetIntegerValue("NumObstacles", 0);
			m_troopsOnly = GetBooleanValue("TroopsOnly", 0);
			m_targetInfoString = GetValue("TargetInfoString", 0);

			string preferredTarget = GetValue("PreferredTarget", 0);

			if (preferredTarget.Length != 0)
			{
				m_preferredTarget = LogicDataTables.GetBuildingClassByName(preferredTarget, null);

				if (m_preferredTarget == null)
				{
					m_preferredTarget = LogicDataTables.GetBuildingByName(preferredTarget, null);

					if (m_preferredTarget == null)
					{
						m_preferredTarget = LogicDataTables.GetCharacterByName(preferredTarget, null);

						if (m_preferredTarget == null)
						{
							Debugger.Warning(string.Format("CSV row ({0}) has an invalid reference ({1})", GetName(), preferredTarget));
						}
					}
				}
			}

			m_preferredTargetDamageMod = GetIntegerValue("PreferredTargetDamageMod", 0);

			if (m_preferredTargetDamageMod == 0)
			{
				m_preferredTargetDamageMod = 100;
			}

			m_heroDamageMultiplier = GetIntegerValue("HeroDamageMultiplier", 0);

			if (m_heroDamageMultiplier == 0)
			{
				m_heroDamageMultiplier = 100;
			}

			m_snapToGrid = GetBooleanValue("SnapToGrid", 0);
			m_boostDefenders = GetBooleanValue("BoostDefenders", 0);
			m_boostLinkedToPoison = GetBooleanValue("BoostLinkedToPoison", 0);
			m_scaleDeployEffects = GetBooleanValue("ScaleDeployEffects", 0);
			m_summonTroop = LogicDataTables.GetCharacterByName(GetValue("SummonTroop", 0), null);
		}

		public override int GetRequiredProductionHouseLevel()
			=> m_spellForgeLevel;

		public override bool IsUnlockedForProductionHouseLevel(int level)
			=> level >= m_spellForgeLevel;

		public override LogicBuildingData GetProductionHouseData()
			=> LogicDataTables.GetBuildingByName(GetUnitOfType() == 1 ? "Spell Forge" : "Mini Spell Factory", null);

		public bool IsDamageSpell()
			=> m_damage[0] > 0 || m_buildingDamagePermil[0] > 0 || m_troopDamagePermil[0] > 0 || m_poisonDamage[0] > 0;

		public bool IsBuildingDamageSpell()
			=> m_damage[0] > 0 || m_buildingDamagePermil[0] > 0;

		public bool GetRandomRadiusAffectsOnlyGfx()
			=> m_randomRadiusAffectsOnlyGfx;

		public bool GetPoisonIncreaseSlowly()
			=> m_poisonIncreaseSlowly;

		public bool GetPoisonAffectAir()
			=> m_poisonAffectAir;

		public bool IsScaleByTownHall()
			=> m_scaleByTownHall;

		public bool GetTroopsOnly()
			=> m_troopsOnly;

		public bool GetSnapToGrid()
			=> m_snapToGrid;

		public bool GetBoostDefenders()
			=> m_boostDefenders;

		public bool GetBoostLinkedToPoison()
			=> m_boostLinkedToPoison;

		public bool GetScaleDeployEffects()
			=> m_scaleDeployEffects;

		public int GetDamage(int upgLevel)
			=> m_damage[upgLevel];

		public int GetSpawnFirstGroupSize()
			=> m_spawnFirstGroupSize;

		public int GetPauseCombatComponentMs()
			=> m_pauseCombatComponentMs;

		public int GetDamageTHPercent()
			=> m_damageTHPercent;

		public int GetShrinkReduceSpeedRatio()
			=> m_shrinkReduceSpeedRatio;

		public int GetShrinkHitpointsRatio()
			=> m_shrinkHitpointsRatio;

		public int GetDeployEffect2Delay()
			=> m_deployEffect2Delay;

		public int GetHitTimeMS()
			=> m_hitTimeMS;

		public int GetDeployTimeMS()
			=> m_deployTimeMS;

		public int GetChargingTimeMS()
			=> m_chargingTimeMS;

		public int GetSpellForgeLevel()
			=> m_spellForgeLevel;

		public int GetNumObstacles()
			=> m_numObstacles;

		public int GetPreferredTargetDamageMod()
			=> m_preferredTargetDamageMod;

		public int GetHeroDamageMultiplier()
			=> m_heroDamageMultiplier;

		public string GetTargetInfoString()
			=> m_targetInfoString;

		public int GetBuildingDamagePermil(int upgLevel)
			=> m_buildingDamagePermil[upgLevel];

		public int GetTroopDamagePermil(int upgLevel)
			=> m_troopDamagePermil[upgLevel];

		public int GetExecuteHealthPermil(int upgLevel)
			=> m_executeHealthPermil[upgLevel];

		public int GetDamagePermilMin(int upgLevel)
			=> m_damagePermilMin[upgLevel];

		public int GetPreferredDamagePermilMin(int upgLevel)
			=> m_preferredDamagePermilMin[upgLevel];

		public int GetPoisonDamage(int upgLevel)
			=> m_poisonDamage[upgLevel];

		public int GetDamageBoostPercent(int upgLevel)
			=> m_damageBoostPercent[upgLevel];

		public int GetBuildingDamageBoostPercent(int upgLevel)
			=> m_buildingDamageBoostPercent[upgLevel];

		public int GetSpeedBoost(int upgLevel)
			=> m_speedBoost[upgLevel];

		public int GetSpeedBoost2(int upgLevel)
			=> m_speedBoost2[upgLevel];

		public int GetJumpBoostMS(int upgLevel)
			=> m_jumpBoostMS[upgLevel];

		public int GetJumpHousingLimit(int upgLevel)
			=> m_jumpHousingLimit[upgLevel];

		public int GetFreezeTimeMS(int upgLevel)
			=> m_freezeTimeMS[upgLevel];

		public int GetDuplicateHousing(int upgLevel)
			=> m_duplicateHousing[upgLevel];

		public int GetDuplicateLifetime(int upgLevel)
			=> m_duplicateLifetime[upgLevel];

		public int GetUnitsToSpawn(int upgLevel)
			=> m_unitsToSpawn[upgLevel];

		public int GetSpawnDuration(int upgLevel)
			=> m_spawnDuration[upgLevel];

		public int GetStrengthWeight(int upgLevel)
			=> m_strengthWeight[upgLevel];

		public int GetRandomRadius(int upgLevel)
			=> m_randomRadius[upgLevel];

		public int GetRadius(int upgLevel)
			=> m_radius[upgLevel];

		public int GetTimeBetweenHitsMS(int upgLevel)
			=> m_timeBetweenHitsMS[upgLevel];

		public int GetMaxUnitsHit(int upgLevel)
			=> m_maxUnitsHit[upgLevel];

		public int GetNumberOfHits(int upgLevel)
			=> m_numberOfHits[upgLevel];

		public int GetExtraHealthPermil(int upgLevel)
			=> m_extraHealthPermil[upgLevel];

		public int GetExtraHealthMin(int upgLevel)
			=> m_extraHealthMin[upgLevel];

		public int GetExtraHealthMax(int upgLevel)
			=> m_extraHealthMax[upgLevel];

		public int GetInvulnerabilityTime(int upgLevel)
			=> m_invulnerabilityTime[upgLevel];

		public int GetShieldProjectileSpeed(int upgLevel)
			=> m_shieldProjectileSpeed[upgLevel];

		public int GetShieldProjectileDamageMod(int upgLevel)
			=> m_shieldProjectileDamageMod[upgLevel];

		public int GetAttackSpeedBoost(int upgLevel)
			=> m_attackSpeedBoost[upgLevel];

		public int GetBoostTimeMS(int upgLevel)
			=> m_boostTimeMS[upgLevel];

		public LogicObstacleData GetSpawnObstacle()
			=> m_spawnObstacle;

		public LogicEffectData GetPreDeployEffect(int upgLevel)
			=> m_preDeployEffect[upgLevel];

		public LogicEffectData GetDeployEffect(int upgLevel)
			=> m_deployEffect[upgLevel];

		public LogicEffectData GetEnemyDeployEffect(int upgLevel)
			=> m_enemyDeployEffect[upgLevel];

		public LogicEffectData GetDeployEffect2(int upgLevel)
			=> m_deployEffect2[upgLevel];

		public LogicEffectData GetChargingEffect(int upgLevel)
			=> m_chargingEffect[upgLevel];

		public LogicEffectData GetHitEffect(int upgLevel)
			=> m_hitEffect[upgLevel];

		public LogicData GetPreferredTarget()
			=> m_preferredTarget;

		public LogicCharacterData GetSummonTroop()
			=> m_summonTroop;

		public override int GetCombatItemType()
			=> 1;
	}
}