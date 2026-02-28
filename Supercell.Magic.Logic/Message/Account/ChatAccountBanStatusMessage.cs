using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ChatAccountBanStatusMessage : PiranhaMessage
	{
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
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetBanSeconds()
			=> m_banSecs;

		public void SetBanSeconds(int value)
		{
			m_banSecs = value;
		}
	}
}