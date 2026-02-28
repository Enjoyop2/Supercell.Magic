using Supercell.Magic.Logic.Home.Change;

namespace Supercell.Magic.Servers.Home.Logic.Mode.Listener
{
	public class HomeChangeListener : LogicHomeChangeListener
	{
		private readonly GameMode m_gameMode;

		public HomeChangeListener(GameMode gameMode)
		{
			m_gameMode = gameMode;
		}
	}
}