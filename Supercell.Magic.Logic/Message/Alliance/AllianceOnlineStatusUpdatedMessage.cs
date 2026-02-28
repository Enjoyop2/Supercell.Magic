using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceOnlineStatusUpdatedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20207;

		private int m_memberCount;
		private int m_onlineMemberCount;

		public AllianceOnlineStatusUpdatedMessage() : this(0)
		{
			// AllianceOnlineStatusUpdatedMessage.
		}

		public AllianceOnlineStatusUpdatedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceOnlineStatusUpdatedMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_onlineMemberCount = m_stream.ReadVInt();
			m_memberCount = m_stream.ReadVInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteVInt(m_onlineMemberCount);
			m_stream.WriteVInt(m_memberCount);
		}

		public override short GetMessageType()
			=> AllianceOnlineStatusUpdatedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetMembersOnline()
			=> m_onlineMemberCount;

		public void SetMembersOnline(int value)
		{
			m_onlineMemberCount = value;
		}

		public int GetMembersCount()
			=> m_memberCount;

		public void SetMembersCount(int value)
		{
			m_memberCount = value;
		}
	}
}