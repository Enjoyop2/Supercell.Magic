using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class ChangeAllianceSettingsMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14316;

		private LogicData m_originData;

		private string m_allianceDescription;

		private int m_allianceType;
		private int m_allianceBadgeId;
		private int m_requiredScore;
		private int m_requiredDuelScore;
		private int m_warFrequency;

		private bool m_publicWarLog;
		private bool m_amicalWarEnabled;

		public ChangeAllianceSettingsMessage() : this(0)
		{
			// ChangeAllianceSettingsMessage.
		}

		public ChangeAllianceSettingsMessage(short messageVersion) : base(messageVersion)
		{
			// ChangeAllianceSettingsMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceDescription = m_stream.ReadString(900000);
			m_stream.ReadString(900000);
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

			m_stream.WriteString(m_allianceDescription);
			m_stream.WriteString(null);
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
			=> ChangeAllianceSettingsMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceDescription = null;
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

		public bool IsAmicalWarEnabled()
			=> m_amicalWarEnabled;

		public void SetAmicalWarEnabled(bool enabled)
		{
			m_amicalWarEnabled = enabled;
		}

		public bool IsPublicWarLog()
			=> m_publicWarLog;

		public void SetPublicWarLog(bool enabled)
		{
			m_publicWarLog = enabled;
		}
	}
}