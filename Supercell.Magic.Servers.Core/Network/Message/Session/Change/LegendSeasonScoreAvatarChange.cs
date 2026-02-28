using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.League.Entry;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class LegendSeasonScoreAvatarChange : AvatarChange
	{
		public LogicLegendSeasonEntry Entry
		{
			get; set;
		}

		public int ScoreChange
		{
			get; set;
		}
		public bool BestSeason
		{
			get; set;
		}
		public int VillageType
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Entry = new LogicLegendSeasonEntry();
			Entry.Decode(stream);
			ScoreChange = stream.ReadVInt();
			BestSeason = stream.ReadBoolean();
			VillageType = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			Entry.Encode(stream);

			stream.WriteVInt(ScoreChange);
			stream.WriteBoolean(BestSeason);
			stream.WriteVInt(VillageType);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			LogicLegendSeasonEntry legendSeasonEntry = VillageType == 1 ? avatar.GetLegendSeasonEntryVillage2() : avatar.GetLegendSeasonEntry();

			if (legendSeasonEntry.GetLastSeasonState() != Entry.GetLastSeasonState())
			{
				if (VillageType == 1)
				{
					avatar.SetDuelScore(avatar.GetDuelScore() - ScoreChange);
					avatar.SetLegendaryScore(avatar.GetLegendaryScoreVillage2() + ScoreChange);
				}
				else
				{
					avatar.SetScore(avatar.GetScore() - ScoreChange);
					avatar.SetLegendaryScore(avatar.GetLegendaryScore() + ScoreChange);
				}

				legendSeasonEntry.SetLastSeasonState(Entry.GetLastSeasonState());
				legendSeasonEntry.SetLastSeasonDate(Entry.GetLastSeasonYear(), Entry.GetLastSeasonMonth());
				legendSeasonEntry.SetLastSeasonRank(Entry.GetLastSeasonRank());
				legendSeasonEntry.SetLastSeasonScore(Entry.GetLastSeasonScore());

				if (BestSeason)
				{
					legendSeasonEntry.SetBestSeasonState(Entry.GetBestSeasonState());
					legendSeasonEntry.SetBestSeasonDate(Entry.GetBestSeasonYear(), Entry.GetBestSeasonMonth());
					legendSeasonEntry.SetBestSeasonRank(Entry.GetBestSeasonRank());
					legendSeasonEntry.SetBestSeasonScore(Entry.GetBestSeasonScore());
				}
			}
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
			if (VillageType == 1)
				memberEntry.SetDuelScore(memberEntry.GetDuelScore() - ScoreChange);
			else
				memberEntry.SetScore(memberEntry.GetScore() - ScoreChange);
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.LEGEND_SEASON_SCORE;
	}
}