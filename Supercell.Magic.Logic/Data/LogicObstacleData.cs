using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicObstacleData : LogicGameObjectData
	{
		private LogicResourceData m_clearResourceData;
		private LogicResourceData m_lootResourceData;
		private LogicEffectData m_clearEffect;
		private LogicEffectData m_pickUpEffect;
		private LogicObstacleData m_spawnObstacle;

		private string m_exportName;
		private string m_exportNameBase;
		private string m_highlightExportName;

		private bool m_passable;
		private bool m_isTombstone;
		private bool m_lightsOn;
		private bool m_tallGrass;
		private bool m_tallGrassSpawnPoint;

		private int m_lootCount;
		private int m_clearCost;
		private int m_clearTimeSecs;
		private int m_respawnWeight;
		private int m_lootMultiplierVersion2;
		private int m_width;
		private int m_height;
		private int m_tombGroup;
		private int m_appearancePeriodHours;
		private int m_minRespawnTimeHours;
		private int m_spawnRadius;
		private int m_spawnIntervalSeconds;
		private int m_spawnCount;
		private int m_maxSpawned;
		private int m_maxLifetimeSpawns;
		private int m_lootDefensePercentage;
		private int m_redMul;
		private int m_greenMul;
		private int m_blueMul;
		private int m_redAdd;
		private int m_greenAdd;
		private int m_blueAdd;
		private int m_village2RespawnCount;
		private int m_variationCount;
		private int m_lootHighlightPercentage;

		public LogicObstacleData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicObstacleData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_exportName = GetValue("ExportName", 0);
			m_exportNameBase = GetValue("ExportNameBase", 0);
			m_width = GetIntegerValue("Width", 0);
			m_height = GetIntegerValue("Height", 0);
			m_passable = GetBooleanValue("Passable", 0);
			m_clearEffect = LogicDataTables.GetEffectByName(GetValue("ClearEffect", 0), this);
			m_pickUpEffect = LogicDataTables.GetEffectByName(GetValue("PickUpEffect", 0), this);
			m_isTombstone = GetBooleanValue("IsTombstone", 0);
			m_tombGroup = GetIntegerValue("TombGroup", 0);
			m_appearancePeriodHours = GetIntegerValue("AppearancePeriodHours", 0);
			m_minRespawnTimeHours = GetIntegerValue("MinRespawnTimeHours", 0);
			m_lootDefensePercentage = GetIntegerValue("LootDefensePercentage", 0);
			m_redMul = GetIntegerValue("RedMul", 0);
			m_greenMul = GetIntegerValue("GreenMul", 0);
			m_blueMul = GetIntegerValue("BlueMul", 0);
			m_redAdd = GetIntegerValue("RedAdd", 0);
			m_greenAdd = GetIntegerValue("GreenAdd", 0);
			m_blueAdd = GetIntegerValue("BlueAdd", 0);
			m_lightsOn = GetBooleanValue("LightsOn", 0);
			m_village2RespawnCount = GetIntegerValue("Village2RespawnCount", 0);
			m_variationCount = GetIntegerValue("VariationCount", 0);
			m_tallGrass = GetBooleanValue("TallGrass", 0);
			m_tallGrassSpawnPoint = GetBooleanValue("TallGrassSpawnPoint", 0);
			m_lootHighlightPercentage = GetIntegerValue("LootHighlightPercentage", 0);
			m_highlightExportName = GetValue("HighlightExportName", 0);

			m_clearResourceData = LogicDataTables.GetResourceByName(GetValue("ClearResource", 0), this);

			if (m_clearResourceData == null)
			{
				Debugger.Error("Clear resource is not defined for obstacle: " + GetName());
			}

			m_clearCost = GetIntegerValue("ClearCost", 0);
			m_clearTimeSecs = GetIntegerValue("ClearTimeSeconds", 0);
			m_respawnWeight = GetIntegerValue("RespawnWeight", 0);

			string lootResourceName = GetValue("LootResource", 0);

			if (lootResourceName.Length <= 0)
			{
				m_respawnWeight = 0;
			}
			else
			{
				m_lootResourceData = LogicDataTables.GetResourceByName(lootResourceName, this);
				m_lootCount = GetIntegerValue("LootCount", 0);
			}

			m_lootMultiplierVersion2 = GetIntegerValue("LootMultiplierForVersion2", 0);

			if (m_lootMultiplierVersion2 == 0)
			{
				m_lootMultiplierVersion2 = 1;
			}

			string spawnObstacle = GetValue("SpawnObstacle", 0);

			if (spawnObstacle.Length > 0)
			{
				m_spawnObstacle = LogicDataTables.GetObstacleByName(spawnObstacle, this);
				m_spawnRadius = GetIntegerValue("SpawnRadius", 0);
				m_spawnIntervalSeconds = GetIntegerValue("SpawnIntervalSeconds", 0);
				m_spawnCount = GetIntegerValue("SpawnCount", 0);
				m_maxSpawned = GetIntegerValue("MaxSpawned", 0);
				m_maxLifetimeSpawns = GetIntegerValue("MaxLifetimeSpawns", 0);
			}
		}

		public override void CreateReferences2()
		{
			if (m_lootResourceData != null)
			{
				if (m_lootResourceData.GetVillageType() != GetVillageType() && !m_lootResourceData.IsPremiumCurrency())
				{
					Debugger.Error("invalid resource");
				}
			}

			if (m_clearResourceData.GetVillageType() != GetVillageType() && !m_clearResourceData.IsPremiumCurrency())
			{
				Debugger.Error("invalid clear resource");
			}
		}

		public int GetRespawnWeight()
			=> m_respawnWeight;

		public int GetClearTime()
			=> m_clearTimeSecs;

		public LogicResourceData GetClearResourceData()
			=> m_clearResourceData;

		public LogicResourceData GetLootResourceData()
			=> m_lootResourceData;

		public int GetLootCount()
			=> m_lootCount;

		public int GetClearCost()
			=> m_clearCost;

		public int GetLootMultiplierVersion2()
			=> m_lootMultiplierVersion2;

		public bool IsLootCart()
			=> m_lootDefensePercentage > 0;

		public string GetExportName()
			=> m_exportName;

		public string GetExportNameBase()
			=> m_exportNameBase;

		public int GetWidth()
			=> m_width;

		public int GetHeight()
			=> m_height;

		public bool IsPassable()
			=> m_passable;

		public int GetTombGroup()
			=> m_tombGroup;

		public LogicEffectData GetClearEffect()
			=> m_clearEffect;

		public LogicEffectData GetPickUpEffect()
			=> m_pickUpEffect;

		public LogicObstacleData GetSpawnObstacle()
			=> m_spawnObstacle;

		public bool IsTombstone()
			=> m_isTombstone;

		public int GetAppearancePeriodHours()
			=> m_appearancePeriodHours;

		public int GetMinRespawnTimeHours()
			=> m_minRespawnTimeHours;

		public int GetSpawnRadius()
			=> m_spawnRadius;

		public int GetSpawnIntervalSeconds()
			=> m_spawnIntervalSeconds;

		public int GetSpawnCount()
			=> m_spawnCount;

		public int GetMaxSpawned()
			=> m_maxSpawned;

		public int GetMaxLifetimeSpawns()
			=> m_maxLifetimeSpawns;

		public int GetLootDefensePercentage()
			=> m_lootDefensePercentage;

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

		public bool IsLightsOn()
			=> m_lightsOn;

		public int GetVillage2RespawnCount()
			=> m_village2RespawnCount;

		public int GetVariationCount()
			=> m_variationCount;

		public bool IsTallGrass()
			=> m_tallGrass;

		public bool IsTallGrassSpawnPoint()
			=> m_tallGrassSpawnPoint;

		public int GetLootHighlightPercentage()
			=> m_lootHighlightPercentage;

		public string GetHighlightExportName()
			=> m_highlightExportName;
	}
}