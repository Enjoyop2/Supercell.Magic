using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class VisitedHomeDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24113;

		private int m_currentTimestamp;
		private int m_secondsSinceLastSave;

		private LogicClientAvatar m_visitorLogicClientAvatar;
		private LogicClientAvatar m_ownerLogicClientAvatar;
		private LogicClientHome m_logicClientHome;

		public VisitedHomeDataMessage() : this(0)
		{
			// VisitedHomeDataMessage.
		}

		public VisitedHomeDataMessage(short messageVersion) : base(messageVersion)
		{
			// VisitedHomeDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_secondsSinceLastSave = m_stream.ReadInt();
			m_currentTimestamp = m_stream.ReadInt();

			m_logicClientHome = new LogicClientHome();
			m_logicClientHome.Decode(m_stream);

			m_ownerLogicClientAvatar = new LogicClientAvatar();
			m_ownerLogicClientAvatar.Decode(m_stream);

			m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_visitorLogicClientAvatar = new LogicClientAvatar();
				m_visitorLogicClientAvatar.Decode(m_stream);
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_secondsSinceLastSave);
			m_stream.WriteInt(m_currentTimestamp);

			m_logicClientHome.Encode(m_stream);
			m_ownerLogicClientAvatar.Encode(m_stream);
			m_stream.WriteInt(0);

			if (m_visitorLogicClientAvatar != null)
			{
				m_stream.WriteBoolean(true);
				m_visitorLogicClientAvatar.Encode(m_stream);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> VisitedHomeDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();

			m_logicClientHome = null;
			m_ownerLogicClientAvatar = null;
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

		public LogicClientAvatar RemoveVisitorLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_visitorLogicClientAvatar;
			m_visitorLogicClientAvatar = null;
			return tmp;
		}

		public void SetVisitorLogicClientAvatar(LogicClientAvatar logicClientAvatar)
		{
			m_visitorLogicClientAvatar = logicClientAvatar;
		}

		public LogicClientAvatar RemoveOwnerLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_ownerLogicClientAvatar;
			m_ownerLogicClientAvatar = null;
			return tmp;
		}

		public void SetOwnerLogicClientAvatar(LogicClientAvatar logicClientAvatar)
		{
			m_ownerLogicClientAvatar = logicClientAvatar;
		}
	}
}