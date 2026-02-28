using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Battle
{
	public class AttackEventMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 25027;

		private EventType m_eventType;
		private int m_stars;

		public AttackEventMessage() : this(0)
		{
			// AttackEventMessage.
		}

		public AttackEventMessage(short messageVersion) : base(messageVersion)
		{
			// AttackEventMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_eventType = (EventType)m_stream.ReadInt();
			m_stars = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt((int)m_eventType);
			m_stream.WriteInt(m_stars);
		}

		public override short GetMessageType()
			=> AttackEventMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}

		public EventType GetEventType()
			=> m_eventType;

		public void SetEventType(EventType value)
		{
			m_eventType = value;
		}

		public int GetStars()
			=> m_stars;

		public void SetStars(int value)
		{
			m_stars = value;
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