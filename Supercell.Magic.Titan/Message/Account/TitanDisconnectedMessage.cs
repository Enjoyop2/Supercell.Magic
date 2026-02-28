namespace Supercell.Magic.Titan.Message.Account
{
	public class TitanDisconnectedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 25892;

		private int m_reason;

		public TitanDisconnectedMessage() : this(0)
		{
			// TitanDisconnectedMessage.
		}

		public TitanDisconnectedMessage(short messageVersion) : base(messageVersion)
		{
			// TitanDisconnectedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_reason = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_reason);
		}

		public override short GetMessageType()
			=> TitanDisconnectedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetReason()
			=> m_reason;

		public void SetReason(int value)
		{
			m_reason = value;
		}
	}
}