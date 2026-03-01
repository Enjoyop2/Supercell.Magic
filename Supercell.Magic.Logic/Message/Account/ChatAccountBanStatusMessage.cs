using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ChatAccountBanStatusMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20118;

		private int m_banSecs;

		public ChatAccountBanStatusMessage() : this(0)
		{
			// ChatAccountBanStatusMessage.
		}

		public ChatAccountBanStatusMessage(short messageVersion) : base(messageVersion)
		{
			// ChatAccountBanStatusMessage.
		}

		public override void Decode()
		{
			base.Decode();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_banSecs);
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override short GetMessageType()
			=> ChatAccountBanStatusMessage.MESSAGE_TYPE;

		public int GetBanSeconds()
			=> m_banSecs;

		public void SetBanSeconds(int value)
		{
			m_banSecs = value;
		}
	}
}