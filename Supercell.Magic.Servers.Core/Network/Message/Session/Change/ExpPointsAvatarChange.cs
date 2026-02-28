using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class ExpPointsAvatarChange : AvatarChange
	{
		public int Points
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Points = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Points);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetExpPoints(avatar.GetExpPoints() + Points);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.EXP_POINTS;
	}
}