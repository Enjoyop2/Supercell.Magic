using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class OwnHomeDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24101;

		private int m_secondsSinceLastMaintenance;
		private int m_currentTimestamp;
		private int m_secondsSinceLastSave;
		private int m_reengagementSeconds;

		private int m_mapId;
		private int m_layoutId;

		private LogicClientAvatar m_logicClientAvatar;
		private LogicClientHome m_logicClientHome;

		public OwnHomeDataMessage() : this(0)
		{
			// OwnHomeDataMessage.
		}

		public OwnHomeDataMessage(short messageVersion) : base(messageVersion)
		{
			// OwnHomeDataMessage.
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

			m_mapId = m_stream.ReadInt();
			m_layoutId = m_stream.ReadInt();

			/* sub_36BCBC - START */

			m_stream.ReadInt();
			m_stream.ReadInt();

			m_stream.ReadInt();
			m_stream.ReadInt();

			m_stream.ReadInt();
			m_stream.ReadInt();

			/* sub_36BCBC - END */

			m_reengagementSeconds = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_secondsSinceLastSave);
			m_stream.WriteInt(m_secondsSinceLastMaintenance);
			m_stream.WriteInt(m_currentTimestamp);

			m_logicClientHome.Encode(m_stream);
			m_logicClientAvatar.Encode(m_stream);

			m_stream.WriteInt(m_mapId);
			m_stream.WriteInt(m_layoutId);

			m_stream.WriteInt(352);
			m_stream.WriteInt(1190797808);

			m_stream.WriteInt(352);
			m_stream.WriteInt(1192597808);

			m_stream.WriteInt(352);
			m_stream.WriteInt(1192597808);

			m_stream.WriteInt(m_reengagementSeconds);
		}

		public override short GetMessageType()
			=> OwnHomeDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

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

		public int GetSecondsSinceLastSave()
			=> m_secondsSinceLastSave;

		public void SetSecondsSinceLastSave(int value)
		{
			m_secondsSinceLastSave = value;
		}

		public int GetSecondsSinceLastMaintenance()
			=> m_secondsSinceLastMaintenance;

		public void SetSecondsSinceLastMaintenance(int value)
		{
			m_secondsSinceLastMaintenance = value;
		}

		public int GetReengagementSeconds()
			=> m_reengagementSeconds;

		public void SetReengagementSeconds(int value)
		{
			m_reengagementSeconds = value;
		}

		public int GetMapId()
			=> m_mapId;

		public void SetMapId(int value)
		{
			m_mapId = value;
		}

		public int GetLayoutId()
			=> m_layoutId;

		public void SetLayoutId(int value)
		{
			m_layoutId = value;
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