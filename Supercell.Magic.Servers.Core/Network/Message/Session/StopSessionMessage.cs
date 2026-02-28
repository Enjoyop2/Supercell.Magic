using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class StopSessionMessage : ServerSessionMessage
	{
		public int Reason
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Reason);
		}

		public override void Decode(ByteStream stream)
		{
			Reason = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.STOP_SESSION;
	}
}