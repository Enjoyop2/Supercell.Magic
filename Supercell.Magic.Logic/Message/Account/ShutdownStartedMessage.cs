using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ShutdownStartedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20161;

		private int m_secondsUntilShutdown;

		public ShutdownStartedMessage() : this(0)
		{
			// ShutdownStartedMessage.
		}

		public ShutdownStartedMessage(short messageVersion) : base(messageVersion)
		{
			// ShutdownStartedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_secondsUntilShutdown = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_secondsUntilShutdown);
		}

		public override short GetMessageType()
			=> ShutdownStartedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetSecondsUntilShutdown()
			=> m_secondsUntilShutdown;

		public void SetSecondsUntilShutdown(int value)
		{
			m_secondsUntilShutdown = value;
		}
	}
}