using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarEntry
	{
		private AllianceWarHeader m_allianceWarHeader;
		private LogicArrayList<AllianceWarMemberEntry> m_allianceWarMemberList;
		private LogicArrayList<int> m_allianceExpMap;

		private bool m_ended;

		private int m_warState;
		private int m_warStateRemainingSeconds;
		private int m_allianceWarLootBonusPecentWin;
		private int m_allianceWarLootBonusPecentDraw;
		private int m_allianceWarLootBonusPecentLose;

		public void Decode(ByteStream stream)
		{
			m_allianceWarHeader = new AllianceWarHeader();
			m_allianceWarHeader.Decode(stream);

			int memberCount = stream.ReadInt();

			if (memberCount >= 0)
			{
				Debugger.DoAssert(memberCount < 1000, "Too many alliance war member entries in AllianceWarEntry");

				m_allianceWarMemberList = new LogicArrayList<AllianceWarMemberEntry>();
				m_allianceWarMemberList.EnsureCapacity(memberCount);

				for (int i = 0; i < memberCount; i++)
				{
					AllianceWarMemberEntry allianceWarMemberEntry = new AllianceWarMemberEntry();
					allianceWarMemberEntry.Decode(stream);
					m_allianceWarMemberList.Add(allianceWarMemberEntry);
				}
			}

			m_ended = stream.ReadBoolean();
			stream.ReadBoolean();

			int expMapCount = stream.ReadInt();

			if (expMapCount >= 0)
			{
				Debugger.DoAssert(expMapCount <= 50, "Too many entries in the alliance exp map");

				m_allianceExpMap = new LogicArrayList<int>();
				m_allianceExpMap.EnsureCapacity(expMapCount);

				for (int i = 0; i < expMapCount; i++)
				{
					m_allianceExpMap.Add(stream.ReadInt());
				}
			}

			m_warState = stream.ReadInt();
			m_warStateRemainingSeconds = stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			m_allianceWarLootBonusPecentWin = stream.ReadInt();
			m_allianceWarLootBonusPecentDraw = stream.ReadInt();
			m_allianceWarLootBonusPecentLose = stream.ReadInt();
		}

		public void Encode(ByteStream encoder)
		{
			m_allianceWarHeader.Encode(encoder);

			if (m_allianceWarMemberList != null)
			{
				encoder.WriteInt(m_allianceWarMemberList.Size());

				for (int i = 0; i < m_allianceWarMemberList.Size(); i++)
				{
					m_allianceWarMemberList[i].Encode(encoder);
				}
			}
			else
			{
				encoder.WriteInt(-1);
			}

			encoder.WriteBoolean(m_ended);
			encoder.WriteBoolean(true);

			if (m_allianceExpMap != null)
			{
				encoder.WriteInt(m_allianceExpMap.Size());

				for (int i = 0; i < m_allianceExpMap.Size(); i++)
				{
					encoder.WriteInt(m_allianceExpMap[i]);
				}
			}
			else
			{
				encoder.WriteInt(-1);
			}

			encoder.WriteInt(m_warState);
			encoder.WriteInt(0); // remSecs ?
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(m_allianceWarLootBonusPecentWin);
			encoder.WriteInt(m_allianceWarLootBonusPecentDraw);
			encoder.WriteInt(m_allianceWarLootBonusPecentLose);
		}

		public AllianceWarHeader GetAllianceWarHeader()
			=> m_allianceWarHeader;

		public void SetAllianceWarHeader(AllianceWarHeader value)
		{
			m_allianceWarHeader = value;
		}

		public LogicArrayList<AllianceWarMemberEntry> GetAllianceWarMemberList()
			=> m_allianceWarMemberList;

		public int GetAllianceWarMemberCount()
			=> m_allianceWarMemberList.Size();

		public void SetAllianceWarMemberList(LogicArrayList<AllianceWarMemberEntry> value)
		{
			m_allianceWarMemberList = value;
		}

		public LogicArrayList<int> GetAllianceExpMap()
			=> m_allianceExpMap;

		public void SetAllianceExpMap(LogicArrayList<int> value)
		{
			m_allianceExpMap = value;
		}

		public bool IsEnded()
			=> m_ended;

		public void SetEnded(bool value)
		{
			m_ended = value;
		}

		public int GetWarState()
			=> m_warState;

		public void SetWarState(int value)
		{
			m_warState = value;
		}

		public int GetWarStateRemainingSeconds()
			=> m_warStateRemainingSeconds;

		public void SetWarStateRemainingSeconds(int value)
		{
			m_warStateRemainingSeconds = value;
		}

		public int GetAllianceWarLootBonusPecentWin()
			=> m_allianceWarLootBonusPecentWin;

		public void SetAllianceWarLootBonusPecentWin(int value)
		{
			m_allianceWarLootBonusPecentWin = value;
		}

		public int GetAllianceWarLootBonusPecentDraw()
			=> m_allianceWarLootBonusPecentDraw;

		public void SetAllianceWarLootBonusPecentDraw(int value)
		{
			m_allianceWarLootBonusPecentDraw = value;
		}

		public int GetAllianceWarLootBonusPecentLose()
			=> m_allianceWarLootBonusPecentLose;

		public void SetAllianceWarLootBonusPecentLose(int value)
		{
			m_allianceWarLootBonusPecentLose = value;
		}
	}
}