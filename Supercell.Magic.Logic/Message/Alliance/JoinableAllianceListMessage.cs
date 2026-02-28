using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class JoinableAllianceListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24304;

		private LogicArrayList<LogicLong> m_bookmarkList;
		private LogicArrayList<AllianceHeaderEntry> m_allianceList;

		public JoinableAllianceListMessage() : this(0)
		{
			// JoinableAllianceListMessage.
		}

		public JoinableAllianceListMessage(short messageVersion) : base(messageVersion)
		{
			// JoinableAllianceListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int arraySize = m_stream.ReadInt();

			if (arraySize <= 10000)
			{
				if (arraySize > -1)
				{
					m_allianceList = new LogicArrayList<AllianceHeaderEntry>(arraySize);

					for (int i = 0; i < arraySize; i++)
					{
						AllianceHeaderEntry allianceHeaderEntry = new AllianceHeaderEntry();
						allianceHeaderEntry.Decode(m_stream);
						m_allianceList.Add(allianceHeaderEntry);
					}
				}
			}

			int array2Size = m_stream.ReadInt();

			if (array2Size <= 10000)
			{
				if (array2Size > -1)
				{
					m_bookmarkList = new LogicArrayList<LogicLong>(array2Size);

					for (int i = 0; i < array2Size; i++)
					{
						m_bookmarkList.Add(m_stream.ReadLong());
					}
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_allianceList != null)
			{
				m_stream.WriteInt(m_allianceList.Size());

				for (int i = 0; i < m_allianceList.Size(); i++)
				{
					m_allianceList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}

			if (m_bookmarkList != null)
			{
				m_stream.WriteInt(m_bookmarkList.Size());

				for (int i = 0; i < m_bookmarkList.Size(); i++)
				{
					m_stream.WriteLong(m_bookmarkList[i]);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> JoinableAllianceListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();

			m_allianceList = null;
			m_bookmarkList = null;
		}

		public LogicArrayList<AllianceHeaderEntry> RemoveAlliances()
		{
			LogicArrayList<AllianceHeaderEntry> tmp = m_allianceList;
			m_allianceList = null;
			return tmp;
		}

		public void SetAlliances(LogicArrayList<AllianceHeaderEntry> alliances)
		{
			m_allianceList = alliances;
		}

		public void SetBookmarkList(LogicArrayList<LogicLong> list)
		{
			m_bookmarkList = list;
		}
	}
}