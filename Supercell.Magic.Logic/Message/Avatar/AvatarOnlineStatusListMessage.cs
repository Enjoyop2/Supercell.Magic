using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarOnlineStatusListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20208;

		private LogicArrayList<int> m_avatarStatusList;
		private LogicArrayList<LogicLong> m_avatarIdList;

		private int m_listType;

		public AvatarOnlineStatusListMessage() : this(0)
		{
			// AvatarOnlineStatusListMessage.
		}

		public AvatarOnlineStatusListMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarOnlineStatusListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			for (int i = m_stream.ReadVInt(); i > 0; i--)
			{
				m_avatarStatusList.Add(m_stream.ReadVInt());
				m_avatarIdList.Add(m_stream.ReadLong());
			}

			if (!m_stream.IsAtEnd())
			{
				m_listType = m_stream.ReadVInt();
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteVInt(m_avatarIdList.Size());

			for (int i = 0; i < m_avatarIdList.Size(); i++)
			{
				m_stream.WriteVInt(m_avatarStatusList[i]);
				m_stream.WriteLong(m_avatarIdList[i]);
			}

			m_stream.WriteVInt(m_listType);
		}

		public override short GetMessageType()
			=> AvatarOnlineStatusListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarIdList = null;
		}

		public LogicArrayList<int> GetAvatarStatus()
			=> m_avatarStatusList;

		public void SetAvatarStatusList(LogicArrayList<int> value)
		{
			m_avatarStatusList = value;
		}

		public LogicArrayList<LogicLong> GetAvatarIdList()
			=> m_avatarIdList;

		public void SetAvatarIdList(LogicArrayList<LogicLong> value)
		{
			m_avatarIdList = value;
		}

		public int GetListType()
			=> m_listType;

		public void SetListType(int value)
		{
			m_listType = value;
		}
	}
}