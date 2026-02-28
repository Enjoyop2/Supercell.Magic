namespace Supercell.Magic.Titan.Message.Security
{
	public class ClientCryptoErrorMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10099;

		public ClientCryptoErrorMessage() : this(0)
		{
			// ClientCryptoErrorMessage.
		}

		public ClientCryptoErrorMessage(short messageVersion) : base(messageVersion)
		{
			// ClientCryptoErrorMessage.
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(0);
		}

		public override void Decode()
		{
			base.Decode();
			m_stream.ReadInt();
		}

		public override short GetMessageType()
			=> ClientCryptoErrorMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}