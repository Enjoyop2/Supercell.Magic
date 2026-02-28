using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24301;

		private AllianceFullEntry m_fullEntry;

		public AllianceDataMessage() : this(0)
		{
			// AllianceDataMessage.
		}

		public AllianceDataMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_fullEntry = new AllianceFullEntry();
			m_fullEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			m_fullEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AllianceDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_fullEntry = null;
		}

		public AllianceFullEntry RemoveAllianceFullEntry()
		{
			AllianceFullEntry tmp = m_fullEntry;
			m_fullEntry = null;
			return tmp;
		}

		public void SetAllianceFullEntry(AllianceFullEntry entry)
		{
			m_fullEntry = entry;
		}
	}
}