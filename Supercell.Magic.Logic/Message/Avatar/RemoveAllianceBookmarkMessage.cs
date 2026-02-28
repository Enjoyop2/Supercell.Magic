using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class RemoveAllianceBookmarkMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14344;
		private LogicLong m_allianceId;

		public RemoveAllianceBookmarkMessage() : this(0)
		{
			// RemoveAllianceBookmarkMessage.
		}

		public RemoveAllianceBookmarkMessage(short messageVersion) : base(messageVersion)
		{
			// RemoveAllianceBookmarkMessage.
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
			=> RemoveAllianceBookmarkMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceId = null;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}
	}
}