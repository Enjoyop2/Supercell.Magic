using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class NpcDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24133;

		private LogicClientHome m_clientHome;
		private LogicNpcAvatar m_npcAvatar;
		private LogicClientAvatar m_clientAvatar;

		private int m_secondsSinceLastSave;
		private int m_currentTimestamp;
		private bool m_npcDuel;

		public NpcDataMessage() : this(0)
		{
			// NpcDataMessage.
		}

		public NpcDataMessage(short messageVersion) : base(messageVersion)
		{
			// NpcDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_secondsSinceLastSave = m_stream.ReadInt();
			m_currentTimestamp = m_stream.ReadInt();
			m_clientHome = new LogicClientHome();
			m_clientHome.Decode(m_stream);
			m_clientAvatar = new LogicClientAvatar();
			m_clientAvatar.Decode(m_stream);
			m_npcAvatar = new LogicNpcAvatar();
			m_npcAvatar.Decode(m_stream);
			m_npcDuel = m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_secondsSinceLastSave);
			m_stream.WriteInt(m_currentTimestamp);
			m_clientHome.Encode(m_stream);
			m_clientAvatar.Encode(m_stream);
			m_npcAvatar.Encode(m_stream);
			m_stream.WriteBoolean(m_npcDuel);
		}

		public override short GetMessageType()
			=> NpcDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();

			m_clientHome = null;
			m_clientAvatar = null;
			m_npcAvatar = null;
		}

		public LogicClientHome RemoveLogicClientHome()
		{
			LogicClientHome tmp = m_clientHome;
			m_clientHome = null;
			return tmp;
		}

		public void SetLogicClientHome(LogicClientHome instance)
		{
			m_clientHome = instance;
		}

		public LogicClientAvatar RemoveLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_clientAvatar;
			m_clientAvatar = null;
			return tmp;
		}

		public void SetLogicClientAvatar(LogicClientAvatar instance)
		{
			m_clientAvatar = instance;
		}

		public LogicNpcAvatar RemoveLogicNpcAvatar()
		{
			LogicNpcAvatar tmp = m_npcAvatar;
			m_npcAvatar = null;
			return tmp;
		}

		public void SetLogicNpcAvatar(LogicNpcAvatar instance)
		{
			m_npcAvatar = instance;
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

		public bool IsNpcDuel()
			=> m_npcDuel;

		public void SetNpcDuel(bool npcDuel)
		{
			m_npcDuel = npcDuel;
		}
	}
}