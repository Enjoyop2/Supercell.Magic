using Supercell.Magic.Servers.Core.Network.Message.Session.Change;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceAvatarChangesMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}
		public LogicArrayList<AvatarChange> AvatarChanges
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
			stream.WriteVInt(AvatarChanges.Size());

			for (int i = 0; i < AvatarChanges.Size(); i++)
				AvatarChangeFactory.Encode(stream, AvatarChanges[i]);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			AvatarChanges = new LogicArrayList<AvatarChange>();

			for (int i = stream.ReadVInt(); i > 0; i--)
				AvatarChanges.Add(AvatarChangeFactory.Decode(stream));
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_AVATAR_CHANGES;
	}
}