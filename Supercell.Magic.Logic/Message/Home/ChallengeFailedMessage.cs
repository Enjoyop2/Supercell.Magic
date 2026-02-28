using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class ChallengeFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24121;

		private Reason m_reason;

		public ChallengeFailedMessage() : this(0)
		{
			// ChallengeFailedMessage.
		}

		public ChallengeFailedMessage(short messageVersion) : base(messageVersion)
		{
			// ChallengeFailedMessage.
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
			=> ChallengeFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

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
			GENERIC = 0,
			ALREADY_CLOSED = 3,
			PERSONAL_BREAK_ATTACK_DISABLED = 5
		}
	}
}