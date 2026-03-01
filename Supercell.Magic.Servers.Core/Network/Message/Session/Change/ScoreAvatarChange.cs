using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class ScoreAvatarChange : AvatarChange
	{
		public int ScoreGain
		{
			get; set;
		}
		public bool Attacker
		{
			get; set;
		}

		public LogicLeagueData PrevLeagueData
		{
			get; set;
		}
		public LogicLeagueData LeagueData
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			ScoreGain = stream.ReadVInt();
			Attacker = stream.ReadBoolean();

			PrevLeagueData = (LogicLeagueData)ByteStreamHelper.ReadDataReference(stream, DataType.LEAGUE);
			LeagueData = (LogicLeagueData)ByteStreamHelper.ReadDataReference(stream, DataType.LEAGUE);
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(ScoreGain);
			stream.WriteBoolean(Attacker);

			ByteStreamHelper.WriteDataReference(stream, PrevLeagueData);
			ByteStreamHelper.WriteDataReference(stream, LeagueData);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetScore(LogicMath.Max(avatar.GetScore() + ScoreGain, 0));
			avatar.SetLeagueType(LeagueData.GetInstanceID());

			if (PrevLeagueData != null)
			{
				if (Attacker)
				{
					if (ScoreGain < 0)
					{
						avatar.SetAttackLoseCount(avatar.GetAttackLoseCount() + 1);
					}
					else
					{
						avatar.SetAttackWinCount(avatar.GetAttackWinCount() + 1);
					}
				}
				else
				{
					if (ScoreGain < 0)
					{
						avatar.SetDefenseLoseCount(avatar.GetDefenseLoseCount() + 1);
					}
					else
					{
						avatar.SetDefenseWinCount(avatar.GetDefenseWinCount() + 1);
					}
				}
			}
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			memberEntry.SetScore(LogicMath.Max(memberEntry.GetScore() + ScoreGain, 0));
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.SCORE;
	}
}