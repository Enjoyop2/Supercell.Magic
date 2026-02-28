using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class AttackHomeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14106;

		private LogicLong m_homeId;
		private LogicLong m_avatarStreamId;

		private int m_attackSource;

		public AttackHomeMessage() : this(0)
		{
			// AttackHomeMessage.
		}

		public AttackHomeMessage(short messageVersion) : base(messageVersion)
		{
			// AttackHomeMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_homeId = m_stream.ReadLong();
			m_attackSource = m_stream.ReadInt();
			m_avatarStreamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_homeId);
			m_stream.WriteInt(m_attackSource);
			m_stream.WriteLong(m_avatarStreamId);
		}

		public override short GetMessageType()
			=> AttackHomeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public LogicLong GetAvatarStreamId()
			=> m_avatarStreamId;

		public void SetAvatarStreamId(LogicLong value)
		{
			m_avatarStreamId = value;
		}

		public int GetAttackSource()
			=> m_attackSource;

		public void SetAttackSource(int value)
		{
			m_attackSource = value;
		}
	}
}