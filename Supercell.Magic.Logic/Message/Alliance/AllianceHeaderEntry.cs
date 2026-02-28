using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceHeaderEntry
	{
		private LogicLong m_allianceId;
		private LogicData m_localeData;
		private LogicData m_originData;

		private string m_allianceName;

		private int m_allianceBadgeId;
		private AllianceType m_allianceType;
		private int m_memberCount;
		private int m_score;
		private int m_duelScore;
		private int m_requiredScore;
		private int m_requiredDuelScore;
		private int m_winWarCount;
		private int m_lostWarCount;
		private int m_drawWarCount;
		private int m_warFrequency;
		private int m_expPoints;
		private int m_expLevel;
		private int m_consecutiveWinWarCount;

		private bool m_publicWarLog;
		private bool m_amicalWarEnabled;

		public AllianceHeaderEntry()
		{
			m_allianceId = new LogicLong();
			m_expLevel = 1;
		}

		public void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceName = stream.ReadString(900000);
			m_allianceBadgeId = stream.ReadInt();
			m_allianceType = (AllianceType)stream.ReadInt();
			m_memberCount = stream.ReadInt();
			m_score = stream.ReadInt();
			m_duelScore = stream.ReadInt();
			m_requiredScore = stream.ReadInt();
			m_requiredDuelScore = stream.ReadInt();
			m_winWarCount = stream.ReadInt();
			m_lostWarCount = stream.ReadInt();
			m_drawWarCount = stream.ReadInt();
			m_localeData = ByteStreamHelper.ReadDataReference(stream);
			m_warFrequency = stream.ReadInt();
			m_originData = ByteStreamHelper.ReadDataReference(stream);
			m_expPoints = stream.ReadInt();
			m_expLevel = stream.ReadInt();
			m_consecutiveWinWarCount = stream.ReadInt();
			m_publicWarLog = stream.ReadBoolean();
			stream.ReadInt();
			m_amicalWarEnabled = stream.ReadBoolean();
		}

		public void Encode(ByteStream stream)
		{
			stream.WriteLong(m_allianceId);
			stream.WriteString(m_allianceName);
			stream.WriteInt(m_allianceBadgeId);
			stream.WriteInt((int)m_allianceType);
			stream.WriteInt(m_memberCount);
			stream.WriteInt(m_score);
			stream.WriteInt(m_duelScore);
			stream.WriteInt(m_requiredScore);
			stream.WriteInt(m_requiredDuelScore);
			stream.WriteInt(m_winWarCount);
			stream.WriteInt(m_lostWarCount);
			stream.WriteInt(m_drawWarCount);
			ByteStreamHelper.WriteDataReference(stream, m_localeData);
			stream.WriteInt(m_warFrequency);
			ByteStreamHelper.WriteDataReference(stream, m_originData);
			stream.WriteInt(m_expPoints);
			stream.WriteInt(m_expLevel);
			stream.WriteInt(m_consecutiveWinWarCount);
			stream.WriteBoolean(m_publicWarLog);
			stream.WriteInt(0);
			stream.WriteBoolean(m_amicalWarEnabled);
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public AllianceType GetAllianceType()
			=> m_allianceType;

		public void SetAllianceType(AllianceType value)
		{
			m_allianceType = value;
		}

		public int GetRequiredScore()
			=> m_requiredScore;

		public void SetRequiredScore(int value)
		{
			m_requiredScore = value;
		}

		public int GetRequiredDuelScore()
			=> m_requiredDuelScore;

		public void SetRequiredDuelScore(int value)
		{
			m_requiredDuelScore = value;
		}

		public LogicData GetOriginData()
			=> m_originData;

		public void SetOriginData(LogicData value)
		{
			m_originData = value;
		}

		public int GetNumberOfMembers()
			=> m_memberCount;

		public void SetNumberOfMembers(int value)
		{
			m_memberCount = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public int GetWarFrequency()
			=> m_warFrequency;

		public void SetWarFrequency(int value)
		{
			m_warFrequency = value;
		}

		public bool IsPublicWarLog()
			=> m_publicWarLog;

		public void SetPublicWarLog(bool enabled)
		{
			m_publicWarLog = enabled;
		}

		public bool IsAmicalWarEnabled()
			=> m_amicalWarEnabled;

		public void SetArrangedWarEnabled(bool enabled)
		{
			m_amicalWarEnabled = enabled;
		}

		public int GetScore()
			=> m_score;

		public void SetScore(int value)
		{
			m_score = value;
		}

		public int GetDuelScore()
			=> m_duelScore;

		public void SetDuelScore(int value)
		{
			m_duelScore = value;
		}

		public int GetAllianceLevel()
			=> m_expLevel;

		public void SetAllianceLevel(int value)
		{
			m_expLevel = value;
		}

		public int GetAllianceExpPoints()
			=> m_expPoints;

		public void SetAllianceExpPoints(int value)
		{
			m_expPoints = value;
		}

		public void Load(LogicJSONObject jsonObject)
		{
			m_allianceName = jsonObject.GetJSONString("alliance_name").GetStringValue();
			m_allianceBadgeId = jsonObject.GetJSONNumber("badge_id").GetIntValue();
			m_allianceType = (AllianceType)jsonObject.GetJSONNumber("type").GetIntValue();
			m_memberCount = jsonObject.GetJSONNumber("member_count").GetIntValue();
			m_score = jsonObject.GetJSONNumber("score").GetIntValue();
			m_duelScore = jsonObject.GetJSONNumber("duel_score").GetIntValue();
			m_requiredScore = jsonObject.GetJSONNumber("required_score").GetIntValue();
			m_requiredDuelScore = jsonObject.GetJSONNumber("required_duel_score").GetIntValue();
			m_winWarCount = jsonObject.GetJSONNumber("win_war_count").GetIntValue();
			m_lostWarCount = jsonObject.GetJSONNumber("lost_war_count").GetIntValue();
			m_drawWarCount = jsonObject.GetJSONNumber("draw_war_count").GetIntValue();
			m_warFrequency = jsonObject.GetJSONNumber("war_freq").GetIntValue();
			m_expLevel = jsonObject.GetJSONNumber("xp_level").GetIntValue();
			m_expPoints = jsonObject.GetJSONNumber("xp_points").GetIntValue();
			m_consecutiveWinWarCount = jsonObject.GetJSONNumber("cons_win_war_count").GetIntValue();
			m_publicWarLog = jsonObject.GetJSONBoolean("public_war_log").IsTrue();
			m_amicalWarEnabled = jsonObject.GetJSONBoolean("amical_war_enabled").IsTrue();

			LogicJSONNumber localeObject = jsonObject.GetJSONNumber("locale");

			if (localeObject != null)
			{
				m_localeData = LogicDataTables.GetDataById(localeObject.GetIntValue());
			}

			LogicJSONNumber originObject = jsonObject.GetJSONNumber("origin");

			if (originObject != null)
			{
				m_originData = LogicDataTables.GetDataById(originObject.GetIntValue());
			}
		}

		public void Save(LogicJSONObject jsonObject)
		{
			jsonObject.Put("alliance_name", new LogicJSONString(m_allianceName));
			jsonObject.Put("badge_id", new LogicJSONNumber(m_allianceBadgeId));
			jsonObject.Put("type", new LogicJSONNumber((int)m_allianceType));
			jsonObject.Put("member_count", new LogicJSONNumber(m_memberCount));
			jsonObject.Put("score", new LogicJSONNumber(m_score));
			jsonObject.Put("duel_score", new LogicJSONNumber(m_duelScore));
			jsonObject.Put("required_score", new LogicJSONNumber(m_requiredScore));
			jsonObject.Put("required_duel_score", new LogicJSONNumber(m_requiredDuelScore));
			jsonObject.Put("win_war_count", new LogicJSONNumber(m_winWarCount));
			jsonObject.Put("lost_war_count", new LogicJSONNumber(m_lostWarCount));
			jsonObject.Put("draw_war_count", new LogicJSONNumber(m_drawWarCount));
			jsonObject.Put("war_freq", new LogicJSONNumber(m_warFrequency));
			jsonObject.Put("xp_level", new LogicJSONNumber(m_expLevel));
			jsonObject.Put("xp_points", new LogicJSONNumber(m_expPoints));
			jsonObject.Put("cons_win_war_count", new LogicJSONNumber(m_consecutiveWinWarCount));
			jsonObject.Put("public_war_log", new LogicJSONBoolean(m_publicWarLog));
			jsonObject.Put("amical_war_enabled", new LogicJSONBoolean(m_amicalWarEnabled));

			if (m_localeData != null)
			{
				jsonObject.Put("locale", new LogicJSONNumber(m_localeData.GetGlobalID()));
			}

			if (m_originData != null)
			{
				jsonObject.Put("origin", new LogicJSONNumber(m_originData.GetGlobalID()));
			}
		}
	}

	public enum AllianceType
	{
		OPEN = 1,
		INVITE_ONLY,
		CLOSED
	}
}