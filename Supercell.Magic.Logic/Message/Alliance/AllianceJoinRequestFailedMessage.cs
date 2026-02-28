using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceJoinRequestFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24320;

		private Reason m_reason;

		public AllianceJoinRequestFailedMessage() : this(0)
		{
			// AllianceJoinRequestFailedMessage.
		}

		public AllianceJoinRequestFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceJoinRequestFailedMessage.
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
			=> AllianceJoinRequestFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public Reason GetReason()
			=> m_reason;

		public void SetReason(Reason reason)
		{
			m_reason = reason;
		}

		public enum Reason
		{
			GENERIC,
			CLOSED,
			ALREADY_SENT,
			NO_SCORE,
			BANNED,
			TOO_MANY_PENDING_REQUESTS,
			NO_DUEL_SCORE,
		}
	}
}