using System.IO;
using System.Reflection;

using Supercell.Magic.Logic;
using Supercell.Magic.Logic.Message.Account;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Tools.Client.Network
{
	public class ServerConnection
	{
		private Messaging m_messaging;
		private MessageManager m_messageManager;
		private AccountInfo m_accountInfo;

		private ServerConnectionState m_state;

		public ServerConnection(LogicLong accountId = null)
		{
			LoadAccount(accountId);
			Connect();
		}

		private void LoadAccount(LogicLong id)
		{
			if (id != null)
			{
				string path = string.Format("accounts/{0}-{1}", id.GetHigherInt(), id.GetLowerInt());

				if (File.Exists(path))
				{
					LogicJSONObject jsonObject = LogicJSONParser.ParseObject(File.ReadAllText(path));

					m_accountInfo = new AccountInfo
					{
						AccountId = new LogicLong(jsonObject.GetJSONNumber("id_high").GetIntValue(), jsonObject.GetJSONNumber("id_low").GetIntValue()),
						PassToken = jsonObject.GetJSONString("passToken").GetStringValue()
					};
				}
				else
				{
					Debugger.Warning("ServerConnection.loadAccount: account doesn't exists!");
				}
			}
			else
			{
				m_accountInfo = new AccountInfo();
			}
		}

		public void Connect(string host = "127.0.0.1", int port = 9339)
		{
			if (m_messaging != null)
				m_messaging.Destruct();
			m_messaging = new Messaging();
			m_messaging.Connect(host, port);
			m_messageManager = new MessageManager(this, m_messaging);
			m_state = ServerConnectionState.CONNECT;
		}

		public void Update(float time)
		{
			switch (m_state)
			{
				case ServerConnectionState.CONNECT:
					if (m_messaging.IsConnected())
					{
						m_state = ServerConnectionState.LOGIN;

						LoginMessage loginMessage = new LoginMessage();

						loginMessage.SetAccountId(m_accountInfo.AccountId);
						loginMessage.SetPassToken(m_accountInfo.PassToken);
						loginMessage.SetClientMajorVersion(LogicVersion.MAJOR_VERSION);
						loginMessage.SetClientBuildVersion(LogicVersion.BUILD_VERSION);
						loginMessage.SetResourceSha(ResourceManager.FINGERPRINT_SHA);
						loginMessage.SetDevice(Assembly.GetExecutingAssembly().FullName);
						loginMessage.SetScramblerSeed(m_messaging.GetScramblerSeed());

						m_messaging.Send(loginMessage);
					}
					break;
				case ServerConnectionState.LOGIN:
				case ServerConnectionState.LOGGED:
					while (m_messaging.TryDequeueReceiveMessage(out PiranhaMessage piranhaMessage))
						m_messageManager.ReceiveMessage(piranhaMessage);
					m_messageManager.Update(time);
					m_messaging.SendQueue();

					break;
			}
		}

		public void SetAccountInfo(LogicLong accountId, string passToken)
		{
			m_accountInfo.AccountId = accountId;
			m_accountInfo.PassToken = passToken;

			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("id_high", new LogicJSONNumber(accountId.GetHigherInt()));
			jsonObject.Put("id_low", new LogicJSONNumber(accountId.GetLowerInt()));
			jsonObject.Put("passToken", new LogicJSONString(passToken));

			Directory.CreateDirectory("accounts");
			File.WriteAllText(string.Format("accounts/{0}-{1}", accountId.GetHigherInt(), accountId.GetLowerInt()), LogicJSONParser.CreateJSONString(jsonObject));
		}

		public void SetState(ServerConnectionState state)
		{
			m_state = state;
		}
	}

	public class AccountInfo
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public string PassToken
		{
			get; set;
		}

		public AccountInfo()
		{
			AccountId = new LogicLong();
		}
	}

	public enum ServerConnectionState
	{
		NULL,
		CONNECT,
		LOGIN,
		LOGIN_FAILED,
		LOGGED,
	}
}