using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceMemberMessage : PiranhaMessage
	{
		private AllianceMemberEntry m_allianceMemberEntry;

		public AllianceMemberMessage() : this(0)
		{
			// AllianceMemberMessage.
		}

		public AllianceMemberMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceMemberMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceMemberEntry = new AllianceMemberEntry();
			m_allianceMemberEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			m_allianceMemberEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> 24308;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceMemberEntry = null;
		}

		public AllianceMemberEntry RemoveAllianceMemberEntry()
		{
			AllianceMemberEntry tmp = m_allianceMemberEntry;
			m_allianceMemberEntry = null;
			return tmp;
		}

		public void SetAllianceMemberEntry(AllianceMemberEntry value)
		{
			m_allianceMemberEntry = value;
		}
	}
}