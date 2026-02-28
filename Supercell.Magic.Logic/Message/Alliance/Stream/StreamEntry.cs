using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public abstract class StreamEntry
	{
		private LogicLong m_id;
		private LogicLong m_senderHomeId;
		private LogicLong m_senderAvatarId;

		private string m_senderName;

		private int m_senderLevel;
		private int m_senderLeagueType;
		private LogicAvatarAllianceRole m_senderRole;
		private int m_ageSeconds;

		private bool m_removed;

		protected StreamEntry()
		{
			m_id = new LogicLong();
		}

		public virtual void Destruct()
		{
			m_id = null;
			m_senderHomeId = null;
			m_senderAvatarId = null;
			m_senderName = null;
		}

		public virtual void Decode(ByteStream stream)
		{
			m_id = stream.ReadLong();

			bool hasSenderAvatarId = stream.ReadBoolean();
			bool hasSenderHomeId = stream.ReadBoolean();

			m_removed = stream.ReadBoolean();

			if (hasSenderAvatarId)
			{
				m_senderAvatarId = stream.ReadLong();
			}

			if (hasSenderHomeId)
			{
				m_senderHomeId = stream.ReadLong();
			}

			m_senderName = stream.ReadString(900000);
			m_senderLevel = stream.ReadInt();
			m_senderLeagueType = stream.ReadInt();
			m_senderRole = (LogicAvatarAllianceRole)stream.ReadInt();
			m_ageSeconds = stream.ReadInt();
		}

		public virtual void Encode(ByteStream stream)
		{
			stream.WriteLong(m_id);
			stream.WriteBoolean(m_senderAvatarId != null);
			stream.WriteBoolean(m_senderHomeId != null);
			stream.WriteBoolean(m_removed);

			if (m_senderAvatarId != null)
			{
				stream.WriteLong(m_senderAvatarId);
			}

			if (m_senderHomeId != null)
			{
				stream.WriteLong(m_senderHomeId);
			}

			stream.WriteString(m_senderName);
			stream.WriteInt(m_senderLevel);
			stream.WriteInt(m_senderLeagueType);
			stream.WriteInt((int)m_senderRole);
			stream.WriteInt(m_ageSeconds);
		}

		public LogicLong GetSenderAvatarId()
			=> m_senderAvatarId;

		public void SetSenderAvatarId(LogicLong id)
		{
			m_senderAvatarId = id;
		}

		public LogicLong GetSenderHomeId()
			=> m_senderHomeId;

		public void SetSenderHomeId(LogicLong id)
		{
			m_senderHomeId = id;
		}

		public LogicLong GetId()
			=> m_id;

		public void SetId(LogicLong id)
		{
			m_id = id;
		}

		public string GetSenderName()
			=> m_senderName;

		public void SetSenderName(string name)
		{
			m_senderName = name;
		}

		public int GetSenderLevel()
			=> m_senderLevel;

		public void SetSenderLevel(int value)
		{
			m_senderLevel = value;
		}

		public int GetSenderLeagueType()
			=> m_senderLeagueType;

		public void SetSenderLeagueType(int value)
		{
			m_senderLeagueType = value;
		}

		public LogicAvatarAllianceRole GetSenderRole()
			=> m_senderRole;

		public void SetSenderRole(LogicAvatarAllianceRole value)
		{
			m_senderRole = value;
		}

		public int GetAgeSeconds()
			=> m_ageSeconds;

		public void SetAgeSeconds(int value)
		{
			m_ageSeconds = value;
		}

		public bool IsRemoved()
			=> m_removed;

		public void SetRemoved(bool removed)
		{
			m_removed = removed;
		}

		public abstract StreamEntryType GetStreamEntryType();

		public virtual void Save(LogicJSONObject baseObject)
		{
			if (m_senderAvatarId != null)
			{
				baseObject.Put("sender_avatar_id_high", new LogicJSONNumber(m_senderAvatarId.GetHigherInt()));
				baseObject.Put("sender_avatar_id_low", new LogicJSONNumber(m_senderAvatarId.GetLowerInt()));
			}

			if (m_senderHomeId != null)
			{
				baseObject.Put("sender_home_id_high", new LogicJSONNumber(m_senderHomeId.GetHigherInt()));
				baseObject.Put("sender_home_id_low", new LogicJSONNumber(m_senderHomeId.GetLowerInt()));
			}

			baseObject.Put("sender_name", new LogicJSONString(m_senderName));
			baseObject.Put("sender_level", new LogicJSONNumber(m_senderLevel));
			baseObject.Put("sender_league_type", new LogicJSONNumber(m_senderLeagueType));
			baseObject.Put("sender_role", new LogicJSONNumber((int)m_senderRole));
			baseObject.Put("removed", new LogicJSONBoolean(m_removed));
		}

		public virtual void Load(LogicJSONObject jsonObject)
		{
			LogicJSONNumber senderAvatarIdHighObject = jsonObject.GetJSONNumber("sender_avatar_id_high");
			LogicJSONNumber senderAvatarIdLowObject = jsonObject.GetJSONNumber("sender_avatar_id_low");

			if (senderAvatarIdHighObject != null && senderAvatarIdLowObject != null)
			{
				m_senderAvatarId = new LogicLong(senderAvatarIdHighObject.GetIntValue(), senderAvatarIdLowObject.GetIntValue());
			}

			LogicJSONNumber senderHomeIdHighObject = jsonObject.GetJSONNumber("sender_home_id_high");
			LogicJSONNumber senderHomeIdLowObject = jsonObject.GetJSONNumber("sender_home_id_low");

			if (senderHomeIdHighObject != null && senderHomeIdLowObject != null)
			{
				m_senderHomeId = new LogicLong(senderHomeIdHighObject.GetIntValue(), senderHomeIdLowObject.GetIntValue());
			}


			m_senderName = LogicJSONHelper.GetString(jsonObject, "sender_name");
			m_senderLevel = LogicJSONHelper.GetInt(jsonObject, "sender_level");
			m_senderLeagueType = LogicJSONHelper.GetInt(jsonObject, "sender_league_type");
			m_senderRole = (LogicAvatarAllianceRole)LogicJSONHelper.GetInt(jsonObject, "sender_role");
			m_removed = LogicJSONHelper.GetBool(jsonObject, "removed");
		}
	}

	public enum StreamEntryType
	{
		DONATE = 1,
		CHAT = 2,
		JOIN_REQUEST = 3,
		ALLIANCE_EVENT = 4,
		REPLAY = 5,
		CHALLENGE_REPLAY = 11,
		CHALLENGE = 12,
		ALLIANCE_GIFT = 16,
		VERSUS_BATTLE_REQUEST = 18,
		VERSUS_BATTLE_REPLAY = 19,
		DUEL_REPLAY = 21
	}
}