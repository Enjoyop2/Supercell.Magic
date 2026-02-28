using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class LoadAvatarStreamOfTypeResponseMessage : ServerResponseMessage
	{
		public LogicArrayList<AvatarStreamEntry> StreamList
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				stream.WriteVInt(StreamList.Size());

				for (int i = 0; i < StreamList.Size(); i++)
				{
					stream.WriteVInt((int)StreamList[i].GetAvatarStreamEntryType());
					StreamList[i].Encode(stream);
				}
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				StreamList = new LogicArrayList<AvatarStreamEntry>();

				for (int i = stream.ReadVInt() - 1; i >= 0; i--)
				{
					AvatarStreamEntry streamEntry = AvatarStreamEntryFactory.CreateStreamEntryByType((AvatarStreamEntryType)stream.ReadVInt());
					streamEntry.Decode(stream);
					StreamList.Add(streamEntry);
				}
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LOAD_AVATAR_STREAM_OF_TYPE_RESPONSE;

		public enum Reason
		{
			GENERIC,
			ALREADY_SENT
		}
	}
}