using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AskForAllianceDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14302;

		private LogicLong m_allianceId;

		public AskForAllianceDataMessage() : this(0)
		{
			// AskForAllianceDataMessage.
		}

		public AskForAllianceDataMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAllianceDataMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_allianceId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_allianceId);
		}

		public override short GetMessageType()
			=> AskForAllianceDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

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
	}
}