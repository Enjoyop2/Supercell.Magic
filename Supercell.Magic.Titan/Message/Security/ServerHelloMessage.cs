namespace Supercell.Magic.Titan.Message.Security
{
	public class ServerHelloMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20100;

		private byte[] m_serverNonce;

		public ServerHelloMessage() : this(0)
		{
			// ServerHelloMessage.
		}

		public ServerHelloMessage(short messageVersion) : base(messageVersion)
		{
			// ServerHelloMessage.
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteBytes(m_serverNonce, m_serverNonce.Length);
		}

		public override void Decode()
		{
			base.Decode();
			m_serverNonce = m_stream.ReadBytes(m_stream.ReadBytesLength(), 24);
		}

		public byte[] RemoveServerNonce()
		{
			byte[] tmp = m_serverNonce;
			m_serverNonce = null;
			return tmp;
		}

		public void SetServerNonce(byte[] value)
		{
			m_serverNonce = value;
		}

		public override short GetMessageType()
			=> ServerHelloMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			m_serverNonce = null;
		}
	}
}