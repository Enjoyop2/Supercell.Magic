using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AllianceRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24401;

		private int m_villageType;
		private int m_nextEndTimeSeconds;

		private LogicArrayList<int> m_diamondPrizes;
		private LogicArrayList<AllianceRankingEntry> m_allianceRankingList;

		public AllianceRankingListMessage() : this(0)
		{
			// AllianceRankingListMessage.
		}

		public AllianceRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceRankingListMessage.
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

			m_nextEndTimeSeconds = m_stream.ReadInt();
			m_diamondPrizes = new LogicArrayList<int>();

			for (int i = 0, size = m_stream.ReadInt(); i < size; i++)
			{
				m_diamondPrizes.Add(m_stream.ReadInt());
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

			m_stream.WriteInt(m_nextEndTimeSeconds);
			m_stream.WriteInt(m_diamondPrizes.Size());

			for (int i = 0; i < m_diamondPrizes.Size(); i++)
			{
				m_stream.WriteInt(m_diamondPrizes[i]);
			}

			m_stream.WriteInt(m_villageType);
		}

		public override short GetMessageType()
			=> AllianceRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();

			m_allianceRankingList = null;
			m_diamondPrizes = null;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int value)
		{
			m_villageType = value;
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

		public LogicArrayList<int> RemoveDiamondPrizes()
		{
			LogicArrayList<int> tmp = m_diamondPrizes;
			m_diamondPrizes = null;
			return tmp;
		}

		public void SetDiamondPrizes(LogicArrayList<int> list)
		{
			m_diamondPrizes = list;
		}

		public int GetNextEndTimeSeconds()
			=> m_nextEndTimeSeconds;

		public void SetNextEndTimeSeconds(int value)
		{
			m_nextEndTimeSeconds = value;
		}
	}
}