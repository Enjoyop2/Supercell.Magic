using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarSearchDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24325;
		private LogicArrayList<AllianceWarMemberEntry> m_warMemberEntryList;

		public AllianceWarSearchDataMessage() : this(0)
		{
			// AllianceWarSearchDataMessage.
		}

		public AllianceWarSearchDataMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceWarSearchDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count >= 0)
			{
				m_warMemberEntryList = new LogicArrayList<AllianceWarMemberEntry>();
				m_warMemberEntryList.EnsureCapacity(count);

				for (int i = 0; i < count; i++)
				{
					AllianceWarMemberEntry warMemberEntry = new AllianceWarMemberEntry();
					warMemberEntry.Decode(m_stream);
					m_warMemberEntryList.Add(warMemberEntry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_warMemberEntryList.Size());

			for (int i = 0; i < m_warMemberEntryList.Size(); i++)
			{
				m_warMemberEntryList[i].Encode(m_stream);
			}
		}

		public override short GetMessageType()
			=> AllianceWarSearchDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 25;

		public override void Destruct()
		{
			base.Destruct();
			m_warMemberEntryList = null;
		}

		public LogicArrayList<AllianceWarMemberEntry> GetWarMemberEntryList()
			=> m_warMemberEntryList;

		public void SetWarMemberEntryList(LogicArrayList<AllianceWarMemberEntry> value)
		{
			m_warMemberEntryList = value;
		}
	}
}