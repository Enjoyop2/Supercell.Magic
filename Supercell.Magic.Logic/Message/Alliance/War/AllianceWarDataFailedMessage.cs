using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarDataFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24337;

		public const int WAR_DATA_ERROR_INTERNAL = 2;
		public const int WAR_DATA_ERROR_INVALID_WAR = 1;
		public const int WAR_DATA_ERROR_ERROR_NO_LONGER_AVAILABLE = 0;

		private int m_errorCode;

		public AllianceWarDataFailedMessage() : this(0)
		{
			// AllianceWarDataFailedMessage.
		}

		public AllianceWarDataFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarDataFailedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_errorCode = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_errorCode);
		}

		public override short GetMessageType()
			=> AllianceWarDataFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetErrorCode()
			=> m_errorCode;

		public void SetErrorCode(int value)
		{
			m_errorCode = value;
		}
	}
}