using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Account
{
	public class LoginOkMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20104;

		private LogicLong m_accountId;
		private LogicLong m_homeId;

		private string m_passToken;
		private string m_facebookId;
		private string m_gamecenterId;
		private string m_serverEnvironment;
		private string m_facebookAppId;
		private string m_serverTime;
		private string m_accountCreatedDate;
		private string m_googleServiceId;
		private string m_region;

		private int m_serverMajorVersion;
		private int m_serverBuildVersion;
		private int m_contentVersion;
		private int m_sessionCount;
		private int m_playTimeSeconds;
		private int m_daysSinceStartedPlaying;
		private int m_startupCooldownSeconds;

		private LogicArrayList<string> m_contentUrlList;
		private LogicArrayList<string> m_chronosContentUrlList;

		public LoginOkMessage() : this(1)
		{
			// LoginOkMessage.
		}

		public LoginOkMessage(short messageVersion) : base(messageVersion)
		{
			// LoginOkMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_accountId = m_stream.ReadLong();
			m_homeId = m_stream.ReadLong();
			m_passToken = m_stream.ReadString(900000);
			m_facebookId = m_stream.ReadString(900000);
			m_gamecenterId = m_stream.ReadString(900000);
			m_serverMajorVersion = m_stream.ReadInt();
			m_serverBuildVersion = m_stream.ReadInt();
			m_contentVersion = m_stream.ReadInt();
			m_serverEnvironment = m_stream.ReadString(900000);
			m_sessionCount = m_stream.ReadInt();
			m_playTimeSeconds = m_stream.ReadInt();
			m_daysSinceStartedPlaying = m_stream.ReadInt();
			m_facebookAppId = m_stream.ReadString(900000);
			m_serverTime = m_stream.ReadString(900000);
			m_accountCreatedDate = m_stream.ReadString(900000);
			m_startupCooldownSeconds = m_stream.ReadInt();
			m_googleServiceId = m_stream.ReadString(9000);
			m_region = m_stream.ReadString(9000);
			m_stream.ReadString(9000);
			m_stream.ReadInt();
			m_stream.ReadString(9000);
			m_stream.ReadString(9000);
			m_stream.ReadString(9000);

			int contentUrlListSize = m_stream.ReadInt();

			if (contentUrlListSize != -1)
			{
				m_contentUrlList = new LogicArrayList<string>(contentUrlListSize);

				if (contentUrlListSize != 0)
				{
					for (int i = 0; i < contentUrlListSize; i++)
					{
						m_contentUrlList.Add(m_stream.ReadString(900000));
					}
				}
			}

			int chronosContentUrlListSize = m_stream.ReadInt();

			if (chronosContentUrlListSize != -1)
			{
				m_chronosContentUrlList = new LogicArrayList<string>(chronosContentUrlListSize);

				if (chronosContentUrlListSize != 0)
				{
					for (int i = 0; i < chronosContentUrlListSize; i++)
					{
						m_chronosContentUrlList.Add(m_stream.ReadString(900000));
					}
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_accountId);
			m_stream.WriteLong(m_homeId);
			m_stream.WriteString(m_passToken);
			m_stream.WriteString(m_facebookId);
			m_stream.WriteString(m_gamecenterId);
			m_stream.WriteInt(m_serverMajorVersion);
			m_stream.WriteInt(m_serverBuildVersion);
			m_stream.WriteInt(m_contentVersion);
			m_stream.WriteString(m_serverEnvironment);
			m_stream.WriteInt(m_sessionCount);
			m_stream.WriteInt(m_playTimeSeconds);
			m_stream.WriteInt(m_daysSinceStartedPlaying);
			m_stream.WriteString(m_facebookAppId);
			m_stream.WriteString(m_serverTime);
			m_stream.WriteString(m_accountCreatedDate);
			m_stream.WriteInt(m_startupCooldownSeconds);
			m_stream.WriteString(m_googleServiceId);
			m_stream.WriteString(m_region);
			m_stream.WriteString(null);
			m_stream.WriteInt(1);
			m_stream.WriteString(null);
			m_stream.WriteString(null);
			m_stream.WriteString(null);

			if (m_contentUrlList != null)
			{
				m_stream.WriteInt(m_contentUrlList.Size());

				for (int i = 0; i < m_contentUrlList.Size(); i++)
				{
					m_stream.WriteString(m_contentUrlList[i]);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}

			if (m_chronosContentUrlList != null)
			{
				m_stream.WriteInt(m_chronosContentUrlList.Size());

				for (int i = 0; i < m_chronosContentUrlList.Size(); i++)
				{
					m_stream.WriteString(m_chronosContentUrlList[i]);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> LoginOkMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_chronosContentUrlList = null;
			m_contentUrlList = null;

			m_passToken = null;
			m_facebookId = null;
			m_gamecenterId = null;
			m_serverEnvironment = null;
			m_facebookAppId = null;
			m_serverTime = null;
			m_accountCreatedDate = null;
			m_googleServiceId = null;
			m_region = null;
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public string GetPassToken()
			=> m_passToken;

		public void SetPassToken(string value)
		{
			m_passToken = value;
		}

		public string GetFacebookId()
			=> m_facebookId;

		public void SetFacebookId(string value)
		{
			m_facebookId = value;
		}

		public string GetGamecenterId()
			=> m_gamecenterId;

		public void SetGamecenterId(string value)
		{
			m_gamecenterId = value;
		}

		public string GetServerEnvironment()
			=> m_serverEnvironment;

		public void SetServerEnvironment(string value)
		{
			m_serverEnvironment = value;
		}

		public string GetFacebookAppId()
			=> m_facebookAppId;

		public void SetFacebookAppId(string value)
		{
			m_facebookAppId = value;
		}

		public string GetServerTime()
			=> m_serverTime;

		public void SetServerTime(string value)
		{
			m_serverTime = value;
		}

		public string GetAccountCreatedDate()
			=> m_accountCreatedDate;

		public void SetAccountCreatedDate(string value)
		{
			m_accountCreatedDate = value;
		}

		public string GetGoogleServiceId()
			=> m_googleServiceId;

		public void SetGoogleServiceId(string value)
		{
			m_googleServiceId = value;
		}

		public string GetRegion()
			=> m_region;

		public void SetRegion(string value)
		{
			m_region = value;
		}

		public int GetServerMajorVersion()
			=> m_serverMajorVersion;

		public void SetServerMajorVersion(int value)
		{
			m_serverMajorVersion = value;
		}

		public int GetServerBuildVersion()
			=> m_serverBuildVersion;

		public void SetServerBuildVersion(int value)
		{
			m_serverBuildVersion = value;
		}

		public int GetContentVersion()
			=> m_contentVersion;

		public void SetContentVersion(int value)
		{
			m_contentVersion = value;
		}

		public int GetSessionCount()
			=> m_sessionCount;

		public void SetSessionCount(int value)
		{
			m_sessionCount = value;
		}

		public int GetPlayTimeSeconds()
			=> m_playTimeSeconds;

		public void SetPlayTimeSeconds(int value)
		{
			m_playTimeSeconds = value;
		}

		public int GetDaysSinceStartedPlaying()
			=> m_daysSinceStartedPlaying;

		public void SetDaysSinceStartedPlaying(int value)
		{
			m_daysSinceStartedPlaying = value;
		}

		public int GetStartupCooldownSeconds()
			=> m_startupCooldownSeconds;

		public void SetStartupCooldownSeconds(int value)
		{
			m_startupCooldownSeconds = value;
		}

		public LogicArrayList<string> GetContentUrlList()
			=> m_contentUrlList;

		public void SetContentUrlList(LogicArrayList<string> value)
		{
			m_contentUrlList = value;
		}

		public LogicArrayList<string> GetChronosContentUrlList()
			=> m_chronosContentUrlList;

		public void SetChronosContentUrlList(LogicArrayList<string> value)
		{
			m_chronosContentUrlList = value;
		}
	}
}