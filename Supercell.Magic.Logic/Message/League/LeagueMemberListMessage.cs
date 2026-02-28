using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.League
{
	public class LeagueMemberListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24503;

		private int m_remainingSeasonTime;
		private LogicArrayList<LeagueMemberEntry> m_memberList;

		public LeagueMemberListMessage() : this(0)
		{
			// LeagueMemberListMessage.
		}

		public LeagueMemberListMessage(short messageVersion) : base(messageVersion)
		{
			// LeagueMemberListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_remainingSeasonTime = m_stream.ReadInt();
			int arraySize = m_stream.ReadInt();

			if (arraySize > -1)
			{
				m_memberList = new LogicArrayList<LeagueMemberEntry>(arraySize);

				for (int i = 0; i < arraySize; i++)
				{
					LeagueMemberEntry leagueMemberEntry = new LeagueMemberEntry();
					leagueMemberEntry.Decode(m_stream);
					m_memberList.Add(leagueMemberEntry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_remainingSeasonTime);

			if (m_memberList != null)
			{
				m_stream.WriteInt(m_memberList.Size());

				for (int i = 0; i < m_memberList.Size(); i++)
				{
					m_memberList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> LeagueMemberListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 13;

		public override void Destruct()
		{
			base.Destruct();
			m_memberList = null;
		}

		public LogicArrayList<LeagueMemberEntry> GetMemberList()
			=> m_memberList;

		public void SetMemberList(LogicArrayList<LeagueMemberEntry> entry)
		{
			m_memberList = entry;
		}

		public int GetRemainingSeasonTime()
			=> m_remainingSeasonTime;

		public void SetRemainingSeasonTime(int value)
		{
			m_remainingSeasonTime = value;
		}
	}
}