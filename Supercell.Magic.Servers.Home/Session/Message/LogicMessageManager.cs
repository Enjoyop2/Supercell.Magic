using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Logic.Message.Home;

using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Request.Stream;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Home.Logic.Mode;
using Supercell.Magic.Servers.Home.Session;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Servers.Home.Session.Message
{
	public class LogicMessageManager
	{
		private readonly HomeSession m_session;

		public LogicMessageManager(HomeSession session)
		{
			m_session = session;
		}

		public void ReceiveMessage(PiranhaMessage message)
		{
			switch (message.GetMessageType())
			{
				case GoHomeMessage.MESSAGE_TYPE:
					OnGoHomeMessageReceived((GoHomeMessage)message);
					break;
				case EndClientTurnMessage.MESSAGE_TYPE:
					OnEndClientTurnMessageReceived((EndClientTurnMessage)message);
					break;
				case VisitHomeMessage.MESSAGE_TYPE:
					OnVisitHomeMessageReceived((VisitHomeMessage)message);
					break;
				case AttackNpcMessage.MESSAGE_TYPE:
					OnAttackNpcMessageReceived((AttackNpcMessage)message);
					break;
				case DuelNpcMessage.MESSAGE_TYPE:
					OnDuelNpcMessageReceived((DuelNpcMessage)message);
					break;
				case CreateAllianceMessage.MESSAGE_TYPE:
					OnCreateAllianceMessageReceived((CreateAllianceMessage)message);
					break;
				case JoinAllianceMessage.MESSAGE_TYPE:
					OnJoinAllianceMessageReceived((JoinAllianceMessage)message);
					break;
				case RequestJoinAllianceMessage.MESSAGE_TYPE:
					OnRequestJoinAllianceMessageReceived((RequestJoinAllianceMessage)message);
					break;
				case JoinAllianceUsingInvitationMessage.MESSAGE_TYPE:
					OnJoinAllianceUsingInvitationMessageReceived((JoinAllianceUsingInvitationMessage)message);
					break;
			}
		}

		private void OnGoHomeMessageReceived(GoHomeMessage message)
		{
			m_session.SendMessage(new ChangeGameStateMessage
			{
				StateType = GameStateType.HOME,
				LayoutId = message.GetLayoutId(),
				MapId = message.GetMapId()
			}, 9);
		}

		private void OnEndClientTurnMessageReceived(EndClientTurnMessage message)
		{
			GameMode gameMode = m_session.GameMode;

			if (gameMode != null)
				gameMode.OnClientTurnReceived(message.GetSubTick(), message.GetChecksum(), message.GetCommands());
		}

		private void OnVisitHomeMessageReceived(VisitHomeMessage message)
		{
			m_session.SendMessage(new ChangeGameStateMessage
			{
				StateType = GameStateType.VISIT,
				HomeId = message.RemoveHomeId(),
				VisitType = message.GetVillageType()
			}, 9);
		}

		private void OnAttackNpcMessageReceived(AttackNpcMessage message)
		{
			LogicNpcData data = message.GetNpcData();
			GameMode gameMode = m_session.GameMode;

			if (gameMode == null)
				return;
			if (data == null || !data.IsUnlockedInMap(gameMode.GetPlayerAvatar()) || !data.IsSinglePlayer())
				return;

			m_session.SendMessage(new ChangeGameStateMessage
			{
				StateType = GameStateType.NPC_ATTACK,
				NpcData = data
			}, 9);
		}

		private void OnDuelNpcMessageReceived(DuelNpcMessage message)
		{
			LogicNpcData data = message.GetNpcData();
			GameMode gameMode = m_session.GameMode;

			if (gameMode == null)
				return;
			if (data == null || !data.IsUnlockedInMap(gameMode.GetPlayerAvatar()) || data.IsSinglePlayer())
				return;

			m_session.SendMessage(new ChangeGameStateMessage
			{
				StateType = GameStateType.NPC_DUEL,
				NpcData = data
			}, 9);
		}

		private void OnCreateAllianceMessageReceived(CreateAllianceMessage message)
		{
			if (!CanJoinAlliance())
			{
				AllianceCreateFailedMessage allianceCreateFailedMessage = new AllianceCreateFailedMessage();
				allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceCreateFailedMessage, 1);
				return;
			}

			LogicClientAvatar playerAvatar = m_session.GameMode.GetPlayerAvatar();

			if (playerAvatar.GetResourceCount(LogicDataTables.GetGlobals().GetAllianceCreateResourceData()) < LogicDataTables.GetGlobals().GetAllianceCreateCost())
			{
				AllianceCreateFailedMessage allianceCreateFailedMessage = new AllianceCreateFailedMessage();
				allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceCreateFailedMessage, 1);
				return;
			}

			ServerSocket allianceServer = ServerManager.GetNextSocket(11);

			if (allianceServer != null)
			{
				ServerRequestManager.Create(new CreateAllianceRequestMessage
				{
					AllianceName = message.GetAllianceName(),
					AllianceDescription = message.GetAllianceDescription(),
					AllianceType = (AllianceType)message.GetAllianceType(),
					AllianceBadgeId = message.GetAllianceBadgeId(),
					RequiredScore = message.GetRequiredScore(),
					RequiredDuelScore = message.GetRequiredDuelScore(),
					WarFrequency = message.GetWarFrequency(),
					PublicWarLog = message.GetPublicWarLog(),
					ArrangedWarEnabled = message.GetArrangedWarEnabled()
				}, allianceServer).OnComplete = OnCreateAlliance;
			}
			else
			{
				AllianceCreateFailedMessage allianceCreateFailedMessage = new AllianceCreateFailedMessage();
				allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceCreateFailedMessage, 1);
			}
		}

		private void OnCreateAlliance(ServerRequestArgs args)
		{
			if (args.ErrorCode == ServerRequestError.Success)
			{
				CreateAllianceResponseMessage createAllianceResponseMessage = (CreateAllianceResponseMessage)args.ResponseMessage;

				if (createAllianceResponseMessage.Success)
				{
					LogicLong avatarId = m_session.GameMode.GetPlayerAvatar().GetId();
					ServerRequestManager.Create(new GameJoinAllianceRequestMessage
					{
						AccountId = avatarId,
						AllianceId = createAllianceResponseMessage.AllianceId,
						Created = true
					}, ServerManager.GetDocumentSocket(9, avatarId)).OnComplete = OnGameAllianceJoin;
				}
				else
				{
					AllianceCreateFailedMessage allianceCreateFailedMessage = new AllianceCreateFailedMessage();

					switch (createAllianceResponseMessage.ErrorReason)
					{
						case CreateAllianceResponseMessage.Reason.GENERIC:
							allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.GENERIC);
							break;
						case CreateAllianceResponseMessage.Reason.INVALID_DESCRIPTION:
							allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.INVALID_DESCRIPTION);
							break;
						case CreateAllianceResponseMessage.Reason.INVALID_NAME:
							allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.INVALID_NAME);
							break;
						case CreateAllianceResponseMessage.Reason.NAME_TOO_LONG:
							allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.NAME_TOO_LONG);
							break;
						case CreateAllianceResponseMessage.Reason.NAME_TOO_SHORT:
							allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.NAME_TOO_SHORT);
							break;
					}

					m_session.SendPiranhaMessage(allianceCreateFailedMessage, 1);
				}
			}
			else
			{
				AllianceCreateFailedMessage allianceCreateFailedMessage = new AllianceCreateFailedMessage();
				allianceCreateFailedMessage.SetReason(AllianceCreateFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceCreateFailedMessage, 1);
			}
		}

		private void OnJoinAllianceMessageReceived(JoinAllianceMessage message)
		{
			if (!CanJoinAlliance())
			{
				AllianceJoinFailedMessage allianceJoinFailedMessage = new AllianceJoinFailedMessage();
				allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.ALREADY_IN_ALLIANCE);
				m_session.SendPiranhaMessage(allianceJoinFailedMessage, 1);
				return;
			}

			LogicLong avatarId = m_session.GameMode.GetPlayerAvatar().GetId();
			ServerRequestManager.Create(new GameJoinAllianceRequestMessage
			{
				AccountId = avatarId,
				AllianceId = message.RemoveAllianceId()
			}, ServerManager.GetDocumentSocket(9, avatarId)).OnComplete = OnGameAllianceJoin;
		}

		private void OnGameAllianceJoin(ServerRequestArgs args)
		{
			if (args.ErrorCode == ServerRequestError.Aborted)
			{
				AllianceJoinFailedMessage allianceJoinFailedMessage = new AllianceJoinFailedMessage();
				allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceJoinFailedMessage, 1);
			}
			else if (!args.ResponseMessage.Success)
			{
				GameJoinAllianceResponseMessage gameJoinAllianceResponseMessage = (GameJoinAllianceResponseMessage)args.ResponseMessage;
				AllianceJoinFailedMessage allianceJoinFailedMessage = new AllianceJoinFailedMessage();

				switch (gameJoinAllianceResponseMessage.ErrorReason)
				{
					case GameJoinAllianceResponseMessage.Reason.NO_CASTLE:
					case GameJoinAllianceResponseMessage.Reason.ALREADY_IN_ALLIANCE:
					case GameJoinAllianceResponseMessage.Reason.GENERIC:
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.GENERIC);
						break;
					case GameJoinAllianceResponseMessage.Reason.FULL:
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.FULL);
						break;
					case GameJoinAllianceResponseMessage.Reason.CLOSED:
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.CLOSED);
						break;
					case GameJoinAllianceResponseMessage.Reason.SCORE:
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.SCORE);
						break;
					case GameJoinAllianceResponseMessage.Reason.BANNED:
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.BANNED);
						break;
				}

				m_session.SendPiranhaMessage(allianceJoinFailedMessage, 1);
			}
		}

		private void OnRequestJoinAllianceMessageReceived(RequestJoinAllianceMessage message)
		{
			if (CanJoinAlliance())
			{
				LogicLong allianceId = message.RemoveAllianceId();
				ServerRequestManager.Create(new RequestAllianceJoinRequestMessage
				{
					Avatar = m_session.GameMode.GetPlayerAvatar(),
					AllianceId = allianceId,
					Message = message.GetMessage()
				}, ServerManager.GetDocumentSocket(11, allianceId)).OnComplete = OnRequestAlliance;
			}
		}

		private void OnJoinAllianceUsingInvitationMessageReceived(JoinAllianceUsingInvitationMessage message)
		{
			if (CanJoinAlliance())
			{
				ServerRequestManager.Create(new LoadAvatarStreamRequestMessage
				{
					Id = message.GetAvatarStreamEntryId()
				}, ServerManager.GetDocumentSocket(11, m_session.AccountId), 5).OnComplete = args =>
				{
					if (m_session.IsDestructed())
						return;

					if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
					{
						LoadAvatarStreamResponseMessage responseMessage = (LoadAvatarStreamResponseMessage)args.ResponseMessage;
						AllianceInvitationAvatarStreamEntry allianceInvitationAvatarStreamEntry = (AllianceInvitationAvatarStreamEntry)responseMessage.Entry;
						LogicLong allianceId = allianceInvitationAvatarStreamEntry.GetAllianceId();
						ServerRequestManager.Create(new GameJoinAllianceRequestMessage
						{
							AccountId = m_session.AccountId,
							AllianceId = allianceId,
							AvatarStreamId = allianceInvitationAvatarStreamEntry.GetId(),
							Invited = true
						}, ServerManager.GetDocumentSocket(9, m_session.AccountId)).OnComplete = OnGameAllianceJoin;
					}
					else
					{
						AllianceJoinFailedMessage allianceJoinFailedMessage = new AllianceJoinFailedMessage();
						allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.ALREADY_IN_ALLIANCE);
						m_session.SendPiranhaMessage(allianceJoinFailedMessage, 1);
					}
				};
			}
			else
			{
				AllianceJoinFailedMessage allianceJoinFailedMessage = new AllianceJoinFailedMessage();
				allianceJoinFailedMessage.SetReason(AllianceJoinFailedMessage.Reason.ALREADY_IN_ALLIANCE);
				m_session.SendPiranhaMessage(allianceJoinFailedMessage, 1);
			}
		}

		private void OnRequestAlliance(ServerRequestArgs args)
		{
			if (m_session.IsDestructed())
				return;

			if (args.ErrorCode == ServerRequestError.Success)
			{
				RequestAllianceJoinResponseMessage responseMessage = (RequestAllianceJoinResponseMessage)args.ResponseMessage;

				if (responseMessage.Success)
				{
					m_session.SendPiranhaMessage(new AllianceJoinRequestOkMessage(), 1);
				}
				else
				{
					AllianceJoinRequestFailedMessage allianceJoinRequestFailedMessage = new AllianceJoinRequestFailedMessage();

					switch (responseMessage.ErrorReason)
					{
						case RequestAllianceJoinResponseMessage.Reason.GENERIC:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.GENERIC);
							break;
						case RequestAllianceJoinResponseMessage.Reason.CLOSED:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.CLOSED);
							break;
						case RequestAllianceJoinResponseMessage.Reason.ALREADY_SENT:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.ALREADY_SENT);
							break;
						case RequestAllianceJoinResponseMessage.Reason.NO_SCORE:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.NO_SCORE);
							break;
						case RequestAllianceJoinResponseMessage.Reason.BANNED:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.BANNED);
							break;
						case RequestAllianceJoinResponseMessage.Reason.TOO_MANY_PENDING_REQUESTS:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.TOO_MANY_PENDING_REQUESTS);
							break;
						case RequestAllianceJoinResponseMessage.Reason.NO_DUEL_SCORE:
							allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.NO_DUEL_SCORE);
							break;
					}

					m_session.SendPiranhaMessage(allianceJoinRequestFailedMessage, 1);
				}
			}
			else
			{
				AllianceJoinRequestFailedMessage allianceJoinRequestFailedMessage = new AllianceJoinRequestFailedMessage();
				allianceJoinRequestFailedMessage.SetReason(AllianceJoinRequestFailedMessage.Reason.GENERIC);
				m_session.SendPiranhaMessage(allianceJoinRequestFailedMessage, 1);
			}
		}

		private bool CanJoinAlliance()
			=> !m_session.GameMode.GetPlayerAvatar().IsInAlliance() && !m_session.GameMode.GetAwaitingExecutionOfCommandType(LogicCommandType.JOIN_ALLIANCE) &&
				   m_session.GameMode.GetPlayerAvatar().HasAllianceCastle();
	}
}