using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class BindServerSocketResponseMessage : ServerResponseMessage
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
			if (Success)
			{
				stream.WriteVInt(ServerType);
				stream.WriteVInt(ServerId);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				ServerType = stream.ReadVInt();
				ServerId = stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.BIND_SERVER_SOCKET_RESPONSE;
	}
}