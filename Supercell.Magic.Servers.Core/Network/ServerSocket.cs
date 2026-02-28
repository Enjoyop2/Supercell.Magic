using System.Threading;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

namespace Supercell.Magic.Servers.Core.Network
{
	public class ServerSocket
	{
		private NetMQSocket m_socket;

		public int ServerType
		{
			get;
		}
		public int ServerId
		{
			get;
		}

		public ServerSocket(int type, int id, string host, int port)
		{
			ServerType = type;
			ServerId = id;

			m_socket = new PushSocket(string.Format(">tcp://{0}:{1}", host, port));
			m_socket.Options.SendHighWatermark = 10000;
		}

		public void Destruct()
		{
			if (m_socket != null)
			{
				m_socket.Dispose();
				m_socket = null;
			}
		}

		public void Send(byte[] buffer)
		{
			m_socket.SendFrame(buffer);
		}

		public override string ToString()
			=> ServerType + "-" + ServerId;
	}
}