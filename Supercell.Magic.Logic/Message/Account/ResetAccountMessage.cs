using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ResetAccountMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10116;

		private int m_accountPreset;

		public ResetAccountMessage() : this(0)
		{
			// ResetAccountMessage.
		}

		public ResetAccountMessage(short messageVersion) : base(messageVersion)
		{
			// ResetAccountMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_accountPreset = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_accountPreset);
		}

		public override short GetMessageType()
			=> ResetAccountMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetAccountPreset()
			=> m_accountPreset;

		public void SetAccountPreset(int value)
		{
			m_accountPreset = value;
		}
	}
}