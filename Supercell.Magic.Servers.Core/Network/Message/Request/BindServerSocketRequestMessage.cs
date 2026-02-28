using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class BindServerSocketRequestMessage : ServerRequestMessage
	{
		public long SessionId
		{
			get; set;
		}
		public int ServerType
		{
			get; set;
		}
		public int ServerId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLongLong(SessionId);
			stream.WriteVInt(ServerType);
			stream.WriteVInt(ServerId);
		}

		public override void Decode(ByteStream stream)
		{
			SessionId = stream.ReadLongLong();
			ServerType = stream.ReadVInt();
			ServerId = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.BIND_SERVER_SOCKET_REQUEST;
	}
}