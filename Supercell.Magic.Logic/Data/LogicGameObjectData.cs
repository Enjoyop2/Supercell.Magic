using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public abstract class LogicGameObjectData : LogicData
	{
		protected int m_villageType;

		protected LogicGameObjectData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			m_villageType = -1;
		}

		public override void CreateReferences()
		{
			base.CreateReferences();
			int columnIndex = m_row.GetColumnIndexByName("VillageType");

			if (columnIndex > 0)
			{
				m_villageType = m_row.GetIntegerValueAt(columnIndex, 0);

				if (m_villageType >= 2)
				{
					Debugger.Error("invalid VillageType");
				}
			}
		}

		public int GetVillageType()
			=> m_villageType;

		public bool IsEnabledInVillageType(int villageType)
			=> m_villageType == -1 || m_villageType == villageType;
	}
}