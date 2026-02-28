using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class CreateAvatarStreamMessage : ServerAccountMessage
	{
		public AvatarStreamEntry Entry
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt((int)Entry.GetAvatarStreamEntryType());
			Entry.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			Entry = AvatarStreamEntryFactory.CreateStreamEntryByType((AvatarStreamEntryType)stream.ReadVInt());
			Entry.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_AVATAR_STREAM;
	}
}