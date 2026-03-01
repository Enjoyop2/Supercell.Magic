namespace Supercell.Magic.Logic.Data
{
	public class LogicDataTableResource
	{
		private string m_fileName;

		private DataType m_tableIndex;
		private int m_type;

		public LogicDataTableResource(string fileName, DataType tableIndex, int type)
		{
			m_fileName = fileName;
			m_tableIndex = tableIndex;
			m_type = type;
		}

		public void Destruct()
		{
			m_fileName = null;
			m_tableIndex = 0;
			m_type = 0;
		}

		public string GetFileName()
			=> m_fileName;

		public DataType GetTableIndex()
			=> m_tableIndex;

		public int GetTableType()
			=> m_type;
	}
}