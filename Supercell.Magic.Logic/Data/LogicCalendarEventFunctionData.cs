using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicCalendarEventFunctionData : LogicData
	{
		public const int FUNCTION_TYPE_TROOP_TRAINING_BOOST = 1;
		public const int FUNCTION_TYPE_SPELL_BREWING_BOOST = 2;
		public const int FUNCTION_TYPE_BUILDING_BOOST = 3;
		public const int FUNCTION_TYPE_TROOP_DISCOUNT = 4;
		public const int FUNCTION_TYPE_SPELL_DISCOUNT = 5;
		public const int FUNCTION_TYPE_CLAN_XP_MULTIPLIER = 6;
		public const int FUNCTION_TYPE_OFFER_BUNDLE = 7;
		public const int FUNCTION_TYPE_STAR_BONUS_MULTIPLIER = 8;
		public const int FUNCTION_TYPE_ENABLE_TROOP = 9;
		public const int FUNCTION_TYPE_ENABLE_SPELL = 10;
		public const int FUNCTION_TYPE_ENABLE_TRAP = 11;
		public const int FUNCTION_TYPE_USE_TROOP = 12;
		public const int FUNCTION_TYPE_TARGETING_TOWN_HALL_LEVEL = 13;
		public const int FUNCTION_TYPE_TARGETING_PURCHASED_DIAMONDS = 14;
		public const int FUNCTION_TYPE_ENABLE_BILLING_PACKAGE = 15;
		public const int FUNCTION_TYPE_CHANGE_WORKER_LOOK = 16;
		public const int FUNCTION_TYPE_GIVE_FREE_TROOPS = 17;
		public const int FUNCTION_TYPE_GIVE_FREE_SPELLS = 18;
		public const int FUNCTION_TYPE_GIVE_FREE_HERO_HEALTH = 19;
		public const int FUNCTION_TYPE_BUILDING_SIGN = 20;
		public const int FUNCTION_TYPE_BUILDING_DESTROYED_SPAWN_UNIT = 21;
		public const int FUNCTION_TYPE_USE_SPELL = 22;
		public const int FUNCTION_TYPE_CLAN_WAR_LOOT_MULTIPLIER = 23;

		public const int PARAMETER_TYPE_BOOLEAN = 0;
		public const int PARAMETER_TYPE_INT = 1;
		public const int PARAMETER_TYPE_STRING = 2;
		public const int PARAMETER_TYPE_TROOP = 3;
		public const int PARAMETER_TYPE_SPELL = 4;
		public const int PARAMETER_TYPE_BUILDING = 5;
		public const int PARAMETER_TYPE_TRAP = 6;
		public const int PARAMETER_TYPE_BUNDLE = 7;
		public const int PARAMETER_TYPE_BILLING_PACKAGE = 8;
		public const int PARAMETER_TYPE_ANIMATION = 9;
		public const int PARAMETER_TYPE_HERO = 10;

		private bool m_targetingSupported;
		private bool m_deprecated;

		private int[] m_parameterType;
		private string[] m_parameterName;
		private string[] m_description;

		private int[] m_minValue;
		private int[] m_maxValue;

		private int m_category;
		private int m_functionType;

		public LogicCalendarEventFunctionData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicCalendarEventFunctionData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			int size = GetArraySize("ParameterType");

			m_parameterType = new int[size];
			m_parameterName = new string[size];
			m_description = new string[size];
			m_minValue = new int[size];
			m_maxValue = new int[size];

			for (int i = 0; i < size; i++)
			{
				m_parameterType[i] = GetParameterTypeByName(GetValue("ParameterType", i));
				m_parameterName[i] = GetValue("ParameterName", i);
				m_description[i] = GetValue("Description", i);
				m_minValue[i] = GetIntegerValue("MinValue", i);
				m_maxValue[i] = GetIntegerValue("MaxValue", i);
			}

			m_category = GetCategoryByName(GetValue("Category", 0));
			m_functionType = GetFunctionByName(GetName());

			m_targetingSupported = GetBooleanValue("TargetingSupported", 0);
			m_deprecated = GetBooleanValue("Deprecated", 0);

			for (int i = 0; i < size; i++)
			{
				if (m_parameterType[i] == null)
				{
					break;
				}

				Debugger.DoAssert(m_parameterName[i] != null, "Parameter index " + i + " name missing!");
			}
		}

		public int GetCategoryByName(string name)
		{
			switch (name)
			{
				case "Modifier":
					return 1;
				case "Targeting":
					return 2;
				default:
					Debugger.Error("Unknown category. " + name);
					return -1;
			}
		}

		public int GetFunctionByName(string name)
		{
			switch (name)
			{
				case "TroopTrainingBoost":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_TROOP_TRAINING_BOOST;
				case "SpellBrewingBoost":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_SPELL_BREWING_BOOST;
				case "BuildingBoost":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_BOOST;
				case "TroopDiscount":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_TROOP_DISCOUNT;
				case "SpellDiscount":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_SPELL_DISCOUNT;
				case "ClanXPMultiplier":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_CLAN_XP_MULTIPLIER;
				case "StarBonusMultiplier":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_STAR_BONUS_MULTIPLIER;
				case "OfferBundle":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_OFFER_BUNDLE;
				case "EnableTroop":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_TROOP;
				case "EnableSpell":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_SPELL;
				case "EnableTrap":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_TRAP;
				case "UseTroop":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_USE_TROOP;
				case "EnableBillingPackage":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_BILLING_PACKAGE;
				case "TargetingTownHallLevel":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_TARGETING_TOWN_HALL_LEVEL;
				case "TargetingPurchasedDiamonds":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_TARGETING_PURCHASED_DIAMONDS;
				case "ChangeWorkerLook":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_CHANGE_WORKER_LOOK;
				case "GiveFreeTroops":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_TROOPS;
				case "GiveFreeSpells":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_SPELLS;
				case "GiveFreeHeroHealth":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_HERO_HEALTH;
				case "BuildingSign":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_SIGN;
				case "BuildingDestroyedSpawnUnit":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_DESTROYED_SPAWN_UNIT;
				case "UseSpell":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_USE_SPELL;
				case "ClanWarLootMultiplier":
					return LogicCalendarEventFunctionData.FUNCTION_TYPE_CLAN_WAR_LOOT_MULTIPLIER;
				default:
					Debugger.Error("Unknown function. " + name);
					return -1;
			}
		}

		private int GetParameterTypeByName(string name)
		{
			switch (name.ToLowerInvariant())
			{
				case "boolean":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_BOOLEAN;
				case "int":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_INT;
				case "string":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_STRING;
				case "troop":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_TROOP;
				case "spell":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_SPELL;
				case "building":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_BUILDING;
				case "trap":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_TRAP;
				case "bundle":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_BUNDLE;
				case "billingpackage":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_BILLING_PACKAGE;
				case "animation":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_ANIMATION;
				case "hero":
					return LogicCalendarEventFunctionData.PARAMETER_TYPE_HERO;
				default:
					Debugger.Error("Unknown parameter type " + name);
					return -1;
			}
		}

		public int GetParameterType(int index)
		{
			if (index > m_parameterType.Length)
			{
				Debugger.Error(string.Format("Functions can only takes {0} parameters. index={1}", m_parameterType.Length, index));
			}

			return m_parameterType[index];
		}

		public string GetParameterName(int index)
			=> m_parameterName[index];

		public string GetDescription(int index)
			=> m_description[index];

		public int GetMinValue(int index)
			=> m_minValue[index];

		public int GetMaxValue(int index)
			=> m_maxValue[index];

		public int GetFunctionType()
			=> m_functionType;

		public int GetParameterCount()
			=> m_parameterType.Length;

		public bool IsDeprecated()
			=> m_deprecated;
	}
}