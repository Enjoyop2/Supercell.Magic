namespace Supercell.Magic.Titan.Message.Security
{
	public class CryptoErrorMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 29997;

		private int m_errorCode;

		public CryptoErrorMessage() : this(0)
		{
			// CryptoErrorMessage.
		}

		public CryptoErrorMessage(short messageVersion) : base(messageVersion)
		{
			// CryptoErrorMessage.
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteVInt(m_errorCode);
		}

		public override void Decode()
		{
			base.Decode();
			m_errorCode = m_stream.ReadVInt();
		}

		public override short GetMessageType()
			=> CryptoErrorMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}