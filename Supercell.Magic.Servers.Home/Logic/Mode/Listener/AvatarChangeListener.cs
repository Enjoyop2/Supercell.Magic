using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Avatar.Change;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Data;

using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Session.Change;

using Supercell.Magic.Servers.Home.Logic.Mode;

using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Home.Logic.Mode.Listener
{
	public class AvatarChangeListener : LogicAvatarChangeListener
	{
		private readonly GameMode m_gameMode;
		private readonly LogicClientAvatar m_playerAvatar;
		private readonly LogicArrayList<AvatarChange> m_avatarChanges;

		public AvatarChangeListener(GameMode gameMode, LogicClientAvatar playerAvatar)
		{
			m_gameMode = gameMode;
			m_playerAvatar = playerAvatar;
			m_avatarChanges = new LogicArrayList<AvatarChange>(16);
		}

		public LogicArrayList<AvatarChange> RemoveAvatarChanges()
		{
			LogicArrayList<AvatarChange> arrayList = new LogicArrayList<AvatarChange>();
			arrayList.AddAll(m_avatarChanges);
			m_avatarChanges.Clear();
			return arrayList;
		}

		public override void FreeDiamondsAdded(int count, int mode)
		{
			m_avatarChanges.Add(new DiamondAvatarChange
			{
				Count = count
			});
		}

		public override void DiamondPurchaseMade(int type, int globalId, int level, int count, int villageType)
		{
			m_avatarChanges.Add(new DiamondAvatarChange
			{
				Count = -count
			});
		}

		public override void CommodityCountChanged(int commodityType, LogicData data, int count)
		{
			m_avatarChanges.Add(new CommodityCountAvatarChange
			{
				Type = commodityType,
				Data = data,
				Count = count
			});
		}

		public override void WarPreferenceChanged(int preference)
		{
			ServerMessageManager.SendMessage(new GameAllowServerCommandMessage
			{
				AccountId = m_playerAvatar.GetId(),
				ServerCommand = new LogicUpdateWarPreferenceCommand(preference)
			}, 9);

			m_avatarChanges.Add(new WarPreferenceAvatarChange
			{
				Preference = preference
			});
		}

		public override void ExpPointsGained(int count)
		{
			m_avatarChanges.Add(new ExpPointsAvatarChange
			{
				Points = count
			});
		}

		public override void ExpLevelGained(int count)
		{
			m_avatarChanges.Add(new ExpLevelAvatarChange
			{
				Points = count
			});
		}

		public override void AllianceJoined(LogicLong allianceId, string allianceName, int allianceBadgeId, int allianceExpLevel, LogicAvatarAllianceRole allianceRole)
		{
			m_avatarChanges.Add(new AllianceJoinedAvatarChange
			{
				AllianceId = allianceId,
				AllianceName = allianceName,
				AllianceBadgeId = allianceBadgeId,
				AllianceExpLevel = allianceExpLevel,
				AllianceRole = allianceRole
			});
		}

		public override void AllianceLeft()
		{
			m_avatarChanges.Add(new AllianceLeftAvatarChange());
		}

		public override void AllianceLevelChanged(int expLevel)
		{
			m_avatarChanges.Add(new AllianceLevelAvatarChange
			{
				Level = expLevel
			});
		}

		public override void AllianceUnitAdded(LogicCombatItemData data, int upgLevel)
		{
			m_avatarChanges.Add(new AllianceUnitAddedAvatarChange
			{
				Data = data,
				UpgradeLevel = upgLevel
			});
		}

		public override void AllianceUnitRemoved(LogicCombatItemData data, int upgLevel)
		{
			m_avatarChanges.Add(new AllianceUnitRemovedAvatarChange
			{
				Data = data,
				UpgradeLevel = upgLevel
			});
		}

		public override void AllianceUnitCountChanged(LogicCombatItemData data, int upgLevel, int count)
		{
			m_avatarChanges.Add(new AllianceUnitCountAvatarChange
			{
				Data = data,
				UpgradeLevel = upgLevel,
				Count = count
			});
		}

		public override void SetAllianceCastleLevel(int count)
		{
			m_avatarChanges.Add(new AllianceCastleLevelAvatarChange
			{
				Level = count
			});
		}

		public override void SetTownHallLevel(int count)
		{
			m_avatarChanges.Add(new TownHallLevelAvatarChange
			{
				Level = count
			});
		}

		public override void SetVillage2TownHallLevel(int count)
		{
			m_avatarChanges.Add(new TownHallV2LevelAvatarChange
			{
				Level = count
			});
		}

		public override void LegendSeasonScoreChanged(int state, int score, int scoreChange, bool bestSeason, int villageType)
		{
			m_avatarChanges.Add(new LegendSeasonScoreAvatarChange
			{
				Entry = villageType == 0
					? m_playerAvatar.GetLegendSeasonEntry()
					: m_playerAvatar.GetLegendSeasonEntryVillage2()
			});
		}

		public override void ScoreChanged(LogicLong allianceId, int scoreGain, int minScoreGain, bool attacker, LogicLeagueData leagueData, LogicLeagueData prevLeagueData, int destructionPercentage)
		{
			m_avatarChanges.Add(new ScoreAvatarChange
			{
				Attacker = attacker,
				LeagueData = leagueData,
				PrevLeagueData = prevLeagueData,
				ScoreGain = scoreGain
			});
		}

		public override void DuelScoreChanged(LogicLong allianceId, int duelScoreGain, int resultType, bool attacker)
		{
			m_avatarChanges.Add(new DuelScoreAvatarChange
			{
				Attacker = attacker,
				DuelScoreGain = duelScoreGain,
				ResultType = resultType
			});
		}

		public override void LeagueChanged(int leagueType, LogicLong leagueInstanceId)
		{
			m_avatarChanges.Add(new LeagueAvatarChange
			{
				LeagueInstanceId = leagueInstanceId,
				LeagueType = leagueType
			});
		}

		public override void AttackShieldReduceCounterChanged(int count)
		{
			m_avatarChanges.Add(new AttackShieldReduceCounterAvatarChange
			{
				Count = count
			});
		}

		public override void DefenseVillageGuardCounterChanged(int count)
		{
			m_avatarChanges.Add(new DefenseVillageGuardCounterAvatarChange
			{
				Count = count
			});
		}

		public override void REDPackageStateChanged(int value)
		{
			m_avatarChanges.Add(new REDPackageStateAvatarChange
			{
				State = value
			});
		}

		public void NameChanged(string name, int nameChangeState)
		{
			m_avatarChanges.Add(new NameAvatarChange
			{
				Name = name,
				NameChangeState = nameChangeState
			});
		}

		public override void SendClanMail(string message)
		{
			ServerMessageManager.SendMessage(new AllianceCreateMailMessage
			{
				AccountId = m_playerAvatar.GetAllianceId(),
				MemberId = m_playerAvatar.GetId(),
				Message = message
			}, 11);
		}

		public override void ShareReplay(LogicLong replayId, string message)
		{
			ServerMessageManager.SendMessage(new AllianceShareReplayMessage
			{
				AccountId = m_playerAvatar.GetAllianceId(),
				MemberId = m_playerAvatar.GetId(),
				ReplayId = replayId,
				Message = message
			}, 11);
		}

		public override void RequestAllianceUnits(int upgLevel, int usedCapacity, int maxCapacity, int spellUsedCapacity, int maxSpellCapacity, string message)
		{
			ServerMessageManager.SendMessage(new AllianceRequestAllianceUnitsMessage
			{
				AccountId = m_playerAvatar.GetAllianceId(),
				MemberId = m_playerAvatar.GetId(),
				Message = message,
				CastleUpgradeLevel = upgLevel,
				CastleUsedCapacity = usedCapacity,
				CastleTotalCapacity = maxCapacity,
				CastleSpellUsedCapacity = spellUsedCapacity,
				CastleSpellTotalCapacity = maxSpellCapacity
			}, 11);
		}

		public override void AllianceUnitDonateOk(LogicCombatItemData data, int upgLevel, LogicLong streamId, bool quickDonate)
		{
			ServerMessageManager.SendMessage(new AllianceUnitDonateResponseMessage
			{
				AccountId = m_playerAvatar.GetAllianceId(),
				MemberId = m_playerAvatar.GetId(),
				MemberName = m_playerAvatar.GetName(),
				StreamId = streamId,
				Data = data,
				UpgradeLevel = upgLevel,
				QuickDonate = quickDonate,
				Success = true
			}, 11);
		}

		public override void AllianceUnitDonateFailed(LogicCombatItemData data, int upgLevel, LogicLong streamId, bool quickDonate)
		{
			ServerMessageManager.SendMessage(new AllianceUnitDonateResponseMessage
			{
				AccountId = m_playerAvatar.GetAllianceId(),
				MemberId = m_playerAvatar.GetId(),
				StreamId = streamId,
				Data = data,
				UpgradeLevel = upgLevel,
				QuickDonate = quickDonate
			}, 11);
		}

		public override void SendChallengeRequest(string message, int layoutId, bool warLayout, int villageType)
		{
			if (villageType == 0)
			{
				ServerMessageManager.SendMessage(new AllianceChallengeRequestMessage
				{
					AccountId = m_playerAvatar.GetAllianceId(),
					MemberId = m_playerAvatar.GetId(),
					HomeJSON = m_gameMode.CreateChallengeSnapshot(),
					Message = message,
					WarLayout = warLayout
				}, 11);
			}
		}
	}
}