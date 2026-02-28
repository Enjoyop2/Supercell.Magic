using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24310;

		private string m_searchString;

		private LogicArrayList<LogicLong> m_bookmarkList;
		private LogicArrayList<AllianceHeaderEntry> m_allianceList;

		public AllianceListMessage() : this(0)
		{
			// AllianceListMessage.
		}

		public AllianceListMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceListMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_searchString = m_stream.ReadString(900000);

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
			m_stream.WriteString(m_searchString);

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
			=> AllianceListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();

			m_searchString = null;
			m_allianceList = null;
			m_bookmarkList = null;
		}

		public string RemoveSearchString()
		{
			string tmp = m_searchString;
			m_searchString = null;
			return tmp;
		}

		public void SetSearchString(string value)
		{
			m_searchString = value;
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