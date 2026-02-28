using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Chat
{
	public class GlobalChatLineMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24715;

		private string m_message;
		private string m_avatarName;
		private string m_allianceName;

		private int m_avatarExpLevel;
		private int m_avatarLeagueType;
		private int m_allianceBadgeId;

		private LogicLong m_avatarId;
		private LogicLong m_homeId;
		private LogicLong m_allianceId;

		public GlobalChatLineMessage() : this(0)
		{
			// GlobalChatLineMessage.
		}

		public GlobalChatLineMessage(short messageVersion) : base(messageVersion)
		{
			// GlobalChatLineMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_message = m_stream.ReadString(900000);
			m_avatarName = m_stream.ReadString(900000);
			m_avatarExpLevel = m_stream.ReadInt();
			m_avatarLeagueType = m_stream.ReadInt();
			m_avatarId = m_stream.ReadLong();
			m_homeId = m_stream.ReadLong();

			if (m_stream.ReadBoolean())
			{
				m_allianceId = m_stream.ReadLong();
				m_allianceName = m_stream.ReadString(900000);
				m_allianceBadgeId = m_stream.ReadInt();
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_message);
			m_stream.WriteString(m_avatarName);
			m_stream.WriteInt(m_avatarExpLevel);
			m_stream.WriteInt(m_avatarLeagueType);
			m_stream.WriteLong(m_avatarId);
			m_stream.WriteLong(m_homeId);

			if (m_allianceId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_allianceId);
				m_stream.WriteString(m_allianceName);
				m_stream.WriteInt(m_allianceBadgeId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> GlobalChatLineMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 6;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
			m_avatarName = null;
			m_allianceName = null;
			m_avatarId = null;
			m_homeId = null;
			m_allianceId = null;
		}

		public string RemoveMessage(string message)
		{
			string tmp = m_message;
			m_message = null;
			return tmp;
		}

		public void SetMessage(string message)
		{
			m_message = message;
		}

		public string GetAvatarName()
			=> m_avatarName;

		public void SetAvatarName(string name)
		{
			m_avatarName = name;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string name)
		{
			m_allianceName = name;
		}

		public int GetAvatarExpLevel()
			=> m_avatarExpLevel;

		public void SetAvatarExpLevel(int lvl)
		{
			m_avatarExpLevel = lvl;
		}

		public int GetAvatarLeagueType()
			=> m_avatarLeagueType;

		public void SetAvatarLeagueType(int leagueType)
		{
			m_avatarLeagueType = leagueType;
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong id)
		{
			m_avatarId = id;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong id)
		{
			m_homeId = id;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong id)
		{
			m_allianceId = id;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int id)
		{
			m_allianceBadgeId = id;
		}
	}
}