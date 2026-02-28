using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class FriendlyScoutHomeDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 25007;

		private int m_currentTimestamp;
		private int m_mapId;

		private LogicLong m_avatarId;
		private LogicLong m_accountId;
		private LogicLong m_streamId;

		private LogicClientAvatar m_logicClientAvatar;
		private LogicClientHome m_logicClientHome;

		public FriendlyScoutHomeDataMessage() : this(0)
		{
			// FriendlyScoutHomeDataMessage.
		}

		public FriendlyScoutHomeDataMessage(short messageVersion) : base(messageVersion)
		{
			// FriendlyScoutHomeDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_currentTimestamp = m_stream.ReadInt();
			m_avatarId = m_stream.ReadLong();
			m_accountId = m_stream.ReadLong();
			m_logicClientHome = new LogicClientHome();
			m_logicClientHome.Decode(m_stream);
			m_mapId = m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_logicClientAvatar = new LogicClientAvatar();
				m_logicClientAvatar.Decode(m_stream);
			}

			m_streamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_currentTimestamp);
			m_stream.WriteLong(m_avatarId);
			m_stream.WriteLong(m_accountId);
			m_logicClientHome.Encode(m_stream);
			m_stream.WriteInt(m_mapId);

			if (m_logicClientAvatar != null)
			{
				m_stream.WriteBoolean(true);
				m_logicClientAvatar.Encode(m_stream);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_stream.WriteLong(m_streamId);
		}

		public override short GetMessageType()
			=> FriendlyScoutHomeDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();

			m_logicClientHome = null;
			m_logicClientAvatar = null;
		}

		public int GetCurrentTimestamp()
			=> m_currentTimestamp;

		public void SetCurrentTimestamp(int value)
		{
			m_currentTimestamp = value;
		}

		public int GetMapId()
			=> m_mapId;

		public void SetMapId(int value)
		{
			m_mapId = value;
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong value)
		{
			m_streamId = value;
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
	}
}