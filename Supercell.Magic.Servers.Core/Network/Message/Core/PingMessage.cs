using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class PingMessage : ServerCoreMessage
	{
		public override void Encode(ByteStream stream)
		{
		}

		public override void Decode(ByteStream stream)
		{
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.PING;
	}
}