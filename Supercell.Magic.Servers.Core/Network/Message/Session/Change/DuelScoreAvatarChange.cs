using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class DuelScoreAvatarChange : AvatarChange
	{
		public int DuelScoreGain
		{
			get; set;
		}
		public int ResultType
		{
			get; set;
		}
		public bool Attacker
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			DuelScoreGain = stream.ReadVInt();
			ResultType = stream.ReadVInt();
			Attacker = stream.ReadBoolean();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(DuelScoreGain);
			stream.WriteVInt(ResultType);
			stream.WriteBoolean(Attacker);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetDuelScore(avatar.GetDuelScore() + DuelScoreGain);

			switch (ResultType)
			{
				case 0:
					avatar.SetDuelLoseCount(avatar.GetDuelLoseCount() + 1);
					break;
				case 1:
					avatar.SetDuelWinCount(avatar.GetDuelWinCount() + 1);
					break;
				case 2:
					avatar.SetDuelDrawCount(avatar.GetDuelDrawCount() + 1);
					break;
			}
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			memberEntry.SetDuelScore(memberEntry.GetDuelScore() + DuelScoreGain);
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.DUEL_SCORE;
	}
}