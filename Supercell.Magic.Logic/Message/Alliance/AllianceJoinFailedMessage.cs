using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceJoinFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24302;

		private Reason m_reason;

		public AllianceJoinFailedMessage() : this(0)
		{
			// AllianceJoinFailedMessage.
		}

		public AllianceJoinFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceJoinFailedMessage.
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
			=> AllianceJoinFailedMessage.MESSAGE_TYPE;

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
			FULL,
			CLOSED,
			ALREADY_IN_ALLIANCE,
			SCORE,
			BANNED,
		}
	}
}