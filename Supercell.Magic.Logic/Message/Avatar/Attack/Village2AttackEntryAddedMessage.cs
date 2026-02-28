using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2AttackEntryAddedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24372;

		private Village2AttackEntry m_attackEntry;

		public Village2AttackEntryAddedMessage() : this(0)
		{
			// Village2AttackEntryAddedMessage.
		}

		public Village2AttackEntryAddedMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackEntryAddedMessage.
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
			=> Village2AttackEntryAddedMessage.MESSAGE_TYPE;

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