using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AllianceBookmarksFullDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24341;
		private LogicArrayList<AllianceHeaderEntry> m_allianceList;

		public AllianceBookmarksFullDataMessage() : this(0)
		{
			// AllianceBookmarksFullDataMessage.
		}

		public AllianceBookmarksFullDataMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceBookmarksFullDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count >= 0)
			{
				m_allianceList = new LogicArrayList<AllianceHeaderEntry>(count);

				for (int i = 0; i < count; i++)
				{
					AllianceHeaderEntry headerEntry = new AllianceHeaderEntry();
					headerEntry.Decode(m_stream);
					m_allianceList.Add(headerEntry);
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
		}

		public override short GetMessageType()
			=> AllianceBookmarksFullDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceList = null;
		}

		public LogicArrayList<AllianceHeaderEntry> GetAllianceList()
			=> m_allianceList;

		public void SetAlliances(LogicArrayList<AllianceHeaderEntry> list)
		{
			m_allianceList = list;
		}
	}
}