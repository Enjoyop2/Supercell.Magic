using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarDuelRankingEntry : RankingEntry
	{
		private int m_expLevel;
		private int m_duelWinCount;
		private int m_duelDrawCount;
		private int m_duelLoseCount;
		private int m_allianceBadgeId;

		private string m_country;
		private string m_allianceName;

		private LogicLong m_homeId;
		private LogicLong m_allianceId;

		public AvatarDuelRankingEntry()
		{
			m_allianceBadgeId = -1;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_expLevel);
			stream.WriteInt(m_duelWinCount);
			stream.WriteInt(m_duelDrawCount);
			stream.WriteInt(m_duelLoseCount);
			stream.WriteString(m_country);
			stream.WriteLong(m_homeId);
			stream.WriteInt(0);
			stream.WriteInt(0);

			if (m_allianceId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_allianceId);
				stream.WriteString(m_allianceName);
				stream.WriteInt(m_allianceBadgeId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_expLevel = stream.ReadInt();
			m_duelWinCount = stream.ReadInt();
			m_duelDrawCount = stream.ReadInt();
			m_duelLoseCount = stream.ReadInt();
			m_country = stream.ReadString(900000);
			m_homeId = stream.ReadLong();

			stream.ReadInt();
			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_allianceId = stream.ReadLong();
				m_allianceName = stream.ReadString(900000);
				m_allianceBadgeId = stream.ReadInt();
			}
		}

		public int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int value)
		{
			m_expLevel = value;
		}

		public int GetDuelWinCount()
			=> m_duelWinCount;

		public void SetDuelWinCount(int value)
		{
			m_duelWinCount = value;
		}

		public int GetDuelLoseCount()
			=> m_duelLoseCount;

		public void SetDuelLoseCount(int value)
		{
			m_duelLoseCount = value;
		}

		public int GetDuelDrawCount()
			=> m_duelDrawCount;

		public void SetDuelDrawCount(int value)
		{
			m_duelDrawCount = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public string GetCountry()
			=> m_country;

		public void SetCountry(string value)
		{
			m_country = value;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}
	}
}