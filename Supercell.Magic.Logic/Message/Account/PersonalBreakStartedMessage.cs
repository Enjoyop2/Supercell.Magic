using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class PersonalBreakStartedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20171;

		private int m_secondsUntilBreak;

		public PersonalBreakStartedMessage() : this(0)
		{
			// PersonalBreakStartedMessage.
		}

		public PersonalBreakStartedMessage(short messageVersion) : base(messageVersion)
		{
			// PersonalBreakStartedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_secondsUntilBreak = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_secondsUntilBreak);
		}

		public override short GetMessageType()
			=> PersonalBreakStartedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetSecondsUntilBreak()
			=> m_secondsUntilBreak;

		public void SetSecondsUntilBreak(int value)
		{
			m_secondsUntilBreak = value;
		}
	}
}