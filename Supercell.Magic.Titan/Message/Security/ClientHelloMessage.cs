namespace Supercell.Magic.Titan.Message.Security
{
	public class ClientHelloMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10100;

		private int m_protocol;
		private int m_keyVersion;
		private int m_majorVersion;
		private int m_minorVersion;
		private int m_buildVersion;
		private int m_deviceType;
		private int m_appStore;

		private string m_contentHash;

		public ClientHelloMessage() : this(0)
		{
			// ClientHelloMessage.
		}

		public ClientHelloMessage(short messageVersion) : base(messageVersion)
		{
			m_contentHash = string.Empty;
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_protocol);
			m_stream.WriteInt(m_keyVersion);
			m_stream.WriteInt(m_majorVersion);
			m_stream.WriteInt(m_minorVersion);
			m_stream.WriteInt(m_buildVersion);
			m_stream.WriteStringReference(m_contentHash);
			m_stream.WriteInt(m_deviceType);
			m_stream.WriteInt(m_appStore);
		}

		public override void Decode()
		{
			base.Decode();

			m_protocol = m_stream.ReadInt();
			m_keyVersion = m_stream.ReadInt();
			m_majorVersion = m_stream.ReadInt();
			m_minorVersion = m_stream.ReadInt();
			m_buildVersion = m_stream.ReadInt();
			m_contentHash = m_stream.ReadStringReference(900000);
			m_deviceType = m_stream.ReadInt();
			m_appStore = m_stream.ReadInt();
		}

		public override short GetMessageType()
			=> ClientHelloMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
			m_contentHash = null;
		}

		public int GetProtocol()
			=> m_protocol;

		public void SetProtocol(int value)
		{
			m_protocol = value;
		}

		public int GetKeyVersion()
			=> m_keyVersion;

		public void SetKeyVersion(int value)
		{
			m_keyVersion = value;
		}

		public int GetMajorVersion()
			=> m_majorVersion;

		public void SetMajorVersion(int value)
		{
			m_majorVersion = value;
		}

		public int GetMinorVersion()
			=> m_minorVersion;

		public void SetMinorVersion(int value)
		{
			m_minorVersion = value;
		}

		public int GetBuildVersion()
			=> m_buildVersion;

		public void SetBuildVersion(int value)
		{
			m_buildVersion = value;
		}

		public string GetContentHash()
			=> m_contentHash;

		public void SetContentHash(string value)
		{
			m_contentHash = value;
		}

		public int GetDeviceType()
			=> m_deviceType;

		public void SetDeviceType(int value)
		{
			m_deviceType = value;
		}

		public int GetAppStore()
			=> m_appStore;

		public void SetAppStore(int value)
		{
			m_appStore = value;
		}
	}
}