using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24329;
		private AllianceWarEntry m_allianceWarEntry;

		public AllianceWarDataMessage() : this(0)
		{
			// AllianceWarDataMessage.
		}

		public AllianceWarDataMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceWarEntry = new AllianceWarEntry();
			m_allianceWarEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			m_allianceWarEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AllianceWarDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public AllianceWarEntry GetAllianceWarEntry()
			=> m_allianceWarEntry;

		public void SetAllianceWarEntry(AllianceWarEntry list)
		{
			m_allianceWarEntry = list;
		}
	}
}