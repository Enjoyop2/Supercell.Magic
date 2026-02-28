using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Data
{
	public class LogicHeroData : LogicCharacterData
	{
		private bool m_noDefence;
		private bool m_abilityOnce;
		private bool m_abilityStealth;
		private bool m_abilityAffectsHero;
		private bool m_hasAltMode;
		private bool m_altModeFlying;

		private int m_maxSearchRadiusForDefender;
		private int m_abilityCooldown;
		private int m_activationTime;
		private int m_activeDuration;
		private int m_alertRadius;
		private int m_abilityRadius;
		private int m_sleepOffsetX;
		private int m_sleepOffsetY;
		private int m_patrolRadius;
		private int m_abilityDelay;

		private int[] m_abilityTime;
		private int[] m_abilitySpeedBoost;
		private int[] m_abilitySpeedBoost2;
		private int[] m_abilityDamageBoostPercent;
		private int[] m_abilitySummonTroopCount;
		private int[] m_abilityHealthIncrease;
		private int[] m_abilityShieldProjectileSpeed;
		private int[] m_abilityShieldProjectileDamageMod;
		private int[] m_abilitySpellLevel;
		private int[] m_abilityDamageBoostOffset;
		private int[] m_abilityDamageBoost;
		private int[] m_regenerationTimeSecs;
		private int[] m_requiredTownHallLevel;
		private int[] m_strengthWeight2;

		private string m_smallPictureSWF;
		private string m_smallPicture;
		private string m_abilityTID;
		private string m_abilityDescTID;
		private string m_abilityIcon;
		private string m_abilityBigPictureExportName;

		private LogicCharacterData m_abilityAffectsCharacterData;
		private LogicCharacterData m_abilitySummonTroopData;
		private LogicEffectData m_abilityTriggerEffectData;
		private LogicEffectData m_specialAbilityEffectData;
		private LogicEffectData m_celebreateEffectData;

		private LogicSpellData[] m_abilitySpellData;

		public LogicHeroData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicHeroData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_maxSearchRadiusForDefender = (GetIntegerValue("MaxSearchRadiusForDefender", 0) << 9) / 100;

			int biggestArraySize = m_row.GetBiggestArraySize();

			m_abilitySpellData = new LogicSpellData[biggestArraySize];
			m_regenerationTimeSecs = new int[biggestArraySize];
			m_requiredTownHallLevel = new int[biggestArraySize];
			m_abilityTime = new int[biggestArraySize];
			m_abilitySpeedBoost = new int[biggestArraySize];
			m_abilitySpeedBoost2 = new int[biggestArraySize];
			m_abilityDamageBoostPercent = new int[biggestArraySize];
			m_abilitySummonTroopCount = new int[biggestArraySize];
			m_abilityHealthIncrease = new int[biggestArraySize];
			m_abilityShieldProjectileSpeed = new int[biggestArraySize];
			m_abilityShieldProjectileDamageMod = new int[biggestArraySize];
			m_abilitySpellLevel = new int[biggestArraySize];
			m_abilityDamageBoostOffset = new int[biggestArraySize];
			m_abilityDamageBoost = new int[biggestArraySize];
			m_strengthWeight2 = new int[biggestArraySize];

			for (int i = 0; i < biggestArraySize; i++)
			{
				m_regenerationTimeSecs[i] = 60 * GetClampedIntegerValue("RegenerationTimeMinutes", i);
				m_requiredTownHallLevel[i] = GetClampedIntegerValue("RequiredTownHallLevel", i) - 1;
				m_abilityTime[i] = GetClampedIntegerValue("AbilityTime", i);
				m_abilitySpeedBoost[i] = GetClampedIntegerValue("AbilitySpeedBoost", i);
				m_abilitySpeedBoost2[i] = GetClampedIntegerValue("AbilitySpeedBoost2", i);
				m_abilityDamageBoostPercent[i] = GetClampedIntegerValue("AbilityDamageBoostPercent", i);
				m_abilitySummonTroopCount[i] = GetClampedIntegerValue("AbilitySummonTroopCount", i);
				m_abilityHealthIncrease[i] = GetClampedIntegerValue("AbilityHealthIncrease", i);
				m_abilityShieldProjectileSpeed[i] = GetClampedIntegerValue("AbilityShieldProjectileSpeed", i);
				m_abilityShieldProjectileDamageMod[i] = GetClampedIntegerValue("AbilityShieldProjectileDamageMod", i);
				m_abilitySpellData[i] = LogicDataTables.GetSpellByName(GetClampedValue("AbilitySpell", i), this);
				m_abilitySpellLevel[i] = GetClampedIntegerValue("AbilitySpellLevel", i);
				m_abilityDamageBoostOffset[i] = GetClampedIntegerValue("AbilityDamageBoostOffset", i);

				int damage = m_attackerItemData[i].GetDamagePerMS(0, false);
				int offset = m_abilityDamageBoostOffset[i];

				m_abilityDamageBoost[i] = (100 * (damage + offset) + damage / 2) / damage - 100;
				m_strengthWeight2[i] = GetClampedIntegerValue("StrengthWeight2", i);
			}

			m_alertRadius = (GetIntegerValue("AlertRadius", 0) << 9) / 100;
			m_abilityStealth = GetBooleanValue("AbilityStealth", 0);
			m_abilityRadius = GetIntegerValue("AbilityRadius", 0);
			m_abilityAffectsHero = GetBooleanValue("AbilityAffectsHero", 0);
			m_abilityAffectsCharacterData = LogicDataTables.GetCharacterByName(GetValue("AbilityAffectsCharacter", 0), this);
			m_abilityTriggerEffectData = LogicDataTables.GetEffectByName(GetValue("AbilityTriggerEffect", 0), this);
			m_abilityOnce = GetBooleanValue("AbilityOnce", 0);
			m_abilityCooldown = GetIntegerValue("AbilityCooldown", 0);
			m_abilitySummonTroopData = LogicDataTables.GetCharacterByName(GetValue("AbilitySummonTroop", 0), this);
			m_specialAbilityEffectData = LogicDataTables.GetEffectByName(GetValue("SpecialAbilityEffect", 0), this);
			m_celebreateEffectData = LogicDataTables.GetEffectByName(GetValue("CelebrateEffect", 0), this);
			m_sleepOffsetX = (GetIntegerValue("SleepOffsetX", 0) << 9) / 100;
			m_sleepOffsetY = (GetIntegerValue("SleepOffsetY", 0) << 9) / 100;
			m_patrolRadius = (GetIntegerValue("PatrolRadius", 0) << 9) / 100;
			m_smallPictureSWF = GetValue("SmallPictureSWF", 0);
			m_smallPicture = GetValue("SmallPicture", 0);
			m_abilityTID = GetValue("AbilityTID", 0);
			m_abilityDescTID = GetValue("AbilityDescTID", 0);
			m_abilityIcon = GetValue("AbilityIcon", 0);
			m_abilityDelay = GetIntegerValue("AbilityDelay", 0);
			m_abilityBigPictureExportName = GetValue("AbilityBigPictureExportName", 0);
			m_hasAltMode = GetBooleanValue("HasAltMode", 0);
			m_altModeFlying = GetBooleanValue("AltModeFlying", 0);
			m_noDefence = GetBooleanValue("NoDefence", 0);
			m_activationTime = GetIntegerValue("ActivationTime", 0);
			m_activeDuration = GetIntegerValue("ActiveDuration", 0);
		}

		public bool GetAbilityStealth()
			=> m_abilityStealth;

		public bool GetAbilityAffectsHero()
			=> m_abilityAffectsHero;

		public int GetAbilityHealthIncrease(int upgLevel)
			=> m_abilityHealthIncrease[upgLevel];

		public bool HasHasAltMode()
			=> m_hasAltMode;

		public bool GetAltModeFlying()
			=> m_altModeFlying;

		public int GetMaxSearchRadiusForDefender()
			=> m_maxSearchRadiusForDefender;

		public int GetActivationTime()
			=> m_activationTime;

		public int GetActiveDuration()
			=> m_activeDuration;

		public int GetAlertRadius()
			=> m_alertRadius;

		public int GetAbilityRadius()
			=> m_abilityRadius;

		public int GetSleepOffsetX()
			=> m_sleepOffsetX;

		public int GetSleepOffsetY()
			=> m_sleepOffsetY;

		public int GetPatrolRadius()
			=> m_patrolRadius;

		public int GetAbilityDelay()
			=> m_abilityDelay;

		public string GetSmallPictureSWF()
			=> m_smallPictureSWF;

		public string GetSmallPicture()
			=> m_smallPicture;

		public string GetAbilityTID()
			=> m_abilityTID;

		public string GetAbilityDescTID()
			=> m_abilityDescTID;

		public string GetAbilityIcon()
			=> m_abilityIcon;

		public string GetAbilityBigPictureExportName()
			=> m_abilityBigPictureExportName;

		public LogicCharacterData GetAbilityAffectsCharacter()
			=> m_abilityAffectsCharacterData;

		public LogicCharacterData GetAbilitySummonTroop()
			=> m_abilitySummonTroopData;

		public LogicEffectData GetAbilityTriggerEffect()
			=> m_abilityTriggerEffectData;

		public LogicEffectData GetSpecialAbilityEffect()
			=> m_specialAbilityEffectData;

		public LogicEffectData GetCelebreateEffect()
			=> m_celebreateEffectData;

		public LogicSpellData GetAbilitySpell(int upgLevel)
			=> m_abilitySpellData[upgLevel];

		public int GetAbilitySpellLevel(int upgLevel)
			=> m_abilitySpellLevel[upgLevel];

		public int GetRequiredTownHallLevel(int index)
			=> m_requiredTownHallLevel[index];

		public int GetAbilityShieldProjectileSpeed(int upgLevel)
			=> m_abilityShieldProjectileSpeed[upgLevel];

		public int GetAbilityShieldProjectileDamageMod(int upgLevel)
			=> m_abilityShieldProjectileDamageMod[upgLevel];

		public int GetAbilityTime(int index)
			=> m_abilityTime[index];

		public int GetAbilityCooldown()
			=> m_abilityCooldown;

		public int GetAbilitySpeedBoost(int index)
			=> m_abilitySpeedBoost[index];

		public int GetAbilitySpeedBoost2(int index)
			=> m_abilitySpeedBoost2[index];

		public int GetAbilityDamageBoostPercent(int index)
			=> m_abilityDamageBoostPercent[index];

		public int GetAbilityDamageBoost(int index)
			=> m_abilityDamageBoost[index];

		public int GetAbilitySummonTroopCount(int index)
			=> m_abilitySummonTroopCount[index];

		public bool HasNoDefence()
			=> m_noDefence;

		public bool HasOnceAbility()
			=> m_abilityOnce;

		public int GetHeroHitpoints(int hp, int upgLevel)
		{
			hp = LogicMath.Max(0, hp);

			int regenTime = m_regenerationTimeSecs[upgLevel];
			int hitpoints = m_hitpoints[upgLevel];

			if (regenTime != 0)
			{
				hitpoints = hitpoints * (LogicMath.Max(regenTime - hp, 0) / 60) / (regenTime / 60);
			}

			return hitpoints;
		}

		public bool HasEnoughHealthForAttack(int hp, int upgLevel)
			=> m_hitpoints[upgLevel] == hp;

		public bool HasAbility(int upgLevel)
		{
			if (m_abilityTime[upgLevel] <= 0)
			{
				return m_abilitySpellData[upgLevel] != null;
			}

			return true;
		}

		public int GetFullRegenerationTimeSec(int upgLevel)
			=> m_regenerationTimeSecs[upgLevel];

		public int GetSecondsToFullHealth(int hp, int upgLevel)
			=> 60 * (m_regenerationTimeSecs[upgLevel] / 60 * (m_hitpoints[upgLevel] - LogicMath.Clamp(hp, 0, m_hitpoints[upgLevel])) /
						 m_hitpoints[upgLevel]);

		public int GetStrengthWeight2(int upgLevel)
			=> m_strengthWeight2[upgLevel];

		public bool IsFlying(int mode)
		{
			if (mode == 1)
			{
				if (m_hasAltMode)
				{
					return m_altModeFlying;
				}
			}

			return IsFlying();
		}

		public override int GetCombatItemType()
			=> LogicCombatItemData.COMBAT_ITEM_TYPE_HERO;
	}
}