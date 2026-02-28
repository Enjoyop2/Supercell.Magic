using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Titan.CSV
{
	public class CSVRow
	{
		private readonly int m_rowOffset;
		private readonly CSVTable m_table;

		public CSVRow(CSVTable table)
		{
			m_table = table;
			m_rowOffset = table.GetColumnRowCount();
		}

		public int GetArraySize(string column)
		{
			int columnIndex = GetColumnIndexByName(column);

			if (columnIndex == -1)
			{
				return 0;
			}

			return m_table.GetArraySizeAt(this, columnIndex);
		}

		public int GetBiggestArraySize()
		{
			int columnCount = m_table.GetColumnCount();
			int maxSize = 1;

			for (int i = columnCount - 1; i >= 0; i--)
			{
				maxSize = LogicMath.Max(m_table.GetArraySizeAt(this, i), maxSize);
			}

			return maxSize;
		}

		public int GetColumnCount()
			=> m_table.GetColumnCount();

		public int GetColumnIndexByName(string name)
			=> m_table.GetColumnIndexByName(name);

		public bool GetBooleanValue(string columnName, int index)
			=> m_table.GetBooleanValue(columnName, m_rowOffset + index);

		public bool GetBooleanValueAt(int columnIndex, int index)
			=> m_table.GetBooleanValueAt(columnIndex, m_rowOffset + index);

		public bool GetClampedBooleanValue(string columnName, int index)
		{
			int columnIndex = GetColumnIndexByName(columnName);

			if (columnIndex != -1)
			{
				int arraySize = m_table.GetArraySizeAt(this, columnIndex);

				if (index >= arraySize || arraySize < 1)
				{
					index = LogicMath.Max(arraySize - 1, 0);
				}

				return m_table.GetBooleanValueAt(columnIndex, m_rowOffset + index);
			}

			return false;
		}

		public int GetIntegerValue(string columnName, int index)
			=> m_table.GetIntegerValue(columnName, m_rowOffset + index);

		public int GetIntegerValueAt(int columnIndex, int index)
			=> m_table.GetIntegerValueAt(columnIndex, m_rowOffset + index);

		public int GetClampedIntegerValue(string columnName, int index)
		{
			int columnIndex = GetColumnIndexByName(columnName);

			if (columnIndex != -1)
			{
				int arraySize = m_table.GetArraySizeAt(this, columnIndex);

				if (index >= arraySize || arraySize < 1)
				{
					index = LogicMath.Max(arraySize - 1, 0);
				}

				return m_table.GetIntegerValueAt(columnIndex, m_rowOffset + index);
			}

			return 0;
		}

		public string GetValue(string columnName, int index)
			=> m_table.GetValue(columnName, m_rowOffset + index);

		public string GetValueAt(int columnIndex, int index)
			=> m_table.GetValueAt(columnIndex, m_rowOffset + index);

		public string GetClampedValue(string columnName, int index)
		{
			int columnIndex = GetColumnIndexByName(columnName);

			if (columnIndex != -1)
			{
				int arraySize = m_table.GetArraySizeAt(this, columnIndex);

				if (index >= arraySize || arraySize < 1)
				{
					index = LogicMath.Max(arraySize - 1, 0);
				}

				return m_table.GetValueAt(columnIndex, m_rowOffset + index);
			}

			return string.Empty;
		}

		public string GetName()
			=> m_table.GetValueAt(0, m_rowOffset);

		public int GetRowOffset()
			=> m_rowOffset;
	}
}