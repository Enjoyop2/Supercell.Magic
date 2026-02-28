using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class CreateAllianceMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14301;

		private LogicData m_originData;

		private string m_allianceName;
		private string m_allianceDescription;

		private int m_allianceType;
		private int m_allianceBadgeId;
		private int m_requiredScore;
		private int m_requiredDuelScore;
		private int m_warFrequency;

		private bool m_publicWarLog;
		private bool m_amicalWarEnabled;

		public CreateAllianceMessage() : this(0)
		{
			// CreateAllianceMessage.
		}

		public CreateAllianceMessage(short messageVersion) : base(messageVersion)
		{
			// CreateAllianceMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceName = m_stream.ReadString(900000);
			m_allianceDescription = m_stream.ReadString(900000);
			m_allianceBadgeId = m_stream.ReadInt();
			m_allianceType = m_stream.ReadInt();
			m_requiredScore = m_stream.ReadInt();
			m_requiredDuelScore = m_stream.ReadInt();
			m_warFrequency = m_stream.ReadInt();
			m_originData = ByteStreamHelper.ReadDataReference(m_stream);
			m_publicWarLog = m_stream.ReadBoolean();
			m_amicalWarEnabled = m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_allianceName);
			m_stream.WriteString(m_allianceDescription);
			m_stream.WriteInt(m_allianceBadgeId);
			m_stream.WriteInt(m_allianceType);
			m_stream.WriteInt(m_requiredScore);
			m_stream.WriteInt(m_requiredDuelScore);
			m_stream.WriteInt(m_warFrequency);
			ByteStreamHelper.WriteDataReference(m_stream, m_originData);
			m_stream.WriteBoolean(m_publicWarLog);
			m_stream.WriteBoolean(m_amicalWarEnabled);
		}

		public override short GetMessageType()
			=> CreateAllianceMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public string GetAllianceDescription()
			=> m_allianceDescription;

		public void SetAllianceDescription(string value)
		{
			m_allianceDescription = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public int GetAllianceType()
			=> m_allianceType;

		public void SetAllianceType(int value)
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

		public void SetOriginData(LogicData data)
		{
			m_originData = data;
		}

		public int GetWarFrequency()
			=> m_warFrequency;

		public bool GetArrangedWarEnabled()
			=> m_amicalWarEnabled;

		public void SetAmicalWarEnabled(bool enabled)
		{
			m_amicalWarEnabled = enabled;
		}

		public bool GetPublicWarLog()
			=> m_publicWarLog;

		public void SetPublicWarLog(bool enabled)
		{
			m_publicWarLog = enabled;
		}
	}
}