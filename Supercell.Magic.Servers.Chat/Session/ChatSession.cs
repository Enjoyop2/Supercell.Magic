using Supercell.Magic.Servers.Chat.Logic;
using Supercell.Magic.Servers.Chat.Session.Message;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Session;

namespace Supercell.Magic.Servers.Chat.Session
{
	public class ChatSession : ServerSession
	{
		public LogicMessageManager LogicMessageManager
		{
			get;
		}
		public ChatInstance ChatInstance
		{
			get; private set;
		}

		public ChatSession(StartServerSessionMessage message) : base(message)
		{
			LogicMessageManager = new LogicMessageManager(this);
			ChatInstance = ChatInstanceManager.GetJoinableInstance(Country);
			ChatInstance.Add(this);
		}

		public override void Destruct()
		{
			ChatInstance.Remove(this);
			ChatInstance = null;
			base.Destruct();
		}
	}
}