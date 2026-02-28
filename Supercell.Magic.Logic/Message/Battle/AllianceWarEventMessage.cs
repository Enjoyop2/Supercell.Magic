using Supercell.Magic.Logic.Message.Alliance.War;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Battle
{
	public class AllianceWarEventMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 25006;

		private AllianceWarMemberEntry m_allianceWarMemberEntry;
		private EventType m_eventType;

		public AllianceWarEventMessage() : this(0)
		{
			// AllianceWarEventMessage.
		}

		public AllianceWarEventMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarEventMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceWarMemberEntry = new AllianceWarMemberEntry();
			m_allianceWarMemberEntry.Decode(m_stream);
			m_eventType = (EventType)m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_allianceWarMemberEntry.Encode(m_stream);
			m_stream.WriteInt((int)m_eventType);
		}

		public override short GetMessageType()
			=> AllianceWarEventMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public enum EventType
		{
			DESTRUCTION_25_PERCENT,
			DESTRUCTION_50_PERCENT,
			DESTRUCTION_75_PERCENT,
			TOWN_HALL_DESTROYED
		}
	}
}