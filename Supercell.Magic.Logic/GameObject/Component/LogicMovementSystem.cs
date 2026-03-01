using System;

using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public class LogicMovementSystem
	{
		private LogicMovementComponent m_parent;
		private LogicPathFinder m_pathFinder;

		private readonly LogicVector2 m_pushBackStartPosition; // 40
		private readonly LogicVector2 m_pushBackEndPosition; // 48
		private readonly LogicVector2 m_position; // 72
		private readonly LogicVector2 m_moveDistance; // 80
		private readonly LogicVector2 m_pathDistance; // 88
		private readonly LogicVector2 m_pathStartPosition; // 96
		private readonly LogicVector2 m_pathEndPosition; // 104
		private LogicArrayList<LogicVector2> m_wayPoints; // 12
		private readonly LogicArrayList<LogicVector2> m_path; // 120
		private LogicGameObject m_wall; // 132
		private LogicGameObject m_patrolPost; // 168

		private bool m_ignorePush;

		private int m_speed;
		private int m_direction;
		private int m_pathLength;

		private int m_slowTime;
		private int m_slowSpeed;
		private int m_wallCount;
		private int m_freezeTime; // 164
		private int m_pushTime; // 56
		private int m_pushInitTime; // 60
		private int m_patrolAreaCounter; // 8

		private readonly int[] m_boostTime;
		private readonly int[] m_boostSpeed;
		private readonly int[] m_preventsPushId;
		private readonly int[] m_preventsPushTime;

		public LogicMovementSystem()
		{
			m_pushBackStartPosition = new LogicVector2();
			m_pushBackEndPosition = new LogicVector2();
			m_position = new LogicVector2();
			m_moveDistance = new LogicVector2();
			m_pathDistance = new LogicVector2();
			m_pathStartPosition = new LogicVector2();
			m_pathEndPosition = new LogicVector2();
			m_wayPoints = new LogicArrayList<LogicVector2>();
			m_path = new LogicArrayList<LogicVector2>();

			m_preventsPushId = new int[3];
			m_preventsPushTime = new int[3];
			m_boostTime = new int[2];
			m_boostSpeed = new int[2];

			m_pushInitTime = 500;
		}

		public void Init(int speed, LogicMovementComponent parent, LogicPathFinder pathFinder)
		{
			m_parent = parent;
			m_pathFinder = pathFinder;

			if (parent != null && pathFinder != null)
			{
				Debugger.Error("LogicMovementSystem: both m_pParent and m_pPathFinder cant be used");
			}

			SetSpeed(speed);
		}

		public void Destruct()
		{
			ClearPath();
			ClearPatrolArea();

			m_pushBackStartPosition.Destruct();
			m_pushBackEndPosition.Destruct();
			m_moveDistance.Destruct();
			m_position.Destruct();
			m_pathDistance.Destruct();
			m_pathStartPosition.Destruct();
			m_pathEndPosition.Destruct();

			m_pathLength = 0;
		}

		public int GetSpeed()
		{
			int speed = m_speed;

			if (m_boostTime[0] > 0)
			{
				speed += m_boostSpeed[0];
			}

			if (m_slowTime > 0)
			{
				speed = (int)((long)speed * (m_slowSpeed + 100) / 100);
			}

			if (m_parent != null && m_parent.GetParent().IsFrozen())
			{
				speed = 0;
			}

			return LogicMath.Max(0, speed);
		}

		public void SetSpeed(int speed)
		{
			m_speed = 16 * speed / 1000;
		}

		public int GetDirection()
			=> m_direction;

		public void SetDirection(int angle)
		{
			m_direction = angle;
		}

		public bool IgnorePush()
			=> m_ignorePush;

		public LogicVector2 GetPosition()
			=> m_position;

		public LogicVector2 GetPathEndPosition()
			=> m_pathEndPosition;

		public int GetPathLength()
			=> m_pathLength;

		public void ClearPath()
		{
			m_wall = null;
			m_wallCount = 0;

			for (int i = m_path.Size() - 1; i >= 0; i--)
			{
				m_path[i].Destruct();
				m_path.Remove(i);
			}

			m_pathLength = 0;
		}

		public int GetWallCount()
			=> m_wallCount;

		public bool NotMoving()
			=> m_path.Size() == 0;

		public void Reset(int x, int y)
		{
			m_position.Set(x, y);

			if (m_parent != null)
			{
				ValidatePos();
			}
		}

		public void ValidatePos()
		{
			/*
            if (this.m_parent != null && !this.m_parent.IsFlying() && !this.m_parent.IsUnderground() && this.m_parent.GetJump() <= 0 && !this.m_ignorePush)
            {
                this.m_parent.GetParent().GetLevel().GetTileMap().IsPassablePathFinder(this.m_position.m_x >> 8, this.m_position.m_y >> 8);
            }
            */
		}

		public void CalculatePathLength()
		{
			m_pathLength = 0;

			if (m_path.Size() > 0)
			{
				m_pathLength += m_position.GetDistance(m_path[m_path.Size() - 1]);

				for (int i = 0; i < m_path.Size() - 1; i++)
				{
					m_pathLength += m_path[i].GetDistance(m_path[i + 1]);
				}
			}
		}

		public void CalculateDirection(LogicVector2 pos)
		{
			LogicVector2 nextPath = m_path[m_path.Size() - 1];

			pos.m_x = nextPath.m_x - m_position.m_x;
			pos.m_y = nextPath.m_y - m_position.m_y;

			m_direction = pos.GetAngle();
		}

		public void ClearPatrolArea()
		{
			if (m_wayPoints != null)
			{
				while (m_wayPoints.Size() > 0)
				{
					LogicVector2 wayPoint = m_wayPoints[m_wayPoints.Size() - 1];

					if (wayPoint == null)
					{
						Debugger.Error("LogicMovementSystem::calculatePatrolArea: removed waypoint is NULL");
					}

					wayPoint.Destruct();

					m_wayPoints.Remove(m_wayPoints.Size() - 1);
				}
			}

			m_wayPoints = null;
		}

		public int GetDistSqToEnd()
		{
			if (m_path.Size() > 0)
			{
				return m_position.GetDistanceSquared(m_path[0]);
			}

			return 0;
		}

		public void AddPoint(int x, int y)
		{
			ClearPath();

			m_path.Add(new LogicVector2(x, y));

			CalculatePathLength();
			CalculateDirection(m_pathDistance);
		}

		public void PopTarget()
		{
			int idx = m_path.Size() - 1;

			m_path[idx].Destruct();
			m_path.Remove(idx);
		}

		public void SubTick()
		{
			UpdateMovement(m_parent.GetParent().GetLevel());
		}

		public LogicGameObject GetWall()
			=> m_wall;

		public void UpdateMovement(LogicLevel level)
		{
			if (m_boostTime[0] > 0)
			{
				m_boostTime[0] -= 1;

				if (m_boostTime[0] == 0)
				{
					m_boostTime[0] = m_boostTime[1];
					m_boostSpeed[0] = m_boostSpeed[1];

					m_boostTime[1] = 0;
					m_boostSpeed[1] = 0;
				}
			}

			if (m_slowTime > 0)
			{
				m_slowTime -= 1;

				if (m_slowTime == 0)
				{
					m_slowSpeed = 0;
				}
			}

			if (m_freezeTime <= 0)
			{
				if (m_pushTime <= 0)
				{
					if (m_path.Size() != 0)
					{
						int speed = GetSpeed();

						while (speed > 0)
						{
							if (m_path.Size() == 0)
							{
								break;
							}

							LogicVector2 path = m_path[m_path.Size() - 1];

							int distanceX = path.m_x - m_position.m_x;
							int distanceY = path.m_y - m_position.m_y;

							m_moveDistance.m_x = distanceX;
							m_moveDistance.m_y = distanceY;

							int length = m_moveDistance.Normalize(speed);

							if (length > speed)
							{
								if (distanceX != 0 && m_moveDistance.m_x == 0)
								{
									m_moveDistance.m_x = distanceX <= 0 ? -1 : 1;
								}

								if (distanceY != 0 && m_moveDistance.m_y == 0)
								{
									m_moveDistance.m_y = distanceY <= 0 ? -1 : 1;
								}

								SetPosition(m_position.m_x + m_moveDistance.m_x, m_position.m_y + m_moveDistance.m_y);
								m_pathLength += m_position.GetDistance(path) - length;
								ValidatePos();

								break;
							}

							SetPosition(path.m_x, path.m_y);
							m_pathLength += m_position.GetDistance(path) - length;
							ValidatePos();
							PopTarget();

							if (m_path.Size() == 0)
							{
								m_pathLength = 0;
								UpdatePatrolArea(level);

								break;
							}

							CalculateDirection(m_moveDistance);

							speed -= m_moveDistance.GetLength();
						}
					}
				}
				else
				{
					UpdatePushBack();
				}

				ValidatePos();

				if (m_parent != null && m_parent.GetParent().IsFrozen())
				{
					if (m_path.Size() != 0)
					{
						ValidatePos();
						ClearPath();
					}
				}

				for (int i = 0; i < 3; i++)
				{
					if (m_preventsPushTime[i] > 0)
					{
						m_preventsPushTime[i] -= 16;

						if (m_preventsPushTime[i] <= 0)
						{
							m_preventsPushTime[i] = 0;
							m_preventsPushId[i] = 0;
						}
					}
				}
			}
			else
			{
				m_freezeTime = LogicMath.Max(m_freezeTime - 16, 0);
			}
		}

		public void UpdatePatrolArea(LogicLevel level)
		{
			if (m_wayPoints != null)
			{
				int wayPointCount = m_wayPoints.Size();

				if (wayPointCount > 0 && m_pathLength == 0)
				{
					for (int i = wayPointCount - 1; i >= 0; i--)
					{
						m_patrolAreaCounter = (m_patrolAreaCounter + 1) % wayPointCount;

						if (MoveTo(m_wayPoints[m_patrolAreaCounter].m_x, m_wayPoints[m_patrolAreaCounter].m_y, level.GetTileMap(), true))
						{
							break;
						}
					}

					if (m_patrolPost != null && m_patrolPost.GetDefenceUnitProduction() != null)
					{
						m_freezeTime = (level.IsInCombatState() ? 100 : 5000) + level.GetLogicTime().GetTick() % (level.IsInCombatState() ? 200 : 3000);
					}
				}
			}
		}

		public void UpdatePushBack()
		{
			int startSpeed = m_pushTime * m_pushTime / m_pushInitTime;
			int endSpeed = m_pushInitTime - startSpeed;
			int pushBackX = (startSpeed * m_pushBackStartPosition.m_x + endSpeed * m_pushBackEndPosition.m_x) / m_pushInitTime;
			int pushBackY = (startSpeed * m_pushBackStartPosition.m_y + endSpeed * m_pushBackEndPosition.m_y) / m_pushInitTime;

			if (m_parent == null || m_parent.IsFlying() || m_parent.GetParent().GetLevel().GetTileMap().IsPassablePathFinder(pushBackX >> 8, pushBackY >> 8) ||
				m_ignorePush)
			{
				SetPosition(pushBackX, pushBackY);
			}
			else
			{
				m_pushBackStartPosition.m_x = m_position.m_x;
				m_pushBackStartPosition.m_y = m_position.m_y;
				m_pushBackEndPosition.m_x = m_position.m_x;
				m_pushBackEndPosition.m_y = m_position.m_y;
			}

			m_pushTime = LogicMath.Max(m_pushTime - 16, 0);

			if (m_pushTime == 0)
			{
				LogicGameObject parent = m_parent.GetParent();
				LogicCombatComponent combatComponent = parent.GetCombatComponent();

				if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)parent;

					if (character.GetCharacterData().GetPickNewTargetAfterPushback() || m_ignorePush)
					{
						if (combatComponent != null)
						{
							combatComponent.ForceNewTarget();
						}
					}
				}

				m_parent.NewTargetFound();

				if (combatComponent != null)
				{
					combatComponent.StopAttack();
				}

				m_ignorePush = false;
			}
		}

		public void SetPosition(int x, int y)
		{
			if (m_parent != null)
			{
				if (!m_parent.IsFlying() &&
					!m_parent.IsUnderground() &&
					m_parent.GetJump() <= 0 &&
					!m_ignorePush)
				{
					ValidatePos();

					if (m_position.m_x >> 8 == x >> 8)
					{
						if ((m_position.m_y ^ (uint)y) < 256)
						{
							goto set;
						}
					}

					LogicTileMap tileMap = m_parent.GetParent().GetLevel().GetTileMap();

					int pathFinderX = x >> 8;
					int pathFinderY = y >> 8;

					if (!tileMap.IsPassablePathFinder(pathFinderX, pathFinderY))
					{
						LogicTile tile = tileMap.GetTile(pathFinderX / 2, pathFinderY / 2);

						if (LogicDataTables.GetGlobals().JumpWhenHitJumpable())
						{
							bool allowJump = false;

							for (int i = 0; i < tile.GetGameObjectCount(); i++)
							{
								LogicGameObject gameObject = tile.GetGameObject(i);

								if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									LogicBuilding building = (LogicBuilding)gameObject;

									if (building.GetHitWallDelay() > 0)
									{
										allowJump = true;
									}
								}
							}

							if (allowJump)
							{
								m_position.m_x = x;
								m_position.m_y = y;

								m_parent.EnableJump(128);

								return;
							}
						}

						if (LogicDataTables.GetGlobals().SlideAlongObstacles())
						{
							throw new NotImplementedException(); // TODO: Implement this.
						}
						else
						{
							x = LogicMath.Clamp(x, (int)(m_position.m_x & 0xFFFFFF00), m_position.m_x | 0xFF);
							y = LogicMath.Clamp(y, (int)(m_position.m_y & 0xFFFFFF00), m_position.m_y | 0xFF);
						}

						m_position.m_x = x;
						m_position.m_y = y;

						ValidatePos();
						return;
					}
				}
			}

		set:

			m_position.m_x = x;
			m_position.m_y = y;
		}

		public bool IsPushed()
			=> m_pushTime > 0;

		public bool MoveTo(int x, int y, LogicTileMap tileMap, bool defaultEndPoint)
		{
			ClearPath();

			if (m_parent != null)
			{
				if (m_parent.GetParent().IsFrozen())
				{
					return false;
				}
			}

			m_wall = null;
			m_wallCount = 0;

			m_pathStartPosition.m_x = m_position.m_x >> 8;
			m_pathStartPosition.m_y = m_position.m_y >> 8;
			m_pathEndPosition.m_x = x >> 8;
			m_pathEndPosition.m_y = y >> 8;

			m_pathStartPosition.m_x = LogicMath.Clamp(m_pathStartPosition.m_x, 0, 99);
			m_pathStartPosition.m_y = LogicMath.Clamp(m_pathStartPosition.m_y, 0, 99);
			m_pathEndPosition.m_x = LogicMath.Clamp(m_pathEndPosition.m_x, 0, 99);
			m_pathEndPosition.m_y = LogicMath.Clamp(m_pathEndPosition.m_y, 0, 99);

			LogicPathFinder pathFinder;

			if (m_parent == null)
			{
				pathFinder = m_pathFinder;
				pathFinder.ResetCostStrategyToDefault();
			}
			else
			{
				bool resetStrategyCost = true;
				int strategyCost = 256;

				LogicGameObject parent = m_parent.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();

				if (hitpointComponent != null)
				{
					if (hitpointComponent.GetTeam() == 1)
					{
						resetStrategyCost = false;
						strategyCost = 768;
					}
				}

				if (m_parent.CanJumpWall())
				{
					resetStrategyCost = false;
					strategyCost = 16;
				}

				if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)parent;

					if (character.IsWallBreaker())
					{
						resetStrategyCost = false;
						strategyCost = 128;
					}
				}

				pathFinder = tileMap.GetPathFinder();

				if (resetStrategyCost)
				{
					pathFinder.ResetCostStrategyToDefault();
				}
				else
				{
					pathFinder.SetCostStrategy(true, strategyCost);
				}

				pathFinder.FindPath(m_pathStartPosition, m_pathEndPosition, true);
				pathFinder.GetPathLength();

				int pathLength = pathFinder.GetPathLength();

				m_path.EnsureCapacity(pathLength + 1);

				if (pathLength != 0 && defaultEndPoint)
				{
					LogicVector2 pathPoint = new LogicVector2(x, y);

					CheckWall(pathPoint);
					m_path.Add(pathPoint);
				}

				if (LogicDataTables.GetGlobals().UseNewPathFinder())
				{
					LogicTileMap pathFinderTileMap = pathFinder.GetTileMap();

					int width = 2 * pathFinderTileMap.GetSizeX();
					int height = 2 * pathFinderTileMap.GetSizeY();

					int startTileIdx = m_pathStartPosition.m_x + width * m_pathStartPosition.m_y;
					int endTileIdx = m_pathEndPosition.m_x + width * m_pathEndPosition.m_y;

					if (!defaultEndPoint)
					{
						LogicVector2 pathPoint = new LogicVector2((endTileIdx % width) << 8, (endTileIdx / height) << 8);

						CheckWall(pathPoint);
						m_path.Add(pathPoint);
					}

					if (pathLength > 0 && !pathFinder.IsLineOfSightClear())
					{
						int iterationCount = 0;

						while (endTileIdx != startTileIdx && endTileIdx != -1)
						{
							endTileIdx = pathFinder.GetParent(endTileIdx);

							if (endTileIdx != startTileIdx && endTileIdx != -1)
							{
								LogicVector2 pathPoint = new LogicVector2((endTileIdx % width) << 8, (endTileIdx / height) << 8);

								pathPoint.m_x += 128;
								pathPoint.m_y += 128;

								CheckWall(pathPoint);
								m_path.Add(pathPoint);

								if (iterationCount >= 100000)
								{
									Debugger.Warning("LMSystem: iteration count > 100000");
									break;
								}
							}

							iterationCount += 1;
						}
					}
				}
				else
				{
					for (int i = -pathLength, j = 0; j + i != 0; j++)
					{
						LogicVector2 pathPoint = new LogicVector2();

						pathFinder.GetPathPoint(pathPoint, i + j);

						if (i + j == -1 && m_pathStartPosition.Equals(pathPoint))
						{
							pathPoint.Destruct();
							pathPoint = null;
						}
						else
						{
							if (j != 0 || !m_pathStartPosition.Equals(pathPoint))
							{
								pathPoint.m_x = (pathPoint.m_x << 8) | 128;
								pathPoint.m_y = (pathPoint.m_y << 8) | 128;
							}
							else
							{
								pathPoint.m_x = x;
								pathPoint.m_y = y;
							}

							CheckWall(pathPoint);
							m_path.Add(pathPoint);
						}
					}
				}
			}

			CalculatePathLength();

			if (m_path.Size() > 0)
			{
				CalculateDirection(m_pathDistance);
				return true;
			}

			return false;
		}

		public void CheckWall(LogicVector2 position)
		{
			if (m_parent != null)
			{
				LogicGameObject gameObject = m_parent.GetParent();
				LogicTile tile = gameObject.GetLevel().GetTileMap().GetTile(position.m_x >> 9, position.m_y >> 9);

				if (tile != null)
				{
					for (int i = 0; i < tile.GetGameObjectCount(); i++)
					{
						LogicGameObject go = tile.GetGameObject(i);

						if (go.IsWall() &&
							go.IsAlive())
						{
							m_wall = go;

							if (((LogicBuilding)go).GetHitWallDelay() <= 0)
							{
								++m_wallCount;
							}
						}
					}
				}
			}
		}

		public void PushBack(LogicVector2 position, int speed, int unk1, int id, bool ignoreWeight, bool gravity)
		{
			if (speed > 0 && m_pushTime <= 0)
			{
				if (m_parent != null && m_parent.GetJump() <= 0 && !m_parent.GetParent().IsHero())
				{
					if (id != 0)
					{
						int idx = -1;

						for (int k = 0; k < 3; k++)
						{
							if (m_preventsPushId[k] == id)
							{
								return;
							}

							if (m_preventsPushTime[k] == 0)
							{
								idx = k;
							}
						}

						if (idx == -1)
						{
							return;
						}

						m_preventsPushId[idx] = id;
						m_preventsPushTime[idx] = 1500;
					}

					LogicGameObject parent = m_parent.GetParent();
					LogicGameObjectData data = parent.GetData();

					int housingSpace = 1;

					if (data.GetDataType() == DataType.CHARACTER)
					{
						housingSpace = ((LogicCombatItemData)data).GetHousingSpace();

						if (housingSpace >= 4 && !ignoreWeight)
						{
							return;
						}
					}

					int pushForce = 256;

					if (100 / unk1 != 0)
					{
						pushForce = 256 / (100 / unk1);
					}

					if (gravity)
					{
						pushForce = (LogicMath.Min(speed, 5000) << 8) / 5000 / housingSpace;
					}

					int rndX = parent.Rand(100) & 0x7F;
					int rndY = parent.Rand(200) & 0x7F;
					int pushBackX = rndX + position.m_x - 0x3F;
					int pushBackY = rndY + position.m_y - 0x3F;
					int pushBackTime = (1000 * pushForce) >> 8;

					m_pushTime = pushBackTime;
					m_pushInitTime = pushBackTime;
					m_ignorePush = false;
					m_pushBackStartPosition.m_x = m_position.m_x;
					m_pushBackStartPosition.m_y = m_position.m_y;

					pushForce *= 2;

					m_pushBackEndPosition.m_x = m_position.m_x + ((pushForce * pushBackX) >> 8);
					m_pushBackEndPosition.m_y = m_position.m_y + ((pushForce * pushBackY) >> 8);

					int angle = position.GetAngle();
					int direction = angle <= 180 ? 180 : -180;

					m_direction = direction + angle;
				}
			}
		}

		public bool ManualPushBack(LogicVector2 position, int speed, int time, int id)
		{
			if (speed > 0)
			{
				if (m_parent != null && m_parent.GetJump() <= 0)
				{
					if (id != 0)
					{
						int idx = -1;

						for (int k = 0; k < 3; k++)
						{
							if (m_preventsPushId[k] == id)
							{
								return false;
							}

							if (m_preventsPushTime[k] == 0)
							{
								idx = k;
							}
						}

						if (idx == -1)
						{
							return false;
						}

						m_preventsPushId[idx] = id;
						m_preventsPushTime[idx] = 1500;
					}

					LogicGameObject parent = m_parent.GetParent();

					int rndX = parent.Rand(100) & 0x7F;
					int rndY = parent.Rand(200) & 0x7F;

					int pushBackX = rndX + position.m_x - 0x3F;
					int pushBackY = rndY + position.m_y - 0x3F;

					LogicVector2 pushForce = new LogicVector2((2 * speed * pushBackX) >> 8, (2 * speed * pushBackY) >> 8);

					int prevPushBackTime = m_pushTime;

					if (prevPushBackTime <= 0)
					{
						m_pushTime = time - 16;
						m_pushInitTime = time;

						m_ignorePush = false;

						m_pushBackStartPosition.m_x = m_position.m_x;
						m_pushBackStartPosition.m_y = m_position.m_y;

						m_pushBackEndPosition.m_x = m_position.m_x + pushForce.m_x;
						m_pushBackEndPosition.m_y = m_position.m_y + pushForce.m_y;
					}
					else
					{
						LogicVector2 prevPushForce = new LogicVector2(m_pushBackEndPosition.m_x - m_position.m_x, m_pushBackEndPosition.m_y - m_position.m_y);

						m_pushTime = prevPushBackTime + time - 16;
						m_pushInitTime = prevPushBackTime + time;

						m_ignorePush = false;

						pushForce.Add(prevPushForce);

						m_pushBackStartPosition.m_x = m_position.m_x;
						m_pushBackStartPosition.m_y = m_position.m_y;
						m_pushBackEndPosition.m_x = m_position.m_x + pushForce.m_x;
						m_pushBackEndPosition.m_y = m_position.m_y + pushForce.m_y;

						prevPushForce.Destruct();
					}

					return true;
				}
			}

			return false;
		}

		public void PushTrap(LogicVector2 position, int time, int id, bool ignorePrevPush, bool verifyPushPosition)
		{
			if (m_pushTime <= 0 || ignorePrevPush)
			{
				if (m_parent != null && m_parent.GetJump() <= 0 && !m_parent.GetParent().IsHero())
				{
					LogicGameObject parent = m_parent.GetParent();

					if (!parent.IsHero())
					{
						if (id != 0 && !ignorePrevPush)
						{
							int idx = -1;

							for (int k = 0; k < 3; k++)
							{
								if (m_preventsPushId[k] == id)
								{
									return;
								}

								if (m_preventsPushTime[k] == 0)
								{
									idx = k;
								}
							}

							if (idx == -1)
							{
								return;
							}

							m_preventsPushId[idx] = id;
							m_preventsPushTime[idx] = 1500;
						}

						m_pushTime = time;
						m_pushInitTime = time;
						m_pushBackStartPosition.m_x = m_position.m_x;
						m_pushBackStartPosition.m_y = m_position.m_y;
						m_pushBackEndPosition.m_x = m_position.m_x + position.m_x;
						m_pushBackEndPosition.m_y = m_position.m_y + position.m_y;

						if (verifyPushPosition)
						{
							int pushBackEndPositionX = m_pushBackEndPosition.m_x;
							int pushBackEndPositionY = m_pushBackEndPosition.m_y;

							if (LogicMath.Max(LogicMath.Abs(position.m_x), LogicMath.Abs(position.m_y)) != 0)
							{
								LogicTileMap tileMap = parent.GetLevel().GetTileMap();

								if (!tileMap.IsPassablePathFinder(pushBackEndPositionX >> 8, pushBackEndPositionY >> 8))
								{
									LogicVector2 pos = new LogicVector2();
									LogicRandom rnd = new LogicRandom(pushBackEndPositionX + pushBackEndPositionY);

									tileMap.GetNearestPassablePosition(pushBackEndPositionX + rnd.Rand(512) - 256,
																	   pushBackEndPositionY + rnd.Rand(512) - 256, pos, 2048);

									pushBackEndPositionX = pos.m_x;
									pushBackEndPositionY = pos.m_y;
								}

								if (!tileMap.IsPassablePathFinder(pushBackEndPositionX >> 8, pushBackEndPositionY >> 8))
								{
									Debugger.Warning("PushTrap->ended on inmovable");
								}
							}

							m_pushBackEndPosition.m_x = pushBackEndPositionX;
							m_pushBackEndPosition.m_y = pushBackEndPositionY;
						}

						m_ignorePush = verifyPushPosition;

						int angle = position.GetAngle();
						m_direction = angle + (angle <= 180 ? 180 : -180);
					}
				}
			}
		}

		public bool ManualPushTrap(LogicVector2 position, int time, int id)
		{
			if (m_parent != null)
			{
				if (m_parent.GetJump() <= 0 && !m_parent.GetParent().IsHero())
				{
					if (id != 0)
					{
						int idx = -1;

						for (int k = 0; k < 3; k++)
						{
							if (m_preventsPushId[k] == id)
							{
								return false;
							}

							if (m_preventsPushTime[k] == 0)
							{
								idx = k;
							}
						}

						if (idx == -1)
						{
							return false;
						}

						m_preventsPushId[idx] = id;
						m_preventsPushTime[idx] = 1500;
					}

					LogicVector2 pushForce = new LogicVector2(time * position.m_x / 32, time * position.m_y / 32);

					m_pushTime = time - 16;
					m_pushInitTime = time;

					m_ignorePush = false;

					m_pushBackStartPosition.m_x = m_position.m_x;
					m_pushBackStartPosition.m_y = m_position.m_y;
					m_pushBackEndPosition.m_x = m_position.m_x + pushForce.m_x;
					m_pushBackEndPosition.m_y = m_position.m_y + pushForce.m_y;

					return true;
				}
			}

			return false;
		}

		public void Boost(int speed, int time)
		{
			if (speed < 0)
			{
				m_slowSpeed = LogicMath.Min(LogicMath.Max(-100, speed), m_slowSpeed);
				m_slowTime = time;
			}
			else
			{
				int idx = m_boostSpeed[0] != 0 ? 1 : 0;

				m_boostSpeed[idx] = LogicMath.Max(speed, m_boostSpeed[idx]);
				m_boostTime[idx] = time;
			}
		}

		public bool IsBoosted()
			=> m_boostTime[0] > 0;

		public bool IsSlowed()
			=> m_slowTime > 0;

		public LogicGameObject GetPatrolPost()
			=> m_patrolPost;

		public void SetPatrolPost(LogicGameObject gameObject)
		{
			m_patrolPost = gameObject;
		}

		public int GetFreezeTime()
			=> m_freezeTime;

		public void SetFreezeTime(int value)
		{
			m_freezeTime = value;
		}

		public void CreatePatrolArea(LogicGameObject patrolPost, LogicLevel level, bool unk, int idx)
		{
			LogicArrayList<LogicVector2> wayPoints = new LogicArrayList<LogicVector2>(8);

			if (m_patrolPost == null)
			{
				m_patrolPost = patrolPost;
			}

			int startX = 0;
			int startY = 0;
			int endX = 0;
			int endY = 0;
			int midX = 0;
			int midY = 0;

			int width = 0;
			int height = 0;

			int radius = 0;

			if (patrolPost != null)
			{
				startX = patrolPost.GetX() - 128;
				startY = patrolPost.GetY() - 128;
				endX = patrolPost.GetX() + (patrolPost.GetWidthInTiles() << 9) + 128;
				endY = patrolPost.GetY() + (patrolPost.GetHeightInTiles() << 9) + 128;
				midX = patrolPost.GetMidX();
				midY = patrolPost.GetMidY();
				width = patrolPost.GetWidthInTiles() << 8;
				height = patrolPost.GetHeightInTiles() << 8;
				radius = 1536;
			}

			if (radius * radius >= (uint)(width * width + height * height))
			{
				LogicVector2 tmp1 = new LogicVector2();
				LogicVector2 tmp2 = new LogicVector2();
				LogicVector2 tmp3 = new LogicVector2();
				LogicVector2 tmp4 = new LogicVector2();

				tmp2.Set(midX, midY);

				int rnd = patrolPost.GetLevel().GetLogicTime().GetTick() + idx;

				midX = midX + 127 * rnd % 1024 - 512;
				midY = midY + 271 * rnd % 1024 - 512;

				for (int i = 0, j = 45; i < 4; i++, j += 90)
				{
					tmp1.Set(midX + LogicMath.Cos(j, radius), midY + LogicMath.Sin(j, radius));
					LogicHeroBaseComponent.FindPoint(patrolPost.GetLevel().GetTileMap(), tmp3, tmp2, tmp1, tmp4);
					wayPoints.Add(new LogicVector2(tmp4.m_x, tmp4.m_y));
				}

				tmp1.Destruct();
				tmp2.Destruct();
				tmp3.Destruct();
				tmp4.Destruct();
			}
			else
			{
				wayPoints.Add(new LogicVector2(endX, endY));
				wayPoints.Add(new LogicVector2(startX, endY));
				wayPoints.Add(new LogicVector2(startX, startY));
				wayPoints.Add(new LogicVector2(endX, startY));
			}

			ClearPatrolArea();

			m_wayPoints = wayPoints;
			m_patrolAreaCounter = 0;

			if (m_wayPoints.Size() > 1)
			{
				int closestLength = 0x7FFFFFFF;

				for (int i = 1, size = m_wayPoints.Size(); i < size; i++)
				{
					LogicVector2 wayPoint = m_wayPoints[i];

					int length = (wayPoint.m_x - (m_position.m_x >> 16)) * (wayPoint.m_x - (m_position.m_x >> 16)) +
								 (wayPoint.m_y - (m_position.m_y >> 16)) * (wayPoint.m_y - (m_position.m_y >> 16));

					if (length < closestLength)
					{
						m_patrolAreaCounter = i;
						closestLength = length;
					}
				}
			}

			MoveTo(m_wayPoints[m_patrolAreaCounter].m_x, m_wayPoints[m_patrolAreaCounter].m_y, level.GetTileMap(), true);
		}
	}
}