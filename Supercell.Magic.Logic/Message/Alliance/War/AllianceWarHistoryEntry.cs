using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarHistoryEntry
	{
		private LogicLong m_allianceId;
		private LogicLong m_allianceEnemyId;

		private string m_allianceName;
		private string m_allianceEnemyName;

		private int m_allianceBadgeId;
		private int m_allianceLevel;
		private int m_allianceEnemyBadgeId;
		private int m_allianceEnemyLevel;
		private int m_expEarned;

		private bool m_removed;

		private int m_ageSeconds;
		private int m_villageType;

		public void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceName = stream.ReadString(900000);
			m_allianceBadgeId = stream.ReadInt();
			m_allianceLevel = stream.ReadInt();

			m_allianceEnemyId = stream.ReadLong();
			m_allianceEnemyName = stream.ReadString(900000);
			m_allianceEnemyBadgeId = stream.ReadInt();
			m_allianceEnemyLevel = stream.ReadInt();

			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadLong();
			stream.ReadInt();
			m_expEarned = stream.ReadInt();
			m_removed = stream.ReadBoolean();
			m_ageSeconds = stream.ReadInt();
			stream.ReadLong();
			m_villageType = stream.ReadInt();
		}

		public void Encode(ByteStream encoder)
		{
			encoder.WriteLong(m_allianceId);
			encoder.WriteString(m_allianceName);
			encoder.WriteInt(m_allianceBadgeId);
			encoder.WriteInt(m_allianceLevel);

			encoder.WriteLong(m_allianceEnemyId);
			encoder.WriteString(m_allianceEnemyName);
			encoder.WriteInt(m_allianceEnemyBadgeId);
			encoder.WriteInt(m_allianceEnemyLevel);

			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteLong(0);
			encoder.WriteInt(0);
			encoder.WriteInt(m_expEarned);
			encoder.WriteBoolean(m_removed);
			encoder.WriteInt(m_ageSeconds);
			encoder.WriteLong(0);
			encoder.WriteInt(m_villageType);
		}
	}
}