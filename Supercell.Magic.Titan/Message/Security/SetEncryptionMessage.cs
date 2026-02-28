namespace Supercell.Magic.Titan.Message.Security
{
	public class SetEncryptionMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20000;

		private byte[] m_nonce;

		public SetEncryptionMessage() : this(0)
		{
			// SetEncryptionMessage.
		}

		public SetEncryptionMessage(short messageVersion) : base(messageVersion)
		{
			// SetEncryptionMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_nonce = m_stream.ReadBytes(m_stream.ReadBytesLength(), 900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteBytes(m_nonce, m_nonce.Length);
		}

		public override short GetMessageType()
			=> SetEncryptionMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
			m_nonce = null;
		}

		public byte[] RemoveNonce()
		{
			byte[] tmp = m_nonce;
			m_nonce = null;
			return tmp;
		}

		public void SetNonce(byte[] value)
		{
			m_nonce = value;
		}
	}
}