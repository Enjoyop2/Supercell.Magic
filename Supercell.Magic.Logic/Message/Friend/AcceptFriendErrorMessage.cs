using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class AcceptFriendErrorMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20501;

		private ErrorCode m_errorCode;

		public AcceptFriendErrorMessage() : this(0)
		{
			// AcceptFriendErrorMessage.
		}

		public AcceptFriendErrorMessage(short messageVersion) : base(messageVersion)
		{
			// AcceptFriendErrorMessage.
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
			=> AcceptFriendErrorMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 3;

		public override void Destruct()
		{
			base.Destruct();
		}

		public ErrorCode GetErrorCode()
			=> m_errorCode;

		public void SetErrorCode(ErrorCode value)
		{
			m_errorCode = value;
		}

		public enum ErrorCode
		{
			GENERIC,
			BANNED,
			TOO_MANY_FRIENDS
		}
	}
}