using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AddAllianceBookmarkMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14343;
		private LogicLong m_allianceId;

		public AddAllianceBookmarkMessage() : this(0)
		{
			// BookmarksListMessage.
		}

		public AddAllianceBookmarkMessage(short messageVersion) : base(messageVersion)
		{
			// AddAllianceBookmarkMessage.
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
			=> AddAllianceBookmarkMessage.MESSAGE_TYPE;

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