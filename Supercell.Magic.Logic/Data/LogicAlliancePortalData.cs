using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAlliancePortalData : LogicGameObjectData
	{
		private string m_swf;
		private string m_exportName;

		private int m_width;
		private int m_height;

		public LogicAlliancePortalData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicAlliancePortalData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_swf = GetValue("SWF", 0);
			m_exportName = GetValue("ExportName", 0);
			m_width = GetIntegerValue("Width", 0);
			m_height = GetIntegerValue("Height", 0);
		}

		public int GetWidth()
			=> m_width;

		public int GetHeight()
			=> m_height;
	}
}