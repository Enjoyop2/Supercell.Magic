using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.League
{
	public class LeagueMemberEntry
	{
		private LogicLong m_accountId;
		private LogicLong m_avatarId;
		private LogicLong m_homeId;

		private string m_name;
		private string m_allianceName;

		private int m_score;
		private int m_order;
		private int m_previousOrder;
		private int m_attackWinCount;
		private int m_attackLoseCount;
		private int m_defenseWinCount;
		private int m_defenseLoseCount;
		private int m_allianceBadgeId;

		private LogicLong m_allianceId;

		public LeagueMemberEntry()
		{
			m_allianceBadgeId = -1;
		}

		public void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_accountId);
			encoder.WriteString(m_name);
			encoder.WriteInt(m_order);
			encoder.WriteInt(m_score);
			encoder.WriteInt(m_previousOrder);
			encoder.WriteInt(0);
			encoder.WriteInt(m_attackWinCount);
			encoder.WriteInt(m_attackLoseCount);
			encoder.WriteInt(m_defenseWinCount);
			encoder.WriteInt(m_defenseLoseCount);
			encoder.WriteLong(m_avatarId);
			encoder.WriteLong(m_homeId);

			if (m_allianceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_allianceId);
				encoder.WriteString(m_allianceName);
				encoder.WriteInt(m_allianceBadgeId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteLong(new LogicLong(0, 0));
		}

		public void Decode(ByteStream stream)
		{
			m_accountId = stream.ReadLong();
			m_name = stream.ReadString(900000);
			m_order = stream.ReadInt();
			m_score = stream.ReadInt();
			m_previousOrder = stream.ReadInt();

			stream.ReadInt();

			m_attackWinCount = stream.ReadInt();
			m_attackLoseCount = stream.ReadInt();
			m_defenseWinCount = stream.ReadInt();
			m_defenseLoseCount = stream.ReadInt();
			m_avatarId = stream.ReadLong();
			m_homeId = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				m_allianceId = stream.ReadLong();
				m_allianceName = stream.ReadString(900000);
				m_allianceBadgeId = stream.ReadInt();
			}

			stream.ReadLong();
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
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

		public int GetScore()
			=> m_score;

		public void SetScore(int value)
		{
			m_score = value;
		}

		public int GetOrder()
			=> m_order;

		public void SetOrder(int value)
		{
			m_order = value;
		}

		public int GetPreviousOrder()
			=> m_previousOrder;

		public void SetPreviousOrder(int value)
		{
			m_previousOrder = value;
		}

		public int GetAttackWinCount()
			=> m_attackWinCount;

		public void SetAttackWinCount(int value)
		{
			m_attackWinCount = value;
		}

		public int GetAttackLoseCount()
			=> m_attackLoseCount;

		public void SetAttackLoseCount(int value)
		{
			m_attackLoseCount = value;
		}

		public int GetDefenseWinCount()
			=> m_defenseWinCount;

		public void SetDefenseWinCount(int value)
		{
			m_defenseWinCount = value;
		}

		public int GetDefenseLoseCount()
			=> m_defenseLoseCount;

		public void SetDefenseLoseCount(int value)
		{
			m_defenseLoseCount = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}
	}
}