using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class JoinAllianceUsingInvitationMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14323;

		private LogicLong m_avatarStreamEntryId;

		public JoinAllianceUsingInvitationMessage() : this(0)
		{
			// JoinAllianceUsingInvitationMessage.
		}

		public JoinAllianceUsingInvitationMessage(short messageVersion) : base(messageVersion)
		{
			// JoinAllianceUsingInvitationMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_avatarStreamEntryId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_avatarStreamEntryId);
		}

		public override short GetMessageType()
			=> JoinAllianceUsingInvitationMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public LogicLong GetAvatarStreamEntryId()
			=> m_avatarStreamEntryId;

		public void SetAvatarStreamEntryId(LogicLong value)
		{
			m_avatarStreamEntryId = value;
		}
	}
}