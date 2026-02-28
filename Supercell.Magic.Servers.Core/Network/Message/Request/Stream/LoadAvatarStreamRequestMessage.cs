using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class LoadAvatarStreamRequestMessage : ServerRequestMessage
	{
		public LogicLong Id
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(Id);
		}

		public override void Decode(ByteStream stream)
		{
			Id = stream.ReadLong();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LOAD_AVATAR_STREAM_REQUEST;
	}
}