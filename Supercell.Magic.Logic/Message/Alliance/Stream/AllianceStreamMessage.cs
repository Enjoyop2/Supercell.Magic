using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class AllianceStreamMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24311;

		private LogicArrayList<StreamEntry> m_streamEntryList;

		public AllianceStreamMessage() : this(0)
		{
			// AllianceStreamMessage.
		}

		public AllianceStreamMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceStreamMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int arraySize = m_stream.ReadInt();

			if (arraySize > 0)
			{
				m_streamEntryList = new LogicArrayList<StreamEntry>(arraySize);

				do
				{
					StreamEntry streamEntry = StreamEntryFactory.CreateStreamEntryByType((StreamEntryType)m_stream.ReadInt());
					streamEntry.Decode(m_stream);
					m_streamEntryList.Add(streamEntry);
				} while (--arraySize > 0);
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(0);

			if (m_streamEntryList != null)
			{
				m_stream.WriteInt(m_streamEntryList.Size());

				for (int i = 0; i < m_streamEntryList.Size(); i++)
				{
					m_stream.WriteInt((int)m_streamEntryList[i].GetStreamEntryType());
					m_streamEntryList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> AllianceStreamMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_streamEntryList = null;
		}

		public LogicArrayList<StreamEntry> RemovestreamEntries()
		{
			LogicArrayList<StreamEntry> tmp = m_streamEntryList;
			m_streamEntryList = null;
			return tmp;
		}

		public void SetStreamEntries(LogicArrayList<StreamEntry> array)
		{
			m_streamEntryList = array;
		}
	}
}