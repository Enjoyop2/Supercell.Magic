using System;

using Supercell.Magic.Servers.Battle.Logic.Mode;
using Supercell.Magic.Servers.Battle.Session.Message;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Session;

namespace Supercell.Magic.Servers.Battle.Session
{
	public class BattleSession : ServerSession
	{
		public LogicMessageManager LogicMessageManager
		{
			get;
		}
		public GameMode GameMode
		{
			get; private set;
		}

		public BattleSession(StartServerSessionMessage message) : base(message)
		{
			LogicMessageManager = new LogicMessageManager(this);
		}

		public override void Destruct()
		{
			if (GameMode != null)
			{
				GameMode.Destruct();
			}
			base.Destruct();
		}

		public void SetGameMode(GameMode gameMode)
		{
			if (GameMode != null)
			{
				GameMode.Destruct();
			}
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