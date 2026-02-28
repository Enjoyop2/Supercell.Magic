using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAttackerItemData
	{
		private LogicData m_data;
		private CSVRow m_row;

		private int m_index;

		private LogicEffectData m_hitEffectData;
		private LogicEffectData m_hitEffect2Data;
		private LogicEffectData m_hitEffectActiveData;
		private LogicEffectData m_attackEffectData;
		private LogicEffectData m_attackEffect2Data;
		private LogicEffectData m_altAttackEffectData;
		private LogicEffectData m_attackEffectLv2Data;
		private LogicEffectData m_attackEffectLv3Data;
		private LogicEffectData m_transitionEffectLv2Data;
		private LogicEffectData m_transitionEffectLv3Data;
		private LogicEffectData m_preAttackEffectData;
		private LogicEffectData m_becomesTargetableEffectData;
		private LogicEffectData m_hideEffectData;
		private LogicEffectData m_summonEffectData;
		private LogicEffectData m_attackEffectSharedData;
		private LogicProjectileData m_projectileData;
		private LogicProjectileData m_altProjectileData;
		private LogicSpellData m_hitSpellData;
		private LogicData m_preferredTargetData;
		private LogicCharacterData m_summonTroopData;
		private LogicProjectileData m_rageProjectileData;

		private int m_pushBack;
		private int m_damage;
		private int m_attackSpeed;
		private int m_altAttackSpeed;
		private int m_cooldownOverride;
		private int m_prepareSpeed;
		private int m_damageMulti;
		private int m_damageLvl2;
		private int m_damageLvl3;
		private int m_altDamage;
		private int m_attackRange;
		private int m_damageRadius;
		private int m_ammoCount;
		private int m_altNumMultiTargets;
		private int m_switchTimeLv2;
		private int m_switchTimeLv3;
		private int m_statusEffectTime;
		private int m_speedMod;
		private int m_altAttackRange;
		private int m_shockwavePushStrength;
		private int m_hitSpellLevel;
		private int m_damage2;
		private int m_damage2Radius;
		private int m_damage2Delay;
		private int m_damage2Min;
		private int m_damage2FalloffStart;
		private int m_damage2FalloffEnd;
		private int m_alternatePickNewTargetDelay;
		private int m_shockwaveArcLength;
		private int m_shockwaveExpandRadius;
		private int m_targetingConeAngle;
		private int m_penetratingRadius;
		private int m_penetratingExtraRange;
		private int m_targetGroupsRadius;
		private int m_targetGroupsRange;
		private int m_targetGroupsMinWeight;
		private int m_wakeUpSpace;
		private int m_wakeUpSpeed;
		private int m_preferredTargetDamageMod;
		private int m_minAttackRange;
		private int m_summonTroopCount;
		private int m_summonCooldown;
		private int m_summonLimit;
		private int m_projectileBounces;
		private int m_burstCount;
		private int m_burstDelay;
		private int m_altBurstCount;
		private int m_altBurstDelay;
		private int m_dummyProjectileCount;
		private int m_chainAttackDistance;
		private int m_newTargetAttackDelay;

		private bool m_airTargets;
		private bool m_groundTargets;
		private bool m_altAttackMode;
		private bool m_selfAsAoeCenter;
		private bool m_increasingDamage;
		private bool m_preventsHealing;
		private bool m_penetratingProjectile;
		private bool m_targetGroups;
		private bool m_fightWithGroups;
		private bool m_preferredTargetNoTargeting;
		private bool m_altAirTargets;
		private bool m_altGroundTargets;
		private bool m_altMultiTargets;
		private bool m_spawnOnAttack;

		public void CreateReferences(CSVRow row, LogicData data, int idx)
		{
			m_row = row;
			m_data = data;
			m_index = idx;

			m_pushBack = row.GetClampedIntegerValue("PushBack", idx);
			m_airTargets = row.GetClampedBooleanValue("AirTargets", idx);
			m_groundTargets = row.GetClampedBooleanValue("GroundTargets", idx);
			m_altAttackMode = row.GetClampedBooleanValue("AltAttackMode", idx);
			m_damage = 100 * row.GetClampedIntegerValue("Damage", idx);

			int dps = row.GetClampedIntegerValue("DPS", idx);
			int attackSpeed = row.GetClampedIntegerValue("AttackSpeed", idx);
			int altDps = row.GetClampedIntegerValue("AltDPS", idx);
			int altAttackSpeed = row.GetClampedIntegerValue("AltAttackSpeed", idx);

			if (m_altAttackMode && altAttackSpeed == 0)
			{
				altAttackSpeed = attackSpeed;
			}

			int cooldownOverride = row.GetClampedIntegerValue("CoolDownOverride", idx);

			if (cooldownOverride == 0)
			{
				int tmp = (int)(((dps | m_damage) >> 31) & 0xFFFFFAEC) + 1500;

				if (attackSpeed > tmp)
				{
					cooldownOverride = attackSpeed - tmp;
				}
			}

			m_prepareSpeed = row.GetClampedIntegerValue("PrepareSpeed", idx);

			m_attackSpeed = attackSpeed - cooldownOverride;
			m_altAttackSpeed = altAttackSpeed - cooldownOverride;
			m_cooldownOverride = cooldownOverride;

			m_damageMulti = 100 * row.GetClampedIntegerValue("DamageMulti", idx);
			m_damageLvl2 = 100 * row.GetClampedIntegerValue("DamageLv2", idx);
			m_damageLvl3 = 100 * row.GetClampedIntegerValue("DamageLv3", idx);

			m_altDamage = m_damage;

			if (dps != 0)
			{
				if (altDps == 0)
				{
					altDps = dps;
				}

				m_damage = LogicGamePlayUtil.DPSToSingleHit(dps, m_attackSpeed + m_cooldownOverride);
				m_altDamage = LogicGamePlayUtil.DPSToSingleHit(altDps, m_altAttackSpeed + m_cooldownOverride);
				m_damageMulti = LogicGamePlayUtil.DPSToSingleHit(row.GetClampedIntegerValue("DPSMulti", idx), m_attackSpeed + m_cooldownOverride);
				m_damageLvl2 = LogicGamePlayUtil.DPSToSingleHit(row.GetClampedIntegerValue("DPSLv2", idx), m_attackSpeed + m_cooldownOverride);
				m_damageLvl3 = LogicGamePlayUtil.DPSToSingleHit(row.GetClampedIntegerValue("DPSLv3", idx), m_attackSpeed + m_cooldownOverride);
			}

			m_hitEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("HitEffect", idx), data);
			m_hitEffect2Data = LogicDataTables.GetEffectByName(row.GetClampedValue("HitEffect2", idx), data);
			m_hitEffectActiveData = LogicDataTables.GetEffectByName(row.GetClampedValue("HitEffectActive", idx), data);

			m_attackRange = (row.GetClampedIntegerValue("AttackRange", idx) << 9) / 100;
			m_damageRadius = (row.GetClampedIntegerValue("DamageRadius", idx) << 9) / 100;

			m_attackEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffect", idx), data);
			m_altAttackEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffectAlt", idx), data);
			m_ammoCount = row.GetClampedIntegerValue("AmmoCount", idx);
			m_attackEffect2Data = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffect2", idx), data);
			m_attackEffectLv2Data = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffectLv2", idx), data);
			m_attackEffectLv3Data = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffectLv3", idx), data);
			m_transitionEffectLv2Data = LogicDataTables.GetEffectByName(row.GetClampedValue("TransitionEffectLv2", idx), data);
			m_transitionEffectLv3Data = LogicDataTables.GetEffectByName(row.GetClampedValue("TransitionEffectLv3", idx), data);
			m_altNumMultiTargets = row.GetClampedIntegerValue("AltNumMultiTargets", idx);
			m_switchTimeLv2 = row.GetClampedIntegerValue("Lv2SwitchTime", idx);
			m_switchTimeLv3 = row.GetClampedIntegerValue("Lv3SwitchTime", idx);
			m_statusEffectTime = row.GetClampedIntegerValue("StatusEffectTime", idx);
			m_speedMod = row.GetClampedIntegerValue("SpeedMod", idx);
			m_altAttackRange = (row.GetClampedIntegerValue("AltAttackRange", idx) << 9) / 100;
			m_projectileData = LogicDataTables.GetProjectileByName(row.GetClampedValue("Projectile", idx), data);
			m_altProjectileData = LogicDataTables.GetProjectileByName(row.GetClampedValue("AltProjectile", idx), data);
			m_shockwavePushStrength = row.GetClampedIntegerValue("ShockwavePushStrength", idx);
			m_hitSpellData = LogicDataTables.GetSpellByName(row.GetClampedValue("HitSpell", idx), data);
			m_hitSpellLevel = row.GetClampedIntegerValue("HitSpellLevel", idx);
			m_damage2 = 100 * row.GetClampedIntegerValue("Damage2", idx);
			m_damage2Radius = (row.GetClampedIntegerValue("Damage2Radius", idx) << 9) / 100;
			m_damage2Delay = row.GetClampedIntegerValue("Damage2Delay", idx);
			m_damage2Min = 100 * row.GetClampedIntegerValue("Damage2Min", idx);
			m_damage2FalloffStart = (row.GetClampedIntegerValue("Damage2FalloffStart", idx) << 9) / 100;
			m_damage2FalloffEnd = (row.GetClampedIntegerValue("Damage2FalloffStart", idx) << 9) / 100;

			if (m_damage2FalloffEnd < m_damage2FalloffStart)
			{
				Debugger.Error("Building " + row.GetName() + " has falloff end less than falloff start!");
			}

			if (m_damage2FalloffEnd > m_damage2Radius)
			{
				Debugger.Error("Building " + row.GetName() + " has falloff end greater than the damage radius!");
			}

			m_preAttackEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("PreAttackEffect", idx), data);
			m_becomesTargetableEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("BecomesTargetableEffect", idx), data);
			m_increasingDamage = row.GetClampedBooleanValue("IncreasingDamage", idx);
			m_preventsHealing = row.GetClampedBooleanValue("PreventsHealing", idx);
			m_alternatePickNewTargetDelay = row.GetClampedIntegerValue("AlternatePickNewTargetDelay", idx);
			m_shockwaveArcLength = row.GetClampedIntegerValue("ShockwaveArcLength", idx);
			m_shockwaveExpandRadius = row.GetClampedIntegerValue("ShockwaveExpandRadius", idx);
			m_targetingConeAngle = row.GetClampedIntegerValue("TargetingConeAngle", idx);
			m_penetratingProjectile = row.GetClampedBooleanValue("PenetratingProjectile", idx);
			m_penetratingRadius = (row.GetClampedIntegerValue("PenetratingRadius", idx) << 9) / 100;
			m_penetratingExtraRange = (row.GetClampedIntegerValue("PenetratingExtraRange", idx) << 9) / 100;
			m_targetGroups = row.GetClampedBooleanValue("TargetGroups", idx);
			m_fightWithGroups = row.GetClampedBooleanValue("FightWithGroups", idx);
			m_targetGroupsRadius = (row.GetClampedIntegerValue("TargetGroupsRadius", idx) << 9) / 100;
			m_targetGroupsRange = (row.GetClampedIntegerValue("TargetGroupsRange", idx) << 9) / 100;
			m_targetGroupsMinWeight = row.GetClampedIntegerValue("TargetGroupsMinWeight", idx);
			m_wakeUpSpace = row.GetClampedIntegerValue("WakeUpSpace", idx);
			m_wakeUpSpeed = row.GetClampedIntegerValue("WakeUpSpeed", idx);
			m_preferredTargetData = LogicDataTables.GetCharacterByName(row.GetClampedValue("PreferredTarget", idx), data);
			m_preferredTargetDamageMod = row.GetClampedIntegerValue("PreferredTargetDamageMod", idx);
			m_preferredTargetNoTargeting = row.GetClampedBooleanValue("PreferredTargetNoTargeting", idx);
			m_altAirTargets = row.GetClampedBooleanValue("AltAirTargets", idx);
			m_altGroundTargets = row.GetClampedBooleanValue("AltGroundTargets", idx);
			m_altMultiTargets = row.GetClampedBooleanValue("AltMultiTargets", idx);
			m_minAttackRange = (row.GetClampedIntegerValue("MinAttackRange", idx) << 9) / 100;

			if (m_preferredTargetData == null)
			{
				m_preferredTargetData = LogicDataTables.GetBuildingClassByName(row.GetClampedValue("PreferedTargetBuildingClass", idx), data);

				if (m_preferredTargetData == null)
				{
					m_preferredTargetData = LogicDataTables.GetBuildingByName(row.GetClampedValue("PreferedTargetBuilding", idx), data);
				}

				m_preferredTargetDamageMod = row.GetClampedIntegerValue("PreferedTargetDamageMod", idx);

				if (m_preferredTargetDamageMod == 0)
				{
					m_preferredTargetDamageMod = 100;
				}
			}

			m_summonTroopCount = row.GetClampedIntegerValue("SummonTroopCount", idx);
			m_summonCooldown = row.GetClampedIntegerValue("SummonCooldown", idx);
			m_summonEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("SummonEffect", idx), data);
			m_summonLimit = row.GetClampedIntegerValue("SummonLimit", idx);
			m_summonTroopData = LogicDataTables.GetCharacterByName(row.GetClampedValue("SummonTroop", idx), data);
			m_spawnOnAttack = row.GetClampedBooleanValue("SpawnOnAttack", idx);
			m_hideEffectData = LogicDataTables.GetEffectByName(row.GetClampedValue("HideEffect", idx), data);
			m_rageProjectileData = LogicDataTables.GetProjectileByName(row.GetClampedValue("RageProjectile", idx), data);
			m_projectileBounces = row.GetClampedIntegerValue("ProjectileBounces", idx);
			m_selfAsAoeCenter = row.GetClampedBooleanValue("SelfAsAoeCenter", idx);

			if (m_damage2Delay > m_cooldownOverride + m_attackSpeed)
			{
				Debugger.Error(row.GetName() + " has Damage2Delay greater than the attack speed!");
			}

			if (m_ammoCount > 0 && (m_attackSpeed & 63) != 0)
			{
				Debugger.Error(string.Format("Invalid attack speed {0} (must be multiple of 64)", m_attackSpeed));
			}

			m_burstCount = row.GetClampedIntegerValue("BurstCount", idx);
			m_burstDelay = row.GetClampedIntegerValue("BurstDelay", idx);
			m_altBurstCount = row.GetClampedIntegerValue("AltBurstCount", idx);
			m_altBurstDelay = row.GetClampedIntegerValue("AltBurstDelay", idx);
			m_dummyProjectileCount = row.GetClampedIntegerValue("DummyProjectileCount", idx);
			m_attackEffectSharedData = LogicDataTables.GetEffectByName(row.GetClampedValue("AttackEffectShared", idx), data);
			m_chainAttackDistance = row.GetClampedIntegerValue("ChainAttackDistance", idx);
			m_newTargetAttackDelay = row.GetClampedIntegerValue("NewTargetAttackDelay", idx);

			if (m_newTargetAttackDelay > 0)
			{
				m_newTargetAttackDelay = LogicMath.Clamp(attackSpeed - m_newTargetAttackDelay, 0, attackSpeed);
			}
		}

		public int GetDamage(int type, bool multi)
		{
			if (multi)
			{
				return m_damageMulti;
			}

			Debugger.DoAssert(type < 3, string.Empty);

			switch (type)
			{
				case 1:
					return m_damageLvl2;
				case 2:
					return m_damageLvl3;
				default:
					return m_damage;
			}
		}

		public int GetAltDamage(int level, bool alt)
		{
			Debugger.DoAssert(level < 3, string.Empty);

			if (alt)
			{
				return m_damageMulti;
			}

			return m_altDamage;
		}

		public int GetDamagePerMS(int type, bool multi)
		{
			int damage = GetDamage(type, multi);
			int cooldown = m_attackSpeed + m_cooldownOverride;

			if (cooldown != 0)
			{
				return 1000 * damage / (100 * cooldown);
			}

			return 0;
		}

		public int GetDamage2()
			=> m_damage2;

		public int GetDamage2Delay()
			=> m_damage2Delay;

		public int GetAttackCooldownOverride()
			=> m_cooldownOverride;

		public int GetShockwavePushStrength()
			=> m_shockwavePushStrength;

		public LogicSpellData GetHitSpell()
			=> m_hitSpellData;

		public bool HasAlternativeAttackMode()
			=> m_altAttackMode;

		public int GetAmmoCount()
			=> m_ammoCount;

		public int GetTargetingConeAngle()
			=> m_targetingConeAngle;

		public LogicData GetPreferredTargetData()
			=> m_preferredTargetData;

		public LogicEffectData GetHitEffect()
			=> m_hitEffectData;

		public LogicEffectData GetHitEffect2()
			=> m_hitEffect2Data;

		public LogicEffectData GetHitEffectActive()
			=> m_hitEffectActiveData;

		public LogicEffectData GetAttackEffect()
			=> m_attackEffectData;

		public LogicEffectData GetAttackEffect2()
			=> m_attackEffect2Data;

		public LogicEffectData GetAltAttackEffect()
			=> m_altAttackEffectData;

		public LogicEffectData GetAttackEffectLv2()
			=> m_attackEffectLv2Data;

		public LogicEffectData GetAttackEffectLv3()
			=> m_attackEffectLv3Data;

		public LogicEffectData GetTransitionEffectLv2()
			=> m_transitionEffectLv2Data;

		public LogicEffectData GetTransitionEffectLv3()
			=> m_transitionEffectLv3Data;

		public LogicEffectData GetPreAttackEffect()
			=> m_preAttackEffectData;

		public LogicEffectData GetBecomesTargetableEffect()
			=> m_becomesTargetableEffectData;

		public LogicEffectData GetHideEffect()
			=> m_hideEffectData;

		public LogicEffectData GetSummonEffect()
			=> m_summonEffectData;

		public LogicEffectData GetAttackEffectShared()
			=> m_attackEffectSharedData;

		public LogicProjectileData GetProjectile(bool alt)
		{
			if (alt)
			{
				return m_altProjectileData;
			}

			return m_projectileData;
		}

		public LogicCharacterData GetSummonTroop()
			=> m_summonTroopData;

		public LogicProjectileData GetRageProjectile()
			=> m_rageProjectileData;

		public int GetPushBack()
			=> m_pushBack;

		public int GetAttackSpeed()
			=> m_attackSpeed;

		public int GetAltAttackSpeed()
			=> m_altAttackSpeed;

		public int GetPrepareSpeed()
			=> m_prepareSpeed;

		public int GetAttackRange(bool alt)
		{
			if (alt)
			{
				return m_altAttackRange;
			}

			return m_attackRange;
		}

		public int GetDamageRadius()
			=> m_damageRadius;

		public int GetDamage2Radius()
			=> m_damage2Radius;

		public int GetAltNumMultiTargets()
			=> m_altNumMultiTargets;

		public int GetSwitchTimeLv2()
			=> m_switchTimeLv2;

		public int GetSwitchTimeLv3()
			=> m_switchTimeLv3;

		public int GetStatusEffectTime()
			=> m_statusEffectTime;

		public int GetSpeedMod()
			=> m_speedMod;

		public int GetHitSpellLevel()
			=> m_hitSpellLevel;

		public int GetDamage2Min()
			=> m_damage2Min;

		public int GetAlternatePickNewTargetDelay()
			=> m_alternatePickNewTargetDelay;

		public int GetShockwaveArcLength()
			=> m_shockwaveArcLength;

		public int GetShockwaveExpandRadius()
			=> m_shockwaveExpandRadius;

		public int GetPenetratingRadius()
			=> m_penetratingRadius;

		public int GetPenetratingExtraRange()
			=> m_penetratingExtraRange;

		public int GetTargetGroupsRadius()
			=> m_targetGroupsRadius;

		public int GetTargetGroupsRange()
			=> m_targetGroupsRange;

		public int GetTargetGroupsMinWeight()
			=> m_targetGroupsMinWeight;

		public int GetWakeUpSpace()
			=> m_wakeUpSpace;

		public int GetWakeUpSpeed()
			=> m_wakeUpSpeed;

		public int GetPreferredTargetDamageMod()
			=> m_preferredTargetDamageMod;

		public int GetMinAttackRange()
			=> m_minAttackRange;

		public int GetSummonTroopCount()
			=> m_summonTroopCount;

		public int GetSummonCooldown()
			=> m_summonCooldown;

		public int GetSummonLimit()
			=> m_summonLimit;

		public int GetProjectileBounces()
			=> m_projectileBounces;

		public int GetBurstCount()
			=> m_burstCount;

		public int GetBurstDelay()
			=> m_burstDelay;

		public int GetAltBurstCount()
			=> m_altBurstCount;

		public int GetAltBurstDelay()
			=> m_altBurstDelay;

		public int GetDummyProjectileCount()
			=> m_dummyProjectileCount;

		public int GetChainAttackDistance()
			=> m_chainAttackDistance;

		public int GetNewTargetAttackDelay()
			=> m_newTargetAttackDelay;

		public bool GetTrackAirTargets(bool alt)
		{
			if (alt)
			{
				return m_altAirTargets;
			}

			return m_airTargets;
		}

		public bool GetTrackGroundTargets(bool alt)
		{
			if (alt)
			{
				return m_groundTargets;
			}

			return m_groundTargets;
		}

		public bool IsSelfAsAoeCenter()
			=> m_selfAsAoeCenter;

		public bool IsIncreasingDamage()
			=> m_increasingDamage;

		public bool GetPreventsHealing()
			=> m_preventsHealing;

		public bool IsPenetratingProjectile()
			=> m_penetratingProjectile;

		public bool GetTargetGroups()
			=> m_targetGroups;

		public bool GetFightWithGroups()
			=> m_fightWithGroups;

		public bool GetPreferredTargetNoTargeting()
			=> m_preferredTargetNoTargeting;

		public bool GetSpawnOnAttack()
			=> m_spawnOnAttack;

		public bool GetMultiTargets(bool alt)
		{
			if (alt)
			{
				return m_altMultiTargets;
			}

			return false;
		}

		public LogicAttackerItemData Clone()
		{
			LogicAttackerItemData cloned = new LogicAttackerItemData();
			cloned.CreateReferences(m_row, m_data, m_index);
			return cloned;
		}

		public void AddAttackRange(int value)
		{
			m_attackRange += value;
			m_altAttackRange += value;
		}
	}
}