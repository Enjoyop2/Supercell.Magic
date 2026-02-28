using Supercell.Magic.Logic.Calendar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicMovementComponent : LogicComponent
	{
		private int m_jumpTime; // 224
		private int m_wallCount; // 212
		private int m_patrolPathTime; // 216
		private int m_pathLifetime; // 200
		private int m_attackPositionRandom; // 220
		private int m_patrolPoint; // 228

		private long m_closerDistance; // 192

		private int m_targetStartX; // 184
		private int m_targetStartY; // 188
		private int m_alertPositionX; // 240
		private int m_alertPositionY; // 244

		private bool m_moving; // 254
		private bool m_notPassablePos; // 255
		private bool m_patrolEnabled; // 256
		private bool m_alerted; // 257
		private bool m_hasAlerted; // 258

		private bool m_underground; // 252
		private bool m_flying; // 253

		private readonly LogicVector2 m_attackPosition; // 204
		private readonly LogicMovementSystem m_movementSystem; // 12

		private LogicRandom m_random; // 248
		private LogicBuilding m_baseBuilding; // 232
		private LogicArrayList<LogicGameObject> m_nearestBuildings;

		public LogicMovementComponent(LogicGameObject gameObject, int speed, bool flying, bool underground) : base(gameObject)
		{
			m_attackPosition = new LogicVector2();
			m_movementSystem = new LogicMovementSystem();

			m_targetStartX = -1;
			m_targetStartY = -1;

			if (underground)
			{
				int minerSpeedRandomPercentage = LogicDataTables.GetGlobals().GetMinerSpeedRandomPercentage();

				if (minerSpeedRandomPercentage > 0)
				{
					speed = speed * (gameObject.Rand(100) % minerSpeedRandomPercentage + 100) / 100;
				}
			}

			m_movementSystem.Init(speed, this, null);

			m_flying = flying;
			m_underground = underground;
		}

		public void SetSpeed(int speed)
		{
			m_movementSystem.SetSpeed(speed);
		}

		public void SetUnderground(bool value)
		{
			m_underground = value;
		}

		public override void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			base.RemoveGameObjectReferences(gameObject);

			if (m_baseBuilding == gameObject)
			{
				m_baseBuilding = null;
			}
		}

		public LogicBuilding GetBaseBuilding()
			=> m_baseBuilding;

		public void SetBaseBuilding(LogicBuilding building)
		{
			m_baseBuilding = building;
		}

		public int GetClosestPatrolPoint(int x, int y, LogicArrayList<LogicVector2> patrolPath)
		{
			int closestDistance = -1;
			int closestIdx = 0;

			for (int i = 0; i < patrolPath.Size(); i++)
			{
				int distanceSquared = patrolPath[i].GetDistanceSquaredTo(x, y);

				if (distanceSquared < closestDistance || closestDistance < 0)
				{
					closestIdx = i;
					closestDistance = distanceSquared;
				}
			}

			return closestIdx;
		}

		public int EvaluateTargetCost(LogicGameObject target)
		{
			bool move = true;
			LogicMovementComponent movementComponent = target.GetMovementComponent();

			if (movementComponent == null)
			{
				move = target.GetGameObjectType() == LogicGameObjectType.CHARACTER && ((LogicCharacter)target).GetParent() != null;
			}

			if (!m_flying && !m_underground && move)
			{
				if (movementComponent != null && movementComponent.m_notPassablePos)
				{
					return 0x7FFFFFFF;
				}
			}

			MoveTo(target);

			if (m_jumpTime <= 0 || move || m_parent.GetLevel().GetTileMap().IsPassablePathFinder(m_targetStartX >> 8, m_targetStartY >> 8))
			{
				m_wallCount = m_movementSystem.GetWallCount();

				if (m_wallCount > 0 && m_parent.GetHitpointComponent().GetTeam() == 1 && m_jumpTime <= 0)
				{
					return 0x7FFFFFFF;
				}

				int jumpCost = LogicDataTables.GetGlobals().UseWallWeightsForJumpSpell() ? 10000 : 0;
				int wallCost = m_jumpTime <= jumpCost ? 3584 * m_wallCount : 0;

				return m_movementSystem.GetPathLength() + wallCost;
			}

			return 0x7FFFFFFF;
		}

		public bool EnableUnderground()
		{
			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;

				if (character.GetCharacterData().IsUnderground())
				{
					if (character.GetHitpointComponent().GetTeam() == 0)
					{
						m_parent.GetCombatComponent().SetUndergroundTime(3600000);
						return true;
					}
				}
			}

			return false;
		}

		public void EnableJump(int ms)
		{
			if (m_parent.GetGameObjectType() != LogicGameObjectType.CHARACTER || !((LogicCharacter)m_parent).GetCharacterData().IsUnderground() ||
				m_parent.GetHitpointComponent().GetTeam() != 0)
			{
				if (!m_flying)
				{
					if (m_jumpTime <= 0)
					{
						m_parent.GetCombatComponent().ForceNewTarget();
					}

					m_jumpTime = ms;
				}
			}
		}

		public bool IsFlying()
			=> m_flying;

		public void SetFlying(bool value)
		{
			m_flying = value;
		}

		public bool IsInNotPassablePosition()
			=> m_notPassablePos;

		public bool GetPatrolEnabled()
			=> m_patrolEnabled;

		public bool IsUnderground()
			=> m_underground;

		public int GetJump()
			=> m_jumpTime;

		public bool CanJumpWall()
			=> m_jumpTime > (LogicDataTables.GetGlobals().UseWallWeightsForJumpSpell() ? 10000 : 0);

		public LogicMovementSystem GetMovementSystem()
			=> m_movementSystem;

		public void AvoidPoison()
		{
			if (m_movementSystem.NotMoving())
			{
				LogicHitpointComponent hitpointComponent = m_parent.GetHitpointComponent();

				if (hitpointComponent != null && hitpointComponent.GetPoisonRemainingMS() > 0)
				{
					InitRandom();

					int posX = m_parent.GetMidX();
					int posY = m_parent.GetMidY();

					LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

					for (int i = 0; i < 25; i++)
					{
						int posOffset = m_random.Rand(i << 8);
						int posAngle = m_random.Rand(360);

						int randomX = posX + ((posOffset * LogicMath.Sin(posAngle)) >> 10);
						int randomY = posY + ((posOffset * LogicMath.Cos(posAngle)) >> 10);

						if (tileMap.IsPassablePathFinder(randomX >> 8, randomY >> 8))
						{
							LogicArrayList<LogicGameObject> gameObjects = m_parent.GetGameObjectManager().GetGameObjects(LogicGameObjectType.SPELL);

							bool noPoison = true;

							for (int j = 0; j < gameObjects.Size(); j++)
							{
								LogicSpell spell = (LogicSpell)gameObjects[j];
								LogicSpellData data = spell.GetSpellData();

								int radius = data.GetRadius(spell.GetUpgradeLevel());
								int distanceX = randomX - spell.GetMidX();
								int distanceY = randomY - spell.GetMidY();

								if (distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
								{
									if (data.GetPoisonDamage(0) > 0)
									{
										noPoison = false;
										break;
									}
								}
							}

							if (noPoison)
							{
								MoveTo(randomX, randomY);
								break;
							}
						}
					}
				}
			}
		}

		public override void Tick()
		{
			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

			if (!combatComponent.GetUnk596())
			{
				if (combatComponent.GetTarget(0) == null && !m_notPassablePos && !m_patrolEnabled && combatComponent.GetUndergroundTime() <= 0)
				{
					if (LogicDataTables.GetGlobals().UsePoisonAvoidance())
					{
						LogicHitpointComponent hitpointComponent = m_parent.GetHitpointComponent();

						if (hitpointComponent != null && hitpointComponent.GetPoisonRemainingMS() > 0 && hitpointComponent.GetTeam() == 1 && !m_parent.IsFrozen())
						{
							AvoidPoison();
						}
						else
						{
							m_moving = false;
							m_movementSystem.ClearPath();
						}
					}
					else
					{
						m_moving = false;
						m_movementSystem.ClearPath();
					}
				}
			}

			if (m_moving)
			{
				if (m_pathLifetime > 0)
				{
					if (m_notPassablePos)
					{
						m_pathLifetime = LogicMath.Max(m_pathLifetime - 64, 1);
					}
					else
					{
						m_pathLifetime = LogicMath.Max(m_pathLifetime - 64, 0);

						if (m_pathLifetime <= 0)
						{
							NewTargetFound();
						}
					}
				}
			}

			if (!m_movementSystem.NotMoving())
			{
				CheckTriggers();
			}

			if (m_patrolEnabled)
			{
				if (m_baseBuilding != null)
				{
					if (m_baseBuilding.GetHeroBaseComponent() != null || m_baseBuilding.GetBunkerComponent() != null)
					{
						if (!m_notPassablePos)
						{
							int distanceSquaredToEnd = m_movementSystem.GetDistSqToEnd();

							if (distanceSquaredToEnd <= 0xFFFF)
							{
								m_movementSystem.ClearPath();

								if (m_alerted)
								{
									int alertDistanceX = m_alertPositionX - m_parent.GetMidX();
									int alertDistanceY = m_alertPositionY - m_parent.GetMidY();

									m_movementSystem.SetDirection(LogicMath.GetAngle(alertDistanceX, alertDistanceY));

									if (m_hasAlerted)
									{
										m_hasAlerted = false;
										// Listener.
									}
								}

								m_patrolPathTime += 64;
							}
						}

						if (m_patrolPathTime > 2000)
						{
							m_patrolPathTime = 0;

							if (!m_parent.IsHero())
							{
								m_patrolPathTime = LogicMath.Clamp(30 * m_movementSystem.GetSpeed() - 100, 0, 800) +
														(byte)m_parent.GetCombatComponent().Rand(m_parent.GetX());
							}

							LogicArrayList<LogicVector2> patrolPath = m_baseBuilding.GetHeroBaseComponent() != null
								? m_baseBuilding.GetHeroBaseComponent().GetPatrolPath()
								: m_baseBuilding.GetBunkerComponent() != null
									? m_baseBuilding.GetBunkerComponent().GetPatrolPath()
									: null;

							int pointIdx;

							if (m_alerted)
							{
								pointIdx = GetClosestPatrolPoint(m_alertPositionX, m_alertPositionY, patrolPath);
							}
							else if (m_parent.IsHero())
							{
								pointIdx = GetClosestPatrolPoint(m_parent.GetPosition().m_x, m_parent.GetPosition().m_y, patrolPath) + 1;
							}
							else
							{
								if (m_random == null)
								{
									m_random = new LogicRandom();
									m_random.SetIteratedRandomSeed(m_parent.GetGlobalID());
								}

								pointIdx = m_random.Rand(patrolPath.Size());
							}

							if (pointIdx < 0)
							{
								pointIdx = patrolPath.Size() - 1;
							}

							if (pointIdx >= patrolPath.Size())
							{
								pointIdx = 0;
							}

							for (int i = 0; i < patrolPath.Size(); i++)
							{
								int point = (pointIdx + i) % patrolPath.Size();

								if (point != m_patrolPoint)
								{
									int distanceSquared = patrolPath[point].GetDistanceSquared(m_parent.GetPosition());

									if (distanceSquared > 0x10000)
									{
										m_patrolPoint = point;
										MoveToPoint(patrolPath[point]);

										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public override void SubTick()
		{
			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

			if (combatComponent.GetAttackFinished())
			{
				if (m_moving)
				{
					LogicVector2 position = m_movementSystem.GetPosition();

					if (m_flying || m_parent.GetLevel().GetTileMap().IsPassablePathFinder(position.m_x >> 8, position.m_y >> 8))
					{
						LogicGameObject target = combatComponent.GetTarget(0);

						if (target != null)
						{
							m_movementSystem.SetDirection(LogicMath.GetAngle(target.GetMidX() - position.m_x, target.GetMidY() - position.m_y));
							m_movementSystem.ClearPath();

							if (target.GetMovementComponent() != null ||
								target.GetGameObjectType() == LogicGameObjectType.CHARACTER && ((LogicCharacter)target).GetParent() != null)
							{
								m_pathLifetime = 100;
							}
							else
							{
								m_moving = false;
							}
						}
						else
						{
							m_movementSystem.ClearPath();
							m_moving = false;
						}
					}
				}
			}


			if (m_parent.IsAlive())
			{
				m_movementSystem.SubTick();

				if (m_jumpTime > 0)
				{
					LogicVector2 position = m_movementSystem.GetPosition();
					bool passablePathFinder = m_parent.GetLevel().GetTileMap().IsPassablePathFinder(position.m_x >> 8, position.m_y >> 8);

					m_jumpTime = LogicMath.Max(m_jumpTime - 16, 0);
					m_notPassablePos = passablePathFinder ^ true;

					if (m_jumpTime == 0)
					{
						if (passablePathFinder)
						{
							m_parent.GetCombatComponent().ForceNewTarget();
						}
						else
						{
							m_jumpTime = 1;
						}
					}
				}
			}
		}

		public void CheckTriggers()
		{
			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;

				if (!character.GetCharacterData().GetTriggersTraps())
				{
					return;
				}
			}

			LogicVector2 position = m_movementSystem.GetPosition();

			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					LogicTile tile = m_parent.GetLevel().GetTileMap().GetTile((position.m_x >> 9) + i, (position.m_y >> 9) + j);

					if (tile != null)
					{
						for (int k = 0; k < tile.GetGameObjectCount(); k++)
						{
							LogicGameObject gameObject = tile.GetGameObject(k);

							if (!m_movementSystem.IsPushed() || !m_movementSystem.IgnorePush())
							{
								LogicTriggerComponent triggerComponent = gameObject.GetTriggerComponent();

								if (triggerComponent != null && !triggerComponent.IsTriggeredByRadius())
								{
									triggerComponent.ObjectClose(m_parent);
								}
							}
						}
					}
				}
			}
		}

		public void NewTargetFound()
		{
			LogicGameObject target = m_parent.GetCombatComponent().GetTarget(0);

			if (target != null)
			{
				if (LogicDataTables.GetGlobals().RepathDuringFly() || !m_movementSystem.IsPushed() || !m_movementSystem.IgnorePush())
				{
					if (m_parent.GetCombatComponent().IsInRange(target))
					{
						m_moving = true;

						if (m_patrolEnabled)
						{
							// Listener.
						}

						m_patrolEnabled = false;
						m_alerted = false;
						m_targetStartX = m_parent.GetMidX();
						m_targetStartY = m_parent.GetMidY();

						m_movementSystem.AddPoint(m_targetStartX, m_targetStartY);
						m_movementSystem.SetDirection(LogicMath.GetAngle(target.GetMidX() - m_parent.GetMidX(), target.GetMidY() - m_parent.GetMidY()));

						if (target.GetMovementComponent() != null)
						{
							m_pathLifetime = 1;
						}
						else if (target.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							m_pathLifetime = ((LogicCharacter)target).GetParent() != null ? 1 : 0;
						}
					}
					else
					{
						MoveTo(target);

						LogicGameObject wall = m_movementSystem.GetWall();

						if (wall != null)
						{
							if (m_jumpTime <= 0)
							{
								if (!wall.IsWall() || ((LogicBuilding)wall).GetHitWallDelay() <= 0)
								{
									m_parent.GetCombatComponent().ObstacleToDestroy(wall);
									m_pathLifetime = 0;
								}
							}
						}
					}
				}
			}
		}

		public void NoTargetFound()
		{
			LogicHitpointComponent hitpointComponent = m_parent.GetHitpointComponent();

			if (hitpointComponent == null || hitpointComponent.GetTeam() != 0)
			{
				LogicGameObject patrolPost = m_movementSystem.GetPatrolPost();

				if (patrolPost != null)
				{
					LogicCalendarBuildingDestroyedSpawnUnit buildingDestroyedSpawnUnit = m_parent.GetLevel().GetCalendar().GetBuildingDestroyedSpawnUnit();
					LogicBuildingData buildingData = buildingDestroyedSpawnUnit?.GetBuildingData();

					if (patrolPost.GetDefenceUnitProduction() != null || patrolPost.GetData() == buildingData)
					{
						m_movementSystem.UpdatePatrolArea(m_parent.GetLevel());
						m_patrolEnabled = true;
					}
				}

				if (LogicDataTables.GetGlobals().AllianceTroopsPatrol() || m_parent.IsHero())
				{
					bool prevPatrolEnabled = m_patrolEnabled;
					m_patrolEnabled = true;

					if (m_baseBuilding != null)
					{
						LogicHeroBaseComponent heroBaseComponent = m_baseBuilding.GetHeroBaseComponent();

						if (heroBaseComponent != null)
						{
							LogicArrayList<LogicGameObject> gameObjects = m_parent.GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER);

							int alertRadius = heroBaseComponent.GetHeroData().GetAlertRadius() >> 9;
							int alertRadiusSquared = alertRadius * alertRadius;

							uint minDistance = 0xFFFFFFFF;

							LogicGameObject closestGameObject = null;

							for (int i = 0; i < gameObjects.Size(); i++)
							{
								LogicCharacter character = (LogicCharacter)gameObjects[i];
								LogicHitpointComponent characterHitpointComponent = character.GetHitpointComponent();

								if (characterHitpointComponent != null && characterHitpointComponent.GetTeam() == 1 ||
									!character.GetAttackerItemData().GetTrackAirTargets(false) && character.IsFlying() || !character.IsAlive())
								{
									continue;
								}

								int distanceX = (character.GetMidX() - m_baseBuilding.GetMidX()) >> 9;
								int distanceY = (character.GetMidY() - m_baseBuilding.GetMidY()) >> 9;

								uint distanceSquared = (uint)(distanceX * distanceX + distanceY * distanceY);

								if (distanceSquared < alertRadiusSquared && minDistance > distanceSquared)
								{
									minDistance = distanceSquared;
									closestGameObject = character;
								}
							}

							bool prevAlerted = m_alerted;
							m_alerted = closestGameObject != null;

							if (closestGameObject != null && prevPatrolEnabled && !prevAlerted)
							{
								m_hasAlerted = true;
							}

							if (closestGameObject != null)
							{
								m_alertPositionX = closestGameObject.GetMidX();
								m_alertPositionY = closestGameObject.GetMidY();
							}

							if (!prevPatrolEnabled)
							{
								m_movementSystem.ClearPath();
							}
						}
					}
				}
			}
		}

		public void MoveTo(LogicGameObject gameObject)
		{
			EnableUnderground();

			m_moving = true;

			LogicMovementComponent movementComponent = gameObject.GetMovementComponent();

			if (movementComponent != null || gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				if (movementComponent == null)
				{
					LogicCharacter character = (LogicCharacter)gameObject;

					if (character.GetParent() == null)
					{
						goto DEFAULT_MOVE;
					}
				}

				if (m_patrolEnabled)
				{
					m_parent.GetListener().SpottedEnemy();
				}

				m_patrolEnabled = false;
				m_alerted = false;

				if (FindGoodAttackPosAround(gameObject, GetAttackDist(), false))
				{
					if (m_underground || m_flying)
					{
						m_movementSystem.AddPoint(m_targetStartX, m_targetStartY);
					}
					else
					{
						m_movementSystem.MoveTo(m_targetStartX, m_targetStartY, m_parent.GetLevel().GetTileMap(), true);
					}

					m_pathLifetime = LogicMath.Max(250 * m_movementSystem.GetPathLength() / 512, 1);
				}
				else
				{
					m_pathLifetime = 1000;
					m_movementSystem.ClearPath();
				}

				return;
			}

		DEFAULT_MOVE:

			if (m_patrolEnabled)
			{
				m_parent.GetListener().SpottedEnemy();
			}

			m_alerted = false;
			m_patrolEnabled = false;

			if (FindGoodAttackPosAround(gameObject, GetAttackDist(), false))
			{
				if (m_underground || m_flying)
				{
					m_movementSystem.AddPoint(m_targetStartX, m_targetStartY);
				}
				else
				{
					m_movementSystem.MoveTo(m_targetStartX, m_targetStartY, m_parent.GetLevel().GetTileMap(), true);
				}
			}

			m_pathLifetime = 0;
		}

		public void MoveTo(int x, int y)
		{
			EnableUnderground();
			m_moving = true;

			if (m_patrolEnabled)
			{
				m_parent.GetListener().SpottedEnemy();
			}

			m_patrolEnabled = false;
			m_alerted = false;

			if (m_underground || m_flying)
			{
				m_movementSystem.AddPoint(m_targetStartX, m_targetStartY);
			}
			else
			{
				LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

				if (tileMap.IsPassablePathFinder(x >> 8, y >> 8))
				{
					m_movementSystem.MoveTo(x, y, tileMap, true);
				}
				else
				{
					LogicVector2 nearestPassablePosition = new LogicVector2();

					if (tileMap.GetNearestPassablePosition(x, y, nearestPassablePosition, 512) ||
						tileMap.GetNearestPassablePosition(x, y, nearestPassablePosition, 1536))
					{
						m_movementSystem.MoveTo(nearestPassablePosition.m_x, nearestPassablePosition.m_y, tileMap, true);
					}
					else
					{
						m_movementSystem.ClearPath();
					}
				}
			}

			m_pathLifetime = 0;
		}

		public bool FindGoodAttackPosAround(LogicGameObject gameObject, int attackDistance, bool doNotOverride)
		{
			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

			if (combatComponent.IsInRange(gameObject))
			{
				m_targetStartX = m_parent.GetMidX();
				m_targetStartY = m_parent.GetMidY();

				return true;
			}

			if (m_flying)
			{
				int midX = gameObject.GetMidX();
				int midY = gameObject.GetMidY();

				if (gameObject.GetMovementComponent() != null)
				{
					m_attackPosition.m_x = m_parent.GetMidX() - midX;
					m_attackPosition.m_y = m_parent.GetMidY() - midY;

					m_attackPosition.Normalize(attackDistance);
				}
				else
				{
					int randomAngle = combatComponent.Rand(29) % 360;

					m_attackPosition.m_x = LogicMath.Sin(randomAngle, attackDistance);
					m_attackPosition.m_y = LogicMath.Cos(randomAngle, attackDistance);
				}

				m_targetStartX = midX + m_attackPosition.m_x;
				m_targetStartY = midY + m_attackPosition.m_y;

				return true;
			}

			m_attackPositionRandom = 7;

			if (gameObject.IsWall())
			{
				m_attackPositionRandom = 0;
			}

			if (m_parent.IsHero() && !LogicDataTables.GetGlobals().HeroUsesAttackPosRandom())
			{
				m_attackPositionRandom = 0;
			}

			if (m_parent.GetCombatComponent().GetTotalTargets() == 0 && !LogicDataTables.GetGlobals().UseAttackPosRandomOn1stTarget())
			{
				m_attackPositionRandom = 0;
			}

			bool attackOverWalls = true;

			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;

				if (character.IsWallBreaker())
				{
					m_attackPositionRandom = 0;
				}

				attackOverWalls = character.GetCharacterData().GetAttackOverWalls();
			}

			m_closerDistance = 0x7FFFFFFFFFFFFFFF;
			m_targetStartX = -1;
			m_targetStartY = -1;

			int width = (gameObject.GetWidthInTiles() << 8) - (gameObject.PassableSubtilesAtEdge() << 8);
			int height = (gameObject.GetHeightInTiles() << 8) - (gameObject.PassableSubtilesAtEdge() << 8);

			bool success1 = ProcessLine(attackDistance, 0, height + attackDistance, width, height + attackDistance, gameObject, doNotOverride, false, attackOverWalls);
			bool success2 = ProcessLine(attackDistance, width, height, gameObject, doNotOverride, true, attackOverWalls);
			bool success3 = ProcessLine(attackDistance, width + attackDistance, height, width + attackDistance, 0, gameObject, doNotOverride, false, attackOverWalls);

			if (success1 && success2 && success3)
			{
				int gameObjectX = gameObject.GetMidX();
				int gameObjectY = gameObject.GetMidY();

				LogicTileMap tileMap = gameObject.GetLevel().GetTileMap();
				LogicVector2 wallPosition = new LogicVector2();

				if (tileMap.GetWallInPassableLine(gameObjectX, gameObjectY, m_parent.GetMidX(), m_parent.GetMidY(), wallPosition))
				{
					int wallDistanceX = wallPosition.m_x - gameObjectX;
					int wallDistanceY = wallPosition.m_y - gameObjectY;
					int gameObjectWidth = gameObject.GetWidthInTiles() << 8;
					int gameObjectHeight = gameObject.GetHeightInTiles() << 8;

					int distanceX;
					int distanceY;

					if (wallDistanceX >= gameObjectWidth)
					{
						distanceX = wallDistanceX - gameObjectWidth;
					}
					else
					{
						distanceX = wallDistanceX - width;

						if (distanceX < 0)
						{
							distanceX = 0;
						}
					}

					if (wallDistanceY >= gameObjectHeight)
					{
						distanceY = wallDistanceY - gameObjectHeight;
					}
					else
					{
						distanceY = wallDistanceY - height;

						if (distanceY < 0)
						{
							distanceY = 0;
						}
					}

					if (distanceX * distanceX + distanceY * distanceY > (uint)attackDistance * attackDistance)
					{
						LogicVector2 tmp = new LogicVector2();

						tmp.Set(distanceX, distanceY);
						tmp.Normalize(attackDistance);

						wallPosition.Set(tmp.m_x + gameObjectX, tmp.m_y + gameObjectY);
					}

					m_closerDistance = (wallPosition.m_x - m_parent.GetMidX()) * (wallPosition.m_x - m_parent.GetMidX()) +
											(wallPosition.m_y - m_parent.GetMidY()) * (wallPosition.m_y - m_parent.GetMidY());

					if (tileMap.GetNearestPassablePosition(wallPosition.m_x, wallPosition.m_y, wallPosition, LogicDataTables.GetGlobals().UseNewPathFinder() ? 512 : 256))
					{
						m_targetStartX = wallPosition.m_x;
						m_targetStartY = wallPosition.m_y;
					}
					else
					{
						m_targetStartX = -1;
						m_targetStartY = -1;
					}
				}
			}

			return m_targetStartX != -1;
		}

		public bool ProcessLine(int radius, int startX, int startY, int destX, int destY, LogicGameObject gameObject, bool doNotOverride, bool ignoreNearestBuildings,
								bool attackOverWalls)
		{
			bool closest = false;

			if (doNotOverride && m_targetStartX != -1)
			{
				return false;
			}

			LogicVector2 position = m_movementSystem.GetPosition();
			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

			if (combatComponent.GetTargetGroupPosition() != null)
			{
				position = combatComponent.GetTargetGroupPosition();
			}

			int midX = gameObject.GetMidX();
			int midY = gameObject.GetMidY();

			int distance = LogicMath.Sqrt((destX - startX) * (destX - startX) + (destY - startY) * (destY - startY));
			int distanceTile = LogicMath.Max(1, (int)(distance + ((uint)((distance + 256) >> 31) >> 23) + 256) >> 9);

			LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

			int distanceX = destX - startX;
			int distanceY = destY - startY;

			for (int i = 0; i < distanceTile; i++)
			{
				int offsetX = startX + distanceX / distanceTile / 2;
				int offsetY = startY + distanceY / distanceTile / 2;

				bool closest1 = CheckIfCloser(gameObject, radius, position, midX, midY, offsetX, offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest2 = CheckIfCloser(gameObject, radius, position, midX, midY, -offsetX, offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest3 = CheckIfCloser(gameObject, radius, position, midX, midY, offsetX, -offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest4 = CheckIfCloser(gameObject, radius, position, midX, midY, -offsetX, -offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);

				closest = closest1 && closest2 && closest3 && closest4;

				if (doNotOverride)
				{
					if (m_targetStartX != -1)
					{
						break;
					}
				}

				distanceX += 2 * destX - 2 * startX;
				distanceY += 2 * destY - 2 * startY;
			}

			return closest;
		}

		public bool ProcessLine(int radius, int startX, int startY, LogicGameObject gameObject, bool doNotOverride, bool ignoreNearestBuildings, bool attackOverWalls)
		{
			if (doNotOverride && m_targetStartX != -1)
			{
				return false;
			}

			bool closest = true;

			LogicVector2 position = m_movementSystem.GetPosition();
			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

			if (combatComponent.GetTargetGroupPosition() != null)
			{
				position = combatComponent.GetTargetGroupPosition();
			}

			int midX = gameObject.GetMidX();
			int midY = gameObject.GetMidY();
			int distance = LogicMath.Max(1, (int)(((uint)((157 * radius / 100 + 256) >> 31) >> 23) + 157 * radius / 100 + 256) >> 9);

			LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

			int angle = 900 / distance;

			for (int i = 0, j = angle / 2; i < distance; i++, j += angle)
			{
				int offsetX = startX + LogicMath.Cos(j / 10, radius);
				int offsetY = startY + LogicMath.Sin(j / 10, radius);

				bool closest1 = CheckIfCloser(gameObject, radius, position, midX, midY, offsetX, offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest2 = CheckIfCloser(gameObject, radius, position, midX, midY, -offsetX, offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest3 = CheckIfCloser(gameObject, radius, position, midX, midY, offsetX, -offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);
				bool closest4 = CheckIfCloser(gameObject, radius, position, midX, midY, -offsetX, -offsetY, tileMap, ignoreNearestBuildings, attackOverWalls);

				closest = closest1 && closest2 && closest3 && closest4;

				if (doNotOverride)
				{
					if (m_targetStartX != -1)
					{
						break;
					}
				}
			}

			return closest;
		}

		public bool CheckIfCloser(LogicGameObject gameObject, int radius, LogicVector2 position, int midX, int midY, int offsetX, int offsetY, LogicTileMap tileMap,
								  bool ignoreNearestBuildings,
								  bool attackOverWalls)
		{
			int x = midX + offsetX;
			int y = midY + offsetY;
			int subTileX = x >> 8;
			int subTileY = y >> 8;
			int mapWidth = tileMap.GetSizeX() * 2;
			int mapHeight = tileMap.GetSizeY() * 2;

			if (subTileX >= mapWidth || subTileY >= mapHeight || (subTileX | subTileY) < 0)
			{
				return true;
			}

			long v71 = 0;

			if (!m_flying && !m_underground)
			{
				int pathFinderCost = tileMap.GetPathFinderCost(subTileX, subTileY);

				if (pathFinderCost == 0x7FFFFFFF)
				{
					return true;
				}

				if (pathFinderCost > 0)
				{
					v71 = mapWidth;
				}

				if (LogicDataTables.GetGlobals().TargetSelectionConsidersWallsOnPath())
				{
					if (tileMap.GetWallInPassableLine(position.m_x, position.m_y, x, y, new LogicVector2()))
					{
						v71 += 1000;
					}
				}

				if (!attackOverWalls)
				{
					LogicVector2 wallPosition = new LogicVector2();

					if (tileMap.GetWallInPassableLine(midX, midY, x, y, wallPosition))
					{
						x = wallPosition.m_x;
						y = wallPosition.m_y;
						subTileX = x >> 8;
						subTileY = y >> 8;

						int cost = tileMap.GetPathFinderCost(subTileX, subTileY);

						if (cost == 0x7FFFFFFF)
						{
							return true;
						}

						if (cost > 0)
						{
							v71 = mapWidth;
						}
					}

					wallPosition.Destruct();
				}

				if (!tileMap.IsPassablePathFinder(subTileX, subTileY))
				{
					LogicVector2 passablePosition = new LogicVector2();

					if (!tileMap.GetPassablePositionInLine(x, y, midX, midY, 512, passablePosition))
					{
						return true;
					}

					x = passablePosition.m_x;
					y = passablePosition.m_y;
					subTileX = x >> 8;
					subTileY = y >> 8;

					int cost = tileMap.GetPathFinderCost(subTileX, subTileY);

					if (cost == 0x7FFFFFFF)
					{
						return true;
					}

					if (cost > 0)
					{
						v71 = mapWidth;
					}
				}

				int blockedAttackPositionPenalty = LogicDataTables.GetGlobals().GetBlockedAttackPositionPenalty();

				if (blockedAttackPositionPenalty > 0)
				{
					LogicTile currentTile = tileMap.GetTile(m_parent.GetTileX(), m_parent.GetTileY());
					LogicTile endTile = tileMap.GetTile(x >> 9, y >> 9);

					if (currentTile != null && endTile != null)
					{
						if (currentTile.GetRoomIdx() != endTile.GetRoomIdx())
						{
							v71 += 2 * blockedAttackPositionPenalty;
						}
					}
				}
			}

			LogicCombatComponent combatComponent = m_parent.GetCombatComponent();
			LogicGameObject target = combatComponent.GetTarget(0);

			if (combatComponent.GetAttackMultipleBuildings() && target != null && target.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				int nearestBuildings = GetNearestBuildings(x, y, combatComponent.GetAttackRange(0, false), tileMap);

				if (LogicDataTables.GetGlobals().ValkyriePrefers4Buildings())
				{
					v71 += LogicMath.Clamp(4 - nearestBuildings, 0, 3) * 15;
				}
				else
				{
					if (ignoreNearestBuildings)
					{
						nearestBuildings = 1;
					}

					v71 += LogicMath.Clamp(2 - nearestBuildings, 0, 1) * 20;
				}
			}

			int posX = position.m_x - x;
			int posY = position.m_y - y;

			if (m_attackPositionRandom > 0)
			{
				LogicMovementComponent movementComponent = gameObject.GetMovementComponent();

				int distX = midX - x;
				int distY = midY - y;
				int seed = movementComponent == null ? x + y : distX + distY;

				if (LogicDataTables.GetGlobals().TighterAttackPosition())
				{
					int totalRadius = radius + (gameObject.GetWidthInTiles() << 8);

					int randomDistance1 = LogicMath.Abs(combatComponent.Rand(m_parent.GetGlobalID() + seed) % totalRadius);
					int randomDistance2 = combatComponent.Rand(m_parent.GetGlobalID() + seed + 1) % 128;

					posX = position.m_x + distY * randomDistance2 / totalRadius - (x + distX * randomDistance1 / radius);
					posY = position.m_y - (distX * randomDistance2 / radius + y + distY * randomDistance1 / radius);
				}
				else
				{
					posX += (m_attackPositionRandom & combatComponent.Rand(seed + 1)) << 8;
					posY += (m_attackPositionRandom & combatComponent.Rand(seed)) << 8;
				}
			}

			long distance = (long)posX * posX + posY * posY + (v71 << 8) * (v71 << 8);

			if (distance < m_closerDistance)
			{
				m_closerDistance = distance;
				m_targetStartX = x;
				m_targetStartY = y;
			}

			return false;
		}

		public int GetNearestBuildings(int x, int y, int radius, LogicTileMap tileMap)
		{
			if (m_nearestBuildings == null)
			{
				m_nearestBuildings = new LogicArrayList<LogicGameObject>(10);
			}

			int startX = (x - radius) >> 9;
			int endX = (x + radius) >> 9;

			while (startX <= endX)
			{
				int startY = (y - radius) >> 9;
				int endY = (y + radius) >> 9;

				while (startY <= endY)
				{
					LogicTile tile = tileMap.GetTile(startX, startY);

					if (tile != null)
					{
						for (int i = 0; i < tile.GetGameObjectCount(); i++)
						{
							LogicGameObject gameObject = tile.GetGameObject(i);

							if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING && gameObject.IsAlive() && !gameObject.IsWall())
							{
								int idx = m_nearestBuildings.IndexOf(gameObject);

								if (idx == -1 && gameObject.PassableSubtilesAtEdge() <= 1)
								{
									m_nearestBuildings.Add(gameObject);
								}
							}
						}
					}

					++startY;
				}

				++startX;
			}

			int count = m_nearestBuildings.Size();
			m_nearestBuildings.Clear();
			return count;
		}

		public int GetAttackDist()
			=> LogicMath.Max(32, m_parent.GetCombatComponent().GetAttackRange(0, false) - 32);

		public void MoveToPoint(LogicVector2 position)
		{
			EnableUnderground();

			if (m_underground || m_flying)
			{
				m_movementSystem.AddPoint(position.m_x, position.m_y);
			}
			else
			{
				m_movementSystem.MoveTo(position.m_x, position.m_y, m_parent.GetLevel().GetTileMap(), true);
			}
		}

		public void InitRandom()
		{
			if (m_random == null)
			{
				m_random = new LogicRandom();
				m_random.SetIteratedRandomSeed(m_parent.GetLevel().GetLogicTime().GetTick() + m_parent.GetGlobalID());
			}
		}

		public void SetPatrolFreeze()
		{
			if (m_patrolEnabled && m_movementSystem.GetFreezeTime() > 300)
			{
				InitRandom();
				m_movementSystem.SetFreezeTime(m_random.Rand(100) + 200);

				Debugger.Print("Freeze");
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.MOVEMENT;
	}
}