using System;
using System.Collections.Generic;

using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Logic.Message.Alliance.Stream;
using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Database.Document;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Stream.Session;
using Supercell.Magic.Servers.Stream.Util;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Stream.Logic
{
	public class Alliance : AllianceDocument
	{
		public const int MAX_STREAM_ENTRY_COUNT = 100;

		private readonly Dictionary<long, AllianceSession> m_onlineMembers;

		public Alliance()
		{
			m_onlineMembers = new Dictionary<long, AllianceSession>();
		}

		public Alliance(LogicLong id) : base(id)
		{
			m_onlineMembers = new Dictionary<long, AllianceSession>();
		}

		public void AddMember(AllianceMemberEntry member)
		{
			if (!Members.TryAdd(member.GetAvatarId(), member))
				throw new Exception("Alliance.addMember: member already in this alliance!");
			if (IsFull())
				throw new Exception("Alliance.addMember: alliance is FULL!");

			Header.SetNumberOfMembers(Members.Count);
			UpdateScoring();
			OnMemberAdded(member);

			KickedMembersTimes.Remove(member.GetAvatarId());
		}

		public void RemoveMember(LogicLong avatarId)
		{
			if (Members.Remove(avatarId))
			{
				Header.SetNumberOfMembers(Members.Count);
				OnMemberRemoved(avatarId);
				UpdateScoring();

				if (m_onlineMembers.Remove(avatarId))
					OnOnlineMemberChanged();
			}
		}

		public bool IsBanned(LogicLong avatarId)
		{
			if (KickedMembersTimes.TryGetValue(avatarId, out DateTime kickTime))
			{
				if (DateTime.UtcNow.Subtract(kickTime).TotalDays < 1d)
					return true;
				KickedMembersTimes.Remove(KickedMembersTimes.Count);
			}

			return false;
		}

		public void AddOnlineMember(LogicLong avatarId, AllianceSession session)
		{
			if (!m_onlineMembers.TryAdd(avatarId, session))
				m_onlineMembers[avatarId] = session;
			OnOnlineMemberChanged();
		}

		public void RemoveOnlineMember(LogicLong avatarId, AllianceSession session)
		{
			if (m_onlineMembers.TryGetValue(avatarId, out AllianceSession currentSession) && currentSession.Id == session.Id &&
				m_onlineMembers.Remove(avatarId))
			{
				OnOnlineMemberChanged();
			}
		}

		public AllianceSession GetCurrentOnlineMemberSession(LogicLong avatarId)
		{
			if (m_onlineMembers.TryGetValue(avatarId, out AllianceSession session))
				return session;
			return null;
		}

		public void ChangeMemberRole(AllianceMemberEntry allianceMemberEntry, LogicAvatarAllianceRole allianceRole, LogicLong eventAvatarId, string eventAvatarName)
		{
			if (allianceMemberEntry.GetAllianceRole() != allianceRole)
			{
				bool isPromoted = allianceMemberEntry.HasLowerRoleThan(allianceRole);

				allianceMemberEntry.SetAllianceRole(allianceRole);

				LogicChangeAllianceRoleCommand logicChangeAllianceRoleCommand = new LogicChangeAllianceRoleCommand();
				logicChangeAllianceRoleCommand.SetData(Id, allianceRole);
				ServerMessageManager.SendMessage(new GameAllowServerCommandMessage
				{
					AccountId = allianceMemberEntry.GetAvatarId(),
					ServerCommand = logicChangeAllianceRoleCommand
				}, 9);

				AllianceEventStreamEntry allianceEventStreamEntry = new AllianceEventStreamEntry();
				AllianceStreamEntryUtil.SetSenderInfo(allianceEventStreamEntry, allianceMemberEntry);

				allianceEventStreamEntry.SetEventAvatarId(eventAvatarId);
				allianceEventStreamEntry.SetEventAvatarName(eventAvatarName);
				allianceEventStreamEntry.SetEventType(isPromoted ? AllianceEventStreamEntryType.PROMOTED : AllianceEventStreamEntryType.DEMOTED);

				StreamManager.Create(Id, allianceEventStreamEntry);

				AddStreamEntry(allianceEventStreamEntry);
			}
		}

		public void UpdateScoring()
		{
			CalculateRanking();
			CalculateDuelRanking();
			CalculateScore();
		}

		private void CalculateRanking()
		{
			LogicArrayList<AllianceMemberEntry> arrayList = new LogicArrayList<AllianceMemberEntry>(Members.Count);

			foreach (AllianceMemberEntry member in Members.Values)
			{
				int idx = -1;

				for (int i = 0; i < arrayList.Size(); i++)
				{
					if (member.GetScore() > arrayList[i].GetScore())
					{
						idx = i;
						break;
					}
				}

				arrayList.Add(idx == -1 ? arrayList.Size() : idx, member);
			}

			for (int i = 0; i < arrayList.Size(); i++)
			{
				arrayList[i].SetOrder(i + 1);

				if (arrayList[i].GetPreviousOrder() == 0)
					arrayList[i].SetPreviousOrder(i + 1);
			}
		}

		private void CalculateDuelRanking()
		{
			LogicArrayList<AllianceMemberEntry> arrayList = new LogicArrayList<AllianceMemberEntry>(Members.Count);

			foreach (AllianceMemberEntry member in Members.Values)
			{
				int idx = -1;

				for (int i = 0; i < arrayList.Size(); i++)
				{
					if (member.GetDuelScore() > arrayList[i].GetDuelScore())
					{
						idx = i;
						break;
					}
				}

				arrayList.Add(idx == -1 ? arrayList.Size() : idx, member);
			}

			for (int i = 0; i < arrayList.Size(); i++)
			{
				arrayList[i].SetOrderVillage2(i + 1);

				if (arrayList[i].GetPreviousOrderVillage2() == 0)
					arrayList[i].SetPreviousOrderVillage2(i + 1);
			}
		}

		private void CalculateScore()
		{
			int sum1 = 0;
			int sum2 = 0;

			foreach (AllianceMemberEntry member in Members.Values)
			{
				sum1 += member.GetScore();
				sum2 += member.GetDuelScore();
			}

			Header.SetScore(sum1 / 2 + (sum1 % 2 > 0 ? 1 : 0));
			Header.SetDuelScore(sum2 / 2 + (sum2 % 2 > 0 ? 1 : 0));
		}

		public void AddStreamEntry(StreamEntry entry)
		{
			if (entry.GetId().IsZero())
				throw new Exception("Alliance.addStreamEntry: id should be set!");

			while (StreamEntryList.Size() >= Alliance.MAX_STREAM_ENTRY_COUNT)
				RemoveStreamEntry(StreamEntryList[0]);

			StreamEntryList.Add(entry.GetId());
			OnStreamEntryAdded(entry);
		}

		public void UpdateStreamEntry(StreamEntry entry)
		{
			OnStreamEntryAdded(entry);
		}

		public void RemoveStreamEntry(LogicLong streamId)
		{
			int index = StreamEntryList.IndexOf(streamId);

			if (index != -1)
			{
				StreamEntryList.Remove(index);
				OnStreamEntryRemoved(streamId);

				StreamManager.RemoveAllianceStream(streamId);
			}
			else
			{
				Logging.Warning("Alliance.removeStreamEntry: unknown stream entry id: " + (long)streamId);
			}
		}

		private void OnMemberAdded(AllianceMemberEntry entry)
		{
			AllianceMemberMessage allianceMemberMessage = new AllianceMemberMessage();
			allianceMemberMessage.SetAllianceMemberEntry(entry);
			SendPiranhaMessage(allianceMemberMessage, 1);
		}

		private void OnMemberRemoved(LogicLong avatarId)
		{
			AllianceMemberRemovedMessage allianceMemberRemovedMessage = new AllianceMemberRemovedMessage();
			allianceMemberRemovedMessage.SetMemberAvatarId(avatarId);
			SendPiranhaMessage(allianceMemberRemovedMessage, 1);
		}

		private void OnOnlineMemberChanged()
		{
			AllianceOnlineStatusUpdatedMessage allianceOnlineStatusUpdatedMessage = new AllianceOnlineStatusUpdatedMessage();
			allianceOnlineStatusUpdatedMessage.SetMembersCount(Members.Count);
			allianceOnlineStatusUpdatedMessage.SetMembersOnline(m_onlineMembers.Count);
			SendPiranhaMessage(allianceOnlineStatusUpdatedMessage, 1);
		}

		private void OnStreamEntryAdded(StreamEntry entry)
		{
			AllianceStreamEntryMessage allianceStreamEntryMessage = new AllianceStreamEntryMessage();
			allianceStreamEntryMessage.SetStreamEntry(entry);
			SendPiranhaMessage(allianceStreamEntryMessage, 1);
		}

		private void OnStreamEntryRemoved(LogicLong id)
		{
			AllianceStreamEntryRemovedMessage allianceStreamEntryRemovedMessage = new AllianceStreamEntryRemovedMessage();
			allianceStreamEntryRemovedMessage.SetStreamEntryId(id);
			SendPiranhaMessage(allianceStreamEntryRemovedMessage, 1);
		}

		public AllianceDataMessage GetAllianceDataMessage()
		{
			AllianceDataMessage allianceDataMessage = new AllianceDataMessage();
			AllianceFullEntry allianceFullEntry = new AllianceFullEntry();
			LogicArrayList<AllianceMemberEntry> memberEntryList = new LogicArrayList<AllianceMemberEntry>(Members.Count);

			allianceFullEntry.SetAllianceHeaderEntry(Header);
			allianceFullEntry.SetAllianceDescription(Description);
			allianceFullEntry.SetAllianceMembers(memberEntryList);

			foreach (AllianceMemberEntry entry in Members.Values)
				memberEntryList.Add(entry);

			allianceDataMessage.SetAllianceFullEntry(allianceFullEntry);

			return allianceDataMessage;
		}

		public AllianceFullEntryUpdateMessage GetAllianceFulEntryUpdateMessage()
		{
			AllianceFullEntryUpdateMessage allianceFullEntryUpdateMessage = new AllianceFullEntryUpdateMessage();

			allianceFullEntryUpdateMessage.SetAllianceHeaderEntry(Header);
			allianceFullEntryUpdateMessage.SetDescription(Description);

			return allianceFullEntryUpdateMessage;
		}

		public AllianceStreamMessage GetAllianceStreamMessage()
		{
			AllianceStreamMessage allianceStreamMessage = new AllianceStreamMessage();
			LogicArrayList<StreamEntry> streamEntryList = new LogicArrayList<StreamEntry>(StreamEntryList.Size());

			for (int i = 0; i < StreamEntryList.Size(); i++)
			{
				StreamEntry streamEntry = StreamManager.GetAllianceStream(StreamEntryList[i]);

				if (streamEntry != null)
					streamEntryList.Add(streamEntry);
			}

			allianceStreamMessage.SetStreamEntries(streamEntryList);

			return allianceStreamMessage;
		}

		public void SendPiranhaMessage(PiranhaMessage piranhaMessage, int serverType)
		{
			foreach (AllianceSession session in m_onlineMembers.Values)
			{
				session.SendPiranhaMessage(piranhaMessage, serverType);
			}
		}

		public void AllowServerCommand(LogicServerCommand serverCommand)
		{
			foreach (long id in Members.Keys)
			{
				ServerMessageManager.SendMessage(new GameAllowServerCommandMessage
				{
					AccountId = id,
					ServerCommand = serverCommand
				}, 9);
			}
		}

		public void SetAllianceSettings(string description, AllianceType type, int badgeId, int requiredScore, int requiredDuelScore, int warFrequency, LogicData originData, bool publicWarLog, bool arrangedWarEnabled)
		{
			int allianceLevel = Header.GetAllianceLevel();

			if (description == null)
				description = string.Empty;
			if (description.Length > 128)
				description = description.Substring(0, 128);

			type = (AllianceType)LogicMath.Clamp((int)type, (int)AllianceType.OPEN, (int)AllianceType.CLOSED);

			AllianceBadgeUtil.ParseAllianceBadgeLayer(badgeId, out LogicAllianceBadgeLayerData middle, out LogicAllianceBadgeLayerData background, out LogicAllianceBadgeLayerData foreground);

			if (middle != null && middle.GetRequiredClanLevel() > allianceLevel)
				middle = AllianceBadgeUtil.GetFirstUnlockedAllianceBadgeLayerByType(LogicAllianceBadgeLayerType.MIDDLE, allianceLevel);
			if (background != null && background.GetRequiredClanLevel() > allianceLevel)
				background = AllianceBadgeUtil.GetFirstUnlockedAllianceBadgeLayerByType(LogicAllianceBadgeLayerType.BACKGROUND, allianceLevel);
			if (foreground != null && foreground.GetRequiredClanLevel() > allianceLevel)
				foreground = AllianceBadgeUtil.GetFirstUnlockedAllianceBadgeLayerByType(LogicAllianceBadgeLayerType.FOREGROUND, allianceLevel);

			badgeId = AllianceBadgeUtil.GetAllianceBadgeId(middle, background, foreground);

			if (originData != null && originData.GetDataType() != LogicDataType.REGION)
				originData = null;

			Description = description;
			Header.SetAllianceType(type);
			Header.SetAllianceBadgeId(badgeId);
			Header.SetRequiredScore(requiredScore);
			Header.SetRequiredDuelScore(requiredDuelScore);
			Header.SetWarFrequency(warFrequency);
			Header.SetOriginData(originData);
			Header.SetPublicWarLog(publicWarLog);
			Header.SetArrangedWarEnabled(arrangedWarEnabled);

			if (Members.Count != 0)
			{
				LogicAllianceSettingsChangedCommand allianceSettingsChangedCommand = new LogicAllianceSettingsChangedCommand();

				allianceSettingsChangedCommand.SetAllianceData(Id, badgeId);

				AllowServerCommand(allianceSettingsChangedCommand);
				SendPiranhaMessage(GetAllianceFulEntryUpdateMessage(), 1);
			}
		}
	}
}