using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarHeader
	{
		private LogicLong m_allianceId;
		private string m_allianceName;
		private int m_allianceBadgeId;
		private int m_allianceLevel;

		public void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceName = stream.ReadString(900000);
			m_allianceBadgeId = stream.ReadInt();
			m_allianceLevel = stream.ReadInt();
		}

		public void Encode(ByteStream encoder)
		{
			encoder.WriteLong(m_allianceId);
			encoder.WriteString(m_allianceName);
			encoder.WriteInt(m_allianceBadgeId);
			encoder.WriteInt(m_allianceLevel);
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

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public int GetAllianceLevel()
			=> m_allianceLevel;

		public void SetAllianceLevel(int value)
		{
			m_allianceLevel = value;
		}
	}
}