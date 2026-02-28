using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class FriendEntry
	{
		private LogicLong m_avatarId;
		private LogicLong m_homeId;
		private LogicLong m_allianceId;
		private LogicLong m_liveReplayId;

		private string m_name;
		private string m_allianceName;
		private string m_facebookId;
		private string m_gamecenterId;

		private int m_protectionDurationSeconds;
		private int m_expLevel;
		private int m_leagueType;
		private int m_score;
		private int m_duelScore;
		private int m_friendState;
		private int m_allianceBadgeId;
		private int m_allianceRole;
		private int m_allianceLevel;

		public FriendEntry()
		{
			m_friendState = -1;
			m_allianceBadgeId = -1;
		}

		public void Decode(ByteStream stream)
		{
			m_avatarId = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				m_homeId = stream.ReadLong();
			}

			m_name = stream.ReadString(900000);
			m_facebookId = stream.ReadString(900000);
			m_gamecenterId = stream.ReadString(900000);

			stream.ReadString(900000);

			m_protectionDurationSeconds = stream.ReadInt();
			m_expLevel = stream.ReadInt();
			m_leagueType = stream.ReadInt();
			m_score = stream.ReadInt();
			m_duelScore = stream.ReadInt();
			m_friendState = stream.ReadInt();

			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_allianceId = stream.ReadLong();
				m_allianceBadgeId = stream.ReadInt();
				m_allianceName = stream.ReadString(900000);
				m_allianceRole = stream.ReadInt();
				m_allianceLevel = stream.ReadInt();
			}

			if (stream.ReadBoolean())
			{
				m_liveReplayId = stream.ReadLong();
				stream.ReadInt();
			}
		}

		public void Encode(ByteStream stream)
		{
			stream.WriteLong(m_avatarId);

			if (m_homeId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_homeId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteString(m_name);
			stream.WriteString(m_facebookId);
			stream.WriteString(m_gamecenterId);
			stream.WriteString(null);
			stream.WriteInt(m_protectionDurationSeconds);
			stream.WriteInt(m_expLevel);
			stream.WriteInt(m_leagueType);
			stream.WriteInt(m_score);
			stream.WriteInt(m_duelScore);
			stream.WriteInt(m_friendState);
			stream.WriteInt(0);

			if (m_allianceId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_allianceId);
				stream.WriteInt(m_allianceBadgeId);
				stream.WriteString(m_allianceName);
				stream.WriteInt(m_allianceRole);
				stream.WriteInt(m_allianceLevel);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			if (m_liveReplayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_liveReplayId);
				stream.WriteInt(0);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
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

		public LogicLong GetLiveReplayId()
			=> m_liveReplayId;

		public void SetLiveReplayId(LogicLong value)
		{
			m_liveReplayId = value;
		}

		public string GetName()
			=> m_name;

		public void SetName(string value)
		{
			m_name = value;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public string GetFacebookId()
			=> m_facebookId;

		public void SetFacebookId(string value)
		{
			m_facebookId = value;
		}

		public string GetGamecenterId()
			=> m_gamecenterId;

		public void SetGamecenterId(string value)
		{
			m_gamecenterId = value;
		}

		public int GetProtectionDurationSeconds()
			=> m_protectionDurationSeconds;

		public void SetProtectionDurationSeconds(int value)
		{
			m_protectionDurationSeconds = value;
		}

		public int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int value)
		{
			m_expLevel = value;
		}

		public int GetLeagueType()
			=> m_leagueType;

		public void SetLeagueType(int value)
		{
			m_leagueType = value;
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

		public int GetFriendState()
			=> m_friendState;

		public void SetFriendState(int value)
		{
			m_friendState = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public int GetAllianceRole()
			=> m_allianceRole;

		public void SetAllianceRole(int value)
		{
			m_allianceRole = value;
		}

		public int GetAllianceLevel()
			=> m_allianceLevel;

		public void SetAllianceLevel(int value)
		{
			m_allianceLevel = value;
		}
	}
}