using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ReportUserMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10117;

		private int m_reportSource;

		private LogicLong m_reportedAvatarId;

		public ReportUserMessage() : this(0)
		{
			// ReportUserMessage.
		}

		public ReportUserMessage(short messageVersion) : base(messageVersion)
		{
			// ReportUserMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_reportSource = m_stream.ReadInt();
			m_reportedAvatarId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_reportSource);
			m_stream.WriteLong(m_reportedAvatarId);
		}

		public override short GetMessageType()
			=> ReportUserMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
		public int GetReportSource()
			=> m_reportSource;

		public void SetReportSource(int value)
		{
			m_reportSource = value;
		}

		public LogicLong RemoveReportedAvatarId()
		{
			LogicLong tmp = m_reportedAvatarId;
			m_reportedAvatarId = null;
			return tmp;
		}

		public void SetReportedAvatarId(LogicLong value)
		{
			m_reportedAvatarId = value;
		}
	}
}