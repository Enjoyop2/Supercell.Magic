using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicVillageObjectData : LogicGameObjectData
	{
		private int m_upgradeLevelCount;
		private int m_tileX100;
		private int m_tileY100;
		private int m_requiredTH;
		private int m_animX;
		private int m_animY;
		private int m_animID;
		private int m_animDir;
		private int m_animVisibilityOdds;
		private int m_unitHousing;

		private int[] m_buildTime;
		private int[] m_buildCost;
		private int[] m_requiredTownHallLevel;

		private bool m_shipyard;
		private bool m_rowBoat;
		private bool m_clanGate;
		private bool m_disabled;
		private bool m_automaticUpgrades;
		private bool m_requiresBuilder;
		private bool m_hasInfoScreen;
		private bool m_housesUnits;

		private LogicResourceData m_buildResourceData;
		private LogicEffectData m_pickUpEffect;

		public LogicVillageObjectData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicVillageObjectData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_disabled = GetBooleanValue("Disabled", 0);
			m_tileX100 = GetIntegerValue("TileX100", 0);
			m_tileY100 = GetIntegerValue("TileY100", 0);
			m_requiredTH = GetIntegerValue("RequiredTH", 0);
			m_automaticUpgrades = GetBooleanValue("AutomaticUpgrades", 0);
			m_requiresBuilder = GetBooleanValue("RequiresBuilder", 0);
			m_pickUpEffect = LogicDataTables.GetEffectByName(GetValue("PickUpEffect", 0), this);
			m_animX = GetIntegerValue("AnimX", 0);
			m_animY = GetIntegerValue("AnimY", 0);
			m_animID = GetIntegerValue("AnimID", 0);
			m_animDir = GetIntegerValue("AnimDir", 0);
			m_animVisibilityOdds = GetIntegerValue("AnimVisibilityOdds", 0);
			m_hasInfoScreen = GetBooleanValue("HasInfoScreen", 0);
			m_unitHousing = GetIntegerValue("UnitHousing", 0);
			m_housesUnits = GetBooleanValue("HousesUnits", 0);

			m_shipyard = string.Equals("Shipyard", GetName());

			if (!m_shipyard)
			{
				m_shipyard = string.Equals("Shipyard2", GetName());
			}

			m_rowBoat = string.Equals("Rowboat", GetName());

			if (!m_rowBoat)
			{
				m_rowBoat = string.Equals("Rowboat2", GetName());
			}

			m_clanGate = string.Equals("ClanGate", GetName());

			m_upgradeLevelCount = m_row.GetBiggestArraySize();
			m_buildCost = new int[m_row.GetBiggestArraySize()];
			m_buildTime = new int[m_row.GetBiggestArraySize()];
			m_requiredTownHallLevel = new int[m_row.GetBiggestArraySize()];

			for (int i = 0; i < m_upgradeLevelCount; i++)
			{
				m_requiredTownHallLevel[i] = GetClampedIntegerValue("RequiredTownHall", i);
				m_buildCost[i] = GetClampedIntegerValue("BuildCost", i);
				m_buildTime[i] = 86400 * GetClampedIntegerValue("BuildTimeD", i) +
									  3600 * GetClampedIntegerValue("BuildTimeH", i) +
									  60 * GetClampedIntegerValue("BuildTimeM", i) +
									  GetClampedIntegerValue("BuildTimeS", i);
			}

			m_buildResourceData = LogicDataTables.GetResourceByName(GetValue("BuildResource", 0), this);
		}

		public bool IsShipyard()
			=> m_shipyard;

		public bool IsRowBoat()
			=> m_rowBoat;

		public bool IsClanGate()
			=> m_clanGate;

		public int GetBuildTime(int index)
			=> m_buildTime[index];

		public int GetBuildCost(int index)
			=> m_buildCost[index];

		public int GetRequiredTownHallLevel(int index)
			=> m_requiredTownHallLevel[index];

		public int GetUpgradeLevelCount()
			=> m_upgradeLevelCount;

		public LogicResourceData GetBuildResource()
			=> m_buildResourceData;

		public bool IsDisabled()
			=> m_disabled;

		public int GetTileX100()
			=> m_tileX100;

		public int GetTileY100()
			=> m_tileY100;

		public int GetRequiredTH()
			=> m_requiredTH;

		public bool IsAutomaticUpgrades()
			=> m_automaticUpgrades;

		public bool IsRequiresBuilder()
			=> m_requiresBuilder;

		public LogicEffectData GetPickUpEffect()
			=> m_pickUpEffect;

		public int GetAnimX()
			=> m_animX;

		public int GetAnimY()
			=> m_animY;

		public int GetAnimID()
			=> m_animID;

		public int GetAnimDir()
			=> m_animDir;

		public int GetAnimVisibilityOdds()
			=> m_animVisibilityOdds;

		public bool IsHasInfoScreen()
			=> m_hasInfoScreen;

		public int GetUnitHousing()
			=> m_unitHousing;

		public bool IsHousesUnits()
			=> m_housesUnits;
	}
}