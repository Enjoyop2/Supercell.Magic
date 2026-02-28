using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class VisitFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24122;

		private int m_reason;

		public VisitFailedMessage() : this(0)
		{
			// VisitFailedMessage.
		}

		public VisitFailedMessage(short messageVersion) : base(messageVersion)
		{
			// VisitFailedMessage.
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
			=> VisitFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

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