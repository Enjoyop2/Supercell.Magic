using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AskForAvatarProfileMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14325;

		private LogicLong m_avatarId;
		private LogicLong m_homeId;

		public AskForAvatarProfileMessage() : this(0)
		{
			// AskForAvatarProfileMessage.
		}

		public AskForAvatarProfileMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAvatarProfileMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_avatarId = m_stream.ReadLong();

			if (m_stream.ReadBoolean())
			{
				m_homeId = m_stream.ReadLong();
			}

			m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_avatarId);

			if (m_homeId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_homeId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_stream.WriteBoolean(false);
		}

		public override short GetMessageType()
			=> AskForAvatarProfileMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();

			m_avatarId = null;
			m_homeId = null;
		}

		public LogicLong RemoveAvatarId()
		{
			LogicLong tmp = m_avatarId;
			m_avatarId = null;
			return tmp;
		}

		public void SetAvatarId(LogicLong id)
		{
			m_avatarId = id;
		}

		public LogicLong RemoveHomeId()
		{
			LogicLong tmp = m_homeId;
			m_homeId = null;
			return tmp;
		}

		public void SetHomeId(LogicLong id)
		{
			m_homeId = id;
		}
	}
}