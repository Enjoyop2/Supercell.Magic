using System.Text;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONNull : LogicJSONNode
	{
		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.NULL;

		public override void WriteToString(StringBuilder builder)
		{
			builder.Append("null");
		}
	}
}