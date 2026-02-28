using Supercell.Magic.Logic;
using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Message.Avatar;
using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Logic.Message.Home;
using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Request.Stream;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Core.Util;
using Supercell.Magic.Servers.Game.Logic;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Servers.Game.Session.Message
{
	public class LogicMessageManager
	{
		private readonly GameSession m_session;

		public LogicMessageManager(GameSession session)
		{
			m_session = session;
		}

		public void ReceiveMessage(PiranhaMessage message)
		{
			switch (message.GetMessageType())
			{
				case ChangeAvatarNameMessage.MESSAGE_TYPE:
					OnChangeAvatarNameMessageReceived((ChangeAvatarNameMessage)message);
					break;
				case AvatarNameCheckRequestMessage.MESSAGE_TYPE:
					OnAvatarNameCheckRequestMessageReceived((AvatarNameCheckRequestMessage)message);
					break;
				case CancelMatchmakingMessage.MESSAGE_TYPE:
					OnCancelMatchmakingMessageReceived((CancelMatchmakingMessage)message);
					break;
				case HomeBattleReplayMessage.MESSAGE_TYPE:
					OnHomeBattleReplayMessageReceived((HomeBattleReplayMessage)message);
					break;
				case AskForAllianceBookmarksFullDataMessage.MESSAGE_TYPE:
					OnAskForAllianceBookmarksFullDataMessageReceived((AskForAllianceBookmarksFullDataMessage)message);
					break;
				case AddAllianceBookmarkMessage.MESSAGE_TYPE:
					OnAddAllianceBookmarkMessageReceived((AddAllianceBookmarkMessage)message);
					break;
				case RemoveAllianceBookmarkMessage.MESSAGE_TYPE:
					OnRemoveAllianceBookmarkMessageReceived((RemoveAllianceBookmarkMessage)message);
					break;
				case RemoveAvatarStreamEntryMessage.MESSAGE_TYPE:
					OnRemoveAvatarStreamEntryMessageReceived((RemoveAvatarStreamEntryMessage)message);
					break;
			}
		}

		private void OnChangeAvatarNameMessageReceived(ChangeAvatarNameMessage message)
		{
			if (message.GetNameSetByUser())
			{
				string name = message.RemoveAvatarName();

				if (name == null)
					return;

				name = StringUtil.RemoveMultipleSpaces(name.Trim());

				if (name.Length < 2)
				{
					AvatarNameChangeFailedMessage avatarNameChangeFailedMessage = new AvatarNameChangeFailedMessage();
					avatarNameChangeFailedMessage.SetErrorCode(AvatarNameChangeFailedMessage.ErrorCode.TOO_SHORT);
					m_session.SendPiranhaMessage(avatarNameChangeFailedMessage, 1);
					return;
				}

				if (name.Length > 16)
				{
					AvatarNameChangeFailedMessage avatarNameChangeFailedMessage = new AvatarNameChangeFailedMessage();
					avatarNameChangeFailedMessage.SetErrorCode(AvatarNameChangeFailedMessage.ErrorCode.TOO_LONG);
					m_session.SendPiranhaMessage(avatarNameChangeFailedMessage, 1);
					return;
				}

				if (WordCensorUtil.IsValidMessage(name))
				{
					LogicClientAvatar logicClientAvatar = m_session.GameAvatar.LogicClientAvatar;

					if (logicClientAvatar.GetNameChangeState() >= 1)
					{
						AvatarNameChangeFailedMessage avatarNameChangeFailedMessage = new AvatarNameChangeFailedMessage();
						avatarNameChangeFailedMessage.SetErrorCode(AvatarNameChangeFailedMessage.ErrorCode.ALREADY_CHANGED);
						m_session.SendPiranhaMessage(avatarNameChangeFailedMessage, 1);
						return;
					}

					if (logicClientAvatar.GetNameChangeState() == 0 && logicClientAvatar.GetTownHallLevel() < LogicDataTables.GetGlobals().GetEnableNameChangeTownHallLevel())
					{
						AvatarNameChangeFailedMessage avatarNameChangeFailedMessage = new AvatarNameChangeFailedMessage();
						avatarNameChangeFailedMessage.SetErrorCode(AvatarNameChangeFailedMessage.ErrorCode.TH_LEVEL_TOO_LOW);
						m_session.SendPiranhaMessage(avatarNameChangeFailedMessage, 1);
						return;
					}

					LogicChangeAvatarNameCommand serverCommand = new LogicChangeAvatarNameCommand();

					serverCommand.SetAvatarName(name);
					serverCommand.SetAvatarNameChangeState(logicClientAvatar.GetNameChangeState() + 1);

					m_session.GameAvatar.LogicClientAvatar.SetName(name);
					m_session.GameAvatar.LogicClientAvatar.SetNameChangeState(logicClientAvatar.GetNameChangeState() + 1);
					m_session.GameAvatar.AddServerCommand(serverCommand);
				}
				else
				{
					AvatarNameChangeFailedMessage avatarNameChangeFailedMessage = new AvatarNameChangeFailedMessage();
					avatarNameChangeFailedMessage.SetErrorCode(AvatarNameChangeFailedMessage.ErrorCode.BAD_WORD);
					m_session.SendPiranhaMessage(avatarNameChangeFailedMessage, 1);
				}
			}
		}

		private void OnAvatarNameCheckRequestMessageReceived(AvatarNameCheckRequestMessage message)
		{
			string name = message.GetName();

			if (name == null)
				return;

			name = StringUtil.RemoveMultipleSpaces(name.Trim());

			if (name.Length < 2)
			{
				AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();

				avatarNameCheckResponseMessage.SetName(message.GetName());
				avatarNameCheckResponseMessage.SetInvalid(true);
				avatarNameCheckResponseMessage.SetErrorCode(AvatarNameCheckResponseMessage.ErrorCode.NAME_TOO_SHORT);

				m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);
				return;
			}

			if (name.Length > 16)
			{
				AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();

				avatarNameCheckResponseMessage.SetName(message.GetName());
				avatarNameCheckResponseMessage.SetInvalid(true);
				avatarNameCheckResponseMessage.SetErrorCode(AvatarNameCheckResponseMessage.ErrorCode.NAME_TOO_LONG);

				m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);
				return;
			}

			if (WordCensorUtil.IsValidMessage(name))
			{
				LogicClientAvatar logicClientAvatar = m_session.GameAvatar.LogicClientAvatar;

				if (logicClientAvatar.GetNameChangeState() >= 1)
				{
					AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();

					avatarNameCheckResponseMessage.SetName(message.GetName());
					avatarNameCheckResponseMessage.SetInvalid(true);
					avatarNameCheckResponseMessage.SetErrorCode(AvatarNameCheckResponseMessage.ErrorCode.NAME_ALREADY_CHANGED);

					m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);
					return;
				}

				if (logicClientAvatar.GetNameChangeState() == 0 && logicClientAvatar.GetTownHallLevel() < LogicDataTables.GetGlobals().GetEnableNameChangeTownHallLevel())
				{
					AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();

					avatarNameCheckResponseMessage.SetName(message.GetName());
					avatarNameCheckResponseMessage.SetInvalid(true);
					avatarNameCheckResponseMessage.SetErrorCode(AvatarNameCheckResponseMessage.ErrorCode.NAME_TH_LEVEL_TOO_LOW);

					m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);

				}
				else
				{
					AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();
					avatarNameCheckResponseMessage.SetName(message.GetName());
					m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);
				}
			}
			else
			{
				AvatarNameCheckResponseMessage avatarNameCheckResponseMessage = new AvatarNameCheckResponseMessage();

				avatarNameCheckResponseMessage.SetName(message.GetName());
				avatarNameCheckResponseMessage.SetInvalid(true);
				avatarNameCheckResponseMessage.SetErrorCode(AvatarNameCheckResponseMessage.ErrorCode.INVALID_NAME);

				m_session.SendPiranhaMessage(avatarNameCheckResponseMessage, 1);
			}
		}

		private void OnCancelMatchmakingMessageReceived(CancelMatchmakingMessage message)
		{
			if (m_session.InDuelMatchmaking)
			{
				GameDuelMatchmakingManager.Dequeue(m_session);

				if (!m_session.InDuelMatchmaking)
				{
					m_session.LoadGameState(new GameHomeState
					{
						PlayerAvatar = m_session.GameAvatar.LogicClientAvatar,
						Home = m_session.GameAvatar.LogicClientHome,
						SaveTime = m_session.GameAvatar.SaveTime,
						MaintenanceTime = m_session.GameAvatar.MaintenanceTime,
						ServerCommands = m_session.GameAvatar.ServerCommands
					});
				}
			}
		}

		private void OnHomeBattleReplayMessageReceived(HomeBattleReplayMessage message)
		{
			ServerRequestManager.Create(new LoadReplayStreamRequestMessage
			{
				Id = message.GetReplayId()
			}, ServerManager.GetDocumentSocket(11, m_session.AccountId), 5).OnComplete = OnHomeBattleReplayLoaded;
		}

		private void OnHomeBattleReplayLoaded(ServerRequestArgs args)
		{
			if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success && !m_session.IsDestructed())
			{
				LoadReplayStreamResponseMessage loadReplayStreamResponseMessage = (LoadReplayStreamResponseMessage)args.ResponseMessage;

				if (loadReplayStreamResponseMessage.MajorVersion == LogicVersion.MAJOR_VERSION && loadReplayStreamResponseMessage.BuildVersion == LogicVersion.BUILD_VERSION &&
					loadReplayStreamResponseMessage.ContentVersion == ResourceManager.GetContentVersion())
				{
					HomeBattleReplayDataMessage homeBattleReplayDataMessage = new HomeBattleReplayDataMessage();
					homeBattleReplayDataMessage.SetReplayData(loadReplayStreamResponseMessage.StreamData);
					m_session.SendPiranhaMessage(homeBattleReplayDataMessage, 1);
				}
				else
				{
					m_session.SendPiranhaMessage(new HomeBattleReplayFailedMessage(), 1);
				}
			}
			else
			{
				m_session.SendPiranhaMessage(new HomeBattleReplayFailedMessage(), 1);
			}
		}

		private void OnAskForAllianceBookmarksFullDataMessageReceived(AskForAllianceBookmarksFullDataMessage message)
		{
			ServerMessageManager.SendMessage(new SendAllianceBookmarksFullDataToClientMessage
			{
				SessionId = m_session.Id,
				AllianceIds = m_session.GameAvatar.AllianceBookmarksList
			}, ServerManager.GetNextSocket(29));
		}

		private void OnAddAllianceBookmarkMessageReceived(AddAllianceBookmarkMessage message)
		{
			if (m_session.GameAvatar.AllianceBookmarksList.Size() < LogicDataTables.GetGlobals().GetBookmarksMaxAlliances())
				m_session.GameAvatar.AddAllianceBookmark(message.GetAllianceId());
		}

		private void OnRemoveAllianceBookmarkMessageReceived(RemoveAllianceBookmarkMessage message)
		{
			m_session.GameAvatar.RemoveAllianceBookmark(message.GetAllianceId());
		}

		private void OnRemoveAvatarStreamEntryMessageReceived(RemoveAvatarStreamEntryMessage message)
		{
			m_session.GameAvatar.RemoveAvatarStreamEntry(message.GetStreamEntryId());
		}
	}
}