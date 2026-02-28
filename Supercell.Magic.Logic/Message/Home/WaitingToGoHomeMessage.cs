using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class WaitingToGoHomeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24112;

		private int m_estimatedTimeSeconds;

		public WaitingToGoHomeMessage() : this(0)
		{
			// WaitingToGoHomeMessage.
		}

		public WaitingToGoHomeMessage(short messageVersion) : base(messageVersion)
		{
			// WaitingToGoHomeMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_estimatedTimeSeconds = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_estimatedTimeSeconds);
		}

		public override short GetMessageType()
			=> WaitingToGoHomeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetEstimatedTimeSeconds()
			=> m_estimatedTimeSeconds;

		public void SetEstimatedTimeSeconds(int value)
		{
			m_estimatedTimeSeconds = value;
		}
	}
}