using System.Net;
using System.Net.Sockets;

using MaxMind.GeoIP2.Responses;

using Supercell.Magic.Servers.Proxy.Network.Message;
using Supercell.Magic.Servers.Proxy.Session;

namespace Supercell.Magic.Servers.Proxy.Network
{
	public class ClientConnection
	{
		private readonly SocketBuffer m_receiveBuffer;
		private readonly SocketAsyncEventArgs m_receiveAsyncEventArgs;

		public Socket Socket
		{
			get;
		}
		public Messaging Messaging
		{
			get;
		}
		public MessageManager MessageManager
		{
			get;
		}
		public ProxySession Session
		{
			get; private set;
		}

		public string Location
		{
			get;
		}

		public bool Destructed
		{
			get; private set;
		}
		public long Id
		{
			get; private set;
		}
		public ClientConnectionState State
		{
			get; private set;
		}

		public IPAddress ClientIP
		{
			get
			{
				return ((IPEndPoint)Socket.RemoteEndPoint).Address;
			}
		}

		public ClientConnection(Socket socket, SocketAsyncEventArgs receiveAsyncEventArgs, long id)
		{
			Id = id;
			Socket = socket;
			m_receiveAsyncEventArgs = receiveAsyncEventArgs;
			m_receiveBuffer = new SocketBuffer(4096);
			Messaging = new Messaging(this);
			MessageManager = new MessageManager(this);
			State = ClientConnectionState.DEFAULT;
			Location = ServerProxy.MaxMind.TryCountry(ClientIP, out CountryResponse response)
				? response.Country.IsoCode
				: "LO";
		}

		public void Destruct()
		{
			if (!Destructed)
			{
				Destructed = true;
				Id = -1;
				Socket.Close(5);
				m_receiveAsyncEventArgs.Dispose();
				State = ClientConnectionState.DISCONNECTED;
				DestructSession();
			}
		}

		public void DestructSession()
		{
			if (Session != null)
			{
				ProxySessionManager.Remove(Session);

				Session.Destruct();
				Session = null;
			}
		}

		public void SetState(ClientConnectionState state)
		{
			if (State != ClientConnectionState.DISCONNECTED)
			{
				State = state;
			}
		}

		public void SetSession(ProxySession session)
		{
			Session = session;
		}

		public void ReceiveData()
		{
			if (!Destructed)
			{
				m_receiveBuffer.Write(m_receiveAsyncEventArgs.Buffer, m_receiveAsyncEventArgs.BytesTransferred);

				int length = m_receiveBuffer.Size();

				do
				{
					int read = Messaging.OnReceive(m_receiveBuffer.GetBuffer(), length);

					if (read <= 0)
					{
						if (read == -1)
							TcpServerSocket.Disconnect(this);
						break;
					}

					m_receiveBuffer.Remove(read);
				} while ((length = m_receiveBuffer.Size()) > 0);
			}
		}

		public void Send(byte[] buffer, int length)
		{
			if (!Destructed)
			{
				TcpServerSocket.Send(this, buffer, length);
			}
		}
	}

	public enum ClientConnectionState
	{
		DEFAULT,
		LOGIN,
		LOGGED,
		LOGIN_FAILED,
		DISCONNECTED,
	}
}