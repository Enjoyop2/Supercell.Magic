using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class SendAllianceInvitationMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14322;

		private LogicLong m_avatarId;

		public SendAllianceInvitationMessage() : this(0)
		{
			// SendAllianceInvitationMessage.
		}

		public SendAllianceInvitationMessage(short messageVersion) : base(messageVersion)
		{
			// SendAllianceInvitationMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_avatarId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_avatarId);
		}

		public override short GetMessageType()
			=> SendAllianceInvitationMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong id)
		{
			m_avatarId = id;
		}
	}
}