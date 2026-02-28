using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AvatarStreamEntryRemovedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24418;
		private LogicLong m_streamId;

		public AvatarStreamEntryRemovedMessage() : this(0)
		{
			// AvatarStreamEntryRemovedMessage.
		}

		public AvatarStreamEntryRemovedMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarStreamEntryRemovedMessage.
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
			=> AvatarStreamEntryRemovedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

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