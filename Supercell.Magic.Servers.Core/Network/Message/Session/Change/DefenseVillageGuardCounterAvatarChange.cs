using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class DefenseVillageGuardCounterAvatarChange : AvatarChange
	{
		public int Count
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Count = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Count);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetDefenseVillageGuardCounter(Count);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.DEFENSE_VILLAGE_GUARD_COUNTER;
	}
}