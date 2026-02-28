using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class ForwardLogicRequestMessage : ServerSessionMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}

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
			stream.WriteLong(AccountId);
			stream.WriteShort(MessageType);
			stream.WriteShort(MessageVersion);
			stream.WriteVInt(MessageLength);
			stream.WriteBytesWithoutLength(MessageBytes, MessageLength);
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
			MessageType = stream.ReadShort();
			MessageVersion = stream.ReadShort();
			MessageLength = stream.ReadVInt();
			MessageBytes = stream.ReadBytes(MessageLength, 0xFFFFFF);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.FORWARD_LOGIC_REQUEST_MESSAGE;
	}
}