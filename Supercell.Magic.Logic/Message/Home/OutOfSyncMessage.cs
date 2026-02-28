using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class OutOfSyncMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24104;

		private int m_subtick;
		private int m_clientChecksum;
		private int m_serverChecksum;

		private LogicJSONObject m_debugJSON;

		public OutOfSyncMessage() : this(0)
		{
			// OutOfSyncMessage.
		}

		public OutOfSyncMessage(short messageVersion) : base(messageVersion)
		{
			// OutOfSyncMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_serverChecksum = m_stream.ReadInt();
			m_clientChecksum = m_stream.ReadInt();
			m_subtick = m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				string json = m_stream.ReadString(900000);

				if (json != null)
				{
					m_debugJSON = LogicJSONParser.ParseObject(json);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_serverChecksum);
			m_stream.WriteInt(m_clientChecksum);
			m_stream.WriteInt(m_subtick);

			if (m_debugJSON != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteString(LogicJSONParser.CreateJSONString(m_debugJSON, 1024));
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> OutOfSyncMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
			m_debugJSON = null;
		}

		public int GetServerChecksum()
			=> m_serverChecksum;

		public void SetServerChecksum(int value)
		{
			m_serverChecksum = value;
		}

		public int GetClientChecksum()
			=> m_clientChecksum;

		public void SetClientChecksum(int value)
		{
			m_clientChecksum = value;
		}

		public int GetSubTick()
			=> m_subtick;

		public void SetSubTick(int value)
		{
			m_subtick = value;
		}

		public LogicJSONObject RemoveDebugJSON()
		{
			LogicJSONObject tmp = m_debugJSON;
			m_debugJSON = null;
			return tmp;
		}

		public void SetDebugJSON(LogicJSONObject json)
		{
			m_debugJSON = json;
		}
	}
}