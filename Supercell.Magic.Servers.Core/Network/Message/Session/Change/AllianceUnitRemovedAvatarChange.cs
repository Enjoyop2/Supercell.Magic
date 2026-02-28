using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class AllianceUnitRemovedAvatarChange : AvatarChange
	{
		public LogicCombatItemData Data
		{
			get; set;
		}
		public int UpgradeLevel
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Data = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream);
			UpgradeLevel = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			ByteStreamHelper.WriteDataReference(stream, Data);
			stream.WriteVInt(UpgradeLevel);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.RemoveAllianceUnit(Data, UpgradeLevel);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.ALLIANCE_UNIT_REMOVED;
	}
}