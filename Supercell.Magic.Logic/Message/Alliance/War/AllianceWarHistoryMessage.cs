using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarHistoryMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24338;
		private LogicArrayList<AllianceWarHistoryEntry> m_allianceWarHistoryList;

		public AllianceWarHistoryMessage() : this(0)
		{
			// AllianceWarHistoryMessage.
		}

		public AllianceWarHistoryMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarHistoryMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count >= 0)
			{
				Debugger.DoAssert(count < 1000, "Too many entries for alliance war history message");
				m_allianceWarHistoryList = new LogicArrayList<AllianceWarHistoryEntry>(count);

				for (int i = 0; i < count; i++)
				{
					AllianceWarHistoryEntry allianceWarHistoryEntry = new AllianceWarHistoryEntry();
					allianceWarHistoryEntry.Decode(m_stream);
					m_allianceWarHistoryList.Add(allianceWarHistoryEntry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_allianceWarHistoryList != null)
			{
				m_stream.WriteInt(m_allianceWarHistoryList.Size());

				for (int i = 0; i < m_allianceWarHistoryList.Size(); i++)
				{
					m_allianceWarHistoryList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> AllianceWarDataFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicArrayList<AllianceWarHistoryEntry> GetAllianceWarHistoryList()
			=> m_allianceWarHistoryList;

		public void SetAllianceWarHistoryList(LogicArrayList<AllianceWarHistoryEntry> list)
		{
			m_allianceWarHistoryList = list;
		}
	}
}