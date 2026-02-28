using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class JoinAllianceMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14305;

		private LogicLong m_allianceId;

		public JoinAllianceMessage() : this(0)
		{
			// JoinAllianceMessage.
		}

		public JoinAllianceMessage(short messageVersion) : base(messageVersion)
		{
			// JoinAllianceMessage.
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
			=> JoinAllianceMessage.MESSAGE_TYPE;

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
	}
}