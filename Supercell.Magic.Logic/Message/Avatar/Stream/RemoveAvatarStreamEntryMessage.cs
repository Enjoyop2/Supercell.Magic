using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class RemoveAvatarStreamEntryMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14418;
		private LogicLong m_streamId;

		public RemoveAvatarStreamEntryMessage() : this(0)
		{
			// RemoveAvatarStreamEntryMessage.
		}

		public RemoveAvatarStreamEntryMessage(short messageVersion) : base(messageVersion)
		{
			// RemoveAvatarStreamEntryMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_streamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_streamId);
		}

		public override short GetMessageType()
			=> RemoveAvatarStreamEntryMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_streamId = null;
		}

		public LogicLong GetStreamEntryId()
			=> m_streamId;

		public void SetStreamEntryId(LogicLong value)
		{
			m_streamId = value;
		}
	}
}