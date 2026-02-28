using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Battle;
using Supercell.Magic.Logic.Calendar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Mode
{
	public class LogicGameMode
	{
		private bool m_battleOver;
		private bool m_liveReplayMode;

		private int m_state;
		private int m_visitType;
		private int m_startTimestamp;
		private int m_debugFastForwardSecs;
		private int m_shieldTime;
		private int m_guardTime;
		private int m_personalBreakTime;
		private int m_startGuardTime;
		private int m_secondsSinceLastMaintenance;
		private int m_skipPreparationSecs;
		private int m_liveReplayClientDelay;
		private int m_liveReplaySubTick;
		private int m_villageType;

		private readonly LogicLevel m_level;
		private readonly LogicCommandManager m_commandManager;
		private readonly LogicCalendar m_calendar;
		private readonly LogicConfiguration m_configuration;

		private LogicTimer m_battleTimer;
		private LogicReplay m_replay;

		public LogicGameMode()
		{
			m_level = new LogicLevel(this);
			m_commandManager = new LogicCommandManager(m_level);
			m_calendar = new LogicCalendar();
			m_configuration = new LogicConfiguration();
			m_startTimestamp = -1;
		}

		public void Destruct()
		{
			m_level.Destruct();
			m_commandManager.Destruct();
			m_calendar.Destruct();

			if (m_battleTimer != null)
			{
				m_battleTimer.Destruct();
				m_battleTimer = null;
			}

			if (m_replay != null)
			{
				m_replay.Destruct();
				m_replay = null;
			}
		}

		public ChecksumHelper CalculateChecksum(LogicJSONObject root, bool includeGameObjects)
		{
			ChecksumHelper checksum = new ChecksumHelper(root);

			checksum.StartObject("LogicGameMode");

			checksum.WriteValue("subtick", m_level.GetLogicTime().GetTick());
			checksum.WriteValue("m_currentTimestamp", m_startTimestamp);

			if (m_level.GetHomeOwnerAvatar() != null)
			{
				checksum.StartObject("homeOwner");
				m_level.GetHomeOwnerAvatar().GetChecksum(checksum);
				checksum.EndObject();
			}

			if (m_level.GetVisitorAvatar() != null)
			{
				checksum.StartObject("visitor");
				m_level.GetVisitorAvatar().GetChecksum(checksum);
				checksum.EndObject();
			}

			m_level.GetGameObjectManager().GetChecksum(checksum, includeGameObjects);

			if (m_calendar != null)
			{
				checksum.StartObject("calendar");
				m_calendar.GetChecksum(checksum);
				checksum.EndObject();
			}

			checksum.WriteValue("checksum", checksum.GetChecksum());
			checksum.EndObject();

			return checksum;
		}

		public LogicCommandManager GetCommandManager()
			=> m_commandManager;

		public LogicLevel GetLevel()
			=> m_level;

		public LogicConfiguration GetConfiguration()
			=> m_configuration;

		public LogicCalendar GetCalendar()
			=> m_calendar;

		public LogicReplay GetReplay()
			=> m_replay;

		public int GetServerTimeInSecondsSince1970()
			=> 16 * m_level.GetLogicTime().GetTick() / 1000 + m_startTimestamp + m_debugFastForwardSecs;

		public int GetStartTime()
			=> m_startTimestamp;

		public int GetDebugFastForwardSecs()
			=> m_debugFastForwardSecs;

		public void SetDebugFastForwardSecs(int secs)
		{
			m_debugFastForwardSecs = secs;
		}

		public int GetSecondsSinceLastMaintenance()
			=> m_secondsSinceLastMaintenance;

		public int GetVisitType()
		{
			if (m_state == 4)
			{
				return m_visitType;
			}

			return -1;
		}

		public int GetVillageType()
		{
			if (m_state == 1)
			{
				return m_villageType;
			}

			return -1;
		}

		public void SetBattleOver()
		{
			if (m_battleOver)
			{
				return;
			}

			m_level.GetBattleLog().SetBattleEnded(LogicDataTables.GetGlobals().GetAttackLengthSecs() - GetRemainingAttackSeconds());
			m_level.GetMissionManager().Tick();

			LogicArrayList<LogicComponent> components = m_level.GetComponentManager().GetComponents(LogicComponentType.COMBAT);

			for (int i = 0; i < components.Size(); i++)
			{
				((LogicCombatComponent)components[i]).Boost(0, 0, 0);
			}

			bool duelMatch = (m_level.GetMatchType() & 0xFFFFFFFE) == 8;

			if (duelMatch)
			{
				LogicAvatar avatar = m_level.GetVisitorAvatar();

				if (avatar != null && avatar.IsClientAvatar())
				{
					((LogicClientAvatar)avatar).RemoveUnitsVillage2();
				}
			}

			if (m_state == 3)
			{
				EndDefendState();
			}
			else
			{
				LogicBattleLog battleLog = m_level.GetBattleLog();

				if (battleLog.GetBattleStarted())
				{
					LogicAvatar visitorAvatar = m_level.GetVisitorAvatar();
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

					int stars = battleLog.GetStars();

					if (!m_level.GetVisitorAvatar().IsClientAvatar() || !m_level.GetHomeOwnerAvatar().IsClientAvatar())
					{
						if (visitorAvatar.IsClientAvatar() && homeOwnerAvatar.IsNpcAvatar())
						{
							LogicNpcAvatar npcAvatar = (LogicNpcAvatar)homeOwnerAvatar;
							LogicNpcData npcData = npcAvatar.GetNpcData();

							int npcStars = visitorAvatar.GetNpcStars(npcData);

							if (stars > npcStars && npcData.IsSinglePlayer())
							{
								visitorAvatar.SetNpcStars(npcData, stars);
								visitorAvatar.GetChangeListener().CommodityCountChanged(0, npcData, stars);
							}

							// TODO: LogicBattleLog::sendNpcAttackEndEvents.
						}
					}
					else
					{
						LogicClientAvatar attacker = (LogicClientAvatar)visitorAvatar;
						LogicClientAvatar defender = (LogicClientAvatar)homeOwnerAvatar;

						int originalAttackerScore = attacker.GetScore();
						int originalDefenderScore = defender.GetScore();
						int matchType = m_level.GetMatchType();

						if (matchType == 1 || !LogicDataTables.GetGlobals().ScoringOnlyFromMatchedMode() && (matchType == 0 || matchType == 2 || matchType == 4 || matchType == 6))
						{
							LogicGamePlayUtil.CalculateCombatScore(attacker, defender, stars, false,
																   matchType == 4, battleLog.GetDestructionPercentage(), m_calendar.GetStarBonusMultiplier(), duelMatch);

							if (!duelMatch && homeOwnerAvatar.GetTownHallLevel() >= LogicDataTables.GetGlobals().GetLootCartEnabledTownHall())
							{
								LogicDataTable resourceTable = LogicDataTables.GetTable(LogicDataType.RESOURCE);

								if (resourceTable.GetItemCount() > 0)
								{
									bool hasStolen = false;

									for (int i = 0; i < resourceTable.GetItemCount(); i++)
									{
										LogicResourceData data = (LogicResourceData)resourceTable.GetItemAt(i);

										if (!data.IsPremiumCurrency())
										{
											if (battleLog.GetStolenResources(data) > 0)
											{
												hasStolen = true;
											}
										}
									}

									if (hasStolen)
									{
										LogicGameObjectManager gameObjectManager = m_level.GetGameObjectManagerAt(0);
										LogicObstacle lootCart = gameObjectManager.GetLootCart();

										if (lootCart == null)
										{
											gameObjectManager.AddLootCart();
											lootCart = gameObjectManager.GetLootCart();
										}

										if (lootCart != null)
										{
											LogicLootCartComponent lootCartComponent = lootCart.GetLootCartComponent();

											if (lootCartComponent != null)
											{
												for (int i = 0; i < resourceTable.GetItemCount(); i++)
												{
													LogicResourceData data = (LogicResourceData)resourceTable.GetItemAt(i);

													if (!data.IsPremiumCurrency() && data.GetWarResourceReferenceData() == null)
													{
														int lootPercentage = lootCart.GetObstacleData().GetLootDefensePercentage();
														int lootCount = battleLog.GetStolenResources(data) * lootPercentage / 100;

														lootCartComponent.SetResourceCount(i,
																						   LogicMath.Min(LogicMath.Max(lootCount, lootCartComponent.GetResourceCount(i)),
																										 lootCartComponent.GetCapacityCount(i)));
													}
												}
											}
										}
									}
								}
							}

							m_level.UpdateBattleShieldStatus(false);

							if (stars > 0)
							{
								LogicArrayList<LogicDataSlot> castedUnits = battleLog.GetCastedUnits();
								LogicArrayList<LogicDataSlot> castedSpells = battleLog.GetCastedSpells();
								LogicArrayList<LogicUnitSlot> castedAllianceUnits = battleLog.GetCastedAllianceUnits();

								LogicArrayList<LogicDataSlot> placedUnits = new LogicArrayList<LogicDataSlot>(castedUnits.Size());

								for (int i = 0; i < castedUnits.Size(); i++)
								{
									placedUnits.Add(new LogicDataSlot(castedUnits[i].GetData(), castedUnits[i].GetCount()));
								}

								for (int i = 0; i < castedSpells.Size(); i++)
								{
									int idx = -1;

									for (int j = 0; j < placedUnits.Size(); j++)
									{
										if (placedUnits[j].GetData() == castedSpells[i].GetData())
										{
											idx = j;
											break;
										}
									}

									if (idx != -1)
									{
										placedUnits[idx].SetCount(placedUnits[idx].GetCount() + castedSpells[i].GetCount());
									}
									else
									{
										placedUnits.Add(new LogicDataSlot(castedSpells[i].GetData(), castedSpells[i].GetCount()));
									}
								}

								for (int i = 0; i < castedAllianceUnits.Size(); i++)
								{
									placedUnits.Add(new LogicDataSlot(castedAllianceUnits[i].GetData(), castedAllianceUnits[i].GetCount()));
								}

								for (int i = 0; i < placedUnits.Size(); i++)
								{
									LogicCombatItemData data = (LogicCombatItemData)placedUnits[i].GetData();
									LogicCalendarUseTroop calendarUseTroopEvent = m_calendar.GetUseTroopEvents(data);

									if (calendarUseTroopEvent != null)
									{
										int count = attacker.GetEventUnitCounterCount(data);

										if (placedUnits[i].GetCount() >= count >> 16)
										{
											int progressCount = (short)count + 1;
											int eventCounter = progressCount | (int)(count & 0xFFFF0000);

											attacker.SetCommodityCount(6, data, eventCounter);
											attacker.GetChangeListener().CommodityCountChanged(6, data, eventCounter);

											if (calendarUseTroopEvent.GetParameter(0) == progressCount)
											{
												int diamonds = calendarUseTroopEvent.GetParameter(2);
												int xp = calendarUseTroopEvent.GetParameter(3);

												attacker.XpGainHelper(xp);
												attacker.SetDiamonds(attacker.GetDiamonds() + diamonds);
												attacker.SetFreeDiamonds(attacker.GetFreeDiamonds() + diamonds);
												attacker.GetChangeListener().FreeDiamondsAdded(diamonds, 9);

												Debugger.HudPrint(string.Format("USE TROOP Event: Awarding XP: {0} GEMS {1}", xp, diamonds));
											}
										}
									}
								}

								for (int i = 0; i < placedUnits.Size(); i++)
								{
									placedUnits[i].Destruct();
								}

								placedUnits.Destruct();
							}
						}

						if (m_state != 5 &&
							m_level.GetDefenseShieldActivatedHours() == 0 &&
							battleLog.GetDestructionPercentage() > 0)
						{
							int defenseVillageGuardCounter = defender.GetDefenseVillageGuardCounter() + 1;

							defender.SetDefenseVillageGuardCounter(defenseVillageGuardCounter);
							defender.GetChangeListener().DefenseVillageGuardCounterChanged(defenseVillageGuardCounter);

							int villageGuardMins = (defenseVillageGuardCounter & 0xFFFFFF) == 3 * ((defenseVillageGuardCounter & 0xFFFFFF) / 3)
								? defender.GetLeagueTypeData().GetVillageGuardInMins()
								: LogicDataTables.GetGlobals().GetDefaultDefenseVillageGuard();

							m_level.GetHome().GetChangeListener().GuardActivated(60 * villageGuardMins);

							Debugger.HudPrint(string.Format("Battle end. No Shield, Village Guard for defender: {0}", villageGuardMins));
						}

						battleLog.SetAttackerScore(attacker.GetScore() - originalAttackerScore);
						battleLog.SetDefenderScore(defender.GetScore() - originalDefenderScore);
						battleLog.SetOriginalAttackerScore(originalAttackerScore);
						battleLog.SetOriginalDefenderScore(originalDefenderScore);

						if (m_state != 5)
						{
							if (stars != 0)
							{
								if (matchType != 3 && matchType != 7 && matchType != 8 && matchType != 9)
								{
									if (matchType == 5)
									{
										if (stars > m_level.GetPreviousAttackStars() && !m_level.GetIgnoreAttack())
										{
											m_level.GetAchievementManager().IncreaseWarStars(stars);
										}
									}
									else
									{
										m_level.GetAchievementManager().PvpAttackWon();
									}
								}
							}
							else if (matchType > 9 || matchType == 3 || matchType == 5 || matchType == 7 || matchType == 8 || matchType == 9)
							{
								m_level.GetAchievementManager().PvpDefenseWon();
							}
						}
					}
				}
			}

			m_battleOver = true;
		}

		public int GetState()
			=> m_state;

		public int GetShieldRemainingSeconds()
			=> LogicMath.Max(LogicTime.GetTicksInSeconds(m_shieldTime - m_level.GetLogicTime().GetTick()), 0);

		public void SetShieldRemainingSeconds(int secs)
		{
			m_shieldTime = LogicTime.GetSecondsInTicks(secs) + m_level.GetLogicTime().GetTick();

			int logicTime = m_level.GetLogicTime().GetTick();
			int startGuardTime = m_shieldTime;

			if (m_shieldTime < logicTime)
			{
				startGuardTime = logicTime;
			}

			m_startGuardTime = startGuardTime;
		}

		public int GetGuardRemainingSeconds()
		{
			int startTime = m_startGuardTime - m_level.GetLogicTime().GetTick();

			if (startTime > 0)
			{
				startTime = 0;
			}

			return LogicMath.Max(LogicTime.GetTicksInSeconds(m_guardTime + startTime), 0);
		}

		public void SetGuardRemainingSeconds(int secs)
		{
			m_guardTime = LogicTime.GetSecondsInTicks(secs);

			int logicTime = m_level.GetLogicTime().GetTick();
			int startGuardTime = logicTime;

			if (m_shieldTime >= logicTime)
			{
				startGuardTime = m_shieldTime;
			}

			m_startGuardTime = startGuardTime;
		}

		public int GetPersonalBreakCooldownSeconds()
			=> LogicMath.Max(LogicTime.GetTicksInSeconds(m_personalBreakTime - m_level.GetLogicTime().GetTick()), 0);

		public void SetPersonalBreakCooldownSeconds(int secs)
		{
			m_personalBreakTime = LogicTime.GetSecondsInTicks(secs) + m_level.GetLogicTime().GetTick();
		}

		public void SaveToJSON(LogicJSONObject jsonObject)
		{
			m_level.SaveToJSON(jsonObject);
		}

		public void SubTick()
		{
			if (m_state == 1)
			{
				m_calendar.Update(GetServerTimeInSecondsSince1970(), m_level.GetHomeOwnerAvatar(), m_level);
			}

			m_commandManager.SubTick();
			m_level.SubTick();

			if (m_replay != null)
			{
				m_replay.SubTick();
			}
		}

		public void Tick()
		{
			if (m_liveReplayMode)
			{
				m_liveReplayClientDelay = m_liveReplaySubTick - m_level.GetLogicTime().GetTick();
			}

			m_level.Tick();
		}

		public void UpdateOneSubTick()
		{
			LogicTime time = m_level.GetLogicTime();

			if (m_state != 2 || !m_battleOver)
			{
				SubTick();

				if (time.IsFullTick())
				{
					Tick();
				}
			}

			if (m_level.IsInCombatState())
			{
				if (m_battleTimer != null &&
					m_battleTimer.GetRemainingSeconds(time) == 0 ||
					m_level.GetBattleEndPending())
				{
					SetBattleOver();
				}
			}

			time.IncreaseTick();
		}

		public void StartDefendState(LogicAvatar avatar)
		{
			if (m_state == 1 || m_state == 3)
			{
				m_state = 3;
				m_battleOver = false;
				m_level.DefenseStateStarted(avatar);
			}
			else
			{
				Debugger.Error("startDefendState called from invalid state");
			}
		}

		public void EndDefendState()
		{
			if (m_state == 3)
			{
				m_state = 1;
				m_level.DefenseStateEnded();
			}
			else
			{
				Debugger.Error("endDefendState called from invalid state");
			}
		}

		public void EndAttackPreparation()
		{
			if (m_battleTimer != null)
			{
				int attackLength = LogicDataTables.GetGlobals().GetAttackLengthSecs();
				int battleRemainingSecs = m_battleTimer.GetRemainingSeconds(m_level.GetLogicTime());

				if (battleRemainingSecs > attackLength)
				{
					int remainingPrepSecs = battleRemainingSecs - attackLength;

					if (m_replay != null)
					{
						m_replay.RecordPreparationSkipTime(remainingPrepSecs);
					}

					m_skipPreparationSecs = remainingPrepSecs;
					m_battleTimer.StartTimer(attackLength, m_level.GetLogicTime(), false, -1);
				}

				if (m_level.GetPlayerAvatar() != null)
				{
					LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();

					if (playerAvatar.GetChangeListener() != null)
					{
						playerAvatar.GetChangeListener().BattleFeedback(5, 0);
					}
				}
			}
		}

		public bool IsInAttackPreparationMode()
		{
			if (m_state == 2 || m_state == 5)
			{
				LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

				if (homeOwnerAvatar.IsClientAvatar())
				{
					return GetRemainingAttackSeconds() > LogicDataTables.GetGlobals().GetAttackLengthSecs();
				}
			}

			return false;
		}

		public bool IsBattleOver()
			=> m_battleOver;

		public int GetRemainingAttackSeconds()
		{
			if ((m_state == 2 || m_state == 5) && !m_battleOver)
			{
				if (!m_level.GetInvulnerabilityEnabled())
				{
					if (m_battleTimer != null)
					{
						return LogicMath.Max(m_battleTimer.GetRemainingSeconds(m_level.GetLogicTime()), 1);
					}
				}

				return 1;
			}

			return 0;
		}

		public void LoadHomeState(LogicClientHome home, LogicAvatar homeOwnerAvatar, int secondsSinceLastSave, int villageType, int currentTimestamp,
								  int secondsSinceLastMaintenance, int reengagementSeconds)
		{
			if (villageType == 2)
			{
				m_level.SetVillageType(villageType);
			}

			if (home != null)
			{
				m_state = 1;
				m_villageType = villageType;

				if (LogicDataTables.GetGlobals().StartInLastUsedVillage())
				{
					int lastUsedVillage = homeOwnerAvatar.GetVillageToGoTo();

					if (!m_level.GetMissionManager().HasTravel(homeOwnerAvatar))
					{
						lastUsedVillage = 0;
					}

					if (lastUsedVillage < 0)
					{
						Debugger.Warning("VillageToGoTo<0");
					}
					else
					{
						if (lastUsedVillage > 1)
						{
							Debugger.Warning("VillageToGoTo too big");
						}
						else
						{
							m_level.SetVillageType(lastUsedVillage);
						}
					}
				}

				m_secondsSinceLastMaintenance = secondsSinceLastMaintenance;
				m_startTimestamp = currentTimestamp;
				m_configuration.Load(LogicJSONParser.ParseObject(home.GetGlobalJSON()));
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

				if (m_battleTimer != null)
				{
					m_battleTimer.Destruct();
					m_battleTimer = null;
				}

				m_level.SetHome(home, false);
				m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
				m_level.FastForwardTime(secondsSinceLastSave);

				homeOwnerAvatar.SetLevel(m_level);

				m_level.ReengageLootCart(reengagementSeconds);
				m_level.LoadingFinished();

				m_shieldTime = LogicTime.GetSecondsInTicks(home.GetShieldDurationSeconds());
				m_guardTime = LogicTime.GetSecondsInTicks(home.GetGuardDurationSeconds());
				m_personalBreakTime = LogicTime.GetSecondsInTicks(home.GetPersonalBreakSeconds());

				int logicTime = m_level.GetLogicTime().GetTick();
				int startGuardTime = logicTime;

				if (m_shieldTime >= logicTime)
				{
					startGuardTime = m_shieldTime;
				}

				m_startGuardTime = startGuardTime;

				if (LogicDataTables.GetGlobals().UseVillageObjects())
				{
					m_level.LoadVillageObjects();
				}
			}
		}

		public void LoadNpcAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave)
		{
			if (m_state == 1)
			{
				Debugger.Error("loadAttackState called from invalid state");
			}
			else
			{
				m_state = 2;
				m_startTimestamp = currentTimestamp;
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

				if (m_battleTimer != null)
				{
					m_battleTimer.Destruct();
					m_battleTimer = null;
				}

				if (homeOwnerAvatar.IsNpcAvatar())
				{
					LogicNpcAvatar npcAvatar = (LogicNpcAvatar)homeOwnerAvatar;
					LogicNpcData npcData = npcAvatar.GetNpcData();

					homeOwnerAvatar.SetResourceCount(LogicDataTables.GetGoldData(), LogicMath.Max(npcData.GetGoldCount() - visitorAvatar.GetLootedNpcGold(npcData), 0));
					homeOwnerAvatar.SetResourceCount(LogicDataTables.GetElixirData(), LogicMath.Max(npcData.GetElixirCount() - visitorAvatar.GetLootedNpcElixir(npcData), 0));

					m_level.SetMatchType(2, 0);
					m_level.SetHome(home, false);
					m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
					m_level.SetVisitorAvatar(visitorAvatar);
					m_level.FastForwardTime(secondsSinceLastSave);
					m_level.LoadingFinished();
				}
				else
				{
					Debugger.Error("loadNpcAttackState called and home owner is not npc avatar");
				}
			}
		}

		public void LoadNpcDuelState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave)
		{
			if (m_state != 0)
			{
				Debugger.Error("loadNpcDuelState called from invalid state");
			}
			else
			{
				m_state = 2;
				m_startTimestamp = currentTimestamp;
				m_configuration.Load((LogicJSONObject)LogicJSONParser.Parse(home.GetGlobalJSON()));
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

				if (m_battleTimer != null)
				{
					m_battleTimer.Destruct();
					m_battleTimer = null;
				}

				m_level.SetMatchType(9, null);
				m_level.SetVillageType(1);
				m_level.SetHome(home, false);
				m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
				m_level.SetVisitorAvatar(visitorAvatar);
				m_level.FastForwardTime(secondsSinceLastSave);
				m_level.LoadingFinished();

				m_replay = new LogicReplay(m_level);
			}
		}

		public void LoadVisitState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave)
		{
			if (m_state != 0)
			{
				Debugger.Error("loadVisitState called from invalid state");
			}
			else
			{
				m_state = 4;
				m_startTimestamp = currentTimestamp;
				m_configuration.Load((LogicJSONObject)LogicJSONParser.Parse(home.GetGlobalJSON()));
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);
				m_level.SetNpcVillage(homeOwnerAvatar.IsNpcAvatar());
				m_level.SetHome(home, false);
				m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
				m_level.SetVisitorAvatar(visitorAvatar);
				m_level.FastForwardTime(secondsSinceLastSave);

				homeOwnerAvatar.SetLevel(m_level);

				m_level.LoadingFinished();
			}
		}

		public void LoadMatchedAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave,
										   int secondsSinceLastMaintenance)
		{
			if (m_state != 0)
			{
				Debugger.Error("loadAttackState called from invalid state");
			}
			else
			{
				m_state = 2;
				m_startTimestamp = currentTimestamp;
				m_secondsSinceLastMaintenance = secondsSinceLastMaintenance;
				m_configuration.Load((LogicJSONObject)LogicJSONParser.Parse(home.GetGlobalJSON()));
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

				if (m_battleTimer != null)
				{
					m_battleTimer.Destruct();
					m_battleTimer = null;
				}

				m_battleTimer = new LogicTimer();
				m_battleTimer.StartTimer(LogicDataTables.GetGlobals().GetAttackLengthSecs() + LogicDataTables.GetGlobals().GetAttackPreparationLengthSecs(),
											  m_level.GetLogicTime(), false, -1);

				if (homeOwnerAvatar.IsClientAvatar())
				{
					m_level.SetMatchType(1, null);
					m_level.SetHome(home, false);
					m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
					m_level.SetVisitorAvatar(visitorAvatar);
					m_level.FastForwardTime(secondsSinceLastSave);

					homeOwnerAvatar.SetLevel(m_level);

					m_level.LoadingFinished();
					m_replay = new LogicReplay(m_level);
				}
				else
				{
					Debugger.Error("loadDirectAttackState called and home owner is not client avatar");
				}
			}
		}

		public void LoadRevengeAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave,
										   int secondsSinceLastMaintenance, LogicLong revengeId)
		{
			if (m_state != 0)
			{
				Debugger.Error("loadAttackState called from invalid state");
			}
			else
			{
				m_state = 2;
				m_startTimestamp = currentTimestamp;
				m_secondsSinceLastMaintenance = secondsSinceLastMaintenance;
				m_configuration.Load((LogicJSONObject)LogicJSONParser.Parse(home.GetGlobalJSON()));
				m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

				if (m_battleTimer != null)
				{
					m_battleTimer.Destruct();
					m_battleTimer = null;
				}

				m_battleTimer = new LogicTimer();
				m_battleTimer.StartTimer(LogicDataTables.GetGlobals().GetAttackLengthSecs() + LogicDataTables.GetGlobals().GetAttackPreparationLengthSecs(),
											  m_level.GetLogicTime(), false, -1);

				if (homeOwnerAvatar.IsClientAvatar())
				{
					m_level.SetMatchType(4, revengeId);
					m_level.SetHome(home, false);
					m_level.SetHomeOwnerAvatar(homeOwnerAvatar);
					m_level.SetVisitorAvatar(visitorAvatar);
					m_level.FastForwardTime(secondsSinceLastSave);

					homeOwnerAvatar.SetLevel(m_level);

					m_level.LoadingFinished();
					m_replay = new LogicReplay(m_level);
				}
				else
				{
					Debugger.Error("loadDirectAttackState called and home owner is not client avatar");
				}
			}
		}

		public void LoadDirectAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int secondsSinceLastSave, bool war, bool village2,
										  int currentTimestamp)
		{
			if (m_state != 0)
			{
				Debugger.Error("loadAttackState called from invalid state");
			}

			m_state = 2;
			m_startTimestamp = currentTimestamp;
			m_configuration.Load((LogicJSONObject)LogicJSONParser.Parse(home.GetGlobalJSON()));
			m_calendar.Load(home.GetCalendarJSON(), currentTimestamp);

			if (m_battleTimer != null)
			{
				m_battleTimer.Destruct();
				m_battleTimer = null;
			}

			m_battleTimer = new LogicTimer();
			m_battleTimer.StartTimer(
				LogicDataTables.GetGlobals().GetAttackLengthSecs() + (village2
					? LogicDataTables.GetGlobals().GetAttackVillage2PreparationLengthSecs()
					: LogicDataTables.GetGlobals().GetAttackPreparationLengthSecs()),
				m_level.GetLogicTime(), false, -1);

			if (!homeOwnerAvatar.IsClientAvatar())
			{
				Debugger.Error("loadDirectAttackState called and home owner is not client avatar");
			}

			m_level.SetMatchType(village2 ? 8 : war ? 7 : 3, null);

			if (village2)
			{
				m_level.SetVillageType(1);
			}

			m_level.SetHome(home, false);
			m_level.SetHomeOwnerAvatar(homeOwnerAvatar);

			homeOwnerAvatar.SetLevel(m_level);

			m_level.SetVisitorAvatar(visitorAvatar);
			m_level.FastForwardTime(secondsSinceLastSave);
			m_level.LoadingFinished();
			m_replay = new LogicReplay(m_level);
		}
	}
}