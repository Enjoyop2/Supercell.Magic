using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class EndLiveReplayMessage : ServerAccountMessage
	{
		public override void Encode(ByteStream stream)
		{
		}

		public override void Decode(ByteStream stream)
		{
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.END_LIVE_REPLAY;
	}
}