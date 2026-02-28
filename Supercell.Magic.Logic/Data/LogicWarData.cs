using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicWarData : LogicData
	{
		private int m_teamSize;
		private int m_preparationMinutes;
		private int m_warMinutes;

		private bool m_disableProduction;

		public LogicWarData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicWarData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_teamSize = GetIntegerValue("TeamSize", 0);
			m_preparationMinutes = GetIntegerValue("PreparationMinutes", 0);
			m_warMinutes = GetIntegerValue("WarMinutes", 0);
			m_disableProduction = GetBooleanValue("DisableProduction", 0);
		}

		public int GetTeamSize()
			=> m_teamSize;

		public int GetPreparationMinutes()
			=> m_preparationMinutes;

		public int GetWarMinutes()
			=> m_warMinutes;

		public bool IsDisableProduction()
			=> m_disableProduction;
	}
}