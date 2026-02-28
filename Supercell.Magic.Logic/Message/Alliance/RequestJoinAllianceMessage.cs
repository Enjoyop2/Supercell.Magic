using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class RequestJoinAllianceMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14317;

		private LogicLong m_allianceId;
		private string m_message;

		public RequestJoinAllianceMessage() : this(0)
		{
			// RequestJoinAllianceMessage.
		}

		public RequestJoinAllianceMessage(short messageVersion) : base(messageVersion)
		{
			// RequestJoinAllianceMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_allianceId = m_stream.ReadLong();
			m_message = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_allianceId);
			m_stream.WriteString(m_message);
		}

		public override short GetMessageType()
			=> RequestJoinAllianceMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public LogicLong RemoveAllianceId()
		{
			LogicLong tmp = m_allianceId;
			m_allianceId = null;
			return tmp;
		}

		public void SetAllianceId(LogicLong id)
		{
			m_allianceId = id;
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string value)
		{
			m_message = value;
		}
	}
}