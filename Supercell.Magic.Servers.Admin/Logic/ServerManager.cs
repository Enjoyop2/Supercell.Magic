using System.Diagnostics;
using System.Threading;

using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Core;
using Supercell.Magic.Servers.Core.Settings;
using Supercell.Magic.Servers.Core.Util;

namespace Supercell.Magic.Servers.Admin.Logic
{
	public static class ServerManager
	{
		private static ServerPerformance[][] m_entry;
		private static Thread m_thread;

		public static int GetOnlineUsers()
		{
			int count = 0;
			ServerPerformance[] proxies = ServerManager.m_entry[1];

			for (int i = 0; i < proxies.Length; i++)
			{
				count += proxies[i].SessionCount;
			}

			return count;
		}

		public static int GetAveragePings()
		{
			int totalServers = 0;
			int sumPing = 0;

			for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				ServerPerformance[] serverPerformances = ServerManager.m_entry[i];

				for (int j = 0; j < serverPerformances.Length; j++)
				{
					if (serverPerformances[j].Status == ServerPerformanceStatus.ONLINE)
					{
						totalServers += 1;
						sumPing = serverPerformances[j].Ping;
					}
				}
			}

			return totalServers > 0 ? sumPing / totalServers : 0;
		}

		public static void Init()
		{
			ServerManager.m_entry = new ServerPerformance[EnvironmentSettings.SERVER_TYPE_COUNT][];

			for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				ServerPerformance[] entryArray = new ServerPerformance[Core.Network.ServerManager.GetServerCount(i)];

				for (int j = 0; j < entryArray.Length; j++)
				{
					entryArray[j] = new ServerPerformance(Core.Network.ServerManager.GetSocket(i, j));
				}

				ServerManager.m_entry[i] = entryArray;
			}

			ServerManager.m_thread = new Thread(ServerManager.Update);
			ServerManager.m_thread.Start();
		}

		private static void Update()
		{
			Thread.Sleep(5000);

			int counter = 0;

			while (true)
			{
				if (ServerStatus.Status == ServerStatusType.SHUTDOWN_STARTED && ServerStatus.Time - TimeUtil.GetTimestamp() < 0)
					ServerStatus.SetStatus(ServerStatusType.MAINTENANCE, ServerStatus.NextTime + TimeUtil.GetTimestamp(), 0);
				if (ServerStatus.Status == ServerStatusType.COOLDOWN_AFTER_MAINTENANCE && ServerStatus.Time - TimeUtil.GetTimestamp() < 0)
					ServerStatus.SetStatus(ServerStatusType.NORMAL, 0, 0);

				if (counter++ % 20 == 0)
				{
					for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
					{
						ServerPerformance[] entryArray = ServerManager.m_entry[i];

						for (int j = 0; j < entryArray.Length; j++)
						{
							entryArray[j].SendPingMessage();
						}
					}

					for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
					{
						ServerPerformance[] entryArray = ServerManager.m_entry[i];

						for (int j = 0; j < entryArray.Length; j++)
						{
							ServerMessageManager.SendMessage(new ServerPerformanceMessage(), entryArray[j].Socket);
						}
					}

					if (counter == 20)
						counter = 1;
				}

				Thread.Sleep(500);
			}
		}

		public static void OnPongMessageReceived(PongMessage message)
		{
			ServerManager.m_entry[message.SenderType][message.SenderId].OnPongMessageReceived();
		}

		public static void OnServerPerformanceDataMessageReceived(ServerPerformanceDataMessage message)
		{
			ServerManager.m_entry[message.SenderType][message.SenderId].OnServerPerformanceDataMessageReceived(message);
		}

		public static void OnClusterPerformanceDataMessageReceived(ClusterPerformanceDataMessage message)
		{
			ServerManager.m_entry[message.SenderType][message.SenderId].OnClusterPerformanceDataMessageReceived(message);
		}

		public static JObject Save()
		{
			JObject jObject = new JObject();
			JArray servers = new JArray();

			for (int i = 0; i < EnvironmentSettings.SERVER_TYPE_COUNT; i++)
			{
				ServerPerformance[] entryArray = ServerManager.m_entry[i];

				for (int j = 0; j < entryArray.Length; j++)
				{
					servers.Add(entryArray[j].Save());
				}
			}

			jObject.Add("slots", servers);
			jObject.Add("status", new JObject
			{
				{"type", (int) ServerStatus.Status},
				{"arg", ServerStatus.Time},
			});

			return jObject;
		}
	}

	public class ServerPerformance
	{
		public ServerPerformanceStatus Status
		{
			get; private set;
		}

		public ServerSocket Socket
		{
			get;
		}
		public ClusterPerformance[] ClusterPerformances
		{
			get; private set;
		}

		public int Ping
		{
			get; private set;
		}
		public int SessionCount
		{
			get; private set;
		}

		private readonly Stopwatch m_watch;

		public ServerPerformance(ServerSocket socket)
		{
			Socket = socket;
			Status = ServerPerformanceStatus.UNKNOWN;
			m_watch = new Stopwatch();
		}

		public void SendPingMessage()
		{
			if (m_watch.IsRunning)
				Status = ServerPerformanceStatus.OFFLINE;

			ServerMessageManager.SendMessage(new PingMessage(), Socket);
			m_watch.Restart();
		}

		public void OnPongMessageReceived()
		{
			m_watch.Stop();
			Ping = (int)m_watch.ElapsedMilliseconds;
			Status = ServerPerformanceStatus.ONLINE;
		}

		public void OnServerPerformanceDataMessageReceived(ServerPerformanceDataMessage message)
		{
			SessionCount = message.SessionCount;

			if (ClusterPerformances == null ? message.ClusterCount != 0 : ClusterPerformances.Length != message.ClusterCount)
			{
				ClusterPerformances = new ClusterPerformance[message.ClusterCount];

				for (int i = 0; i < ClusterPerformances.Length; i++)
				{
					ClusterPerformances[i] = new ClusterPerformance();
				}
			}
		}

		public void OnClusterPerformanceDataMessageReceived(ClusterPerformanceDataMessage message)
		{
			ClusterPerformances[message.Id].Ping = message.Ping;
			ClusterPerformances[message.Id].SessionCount = message.SessionCount;
			ClusterPerformances[message.Id].Defined = true;
		}

		public JObject Save()
		{
			JObject jObject = new JObject();

			jObject.Add("name", string.Format("{0} - {1}", ServerUtil.GetServerName(Socket.ServerType), Socket.ServerId + 1));
			jObject.Add("status", Status.ToString());
			jObject.Add("ping", Ping);

			if (ClusterPerformances != null)
			{
				JArray clusters = new JArray();

				for (int i = 0; i < ClusterPerformances.Length; i++)
				{
					if (ClusterPerformances[i].Defined)
						clusters.Add(ClusterPerformances[i].Save());
				}

				jObject.Add("clusters", clusters);
			}

			return jObject;
		}
	}

	public class ClusterPerformance
	{
		public int Ping
		{
			get; set;
		}
		public int SessionCount
		{
			get; set;
		}
		public bool Defined
		{
			get; set;
		}

		public JObject Save()
		{
			JObject jobject = new JObject();

			jobject.Add("ping", Ping);
			jobject.Add("sessionCount", SessionCount);

			return jobject;
		}
	}

	public enum ServerPerformanceStatus
	{
		UNKNOWN,
		OFFLINE,
		ONLINE
	}
}