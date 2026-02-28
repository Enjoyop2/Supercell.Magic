using Supercell.Magic.Servers.Home.Cluster;

namespace Supercell.Magic.Servers.Home
{
	public static class ServerHome
	{
		public static void Init()
		{
			GameModeClusterManager.Init();
		}
	}
}