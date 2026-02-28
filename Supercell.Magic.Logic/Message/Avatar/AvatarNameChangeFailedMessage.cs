using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarNameChangeFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20205;

		private ErrorCode m_errorCode;

		public AvatarNameChangeFailedMessage() : this(0)
		{
			// AvatarNameChangeFailedMessage.
		}

		public AvatarNameChangeFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarNameChangeFailedMessage.
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
			=> AvatarNameChangeFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

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
			TOO_LONG = 1,
			TOO_SHORT = 2,
			BAD_WORD = 3,
			ALREADY_CHANGED = 4,
			TH_LEVEL_TOO_LOW = 5
		}
	}
}