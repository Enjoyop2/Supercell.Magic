using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class ForwardLogicMessage : ServerSessionMessage
	{
		public short MessageType
		{
			get; set;
		}
		public short MessageVersion
		{
			get; set;
		}
		public int MessageLength
		{
			get; set;
		}

		public byte[] MessageBytes
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteShort(MessageType);
			stream.WriteShort(MessageVersion);
			stream.WriteVInt(MessageLength);
			stream.WriteBytesWithoutLength(MessageBytes, MessageLength);
		}

		public override void Decode(ByteStream stream)
		{
			MessageType = stream.ReadShort();
			MessageVersion = stream.ReadShort();
			MessageLength = stream.ReadVInt();
			MessageBytes = stream.ReadBytes(MessageLength, 0xFFFFFF);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.FORWARD_LOGIC_MESSAGE;
	}
}