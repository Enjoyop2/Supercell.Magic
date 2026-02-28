using System;

using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Servers.Core.Network.Message.Session;
using Supercell.Magic.Servers.Core.Network.Request;
using Supercell.Magic.Servers.Core.Session;
using Supercell.Magic.Servers.Stream.Logic;
using Supercell.Magic.Servers.Stream.Session.Message;
using Supercell.Magic.Servers.Stream.Util;

namespace Supercell.Magic.Servers.Stream.Session
{
	public class AllianceSession : ServerSession
	{
		public LogicMessageManager LogicMessageManager
		{
			get;
		}
		public Alliance Alliance
		{
			get; private set;
		}
		public LogicClientAvatar LogicClientAvatar
		{
			get; private set;
		}

		public AllianceSession(StartServerSessionMessage message) : base(message)
		{
			LogicMessageManager = new LogicMessageManager(this);

			ServerRequestManager.Create(new AvatarRequestMessage
			{
				AccountId = AccountId
			}, ServerManager.GetDocumentSocket(9, AccountId)).OnComplete = OnAvatarReceived;
		}

		private void OnAvatarReceived(ServerRequestArgs args)
		{
			if (args.ErrorCode == ServerRequestError.Success && args.ResponseMessage.Success)
			{
				LogicClientAvatar = ((AvatarResponseMessage)args.ResponseMessage).LogicClientAvatar;

				if (AllianceManager.TryGet(LogicClientAvatar.GetAllianceId(), out Alliance avatarAlliance) && avatarAlliance.Members.ContainsKey(AccountId))
				{
					Alliance = avatarAlliance;
					Alliance.AddOnlineMember(AccountId, this);

					SendPiranhaMessage(Alliance.GetAllianceFulEntryUpdateMessage(), 1);
					SendPiranhaMessage(Alliance.GetAllianceStreamMessage(), 1);

					AllianceMemberUtil.SetLogicClientAvatarToAllianceMemberEntry(LogicClientAvatar, Alliance.Members[AccountId], Alliance);
					AllianceManager.Save(Alliance);
				}
				else
				{
					SendMessage(new StopServerSessionMessage(), 1);

					ServerMessageManager.SendMessage(new AllianceLeavedMessage
					{
						AccountId = AccountId,
						AllianceId = LogicClientAvatar.GetAllianceId()
					}, 9);
					AllianceSessionManager.Remove(Id);
				}
			}
			else
			{
				SendMessage(new StopServerSessionMessage(), 1);
				AllianceSessionManager.Remove(Id);
			}
		}

		public override void Destruct()
		{
			base.Destruct();

			if (Alliance != null)
				Alliance.RemoveOnlineMember(LogicClientAvatar.GetId(), this);
		}
	}
}