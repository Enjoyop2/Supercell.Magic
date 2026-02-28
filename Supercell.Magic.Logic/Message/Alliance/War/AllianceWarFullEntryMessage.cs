using Supercell.Magic.Logic.Message.Alliance.War.Event;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarFullEntryMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24335;

		private int m_warState;
		private int m_warStateRemainingSeconds;

		private LogicLong m_warId;

		private AllianceWarEntry m_ownAllianceWarEntry;
		private AllianceWarEntry m_enemyAllianceWarEntry;
		private LogicArrayList<WarEventEntry> m_warEventEntryList;

		public AllianceWarFullEntryMessage() : this(0)
		{
			// AllianceWarDataMessage.
		}

		public AllianceWarFullEntryMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarFullEntryMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_warState = m_stream.ReadInt();
			m_warStateRemainingSeconds = m_stream.ReadInt();
			m_ownAllianceWarEntry = new AllianceWarEntry();
			m_ownAllianceWarEntry.Decode(m_stream);

			if (m_stream.ReadBoolean())
			{
				m_enemyAllianceWarEntry = new AllianceWarEntry();
				m_enemyAllianceWarEntry.Decode(m_stream);
			}

			if (m_stream.ReadBoolean())
			{
				m_warId = m_stream.ReadLong();
			}

			m_stream.ReadInt();

			int count = m_stream.ReadInt();

			if (count >= 0)
			{
				m_warEventEntryList = new LogicArrayList<WarEventEntry>(count);

				for (int i = count - 1; i >= 0; i--)
				{
					WarEventEntry warEventEntry = WarEventEntryFactory.CreateWarEventEntryByType(m_stream.ReadInt());
					warEventEntry.Decode(m_stream);
					m_warEventEntryList.Add(warEventEntry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_warState);
			m_stream.WriteInt(m_warStateRemainingSeconds);

			m_ownAllianceWarEntry.Encode(m_stream);

			if (m_enemyAllianceWarEntry != null)
			{
				m_stream.WriteBoolean(true);
				m_enemyAllianceWarEntry.Encode(m_stream);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_stream.WriteInt(1);

			if (m_warEventEntryList != null)
			{
				m_stream.WriteInt(m_warEventEntryList.Size());

				for (int i = 0; i < m_warEventEntryList.Size(); i++)
				{
					m_stream.WriteInt(m_warEventEntryList[i].GetWarEventEntryType());
					m_warEventEntryList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> AllianceWarFullEntryMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 25;

		public override void Destruct()
		{
			base.Destruct();
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

		public LogicLong GetWarId()
			=> m_warId;

		public void SetWarId(LogicLong value)
		{
			m_warId = value;
		}

		public AllianceWarEntry GetOwnAllianceWarEntry()
			=> m_ownAllianceWarEntry;

		public void SetOwnAllianceWarEntry(AllianceWarEntry value)
		{
			m_ownAllianceWarEntry = value;
		}

		public AllianceWarEntry GetEnemyAllianceWarEntry()
			=> m_enemyAllianceWarEntry;

		public void SetEnemyAllianceWarEntry(AllianceWarEntry value)
		{
			m_enemyAllianceWarEntry = value;
		}

		public LogicArrayList<WarEventEntry> GetWarEventEntryList()
			=> m_warEventEntryList;

		public void SetWarEventEntryList(LogicArrayList<WarEventEntry> value)
		{
			m_warEventEntryList = value;
		}
	}
}