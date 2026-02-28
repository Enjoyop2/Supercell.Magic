using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2AttackEntryListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24370;

		private bool m_targetList;
		private LogicArrayList<Village2AttackEntry> m_attackEntryList;

		public Village2AttackEntryListMessage() : this(0)
		{
			// Village2AttackEntryListMessage.
		}

		public Village2AttackEntryListMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackEntryListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_targetList = m_stream.ReadBoolean();
			int cnt = m_stream.ReadInt();

			if (cnt != -1)
			{
				m_attackEntryList = new LogicArrayList<Village2AttackEntry>(cnt);

				for (int i = 0; i < cnt; i++)
				{
					Village2AttackEntry entry = Village2AttackEntryFactory.CreateAttackEntryByType(m_stream.ReadInt());
					entry.Decode(m_stream);
					m_attackEntryList.Add(entry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteBoolean(m_targetList);

			if (m_attackEntryList != null)
			{
				m_stream.WriteInt(m_attackEntryList.Size());

				for (int i = 0; i < m_attackEntryList.Size(); i++)
				{
					m_stream.WriteInt(m_attackEntryList[i].GetAttackEntryType());
					m_attackEntryList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> Village2AttackEntryListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_attackEntryList = null;
		}

		public LogicArrayList<Village2AttackEntry> RemoveStreamEntries()
		{
			LogicArrayList<Village2AttackEntry> tmp = m_attackEntryList;
			m_attackEntryList = null;
			return tmp;
		}

		public void SetStreamEntries(LogicArrayList<Village2AttackEntry> entry)
		{
			m_attackEntryList = entry;
		}
	}
}