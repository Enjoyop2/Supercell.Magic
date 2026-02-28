using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.League.Entry;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class LeagueAvatarChange : AvatarChange
	{
		public int LeagueType
		{
			get; set;
		}
		public LogicLong LeagueInstanceId
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			LeagueType = stream.ReadVInt();

			if (stream.ReadBoolean())
			{
				LeagueInstanceId = stream.ReadLong();
			}
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(LeagueType);

			if (LeagueInstanceId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(LeagueInstanceId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetLeagueType(LeagueType);

			if (LeagueType != 0)
			{
				avatar.SetLeagueInstanceId(LeagueInstanceId);
			}
			else
			{
				avatar.SetLeagueInstanceId(null);
				avatar.SetAttackWinCount(0);
				avatar.SetAttackLoseCount(0);
				avatar.SetDefenseWinCount(0);
				avatar.SetDefenseLoseCount(0);
			}
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			memberEntry.SetLeagueType(LeagueType);
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.LEAGUE;
	}
}