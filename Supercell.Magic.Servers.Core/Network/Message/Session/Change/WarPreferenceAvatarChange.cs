using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class WarPreferenceAvatarChange : AvatarChange
	{
		public int Preference
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Preference = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Preference);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetWarPreference(Preference);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			memberEntry.SetWarPreference(Preference);
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.WAR_PREFERENCE;
	}
}