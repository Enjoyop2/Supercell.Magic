using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class REDPackageStateAvatarChange : AvatarChange
	{
		public int State
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			State = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(State);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetRedPackageState(State);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.RED_PACKAGE_STATE_CHANGED;
	}
}