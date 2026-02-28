using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class FriendAvatarBaseMessage : PiranhaMessage
	{
		private int m_avatarIdHigh;
		private int m_avatarIdLow;

		public FriendAvatarBaseMessage() : this(0)
		{
			// FriendAvatarBaseMessage.
		}

		public FriendAvatarBaseMessage(short messageVersion) : base(messageVersion)
		{
			// FriendAvatarBaseMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_avatarIdHigh = m_stream.ReadInt();
			m_avatarIdLow = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_avatarIdHigh);
			m_stream.WriteInt(m_avatarIdLow);
		}

		public override int GetServiceNodeType()
			=> 3;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong GetAvatarId()
			=> new LogicLong(m_avatarIdHigh, m_avatarIdLow);

		public void SetAvatarId(LogicLong avatarId)
		{
			m_avatarIdHigh = avatarId.GetHigherInt();
			m_avatarIdLow = avatarId.GetLowerInt();
		}

		public void SetAvatarId(int high, int low)
		{
			m_avatarIdHigh = high;
			m_avatarIdLow = low;
		}
	}
}