using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Battle;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicBunkerComponent : LogicUnitStorageComponent
	{
		public const int PATROL_PATHS = 16;

		private readonly LogicGameObjectFilter m_filter;
		private LogicTimer m_requestCooldownTimer;
		private LogicTimer m_clanMailCooldownTimer;
		private LogicTimer m_replayShareCooldownTimer;
		private LogicTimer m_elderKickCooldownTimer;
		private LogicTimer m_challengeCooldownTimer;
		private LogicTimer m_arrangeWarCooldownTimer;
		private LogicArrayList<LogicVector2> m_patrolPath;

		private int m_team;
		private int m_updateAvatarCooldown;
		private int m_bunkerSearchTime;
		private int m_troopSpawnOffset;

		public LogicBunkerComponent(LogicGameObject gameObject, int capacity) : base(gameObject, capacity)
		{
			m_team = 1;

			m_filter = new LogicGameObjectFilter();
			m_filter.AddGameObjectType(LogicGameObjectType.CHARACTER);
			m_filter.PassEnemyOnly(gameObject);
		}

		public int GetTeam()
			=> m_team;

		public void SetComponentMode(int value)
		{
			m_team = value;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.BUNKER;

		public void StartRequestCooldownTime()
		{
			if (m_requestCooldownTimer == null)
			{
				m_requestCooldownTimer = new LogicTimer();
			}

			m_requestCooldownTimer.StartTimer(GetTotalRequestCooldownTime(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void StartClanMailCooldownTime()
		{
			if (m_clanMailCooldownTimer == null)
			{
				m_clanMailCooldownTimer = new LogicTimer();
			}

			m_clanMailCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetClanMailCooldown(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void StartChallengeCooldownTime()
		{
			if (m_challengeCooldownTimer == null)
			{
				m_challengeCooldownTimer = new LogicTimer();
			}

			m_challengeCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetChallengeCooldown(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void StartReplayShareCooldownTime()
		{
			if (m_replayShareCooldownTimer == null)
			{
				m_replayShareCooldownTimer = new LogicTimer();
			}

			m_replayShareCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetReplayShareCooldown(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void StartArrangedWarCooldownTime()
		{
			if (m_arrangeWarCooldownTimer == null)
			{
				m_arrangeWarCooldownTimer = new LogicTimer();
			}

			m_arrangeWarCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetArrangeWarCooldown(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void StopRequestCooldownTime()
		{
			if (m_requestCooldownTimer != null)
			{
				m_requestCooldownTimer.Destruct();
				m_requestCooldownTimer = null;
			}
		}

		public int GetRequestCooldownTime()
		{
			if (m_requestCooldownTimer != null)
			{
				return m_requestCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public void StartElderKickCooldownTime()
		{
			if (m_elderKickCooldownTimer == null)
			{
				m_elderKickCooldownTimer = new LogicTimer();
			}

			m_elderKickCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetElderKickCooldown(), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public int GetElderCooldownTime()
		{
			if (m_elderKickCooldownTimer != null)
			{
				return m_elderKickCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetReplayShareCooldownTime()
		{
			if (m_replayShareCooldownTimer != null)
			{
				return m_replayShareCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetArrangedWarCooldownTime()
		{
			if (m_arrangeWarCooldownTimer != null)
			{
				return m_arrangeWarCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetTotalRequestCooldownTime()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null)
			{
				return ((LogicClientAvatar)homeOwnerAvatar).GetTroopRequestCooldown();
			}

			return LogicDataTables.GetGlobals().GetAllianceTroopRequestCooldown();
		}

		public int GetClanMailCooldownTime()
		{
			if (m_clanMailCooldownTimer != null)
			{
				return m_clanMailCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetChallengeCooldownTime()
		{
			if (m_challengeCooldownTimer != null)
			{
				return m_challengeCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public override void LoadingFinished()
		{
			if (m_parent.GetLevel().IsInCombatState())
			{
				m_patrolPath = CreatePatrolPath();
			}
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			if (m_requestCooldownTimer != null)
			{
				m_requestCooldownTimer.Destruct();
				m_requestCooldownTimer = null;
			}

			if (m_clanMailCooldownTimer != null)
			{
				m_clanMailCooldownTimer.Destruct();
				m_clanMailCooldownTimer = null;
			}

			if (m_replayShareCooldownTimer != null)
			{
				m_replayShareCooldownTimer.Destruct();
				m_replayShareCooldownTimer = null;
			}

			if (m_elderKickCooldownTimer != null)
			{
				m_elderKickCooldownTimer.Destruct();
				m_elderKickCooldownTimer = null;
			}

			if (m_challengeCooldownTimer != null)
			{
				m_challengeCooldownTimer.Destruct();
				m_challengeCooldownTimer = null;
			}

			if (m_arrangeWarCooldownTimer != null)
			{
				m_arrangeWarCooldownTimer.Destruct();
				m_arrangeWarCooldownTimer = null;
			}

			LogicJSONNumber unitRequestTimeNumber = jsonObject.GetJSONNumber("unit_req_time");

			if (unitRequestTimeNumber != null)
			{
				m_requestCooldownTimer = new LogicTimer();
				m_requestCooldownTimer.StartTimer(LogicMath.Min(unitRequestTimeNumber.GetIntValue(), GetTotalRequestCooldownTime()),
													   m_parent.GetLevel().GetLogicTime(), false, -1);
			}

			LogicJSONNumber clanMailTimeNumber = jsonObject.GetJSONNumber("clan_mail_time");

			if (clanMailTimeNumber != null)
			{
				m_clanMailCooldownTimer = new LogicTimer();
				m_clanMailCooldownTimer.StartTimer(LogicMath.Min(clanMailTimeNumber.GetIntValue(), LogicDataTables.GetGlobals().GetClanMailCooldown()),
														m_parent.GetLevel().GetLogicTime(), false, -1);
			}

			LogicJSONNumber shareReplayTimeNumber = jsonObject.GetJSONNumber("share_replay_time");

			if (shareReplayTimeNumber != null)
			{
				m_replayShareCooldownTimer = new LogicTimer();
				m_replayShareCooldownTimer.StartTimer(LogicMath.Min(shareReplayTimeNumber.GetIntValue(), LogicDataTables.GetGlobals().GetReplayShareCooldown()),
														   m_parent.GetLevel().GetLogicTime(), false, -1);
			}

			LogicJSONNumber elderKickTimeNumber = jsonObject.GetJSONNumber("elder_kick_time");

			if (elderKickTimeNumber != null)
			{
				m_elderKickCooldownTimer = new LogicTimer();
				m_elderKickCooldownTimer.StartTimer(LogicMath.Min(elderKickTimeNumber.GetIntValue(), LogicDataTables.GetGlobals().GetElderKickCooldown()),
														 m_parent.GetLevel().GetLogicTime(), false, -1);
			}

			LogicJSONNumber challengeTimeNumber = jsonObject.GetJSONNumber("challenge_time");

			if (challengeTimeNumber != null)
			{
				m_challengeCooldownTimer = new LogicTimer();
				m_challengeCooldownTimer.StartTimer(LogicMath.Min(challengeTimeNumber.GetIntValue(), LogicDataTables.GetGlobals().GetChallengeCooldown()),
														 m_parent.GetLevel().GetLogicTime(), false, -1);
			}

			LogicJSONNumber arrangeWarTimeNumber = jsonObject.GetJSONNumber("arrwar_time");

			if (arrangeWarTimeNumber != null)
			{
				m_arrangeWarCooldownTimer = new LogicTimer();
				m_arrangeWarCooldownTimer.StartTimer(LogicMath.Min(arrangeWarTimeNumber.GetIntValue(), LogicDataTables.GetGlobals().GetArrangeWarCooldown()),
														  m_parent.GetLevel().GetLogicTime(), false, -1);
			}
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			if (m_requestCooldownTimer != null)
			{
				int remainingSecs = m_requestCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("unit_req_time", new LogicJSONNumber(remainingSecs));
				}
			}

			if (m_clanMailCooldownTimer != null)
			{
				int remainingSecs = m_clanMailCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("clan_mail_time", new LogicJSONNumber(remainingSecs));
				}
			}

			if (m_replayShareCooldownTimer != null)
			{
				int remainingSecs = m_replayShareCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("share_replay_time", new LogicJSONNumber(remainingSecs));
				}
			}

			if (m_elderKickCooldownTimer != null)
			{
				int remainingSecs = m_elderKickCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("elder_kick_time", new LogicJSONNumber(remainingSecs));
				}
			}

			if (m_challengeCooldownTimer != null)
			{
				int remainingSecs = m_challengeCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("challenge_time", new LogicJSONNumber(remainingSecs));
				}
			}

			if (m_arrangeWarCooldownTimer != null)
			{
				int remainingSecs = m_arrangeWarCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

				if (remainingSecs > 0)
				{
					jsonObject.Put("arrwar_time", new LogicJSONNumber(remainingSecs));
				}
			}
		}

		public LogicCharacter ClosestAttacker(bool flyingTroop)
		{
			LogicGameObjectManager gameObjectManager = m_parent.GetLevel().GetGameObjectManagerAt(0);
			LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(LogicGameObjectType.CHARACTER);

			int closestDistance = 0x7fffffff;
			LogicCharacter closestCharacter = null;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicCharacter character = (LogicCharacter)gameObjects[i];
				LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();
				LogicCombatComponent combatComponent = character.GetCombatComponent();

				bool deployTime = combatComponent != null && combatComponent.GetUndergroundTime() > 0;

				if (!deployTime && (LogicDataTables.GetGlobals().SkeletonOpenClanCastle() || !LogicDataTables.IsSkeleton(character.GetCharacterData())))
				{
					if (hitpointComponent != null)
					{
						if (character.IsAlive() && character.IsFlying() == flyingTroop && hitpointComponent.GetTeam() == 0)
						{
							int distance = character.GetPosition().GetDistanceSquaredTo(m_parent.GetMidX(), m_parent.GetMidY());

							if (distance < closestDistance)
							{
								closestDistance = distance;
								closestCharacter = character;
							}
						}
					}
				}
			}

			return closestCharacter;
		}

		public override void Tick()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null)
			{
				m_updateAvatarCooldown += 64;

				if (m_updateAvatarCooldown > 1000)
				{
					homeOwnerAvatar.UpdateStarBonusLimitCooldown();
					homeOwnerAvatar.UpdateLootLimitCooldown();

					m_updateAvatarCooldown -= 1000;
				}
			}

			if (m_parent.IsAlive())
			{
				if (!IsEmpty())
				{
					if (m_bunkerSearchTime > 0)
					{
						m_bunkerSearchTime -= 64;
					}
					else
					{
						bool airTriggered = false;
						bool groundLocked = false;

						if (m_team == 1)
						{
							bool inAirDistance = false;
							bool inGroundDistance = false;

							int clanCastleRadius = LogicDataTables.GetGlobals().GetClanCastleRadius();

							if (LogicDataTables.GetGlobals().CastleTroopTargetFilter())
							{
								LogicCharacter closestGroundAttacker = ClosestAttacker(false);
								LogicCharacter closestAirAttacker = ClosestAttacker(true);

								if (closestAirAttacker != null)
								{
									inAirDistance = closestAirAttacker.GetPosition().GetDistanceSquaredTo(m_parent.GetX(), m_parent.GetY()) <
													clanCastleRadius * clanCastleRadius;
								}

								if (closestGroundAttacker != null)
								{
									inGroundDistance = closestGroundAttacker.GetPosition().GetDistanceSquaredTo(m_parent.GetX(), m_parent.GetY()) <
													   clanCastleRadius * clanCastleRadius;
								}
							}
							else
							{
								LogicCharacter closestAttacker =
									(LogicCharacter)m_parent.GetLevel().GetGameObjectManager()
														 .GetClosestGameObject(m_parent.GetX(), m_parent.GetY(), m_filter);

								if (closestAttacker != null)
								{
									inAirDistance = inGroundDistance = closestAttacker.GetPosition().GetDistanceSquaredTo(m_parent.GetX(), m_parent.GetY()) <
																	   clanCastleRadius * clanCastleRadius;
								}
							}

							groundLocked = !inGroundDistance;
							airTriggered = inAirDistance;

							if (!airTriggered && groundLocked)
							{
								m_bunkerSearchTime = LogicDataTables.GetGlobals().GetBunkerSearchTime();
								return;
							}
						}
						else
						{
							airTriggered = true;
						}

						LogicCharacterData spawnData = null;
						int spawnLevel = -1;

						for (int i = 0; i < GetUnitTypeCount(); i++)
						{
							LogicCombatItemData data = GetUnitType(i);

							if (data != null)
							{
								int count = GetUnitCount(i);

								if (count > 0)
								{
									int upgLevel = GetUnitLevel(i);

									if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
									{
										LogicCharacterData characterData = (LogicCharacterData)data;
										LogicAttackerItemData attackerItemData = characterData.GetAttackerItemData(upgLevel);

										if (!(airTriggered & groundLocked) || attackerItemData.GetTrackAirTargets(false))
										{
											if (airTriggered | groundLocked || attackerItemData.GetTrackGroundTargets(false))
											{
												RemoveUnits(data, upgLevel, 1);

												spawnData = characterData;
												spawnLevel = upgLevel;
											}
										}
									}
								}
							}

							if (spawnData != null)
							{
								break;
							}
						}

						if (spawnData != null)
						{
							LogicCharacter character =
								(LogicCharacter)LogicGameObjectFactory.CreateGameObject(spawnData, m_parent.GetLevel(), m_parent.GetVillageType());

							character.GetHitpointComponent().SetTeam(m_team);

							if (character.GetChildTroops() != null)
							{
								LogicArrayList<LogicCharacter> childrens = character.GetChildTroops();

								for (int i = 0; i < childrens.Size(); i++)
								{
									childrens[i].GetHitpointComponent().SetTeam(m_team);
								}
							}

							character.SetUpgradeLevel(spawnLevel == -1 ? 0 : spawnLevel);
							character.SetAllianceUnit();

							if (character.GetCharacterData().IsJumper())
							{
								character.GetMovementComponent().EnableJump(3600000);
							}

							if (m_team == 1)
							{
								if (LogicDataTables.GetGlobals().EnableDefendingAllianceTroopJump())
								{
									character.GetMovementComponent().EnableJump(3600000);
								}

								if (LogicDataTables.GetGlobals().AllianceTroopsPatrol())
								{
									character.GetCombatComponent().SetSearchRadius(LogicDataTables.GetGlobals().GetClanCastleRadius() >> 9);

									if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
									{
										character.GetMovementComponent().SetBaseBuilding((LogicBuilding)m_parent);
									}
								}
							}
							else
							{
								LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();

								visitorAvatar.RemoveAllianceUnit(spawnData, spawnLevel);
								visitorAvatar.GetChangeListener().AllianceUnitRemoved(spawnData, spawnLevel);

								LogicBattleLog battleLog = m_parent.GetLevel().GetBattleLog();

								battleLog.IncrementDeployedAllianceUnits(spawnData, 1, spawnLevel);
								battleLog.SetAllianceUsed(true);
							}

							if (m_team == 1)
							{
								int spawnOffsetX = 0;
								int spawnOffsetY = 0;

								switch (m_troopSpawnOffset)
								{
									case 0:
										spawnOffsetX = 1;
										spawnOffsetY = 0;
										break;
									case 1:
										spawnOffsetX = -1;
										spawnOffsetY = 0;
										break;
									case 2:
										spawnOffsetX = 0;
										spawnOffsetY = 1;
										break;
									case 3:
										spawnOffsetX = 0;
										spawnOffsetY = -1;
										break;
								}

								character.SetInitialPosition(m_parent.GetMidX() + ((m_parent.GetWidthInTiles() << 8) - 128) * spawnOffsetX,
															 m_parent.GetMidY() + ((m_parent.GetHeightInTiles() << 8) - 128) * spawnOffsetY);

								if (++m_troopSpawnOffset > 3)
								{
									m_troopSpawnOffset = 0;
								}
							}
							else if (LogicDataTables.GetGlobals().AllowClanCastleDeployOnObstacles())
							{
								int posX = m_parent.GetX() + (m_parent.GetWidthInTiles() << 9) - 128;
								int posY = m_parent.GetY() + (m_parent.GetHeightInTiles() << 8);

								if (LogicGamePlayUtil.GetNearestValidAttackPos(m_parent.GetLevel(), posX, posY, out int outputX, out int outputY))
								{
									character.SetInitialPosition(outputX, outputY);
								}
								else
								{
									character.SetInitialPosition(posX, posY);
								}
							}
							else
							{
								character.SetInitialPosition(m_parent.GetX() + (m_parent.GetWidthInTiles() << 9) - 128,
															 m_parent.GetY() + (m_parent.GetHeightInTiles() << 8));
							}

							m_parent.GetGameObjectManager().AddGameObject(character, -1);
						}

						m_bunkerSearchTime = LogicDataTables.GetGlobals().GetBunkerSearchTime();
					}
				}
			}
		}

		public override void FastForwardTime(int time)
		{
			if (m_requestCooldownTimer != null)
			{
				m_requestCooldownTimer.FastForward(time);
			}

			if (m_replayShareCooldownTimer != null)
			{
				m_replayShareCooldownTimer.FastForward(time);
			}

			if (m_elderKickCooldownTimer != null)
			{
				m_elderKickCooldownTimer.FastForward(time);
			}

			if (m_clanMailCooldownTimer != null)
			{
				m_clanMailCooldownTimer.FastForward(time);
			}

			if (m_challengeCooldownTimer != null)
			{
				m_challengeCooldownTimer.FastForward(time);
			}

			if (m_arrangeWarCooldownTimer != null)
			{
				m_arrangeWarCooldownTimer.FastForward(time);
			}

			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null)
			{
				homeOwnerAvatar.FastForwardStarBonusLimit(time);
				homeOwnerAvatar.FastForwardLootLimit(time);
			}
		}

		public LogicArrayList<LogicVector2> GetPatrolPath()
			=> m_patrolPath;

		public LogicArrayList<LogicVector2> CreatePatrolPath()
		{
			int width = m_parent.GetWidthInTiles() << 8;
			int height = m_parent.GetHeightInTiles() << 8;

			if (width * width + height * height <= 0x240000)
			{
				int midX = m_parent.GetMidX();
				int midY = m_parent.GetMidY();

				LogicVector2 tmp1 = new LogicVector2();
				LogicVector2 tmp2 = new LogicVector2();
				LogicVector2 tmp3 = new LogicVector2();
				LogicVector2 tmp4 = new LogicVector2();

				tmp2.Set(midX, midY);

				LogicArrayList<LogicVector2> wayPoints = new LogicArrayList<LogicVector2>(LogicBunkerComponent.PATROL_PATHS);

				for (int i = 0, j = 360; i < LogicBunkerComponent.PATROL_PATHS; i++, j += 720)
				{
					tmp1.Set(midX + LogicMath.Cos(j >> 5, 1536), midY + LogicMath.Sin(j >> 5, 1536));
					LogicHeroBaseComponent.FindPoint(m_parent.GetLevel().GetTileMap(), tmp3, tmp2, tmp1, tmp4);
					wayPoints.Add(new LogicVector2(tmp4.m_x, tmp4.m_y));
				}

				tmp1.Destruct();
				tmp2.Destruct();
				tmp3.Destruct();
				tmp4.Destruct();

				return wayPoints;
			}
			else
			{
				int startX = m_parent.GetX() + (m_parent.GetWidthInTiles() << 9) - 128;
				int startY = m_parent.GetY() + (m_parent.GetWidthInTiles() << 9) - 128;
				int endX = m_parent.GetX() + 128;
				int endY = m_parent.GetY() + 128;

				LogicArrayList<LogicVector2> wayPoints = new LogicArrayList<LogicVector2>(4);

				wayPoints.Add(new LogicVector2(startX, startY));
				wayPoints.Add(new LogicVector2(endX, startY));
				wayPoints.Add(new LogicVector2(endX, endY));
				wayPoints.Add(new LogicVector2(startX, endY));

				return wayPoints;
			}
		}
	}
}