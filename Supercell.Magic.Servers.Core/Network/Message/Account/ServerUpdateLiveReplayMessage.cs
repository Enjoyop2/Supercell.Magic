using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class ServerUpdateLiveReplayMessage : ServerAccountMessage
	{
		public int Milliseconds
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Milliseconds);
		}

		public override void Decode(ByteStream stream)
		{
			Milliseconds = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SERVER_UPDATE_LIVE_REPLAY;
	}
}