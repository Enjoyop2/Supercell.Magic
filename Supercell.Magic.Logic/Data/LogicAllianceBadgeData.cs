using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAllianceBadgeData : LogicData
	{
		private string m_iconLayer0;
		private string m_iconLayer1;
		private string m_iconLayer2;

		public LogicAllianceBadgeData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicAllianceBadgeData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_iconLayer0 = GetValue("IconLayer0", 0);
			m_iconLayer1 = GetValue("IconLayer1", 0);
			m_iconLayer2 = GetValue("IconLayer2", 0);
		}
	}
}