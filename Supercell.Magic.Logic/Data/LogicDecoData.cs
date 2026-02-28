using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicDecoData : LogicGameObjectData
	{
		private int m_width;
		private int m_height;
		private int m_buildCost;
		private int m_maxCount;
		private int m_requiredExpLevel;

		private bool m_inShop;
		private bool m_passable;

		private LogicResourceData m_buildResourceData;

		public LogicDecoData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicDecoData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_width = GetIntegerValue("Width", 0);
			m_height = GetIntegerValue("Height", 0);
			m_maxCount = GetIntegerValue("MaxCount", 0);
			m_inShop = !GetBooleanValue("NotInShop", 0);
			m_buildCost = GetIntegerValue("BuildCost", 0);
			m_requiredExpLevel = GetIntegerValue("RequiredExpLevel", 0);
			m_passable = GetBooleanValue("DecoPath", 0);

			m_buildResourceData = LogicDataTables.GetResourceByName(GetValue("BuildResource", 0), this);
		}

		public bool IsInShop()
			=> m_inShop;

		public int GetMaxCount()
			=> m_maxCount;

		public int GetRequiredExpLevel()
			=> m_requiredExpLevel;

		public int GetSellPrice()
			=> m_buildCost / 10;

		public int GetBuildCost()
			=> m_buildCost;

		public int GetWidth()
			=> m_width;

		public int GetHeight()
			=> m_height;

		public LogicResourceData GetBuildResource()
			=> m_buildResourceData;

		public bool IsPassable()
			=> m_passable;
	}
}