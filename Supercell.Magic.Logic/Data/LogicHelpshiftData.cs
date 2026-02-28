using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicHelpshiftData : LogicData
	{
		private string m_helpshiftId;

		public LogicHelpshiftData(CSVRow row, LogicDataTable table) : base(row, table)
		{
		}

		public override void CreateReferences()
		{
			base.CreateReferences();
			m_helpshiftId = GetValue("HelpshiftId", 0);
		}
	}
}