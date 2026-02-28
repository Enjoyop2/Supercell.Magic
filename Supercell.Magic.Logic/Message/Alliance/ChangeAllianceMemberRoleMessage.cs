using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class ChangeAllianceMemberRoleMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14306;

		private LogicLong m_memberId;
		private LogicAvatarAllianceRole m_memberRole;

		public ChangeAllianceMemberRoleMessage() : this(0)
		{
			// ChangeAllianceMemberRoleMessage.
		}

		public ChangeAllianceMemberRoleMessage(short messageVersion) : base(messageVersion)
		{
			// ChangeAllianceMemberRoleMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_memberId = m_stream.ReadLong();
			m_memberRole = (LogicAvatarAllianceRole)m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_memberId);
			m_stream.WriteInt((int)m_memberRole);
		}

		public override short GetMessageType()
			=> ChangeAllianceMemberRoleMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public LogicLong RemoveMemberId()
		{
			LogicLong tmp = m_memberId;
			m_memberId = null;
			return tmp;
		}

		public LogicAvatarAllianceRole GetMemberRole()
			=> m_memberRole;

		public void SetAllianceData(LogicLong memberId, LogicAvatarAllianceRole memberRole)
		{
			m_memberId = memberId;
			m_memberRole = memberRole;
		}
	}
}