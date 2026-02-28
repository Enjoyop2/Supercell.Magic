using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicGlobalData : LogicData
	{
		private int m_numberValue;
		private bool m_booleanValue;
		private string m_textValue;

		private int[] m_numberArray;
		private int[] m_altNumberArray;

		private string[] m_stringArray;

		public LogicGlobalData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicGlobalData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			int size = m_row.GetBiggestArraySize();

			m_numberArray = new int[size];
			m_altNumberArray = new int[size];
			m_stringArray = new string[size];

			m_numberValue = GetIntegerValue("NumberValue", 0);
			m_booleanValue = GetBooleanValue("BooleanValue", 0);
			m_textValue = GetValue("TextValue", 0);

			for (int i = 0; i < size; i++)
			{
				m_numberArray[i] = GetIntegerValue("NumberArray", i);
				m_altNumberArray[i] = GetIntegerValue("AltNumberArray", i);
				m_stringArray[i] = GetValue("StringArray", i);
			}
		}

		public int GetNumberValue()
			=> m_numberValue;

		public bool GetBooleanValue()
			=> m_booleanValue;

		public string GetTextValue()
			=> m_textValue;

		public int GetNumberArraySize()
			=> m_numberArray.Length;

		public int GetNumberArray(int index)
			=> m_numberArray[index];

		public int GetAltNumberArray(int index)
			=> m_altNumberArray[index];
	}
}