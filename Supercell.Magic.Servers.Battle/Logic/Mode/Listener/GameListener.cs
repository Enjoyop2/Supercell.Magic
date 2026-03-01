using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Servers.Core.Network.Message.Session;

namespace Supercell.Magic.Servers.Battle.Logic.Mode.Listener
{
	public class GameListener : LogicGameListener
	{
		private readonly GameMode m_gameMode;

		public GameListener(GameMode gameMode)
		{
			m_gameMode = gameMode;
		}

		public override void MatchmakingCommandExecuted()
		{
			m_gameMode.GetSession().SendMessage(new GameMatchmakingMessage
			{
				MatchmakingType = GameMatchmakingMessage.GameMatchmakingType.DEFAULT
			}, 9);
			m_gameMode.SetShouldDestruct();
		}

		public override void MatchmakingVillage2CommandExecuted()
		{
			m_gameMode.GetSession().SendMessage(new GameMatchmakingMessage
			{
				MatchmakingType = GameMatchmakingMessage.GameMatchmakingType.DUEL
			}, 9);
			m_gameMode.SetShouldDestruct();
		}

		public override void NameChanged(string name)
		{
			m_gameMode.GetAvatarChangeListener().NameChanged(name, m_gameMode.GetPlayerAvatar().GetNameChangeState());
		}
	}
}