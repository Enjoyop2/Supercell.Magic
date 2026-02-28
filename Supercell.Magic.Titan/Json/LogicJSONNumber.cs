using System.Text;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONNumber : LogicJSONNode
	{
		private int m_value;

		public LogicJSONNumber()
		{
			m_value = 0;
		}

		public LogicJSONNumber(int value)
		{
			m_value = value;
		}

		public int GetIntValue()
			=> m_value;

		public void SetIntValue(int value)
		{
			m_value = value;
		}

		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.NUMBER;

		public override void WriteToString(StringBuilder builder)
		{
			builder.Append(m_value);
		}
	}
}