using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarProfileFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24339;

		private ErrorType m_errorType;
		private LogicLong m_avatarId;

		public AvatarProfileFailedMessage() : this(0)
		{
			// AvatarProfileFailedMessage.
		}

		public AvatarProfileFailedMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarProfileFailedMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_errorType = (ErrorType)m_stream.ReadInt();
			m_avatarId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt((int)m_errorType);
			m_stream.WriteLong(m_avatarId);
		}

		public override short GetMessageType()
			=> AvatarProfileFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarId = null;
		}

		public ErrorType GetErrorType()
			=> m_errorType;

		public void SetErrorType(ErrorType value)
		{
			m_errorType = value;
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
		}

		public enum ErrorType
		{
			GENERIC,
			INTERNAL_ERROR,
			NOT_FOUND
		}
	}
}