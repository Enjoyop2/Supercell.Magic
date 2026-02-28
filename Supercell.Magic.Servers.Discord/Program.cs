using global::Discord.WebSocket;

namespace Supercell.Magic.Servers.Discord
{
	internal class Program
	{
		private static DiscordSocketClient m_client;

		private static void Main(string[] args)
		{
			Program.m_client = new DiscordSocketClient();
		}
	}
}