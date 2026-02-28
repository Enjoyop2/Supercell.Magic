using System.Runtime.CompilerServices;

using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Level
{
	public sealed class LogicTile
	{
		private byte m_passableFlag;
		private readonly byte m_tileX;
		private readonly byte m_tileY;

		private short m_roomIndex;
		private int m_pathFinderCost;

		private LogicArrayList<LogicGameObject> m_gameObjects;

		public LogicTile(byte tileX, byte tileY)
		{
			m_gameObjects = new LogicArrayList<LogicGameObject>(4);
			m_tileX = tileX;
			m_tileY = tileY;
			m_passableFlag = 16;
			m_roomIndex = -1;
		}

		public void Destruct()
		{
			m_gameObjects = null;
			m_passableFlag = 16;
		}

		public void AddGameObject(LogicGameObject gameObject)
		{
			m_gameObjects.Add(gameObject);

			if (!gameObject.IsPassable())
			{
				m_passableFlag &= 0xEF;
			}

			RefreshSubTiles();
		}

		public bool IsPassablePathFinder(int x, int y)
			=> ((uint)x | (uint)y) <= 1 && ((1 << (x + 2 * y)) & m_passableFlag) == 0;

		public bool IsPassablePathFinder(int pos)
			=> ((1 << pos) & m_passableFlag) == 0;

		public bool IsBuildable(LogicGameObject gameObject)
		{
			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				LogicGameObject go = m_gameObjects[i];

				if (go != gameObject)
				{
					if (!go.IsPassable() || go.IsUnbuildable())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool IsBuildableWithIgnoreList(LogicGameObject[] gameObjects, int count)
		{
			for (int i = 0, index = -1; i < m_gameObjects.Size(); i++, index = -1)
			{
				LogicGameObject go = m_gameObjects[i];

				for (int j = 0; j < count; j++)
				{
					if (gameObjects[j] == go)
					{
						index = j;
						break;
					}
				}

				if (index == -1)
				{
					if (!m_gameObjects[i].IsPassable() || m_gameObjects[i].IsUnbuildable())
					{
						return false;
					}
				}
			}

			return true;
		}

		public void RemoveGameObject(LogicGameObject gameObject)
		{
			int index = -1;

			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				if (m_gameObjects[i] == gameObject)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_gameObjects.Remove(index);
				RefreshPassableFlag();
			}
		}

		public LogicObstacle GetTallGrass()
		{
			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				LogicGameObject gameObject = m_gameObjects[i];

				if (gameObject.GetGameObjectType() == LogicGameObjectType.OBSTACLE)
				{
					LogicObstacle obstacle = (LogicObstacle)m_gameObjects[i];

					if (obstacle.GetObstacleData().IsTallGrass())
					{
						return obstacle;
					}
				}
			}

			return null;
		}

		public LogicGameObject GetGameObject(int idx)
			=> m_gameObjects[idx];

		public int GetGameObjectCount()
			=> m_gameObjects.Size();

		public bool IsFullyNotPassable()
			=> (m_passableFlag & 0xF) == 0xF;

		public short GetRoomIdx()
			=> m_roomIndex;

		public void SetRoomIdx(short index)
		{
			m_roomIndex = index;
		}

		public int GetPathFinderCost(int x, int y)
		{
			if (m_pathFinderCost <= 0)
			{
				if (((uint)x | (uint)y) <= 1)
				{
					if ((m_passableFlag & (1 << (x + 2 * y))) == 0)
						return 0;
				}

				return 0x7FFFFFFF;
			}

			return m_pathFinderCost;
		}

		public int GetPathFinderCostIgnorePos(int x, int y)
		{
			if (m_pathFinderCost <= 0)
			{
				if ((m_passableFlag & (1 << (x + 2 * y))) == 0)
					return 0;
				return 0x7FFFFFFF;
			}

			return m_pathFinderCost;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetPathFinderCost()
		{
			if (m_pathFinderCost <= 0)
			{
				if (!IsFullyNotPassable())
					return 0;
				return 0x7FFFFFFF;
			}

			return m_pathFinderCost;
		}

		public void RefreshSubTiles()
		{
			m_passableFlag &= 0xF0;
			m_pathFinderCost = 0;

			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				LogicGameObject gameObject = m_gameObjects[i];

				m_pathFinderCost = LogicMath.Max(m_pathFinderCost, gameObject.PathFinderCost());

				if (!gameObject.IsPassable())
				{
					int width = gameObject.GetWidthInTiles();
					int height = gameObject.GetWidthInTiles();

					if (width == 1 || height == 1)
					{
						m_passableFlag |= 0xF;
					}
					else
					{
						int edge = gameObject.PassableSubtilesAtEdge();

						int startX = 2 * (m_tileX - gameObject.GetTileX());
						int startY = 2 * (m_tileY - gameObject.GetTileY());
						int endX = 2 * width - edge;
						int endY = 2 * height - edge;

						for (int j = 0; j < 2; j++)
						{
							int offset = j;
							int x = startX + j;

							for (int k = 0; k < 2; k++)
							{
								int y = startY + k;

								if (y < endY && x < endX && x >= edge && y >= edge)
								{
									m_passableFlag |= (byte)(1 << offset);
								}

								offset += 2;
							}
						}
					}
				}
			}
		}

		public void RefreshPassableFlag()
		{
			byte passableFlag = (byte)(m_passableFlag | 0x10);

			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				if (!m_gameObjects[i].IsPassable())
				{
					passableFlag = (byte)(m_passableFlag & 0xEF);
					break;
				}
			}

			m_passableFlag = passableFlag;
			RefreshSubTiles();
		}

		public byte GetPassableFlag()
			=> (byte)((m_passableFlag >> 4) & 1);

		public int GetX()
			=> m_tileX;

		public int GetY()
			=> m_tileY;

		public bool IsPassableFlag()
			=> (m_passableFlag & 16) >> 4 != 0;

		public bool HasWall()
		{
			for (int i = 0; i < m_gameObjects.Size(); i++)
			{
				LogicGameObject gameObject = m_gameObjects[i];

				if (gameObject.IsWall() && gameObject.IsAlive())
				{
					return true;
				}
			}

			return false;
		}
	}
}