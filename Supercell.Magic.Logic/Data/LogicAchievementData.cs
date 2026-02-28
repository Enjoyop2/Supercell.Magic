using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAchievementData : LogicData
	{
		public const int ACTION_TYPE_NPC_STARS = 0;
		public const int ACTION_TYPE_UPGRADE = 1;
		public const int ACTION_TYPE_VICTORY_POINTS = 2;
		public const int ACTION_TYPE_UNIT_UNLOCK = 3;
		public const int ACTION_TYPE_CLEAR_OBSTACLES = 4;
		public const int ACTION_TYPE_DONATE_UNITS = 5;
		public const int ACTION_TYPE_LOOT = 6;
		public const int ACTION_TYPE_DESTROY = 9;
		public const int ACTION_TYPE_WIN_PVP_DEFENSE = 10;
		public const int ACTION_TYPE_WIN_PVP_ATTACK = 11;
		public const int ACTION_TYPE_LEAGUE = 12;
		public const int ACTION_TYPE_WAR_STARS = 13;
		public const int ACTION_TYPE_WAR_LOOT = 14;
		public const int ACTION_TYPE_DONATE_SPELLS = 15;
		public const int ACTION_TYPE_ACCOUNT_BOUND = 16;
		public const int ACTION_TYPE_VERSUS_BATTLE_TROPHIES = 17;
		public const int ACTION_TYPE_GEAR_UP = 18;
		public const int ACTION_TYPE_REPAIR_BUILDING = 19;

		private bool m_showValue;

		private int m_actionType;
		private int m_diamondReward;
		private int m_expReward;
		private int m_actionCount;
		private int m_level;
		private int m_levelCount;
		private int m_villageType;

		private string m_completedTID;
		private string m_androidId;

		private LogicBuildingData m_buildingData;
		private LogicResourceData m_resourceData;
		private LogicCharacterData m_characterData;
		private LogicArrayList<LogicAchievementData> m_achievementLevel;

		public LogicAchievementData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicAchievementData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_villageType = GetIntegerValue("UIGroup", 0);
			m_diamondReward = GetIntegerValue("DiamondReward", 0);
			m_expReward = GetIntegerValue("ExpReward", 0);
			m_actionCount = GetIntegerValue("ActionCount", 0);
			m_level = GetIntegerValue("Level", 0);
			m_levelCount = GetIntegerValue("LevelCount", 0);

			m_completedTID = GetValue("CompletedTID", 0);
			m_showValue = GetBooleanValue("ShowValue", 0);
			m_androidId = GetValue("AndroidID", 0);

			if (m_actionCount == 0)
			{
				Debugger.Error("Achievement has invalid ActionCount 0");
			}

			string action = GetValue("Action", 0);

			switch (action)
			{
				case "npc_stars":
					m_actionType = LogicAchievementData.ACTION_TYPE_NPC_STARS;
					break;
				case "upgrade":
					m_actionType = LogicAchievementData.ACTION_TYPE_UPGRADE;
					m_buildingData = LogicDataTables.GetBuildingByName(GetValue("ActionData", 0), this);

					if (m_buildingData == null)
					{
						Debugger.Error("LogicAchievementData - Building data is NULL for upgrade achievement");
					}

					break;
				case "victory_points":
					m_actionType = LogicAchievementData.ACTION_TYPE_VICTORY_POINTS;
					break;
				case "unit_unlock":
					m_actionType = LogicAchievementData.ACTION_TYPE_UNIT_UNLOCK;
					m_characterData = LogicDataTables.GetCharacterByName(GetValue("ActionData", 0), this);

					if (m_characterData == null)
					{
						Debugger.Error("LogicCharacterData - Character data is NULL for unit_unlock achievement");
					}

					break;
				case "clear_obstacles":
					m_actionType = LogicAchievementData.ACTION_TYPE_CLEAR_OBSTACLES;
					break;
				case "donate_units":
					m_actionType = LogicAchievementData.ACTION_TYPE_DONATE_UNITS;
					break;
				case "loot":
					m_actionType = LogicAchievementData.ACTION_TYPE_LOOT;
					m_resourceData = LogicDataTables.GetResourceByName(GetValue("ActionData", 0), this);

					if (m_resourceData == null)
					{
						Debugger.Error("LogicAchievementData - Resource data is NULL for loot achievement");
					}

					break;
				case "destroy":
					m_actionType = LogicAchievementData.ACTION_TYPE_DESTROY;
					m_buildingData = LogicDataTables.GetBuildingByName(GetValue("ActionData", 0), this);

					if (m_buildingData == null)
					{
						Debugger.Error("LogicAchievementData - Building data is NULL for destroy achievement");
					}

					break;
				case "win_pvp_attack":
					m_actionType = LogicAchievementData.ACTION_TYPE_WIN_PVP_ATTACK;
					break;
				case "win_pvp_defense":
					m_actionType = LogicAchievementData.ACTION_TYPE_WIN_PVP_DEFENSE;
					break;
				case "league":
					m_actionType = LogicAchievementData.ACTION_TYPE_LEAGUE;
					break;
				case "war_stars":
					m_actionType = LogicAchievementData.ACTION_TYPE_WAR_STARS;
					break;
				case "war_loot":
					m_actionType = LogicAchievementData.ACTION_TYPE_WAR_LOOT;
					break;
				case "donate_spells":
					m_actionType = LogicAchievementData.ACTION_TYPE_DONATE_SPELLS;
					break;
				case "account_bound":
					m_actionType = LogicAchievementData.ACTION_TYPE_ACCOUNT_BOUND;
					break;
				case "vs_battle_trophies":
					m_actionType = LogicAchievementData.ACTION_TYPE_VERSUS_BATTLE_TROPHIES;
					break;
				case "gear_up":
					m_actionType = LogicAchievementData.ACTION_TYPE_GEAR_UP;
					break;
				case "repair_building":
					m_actionType = LogicAchievementData.ACTION_TYPE_REPAIR_BUILDING;
					m_buildingData = LogicDataTables.GetBuildingByName(GetValue("ActionData", 0), this);

					if (m_buildingData == null)
					{
						Debugger.Error("LogicAchievementData - Building data is NULL for repair_building achievement");
					}

					break;
				default:
					Debugger.Error(string.Format("Unknown Action in achievements {0}", action));
					break;
			}

			m_achievementLevel = new LogicArrayList<LogicAchievementData>();

			string achievementName = GetName().Substring(0, GetName().Length - 1);
			LogicDataTable table = LogicDataTables.GetTable(LogicDataType.ACHIEVEMENT);

			for (int i = 0; i < table.GetItemCount(); i++)
			{
				LogicAchievementData achievementData = (LogicAchievementData)table.GetItemAt(i);

				if (achievementData.GetName().Contains(achievementName))
				{
					if (achievementData.GetName().Substring(0, achievementData.GetName().Length - 1).Equals(achievementName))
					{
						m_achievementLevel.Add(achievementData);
					}
				}
			}

			Debugger.DoAssert(m_achievementLevel.Size() == m_levelCount, string.Format(
								  "Expected same amount of achievements named {0}X to be same as LevelCount={1} for {2}.",
								  achievementName,
								  m_levelCount,
								  GetName()));
		}

		public int GetVillageType()
			=> m_villageType;

		public int GetActionType()
			=> m_actionType;

		public int GetDiamondReward()
			=> m_diamondReward;

		public int GetExpReward()
			=> m_expReward;

		public int GetActionCount()
			=> m_actionCount;

		public int GetLevel()
			=> m_level;

		public string GetCompletedTID()
			=> m_completedTID;

		public string GetAndroidID()
			=> m_androidId;

		public LogicArrayList<LogicAchievementData> GetAchievementLevels()
			=> m_achievementLevel;

		public LogicBuildingData GetBuildingData()
			=> m_buildingData;

		public LogicResourceData GetResourceData()
			=> m_resourceData;

		public LogicCharacterData GetCharacterData()
			=> m_characterData;
	}
}