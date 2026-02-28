using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicNpcData : LogicData
	{
		private string m_mapInstanceName;
		private string m_levelFile;
		private string m_playerName;
		private string m_allianceName;

		private int m_expLevel;
		private int m_goldCount;
		private int m_elixirCount;
		private int m_allianceBadge;

		private bool m_alwaysUnlocked;
		private bool m_singlePlayer;

		private readonly LogicArrayList<LogicNpcData> m_dependencies;
		private readonly LogicArrayList<LogicDataSlot> m_unitCount;

		public LogicNpcData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			m_dependencies = new LogicArrayList<LogicNpcData>();
			m_unitCount = new LogicArrayList<LogicDataSlot>();
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_mapInstanceName = GetValue("MapInstanceName", 0);
			m_expLevel = GetIntegerValue("ExpLevel", 0);
			m_levelFile = GetValue("LevelFile", 0);
			m_goldCount = GetIntegerValue("Gold", 0);
			m_elixirCount = GetIntegerValue("Elixir", 0);
			m_alwaysUnlocked = GetBooleanValue("AlwaysUnlocked", 0);
			m_playerName = GetValue("PlayerName", 0);
			m_allianceName = GetValue("AllianceName", 0);
			m_allianceBadge = GetIntegerValue("AllianceBadge", 0);
			m_singlePlayer = GetBooleanValue("SinglePlayer", 0);

			int unitCountSize = GetArraySize("UnitType");

			if (unitCountSize > 0)
			{
				m_unitCount.EnsureCapacity(unitCountSize);

				for (int i = 0; i < unitCountSize; i++)
				{
					int count = GetIntegerValue("UnitCount", i);

					if (count > 0)
					{
						m_unitCount.Add(new LogicDataSlot(LogicDataTables.GetCharacterByName(GetValue("UnitType", i), this), count));
					}
				}
			}

			int mapDependencySize = GetArraySize("MapDependencies");

			for (int i = 0; i < mapDependencySize; i++)
			{
				LogicNpcData data = LogicDataTables.GetNpcByName(GetValue("MapDependencies", i), this);

				if (data != null)
				{
					m_dependencies.Add(data);
				}
			}
		}

		public LogicArrayList<LogicDataSlot> GetClonedUnits()
		{
			LogicArrayList<LogicDataSlot> units = new LogicArrayList<LogicDataSlot>();

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				units.Add(m_unitCount[i].Clone());
			}

			return units;
		}


		public bool IsUnlockedInMap(LogicClientAvatar avatar)
		{
			if (!m_alwaysUnlocked)
			{
				if (!string.IsNullOrEmpty(m_mapInstanceName))
				{
					if (m_dependencies != null)
					{
						for (int i = 0; i < m_dependencies.Size(); i++)
						{
							if (avatar.GetNpcStars(m_dependencies[i]) > 0)
							{
								return true;
							}
						}
					}
				}

				return false;
			}

			return true;
		}

		public string GetMapInstanceName()
			=> m_mapInstanceName;

		public int GetExpLevel()
			=> m_expLevel;

		public string GetLevelFile()
			=> m_levelFile;

		public int GetGoldCount()
			=> m_goldCount;

		public int GetElixirCount()
			=> m_elixirCount;

		public bool IsAlwaysUnlocked()
			=> m_alwaysUnlocked;

		public string GetPlayerName()
			=> m_playerName;

		public string GetAllianceName()
			=> m_allianceName;

		public int GetAllianceBadge()
			=> m_allianceBadge;

		public bool IsSinglePlayer()
			=> m_singlePlayer;
	}
}