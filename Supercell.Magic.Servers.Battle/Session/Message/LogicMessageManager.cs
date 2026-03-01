using Supercell.Magic.Logic.Message.Battle;
using Supercell.Magic.Servers.Battle.Logic.Mode;
using Supercell.Magic.Servers.Battle.Session;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Servers.Battle.Session.Message
{
	public class LogicMessageManager
	{
		private readonly BattleSession m_session;

		public LogicMessageManager(BattleSession session)
		{
			m_session = session;
		}

		public void ReceiveMessage(PiranhaMessage message)
		{
			switch (message.GetMessageType())
			{
				case BattleEndClientTurnMessage.MESSAGE_TYPE:
					OnBattleEndClientTurnMessageReceived((BattleEndClientTurnMessage)message);
					break;
			}
		}

		private void OnBattleEndClientTurnMessageReceived(BattleEndClientTurnMessage message)
		{
			GameMode gameMode = m_session.GameMode;

			if (gameMode != null)
			{

				gameMode.OnClientTurnReceived(message.GetSubTick(), message.GetChecksum(), message.GetCommands());
			}
		}
	}
}