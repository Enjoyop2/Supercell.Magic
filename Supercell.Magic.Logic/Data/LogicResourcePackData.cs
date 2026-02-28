using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicResourcePackData : LogicData
	{
		private LogicResourceData m_resourceData;
		private int m_capacityPercentage;

		public LogicResourcePackData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicResourcePackData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_resourceData = LogicDataTables.GetResourceByName(GetValue("Resource", 0), this);
			m_capacityPercentage = GetIntegerValue("CapacityPercentage", 0);
		}

		public LogicResourceData GetResourceData()
			=> m_resourceData;

		public int GetCapacityPercentage()
			=> m_capacityPercentage;
	}
}