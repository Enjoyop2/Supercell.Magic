using System.Text;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONBoolean : LogicJSONNode
	{
		private readonly bool m_value;

		public LogicJSONBoolean(bool value)
		{
			m_value = value;
		}

		public bool IsTrue()
			=> m_value;

		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.BOOLEAN;

		public override void WriteToString(StringBuilder builder)
		{
			builder.Append(m_value ? "true" : "false");
		}
	}
}