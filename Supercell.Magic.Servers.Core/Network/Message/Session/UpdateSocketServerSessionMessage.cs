using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class UpdateSocketServerSessionMessage : ServerSessionMessage
	{
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
			stream.WriteVInt(ServerType);
			stream.WriteVInt(ServerId);
		}

		public override void Decode(ByteStream stream)
		{
			ServerType = stream.ReadVInt();
			ServerId = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.UPDATE_SOCKET_SERVER_SESSION;
	}
}