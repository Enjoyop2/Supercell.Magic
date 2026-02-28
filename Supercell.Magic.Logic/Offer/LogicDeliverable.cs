using System.Text;

using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverable
	{
		public void Decode(ByteStream stream)
		{
			ReadFromJSON(LogicJSONParser.ParseObject(stream.ReadString(900000) ?? string.Empty));
		}

		public void Encode(ChecksumEncoder encoder)
		{
			LogicJSONObject jsonObject = new LogicJSONObject();
			StringBuilder stringBuilder = new StringBuilder();

			WriteToJSON(jsonObject);

			jsonObject.WriteToString(stringBuilder);
			encoder.WriteString(stringBuilder.ToString());
		}

		public virtual void Destruct()
		{
			// Destruct.
		}

		public virtual void WriteToJSON(LogicJSONObject jsonObject)
		{
			Debugger.DoAssert(GetDeliverableType() != 0, "Deliverable type not set!");
			jsonObject.Put("type", new LogicJSONString(GetDeliverableType().ToString()));
		}

		public virtual void ReadFromJSON(LogicJSONObject jsonObject)
		{
			// ReadFromJSON.
		}

		public virtual int GetDeliverableType()
			=> -1;

		public virtual LogicDeliverableBundle Compensate(LogicLevel level)
			=> null;

		public virtual bool Deliver(LogicLevel level)
			=> true;

		public virtual bool CanBeDeliver(LogicLevel level)
			=> true;
	}
}