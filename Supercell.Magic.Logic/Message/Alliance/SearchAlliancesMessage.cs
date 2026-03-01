using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class SearchAlliancesMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14324;

		private string m_searchString;
		private bool m_openOnly;

		private int m_warFrequency;
		private int m_minMemberCount;
		private int m_maxMemberCount;
		private int m_requiredScore;
		private int m_requiredDuelScore;
		private int m_minExpLevel;

		private LogicData m_originData;

		public SearchAlliancesMessage() : this(0)
		{
			// SearchAlliancesMessage.
		}

		public SearchAlliancesMessage(short messageVersion) : base(messageVersion)
		{
			// SearchAlliancesMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_searchString = m_stream.ReadString(900000);
			m_warFrequency = m_stream.ReadInt();
			m_originData = ByteStreamHelper.ReadDataReference(m_stream, DataType.REGION);
			m_minMemberCount = m_stream.ReadInt();
			m_maxMemberCount = m_stream.ReadInt();
			m_requiredScore = m_stream.ReadInt();
			m_requiredDuelScore = m_stream.ReadInt();
			m_openOnly = m_stream.ReadBoolean();

			m_stream.ReadInt();
			m_stream.ReadInt();

			m_minExpLevel = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_searchString);
			m_stream.WriteInt(m_warFrequency);
			ByteStreamHelper.WriteDataReference(m_stream, m_originData);
			m_stream.WriteInt(m_minMemberCount);
			m_stream.WriteInt(m_maxMemberCount);
			m_stream.WriteInt(m_requiredScore);
			m_stream.WriteInt(m_requiredDuelScore);
			m_stream.WriteBoolean(m_openOnly);
			m_stream.WriteInt(0);
			m_stream.WriteInt(0);
			m_stream.WriteInt(m_minExpLevel);
		}

		public override short GetMessageType()
			=> SearchAlliancesMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public string GetSearchString()
			=> m_searchString;

		public void SetSearchString(string value)
		{
			m_searchString = value;
		}

		public int GetWarFrequency()
			=> m_warFrequency;

		public void SetWarFrequency(int value)
		{
			m_warFrequency = value;
		}

		public LogicData GetOrigin()
			=> m_originData;

		public void SetOrigin(LogicData data)
		{
			m_originData = data;
		}

		public int GetMinimumMembers()
			=> m_minMemberCount;

		public void SetMinimumMembers(int value)
		{
			m_maxMemberCount = value;
		}

		public int GetMaximumMembers()
			=> m_maxMemberCount;

		public void SetMaximumMembers(int value)
		{
			m_minMemberCount = value;
		}

		public int GetRequiredScore()
			=> m_requiredScore;

		public void SetRequiredScore(int value)
		{
			m_requiredScore = value;
		}

		public int GetRequiredDuelScore()
			=> m_requiredDuelScore;

		public void SetRequireDuelScore(int value)
		{
			m_requiredDuelScore = value;
		}

		public int GetMinimumExpLevel()
			=> m_minExpLevel;

		public void SetMinimumExpLevel(int value)
		{
			m_minExpLevel = value;
		}

		public bool IsJoinableOnly()
			=> m_openOnly;

		public void SetOpenOnly(bool enabled)
		{
			m_openOnly = enabled;
		}
	}
}