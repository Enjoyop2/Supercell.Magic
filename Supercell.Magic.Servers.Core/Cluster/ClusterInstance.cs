using System.Diagnostics;
using System.Threading;

using NetMQ;
using NetMQ.Sockets;

using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;

namespace Supercell.Magic.Servers.Core.Cluster
{
	public class ClusterInstance
	{
		private bool m_started;

		protected readonly int m_id;
		private readonly int m_logicUpdateFrequency;

		private readonly Thread m_networkThread;
		private readonly Thread m_logicThread;
		private readonly PushSocket m_pushSocket;
		private readonly PullSocket m_pullSocket;
		private readonly Stopwatch m_pingWatch;

		protected int m_ping;

		public ClusterInstance(int id, int logicUpdateFrequency = -1)
		{
			m_started = true;
			m_id = id;
			m_pullSocket = new PullSocket();
			m_pullSocket.Bind(GetConnectionString());
			m_pushSocket = new PushSocket();
			m_pushSocket.Connect(GetConnectionString());
			m_networkThread = new Thread(NetworkUpdate);
			m_networkThread.Name = string.Format("Cluster #{0}: Network Thread", m_id);

			if (logicUpdateFrequency >= 0)
			{
				m_logicUpdateFrequency = logicUpdateFrequency;
				m_logicThread = new Thread(LogicUpdate);
				m_logicThread.Name = string.Format("Cluster #{0}: Logic Thread", m_id);
				m_logicThread.Start();
			}

			m_networkThread.Start();
			m_pingWatch = new Stopwatch();
		}

		public void Stop()
		{
			m_started = false;
		}

		private void NetworkUpdate()
		{
			while (m_started)
			{
				NetMQMessage message = m_pullSocket.ReceiveMultipartMessage();

				while (!message.IsEmpty)
				{
					OnReceive(message.Pop().Buffer);
				}
			}
		}

		private void LogicUpdate()
		{
			while (m_started)
			{
				Thread.Sleep(m_logicUpdateFrequency);
				Tick();
			}
		}

		private void OnReceive(byte[] buffer)
		{
			if (buffer.Length > 0)
			{
				ServerMessage message = ServerMessaging.ReadMessage(buffer, buffer.Length);

				if (message != null)
				{
					ReceiveMessage(message);
				}
			}
			else
			{
				m_pingWatch.Stop();
				m_ping = (int)m_pingWatch.ElapsedMilliseconds;
				OnPingTestCompleted();
			}
		}

		private string GetConnectionString()
			=> "inproc://cluster-" + m_id;

		protected virtual void ReceiveMessage(ServerMessage message)
		{
		}

		protected virtual void Tick()
		{
		}

		protected virtual void OnPingTestCompleted()
		{
		}

		public void SendMessage(ServerMessage message)
		{
			m_pushSocket.SendFrame(ServerMessaging.WriteMessage(message));
		}

		public void SendPing()
		{
			m_pingWatch.Start();
			m_pushSocket.SendFrame(new byte[0]);
		}
	}
}