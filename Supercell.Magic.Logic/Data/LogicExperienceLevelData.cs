using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicExperienceLevelData : LogicData
	{
		private int m_expPoints;

		public LogicExperienceLevelData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicExperienceLevelData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();
			m_expPoints = GetIntegerValue("ExpPoints", 0);
		}

		public int GetMaxExpPoints()
			=> m_expPoints;

		public static int GetLevelCap()
			=> LogicDataTables.GetTable(LogicDataType.EXPERIENCE_LEVEL).GetItemCount();
	}
}