using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarOnlineStatusUpdated : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20206;

		private LogicLong m_avatarId;
		private int m_state;

		public AvatarOnlineStatusUpdated() : this(0)
		{
			// AvatarOnlineStatusUpdated.
		}

		public AvatarOnlineStatusUpdated(short messageVersion) : base(messageVersion)
		{
			// AvatarOnlineStatusUpdated.
		}

		public override void Decode()
		{
			base.Decode();

			m_avatarId = m_stream.ReadLong();
			m_state = m_stream.ReadVInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_avatarId);
			m_stream.WriteVInt(m_state);
		}

		public override short GetMessageType()
			=> AvatarOnlineStatusUpdated.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarId = null;
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
		}

		public int GetState()
			=> m_state;

		public void SetState(int value)
		{
			m_state = value;
		}
	}
}