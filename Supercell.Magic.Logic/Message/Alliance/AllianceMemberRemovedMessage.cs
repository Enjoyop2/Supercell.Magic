using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceMemberRemovedMessage : PiranhaMessage
	{
		private LogicLong m_allianceMemberId;

		public AllianceMemberRemovedMessage() : this(0)
		{
			// AllianceMemberRemovedMessage.
		}

		public AllianceMemberRemovedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceMemberRemovedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_allianceMemberId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_allianceMemberId);
		}

		public override short GetMessageType()
			=> 24309;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceMemberId = null;
		}

		public LogicLong RemoveMemberAvatarId()
		{
			LogicLong tmp = m_allianceMemberId;
			m_allianceMemberId = null;
			return tmp;
		}

		public void SetMemberAvatarId(LogicLong value)
		{
			m_allianceMemberId = value;
		}
	}
}