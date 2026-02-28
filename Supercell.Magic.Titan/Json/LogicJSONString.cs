using System.Text;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONString : LogicJSONNode
	{
		private string m_value;

		public LogicJSONString(string value)
		{
			m_value = value;
		}

		public string GetStringValue()
			=> m_value;

		public void SetStringValue(string value)
		{
			m_value = value;
		}

		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.STRING;

		public override void WriteToString(StringBuilder builder)
		{
			LogicJSONParser.WriteString(m_value, builder);
		}
	}
}