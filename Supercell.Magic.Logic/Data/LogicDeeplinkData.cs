using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicDeeplinkData : LogicData
	{
		private string[] m_parameterType;
		private string[] m_parameterName;
		private string[] m_description;

		public LogicDeeplinkData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicDeeplinkData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			int size = GetArraySize("ParameterType");

			m_parameterType = new string[size];
			m_parameterName = new string[size];
			m_description = new string[size];

			for (int i = 0; i < size; i++)
			{
				m_parameterType[i] = GetValue("ParameterType", i);
				m_parameterName[i] = GetValue("ParameterName", i);
				m_description[i] = GetValue("Description", i);
			}
		}

		public string GetParameterType(int index)
			=> m_parameterType[index];

		public string GetParameterName(int index)
			=> m_parameterName[index];

		public string GetDescription(int index)
			=> m_description[index];
	}
}