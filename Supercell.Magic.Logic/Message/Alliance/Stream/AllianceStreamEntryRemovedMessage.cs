using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class AllianceStreamEntryRemovedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24318;

		private LogicLong m_streamEntryId;

		public AllianceStreamEntryRemovedMessage() : this(0)
		{
			// AllianceStreamEntryRemovedMessage.
		}

		public AllianceStreamEntryRemovedMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceStreamEntryRemovedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_streamEntryId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_streamEntryId);
		}

		public override short GetMessageType()
			=> AllianceStreamEntryRemovedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_streamEntryId = null;
		}

		public LogicLong GetStreamEntryId()
			=> m_streamEntryId;

		public void SetStreamEntryId(LogicLong value)
		{
			m_streamEntryId = value;
		}
	}
}