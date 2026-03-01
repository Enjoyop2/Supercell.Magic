using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2AttackEntry
	{
		public const int
			ATTACK_ENTRY_TYPE_BASE = 0,
			ATTACK_ENTRY_TYPE_BATTLE_PROGRESS = 1;

		private bool m_new;
		private bool m_removed;

		private LogicLong m_id;
		private LogicLong m_accountId;
		private LogicLong m_avatarId;
		private LogicLong m_homeId;
		private LogicLong m_allianceId;

		private string m_name;
		private string m_allianceName;

		private int m_allianceBadgeId;
		private int m_allianceExpLevel;
		private int m_remainingSeconds;

		public Village2AttackEntry()
		{
			m_new = true;
		}

		public virtual void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteBoolean(m_new);
			encoder.WriteBoolean(m_removed);

			if (!m_removed)
			{
				encoder.WriteLong(m_id);

				if (m_allianceId != null)
				{
					encoder.WriteBoolean(true);
					encoder.WriteLong(m_allianceId);
					encoder.WriteString(m_allianceName);
					encoder.WriteInt(m_allianceBadgeId);
					encoder.WriteInt(m_allianceExpLevel);
				}
				else
				{
					encoder.WriteBoolean(false);
				}

				encoder.WriteLong(m_accountId);
				encoder.WriteLong(m_avatarId);
				encoder.WriteString(m_name);
				encoder.WriteInt(-1);

				if (m_homeId != null)
				{
					encoder.WriteBoolean(true);
					encoder.WriteLong(m_homeId);
				}
				else
				{
					encoder.WriteBoolean(false);
				}

				encoder.WriteVInt(5);
				encoder.WriteVInt(10);
				encoder.WriteVInt(15);
				encoder.WriteVInt(20);
				encoder.WriteBoolean(false);
				encoder.WriteVInt(m_remainingSeconds);
			}
		}

		public virtual void Decode(ByteStream stream)
		{
			m_new = stream.ReadBoolean();
			m_removed = stream.ReadBoolean();

			if (!m_removed)
			{
				m_id = stream.ReadLong();

				if (stream.ReadBoolean())
				{
					m_allianceId = stream.ReadLong();
					m_allianceName = stream.ReadString(900000);
					m_allianceBadgeId = stream.ReadInt();
					m_allianceExpLevel = stream.ReadInt();
				}

				m_accountId = stream.ReadLong();
				m_avatarId = stream.ReadLong();
				m_name = stream.ReadString(900000);

				stream.ReadInt();

				if (stream.ReadBoolean())
				{
					m_homeId = stream.ReadLong();
				}

				stream.ReadVInt();
				stream.ReadVInt();
				stream.ReadVInt();
				stream.ReadVInt();
				stream.ReadBoolean();

				m_remainingSeconds = stream.ReadVInt();
			}
		}

		public virtual int GetAttackEntryType()
			=> Village2AttackEntry.ATTACK_ENTRY_TYPE_BASE;

		public bool IsNew()
			=> m_new;

		public void SetNew(bool value)
			=> m_new = value;

		public bool IsRemoved()
			=> m_removed;

		public void SetRemoved(bool value)
			=> m_removed = value;

		public LogicLong GetId()
			=> m_id;

		public void SetId(LogicLong value)
			=> m_id = value;

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
			=> m_accountId = value;

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
			=> m_avatarId = value;

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
			=> m_homeId = value;

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
			=> m_allianceId = value;

		public string GetName()
			=> m_name;

		public void SetName(string value)
			=> m_name = value;

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
			=> m_allianceName = value;

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
			=> m_allianceBadgeId = value;

		public int GetAllianceExpLevel()
			=> m_allianceExpLevel;

		public void SetAllianceExpLevel(int value)
			=> m_allianceExpLevel = value;

		public int GetRemainingSeconds()
			=> m_remainingSeconds;

		public void SetRemainingSeconds(int value)
			=> m_remainingSeconds = value;

		public virtual void Load(LogicJSONObject jsonObject)
		{
			LogicJSONNumber allianceIdHighNumber = jsonObject.GetJSONNumber("alliance_id_hi");

			if (allianceIdHighNumber != null)
			{
				m_allianceId = new LogicLong(allianceIdHighNumber.GetIntValue(), jsonObject.GetJSONNumber("alliance_id_lo").GetIntValue());
				m_allianceName = jsonObject.GetJSONString("alliance_name").GetStringValue();
				m_allianceBadgeId = jsonObject.GetJSONNumber("alliance_badge").GetIntValue();
				m_allianceExpLevel = jsonObject.GetJSONNumber("alliance_xp_lvl").GetIntValue();
			}

			m_accountId = new LogicLong(jsonObject.GetJSONNumber("acc_id_hi").GetIntValue(), jsonObject.GetJSONNumber("acc_id_lo").GetIntValue());
			m_avatarId = new LogicLong(jsonObject.GetJSONNumber("avatar_id_hi").GetIntValue(), jsonObject.GetJSONNumber("avatar_id_lo").GetIntValue());

			LogicJSONNumber homeIdHighNumber = jsonObject.GetJSONNumber("home_id_hi");

			if (homeIdHighNumber != null)
			{
				m_homeId = new LogicLong(homeIdHighNumber.GetIntValue(), jsonObject.GetJSONNumber("home_id_lo").GetIntValue());
			}

			m_name = jsonObject.GetJSONString("name").GetStringValue();
			m_remainingSeconds = jsonObject.GetJSONNumber("remainingSecs").GetIntValue();
		}

		public virtual void Save(LogicJSONObject jsonObject)
		{
			if (m_allianceId != null)
			{
				jsonObject.Put("alliance_id_hi", new LogicJSONNumber(m_allianceId.GetHigherInt()));
				jsonObject.Put("alliance_id_lo", new LogicJSONNumber(m_allianceId.GetLowerInt()));
				jsonObject.Put("alliance_name", new LogicJSONString(m_allianceName));
				jsonObject.Put("alliance_badge", new LogicJSONNumber(m_allianceBadgeId));
				jsonObject.Put("alliance_xp_lvl", new LogicJSONNumber(m_allianceExpLevel));
			}

			jsonObject.Put("acc_id_hi", new LogicJSONNumber(m_accountId.GetHigherInt()));
			jsonObject.Put("acc_id_lo", new LogicJSONNumber(m_accountId.GetLowerInt()));
			jsonObject.Put("avatar_id_hi", new LogicJSONNumber(m_avatarId.GetHigherInt()));
			jsonObject.Put("avatar_id_lo", new LogicJSONNumber(m_avatarId.GetLowerInt()));

			if (m_homeId != null)
			{
				jsonObject.Put("home_id_hi", new LogicJSONNumber(m_homeId.GetHigherInt()));
				jsonObject.Put("home_id_lo", new LogicJSONNumber(m_homeId.GetLowerInt()));
			}

			jsonObject.Put("name", new LogicJSONString(m_name));
			jsonObject.Put("remainingSecs", new LogicJSONNumber(m_remainingSeconds));
		}
	}
}