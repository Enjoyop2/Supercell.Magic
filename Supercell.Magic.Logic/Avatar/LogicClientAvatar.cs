using Supercell.Magic.Logic.Calendar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.League.Entry;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Avatar
{
	public sealed class LogicClientAvatar : LogicAvatar
	{
		private LogicLong m_id;
		private LogicLong m_currentHomeId;
		private LogicLong m_allianceId;
		private LogicLong m_leagueInstanceId;
		private LogicLong m_challengeId;
		private LogicLong m_warInstanceId;

		private LogicLegendSeasonEntry m_legendSeasonEntry;
		private LogicLegendSeasonEntry m_legendSeasonEntryVillage2;

		private bool m_nameSetByUser;
		private bool m_allianceChatFilter;

		private LogicAvatarAllianceRole m_allianceRole;
		private int m_allianceExpLevel;
		private int m_legendaryScore;
		private int m_legendaryScoreVillage2;
		private int m_expLevel;
		private int m_expPoints;
		private int m_diamonds;
		private int m_freeDiamonds;
		private int m_cumulativePurchasedDiamonds;
		private int m_score;
		private int m_duelScore;
		private int m_warPreference;
		private int m_attackRating;
		private int m_attackKFactor;
		private int m_attackWinCount;
		private int m_attackLoseCount;
		private int m_defenseWinCount;
		private int m_defenseLoseCount;
		private int m_treasuryGoldCount;
		private int m_treasuryElixirCount;
		private int m_treasuryDarkElixirCount;
		private int m_nameChangeState;
		private int m_attackShieldReduceCounter;
		private int m_defenseVillageGuardCounter;
		private int m_duelWinCount;
		private int m_duelLoseCount;
		private int m_duelDrawCount;
		private int m_challengeState;

		private string m_facebookId;
		private string m_allianceName;
		private string m_name;

		public LogicClientAvatar()
		{
			m_legendSeasonEntry = new LogicLegendSeasonEntry();
			m_legendSeasonEntryVillage2 = new LogicLegendSeasonEntry();

			m_expLevel = 1;
			m_allianceBadgeId = -1;
			m_nameChangeState = -1;
			m_attackRating = 1200;
			m_attackKFactor = 60;
			m_warPreference = 1;

			InitBase();
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_legendSeasonEntry != null)
			{
				m_legendSeasonEntry.Destruct();
				m_legendSeasonEntry = null;
			}

			if (m_legendSeasonEntryVillage2 != null)
			{
				m_legendSeasonEntryVillage2.Destruct();
				m_legendSeasonEntryVillage2 = null;
			}

			m_id = null;
			m_currentHomeId = null;
			m_leagueInstanceId = null;
			m_allianceId = null;
			m_allianceName = null;
			m_facebookId = null;
			m_name = null;
		}

		public override void InitBase()
		{
			base.InitBase();

			m_name = string.Empty;

			m_id = new LogicLong();
			m_currentHomeId = new LogicLong();
			m_warInstanceId = new LogicLong();
		}

		public override void GetChecksum(ChecksumHelper checksumHelper)
		{
			checksumHelper.StartObject("LogicClientAvatar");
			base.GetChecksum(checksumHelper);
			checksumHelper.WriteValue("m_expPoints", m_expPoints);
			checksumHelper.WriteValue("m_expLevel", m_expLevel);
			checksumHelper.WriteValue("m_diamonds", m_diamonds);
			checksumHelper.WriteValue("m_freeDiamonds", m_freeDiamonds);
			checksumHelper.WriteValue("m_score", m_score);
			checksumHelper.WriteValue("m_duelScore", m_duelScore);

			if (IsInAlliance())
			{
				checksumHelper.WriteValue("isInAlliance", 13);
			}

			checksumHelper.EndObject();
		}

		public static LogicClientAvatar GetDefaultAvatar()
		{
			LogicClientAvatar defaultAvatar = new LogicClientAvatar();
			LogicGlobals globalsInstance = LogicDataTables.GetGlobals();

			defaultAvatar.m_diamonds = globalsInstance.GetStartingDiamonds();
			defaultAvatar.m_freeDiamonds = globalsInstance.GetStartingDiamonds();

			defaultAvatar.SetResourceCount(LogicDataTables.GetGoldData(), globalsInstance.GetStartingGold());
			defaultAvatar.SetResourceCount(LogicDataTables.GetGold2Data(), globalsInstance.GetStartingGold2());
			defaultAvatar.SetResourceCount(LogicDataTables.GetElixirData(), globalsInstance.GetStartingElixir());
			defaultAvatar.SetResourceCount(LogicDataTables.GetElixir2Data(), globalsInstance.GetStartingElixir2());

			return defaultAvatar;
		}

		public override bool IsClientAvatar()
			=> true;

		public override LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}

		public override string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public override LogicAvatarAllianceRole GetAllianceRole()
			=> m_allianceRole;

		public void SetAllianceRole(LogicAvatarAllianceRole value)
		{
			m_allianceRole = value;
		}

		public override int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public int GetAllianceLevel()
			=> m_allianceExpLevel;

		public void SetAllianceLevel(int value)
		{
			m_allianceExpLevel = value;
		}

		public int GetWarPreference()
			=> m_warPreference;

		public void SetWarPreference(int preference)
		{
			m_warPreference = preference;
		}

		public override bool IsInAlliance()
			=> m_allianceId != null;

		public override int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int expLevel)
		{
			m_expLevel = expLevel;
		}

		public int GetExpPoints()
			=> m_expPoints;

		public void SetExpPoints(int expPoints)
		{
			m_expPoints = expPoints;
		}

		public LogicLong GetId()
			=> m_id;

		public void SetId(LogicLong id)
		{
			m_id = id;
		}

		public LogicLong GetCurrentHomeId()
			=> m_currentHomeId;

		public void SetCurrentHomeId(LogicLong id)
		{
			m_currentHomeId = id;
		}

		public bool GetNameSetByUser()
			=> m_nameSetByUser;

		public void SetNameSetByUser(bool set)
		{
			m_nameSetByUser = set;
		}

		public int GetNameChangeState()
			=> m_nameChangeState;

		public void SetNameChangeState(int state)
		{
			m_nameChangeState = state;
		}

		public override string GetName()
			=> m_name;

		public void SetName(string name)
		{
			m_name = name;
		}

		public string GetFacebookId()
			=> m_facebookId;

		public void SetFacebookId(string facebookId)
		{
			m_facebookId = facebookId;
		}

		public bool GetAllianceChatFilterEnabled()
			=> m_allianceChatFilter;

		public void SetAllianceChatFilterEnabled(bool enabled)
		{
			m_allianceChatFilter = enabled;
		}

		public bool HasEnoughDiamonds(int count, bool callListener, LogicLevel level)
		{
			bool enough = m_diamonds >= count;

			if (!enough && callListener)
			{
				level.GetGameListener().NotEnoughDiamonds();
			}

			return enough;
		}

		public bool HasEnoughResources(LogicResourceData data, int count, bool callListener, LogicCommand command, bool unk)
		{
			bool enough = GetResourceCount(data) >= count;

			if (callListener && !enough)
			{
				if (m_level != null)
				{
					m_level.GetGameListener().NotEnoughResources(data, count, command, unk);
				}
			}

			return enough;
		}

		public bool HasEnoughResources(LogicResourceData data1, int count1, LogicResourceData data2, int count2, bool callListener, LogicCommand command, bool unk)
		{
			int resourceCount1 = GetResourceCount(data1);
			int resourceCount2 = GetResourceCount(data2);

			bool enough = resourceCount1 >= count1 && resourceCount2 >= count2;

			if (callListener && !enough)
			{
				if (m_level != null)
				{
					if (resourceCount1 >= count1 || resourceCount2 >= count2)
					{
						if (resourceCount1 >= count1)
						{
							if (resourceCount2 < count2)
							{
								m_level.GetGameListener().NotEnoughResources(data2, count2, command, unk);
							}
						}
						else
						{
							m_level.GetGameListener().NotEnoughResources(data1, count1, command, unk);
						}
					}
					else
					{
						m_level.GetGameListener().NotEnoughResources(data1, count1, data2, count2, command, unk);
					}
				}
			}

			return enough;
		}

		public int GetDiamonds()
			=> m_diamonds;

		public void SetDiamonds(int count)
		{
			m_diamonds = count;
		}

		public void UseDiamonds(int count)
		{
			m_diamonds -= count;

			if (m_freeDiamonds > m_diamonds)
			{
				m_freeDiamonds = m_diamonds;
			}
		}

		public int GetFreeDiamonds()
			=> m_freeDiamonds;

		public void SetFreeDiamonds(int count)
		{
			m_freeDiamonds = count;
		}

		public void AddCumulativePurchasedDiamonds(int count)
		{
			m_cumulativePurchasedDiamonds += count;
		}

		public int GetCumulativePurchasedDiamonds()
			=> m_cumulativePurchasedDiamonds;

		public int GetLeagueType()
			=> LogicMath.Clamp(m_leagueType, 0, LogicDataTables.GetTable(DataType.LEAGUE).GetItemCount() - 1);

		public void SetLeagueType(int value)
		{
			m_leagueType = value;
		}

		public int GetAttackWinCount()
			=> m_attackWinCount;

		public void SetAttackWinCount(int value)
		{
			m_attackWinCount = value;
		}

		public int GetAttackLoseCount()
			=> m_attackLoseCount;

		public void SetAttackLoseCount(int value)
		{
			m_attackLoseCount = value;
		}

		public int GetDefenseWinCount()
			=> m_defenseWinCount;

		public void SetDefenseWinCount(int value)
		{
			m_defenseWinCount = value;
		}

		public int GetDefenseLoseCount()
			=> m_defenseLoseCount;

		public void SetDefenseLoseCount(int value)
		{
			m_defenseLoseCount = value;
		}

		public int GetDuelWinCount()
			=> m_duelWinCount;

		public void SetDuelWinCount(int value)
		{
			m_duelWinCount = value;
		}

		public int GetDuelLoseCount()
			=> m_duelLoseCount;

		public void SetDuelLoseCount(int value)
		{
			m_duelLoseCount = value;
		}

		public int GetDuelDrawCount()
			=> m_duelDrawCount;

		public void SetDuelDrawCount(int value)
		{
			m_duelDrawCount = value;
		}

		public LogicLong GetLeagueInstanceId()
			=> m_leagueInstanceId;

		public void SetLeagueInstanceId(LogicLong id)
		{
			m_leagueInstanceId = id;
		}

		public int GetAttackShieldReduceCounter()
			=> m_attackShieldReduceCounter;

		public void SetAttackShieldReduceCounter(int value)
		{
			m_attackShieldReduceCounter = value;
		}

		public int GetDefenseVillageGuardCounter()
			=> m_defenseVillageGuardCounter;

		public void SetDefenseVillageGuardCounter(int value)
		{
			m_defenseVillageGuardCounter = value;
		}

		public int GetScore()
			=> m_score;

		public int GetDuelScore()
			=> m_duelScore;

		public void SetDuelScore(int score)
		{
			m_duelScore = score;
		}

		public void SetScore(int value)
		{
			m_score = value;
		}

		public int GetLegendaryScore()
			=> m_legendaryScore;

		public void SetLegendaryScore(int value)
		{
			m_legendaryScore = value;
		}

		public int GetLegendaryScoreVillage2()
			=> m_legendaryScoreVillage2;

		public void SetLegendaryScoreVillage2(int value)
		{
			m_legendaryScoreVillage2 = value;
		}

		public LogicLegendSeasonEntry GetLegendSeasonEntry()
			=> m_legendSeasonEntry;

		public LogicLegendSeasonEntry GetLegendSeasonEntryVillage2()
			=> m_legendSeasonEntryVillage2;

		public int GetChallengeState()
			=> m_challengeState;

		public void SetChallengeState(int value)
		{
			m_challengeState = value;
		}

		public LogicLong GetChallengeId()
			=> m_challengeId;

		public void SetChallengeId(LogicLong value)
		{
			m_challengeId = value;
		}

		public override int GetResourceCount(LogicResourceData data)
		{
			if (data.IsPremiumCurrency())
			{
				return m_diamonds;
			}

			return base.GetResourceCount(data);
		}

		public override LogicLeagueData GetLeagueTypeData()
		{
			LogicDataTable table = LogicDataTables.GetTable(DataType.LEAGUE);
			Debugger.DoAssert(m_leagueType > -1 && table.GetItemCount() > m_leagueType, "Player league ranking out of bounds");
			return (LogicLeagueData)table.GetItemAt(m_leagueType);
		}

		public override void XpGainHelper(int count)
		{
			if (count > 0)
			{
				int maxExpPoints = LogicDataTables.GetExperienceLevel(m_expLevel).GetMaxExpPoints();

				if (m_expLevel < LogicExperienceLevelData.GetLevelCap())
				{
					int gainExpPoints = m_expPoints + count;

					if (gainExpPoints >= maxExpPoints)
					{
						if (m_expLevel + 1 == LogicExperienceLevelData.GetLevelCap())
						{
							gainExpPoints = maxExpPoints;
						}

						gainExpPoints -= maxExpPoints;

						m_expLevel += 1;
						m_listener.ExpLevelGained(gainExpPoints);

						if (m_level != null)
						{
							if (m_level.GetPlayerAvatar() == this)
							{
								m_level.GetGameListener().LevelUp(m_expLevel);
							}

							if (m_level.GetHomeOwnerAvatar() == this)
							{
								m_level.RefreshNewShopUnlocksExp();
							}
						}
					}
					else
					{
						m_listener.ExpPointsGained(gainExpPoints);
					}

					m_expPoints = gainExpPoints;
				}
			}
		}

		public void RemoveUnitsVillage2()
		{
			LogicDataTable table = LogicDataTables.GetTable(DataType.CHARACTER);

			for (int i = 0; i < table.GetItemCount(); i++)
			{
				LogicCharacterData characterData = (LogicCharacterData)table.GetItemAt(i);

				if (characterData.GetVillageType() == 1)
				{
					SetUnitCountVillage2(characterData, 0);
					m_listener.CommodityCountChanged(7, characterData, 0);
				}
			}
		}

		public void AddMissionResourceReward(LogicResourceData resourceData, int count)
		{
			if (resourceData != null)
			{
				if (count > 0)
				{
					SetResourceCount(resourceData, GetResourceCount(resourceData) + count);
					m_listener.CommodityCountChanged(0, resourceData, count);
				}
			}
		}

		public override bool AddDuelReward(int goldCount, int elixirCount, int bonusGoldCount, int bonusElixirCount, LogicLong matchId)
		{
			if (goldCount > 0 || elixirCount > 0)
			{
				m_level.RefreshResourceCaps();
				SetVariableByName("LootLimitWinCount", GetVariableByName("LootLimitWinCount") + 1);

				int goldCap = GetResourceCap(LogicDataTables.GetGold2Data());
				int elixirCap = GetResourceCap(LogicDataTables.GetElixir2Data());

				if (GetVariableByName("LootLimitCooldown") != 1)
				{
					int lootLimitWinCount = GetVariableByName("LootLimitWinCount");

					if (lootLimitWinCount >= m_level.GetGameMode().GetConfiguration().GetDuelBonusLimitWinsPerDay())
					{
						StartLootLimitCooldown();

						if (bonusGoldCount > 0)
						{
							AddResource(0, LogicDataTables.GetGold2Data(), bonusGoldCount, goldCap);
						}

						if (bonusElixirCount > 0)
						{
							AddResource(0, LogicDataTables.GetElixir2Data(), bonusElixirCount, elixirCap);
						}
					}
				}

				if (goldCount != 0)
				{
					AddResource(0, LogicDataTables.GetGold2Data(), goldCount, goldCap);
				}

				if (elixirCount != 0)
				{
					AddResource(0, LogicDataTables.GetElixir2Data(), elixirCount, elixirCap);
				}
			}

			return true;
		}

		public override bool AddStarBonusReward(int goldCount, int elixirCount, int darkElixirCount)
		{
			int currentWarGoldCap = GetResourceCap(LogicDataTables.GetWarGoldData());
			int currentWarElixirCap = GetResourceCap(LogicDataTables.GetWarElixirData());
			int currentWarDarkElixirCap = GetResourceCap(LogicDataTables.GetWarDarkElixirData());

			m_level.RefreshResourceCaps();

			int updatedWarGoldCap = GetResourceCap(LogicDataTables.GetWarGoldData());
			int updatedWarElixirCap = GetResourceCap(LogicDataTables.GetWarElixirData());
			int updatedWarDarkElixirCap = GetResourceCap(LogicDataTables.GetWarDarkElixirData());

			if (goldCount != 0)
			{
				AddResource(0, LogicDataTables.GetWarGoldData(), goldCount, LogicMath.Max(currentWarGoldCap, updatedWarGoldCap));
			}

			if (elixirCount != 0)
			{
				AddResource(0, LogicDataTables.GetWarElixirData(), elixirCount, LogicMath.Max(currentWarElixirCap, updatedWarElixirCap));
			}

			if (darkElixirCount != 0 && IsDarkElixirUnlocked())
			{
				AddResource(0, LogicDataTables.GetWarDarkElixirData(), darkElixirCount, LogicMath.Max(currentWarDarkElixirCap, updatedWarDarkElixirCap));
			}
			else
			{
				darkElixirCount = 0;
			}

			m_level.GetGameListener().StarBonusAdded(goldCount, elixirCount, darkElixirCount);

			if (m_listener != null)
			{
				m_listener.StarBonusAdded(goldCount, elixirCount, darkElixirCount);
			}

			return true;
		}

		public override bool AddWarReward(int gold, int elixir, int darkElixir, int unk, LogicLong warInstanceId)
		{
			if (warInstanceId != null && !m_warInstanceId.Equals(warInstanceId))
			{
				m_treasuryGoldCount += gold;
				m_treasuryElixirCount += elixir;
				m_treasuryDarkElixirCount = darkElixir;

				m_warInstanceId = warInstanceId;

				return true;
			}

			return false;
		}

		public override void FastForwardLootLimit(int secs)
		{
			int remainingSecs = GetRemainingLootLimitTime();

			if (remainingSecs <= secs)
			{
				if (GetVariableByName("LootLimitCooldown") == 1)
				{
					RestartLootLimitTimer(secs - remainingSecs, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					SetVariableByName("LootLimitCooldown", 0);
					ResetLootLimitWinCount();
				}
				else
				{
					SetVariableByName("LootLimitTimerEndSubTick", m_level.GetLogicTime().GetTick());
				}
			}
			else
			{
				int endSubtick = GetVariableByName("LootLimitTimerEndSubTick");
				int endTimestamp = GetVariableByName("LootLimitTimerEndTimestamp");
				int logicTime = m_level.GetLogicTime().GetTick();
				int currentTimestamp = m_level.GetGameMode().GetStartTime();
				int remainingTime = 60 * (endTimestamp - currentTimestamp);

				if (endTimestamp < 1 || currentTimestamp == -1)
				{
					logicTime = endSubtick;
					remainingTime = -60 * secs;
				}

				endSubtick = logicTime + remainingTime;

				if (LogicDataTables.GetGlobals().ClampAvatarTimersToMax())
				{
					endSubtick = m_level.GetLogicTime().GetTick() + 60 * LogicMath.Clamp((endSubtick - m_level.GetLogicTime().GetTick()) / 60, 1,
																							  60 * LogicCalendar.GetDuelLootLimitCooldownInMinutes(
																								  m_level.GetGameMode().GetCalendar(),
																								  m_level.GetGameMode().GetConfiguration()));
				}

				SetVariableByName("LootLimitTimerEndSubTick", endSubtick);
			}
		}

		public override void FastForwardStarBonusLimit(int secs)
		{
			int remainingSecs = GetRemainingStarBonusTime();

			if (remainingSecs <= secs)
			{
				if (GetVariableByName("StarBonusCooldown") == 1)
				{
					RestartStartBonusLimitTimer(secs - remainingSecs, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					SetVariableByName("StarBonusCooldown", 0);
				}
				else
				{
					SetVariableByName("StarBonusTimerEndSubTick", m_level.GetLogicTime().GetTick());
				}
			}
			else
			{
				int endSubtick = GetVariableByName("StarBonusTimerEndSubTick");
				int endTimestamp = GetVariableByName("StarBonusTimerEndTimestep");
				int logicTime = m_level.GetLogicTime().GetTick();
				int currentTimestamp = m_level.GetGameMode().GetStartTime();
				int remainingTime = 60 * (endTimestamp - currentTimestamp);

				if (endTimestamp < 1 || currentTimestamp == -1)
				{
					logicTime = endSubtick;
					remainingTime = -60 * secs;
				}

				endSubtick = logicTime + remainingTime;

				if (LogicDataTables.GetGlobals().ClampAvatarTimersToMax())
				{
					endSubtick = m_level.GetLogicTime().GetTick() + 60 * LogicMath.Clamp((endSubtick - m_level.GetLogicTime().GetTick()) / 60, 1,
																							  60 * LogicCalendar.GetDuelLootLimitCooldownInMinutes(
																								  m_level.GetGameMode().GetCalendar(),
																								  m_level.GetGameMode().GetConfiguration()));
				}

				SetVariableByName("StarBonusTimerEndSubTick", endSubtick);
			}
		}

		public void AddResource(int commodityType, LogicResourceData resourceData, int gainCount, int resourceCap)
		{
			int resourceCount = GetResourceCount(resourceData);
			int newCount = LogicMath.Max(resourceCount + gainCount, 0);

			if (gainCount <= 0)
			{
				SetResourceCount(resourceData, newCount);
				GetChangeListener().CommodityCountChanged(commodityType, resourceData, newCount);
			}
			else
			{
				newCount = LogicMath.Min(newCount, resourceCap);
				resourceCount = LogicMath.Min(resourceCount, resourceCap);

				if (newCount > resourceCount)
				{
					SetResourceCount(resourceData, newCount);
					GetChangeListener().CommodityCountChanged(commodityType, resourceData, newCount);
				}
			}
		}

		public void StartLootLimitCooldown()
		{
			int lootLimitFreeSpeedUp = GetVariableByName("LootLimitFreeSpeedUp");

			if (lootLimitFreeSpeedUp >= LogicDataTables.GetGlobals().GetDuelLootLimitFreeSpeedUps())
			{
				if (GetRemainingLootLimitTime() <= 0)
				{
					RestartLootLimitTimer(0, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					ResetLootLimitWinCount();
				}
				else
				{
					SetVariableByName("LootLimitCooldown", 1);
				}
			}
			else
			{
				RestartLootLimitTimer(0, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
				ResetLootLimitWinCount();
				SetVariableByName("LootLimitFreeSpeedUp", lootLimitFreeSpeedUp + 1);
			}
		}

		public int GetRemainingLootLimitTime()
		{
			int remainingSubtick = GetVariableByName("LootLimitTimerEndSubTick") - m_level.GetLogicTime().GetTick();

			if (remainingSubtick <= 0)
			{
				return 0;
			}

			return LogicMath.Max(1, (remainingSubtick + 59) / 60);
		}

		public int GetRemainingStarBonusTime()
		{
			int remainingSubtick = GetVariableByName("StarBonusTimerEndSubTick") - m_level.GetLogicTime().GetTick();

			if (remainingSubtick <= 0)
			{
				return 0;
			}

			return LogicMath.Max(1, (remainingSubtick + 59) / 60);
		}

		public void RestartLootLimitTimer(int passedSecs, int timestamp)
		{
			int secs = LogicCalendar.GetDuelLootLimitCooldownInMinutes(m_level.GetGameMode().GetCalendar(), m_level.GetGameMode().GetConfiguration()) * 60 - passedSecs;

			if (secs <= passedSecs)
			{
				SetVariableByName("LootLimitTimerEndSubTick", m_level.GetLogicTime().GetTick());
			}
			else
			{
				SetVariableByName("LootLimitTimerEndSubTick", m_level.GetLogicTime().GetTick() + secs * 60);

				if (timestamp != -1)
				{
					SetVariableByName("LootLimitTimerEndTimestamp", timestamp + secs);
				}
			}
		}

		public void RestartStartBonusLimitTimer(int passedSecs, int timestamp)
		{
			int secs = LogicDataTables.GetGlobals().GetStarBonusCooldownMinutes() * 60 - passedSecs;

			if (secs <= passedSecs)
			{
				SetVariableByName("StarBonusTimerEndSubTick", m_level.GetLogicTime().GetTick());
			}
			else
			{
				SetVariableByName("StarBonusTimerEndSubTick", m_level.GetLogicTime().GetTick() + secs * 60);

				if (timestamp != -1)
				{
					SetVariableByName("StarBonusTimerEndTimestep", timestamp + secs);
				}
			}
		}

		public override void UpdateLootLimitCooldown()
		{
			if (GetRemainingLootLimitTime() <= 0)
			{
				if (GetVariableByName("LootLimitCooldown") == 1)
				{
					RestartLootLimitTimer(0, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					SetVariableByName("LootLimitCooldown", 0);
					ResetLootLimitWinCount();
				}
			}
		}

		public override void UpdateStarBonusLimitCooldown()
		{
			if (GetRemainingStarBonusTime() <= 0)
			{
				if (GetVariableByName("StarBonusCooldown") == 1)
				{
					RestartStartBonusLimitTimer(0, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					SetVariableByName("StarBonusCooldown", 0);
				}
			}
		}

		public void ResetLootLimitWinCount()
		{
			SetVariableByName("LootLimitWinCount", 0);
		}

		public int GetStarBonusCounter()
			=> GetVariableByName("StarBonusCounter");

		public int GetLootLimitWinCount()
			=> GetVariableByName("LootLimitWinCount");

		public void SetStarBonusCounter(int count)
		{
			SetVariableByName("StarBonusCounter", count);
		}

		public bool GetStarBonusCooldown()
			=> GetVariableByName("StarBonusCooldown") == 1;

		public bool GetLootLimitCooldown()
			=> GetVariableByName("LootLimitCooldown") == 1;

		public int GetTroopRequestCooldown()
		{
			if (IsInAlliance() && m_allianceExpLevel > 0)
			{
				LogicAllianceLevelData allianceLevelData = LogicDataTables.GetAllianceLevel(m_allianceExpLevel);

				if (allianceLevelData != null)
				{
					return allianceLevelData.GetTroopRequestCooldown() * 60;
				}
			}

			return LogicDataTables.GetGlobals().GetAllianceTroopRequestCooldown();
		}

		public int GetTroopDonationRefund()
		{
			if (IsInAlliance() && m_allianceExpLevel > 0)
			{
				LogicAllianceLevelData allianceLevelData = LogicDataTables.GetAllianceLevel(m_allianceExpLevel);

				if (allianceLevelData != null)
				{
					return allianceLevelData.GetTroopDonationRefund();
				}
			}

			return 0;
		}

		public void SetLastUsedArmy(LogicArrayList<LogicDataSlot> unitCount, LogicArrayList<LogicDataSlot> spellCount)
		{
			for (int i = 0; i < unitCount.Size(); i++)
			{
				LogicDataSlot slot = unitCount[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				int count = slot.GetCount();

				if (GetUnitPresetCount(data, 0) != count)
				{
					SetCommodityCount(2, data, count);
					GetChangeListener().CommodityCountChanged(2, data, count);
				}
			}

			for (int i = 0; i < spellCount.Size(); i++)
			{
				LogicDataSlot slot = spellCount[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				int count = slot.GetCount();

				if (GetUnitPresetCount(data, 0) != count)
				{
					SetCommodityCount(2, data, count);
					GetChangeListener().CommodityCountChanged(2, data, count);
				}
			}
		}

		public void StarBonusCollected()
		{
			int starBonusTimesCollected = GetVariableByName("StarBonusTimesCollected");
			SetVariableByName("StarBonusTimesCollected", starBonusTimesCollected + 1);

			if (starBonusTimesCollected == 0)
			{
				RestartStartBonusLimitTimer(0, m_level.GetHomeOwnerAvatar().GetChangeListener().GetCurrentTimestamp());
			}

			if (GetRemainingStarBonusTime() <= 0)
			{
				RestartStartBonusLimitTimer(0, m_level.GetHomeOwnerAvatar().GetChangeListener().GetCurrentTimestamp());
			}
			else
			{
				SetVariableByName("StarBonusCooldown", 1);
			}
		}

		public void Decode(ByteStream stream)
		{
			m_id = stream.ReadLong();
			m_currentHomeId = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				m_allianceId = stream.ReadLong();
				m_allianceName = stream.ReadString(900000);
				m_allianceBadgeId = stream.ReadInt();
				m_allianceRole = (LogicAvatarAllianceRole)stream.ReadInt();
				m_allianceExpLevel = stream.ReadInt();
			}

			if (stream.ReadBoolean())
			{
				m_leagueInstanceId = stream.ReadLong();
			}

			m_legendaryScore = stream.ReadInt();
			m_legendaryScoreVillage2 = stream.ReadInt();
			m_legendSeasonEntry.Decode(stream);
			m_legendSeasonEntryVillage2.Decode(stream);

			m_duelWinCount = stream.ReadInt();
			m_duelLoseCount = stream.ReadInt();
			m_duelDrawCount = stream.ReadInt();
			m_leagueType = stream.ReadInt();
			m_allianceCastleLevel = stream.ReadInt();
			m_allianceCastleTotalCapacity = stream.ReadInt();
			m_allianceCastleUsedCapacity = stream.ReadInt();
			m_allianceCastleTotalSpellCapacity = stream.ReadInt();
			m_allianceCastleUsedSpellCapacity = stream.ReadInt();

			m_townHallLevel = stream.ReadInt();
			m_townHallLevelVillage2 = stream.ReadInt();

			m_name = stream.ReadString(900000);
			m_facebookId = stream.ReadString(900000);

			m_expLevel = stream.ReadInt();
			m_expPoints = stream.ReadInt();
			m_diamonds = stream.ReadInt();
			m_freeDiamonds = stream.ReadInt();
			m_attackRating = stream.ReadInt();
			m_attackKFactor = stream.ReadInt();
			m_score = stream.ReadInt();
			m_duelScore = stream.ReadInt();
			m_attackWinCount = stream.ReadInt();
			m_attackLoseCount = stream.ReadInt();
			m_defenseWinCount = stream.ReadInt();
			m_defenseLoseCount = stream.ReadInt();
			m_treasuryGoldCount = stream.ReadInt();
			m_treasuryElixirCount = stream.ReadInt();
			m_treasuryDarkElixirCount = stream.ReadInt();

			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				stream.ReadLong();
			}

			m_nameSetByUser = stream.ReadBoolean();
			m_allianceChatFilter = stream.ReadBoolean();
			m_nameChangeState = stream.ReadInt();
			m_cumulativePurchasedDiamonds = stream.ReadInt();
			m_redPackageState = stream.ReadInt();
			m_warPreference = stream.ReadInt();
			m_attackShieldReduceCounter = stream.ReadInt();
			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_challengeState = stream.ReadInt();
				m_challengeId = stream.ReadLong();
			}

			ClearDataSlotArray(m_resourceCap);
			ClearDataSlotArray(m_resourceCount);
			ClearDataSlotArray(m_unitCount);
			ClearDataSlotArray(m_spellCount);
			ClearDataSlotArray(m_unitUpgrade);
			ClearDataSlotArray(m_spellUpgrade);
			ClearDataSlotArray(m_heroUpgrade);
			ClearDataSlotArray(m_heroHealth);
			ClearDataSlotArray(m_heroState);
			ClearUnitSlotArray(m_allianceUnitCount);
			ClearDataSlotArray(m_achievementProgress);
			ClearDataSlotArray(m_npcStars);
			ClearDataSlotArray(m_lootedNpcGold);
			ClearDataSlotArray(m_lootedNpcElixir);
			ClearDataSlotArray(m_heroMode);
			ClearDataSlotArray(m_variables);
			ClearDataSlotArray(m_unitPreset1);
			ClearDataSlotArray(m_unitPreset2);
			ClearDataSlotArray(m_unitPreset3);
			ClearDataSlotArray(m_previousArmy);
			ClearDataSlotArray(m_eventUnitCounter);
			ClearDataSlotArray(m_unitCountVillage2);
			ClearDataSlotArray(m_unitCountNewVillage2);
			ClearDataSlotArray(m_freeActionCount);

			m_missionCompleted.Clear();
			m_achievementRewardClaimed.Clear();

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);
				slot.Decode(stream);
				m_resourceCap.Add(slot);
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_resourceCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - resource slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unit slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_spellCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - spell slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitUpgrade.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unit upgrade slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_spellUpgrade.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - spell upgrade slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_heroUpgrade.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - hero upgrade slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_heroHealth.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - hero health slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_heroState.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - hero state slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicUnitSlot slot = new LogicUnitSlot(null, 0, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_allianceUnitCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - alliance unit data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicMissionData data = (LogicMissionData)ByteStreamHelper.ReadDataReference(stream, DataType.MISSION);

				if (data != null)
				{
					m_missionCompleted.Add(data);
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicAchievementData data = (LogicAchievementData)ByteStreamHelper.ReadDataReference(stream, DataType.ACHIEVEMENT);

				if (data != null)
				{
					m_achievementRewardClaimed.Add(data);
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_achievementProgress.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - achievement progress data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_npcStars.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - npc map progress data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_lootedNpcGold.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - npc looted gold data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_lootedNpcElixir.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - npc looted elixir data is NULL");
				}
			}

			m_allianceUnitVisitCapacity = stream.ReadInt();
			m_allianceUnitSpellVisitCapacity = stream.ReadInt();

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_heroMode.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - hero mode slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_variables.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - variables data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitPreset1.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unitPreset1 data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitPreset2.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unitPreset2 data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitPreset3.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unitPreset3 data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_previousArmy.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - previousArmySize data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_eventUnitCounter.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unitCounterForEvent data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitCountVillage2.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unit village2 slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitCountNewVillage2.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - unit village2 new slot data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);

				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_freeActionCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicClientAvatar::decode - slot data is NULL");
				}
			}
		}

		public void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_id);
			encoder.WriteLong(m_currentHomeId);

			if (m_allianceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_allianceId);
				encoder.WriteString(m_allianceName);
				encoder.WriteInt(m_allianceBadgeId);
				encoder.WriteInt((int)m_allianceRole);
				encoder.WriteInt(m_allianceExpLevel);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			if (m_leagueInstanceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_leagueInstanceId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteInt(m_legendaryScore);
			encoder.WriteInt(m_legendaryScoreVillage2);

			m_legendSeasonEntry.Encode(encoder);
			m_legendSeasonEntryVillage2.Encode(encoder);

			encoder.WriteInt(m_duelWinCount);
			encoder.WriteInt(m_duelLoseCount);
			encoder.WriteInt(m_duelDrawCount);

			encoder.WriteInt(m_leagueType);
			encoder.WriteInt(m_allianceCastleLevel);
			encoder.WriteInt(m_allianceCastleTotalCapacity);
			encoder.WriteInt(m_allianceCastleUsedCapacity);
			encoder.WriteInt(m_allianceCastleTotalSpellCapacity);
			encoder.WriteInt(m_allianceCastleUsedSpellCapacity);

			encoder.WriteInt(m_townHallLevel);
			encoder.WriteInt(m_townHallLevelVillage2);

			encoder.WriteString(m_name);
			encoder.WriteString(m_facebookId);

			encoder.WriteInt(m_expLevel);
			encoder.WriteInt(m_expPoints);
			encoder.WriteInt(m_diamonds);
			encoder.WriteInt(m_freeDiamonds);
			encoder.WriteInt(m_attackRating);
			encoder.WriteInt(m_attackKFactor);
			encoder.WriteInt(m_score);
			encoder.WriteInt(m_duelScore);
			encoder.WriteInt(m_attackWinCount);
			encoder.WriteInt(m_attackLoseCount);
			encoder.WriteInt(m_defenseWinCount);
			encoder.WriteInt(m_defenseLoseCount);
			encoder.WriteInt(m_treasuryGoldCount);
			encoder.WriteInt(m_treasuryElixirCount);
			encoder.WriteInt(m_treasuryDarkElixirCount);
			encoder.WriteInt(0);

			if (m_warInstanceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_warInstanceId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteBoolean(m_nameSetByUser);
			encoder.WriteBoolean(m_allianceChatFilter);
			encoder.WriteInt(m_nameChangeState);
			encoder.WriteInt(m_cumulativePurchasedDiamonds);
			encoder.WriteInt(m_redPackageState);
			encoder.WriteInt(m_warPreference);
			encoder.WriteInt(m_attackShieldReduceCounter);
			encoder.WriteInt(0);

			if (m_challengeId != null)
			{
				encoder.WriteBoolean(true);

				encoder.WriteInt(m_challengeState);
				encoder.WriteLong(m_challengeId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteInt(m_resourceCap.Size());

			for (int i = 0; i < m_resourceCap.Size(); i++)
			{
				m_resourceCap[i].Encode(encoder);
			}

			encoder.WriteInt(m_resourceCount.Size());

			for (int i = 0; i < m_resourceCount.Size(); i++)
			{
				m_resourceCount[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitCount.Size());

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				m_unitCount[i].Encode(encoder);
			}

			encoder.WriteInt(m_spellCount.Size());

			for (int i = 0; i < m_spellCount.Size(); i++)
			{
				m_spellCount[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitUpgrade.Size());

			for (int i = 0; i < m_unitUpgrade.Size(); i++)
			{
				m_unitUpgrade[i].Encode(encoder);
			}

			encoder.WriteInt(m_spellUpgrade.Size());

			for (int i = 0; i < m_spellUpgrade.Size(); i++)
			{
				m_spellUpgrade[i].Encode(encoder);
			}

			encoder.WriteInt(m_heroUpgrade.Size());

			for (int i = 0; i < m_heroUpgrade.Size(); i++)
			{
				m_heroUpgrade[i].Encode(encoder);
			}

			encoder.WriteInt(m_heroHealth.Size());

			for (int i = 0; i < m_heroHealth.Size(); i++)
			{
				m_heroHealth[i].Encode(encoder);
			}

			encoder.WriteInt(m_heroState.Size());

			for (int i = 0; i < m_heroState.Size(); i++)
			{
				m_heroState[i].Encode(encoder);
			}

			encoder.WriteInt(m_allianceUnitCount.Size());

			for (int i = 0; i < m_allianceUnitCount.Size(); i++)
			{
				m_allianceUnitCount[i].Encode(encoder);
			}

			encoder.WriteInt(m_missionCompleted.Size());

			for (int i = 0; i < m_missionCompleted.Size(); i++)
			{
				ByteStreamHelper.WriteDataReference(encoder, m_missionCompleted[i]);
			}

			encoder.WriteInt(m_achievementRewardClaimed.Size());

			for (int i = 0; i < m_achievementRewardClaimed.Size(); i++)
			{
				ByteStreamHelper.WriteDataReference(encoder, m_achievementRewardClaimed[i]);
			}

			encoder.WriteInt(m_achievementProgress.Size());

			for (int i = 0; i < m_achievementProgress.Size(); i++)
			{
				m_achievementProgress[i].Encode(encoder);
			}

			encoder.WriteInt(m_npcStars.Size());

			for (int i = 0; i < m_npcStars.Size(); i++)
			{
				m_npcStars[i].Encode(encoder);
			}

			encoder.WriteInt(m_lootedNpcGold.Size());

			for (int i = 0; i < m_lootedNpcGold.Size(); i++)
			{
				m_lootedNpcGold[i].Encode(encoder);
			}

			encoder.WriteInt(m_lootedNpcElixir.Size());

			for (int i = 0; i < m_lootedNpcElixir.Size(); i++)
			{
				m_lootedNpcElixir[i].Encode(encoder);
			}

			encoder.WriteInt(m_allianceUnitVisitCapacity);
			encoder.WriteInt(m_allianceUnitSpellVisitCapacity);

			encoder.WriteInt(m_heroMode.Size());

			for (int i = 0; i < m_heroMode.Size(); i++)
			{
				m_heroMode[i].Encode(encoder);
			}

			encoder.WriteInt(m_variables.Size());

			for (int i = 0; i < m_variables.Size(); i++)
			{
				m_variables[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitPreset1.Size());

			for (int i = 0; i < m_unitPreset1.Size(); i++)
			{
				m_unitPreset1[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitPreset2.Size());

			for (int i = 0; i < m_unitPreset2.Size(); i++)
			{
				m_unitPreset2[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitPreset3.Size());

			for (int i = 0; i < m_unitPreset3.Size(); i++)
			{
				m_unitPreset3[i].Encode(encoder);
			}

			encoder.WriteInt(m_previousArmy.Size());

			for (int i = 0; i < m_previousArmy.Size(); i++)
			{
				m_previousArmy[i].Encode(encoder);
			}

			encoder.WriteInt(m_eventUnitCounter.Size());

			for (int i = 0; i < m_eventUnitCounter.Size(); i++)
			{
				m_eventUnitCounter[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitCountVillage2.Size());

			for (int i = 0; i < m_unitCountVillage2.Size(); i++)
			{
				m_unitCountVillage2[i].Encode(encoder);
			}

			encoder.WriteInt(m_unitCountNewVillage2.Size());

			for (int i = 0; i < m_unitCountNewVillage2.Size(); i++)
			{
				m_unitCountNewVillage2[i].Encode(encoder);
			}

			encoder.WriteInt(m_freeActionCount.Size());

			for (int i = 0; i < m_freeActionCount.Size(); i++)
			{
				m_freeActionCount[i].Encode(encoder);
			}
		}

		public void Load(LogicJSONObject jsonObject)
		{
			LogicJSONString nameObject = jsonObject.GetJSONString("name");

			if (nameObject != null)
			{
				m_name = nameObject.GetStringValue();
			}

			LogicJSONBoolean nameSetObject = jsonObject.GetJSONBoolean("name_set");

			if (nameSetObject != null)
			{
				m_nameSetByUser = nameSetObject.IsTrue();
			}

			LogicJSONNumber nameChangeStateObject = jsonObject.GetJSONNumber("name_change_state");

			if (nameChangeStateObject != null)
			{
				m_nameChangeState = nameChangeStateObject.GetIntValue();
			}

			LogicJSONNumber badgeIdObject = jsonObject.GetJSONNumber("badge_id");

			if (badgeIdObject != null)
			{
				m_allianceBadgeId = badgeIdObject.GetIntValue();
			}

			LogicJSONNumber allianceExpLevelObject = jsonObject.GetJSONNumber("alliance_exp_level");

			if (allianceExpLevelObject != null)
			{
				m_allianceExpLevel = allianceExpLevelObject.GetIntValue();
			}

			if (m_allianceBadgeId == -1)
			{
				m_allianceId = null;
			}
			else
			{
				LogicJSONNumber allianceIdLowObject = jsonObject.GetJSONNumber("alliance_id_low");
				LogicJSONNumber allianceIdHighObject = jsonObject.GetJSONNumber("alliance_id_high");

				int allIdHigh = -1;
				int allIdLow = -1;

				if (allianceIdHighObject != null && allianceIdLowObject != null)
				{
					allIdHigh = allianceIdHighObject.GetIntValue();
					allIdLow = allianceIdLowObject.GetIntValue();
				}

				m_allianceId = new LogicLong(allIdHigh, allIdLow);
				m_allianceName = LogicJSONHelper.GetString(jsonObject, "alliance_name");
				m_allianceRole = (LogicAvatarAllianceRole)LogicJSONHelper.GetInt(jsonObject, "alliance_role");
			}

			LogicJSONNumber leagueIdLowObject = jsonObject.GetJSONNumber("league_id_low");
			LogicJSONNumber leagueIdHighObject = jsonObject.GetJSONNumber("league_id_high");

			if (leagueIdHighObject != null && leagueIdLowObject != null)
			{
				m_leagueInstanceId = new LogicLong(leagueIdHighObject.GetIntValue(), leagueIdLowObject.GetIntValue());
			}

			m_allianceUnitVisitCapacity = LogicJSONHelper.GetInt(jsonObject, "alliance_unit_visit_capacity", 0);
			m_allianceUnitSpellVisitCapacity = LogicJSONHelper.GetInt(jsonObject, "alliance_unit_spell_visit_capacity", 0);
			m_expLevel = LogicJSONHelper.GetInt(jsonObject, "xp_level", 0);
			m_expPoints = LogicJSONHelper.GetInt(jsonObject, "xp_points", 0);
			m_diamonds = LogicJSONHelper.GetInt(jsonObject, "diamonds", 0);
			m_freeDiamonds = LogicJSONHelper.GetInt(jsonObject, "free_diamonds", 0);

			m_leagueType = LogicJSONHelper.GetInt(jsonObject, "league_type", 0);
			m_legendaryScore = LogicJSONHelper.GetInt(jsonObject, "legendary_score", 0);
			m_legendaryScoreVillage2 = LogicJSONHelper.GetInt(jsonObject, "legendary_score_v2", 0);

			LogicJSONObject legendLeagueEntry = jsonObject.GetJSONObject("legend_league_entry");

			if (legendLeagueEntry != null)
			{
				m_legendSeasonEntry.ReadFromJSON(legendLeagueEntry);
			}

			LogicJSONObject legendLeagueEntryV2 = jsonObject.GetJSONObject("legend_league_entry_v2");

			if (legendLeagueEntryV2 != null)
			{
				m_legendSeasonEntryVillage2.ReadFromJSON(legendLeagueEntryV2);
			}

			LoadDataSlotArray(jsonObject, "units", m_unitCount);
			LoadDataSlotArray(jsonObject, "spells", m_spellCount);
			LoadDataSlotArray(jsonObject, "unit_upgrades", m_unitUpgrade);
			LoadDataSlotArray(jsonObject, "spell_upgrades", m_spellUpgrade);
			LoadDataSlotArray(jsonObject, "resources", m_resourceCount);
			LoadDataSlotArray(jsonObject, "resource_caps", m_resourceCap);
			LoadUnitSlotArray(jsonObject, "alliance_units", m_allianceUnitCount);
			LoadDataSlotArray(jsonObject, "hero_states", m_heroState);
			LoadDataSlotArray(jsonObject, "hero_health", m_heroHealth);
			LoadDataSlotArray(jsonObject, "hero_upgrade", m_heroUpgrade);
			LoadDataSlotArray(jsonObject, "hero_modes", m_heroMode);
			LoadDataSlotArray(jsonObject, "variables", m_variables);
			LoadDataSlotArray(jsonObject, "units2", m_unitCountVillage2);
			LoadDataSlotArray(jsonObject, "units_new2", m_unitCountNewVillage2);
			LoadDataSlotArray(jsonObject, "unit_preset1", m_unitPreset1);
			LoadDataSlotArray(jsonObject, "unit_preset2", m_unitPreset2);
			LoadDataSlotArray(jsonObject, "unit_preset3", m_unitPreset3);
			LoadDataSlotArray(jsonObject, "previous_army", m_previousArmy);
			LoadDataSlotArray(jsonObject, "event_unit_counter", m_eventUnitCounter);
			LoadDataSlotArray(jsonObject, "looted_npc_gold", m_lootedNpcGold);
			LoadDataSlotArray(jsonObject, "looted_npc_elixir", m_lootedNpcElixir);
			LoadDataSlotArray(jsonObject, "npc_stars", m_npcStars);
			LoadDataSlotArray(jsonObject, "achievement_progress", m_achievementProgress);

			LogicJSONArray achievementRewardClaimedArray = jsonObject.GetJSONArray("achievement_rewards");

			if (achievementRewardClaimedArray != null)
			{
				if (achievementRewardClaimedArray.Size() != 0)
				{
					m_achievementRewardClaimed.Clear();
					m_achievementRewardClaimed.EnsureCapacity(achievementRewardClaimedArray.Size());

					for (int i = 0; i < achievementRewardClaimedArray.Size(); i++)
					{
						LogicJSONNumber id = achievementRewardClaimedArray.GetJSONNumber(i);

						if (id != null)
						{
							LogicData data = LogicDataTables.GetDataById(id.GetIntValue());

							if (data != null)
							{
								m_achievementRewardClaimed.Add(data);
							}
						}
					}
				}
			}

			LogicJSONArray missionCompletedArray = jsonObject.GetJSONArray("missions");

			if (missionCompletedArray != null)
			{
				if (missionCompletedArray.Size() != 0)
				{
					m_missionCompleted.Clear();
					m_missionCompleted.EnsureCapacity(missionCompletedArray.Size());

					for (int i = 0; i < missionCompletedArray.Size(); i++)
					{
						LogicJSONNumber id = missionCompletedArray.GetJSONNumber(i);

						if (id != null)
						{
							LogicData data = LogicDataTables.GetDataById(id.GetIntValue());

							if (data != null)
							{
								m_missionCompleted.Add(data);
							}
						}
					}
				}
			}

			m_allianceCastleLevel = LogicJSONHelper.GetInt(jsonObject, "castle_lvl", -1);
			m_allianceCastleTotalCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_total", 0);
			m_allianceCastleUsedCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_used", 0);
			m_allianceCastleTotalSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_total_sp", 0);
			m_allianceCastleUsedSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_used_sp", 0);
			m_townHallLevel = LogicJSONHelper.GetInt(jsonObject, "town_hall_lvl", 0);
			m_townHallLevelVillage2 = LogicJSONHelper.GetInt(jsonObject, "th_v2_lvl", 0);
			m_score = LogicJSONHelper.GetInt(jsonObject, "score", 0);
			m_duelScore = LogicJSONHelper.GetInt(jsonObject, "duel_score", 0);
			m_warPreference = LogicJSONHelper.GetInt(jsonObject, "war_preference", 0);
			m_attackRating = LogicJSONHelper.GetInt(jsonObject, "attack_rating", 0);
			m_attackKFactor = LogicJSONHelper.GetInt(jsonObject, "atack_kfactor", 0);
			m_attackWinCount = LogicJSONHelper.GetInt(jsonObject, "attack_win_cnt", 0);
			m_attackLoseCount = LogicJSONHelper.GetInt(jsonObject, "attack_lose_cnt", 0);
			m_defenseWinCount = LogicJSONHelper.GetInt(jsonObject, "defense_win_cnt", 0);
			m_defenseLoseCount = LogicJSONHelper.GetInt(jsonObject, "defense_lose_cnt", 0);
			m_treasuryGoldCount = LogicJSONHelper.GetInt(jsonObject, "treasury_gold_cnt", 0);
			m_treasuryElixirCount = LogicJSONHelper.GetInt(jsonObject, "treasury_elixir_cnt", 0);
			m_treasuryDarkElixirCount = LogicJSONHelper.GetInt(jsonObject, "treasury_dark_elixir_cnt", 0);
			m_redPackageState = LogicJSONHelper.GetInt(jsonObject, "red_package_state", 0);
		}

		public override void LoadForReplay(LogicJSONObject jsonObject, bool direct)
		{
			LogicJSONNumber avatarIdLowObject = jsonObject.GetJSONNumber("avatar_id_low");
			LogicJSONNumber avatarIdHighObject = jsonObject.GetJSONNumber("avatar_id_high");

			if (avatarIdHighObject != null)
			{
				if (avatarIdLowObject != null)
				{
					m_id = new LogicLong(avatarIdHighObject.GetIntValue(), avatarIdLowObject.GetIntValue());
				}
			}

			LogicJSONString nameObject = jsonObject.GetJSONString("name");

			if (nameObject != null)
			{
				m_name = nameObject.GetStringValue();
			}

			LogicJSONNumber badgeIdObject = jsonObject.GetJSONNumber("badge_id");

			if (badgeIdObject != null)
			{
				m_allianceBadgeId = badgeIdObject.GetIntValue();
			}

			LogicJSONNumber allianceExpLevelObject = jsonObject.GetJSONNumber("alliance_exp_level");

			if (allianceExpLevelObject != null)
			{
				m_allianceExpLevel = allianceExpLevelObject.GetIntValue();
			}

			if (m_allianceBadgeId == -1)
			{
				m_allianceId = null;
			}
			else
			{
				LogicJSONNumber allianceIdLowObject = jsonObject.GetJSONNumber("alliance_id_low");
				LogicJSONNumber allianceIdHighObject = jsonObject.GetJSONNumber("alliance_id_high");

				int allIdHigh = -1;
				int allIdLow = -1;

				if (allianceIdHighObject != null)
				{
					if (allianceIdLowObject != null)
					{
						allIdHigh = allianceIdHighObject.GetIntValue();
						allIdLow = allianceIdLowObject.GetIntValue();
					}
				}

				m_allianceId = new LogicLong(allIdHigh, allIdLow);
				m_allianceName = LogicJSONHelper.GetString(jsonObject, "alliance_name");
			}

			m_allianceUnitVisitCapacity = LogicJSONHelper.GetInt(jsonObject, "alliance_unit_visit_capacity", 0);
			m_allianceUnitSpellVisitCapacity = LogicJSONHelper.GetInt(jsonObject, "alliance_unit_spell_visit_capacity", 0);
			m_leagueType = LogicJSONHelper.GetInt(jsonObject, "league_type", 0);
			m_expLevel = LogicJSONHelper.GetInt(jsonObject, "xp_level", 1);

			if (!direct)
			{
				LoadDataSlotArray(jsonObject, "units", m_unitCount);
				LoadDataSlotArray(jsonObject, "spells", m_spellCount);
				LoadDataSlotArray(jsonObject, "unit_upgrades", m_unitUpgrade);
				LoadDataSlotArray(jsonObject, "spell_upgrades", m_spellUpgrade);
			}

			LoadDataSlotArray(jsonObject, "resources", m_resourceCount);
			LoadUnitSlotArray(jsonObject, "alliance_units", m_allianceUnitCount);
			LoadDataSlotArray(jsonObject, "hero_states", m_heroState);
			LoadDataSlotArray(jsonObject, "hero_health", m_heroHealth);
			LoadDataSlotArray(jsonObject, "hero_upgrade", m_heroUpgrade);
			LoadDataSlotArray(jsonObject, "hero_modes", m_heroMode);
			LoadDataSlotArray(jsonObject, "variables", m_variables);

			if (!direct)
			{
				LoadDataSlotArray(jsonObject, "units2", m_unitCountVillage2);
			}

			m_allianceCastleLevel = LogicJSONHelper.GetInt(jsonObject, "castle_lvl", -1);
			m_allianceCastleTotalCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_total", 0);
			m_allianceCastleUsedCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_used", 0);
			m_allianceCastleTotalSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_total_sp", 0);
			m_allianceCastleUsedSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_used_sp", 0);
			m_townHallLevel = LogicJSONHelper.GetInt(jsonObject, "town_hall_lvl", -1);
			m_townHallLevelVillage2 = LogicJSONHelper.GetInt(jsonObject, "th_v2_lvl", -1);
			m_score = LogicJSONHelper.GetInt(jsonObject, "score", 0);
			m_duelScore = LogicJSONHelper.GetInt(jsonObject, "duel_score", 0);
			m_redPackageState = LogicJSONHelper.GetInt(jsonObject, "red_package_state", 0);
		}

		private void LoadDataSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicDataSlot> dataSlotArray)
		{
			ClearDataSlotArray(dataSlotArray);

			LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

			if (jsonArray != null)
			{
				int arraySize = jsonArray.Size();

				if (arraySize != 0)
				{
					dataSlotArray.EnsureCapacity(arraySize);

					for (int i = 0; i < arraySize; i++)
					{
						LogicJSONObject obj = jsonArray.GetJSONObject(i);

						if (obj != null)
						{
							LogicDataSlot slot = new LogicDataSlot(null, 0);

							slot.ReadFromJSON(obj);

							if (slot.GetData() != null)
							{
								dataSlotArray.Add(slot);
							}
						}
					}
				}
			}
		}

		private void LoadUnitSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicUnitSlot> unitSlotArray)
		{
			ClearUnitSlotArray(unitSlotArray);

			LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

			if (jsonArray != null)
			{
				int arraySize = jsonArray.Size();

				if (arraySize != 0)
				{
					unitSlotArray.EnsureCapacity(arraySize);

					for (int i = 0; i < arraySize; i++)
					{
						LogicJSONObject obj = jsonArray.GetJSONObject(i);

						if (obj != null)
						{
							LogicUnitSlot slot = new LogicUnitSlot(null, 0, 0);

							slot.ReadFromJSON(obj);

							if (slot.GetData() != null)
							{
								unitSlotArray.Add(slot);
							}
						}
					}
				}
			}
		}

		public void Save(LogicJSONObject jsonObject)
		{
			jsonObject.Put("name", new LogicJSONString(m_name));
			jsonObject.Put("name_set", new LogicJSONBoolean(m_nameSetByUser));
			jsonObject.Put("name_change_state", new LogicJSONNumber(m_nameChangeState));
			jsonObject.Put("alliance_name", new LogicJSONString(m_allianceName ?? string.Empty));
			jsonObject.Put("xp_level", new LogicJSONNumber(m_expLevel));
			jsonObject.Put("xp_points", new LogicJSONNumber(m_expPoints));
			jsonObject.Put("diamonds", new LogicJSONNumber(m_diamonds));
			jsonObject.Put("free_diamonds", new LogicJSONNumber(m_freeDiamonds));

			if (m_allianceId != null)
			{
				jsonObject.Put("alliance_id_high", new LogicJSONNumber(m_allianceId.GetHigherInt()));
				jsonObject.Put("alliance_id_low", new LogicJSONNumber(m_allianceId.GetLowerInt()));
				jsonObject.Put("badge_id", new LogicJSONNumber(m_allianceBadgeId));
				jsonObject.Put("alliance_role", new LogicJSONNumber((int)m_allianceRole));
				jsonObject.Put("alliance_exp_level", new LogicJSONNumber(m_allianceExpLevel));
				jsonObject.Put("alliance_unit_visit_capacity", new LogicJSONNumber(m_allianceUnitVisitCapacity));
				jsonObject.Put("alliance_unit_spell_visit_capacity", new LogicJSONNumber(m_allianceUnitSpellVisitCapacity));
			}

			if (m_leagueInstanceId != null)
			{
				jsonObject.Put("league_id_high", new LogicJSONNumber(m_leagueInstanceId.GetHigherInt()));
				jsonObject.Put("league_id_low", new LogicJSONNumber(m_leagueInstanceId.GetLowerInt()));
			}

			jsonObject.Put("league_type", new LogicJSONNumber(m_leagueType));
			jsonObject.Put("legendary_score", new LogicJSONNumber(m_legendaryScore));
			jsonObject.Put("legendary_score_v2", new LogicJSONNumber(m_legendaryScoreVillage2));

			LogicJSONObject legendLeagueTournamentEntryObject = new LogicJSONObject();
			m_legendSeasonEntry.WriteToJSON(legendLeagueTournamentEntryObject);
			jsonObject.Put("legend_league_entry", legendLeagueTournamentEntryObject);

			LogicJSONObject legendLeagueTournamentEntryVillage2Object = new LogicJSONObject();
			m_legendSeasonEntryVillage2.WriteToJSON(legendLeagueTournamentEntryVillage2Object);
			jsonObject.Put("legend_league_entry_v2", legendLeagueTournamentEntryVillage2Object);

			SaveDataSlotArray(jsonObject, "units", m_unitCount);
			SaveDataSlotArray(jsonObject, "spells", m_spellCount);
			SaveDataSlotArray(jsonObject, "unit_upgrades", m_unitUpgrade);
			SaveDataSlotArray(jsonObject, "spell_upgrades", m_spellUpgrade);
			SaveDataSlotArray(jsonObject, "resources", m_resourceCount);
			SaveDataSlotArray(jsonObject, "resource_caps", m_resourceCap);
			SaveUnitSlotArray(jsonObject, "alliance_units", m_allianceUnitCount);
			SaveDataSlotArray(jsonObject, "hero_states", m_heroState);
			SaveDataSlotArray(jsonObject, "hero_health", m_heroHealth);
			SaveDataSlotArray(jsonObject, "hero_upgrade", m_heroUpgrade);
			SaveDataSlotArray(jsonObject, "hero_modes", m_heroMode);
			SaveDataSlotArray(jsonObject, "variables", m_variables);
			SaveDataSlotArray(jsonObject, "units2", m_unitCountVillage2);
			SaveDataSlotArray(jsonObject, "units_new2", m_unitCountNewVillage2);
			SaveDataSlotArray(jsonObject, "unit_preset1", m_unitPreset1);
			SaveDataSlotArray(jsonObject, "unit_preset2", m_unitPreset2);
			SaveDataSlotArray(jsonObject, "unit_preset3", m_unitPreset3);
			SaveDataSlotArray(jsonObject, "previous_army", m_previousArmy);
			SaveDataSlotArray(jsonObject, "event_unit_counter", m_eventUnitCounter);
			SaveDataSlotArray(jsonObject, "looted_npc_gold", m_lootedNpcGold);
			SaveDataSlotArray(jsonObject, "looted_npc_elixir", m_lootedNpcElixir);
			SaveDataSlotArray(jsonObject, "npc_stars", m_npcStars);
			SaveDataSlotArray(jsonObject, "achievement_progress", m_achievementProgress);

			LogicJSONArray achievementRewardClaimedArray = new LogicJSONArray();

			for (int i = 0; i < m_achievementRewardClaimed.Size(); i++)
			{
				achievementRewardClaimedArray.Add(new LogicJSONNumber(m_achievementRewardClaimed[i].GetGlobalID()));
			}

			jsonObject.Put("achievement_rewards", achievementRewardClaimedArray);

			LogicJSONArray missionCompletedArray = new LogicJSONArray();

			for (int i = 0; i < m_missionCompleted.Size(); i++)
			{
				missionCompletedArray.Add(new LogicJSONNumber(m_missionCompleted[i].GetGlobalID()));
			}

			jsonObject.Put("missions", missionCompletedArray);

			jsonObject.Put("castle_lvl", new LogicJSONNumber(m_allianceCastleLevel));
			jsonObject.Put("castle_total", new LogicJSONNumber(m_allianceCastleTotalCapacity));
			jsonObject.Put("castle_used", new LogicJSONNumber(m_allianceCastleUsedCapacity));
			jsonObject.Put("castle_total_sp", new LogicJSONNumber(m_allianceCastleTotalSpellCapacity));
			jsonObject.Put("castle_used_sp", new LogicJSONNumber(m_allianceCastleUsedSpellCapacity));
			jsonObject.Put("town_hall_lvl", new LogicJSONNumber(m_townHallLevel));
			jsonObject.Put("th_v2_lvl", new LogicJSONNumber(m_townHallLevelVillage2));
			jsonObject.Put("score", new LogicJSONNumber(m_score));
			jsonObject.Put("duel_score", new LogicJSONNumber(m_duelScore));
			jsonObject.Put("war_preference", new LogicJSONNumber(m_warPreference));
			jsonObject.Put("attack_rating", new LogicJSONNumber(m_attackRating));
			jsonObject.Put("atack_kfactor", new LogicJSONNumber(m_attackKFactor));
			jsonObject.Put("attack_win_cnt", new LogicJSONNumber(m_attackWinCount));
			jsonObject.Put("attack_lose_cnt", new LogicJSONNumber(m_attackLoseCount));
			jsonObject.Put("defense_win_cnt", new LogicJSONNumber(m_defenseWinCount));
			jsonObject.Put("defense_lose_cnt", new LogicJSONNumber(m_defenseLoseCount));
			jsonObject.Put("treasury_gold_cnt", new LogicJSONNumber(m_treasuryGoldCount));
			jsonObject.Put("treasury_elixir_cnt", new LogicJSONNumber(m_treasuryElixirCount));
			jsonObject.Put("treasury_dark_elixir_cnt", new LogicJSONNumber(m_treasuryDarkElixirCount));

			if (m_redPackageState != 0)
			{
				jsonObject.Put("red_package_state", new LogicJSONNumber(m_redPackageState));
			}
		}

		public override void SaveToReplay(LogicJSONObject jsonObject)
		{
			jsonObject.Put("avatar_id_high", new LogicJSONNumber(m_id.GetHigherInt()));
			jsonObject.Put("avatar_id_low", new LogicJSONNumber(m_id.GetLowerInt()));
			jsonObject.Put("name", new LogicJSONString(m_name));
			jsonObject.Put("alliance_name", new LogicJSONString(m_allianceName ?? string.Empty));
			jsonObject.Put("xp_level", new LogicJSONNumber(m_expLevel));

			if (m_allianceId != null)
			{
				jsonObject.Put("alliance_id_high", new LogicJSONNumber(m_allianceId.GetHigherInt()));
				jsonObject.Put("alliance_id_low", new LogicJSONNumber(m_allianceId.GetLowerInt()));
				jsonObject.Put("badge_id", new LogicJSONNumber(m_allianceBadgeId));
				jsonObject.Put("alliance_exp_level", new LogicJSONNumber(m_allianceExpLevel));
				jsonObject.Put("alliance_unit_visit_capacity", new LogicJSONNumber(m_allianceUnitVisitCapacity));
				jsonObject.Put("alliance_unit_spell_visit_capacity", new LogicJSONNumber(m_allianceUnitSpellVisitCapacity));
			}

			jsonObject.Put("league_type", new LogicJSONNumber(m_leagueType));

			SaveDataSlotArray(jsonObject, "units", m_unitCount);
			SaveDataSlotArray(jsonObject, "spells", m_spellCount);
			SaveDataSlotArray(jsonObject, "unit_upgrades", m_unitUpgrade);
			SaveDataSlotArray(jsonObject, "spell_upgrades", m_spellUpgrade);
			SaveDataSlotArray(jsonObject, "resources", m_resourceCount);
			SaveUnitSlotArray(jsonObject, "alliance_units", m_allianceUnitCount);
			SaveDataSlotArray(jsonObject, "hero_states", m_heroState);
			SaveDataSlotArray(jsonObject, "hero_health", m_heroHealth);
			SaveDataSlotArray(jsonObject, "hero_upgrade", m_heroUpgrade);
			SaveDataSlotArray(jsonObject, "hero_modes", m_heroMode);
			SaveDataSlotArray(jsonObject, "variables", m_variables);
			SaveDataSlotArray(jsonObject, "units2", m_unitCountVillage2);

			jsonObject.Put("castle_lvl", new LogicJSONNumber(m_allianceCastleLevel));
			jsonObject.Put("castle_total", new LogicJSONNumber(m_allianceCastleTotalCapacity));
			jsonObject.Put("castle_used", new LogicJSONNumber(m_allianceCastleUsedCapacity));
			jsonObject.Put("castle_total_sp", new LogicJSONNumber(m_allianceCastleTotalSpellCapacity));
			jsonObject.Put("castle_used_sp", new LogicJSONNumber(m_allianceCastleUsedSpellCapacity));
			jsonObject.Put("town_hall_lvl", new LogicJSONNumber(m_townHallLevel));
			jsonObject.Put("th_v2_lvl", new LogicJSONNumber(m_townHallLevelVillage2));
			jsonObject.Put("score", new LogicJSONNumber(m_score));
			jsonObject.Put("duel_score", new LogicJSONNumber(m_duelScore));

			if (m_redPackageState != 0)
			{
				jsonObject.Put("red_package_state", new LogicJSONNumber(m_redPackageState));
			}
		}

		public override void SaveToDirect(LogicJSONObject jsonObject)
		{
			jsonObject.Put("avatar_id_high", new LogicJSONNumber(m_id.GetHigherInt()));
			jsonObject.Put("avatar_id_low", new LogicJSONNumber(m_id.GetLowerInt()));
			jsonObject.Put("name", new LogicJSONString(m_name));
			jsonObject.Put("alliance_name", new LogicJSONString(m_allianceName ?? string.Empty));
			jsonObject.Put("xp_level", new LogicJSONNumber(m_expLevel));

			if (m_allianceId != null)
			{
				jsonObject.Put("alliance_id_high", new LogicJSONNumber(m_allianceId.GetHigherInt()));
				jsonObject.Put("alliance_id_low", new LogicJSONNumber(m_allianceId.GetLowerInt()));
				jsonObject.Put("badge_id", new LogicJSONNumber(m_allianceBadgeId));
				jsonObject.Put("alliance_exp_level", new LogicJSONNumber(m_allianceExpLevel));
				jsonObject.Put("alliance_unit_visit_capacity", new LogicJSONNumber(m_allianceUnitVisitCapacity));
				jsonObject.Put("alliance_unit_spell_visit_capacity", new LogicJSONNumber(m_allianceUnitSpellVisitCapacity));
			}

			jsonObject.Put("league_type", new LogicJSONNumber(m_leagueType));

			SaveDataSlotArray(jsonObject, "resources", m_resourceCount);
			SaveUnitSlotArray(jsonObject, "alliance_units", m_allianceUnitCount);
			SaveDataSlotArray(jsonObject, "hero_states", m_heroState);
			SaveDataSlotArray(jsonObject, "hero_health", m_heroHealth);
			SaveDataSlotArray(jsonObject, "hero_upgrade", m_heroUpgrade);
			SaveDataSlotArray(jsonObject, "hero_modes", m_heroMode);
			SaveDataSlotArray(jsonObject, "variables", m_variables);

			jsonObject.Put("castle_lvl", new LogicJSONNumber(m_allianceCastleLevel));
			jsonObject.Put("castle_total", new LogicJSONNumber(m_allianceCastleTotalCapacity));
			jsonObject.Put("castle_used", new LogicJSONNumber(m_allianceCastleUsedCapacity));
			jsonObject.Put("castle_total_sp", new LogicJSONNumber(m_allianceCastleTotalSpellCapacity));
			jsonObject.Put("castle_used_sp", new LogicJSONNumber(m_allianceCastleUsedSpellCapacity));
			jsonObject.Put("town_hall_lvl", new LogicJSONNumber(m_townHallLevel));
			jsonObject.Put("th_v2_lvl", new LogicJSONNumber(m_townHallLevelVillage2));
			jsonObject.Put("score", new LogicJSONNumber(m_score));
			jsonObject.Put("duel_score", new LogicJSONNumber(m_duelScore));

			if (m_redPackageState != 0)
			{
				jsonObject.Put("red_package_state", new LogicJSONNumber(m_redPackageState));
			}
		}

		private void SaveDataSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicDataSlot> dataSlotArray)
		{
			LogicJSONArray jsonArray = new LogicJSONArray(dataSlotArray.Size());

			for (int i = 0; i < dataSlotArray.Size(); i++)
			{
				LogicJSONObject obj = new LogicJSONObject();
				dataSlotArray[i].WriteToJSON(obj);
				jsonArray.Add(obj);
			}

			jsonObject.Put(key, jsonArray);
		}

		private void SaveUnitSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicUnitSlot> unitSlotArray)
		{
			LogicJSONArray jsonArray = new LogicJSONArray(unitSlotArray.Size());

			for (int i = 0; i < unitSlotArray.Size(); i++)
			{
				LogicJSONObject obj = new LogicJSONObject();
				unitSlotArray[i].WriteToJSON(obj);
				jsonArray.Add(obj);
			}

			jsonObject.Put(key, jsonArray);
		}
	}
}