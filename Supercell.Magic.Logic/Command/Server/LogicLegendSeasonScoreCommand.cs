using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.League.Entry;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicLegendSeasonScoreCommand : LogicServerCommand
	{
		private int m_lastSeasonState;
		private int m_lastSeasonYear;
		private int m_lastSeasonMonth;
		private int m_lastSeasonRank;
		private int m_lastSeasonScore;

		private int m_scoreChange;
		private int m_villageType;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			m_lastSeasonState = stream.ReadInt();
			m_lastSeasonYear = stream.ReadInt();
			m_lastSeasonMonth = stream.ReadInt();
			m_lastSeasonScore = stream.ReadInt();
			m_lastSeasonRank = stream.ReadInt();
			m_scoreChange = stream.ReadInt();
			m_villageType = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_lastSeasonState);
			encoder.WriteInt(m_lastSeasonYear);
			encoder.WriteInt(m_lastSeasonMonth);
			encoder.WriteInt(m_lastSeasonScore);
			encoder.WriteInt(m_lastSeasonRank);
			encoder.WriteInt(m_scoreChange);
			encoder.WriteInt(m_villageType);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				LogicLegendSeasonEntry legendSeasonEntry;

				if (m_villageType == 1)
				{
					legendSeasonEntry = playerAvatar.GetLegendSeasonEntryVillage2();
				}
				else
				{
					if (m_villageType != 0)
					{
						return -2;
					}

					legendSeasonEntry = playerAvatar.GetLegendSeasonEntry();
				}

				if (legendSeasonEntry.GetLastSeasonState() != m_lastSeasonState)
				{
					if (m_villageType == 1)
					{
						playerAvatar.SetDuelScore(playerAvatar.GetDuelScore() - m_scoreChange);
						playerAvatar.SetLegendaryScore(playerAvatar.GetLegendaryScoreVillage2() + m_scoreChange);
					}
					else
					{
						playerAvatar.SetScore(playerAvatar.GetScore() - m_scoreChange);
						playerAvatar.SetLegendaryScore(playerAvatar.GetLegendaryScore() + m_scoreChange);
					}

					legendSeasonEntry.SetLastSeasonState(m_lastSeasonState);
					legendSeasonEntry.SetLastSeasonDate(m_lastSeasonYear, m_lastSeasonMonth);
					legendSeasonEntry.SetLastSeasonRank(m_lastSeasonRank);
					legendSeasonEntry.SetLastSeasonScore(m_lastSeasonScore);

					bool bestSeason = false;

					if (legendSeasonEntry.GetBestSeasonState() == 0 ||
						m_lastSeasonRank < legendSeasonEntry.GetBestSeasonRank() ||
						m_lastSeasonRank == legendSeasonEntry.GetBestSeasonRank() &&
						m_lastSeasonScore > legendSeasonEntry.GetBestSeasonScore())
					{
						legendSeasonEntry.SetBestSeasonState(m_lastSeasonState);
						legendSeasonEntry.SetBestSeasonDate(m_lastSeasonYear, m_lastSeasonMonth);
						legendSeasonEntry.SetBestSeasonRank(m_lastSeasonRank);
						legendSeasonEntry.SetBestSeasonScore(m_lastSeasonScore);

						bestSeason = true;
					}

					playerAvatar.GetChangeListener().LegendSeasonScoreChanged(m_lastSeasonState, m_lastSeasonScore, m_scoreChange, bestSeason, m_villageType);
					level.GetGameListener().LegendSeasonScoreChanged(m_lastSeasonState, m_lastSeasonScore, m_scoreChange, bestSeason, m_villageType);

					return 0;
				}
			}

			return -1;
		}

		public void SetDatas(int seasonState, int seasonYear, int seasonMonth, int seasonRank, int seasonScore, int scoreChange, int villageType)
		{
			m_lastSeasonState = seasonState;
			m_lastSeasonYear = seasonYear;
			m_lastSeasonMonth = seasonMonth;
			m_lastSeasonRank = seasonRank;
			m_lastSeasonScore = seasonScore;
			m_scoreChange = scoreChange;
			m_villageType = villageType;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.LEGEND_SEASON_SCORE;
	}
}