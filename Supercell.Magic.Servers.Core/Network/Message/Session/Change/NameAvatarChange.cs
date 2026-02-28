using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class NameAvatarChange : AvatarChange
	{
		public string Name
		{
			get; set;
		}
		public int NameChangeState
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Name = stream.ReadString(900000);
			NameChangeState = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteString(Name);
			stream.WriteVInt(NameChangeState);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetName(Name);
			avatar.SetNameSetByUser(true);
			avatar.SetNameChangeState(NameChangeState);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			memberEntry.SetName(Name);
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.NAME;
	}
}