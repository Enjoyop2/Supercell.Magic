using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarNameCheckResponseMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20300;

		private bool m_invalid;
		private ErrorCode m_errorCode;
		private string m_name;

		public AvatarNameCheckResponseMessage() : this(0)
		{
			// AvatarNameCheckResponseMessage.
		}

		public AvatarNameCheckResponseMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarNameCheckResponseMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_invalid = m_stream.ReadBoolean();
			m_errorCode = (ErrorCode)m_stream.ReadInt();
			m_name = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteBoolean(m_invalid);
			m_stream.WriteInt((int)m_errorCode);
			m_stream.WriteString(m_name);
		}

		public override short GetMessageType()
			=> AvatarNameCheckResponseMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_name = null;
		}

		public string GetName()
			=> m_name;

		public void SetName(string name)
		{
			m_name = name;
		}

		public bool IsInvalid()
			=> m_invalid;

		public void SetInvalid(bool invalid)
		{
			m_invalid = invalid;
		}

		public ErrorCode GetErrorCode()
			=> m_errorCode;

		public void SetErrorCode(ErrorCode errorCode)
		{
			m_errorCode = errorCode;
		}

		public enum ErrorCode
		{
			INVALID_NAME = 1,
			NAME_TOO_SHORT = 2,
			NAME_TOO_LONG = 3,
			NAME_ALREADY_CHANGED = 4,
			NAME_TH_LEVEL_TOO_LOW = 5
		}
	}
}