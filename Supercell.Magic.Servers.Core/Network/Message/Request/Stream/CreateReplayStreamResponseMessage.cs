using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class CreateReplayStreamResponseMessage : ServerResponseMessage
	{
		public LogicLong Id
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				stream.WriteLong(Id);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				Id = stream.ReadLong();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_REPLAY_STREAM_RESPONSE;
	}
}