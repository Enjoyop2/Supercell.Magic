using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class RespondToAllianceJoinRequestMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14321;

		private LogicLong m_streamEntryId;
		private bool m_accepted;

		public RespondToAllianceJoinRequestMessage() : this(0)
		{
			// RespondToAllianceJoinRequestMessage.
		}

		public RespondToAllianceJoinRequestMessage(short messageVersion) : base(messageVersion)
		{
			// RespondToAllianceJoinRequestMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_streamEntryId = m_stream.ReadLong();
			m_accepted = m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_streamEntryId);
			m_stream.WriteBoolean(m_accepted);
		}

		public override short GetMessageType()
			=> RespondToAllianceJoinRequestMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public LogicLong GetStreamEntryId()
			=> m_streamEntryId;

		public void SetStreamEntryId(LogicLong value)
		{
			m_streamEntryId = value;
		}

		public bool IsAccepted()
			=> m_accepted;

		public void SetAccepted(bool value)
		{
			m_accepted = value;
		}
	}
}