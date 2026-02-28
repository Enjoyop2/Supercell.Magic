using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Data
{
	public class LogicTrapData : LogicGameObjectData
	{
		private bool m_enableByCalendar;
		private bool m_hasAltMode;
		private bool m_ejectVictims;
		private bool m_doNotScalePushByDamage;
		private bool m_airTrigger;
		private bool m_groundTrigger;
		private bool m_healerTrigger;

		private int m_upgradeLevelCount;
		private int m_preferredTargetDamageMod;
		private int m_minTriggerHousingLimit;
		private int m_timeBetweenSpawnsMS;
		private int m_spawnInitialDelayMS;
		private int m_throwDistance;
		private int m_triggerRadius;
		private int m_directionCount;
		private int m_actionFrame;
		private int m_pushback;
		private int m_speedMod;
		private int m_damageMod;
		private int m_durationMS;
		private int m_hitDelayMS;
		private int m_hitCount;

		private int[] m_constructionTimes;
		private int[] m_townHallLevel;
		private int[] m_buildCost;
		private int[] m_rearmCost;
		private int[] m_strenghtWeight;
		private int[] m_damage;
		private int[] m_damageRadius;
		private int[] m_ejectHousingLimit;
		private int[] m_numSpawns;

		private int m_width;
		private int m_height;

		private LogicProjectileData[] m_projectileData;

		private LogicSpellData m_spell;
		private LogicEffectData m_effectData;
		private LogicEffectData m_effect2Data;
		private LogicEffectData m_effectBrokenData;
		private LogicEffectData m_damageEffectData;
		private LogicEffectData m_pickUpEffectData;
		private LogicEffectData m_placingEffectData;
		private LogicEffectData m_appearEffectData;
		private LogicEffectData m_toggleAttackModeEffectData;
		private LogicCharacterData m_preferredTargetData;
		private LogicCharacterData m_spawnedCharGroundData;
		private LogicCharacterData m_spawnedCharAirData;
		private LogicResourceData m_buildResourceData;

		public LogicTrapData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicTrapData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_width = GetIntegerValue("Width", 0);
			m_height = GetIntegerValue("Height", 0);

			m_upgradeLevelCount = m_row.GetBiggestArraySize();

			m_buildCost = new int[m_upgradeLevelCount];
			m_rearmCost = new int[m_upgradeLevelCount];
			m_townHallLevel = new int[m_upgradeLevelCount];
			m_strenghtWeight = new int[m_upgradeLevelCount];
			m_damage = new int[m_upgradeLevelCount];
			m_damageRadius = new int[m_upgradeLevelCount];
			m_ejectHousingLimit = new int[m_upgradeLevelCount];
			m_numSpawns = new int[m_upgradeLevelCount];
			m_constructionTimes = new int[m_upgradeLevelCount];

			m_projectileData = new LogicProjectileData[m_upgradeLevelCount];

			for (int i = 0; i < m_upgradeLevelCount; i++)
			{
				m_buildCost[i] = GetClampedIntegerValue("BuildCost", i);
				m_rearmCost[i] = GetClampedIntegerValue("RearmCost", i);
				m_townHallLevel[i] = LogicMath.Max(GetClampedIntegerValue("TownHallLevel", i) - 1, 0);
				m_strenghtWeight[i] = GetClampedIntegerValue("StrengthWeight", i);
				m_damage[i] = LogicGamePlayUtil.DPSToSingleHit(GetClampedIntegerValue("Damage", i), 1000);
				m_damageRadius[i] = (GetClampedIntegerValue("DamageRadius", i) << 9) / 100;
				m_ejectHousingLimit[i] = GetIntegerValue("EjectHousingLimit", i);
				m_numSpawns[i] = GetClampedIntegerValue("NumSpawns", i);
				m_constructionTimes[i] = 86400 * GetClampedIntegerValue("BuildTimeD", i) +
											  3600 * GetClampedIntegerValue("BuildTimeH", i) +
											  60 * GetClampedIntegerValue("BuildTimeM", i) +
											  GetClampedIntegerValue("BuildTimeS", i);
				m_projectileData[i] = LogicDataTables.GetProjectileByName(GetValue("Projectile", i), this);
			}

			m_preferredTargetData = LogicDataTables.GetCharacterByName(GetValue("PreferredTarget", 0), this);
			m_preferredTargetDamageMod = GetIntegerValue("PreferredTargetDamageMod", 0);

			if (m_preferredTargetDamageMod == 0)
			{
				m_preferredTargetDamageMod = 100;
			}

			m_buildResourceData = LogicDataTables.GetResourceByName(GetValue("BuildResource", 0), this);

			if (m_buildResourceData == null)
			{
				Debugger.Error("build resource is not defined for trap: " + GetName());
			}

			m_ejectVictims = GetBooleanValue("EjectVictims", 0);
			m_actionFrame = 1000 * GetIntegerValue("ActionFrame", 0) / 24;
			m_pushback = GetIntegerValue("Pushback", 0);
			m_doNotScalePushByDamage = GetBooleanValue("DoNotScalePushByDamage", 0);
			m_effectData = LogicDataTables.GetEffectByName(GetValue("Effect", 0), this);
			m_effect2Data = LogicDataTables.GetEffectByName(GetValue("Effect2", 0), this);
			m_effectBrokenData = LogicDataTables.GetEffectByName(GetValue("EffectBroken", 0), this);
			m_damageEffectData = LogicDataTables.GetEffectByName(GetValue("DamageEffect", 0), this);
			m_pickUpEffectData = LogicDataTables.GetEffectByName(GetValue("PickUpEffect", 0), this);
			m_placingEffectData = LogicDataTables.GetEffectByName(GetValue("PlacingEffect", 0), this);
			m_appearEffectData = LogicDataTables.GetEffectByName(GetValue("AppearEffect", 0), this);
			m_toggleAttackModeEffectData = LogicDataTables.GetEffectByName(GetValue("ToggleAttackModeEffect", 0), this);
			m_triggerRadius = (GetIntegerValue("TriggerRadius", 0) << 9) / 100;
			m_directionCount = GetIntegerValue("DirectionCount", 0);
			m_spell = LogicDataTables.GetSpellByName(GetValue("Spell", 0), this);
			m_airTrigger = GetBooleanValue("AirTrigger", 0);
			m_groundTrigger = GetBooleanValue("GroundTrigger", 0);
			m_healerTrigger = GetBooleanValue("HealerTrigger", 0);
			m_speedMod = GetIntegerValue("SpeedMod", 0);
			m_damageMod = GetIntegerValue("DamageMod", 0);
			m_durationMS = GetIntegerValue("DurationMS", 0);
			m_hitDelayMS = GetIntegerValue("HitDelayMS", 0);
			m_hitCount = GetIntegerValue("HitCnt", 0);
			m_minTriggerHousingLimit = GetIntegerValue("MinTriggerHousingLimit", 0);
			m_spawnedCharGroundData = LogicDataTables.GetCharacterByName(GetValue("SpawnedCharGround", 0), this);
			m_spawnedCharAirData = LogicDataTables.GetCharacterByName(GetValue("SpawnedCharAir", 0), this);
			m_timeBetweenSpawnsMS = GetIntegerValue("TimeBetweenSpawnsMs", 0);
			m_spawnInitialDelayMS = GetIntegerValue("SpawnInitialDelayMs", 0);
			m_throwDistance = GetIntegerValue("ThrowDistance", 0);
			m_hasAltMode = GetBooleanValue("HasAltMode", 0);
			m_enableByCalendar = GetBooleanValue("EnabledByCalendar", 0);

			if (m_enableByCalendar)
			{
				if (m_upgradeLevelCount > 1)
				{
					Debugger.Error("Temporary traps should not have upgrade levels!");
				}
			}
		}

		public int GetWidth()
			=> m_width;

		public int GetHeight()
			=> m_height;

		public int GetUpgradeLevelCount()
			=> m_upgradeLevelCount;

		public int GetSpawnInitialDelayMS()
			=> m_spawnInitialDelayMS;

		public int GetNumSpawns(int upgLevel)
			=> m_numSpawns[upgLevel];

		public int GetBuildTime(int upgLevel)
			=> m_constructionTimes[upgLevel];

		public LogicResourceData GetBuildResource()
			=> m_buildResourceData;

		public int GetBuildCost(int upgLevel)
			=> m_buildCost[upgLevel];

		public int GetRequiredTownHallLevel(int upgLevel)
			=> m_townHallLevel[upgLevel];

		public LogicCharacterData GetSpawnedCharAir()
			=> m_spawnedCharAirData;

		public LogicCharacterData GetSpawnedCharGround()
			=> m_spawnedCharGroundData;

		public bool HasAlternativeMode()
			=> m_hasAltMode;

		public int GetThrowDistance()
			=> m_throwDistance;

		public int GetDirectionCount()
			=> m_directionCount;

		public int GetDamage(int idx)
			=> m_damage[idx];

		public int GetDamageRadius(int idx)
			=> m_damageRadius[idx];

		public override bool IsEnableByCalendar()
			=> m_enableByCalendar;

		public int GetRearmCost(int idx)
			=> m_rearmCost[idx];

		public int GetStrengthWeight(int idx)
			=> m_strenghtWeight[idx];

		public int GetEjectHousingLimit(int idx)
			=> m_ejectHousingLimit[idx];

		public int GetPushback()
			=> m_pushback;

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

		public bool GetEjectVictims()
			=> m_ejectVictims;

		public bool GetDoNotScalePushByDamage()
			=> m_doNotScalePushByDamage;

		public bool GetAirTrigger()
			=> m_airTrigger;

		public bool GetGroundTrigger()
			=> m_groundTrigger;

		public bool GetHealerTrigger()
			=> m_healerTrigger;

		public int GetMinTriggerHousingLimit()
			=> m_minTriggerHousingLimit;

		public int GetTimeBetweenSpawnsMS()
			=> m_timeBetweenSpawnsMS;

		public int GetTriggerRadius()
			=> m_triggerRadius;

		public int GetActionFrame()
			=> m_actionFrame;

		public int GetSpeedMod()
			=> m_speedMod;

		public int GetDamageMod()
			=> m_damageMod;

		public int GetDurationMS()
			=> m_durationMS;

		public int GetHitDelayMS()
			=> m_hitDelayMS;

		public int GetHitCount()
			=> m_hitCount;

		public LogicSpellData GetSpell()
			=> m_spell;

		public LogicProjectileData GetProjectile(int idx)
			=> m_projectileData[idx];

		public LogicEffectData GetEffect()
			=> m_effectData;

		public LogicEffectData GetEffect2()
			=> m_effect2Data;

		public LogicEffectData GetEffectBroken()
			=> m_effectBrokenData;

		public LogicEffectData GetDamageEffect()
			=> m_damageEffectData;

		public LogicEffectData GetPickUpEffect()
			=> m_pickUpEffectData;

		public LogicEffectData GetPlacingEffect()
			=> m_placingEffectData;

		public LogicEffectData GetAppearEffect()
			=> m_appearEffectData;

		public LogicEffectData GetToggleAttackModeEffect()
			=> m_toggleAttackModeEffectData;

		public LogicCharacterData GetPreferredTarget()
			=> m_preferredTargetData;

		public int GetPreferredTargetDamageMod()
			=> m_preferredTargetDamageMod;
	}
}