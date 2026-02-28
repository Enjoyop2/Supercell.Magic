using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class FriendListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20105;

		private int m_listType;
		private LogicArrayList<FriendEntry> m_friendEntryList;

		public FriendListMessage() : this(0)
		{
			// FriendListMessage.
		}

		public FriendListMessage(short messageVersion) : base(messageVersion)
		{
			// FriendListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_listType = m_stream.ReadInt();

			int count = m_stream.ReadInt();

			if (count != -1)
			{
				m_friendEntryList = new LogicArrayList<FriendEntry>(count);

				for (int i = 0; i < count; i++)
				{
					FriendEntry friendEntry = new FriendEntry();
					friendEntry.Decode(m_stream);
					m_friendEntryList.Add(friendEntry);
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_listType);

			if (m_friendEntryList != null)
			{
				m_stream.WriteInt(m_friendEntryList.Size());

				for (int i = 0; i < m_friendEntryList.Size(); i++)
				{
					m_friendEntryList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> FriendListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 3;

		public override void Destruct()
		{
			base.Destruct();
			m_friendEntryList = null;
		}

		public LogicArrayList<FriendEntry> RemoveFriendEntries()
		{
			LogicArrayList<FriendEntry> tmp = m_friendEntryList;
			m_friendEntryList = null;
			return tmp;
		}

		public void SetFriendEntries(LogicArrayList<FriendEntry> list)
		{
			m_friendEntryList = list;
		}

		public int GetListType()
			=> m_listType;

		public void SetListType(int value)
		{
			m_listType = value;
		}
	}
}