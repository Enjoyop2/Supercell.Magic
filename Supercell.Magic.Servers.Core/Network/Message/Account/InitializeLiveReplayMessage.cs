using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class InitializeLiveReplayMessage : ServerAccountMessage
	{
		public byte[] StreamData
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteBytes(StreamData, StreamData.Length);
		}

		public override void Decode(ByteStream stream)
		{
			StreamData = stream.ReadBytes(stream.ReadBytesLength(), 900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.INITIALIZE_LIVE_REPLAY;
	}
}