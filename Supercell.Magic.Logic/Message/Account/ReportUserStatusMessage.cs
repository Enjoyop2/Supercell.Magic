using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ReportUserStatusMessage : PiranhaMessage
	{
		public enum ErrorCode
		{
			GENERIC = 0,
			SUCCESS = 1,
			TOO_MANY_REPORTS_SENT = 2,
			PLAYER_ALREADY_REPORTED = 3,
			TOO_MANY_ALLIANCE_CHAT_REPORTS_SENT = 6,
			PLAYER_ALLIANCE_ALREADY_REPORTED = 7
		}

		public const int MESSAGE_TYPE = 20117;

		private ErrorCode m_errorCode;

		public ReportUserStatusMessage() : this(0)
		{
			// ReportUserStatusMessage.
		}

		public ReportUserStatusMessage(short messageVersion) : base(messageVersion)
		{
			// ReportUserStatusMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_errorCode = (ErrorCode)m_stream.ReadInt();
			m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt((int)m_errorCode);
			m_stream.WriteInt(0);
		}

		public override short GetMessageType()
			=> ReportUserStatusMessage.MESSAGE_TYPE;

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
	}
}