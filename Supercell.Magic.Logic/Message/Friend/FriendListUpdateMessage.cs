using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class FriendListUpdateMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20106;
		private FriendEntry m_friendEntry;

		public FriendListUpdateMessage() : this(0)
		{
			// FriendListUpdateMessage.
		}

		public FriendListUpdateMessage(short messageVersion) : base(messageVersion)
		{
			// FriendListUpdateMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_friendEntry = new FriendEntry();
			m_friendEntry.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			m_friendEntry.Encode(m_stream);
		}

		public override short GetMessageType()
			=> FriendListUpdateMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 3;

		public override void Destruct()
		{
			base.Destruct();
			m_friendEntry = null;
		}

		public FriendEntry RemoveFriendEntry()
		{
			FriendEntry tmp = m_friendEntry;
			m_friendEntry = null;
			return tmp;
		}

		public void SetFriendEntry(FriendEntry entry)
		{
			m_friendEntry = entry;
		}
	}
}