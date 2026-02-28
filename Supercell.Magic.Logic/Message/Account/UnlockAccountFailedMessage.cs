using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class UnlockAccountFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20133;

		private ErrorCode m_errorCode;

		public UnlockAccountFailedMessage() : this(0)
		{
			// UnlockAccountFailedMessage.
		}

		public UnlockAccountFailedMessage(short messageVersion) : base(messageVersion)
		{
			// UnlockAccountFailedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_errorCode = (ErrorCode)m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt((int)m_errorCode);
		}

		public override short GetMessageType()
			=> UnlockAccountFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public ErrorCode GetErrorCode()
			=> m_errorCode;

		public void SetErrorCode(ErrorCode errorCode)
		{
			m_errorCode = errorCode;
		}

		public enum ErrorCode
		{
			UNLOCK_FAILED = 4,
			UNLOCK_UNAVAILABLE = 5,
			SERVER_MAINTENANCE = 10
		}
	}
}