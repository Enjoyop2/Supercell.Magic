using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class AllianceStreamEntryMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24312;

		private StreamEntry m_streamEntry;

		public AllianceStreamEntryMessage() : this(0)
		{
			// AllianceStreamEntryMessage.
		}

		public AllianceStreamEntryMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceStreamEntryMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_streamEntry = StreamEntryFactory.CreateStreamEntryByType((StreamEntryType)m_stream.ReadInt());
			m_streamEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt((int)m_streamEntry.GetStreamEntryType());
			m_streamEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AllianceStreamEntryMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_streamEntry = null;
		}

		public StreamEntry GetStreamEntryId()
			=> m_streamEntry;

		public void SetStreamEntry(StreamEntry value)
		{
			m_streamEntry = value;
		}
	}
}