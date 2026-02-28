using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicDataTables
	{
		public const int TABLE_COUNT = 44;

		private static LogicDataTable[] m_tables;
		private static LogicAnimationTable m_animationTable;

		private static LogicCombatItemData m_skeletonData;
		private static LogicCombatItemData m_balloonSkeletonData;

		private static LogicResourceData m_diamondsData;
		private static LogicResourceData m_goldData;
		private static LogicResourceData m_elixirData;
		private static LogicResourceData m_darkElixirData;
		private static LogicResourceData m_gold2Data;
		private static LogicResourceData m_elixir2Data;
		private static LogicResourceData m_warGoldData;
		private static LogicResourceData m_warElixirData;
		private static LogicResourceData m_warDarkElixirData;

		private static LogicBuildingData m_townHallData;
		private static LogicBuildingData m_townHallVillage2Data;
		private static LogicBuildingData m_allianceCastleData;
		private static LogicBuildingData m_bowData;
		private static LogicBuildingData m_darkTowerData;
		private static LogicBuildingData m_ancientArtilleryData;
		private static LogicBuildingData m_workerData;
		private static LogicBuildingData m_laboratoryVillage2Data;

		public static void Init()
		{
			LogicDataTables.m_tables = new LogicDataTable[LogicDataTables.TABLE_COUNT];
		}

		public static void DeInit()
		{
			if (LogicDataTables.m_tables != null)
			{
				for (int i = 0; i < LogicDataTables.m_tables.Length; i++)
				{
					if (LogicDataTables.m_tables[i] != null)
					{
						LogicDataTables.m_tables[i].Destruct();
						LogicDataTables.m_tables[i] = null;
					}
				}

				LogicDataTables.m_skeletonData = null;
				LogicDataTables.m_balloonSkeletonData = null;

				LogicDataTables.m_diamondsData = null;
				LogicDataTables.m_goldData = null;
				LogicDataTables.m_elixirData = null;
				LogicDataTables.m_darkElixirData = null;
				LogicDataTables.m_gold2Data = null;
				LogicDataTables.m_elixir2Data = null;
				LogicDataTables.m_warGoldData = null;
				LogicDataTables.m_warElixirData = null;
				LogicDataTables.m_warDarkElixirData = null;
				LogicDataTables.m_townHallData = null;
				LogicDataTables.m_townHallVillage2Data = null;
				LogicDataTables.m_allianceCastleData = null;
				LogicDataTables.m_bowData = null;
				LogicDataTables.m_darkTowerData = null;
				LogicDataTables.m_ancientArtilleryData = null;
				LogicDataTables.m_workerData = null;
				LogicDataTables.m_laboratoryVillage2Data = null;
			}
		}

		public static void InitDataTable(CSVNode node, LogicDataType index)
		{
			if (index == LogicDataType.ANIMATION)
			{
				if (LogicDataTables.m_animationTable != null)
				{
					LogicDataTables.m_animationTable.SetTable(node);
				}
				else
				{
					LogicDataTables.m_animationTable = new LogicAnimationTable(node, index);
				}
			}
			else
			{
				if (LogicDataTables.m_tables[(int)index] != null)
				{
					LogicDataTables.m_tables[(int)index].SetTable(node.GetTable());
				}
				else
				{
					switch (index)
					{
						case LogicDataType.GLOBAL:
							LogicDataTables.m_tables[(int)index] = new LogicGlobals(node.GetTable(), index);
							break;
						case LogicDataType.CLIENT_GLOBAL:
							LogicDataTables.m_tables[(int)index] = new LogicClientGlobals(node.GetTable(), index);
							break;
						default:
							LogicDataTables.m_tables[(int)index] = new LogicDataTable(node.GetTable(), index);
							break;
					}
				}
			}
		}

		public static void CreateReferences()
		{
			LogicDataTables.m_tables[(int)LogicDataType.PARTICLE_EMITTER].CreateReferences();

			for (int i = 0; i < LogicDataTables.m_tables.Length; i++)
			{
				if ((LogicDataType)i != LogicDataType.PARTICLE_EMITTER)
				{
					if (LogicDataTables.m_tables[i] != null)
					{
						LogicDataTables.m_tables[i].CreateReferences();
					}
				}
			}

			for (int i = 0; i < LogicDataTables.m_tables.Length; i++)
			{
				if (LogicDataTables.m_tables[i] != null)
				{
					LogicDataTables.m_tables[i].CreateReferences2();
				}
			}

			// LogicDataTables.m_animationTable.CreateReferences();

			LogicDataTable buildingDataTable = LogicDataTables.m_tables[(int)LogicDataType.BUILDING];

			for (int i = 0; i < buildingDataTable.GetItemCount(); i++)
			{
				LogicBuildingData buildingData = (LogicBuildingData)buildingDataTable.GetItemAt(i);

				if (buildingData.IsAllianceCastle())
				{
					LogicDataTables.m_allianceCastleData = buildingData;
				}

				if (buildingData.IsTownHall() && LogicDataTables.m_townHallData == null)
				{
					LogicDataTables.m_townHallData = buildingData;
				}

				if (buildingData.IsTownHallVillage2() && LogicDataTables.m_townHallVillage2Data == null)
				{
					LogicDataTables.m_townHallVillage2Data = buildingData;
				}
			}

			LogicDataTables.m_bowData = LogicDataTables.GetBuildingByName("Bow", null);
			LogicDataTables.m_darkTowerData = LogicDataTables.GetBuildingByName("Dark Tower", null);
			LogicDataTables.m_ancientArtilleryData = LogicDataTables.GetBuildingByName("Ancient Artillery", null);
			LogicDataTables.m_workerData = LogicDataTables.GetBuildingByName("Worker Building", null);
			LogicDataTables.m_laboratoryVillage2Data = LogicDataTables.GetBuildingByName("Laboratory2", null);
			LogicDataTables.m_diamondsData = LogicDataTables.GetResourceByName("Diamonds", null);
			LogicDataTables.m_goldData = LogicDataTables.GetResourceByName("Gold", null);
			LogicDataTables.m_elixirData = LogicDataTables.GetResourceByName("Elixir", null);
			LogicDataTables.m_darkElixirData = LogicDataTables.GetResourceByName("DarkElixir", null);
			LogicDataTables.m_gold2Data = LogicDataTables.GetResourceByName("Gold2", null);
			LogicDataTables.m_elixir2Data = LogicDataTables.GetResourceByName("Elixir2", null);
			LogicDataTables.m_warGoldData = LogicDataTables.GetResourceByName("WarGold", null);
			LogicDataTables.m_warElixirData = LogicDataTables.GetResourceByName("WarElixir", null);
			LogicDataTables.m_warDarkElixirData = LogicDataTables.GetResourceByName("WarDarkElixir", null);
			LogicDataTables.m_skeletonData = LogicDataTables.GetCharacterByName("Skeleton", null);
			LogicDataTables.m_balloonSkeletonData = LogicDataTables.GetCharacterByName("Balloon Skeleton", null);
		}

		public int GetTableCount()
			=> LogicDataTables.TABLE_COUNT;

		public static bool CanReloadTable(LogicDataTable table)
			=> true;

		public static bool IsSkeleton(LogicCharacterData data)
			=> data == LogicDataTables.m_skeletonData || data == LogicDataTables.m_balloonSkeletonData;

		public static LogicDataTable GetTable(LogicDataType tableIndex)
			=> LogicDataTables.m_tables[(int)tableIndex];

		public static LogicData GetDataById(int globalId)
		{
			int tableIndex = GlobalID.GetClassID(globalId) - 1;

			if (tableIndex >= 0 && tableIndex < LogicDataTables.TABLE_COUNT && LogicDataTables.m_tables[tableIndex] != null)
			{
				return LogicDataTables.m_tables[tableIndex].GetItemById(globalId);
			}

			return null;
		}

		public static LogicData GetDataById(int globalId, LogicDataType dataType)
		{
			LogicData data = LogicDataTables.GetDataById(globalId);

			if (data.GetDataType() != dataType)
				return null;

			return data;
		}

		public static LogicBuildingData GetBuildingByName(string name, LogicData data)
			=> (LogicBuildingData)LogicDataTables.m_tables[(int)LogicDataType.BUILDING].GetDataByName(name, data);

		public static LogicLocaleData GetLocaleByName(string name, LogicData data)
			=> (LogicLocaleData)LogicDataTables.m_tables[(int)LogicDataType.LOCALE].GetDataByName(name, data);

		public static LogicDecoData GetDecoByName(string name, LogicData data)
			=> (LogicDecoData)LogicDataTables.m_tables[(int)LogicDataType.DECO].GetDataByName(name, data);

		public static LogicBillingPackageData GetBillingPackageByName(string name, LogicData data)
			=> (LogicBillingPackageData)LogicDataTables.m_tables[(int)LogicDataType.BILLING_PACKAGE].GetDataByName(name, data);

		public static LogicVillageObjectData GetVillageObjectByName(string name, LogicData data)
			=> (LogicVillageObjectData)LogicDataTables.m_tables[(int)LogicDataType.VILLAGE_OBJECT].GetDataByName(name, data);

		public static LogicData GetVariableByName(string name, LogicData data)
			=> LogicDataTables.m_tables[(int)LogicDataType.VARIABLE].GetDataByName(name, data);

		public static LogicObstacleData GetObstacleByName(string name, LogicData data)
			=> (LogicObstacleData)LogicDataTables.m_tables[(int)LogicDataType.OBSTACLE].GetDataByName(name, data);

		public static LogicHeroData GetHeroByName(string name, LogicData data)
			=> (LogicHeroData)LogicDataTables.m_tables[(int)LogicDataType.HERO].GetDataByName(name, data);

		public static LogicCharacterData GetCharacterByName(string name, LogicData data)
			=> (LogicCharacterData)LogicDataTables.m_tables[(int)LogicDataType.CHARACTER].GetDataByName(name, data);

		public static LogicSpellData GetSpellByName(string name, LogicData data)
			=> (LogicSpellData)LogicDataTables.m_tables[(int)LogicDataType.SPELL].GetDataByName(name, data);

		public static LogicEffectData GetEffectByName(string name, LogicData data)
			=> (LogicEffectData)LogicDataTables.m_tables[(int)LogicDataType.EFFECT].GetDataByName(name, data);

		public static LogicParticleEmitterData GetParticleEmitterByName(string name, LogicData data)
			=> (LogicParticleEmitterData)LogicDataTables.m_tables[(int)LogicDataType.PARTICLE_EMITTER].GetDataByName(name, data);

		public static LogicProjectileData GetProjectileByName(string name, LogicData data)
			=> (LogicProjectileData)LogicDataTables.m_tables[(int)LogicDataType.PROJECTILE].GetDataByName(name, data);

		public static LogicRegionData GetRegionByName(string name, LogicData data)
			=> (LogicRegionData)LogicDataTables.m_tables[(int)LogicDataType.REGION].GetDataByName(name, data);

		public static LogicCalendarEventFunctionData GetCalendarEventFunctionByName(string name, LogicData data)
			=> (LogicCalendarEventFunctionData)LogicDataTables.m_tables[(int)LogicDataType.CALENDAR_EVENT_FUNCTION].GetDataByName(name, data);

		public static LogicEventEntryData GetEventEntryByName(string name, LogicData data)
			=> (LogicEventEntryData)LogicDataTables.m_tables[(int)LogicDataType.EVENT_ENTRY].GetDataByName(name, data);

		public static LogicData GetDataByName(string name, int tableIdx, LogicData caller)
		{
			if (LogicDataTables.m_tables[tableIdx] != null)
			{
				return LogicDataTables.m_tables[tableIdx].GetDataByName(name, caller);
			}

			return null;
		}

		public static LogicExperienceLevelData GetExperienceLevel(int level)
		{
			if (level > 0)
			{
				if (level - 1 < LogicDataTables.m_tables[(int)LogicDataType.EXPERIENCE_LEVEL].GetItemCount())
				{
					return (LogicExperienceLevelData)LogicDataTables.m_tables[(int)LogicDataType.EXPERIENCE_LEVEL].GetItemAt(level - 1);
				}
			}

			Debugger.Error("LogicDataTables::getExperienceLevel parameter out of bounds");

			return null;
		}

		public static LogicAllianceLevelData GetAllianceLevel(int level)
			=> (LogicAllianceLevelData)LogicDataTables.m_tables[(int)LogicDataType.ALLIANCE_LEVEL].GetItemAt(level - 1);

		public static int GetExperienceLevelCount()
			=> LogicDataTables.m_tables[(int)LogicDataType.EXPERIENCE_LEVEL].GetItemCount();

		public static int GetAllianceLevelCount()
			=> LogicDataTables.m_tables[(int)LogicDataType.ALLIANCE_LEVEL].GetItemCount();

		public static int GetTownHallLevelCount()
			=> LogicDataTables.m_tables[(int)LogicDataType.TOWNHALL_LEVEL].GetItemCount();

		public static LogicTownhallLevelData GetTownHallLevel(int levelIndex)
		{
			if (levelIndex > -1)
			{
				if (levelIndex < LogicDataTables.m_tables[(int)LogicDataType.TOWNHALL_LEVEL].GetItemCount())
				{
					return (LogicTownhallLevelData)LogicDataTables.m_tables[(int)LogicDataType.TOWNHALL_LEVEL].GetItemAt(levelIndex);
				}
			}

			Debugger.Error("LogicDataTables::getTownHallLevel parameter out of bounds");

			return null;
		}

		public static LogicNpcData GetNpcByName(string name, LogicData data)
			=> (LogicNpcData)LogicDataTables.m_tables[(int)LogicDataType.NPC].GetDataByName(name, data);

		public static LogicMissionData GetMissionByName(string name, LogicData data)
			=> (LogicMissionData)LogicDataTables.m_tables[(int)LogicDataType.MISSION].GetDataByName(name, data);

		public static LogicResourceData GetResourceByName(string name, LogicData data)
			=> (LogicResourceData)LogicDataTables.m_tables[(int)LogicDataType.RESOURCE].GetDataByName(name, data);

		public static LogicGlobalData GetClientGlobalByName(string name, LogicData data)
			=> (LogicGlobalData)LogicDataTables.m_tables[(int)LogicDataType.CLIENT_GLOBAL].GetDataByName(name, data);

		public static LogicGlobalData GetGlobalByName(string name, LogicData data)
			=> (LogicGlobalData)LogicDataTables.m_tables[(int)LogicDataType.GLOBAL].GetDataByName(name, data);

		public static LogicBuildingClassData GetBuildingClassByName(string name, LogicData data)
			=> (LogicBuildingClassData)LogicDataTables.m_tables[(int)LogicDataType.BUILDING_CLASS].GetDataByName(name, data);

		public static LogicClientGlobals GetClientGlobals()
			=> (LogicClientGlobals)LogicDataTables.m_tables[(int)LogicDataType.CLIENT_GLOBAL];

		public static LogicGlobals GetGlobals()
			=> (LogicGlobals)LogicDataTables.m_tables[(int)LogicDataType.GLOBAL];

		public static LogicBuildingData GetAllianceCastleData()
			=> LogicDataTables.m_allianceCastleData;

		public static LogicBuildingData GetWorkerData()
			=> LogicDataTables.m_workerData;

		public static LogicBuildingData GetTownHallData()
			=> LogicDataTables.m_townHallData;

		public static LogicBuildingData GetTownHallVillage2Data()
			=> LogicDataTables.m_townHallVillage2Data;

		public static LogicResourceData GetDiamondsData()
			=> LogicDataTables.m_diamondsData;

		public static LogicResourceData GetGoldData()
			=> LogicDataTables.m_goldData;

		public static LogicResourceData GetElixirData()
			=> LogicDataTables.m_elixirData;

		public static LogicResourceData GetDarkElixirData()
			=> LogicDataTables.m_darkElixirData;

		public static LogicResourceData GetGold2Data()
			=> LogicDataTables.m_gold2Data;

		public static LogicResourceData GetElixir2Data()
			=> LogicDataTables.m_elixir2Data;

		public static LogicResourceData GetWarGoldData()
			=> LogicDataTables.m_warGoldData;

		public static LogicResourceData GetWarElixirData()
			=> LogicDataTables.m_warElixirData;

		public static LogicResourceData GetWarDarkElixirData()
			=> LogicDataTables.m_warDarkElixirData;
	}
}