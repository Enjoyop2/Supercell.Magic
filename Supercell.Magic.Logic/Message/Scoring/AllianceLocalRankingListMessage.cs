using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AllianceLocalRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24402;

		private int m_villageType;
		private LogicArrayList<AllianceRankingEntry> m_allianceRankingList;

		public AllianceLocalRankingListMessage() : this(0)
		{
			// AllianceLocalRankingListMessage.
		}

		public AllianceLocalRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceLocalRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count > -1)
			{
				m_allianceRankingList = new LogicArrayList<AllianceRankingEntry>(count);

				for (int i = 0; i < count; i++)
				{
					AllianceRankingEntry allianceRankingEntry = new AllianceRankingEntry();
					allianceRankingEntry.Decode(m_stream);
					m_allianceRankingList.Add(allianceRankingEntry);
				}
			}

			m_villageType = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			if (m_allianceRankingList != null)
			{
				m_stream.WriteInt(m_allianceRankingList.Size());

				for (int i = 0; i < m_allianceRankingList.Size(); i++)
				{
					m_allianceRankingList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}

			m_stream.WriteInt(m_villageType);
		}

		public override short GetMessageType()
			=> AllianceLocalRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();

			m_allianceRankingList = null;
		}

		public LogicArrayList<AllianceRankingEntry> RemoveAllianceRankingList()
		{
			LogicArrayList<AllianceRankingEntry> tmp = m_allianceRankingList;
			m_allianceRankingList = null;
			return tmp;
		}

		public void SetAllianceRankingList(LogicArrayList<AllianceRankingEntry> list)
		{
			m_allianceRankingList = list;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int value)
		{
			m_villageType = value;
		}
	}
}