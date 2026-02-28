using System;

using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Session;

using Supercell.Magic.Servers.Home.Logic.Mode;
using Supercell.Magic.Servers.Home.Session.Message;

namespace Supercell.Magic.Servers.Home.Session
{
	public class HomeSession : ServerSession
	{
		public LogicMessageManager LogicMessageManager
		{
			get;
		}
		public GameMode GameMode
		{
			get; private set;
		}

		public HomeSession(StartServerSessionMessage message) : base(message)
		{
			LogicMessageManager = new LogicMessageManager(this);
		}

		public override void Destruct()
		{
			if (GameMode != null)
				GameMode.Destruct();
			base.Destruct();
		}

		public void SetGameMode(GameMode gameMode)
		{
			if (GameMode != null)
				GameMode.Destruct();
			GameMode = gameMode;
		}

		public void DestructGameMode()
		{
			if (GameMode != null)
			{
				GameMode.Destruct();
				GameMode = null;
			}
		}
	}
}