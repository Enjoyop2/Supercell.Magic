using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarProfileMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24334;

		private AvatarProfileFullEntry m_entry;

		public AvatarProfileMessage() : this(0)
		{
			// AvatarProfileMessage.
		}

		public AvatarProfileMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarProfileMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_entry = new AvatarProfileFullEntry();
			m_entry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			m_entry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> AvatarProfileMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();

			if (m_entry != null)
			{
				m_entry.Destruct();
				m_entry = null;
			}
		}

		public AvatarProfileFullEntry RemoveAvatarProfileFullEntry()
		{
			AvatarProfileFullEntry tmp = m_entry;
			m_entry = null;
			return tmp;
		}

		public void SetAvatarProfileFullEntry(AvatarProfileFullEntry entry)
		{
			m_entry = entry;
		}
	}
}