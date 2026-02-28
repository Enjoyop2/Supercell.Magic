using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.CSV
{
	public class CSVColumn
	{
		public const int BOOLEAN_VALUE_NOT_SET = 0x2;
		public const int INT_VALUE_NOT_SET = 0x7FFFFFFF;

		private readonly LogicArrayList<byte> m_booleanValues;
		private readonly LogicArrayList<int> m_integerValues;
		private readonly LogicArrayList<string> m_stringValues;

		private readonly int m_columnType;

		public CSVColumn(int type, int size)
		{
			m_columnType = type;

			m_integerValues = new LogicArrayList<int>();
			m_booleanValues = new LogicArrayList<byte>();
			m_stringValues = new LogicArrayList<string>();

			switch (type)
			{
				case -1:
				case 0:
					m_stringValues.EnsureCapacity(size);
					break;
				case 1:
					m_integerValues.EnsureCapacity(size);
					break;
				case 2:
					m_booleanValues.EnsureCapacity(size);
					break;
				default:
					Debugger.Error("Invalid CSVColumn type");
					break;
			}
		}

		public void AddEmptyValue()
		{
			switch (m_columnType)
			{
				case -1:
				case 0:
					{
						m_stringValues.Add(string.Empty);
						break;
					}

				case 1:
					{
						m_integerValues.Add(CSVColumn.INT_VALUE_NOT_SET);
						break;
					}

				case 2:
					{
						m_booleanValues.Add(CSVColumn.BOOLEAN_VALUE_NOT_SET);
						break;
					}
			}
		}

		public void AddBooleanValue(bool value)
		{
			m_booleanValues.Add((byte)(value ? 1 : 0));
		}

		public void AddIntegerValue(int value)
		{
			m_integerValues.Add(value);
		}

		public void AddStringValue(string value)
		{
			m_stringValues.Add(value);
		}

		public int GetArraySize(int startOffset, int endOffset)
		{
			switch (m_columnType)
			{
				default:
					for (int i = endOffset - 1; i + 1 > startOffset; i--)
					{
						if (m_stringValues[i].Length > 0)
						{
							return i - startOffset + 1;
						}
					}

					break;
				case 1:
					for (int i = endOffset - 1; i + 1 > startOffset; i--)
					{
						if (m_integerValues[i] != CSVColumn.INT_VALUE_NOT_SET)
						{
							return i - startOffset + 1;
						}
					}

					break;

				case 2:
					for (int i = endOffset - 1; i + 1 > startOffset; i--)
					{
						if (m_booleanValues[i] != CSVColumn.BOOLEAN_VALUE_NOT_SET)
						{
							return i - startOffset + 1;
						}
					}

					break;
			}

			return 0;
		}

		public bool GetBooleanValue(int index)
			=> m_booleanValues[index] == 1;

		public int GetIntegerValue(int index)
			=> m_integerValues[index];

		public string GetStringValue(int index)
			=> m_stringValues[index];

		public int GetSize()
		{
			switch (m_columnType)
			{
				case -1:
				case 0:
					return m_stringValues.Size();
				case 1:
					return m_integerValues.Size();
				case 2:
					return m_booleanValues.Size();
				default:
					return 0;
			}
		}

		public int GetColumnType()
			=> m_columnType;
	}
}