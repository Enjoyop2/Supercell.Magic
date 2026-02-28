using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class AllianceJoinedAvatarChange : AvatarChange
	{
		public LogicLong AllianceId
		{
			get; set;
		}
		public string AllianceName
		{
			get; set;
		}
		public int AllianceBadgeId
		{
			get; set;
		}
		public int AllianceExpLevel
		{
			get; set;
		}
		public LogicAvatarAllianceRole AllianceRole
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			AllianceId = stream.ReadLong();
			AllianceName = stream.ReadString(900000);
			AllianceBadgeId = stream.ReadVInt();
			AllianceExpLevel = stream.ReadVInt();
			AllianceRole = (LogicAvatarAllianceRole)stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AllianceId);
			stream.WriteString(AllianceName);
			stream.WriteVInt(AllianceBadgeId);
			stream.WriteVInt(AllianceExpLevel);
			stream.WriteVInt((int)AllianceRole);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetAllianceId(AllianceId);
			avatar.SetAllianceName(AllianceName);
			avatar.SetAllianceBadgeId(AllianceBadgeId);
			avatar.SetAllianceLevel(AllianceExpLevel);
			avatar.SetAllianceRole(AllianceRole);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.ALLIANCE_JOINED;
	}
}