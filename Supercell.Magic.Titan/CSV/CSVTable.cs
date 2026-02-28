using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.CSV
{
	public class CSVTable
	{
		private readonly LogicArrayList<string> m_columnNameList;
		private readonly LogicArrayList<CSVColumn> m_columnList;
		private readonly LogicArrayList<CSVRow> m_rowList;

		private readonly CSVNode m_node;

		private readonly int m_size;

		public CSVTable(CSVNode node, int size)
		{
			m_columnNameList = new LogicArrayList<string>();
			m_columnList = new LogicArrayList<CSVColumn>();
			m_rowList = new LogicArrayList<CSVRow>();

			m_node = node;
			m_size = size;
		}

		public void AddAndConvertValue(string value, int columnIndex)
		{
			CSVColumn column = m_columnList[columnIndex];

			if (!string.IsNullOrEmpty(value))
			{
				switch (column.GetColumnType())
				{
					case -1:
					case 0:
						column.AddStringValue(value);
						break;
					case 1:
						column.AddIntegerValue(int.Parse(value));
						break;
					case 2:
						if (bool.TryParse(value, out bool booleanValue))
						{
							column.AddBooleanValue(booleanValue);
						}
						else
						{
							Debugger.Warning(string.Format("CSVTable::addAndConvertValue invalid value '{0}' in Boolean column '{1}', {2}", value,
														   m_columnNameList[columnIndex], GetFileName()));
							column.AddBooleanValue(false);
						}

						break;
				}
			}
			else
			{
				column.AddEmptyValue();
			}
		}

		public void AddColumn(string name)
		{
			m_columnNameList.Add(name);
		}

		public void AddColumnType(int type)
		{
			m_columnList.Add(new CSVColumn(type, m_size));
		}

		public void AddRow(CSVRow row)
		{
			m_rowList.Add(row);
		}

		public void ColumnNamesLoaded()
		{
			m_columnList.EnsureCapacity(m_columnNameList.Size());
		}

		public void CreateRow()
		{
			m_rowList.Add(new CSVRow(this));
		}

		public int GetArraySizeAt(CSVRow row, int columnIdx)
		{
			if (m_rowList.Size() > 0)
			{
				int rowIdx = m_rowList.IndexOf(row);

				if (rowIdx != -1)
				{
					CSVColumn column = m_columnList[columnIdx];
					return column.GetArraySize(m_rowList[rowIdx].GetRowOffset(),
											   rowIdx + 1 >= m_rowList.Size() ? column.GetSize() : m_rowList[rowIdx + 1].GetRowOffset());
				}
			}

			return 0;
		}

		public string GetColumnName(int idx)
			=> m_columnNameList[idx];

		public int GetColumnIndexByName(string name)
			=> m_columnNameList.IndexOf(name);

		public int GetColumnCount()
			=> m_columnNameList.Size();

		public int GetColumnRowCount()
			=> m_columnList[0].GetSize();

		public int GetColumnTypeCount()
			=> m_columnList.Size();

		public string GetFileName()
			=> m_node.GetFileName();

		public bool GetBooleanValue(string name, int index)
			=> GetBooleanValueAt(m_columnNameList.IndexOf(name), index);

		public bool GetBooleanValueAt(int columnIndex, int index)
		{
			if (columnIndex != -1)
			{
				return m_columnList[columnIndex].GetBooleanValue(index);
			}

			return false;
		}

		public int GetIntegerValue(string name, int index)
			=> GetIntegerValueAt(m_columnNameList.IndexOf(name), index);

		public int GetIntegerValueAt(int columnIndex, int index)
		{
			if (columnIndex != -1)
			{
				int value = m_columnList[columnIndex].GetIntegerValue(index);

				if (value == 0x7fffffff)
				{
					value = 0;
				}

				return value;
			}

			return 0;
		}

		public string GetValue(string name, int index)
			=> GetValueAt(m_columnNameList.IndexOf(name), index);

		public string GetValueAt(int columnIndex, int index)
		{
			if (columnIndex != -1)
			{
				return m_columnList[columnIndex].GetStringValue(index);
			}

			return string.Empty;
		}

		public CSVRow GetRowAt(int index)
			=> m_rowList[index];

		public CSVColumn GetCSVColumn(int index)
			=> m_columnList[index];

		public int GetRowCount()
			=> m_rowList.Size();

		public void ValidateColumnTypes()
		{
			if (m_columnNameList.Size() != m_columnList.Size())
			{
				Debugger.Warning($"Column name count {m_columnNameList.Size()}, column type count {m_columnList.Size()}, file {GetFileName()}");
			}
		}
	}
}