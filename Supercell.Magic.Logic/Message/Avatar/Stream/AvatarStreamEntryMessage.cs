using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AvatarStreamEntryMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24412;

		private AvatarStreamEntry m_avatarStreamEntry;

		public AvatarStreamEntryMessage() : this(0)
		{
			// AvatarStreamEntryMessage.
		}

		public AvatarStreamEntryMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarStreamEntryMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_avatarStreamEntry = AvatarStreamEntryFactory.CreateStreamEntryByType((AvatarStreamEntryType)m_stream.ReadInt());
			m_avatarStreamEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt((int)m_avatarStreamEntry.GetAvatarStreamEntryType());
			m_avatarStreamEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AvatarStreamEntryMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarStreamEntry = null;
		}

		public AvatarStreamEntry RemoveAvatarStreamEntry()
		{
			AvatarStreamEntry tmp = m_avatarStreamEntry;
			m_avatarStreamEntry = null;
			return tmp;
		}

		public void SetAvatarStreamEntry(AvatarStreamEntry entry)
		{
			m_avatarStreamEntry = entry;
		}
	}
}