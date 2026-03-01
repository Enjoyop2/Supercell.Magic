using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicData
	{
		protected readonly int m_globalId;

		protected short m_tidIndex;
		protected short m_infoTidIndex;
		protected short m_iconExportNameIndex;
		protected short m_iconSWFIndex;

		protected CSVRow m_row;
		protected readonly LogicDataTable m_table;

		public LogicData(CSVRow row, LogicDataTable table)
		{
			m_row = row;
			m_table = table;
			m_globalId = GlobalID.CreateGlobalID((int)table.GetTableIndex() + 1, table.GetItemCount());

			m_tidIndex = -1;
			m_infoTidIndex = -1;
			m_iconSWFIndex = -1;
			m_iconExportNameIndex = -1;
		}

		public virtual void CreateReferences()
		{
			m_iconSWFIndex = (short)m_row.GetColumnIndexByName("IconSWF");
			m_iconExportNameIndex = (short)m_row.GetColumnIndexByName("IconExportName");
			m_tidIndex = (short)m_row.GetColumnIndexByName("TID");
			m_infoTidIndex = (short)m_row.GetColumnIndexByName("InfoTID");
		}

		public virtual void CreateReferences2()
		{
		}

		public void SetCSVRow(CSVRow row)
		{
			m_row = row;
		}

		public int GetArraySize(string column)
			=> m_row.GetArraySize(column);

		public DataType GetDataType()
			=> m_table.GetTableIndex();

		public int GetGlobalID()
			=> m_globalId;

		public int GetInstanceID()
			=> GlobalID.GetInstanceID(m_globalId);

		public int GetColumnIndex(string name)
		{
			int columnIndex = m_row.GetColumnIndexByName(name);

			if (columnIndex == -1)
			{
				Debugger.Warning(string.Format("Unable to find column {0} from {1} ({2})", name, m_row.GetName(), m_table.GetTableName()));
			}

			return m_row.GetColumnIndexByName(name);
		}

		public string GetDebuggerName()
			=> m_row.GetName() + " (" + m_table.GetTableName() + ")";

		public bool GetBooleanValue(string columnName, int index)
			=> m_row.GetBooleanValue(columnName, index);

		public bool GetClampedBooleanValue(string columnName, int index)
			=> m_row.GetClampedBooleanValue(columnName, index);

		public int GetIntegerValue(string columnName, int index)
			=> m_row.GetIntegerValue(columnName, index);

		public int GetClampedIntegerValue(string columnName, int index)
			=> m_row.GetClampedIntegerValue(columnName, index);

		public string GetValue(string columnName, int index)
			=> m_row.GetValue(columnName, index);

		public string GetClampedValue(string columnName, int index)
			=> m_row.GetClampedValue(columnName, index);

		public string GetName()
			=> m_row.GetName();

		public string GetTID()
		{
			if (m_tidIndex == -1)
				return null;
			return m_row.GetValueAt(m_tidIndex, 0);
		}

		public string GetInfoTID()
		{
			if (m_infoTidIndex == -1)
				return null;
			return m_row.GetValueAt(m_infoTidIndex, 0);
		}

		public string GetIconExportName()
		{
			if (m_iconExportNameIndex == -1)
				return null;
			return m_row.GetValueAt(m_iconExportNameIndex, 0);
		}

		public virtual bool IsEnableByCalendar()
			=> false;
	}
}