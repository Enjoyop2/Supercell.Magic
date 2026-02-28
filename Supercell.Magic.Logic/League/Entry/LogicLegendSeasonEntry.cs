using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.League.Entry
{
	public class LogicLegendSeasonEntry
	{
		private int m_bestSeasonState;
		private int m_bestSeasonMonth;
		private int m_bestSeasonYear;
		private int m_bestSeasonRank;
		private int m_bestSeasonScore;

		private int m_lastSeasonState;
		private int m_lastSeasonMonth;
		private int m_lastSeasonYear;
		private int m_lastSeasonRank;
		private int m_lastSeasonScore;

		public void Destruct()
		{
			// Destruct.
		}

		public void Decode(ByteStream stream)
		{
			m_bestSeasonState = stream.ReadInt();
			m_bestSeasonYear = stream.ReadInt();
			m_bestSeasonMonth = stream.ReadInt();
			m_bestSeasonRank = stream.ReadInt();
			m_bestSeasonScore = stream.ReadInt();
			m_lastSeasonState = stream.ReadInt();
			m_lastSeasonYear = stream.ReadInt();
			m_lastSeasonMonth = stream.ReadInt();
			m_lastSeasonRank = stream.ReadInt();
			m_lastSeasonScore = stream.ReadInt();
		}

		public void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_bestSeasonState);
			encoder.WriteInt(m_bestSeasonYear);
			encoder.WriteInt(m_bestSeasonMonth);
			encoder.WriteInt(m_bestSeasonRank);
			encoder.WriteInt(m_bestSeasonScore);
			encoder.WriteInt(m_lastSeasonState);
			encoder.WriteInt(m_lastSeasonYear);
			encoder.WriteInt(m_lastSeasonMonth);
			encoder.WriteInt(m_lastSeasonRank);
			encoder.WriteInt(m_lastSeasonScore);
		}

		public int GetLastSeasonState()
			=> m_lastSeasonState;

		public void SetLastSeasonState(int value)
		{
			m_lastSeasonState = value;
		}

		public int GetLastSeasonYear()
			=> m_lastSeasonYear;

		public int GetLastSeasonMonth()
			=> m_lastSeasonMonth;

		public void SetLastSeasonDate(int year, int month)
		{
			m_lastSeasonYear = year;
			m_lastSeasonMonth = month;
		}

		public int GetLastSeasonScore()
			=> m_lastSeasonScore;

		public void SetLastSeasonScore(int score)
		{
			m_lastSeasonScore = score;
		}

		public int GetLastSeasonRank()
			=> m_lastSeasonRank;

		public void SetLastSeasonRank(int score)
		{
			m_lastSeasonRank = score;
		}

		public int GetBestSeasonState()
			=> m_bestSeasonState;

		public void SetBestSeasonState(int value)
		{
			m_bestSeasonState = value;
		}

		public int GetBestSeasonYear()
			=> m_bestSeasonYear;

		public int GetBestSeasonMonth()
			=> m_bestSeasonMonth;

		public void SetBestSeasonDate(int year, int month)
		{
			m_bestSeasonYear = year;
			m_bestSeasonMonth = month;
		}

		public int GetBestSeasonScore()
			=> m_bestSeasonScore;

		public void SetBestSeasonScore(int score)
		{
			m_bestSeasonScore = score;
		}

		public int GetBestSeasonRank()
			=> m_bestSeasonRank;

		public void SetBestSeasonRank(int score)
		{
			m_bestSeasonRank = score;
		}

		public void ReadFromJSON(LogicJSONObject jsonObject)
		{
			m_bestSeasonState = LogicJSONHelper.GetInt(jsonObject, "best_season_state");
			m_bestSeasonYear = LogicJSONHelper.GetInt(jsonObject, "best_season_year");
			m_bestSeasonMonth = LogicJSONHelper.GetInt(jsonObject, "best_season_month");
			m_bestSeasonRank = LogicJSONHelper.GetInt(jsonObject, "best_season_rank");
			m_bestSeasonScore = LogicJSONHelper.GetInt(jsonObject, "best_season_score");
			m_lastSeasonState = LogicJSONHelper.GetInt(jsonObject, "last_season_state");
			m_lastSeasonYear = LogicJSONHelper.GetInt(jsonObject, "last_season_year");
			m_lastSeasonMonth = LogicJSONHelper.GetInt(jsonObject, "last_season_month");
			m_lastSeasonRank = LogicJSONHelper.GetInt(jsonObject, "last_season_rank");
			m_lastSeasonScore = LogicJSONHelper.GetInt(jsonObject, "last_season_score");
		}

		public void WriteToJSON(LogicJSONObject jsonObject)
		{
			jsonObject.Put("best_season_state", new LogicJSONNumber(m_bestSeasonState));
			jsonObject.Put("best_season_year", new LogicJSONNumber(m_bestSeasonYear));
			jsonObject.Put("best_season_month", new LogicJSONNumber(m_bestSeasonMonth));
			jsonObject.Put("best_season_rank", new LogicJSONNumber(m_bestSeasonRank));
			jsonObject.Put("best_season_score", new LogicJSONNumber(m_bestSeasonScore));
			jsonObject.Put("last_season_state", new LogicJSONNumber(m_lastSeasonState));
			jsonObject.Put("last_season_year", new LogicJSONNumber(m_lastSeasonYear));
			jsonObject.Put("last_season_month", new LogicJSONNumber(m_lastSeasonMonth));
			jsonObject.Put("last_season_rank", new LogicJSONNumber(m_lastSeasonRank));
			jsonObject.Put("last_season_score", new LogicJSONNumber(m_lastSeasonScore));
		}
	}
}