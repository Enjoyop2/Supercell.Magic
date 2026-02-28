using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class LiveReplayFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24117;

		private Reason m_reason;

		public LiveReplayFailedMessage() : this(0)
		{
			// LiveReplayFailedMessage.
		}

		public LiveReplayFailedMessage(short messageVersion) : base(messageVersion)
		{
			// LiveReplayFailedMessage.
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
			=> LiveReplayFailedMessage.MESSAGE_TYPE;

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
			GENERIC,
			NO_DATA_FOUND,
			NO_FREE_SLOTS
		}
	}
}