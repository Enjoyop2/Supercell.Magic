using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Account
{
	public class LoginFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20103;

		private LogicArrayList<string> m_contentUrlList;

		private bool m_bannedShowHelpshiftContact;

		private ErrorCode m_errorCode;
		private int m_endMaintenanceTime;

		private string m_resourceFingerprintContent;
		private string m_redirectDomain;
		private string m_reason;
		private string m_updateUrl;
		private string m_contentUrl;

		private byte[] m_compressedFingerprintData;

		public LoginFailedMessage() : this(9)
		{
			// LoginFailedMessage.
		}

		public LoginFailedMessage(short messageVersion) : base(messageVersion)
		{
			m_compressedFingerprintData = new byte[0];
		}

		public override void Decode()
		{
			base.Decode();

			m_errorCode = (ErrorCode)m_stream.ReadInt();
			m_resourceFingerprintContent = m_stream.ReadString(900000);
			m_redirectDomain = m_stream.ReadString(900000);
			m_contentUrl = m_stream.ReadString(900000);

			if (m_version >= 1)
			{
				m_updateUrl = m_stream.ReadString(900000);

				if (m_version >= 2)
				{
					m_reason = m_stream.ReadString(900000);

					if (m_version >= 3)
					{
						m_endMaintenanceTime = m_stream.ReadInt();

						if (m_version >= 4)
						{
							m_bannedShowHelpshiftContact = m_stream.ReadBoolean();

							if (m_version >= 5)
							{
								m_compressedFingerprintData = m_stream.ReadBytes(m_stream.ReadBytesLength(), 900000);

								int contentUrlListSize = m_stream.ReadInt();

								if (contentUrlListSize != -1)
								{
									m_contentUrlList = new LogicArrayList<string>(contentUrlListSize);

									for (int i = 0; i < contentUrlListSize; i++)
									{
										m_contentUrlList.Add(m_stream.ReadString(900000));
									}
								}

								if (m_version >= 6)
								{
									m_stream.ReadInt();

									if (m_version >= 7)
									{
										m_stream.ReadInt();

										if (m_version >= 8)
										{
											m_stream.ReadString(900000);

											if (m_version >= 9)
											{
												m_stream.ReadInt();
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt((int)m_errorCode);
			m_stream.WriteString(m_resourceFingerprintContent);
			m_stream.WriteString(m_redirectDomain);
			m_stream.WriteString(m_contentUrl);
			m_stream.WriteString(m_updateUrl);
			m_stream.WriteString(m_reason);
			m_stream.WriteInt(m_endMaintenanceTime);
			m_stream.WriteBoolean(m_bannedShowHelpshiftContact);
			m_stream.WriteBytes(m_compressedFingerprintData, m_compressedFingerprintData.Length);

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

			m_stream.WriteInt(0);
			m_stream.WriteInt(0);
			m_stream.WriteString(null);
			m_stream.WriteInt(-1);
		}

		public override short GetMessageType()
			=> LoginFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_resourceFingerprintContent = null;
			m_redirectDomain = null;
			m_reason = null;
			m_updateUrl = null;
			m_contentUrl = null;
			m_compressedFingerprintData = null;
			m_contentUrlList = null;
		}

		public LogicArrayList<string> GetContentUrlList()
			=> m_contentUrlList;

		public void SetContentUrlList(LogicArrayList<string> value)
		{
			m_contentUrlList = value;
		}

		public bool IsBannedShowHelpshiftContact()
			=> m_bannedShowHelpshiftContact;

		public void SetBannedShowHelpshiftContact(bool value)
		{
			m_bannedShowHelpshiftContact = value;
		}

		public ErrorCode GetErrorCode()
			=> m_errorCode;

		public void SetErrorCode(ErrorCode value)
		{
			m_errorCode = value;
		}

		public int GetEndMaintenanceTime()
			=> m_endMaintenanceTime;

		public void SetEndMaintenanceTime(int value)
		{
			m_endMaintenanceTime = value;
		}

		public string GetResourceFingerprintContent()
			=> m_resourceFingerprintContent;

		public void SetResourceFingerprintContent(string value)
		{
			m_resourceFingerprintContent = value;
		}

		public string GetRedirectDomain()
			=> m_redirectDomain;

		public void SetRedirectDomain(string value)
		{
			m_redirectDomain = value;
		}

		public string GetReason()
			=> m_reason;

		public void SetReason(string value)
		{
			m_reason = value;
		}

		public string GetUpdateUrl()
			=> m_updateUrl;

		public void SetUpdateUrl(string value)
		{
			m_updateUrl = value;
		}

		public string GetContentUrl()
			=> m_contentUrl;

		public void SetContentUrl(string value)
		{
			m_contentUrl = value;
		}

		public byte[] GetCompressedFingerprint()
			=> m_compressedFingerprintData;

		public void SetCompressedFingerprint(byte[] value)
		{
			m_compressedFingerprintData = value;
		}

		public enum ErrorCode
		{
			ACCOUNT_NOT_EXISTS = 1,
			DATA_VERSION = 7,
			CLIENT_VERSION = 8,
			REDIRECTION = 9,
			SERVER_MAINTENANCE = 10,
			BANNED = 11,
			PERSONAL_BREAK = 12,
			ACCOUNT_LOCKED = 13,
			WRONG_STORE = 15,
			VERSION_NOT_UP_TO_DATE_STORE_NOT_READY = 16,
			CHINESE_APP_STORE_CONFLICT_MESSAGE = 18,
			PERSONAL_BREAK_EXTENDED = 19,
			PERSONAL_BREAK_EXTENDED_FINAL = 20,
			PERSONAL_BREAK_FINAL = 21
		}
	}
}