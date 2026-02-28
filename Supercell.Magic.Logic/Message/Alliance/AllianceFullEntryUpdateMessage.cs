using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceFullEntryUpdateMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24324;

		private int m_warState;
		private string m_description;

		private LogicLong m_currentWarId;
		private AllianceHeaderEntry m_headerEntry;

		public AllianceFullEntryUpdateMessage() : this(0)
		{
			// AllianceFullEntryUpdateMessage.
		}

		public AllianceFullEntryUpdateMessage(short messageVersion) : base(messageVersion)
		{
			// AllianceFullEntryUpdateMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_description = m_stream.ReadString(1000);
			m_stream.ReadInt();
			m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_currentWarId = m_stream.ReadLong();
			}

			m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_stream.ReadLong();
			}

			m_headerEntry = new AllianceHeaderEntry();
			m_headerEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_description);
			m_stream.WriteInt(m_warState);
			m_stream.WriteInt(50);

			if (m_currentWarId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_currentWarId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_stream.WriteInt(0);

			if (false)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(new LogicLong());
			}

			m_stream.WriteBoolean(false);

			m_headerEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AllianceFullEntryUpdateMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_headerEntry = null;
		}

		public AllianceHeaderEntry RemoveAllianceHeaderEntry()
		{
			AllianceHeaderEntry tmp = m_headerEntry;
			m_headerEntry = null;
			return tmp;
		}

		public void SetAllianceHeaderEntry(AllianceHeaderEntry entry)
		{
			m_headerEntry = entry;
		}

		public string GetDescription()
			=> m_description;

		public void SetDescription(string value)
		{
			m_description = value;
		}

		public LogicLong GetCurrentWarId()
			=> m_currentWarId;

		public void SetCurrentWarId(LogicLong value)
		{
			m_currentWarId = value;
		}

		public int GetWarState()
			=> m_warState;

		public void SetWarState(int value)
		{
			m_warState = value;
		}
	}
}