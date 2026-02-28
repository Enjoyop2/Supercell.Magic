using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2AttackEntryUpdateMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24371;

		private Village2AttackEntry m_attackEntry;

		public Village2AttackEntryUpdateMessage() : this(0)
		{
			// Village2AttackEntryUpdateMessage.
		}

		public Village2AttackEntryUpdateMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackEntryUpdateMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_attackEntry = Village2AttackEntryFactory.CreateAttackEntryByType(m_stream.ReadInt());
			m_attackEntry?.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_attackEntry.GetAttackEntryType());
			m_attackEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> Village2AttackEntryUpdateMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_attackEntry = null;
		}

		public Village2AttackEntry RemoveAttackEntry()
		{
			Village2AttackEntry tmp = m_attackEntry;
			m_attackEntry = null;
			return tmp;
		}

		public void SetAttackEntry(Village2AttackEntry entry)
		{
			m_attackEntry = entry;
		}
	}
}