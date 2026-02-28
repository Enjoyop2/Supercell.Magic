using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceCreateFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24332;

		private Reason m_reason;

		public AllianceCreateFailedMessage() : this(0)
		{
			// AllianceCreateFailedMessage.
		}

		public AllianceCreateFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceCreateFailedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_reason = (Reason)m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt((int)m_reason);
		}

		public override short GetMessageType()
			=> AllianceCreateFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public Reason GetReason()
			=> m_reason;

		public void SetReason(Reason value)
		{
			m_reason = value;
		}

		public enum Reason
		{
			GENERIC,
			INVALID_NAME = 1,
			INVALID_DESCRIPTION = 2,
			NAME_TOO_SHORT = 3,
			NAME_TOO_LONG = 4
		}
	}
}