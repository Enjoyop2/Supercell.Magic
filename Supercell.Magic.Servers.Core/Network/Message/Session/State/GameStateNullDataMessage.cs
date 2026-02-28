using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameStateNullDataMessage : ServerSessionMessage
	{
		public override void Encode(ByteStream stream)
		{
		}

		public override void Decode(ByteStream stream)
		{
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_STATE_NULL_DATA;
	}
}