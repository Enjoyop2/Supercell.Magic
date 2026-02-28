using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class CreateAvatarStreamRequestMessage : ServerRequestMessage
	{
		public LogicLong OwnerId
		{
			get; set;
		}
		public AvatarStreamEntry Entry
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(OwnerId);
			stream.WriteVInt((int)Entry.GetAvatarStreamEntryType());
			Entry.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			OwnerId = stream.ReadLong();
			Entry = AvatarStreamEntryFactory.CreateStreamEntryByType((AvatarStreamEntryType)stream.ReadVInt());
			Entry.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_AVATAR_STREAM_REQUEST;
	}
}