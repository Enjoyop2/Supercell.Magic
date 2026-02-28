using System;
using System.Collections.Concurrent;
using System.Threading;

using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Core;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message
{
	internal class ServerMessageHandler
	{
		private readonly Thread m_sendThread;
		private readonly Thread m_receiveThread;
		private readonly ConcurrentQueue<QueueItem> m_sendQueue;
		private readonly ConcurrentQueue<ServerMessage> m_receiveQueue;
		private readonly AutoResetEvent m_sendResetEvent;
		private readonly AutoResetEvent m_receiveResetEvent;
		private readonly ServerMessageManager m_messageManager;

		private bool m_started;

		public ServerMessageHandler(ServerMessageManager manager)
		{
			m_started = true;

			m_sendQueue = new ConcurrentQueue<QueueItem>();
			m_receiveQueue = new ConcurrentQueue<ServerMessage>();
			m_sendResetEvent = new AutoResetEvent(false);
			m_receiveResetEvent = new AutoResetEvent(false);
			m_messageManager = manager;
			m_sendThread = new Thread(Send);
			m_sendThread.Start();
			m_receiveThread = new Thread(Receive);
			m_receiveThread.Start();
		}

		private void Receive()
		{
			while (m_started)
			{
				m_receiveResetEvent.WaitOne();

				while (m_receiveQueue.TryDequeue(out ServerMessage message))
				{
					try
					{
						switch (message.GetMessageCategory())
						{
							case ServerMessageCategory.ACCOUNT:
								m_messageManager.OnReceiveAccountMessage((ServerAccountMessage)message);
								break;
							case ServerMessageCategory.REQUEST:
								m_messageManager.OnReceiveRequestMessage((ServerRequestMessage)message);
								break;
							case ServerMessageCategory.SESSION:
								m_messageManager.OnReceiveSessionMessage((ServerSessionMessage)message);
								break;
							case ServerMessageCategory.RESPONSE:
								ServerRequestManager.ResponseReceived((ServerResponseMessage)message);
								break;
							case ServerMessageCategory.CORE:
								if (!ServerMessageManager.ReceiveCoreMessage((ServerCoreMessage)message))
									m_messageManager.OnReceiveCoreMessage((ServerCoreMessage)message);
								break;
							default:
								Logging.Error("ServerMessageHandler.receive: unknown message category: " + message.GetMessageCategory());
								break;
						}
					}
					catch (Exception exception)
					{
						Logging.Warning("ServerMessageHandler.receive: exception when the handle of message type " + message.GetMessageType() + ", trace: " + exception);
					}
				}
			}
		}

		private void Send()
		{
			while (m_started)
			{
				m_sendResetEvent.WaitOne();

				while (m_sendQueue.TryDequeue(out QueueItem item))
				{
					item.Socket.Send(ServerMessaging.WriteMessage(item.Message));
				}
			}
		}

		public void Enqueue(ServerMessage message)
		{
			m_receiveQueue.Enqueue(message);
			m_receiveResetEvent.Set();
		}

		public void Enqueue(ServerMessage message, ServerSocket socket)
		{
			m_sendQueue.Enqueue(new QueueItem(message, socket));
			m_sendResetEvent.Set();
		}

		public virtual void Destruct()
		{
			while (m_sendQueue.Count != 0 || m_receiveQueue.Count != 0)
			{
				Thread.Sleep(50);
			}

			m_started = false;
		}

		private struct QueueItem
		{
			internal ServerMessage Message
			{
				get;
			}
			internal ServerSocket Socket
			{
				get;
			}

			internal QueueItem(ServerMessage message, ServerSocket socket)
			{
				Message = message;
				Socket = socket;
			}
		}
	}
}