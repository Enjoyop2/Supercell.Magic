using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class LoginMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10101;

		private LogicLong m_accountId;
		private LogicData m_preferredLanguage;

		private bool m_androidClient;
		private bool m_advertisingEnabled;

		private int m_scramblerSeed;
		private int m_clientMajorVersion;
		private int m_clientBuildVersion;
		private int m_appStore;

		private string m_androidId;
		private string m_adid;
		private string m_device;
		private string m_imei;
		private string m_macAddress;
		private string m_openUDID;
		private string m_osVersion;
		private string m_advertisingId;
		private string m_appVersion;
		private string m_passToken;
		private string m_preferredDeviceLanguage;
		private string m_resourceSha;
		private string m_udid;
		private string m_kunlunSSO;
		private string m_kunlunUserId;

		public LoginMessage() : this(8)
		{
			// LoginMessage.
		}

		public LoginMessage(short messageVersion) : base(messageVersion)
		{
			m_imei = string.Empty;
			m_androidId = string.Empty;
			m_kunlunSSO = string.Empty;
			m_kunlunUserId = string.Empty;
			m_appVersion = string.Empty;
			m_preferredDeviceLanguage = string.Empty;
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_accountId);
			m_stream.WriteString(m_passToken);
			m_stream.WriteInt(m_clientMajorVersion);
			m_stream.WriteInt(0);
			m_stream.WriteInt(m_clientBuildVersion);
			m_stream.WriteString(m_resourceSha);
			m_stream.WriteString(m_udid);
			m_stream.WriteString(m_openUDID);
			m_stream.WriteString(m_macAddress);
			m_stream.WriteString(m_device);
			ByteStreamHelper.WriteDataReference(m_stream, m_preferredLanguage);
			m_stream.WriteString(m_preferredDeviceLanguage);
			m_stream.WriteString(m_adid);
			m_stream.WriteString(m_osVersion);
			m_stream.WriteBoolean(m_androidClient);
			m_stream.WriteStringReference(m_imei);
			m_stream.WriteStringReference(m_androidId);
			m_stream.WriteStringReference("");
			m_stream.WriteBoolean(false);
			m_stream.WriteString("");
			m_stream.WriteInt(m_scramblerSeed);
			m_stream.WriteVInt(m_appStore);
			m_stream.WriteStringReference(string.Empty);
			m_stream.WriteStringReference(string.Empty);
			m_stream.WriteStringReference(m_appVersion);
			m_stream.WriteStringReference(m_kunlunSSO);
			m_stream.WriteStringReference(m_kunlunUserId);
			m_stream.WriteVInt(0);
		}

		public override void Decode()
		{
			base.Decode();

			m_accountId = m_stream.ReadLong();
			m_passToken = m_stream.ReadString(900000);
			m_clientMajorVersion = m_stream.ReadInt();
			m_stream.ReadInt();
			m_clientBuildVersion = m_stream.ReadInt();
			m_resourceSha = m_stream.ReadString(900000);
			m_udid = m_stream.ReadString(900000);
			m_openUDID = m_stream.ReadString(900000);
			m_macAddress = m_stream.ReadString(900000);
			m_device = m_stream.ReadString(900000);

			if (!m_stream.IsAtEnd())
			{
				m_preferredLanguage = ByteStreamHelper.ReadDataReference(m_stream, LogicDataType.LOCALE);
				m_preferredDeviceLanguage = m_stream.ReadString(900000);

				if (m_preferredDeviceLanguage == null)
				{
					m_preferredDeviceLanguage = string.Empty;
				}

				if (!m_stream.IsAtEnd())
				{
					m_adid = m_stream.ReadString(900000);

					if (!m_stream.IsAtEnd())
					{
						m_osVersion = m_stream.ReadString(900000);

						if (!m_stream.IsAtEnd())
						{
							m_androidClient = m_stream.ReadBoolean();

							if (!m_stream.IsAtEnd())
							{
								m_imei = m_stream.ReadStringReference(900000);
								m_androidId = m_stream.ReadStringReference(900000);

								if (!m_stream.IsAtEnd())
								{
									m_stream.ReadString(900000);

									if (!m_stream.IsAtEnd())
									{
										m_advertisingEnabled = m_stream.ReadBoolean();
										m_advertisingId = m_stream.ReadString(900000);

										if (!m_stream.IsAtEnd())
										{
											m_scramblerSeed = m_stream.ReadInt();

											if (!m_stream.IsAtEnd())
											{
												m_appStore = m_stream.ReadVInt();

												m_stream.ReadStringReference(900000);
												m_stream.ReadStringReference(900000);

												if (!m_stream.IsAtEnd())
												{
													m_appVersion = m_stream.ReadStringReference(900000);

													if (!m_stream.IsAtEnd())
													{
														m_kunlunSSO = m_stream.ReadStringReference(900000);
														m_kunlunUserId = m_stream.ReadStringReference(900000);

														m_stream.ReadVInt();
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
			}
		}

		public override short GetMessageType()
			=> LoginMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_adid = null;
			m_passToken = null;
			m_device = null;
			m_imei = null;
			m_macAddress = null;
			m_osVersion = null;
			m_androidId = null;
			m_openUDID = null;
			m_preferredDeviceLanguage = null;
			m_resourceSha = null;
			m_kunlunSSO = null;
			m_kunlunUserId = null;
			m_udid = null;
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
		}

		public LogicData GetPreferredLanguage()
			=> m_preferredLanguage;

		public void SetPreferredLanguage(LogicData value)
		{
			m_preferredLanguage = value;
		}

		public bool IsAndroidClient()
			=> m_androidClient;

		public void SetAndroidClient(bool value)
		{
			m_androidClient = value;
		}

		public bool IsAdvertisingEnabled()
			=> m_advertisingEnabled;

		public void SetAdvertisingEnabled(bool value)
		{
			m_advertisingEnabled = value;
		}

		public int GetScramblerSeed()
			=> m_scramblerSeed;

		public void SetScramblerSeed(int value)
		{
			m_scramblerSeed = value;
		}

		public int GetClientMajorVersion()
			=> m_clientMajorVersion;

		public void SetClientMajorVersion(int value)
		{
			m_clientMajorVersion = value;
		}

		public int GetClientBuildVersion()
			=> m_clientBuildVersion;

		public void SetClientBuildVersion(int value)
		{
			m_clientBuildVersion = value;
		}

		public int GetAppStore()
			=> m_appStore;

		public void SetAppStore(int value)
		{
			m_appStore = value;
		}

		public string GetAndroidID()
			=> m_androidId;

		public void SetAndroidID(string value)
		{
			m_androidId = value;
		}

		public string GetADID()
			=> m_adid;

		public void SetADID(string value)
		{
			m_adid = value;
		}

		public string GetDevice()
			=> m_device;

		public void SetDevice(string value)
		{
			m_device = value;
		}

		public string GetIMEI()
			=> m_imei;

		public void SetIMEI(string value)
		{
			m_imei = value;
		}

		public string GetMacAddress()
			=> m_macAddress;

		public void SetMacAddress(string value)
		{
			m_macAddress = value;
		}

		public string GetOpenUDID()
			=> m_openUDID;

		public void SetOpenUDID(string value)
		{
			m_openUDID = value;
		}

		public string GetOSVersion()
			=> m_osVersion;

		public void SetOSVersion(string value)
		{
			m_osVersion = value;
		}

		public string GetAdvertisingId()
			=> m_advertisingId;

		public void SetAdvertisingId(string value)
		{
			m_advertisingId = value;
		}

		public string GetAppVersion()
			=> m_appVersion;

		public void SetAppVersion(string value)
		{
			m_appVersion = value;
		}

		public string GetPassToken()
			=> m_passToken;

		public void SetPassToken(string value)
		{
			m_passToken = value;
		}

		public string GetPreferredDeviceLanguage()
			=> m_preferredDeviceLanguage;

		public void SetPreferredDeviceLanguage(string value)
		{
			m_preferredDeviceLanguage = value;
		}

		public string GetResourceSha()
			=> m_resourceSha;

		public void SetResourceSha(string value)
		{
			m_resourceSha = value;
		}

		public string GetUDID()
			=> m_udid;

		public void SetUDID(string value)
		{
			m_udid = value;
		}

		public string GetKunlunSSO()
			=> m_kunlunSSO;

		public void SetKunlunSSO(string value)
		{
			m_kunlunSSO = value;
		}

		public string GetKunlunUID()
			=> m_kunlunUserId;

		public void SetKunlunUID(string value)
		{
			m_kunlunUserId = value;
		}
	}
}