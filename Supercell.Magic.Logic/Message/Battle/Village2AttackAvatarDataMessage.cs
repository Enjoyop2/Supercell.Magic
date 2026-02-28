using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Battle
{
	public class Village2AttackAvatarDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 25023;

		private LogicClientAvatar m_logicClientAvatar;
		private LogicClientHome m_logicClientHome;

		private LogicLong m_enemyId;

		private int m_timestamp;

		public Village2AttackAvatarDataMessage() : this(0)
		{
			// Village2AttackAvatarDataMessage.
		}

		public Village2AttackAvatarDataMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackAvatarDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_logicClientAvatar = new LogicClientAvatar();
			m_logicClientAvatar.Decode(m_stream);
			m_logicClientHome = new LogicClientHome();
			m_logicClientHome.Decode(m_stream);
			m_enemyId = m_stream.ReadLong();
			m_timestamp = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_logicClientAvatar.Encode(m_stream);
			m_logicClientHome.Encode(m_stream);

			m_stream.WriteLong(new LogicLong(0, 1));
			m_stream.WriteInt(m_timestamp);
		}

		public override short GetMessageType()
			=> Village2AttackAvatarDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 27;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicClientAvatar GetLogicClientAvatar()
			=> m_logicClientAvatar;

		public void SetLogicClientAvatar(LogicClientAvatar value)
		{
			m_logicClientAvatar = value;
		}

		public LogicClientHome GetLogicClientHome()
			=> m_logicClientHome;

		public void SetLogicClientHome(LogicClientHome value)
		{
			m_logicClientHome = value;
		}

		public LogicLong GetEnemyId()
			=> m_enemyId;

		public void SetEnemyId(LogicLong value)
		{
			m_enemyId = value;
		}

		public int GetTimestamp()
			=> m_timestamp;

		public void SetTimestamp(int value)
		{
			m_timestamp = value;
		}
	}
}