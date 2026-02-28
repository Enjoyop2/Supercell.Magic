using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class EnemyHomeDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24107;

		private int m_secondsSinceLastMaintenance;
		private int m_currentTimestamp;
		private int m_secondsSinceLastSave;
		private int m_attackSource;
		private int m_mapId;

		private LogicClientAvatar m_attackerLogicClientAvatar;
		private LogicClientAvatar m_logicClientAvatar;
		private LogicClientHome m_logicClientHome;
		private LogicLong m_AvatarStreamEntryId;

		public EnemyHomeDataMessage() : this(0)
		{
			// EnemyHomeDataMessage.
		}

		public EnemyHomeDataMessage(short messageVersion) : base(messageVersion)
		{
			// EnemyHomeDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_secondsSinceLastSave = m_stream.ReadInt();
			m_secondsSinceLastMaintenance = m_stream.ReadInt();
			m_currentTimestamp = m_stream.ReadInt();

			m_logicClientHome = new LogicClientHome();
			m_logicClientHome.Decode(m_stream);
			m_logicClientAvatar = new LogicClientAvatar();
			m_logicClientAvatar.Decode(m_stream);
			m_attackerLogicClientAvatar = new LogicClientAvatar();
			m_attackerLogicClientAvatar.Decode(m_stream);

			m_attackSource = m_stream.ReadInt();
			m_mapId = m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_AvatarStreamEntryId = m_stream.ReadLong();
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_secondsSinceLastSave);
			m_stream.WriteInt(m_secondsSinceLastMaintenance);
			m_stream.WriteInt(m_currentTimestamp);

			m_logicClientHome.Encode(m_stream);
			m_logicClientAvatar.Encode(m_stream);
			m_attackerLogicClientAvatar.Encode(m_stream);
			m_stream.WriteInt(m_attackSource);
			m_stream.WriteInt(m_mapId);

			if (m_AvatarStreamEntryId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_AvatarStreamEntryId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> EnemyHomeDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();

			m_logicClientHome = null;
			m_logicClientAvatar = null;
			m_attackerLogicClientAvatar = null;
			m_AvatarStreamEntryId = null;
		}

		public int GetCurrentTimestamp()
			=> m_currentTimestamp;

		public void SetCurrentTimestamp(int value)
		{
			m_currentTimestamp = value;
		}

		public int GetSecondsSinceLastSave()
			=> m_secondsSinceLastSave;

		public void SetSecondsSinceLastSave(int value)
		{
			m_secondsSinceLastSave = value;
		}

		public int GetSecondsSinceLastMaintenance()
			=> m_secondsSinceLastMaintenance;

		public void SetAttackSource(int value)
		{
			m_attackSource = value;
		}

		public int GetAttackSource()
			=> m_attackSource;

		public void SetMapId(int value)
		{
			m_mapId = value;
		}

		public int GetMapId()
			=> m_mapId;

		public void SetSecondsSinceLastMaintenance(int value)
		{
			m_secondsSinceLastMaintenance = value;
		}

		public LogicClientHome RemoveLogicClientHome()
		{
			LogicClientHome tmp = m_logicClientHome;
			m_logicClientHome = null;
			return tmp;
		}

		public void SetLogicClientHome(LogicClientHome logicClientHome)
		{
			m_logicClientHome = logicClientHome;
		}

		public LogicClientAvatar RemoveLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_logicClientAvatar;
			m_logicClientAvatar = null;
			return tmp;
		}

		public void SetLogicClientAvatar(LogicClientAvatar logicClientAvatar)
		{
			m_logicClientAvatar = logicClientAvatar;
		}

		public LogicClientAvatar RemoveAttackerLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_attackerLogicClientAvatar;
			m_attackerLogicClientAvatar = null;
			return tmp;
		}

		public void SetAttackerLogicClientAvatar(LogicClientAvatar logicClientAvatar)
		{
			m_attackerLogicClientAvatar = logicClientAvatar;
		}

		public LogicLong GetAvatarStreamEntryId()
			=> m_AvatarStreamEntryId;

		public void SetAvatarStreamEntryId(LogicLong id)
		{
			m_AvatarStreamEntryId = id;
		}
	}
}