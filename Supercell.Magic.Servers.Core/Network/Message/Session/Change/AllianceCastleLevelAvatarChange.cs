using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class AllianceCastleLevelAvatarChange : AvatarChange
	{
		public int Level
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Level = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Level);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetAllianceCastleLevel(Level);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.ALLIANCE_CASTLE_LEVEL;
	}
}