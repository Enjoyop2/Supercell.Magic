using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class CreateReplayStreamRequestMessage : ServerRequestMessage
	{
		public string JSON
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteString(JSON);
		}

		public override void Decode(ByteStream stream)
		{
			JSON = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_REPLAY_STREAM_REQUEST;
	}
}