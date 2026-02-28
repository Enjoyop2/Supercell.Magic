using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AvatarStreamEntry
	{
		private LogicLong m_id;
		private LogicLong m_senderAvatarId;

		private string m_senderName;

		private int m_senderExpLevel;
		private int m_senderLeagueType;
		private int m_ageSeconds;

		private bool m_new;
		private bool m_dismiss;

		public AvatarStreamEntry()
		{
			m_new = true;
		}

		public virtual void Destruct()
		{
			m_id = null;
			m_senderAvatarId = null;
			m_senderName = null;
		}

		public virtual void Encode(ByteStream stream)
		{
			stream.WriteLong(m_id);

			if (m_senderAvatarId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_senderAvatarId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteString(m_senderName);
			stream.WriteInt(m_senderExpLevel);
			stream.WriteInt(m_senderLeagueType);
			stream.WriteInt(m_ageSeconds);
			stream.WriteBoolean(m_dismiss);
			stream.WriteBoolean(m_new);
		}

		public virtual void Decode(ByteStream stream)
		{
			m_id = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				m_senderAvatarId = stream.ReadLong();
			}

			m_senderName = stream.ReadString(900000);
			m_senderExpLevel = stream.ReadInt();
			m_senderLeagueType = stream.ReadInt();
			m_ageSeconds = stream.ReadInt();
			m_dismiss = stream.ReadBoolean();
			m_new = stream.ReadBoolean();
		}

		public virtual AvatarStreamEntryType GetAvatarStreamEntryType()
		{
			Debugger.Error("getAvatarStreamEntryType() must be overridden");
			return (AvatarStreamEntryType)(-1);
		}

		public virtual void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject senderObject = new LogicJSONObject();

			if (m_senderAvatarId != null)
			{
				senderObject.Put("avatar_id_hi", new LogicJSONNumber(m_senderAvatarId.GetHigherInt()));
				senderObject.Put("avatar_id_lo", new LogicJSONNumber(m_senderAvatarId.GetLowerInt()));
			}

			senderObject.Put("name", new LogicJSONString(m_senderName));
			senderObject.Put("exp_lvl", new LogicJSONNumber(m_senderExpLevel));
			senderObject.Put("league_type", new LogicJSONNumber(m_senderLeagueType));
			senderObject.Put("is_dismissed", new LogicJSONBoolean(m_dismiss));
			senderObject.Put("is_new", new LogicJSONBoolean(m_new));

			jsonObject.Put("sender", senderObject);
		}

		public virtual void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject senderObject = jsonObject.GetJSONObject("sender");

			if (senderObject != null)
			{
				LogicJSONNumber avatarIdHighNumber = senderObject.GetJSONNumber("avatar_id_hi");

				if (avatarIdHighNumber != null)
				{
					m_senderAvatarId = new LogicLong(avatarIdHighNumber.GetIntValue(), senderObject.GetJSONNumber("avatar_id_lo").GetIntValue());
				}

				m_senderName = senderObject.GetJSONString("name").GetStringValue();
				m_senderExpLevel = senderObject.GetJSONNumber("exp_lvl").GetIntValue();
				m_senderLeagueType = senderObject.GetJSONNumber("league_type").GetIntValue();

				LogicJSONBoolean isDismissedObject = senderObject.GetJSONBoolean("is_dismissed");

				if (isDismissedObject != null)
				{
					m_dismiss = isDismissedObject.IsTrue();
				}

				LogicJSONBoolean isNewObject = senderObject.GetJSONBoolean("is_new");

				if (isNewObject != null)
				{
					m_new = isNewObject.IsTrue();
				}
			}
		}

		public LogicLong GetId()
			=> m_id;

		public void SetId(LogicLong id)
		{
			m_id = id;
		}

		public LogicLong GetSenderAvatarId()
			=> m_senderAvatarId;

		public void SetSenderAvatarId(LogicLong allianceId)
		{
			m_senderAvatarId = allianceId;
		}

		public int GetAgeSeconds()
			=> m_ageSeconds;

		public void SetAgeSeconds(int value)
		{
			m_ageSeconds = value;
		}

		public string GetSenderName()
			=> m_senderName;

		public void SetSenderName(string name)
		{
			m_senderName = name;
		}

		public int GetSenderLevel()
			=> m_senderExpLevel;

		public void SetSenderLevel(int value)
		{
			m_senderExpLevel = value;
		}

		public int GetSenderLeagueType()
			=> m_senderLeagueType;

		public void SetSenderLeagueType(int value)
		{
			m_senderLeagueType = value;
		}

		public bool IsNew()
			=> m_new;

		public void SetNew(bool isNew)
		{
			m_new = isNew;
		}
	}

	public enum AvatarStreamEntryType
	{
		DEFENDER_BATTLE_REPORT = 2,
		JOIN_ALLIANCE_RESPONSE,
		ALLIANCE_INVITATION,
		ALLIANCE_KICKOUT,
		ALLIANCE_MAIL,
		ATTACKER_BATTLE_REPORT,
		DEVICE_LINKED,
		ADMIN_MESSAGE = 10
	}
}