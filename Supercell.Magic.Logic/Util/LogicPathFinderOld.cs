using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Util
{
	public sealed class LogicPathFinderOld : LogicPathFinder
	{
		public const int SAVED_PATHS = 30;

		private bool m_costStrategyMode;

		private readonly int m_mapWidth;
		private readonly int m_mapHeight;
		private int m_costStrategy;
		private int m_pathLength;
		private int m_pathStartX;
		private int m_pathStartY;
		private int m_heapLength;
		private int m_savingPathIndex;
		private int m_maxHeapLength;

		private int[] m_pathState;
		private int[] m_parentBuffer;
		private int[] m_pathBuffer;
		private int[] m_heapBuffer;
		private int[] m_pathCost;

		private readonly LogicSavedPath[] m_savedPaths;

		public LogicPathFinderOld(LogicTileMap tileMap) : base(tileMap)
		{
			if (tileMap != null)
			{
				m_mapWidth = 2 * tileMap.GetSizeX();
				m_mapHeight = 2 * tileMap.GetSizeY();
			}
			else
			{
				m_mapWidth = 3;
				m_mapHeight = 4;
			}

			m_savedPaths = new LogicSavedPath[LogicPathFinderOld.SAVED_PATHS];

			for (int i = 0, j = m_mapWidth * 4; i < LogicPathFinderOld.SAVED_PATHS; i++)
			{
				m_savedPaths[i] = new LogicSavedPath(j);
			}

			int size = m_mapWidth * m_mapHeight;

			m_pathState = new int[size];
			m_heapBuffer = new int[size];
			m_parentBuffer = new int[size];
			m_pathBuffer = new int[size];
			m_pathCost = new int[size];

			ResetCostStrategyToDefault();
		}

		public override void Destruct()
		{
			base.Destruct();

			m_pathState = null;
			m_heapBuffer = null;
			m_parentBuffer = null;
			m_pathBuffer = null;
			m_pathCost = null;

			for (int i = 0; i < LogicPathFinderNew.SAVED_PATHS; i++)
			{
				if (m_savedPaths[i] != null)
				{
					m_savedPaths[i].Destruct();
					m_savedPaths[i] = null;
				}
			}
		}

		public unsafe void AStar(int startTile, int endTile)
		{
			for (int i = 0; i < LogicPathFinderNew.SAVED_PATHS; i++)
			{
				LogicSavedPath savedPath = m_savedPaths[i];

				if (savedPath.IsEqual(startTile, endTile, m_costStrategy))
				{
					m_pathLength = savedPath.GetLength();
					savedPath.ExtractPath(m_pathBuffer);
					return;
				}
			}

			m_pathStartX = endTile % m_mapWidth;
			m_pathStartY = endTile / m_mapWidth;
			m_heapLength = 0;

			int mapSize = 4 * m_mapHeight * m_mapWidth;

			fixed (int* pathState = m_pathState)
			fixed (int* pathCost = m_pathCost)
			fixed (int* heapBuffer = m_heapBuffer)
			{
				for (int i = 0; i < mapSize; i++)
				{
					pathState[i] = 0;
					pathCost[i] = 0;
					heapBuffer[i] = 0x7FFFFFFF;
				}
			}

			m_pathLength = 0;
			m_pathBuffer[m_pathLength++] = startTile;
			m_parentBuffer[startTile] = -1;
			m_parentBuffer[endTile] = -1;

			AStarAddTile(startTile);

			m_pathBuffer[0] = m_pathBuffer[m_pathLength-- - 1];
			m_pathState[startTile] = 2;

			if (m_heapLength != 0 && m_pathLength != 0)
			{
				do
				{
					int removeSmallest = RemoveSmallest();
					m_pathState[removeSmallest] = 2;
					AStarAddTile(removeSmallest);
				} while (m_pathState[endTile] != 2 && m_heapLength > 0);

				m_pathLength = 0;

				if (endTile != -1)
				{
					int parent = m_parentBuffer[endTile];

					if (parent != -1)
					{
						m_pathBuffer[m_pathLength++] = endTile;

						for (int i = m_parentBuffer[parent]; i != -1; parent = i, i = m_parentBuffer[i])
						{
							m_pathBuffer[m_pathLength++] = parent;
						}
					}
				}

				m_savingPathIndex += 1;

				if (m_savingPathIndex >= LogicPathFinderNew.SAVED_PATHS)
				{
					m_savingPathIndex = 0;
				}

				m_savedPaths[m_savingPathIndex].StorePath(m_pathBuffer, m_pathLength, startTile, endTile, m_costStrategy);
			}
		}

		public void AStarAddTile(int tileIndex)
		{
			int tileX = tileIndex % m_mapWidth;
			int tileY = tileIndex / m_mapWidth;

			AStarAddTile(tileIndex, tileX, tileY - 1, tileIndex - m_mapWidth, 100);
			AStarAddTile(tileIndex, tileX, tileY + 1, tileIndex + m_mapWidth, 100);
			AStarAddTile(tileIndex, tileX - 1, tileY, tileIndex - 1, 100);
			AStarAddTile(tileIndex, tileX + 1, tileY, tileIndex + 1, 100);
			AStarAddTile(tileIndex, tileX - 1, tileY - 1, tileIndex - m_mapWidth - 1, 141);
			AStarAddTile(tileIndex, tileX - 1, tileY + 1, tileIndex + m_mapWidth - 1, 141);
			AStarAddTile(tileIndex, tileX + 1, tileY + 1, tileIndex + m_mapWidth + 1, 141);
			AStarAddTile(tileIndex, tileX + 1, tileY - 1, tileIndex - m_mapWidth + 1, 141);
		}

		public bool AStarAddTile(int tileIndex, int tileX, int tileY, int pathIdx, int cost)
		{
			if (tileX >= 0 && tileY >= 0 && m_mapWidth > tileX && m_mapHeight > tileY)
			{
				int state = m_pathState[pathIdx];

				if (state != 2)
				{
					if (state != 0)
					{
						return true;
					}

					m_pathState[pathIdx] = 1;
					m_pathBuffer[m_pathLength++] = pathIdx;

					int pathFinderCost = m_tileMap.GetPathFinderCost(tileX, tileY);

					if (pathFinderCost < (m_costStrategyMode ? 0x7FFFFFFF : 1))
					{
						int tileCost = m_pathCost[tileIndex] + cost + ((m_costStrategy * pathFinderCost) >> 8);

						int tileDistanceX = tileX - m_pathStartX;
						int tileDistanceY = tileY - m_pathStartY;

						if (tileDistanceX < 1)
						{
							tileDistanceX = m_pathStartX - tileX;
						}

						if (tileDistanceY < 1)
						{
							tileDistanceY = m_pathStartY - tileY;
						}

						m_pathCost[tileIndex] = tileCost + 10 * (tileDistanceX + tileDistanceY);
						Add(pathIdx);
						m_maxHeapLength = m_maxHeapLength >= m_heapLength ? m_maxHeapLength : m_heapLength;

						return true;
					}
				}
			}

			return false;
		}

		public bool IsCollision(int x, int y)
		{
			if (m_tileMap == null)
			{
				return false;
			}

			LogicTile tile = m_tileMap.GetTile(x >> 1, y >> 1);

			if (tile != null)
			{
				return !tile.IsPassablePathFinder((x & 1) + 2 * (y & 1));
			}

			return true;
		}

		public void Add(int tileIdx)
		{
			m_heapBuffer[m_heapLength++] = tileIdx;

			if (m_heapLength > 1)
			{
				int idx = m_heapLength - 1;

				do
				{
					int prevHeapIdx = (idx - 1) >> 1;

					int heap = m_heapBuffer[idx];
					int prevHeap = m_heapBuffer[prevHeapIdx];

					if (m_pathCost[heap] >= m_pathCost[prevHeap])
					{
						break;
					}

					m_heapBuffer[idx] = prevHeap;
					m_heapBuffer[prevHeapIdx] = heap;

					idx = prevHeapIdx;
				} while (idx > 0);
			}
		}

		public int RemoveSmallest()
		{
			if (m_heapLength == 0)
			{
				return -1;
			}

			int result = m_heapBuffer[0];
			int prevHeap = m_heapBuffer[--m_heapLength];

			m_heapBuffer[0] = prevHeap;

			int idx = 0;
			int nextIdx = 0;

			while (true)
			{
				int idx1 = 2 * idx + 1;
				int idx2 = 2 * idx + 2;

				if (idx2 < m_heapLength)
				{
					if (m_pathCost[prevHeap] <= m_pathCost[m_heapBuffer[idx2]])
					{
						idx2 = idx;
					}

					nextIdx = idx2;
				}

				if (idx1 < m_heapLength)
				{
					if (m_pathCost[m_heapBuffer[nextIdx]] > m_pathCost[m_heapBuffer[idx1]])
					{
						nextIdx = idx1;
					}
				}

				if (nextIdx == idx)
				{
					break;
				}

				m_heapBuffer[idx] = m_heapBuffer[nextIdx];
				m_heapBuffer[nextIdx] = prevHeap;

				idx = nextIdx;
			}

			return result;
		}

		public override int GetPathLength()
			=> m_pathLength;

		public override void GetPathPoint(LogicVector2 position, int idx)
		{
			if (idx < 0 || m_pathLength <= idx)
			{
				Debugger.Error("illegal path index");
			}

			position.m_x = m_pathBuffer[idx] % m_mapWidth;
			position.m_y = m_pathBuffer[idx] / m_mapWidth;
		}

		public override void GetPathPointSubTile(LogicVector2 position, int idx)
		{
			Debugger.Warning("getPathPointSubTile() called. Should not be called ever for LogicPathFinderOld!");
		}

		public override void FindPath(LogicVector2 startPosition, LogicVector2 endPosition, bool clampPathFinderCost)
		{
			m_maxHeapLength = 0;

			if (m_tileMap.IsPassablePathFinder(startPosition.m_x, startPosition.m_y))
			{
				if (!IsReachable(endPosition.m_x, endPosition.m_y) && clampPathFinderCost)
				{
					int distance = LogicMath.Sqrt((endPosition.m_x - startPosition.m_x) * (endPosition.m_x - startPosition.m_x) +
												  (endPosition.m_y - startPosition.m_y) * (endPosition.m_y - startPosition.m_y));
					int lowDistanceX = LogicMath.Clamp(endPosition.m_x - distance, 0, 2 * m_mapWidth);
					int lowDistanceY = LogicMath.Clamp(endPosition.m_y - distance, 0, 2 * m_mapHeight);
					int highDistanceX = LogicMath.Clamp(endPosition.m_x + distance, 0, 2 * m_mapWidth);
					int highDistanceY = LogicMath.Clamp(endPosition.m_y + distance, 0, 2 * m_mapHeight);

					int minX = -1;
					int minY = -1;
					int minDistance = 0x7FFFFFFF;

					while (lowDistanceX < highDistanceX)
					{
						int posX = lowDistanceX;

						for (int posY = lowDistanceY; posY < highDistanceY; posY++)
						{
							if (IsReachable(posX, posY))
							{
								int pointDistance = (posX - endPosition.m_x) * (posX - endPosition.m_x) +
													(posY - endPosition.m_y) * (posY - endPosition.m_y);

								if (pointDistance < minDistance)
								{
									minX = posX;
									minY = posY;
									minDistance = pointDistance;
								}
							}
						}

						++lowDistanceX;
					}

					if (minX == -1)
					{
						m_pathLength = 0;
						return;
					}

					endPosition.m_x = minX;
					endPosition.m_y = minY;
				}

				if (IsReachable(endPosition.m_x, endPosition.m_y))
				{
					int startTileIndex = startPosition.m_x + startPosition.m_y * m_mapWidth;
					int endTileIndex = endPosition.m_x + endPosition.m_y * m_mapWidth;

					if (IsLineOfSightClear(startPosition.m_x, startPosition.m_y, endPosition.m_x, endPosition.m_y))
					{
						m_pathLength = 0;
						m_pathBuffer[m_pathLength++] = endTileIndex;
						m_pathBuffer[m_pathLength++] = startTileIndex;
					}
					else
					{
						AStar(startTileIndex, endTileIndex);

						if (m_pathLength > 0)
						{
							m_pathBuffer[m_pathLength++] = startTileIndex;
						}
					}
				}
				else
				{
					m_pathLength = 0;
				}
			}
			else
			{
				m_pathLength = 0;
			}
		}

		public bool IsLineOfSightClear(int xA, int yA, int xB, int yB)
		{
			if (IsLineOfSightClearImpl(xA, yA, xB, yB))
			{
				int directionX = Sign(xB - xA);
				int directionY = Sign(yB - yA);

				return IsLineOfSightClearImpl(xA + directionX, yA, xB, yB - directionY) &&
					   IsLineOfSightClearImpl(xA, yA + directionY, xB - directionX, yB);
			}

			return false;
		}

		public bool IsLineOfSightClearImpl(int xA, int yA, int xB, int yB)
		{
			int directionX = xB > xA ? 1 : -1;
			int directionY = yB > yA ? 1 : -1;

			int distanceX = LogicMath.Abs(xB - xA);
			int distanceY = LogicMath.Abs(yB - yA);
			int direction = distanceX - distanceY;

			int subTileDistanceX = distanceX * 2;
			int subTileDistanceY = distanceY * 2;

			for (int i = distanceX + distanceY, posX = xA, posY = yA; i >= 0; i--)
			{
				if (IsCollision(posX, posY))
				{
					return false;
				}

				if (direction > 0)
				{
					direction -= subTileDistanceY;
					posX += directionX;
				}
				else
				{
					direction += subTileDistanceX;
					posY += directionY;
				}
			}

			return true;
		}

		public override void SetCostStrategy(bool enabled, int quality)
		{
			m_costStrategyMode = enabled;
			m_costStrategy = quality;
		}

		public override void ResetCostStrategyToDefault()
		{
			m_costStrategyMode = true;
			m_costStrategy = 256;
		}

		public override void InvalidateCache()
		{
			for (int i = 0; i < LogicPathFinderNew.SAVED_PATHS; i++)
			{
				m_savedPaths[i].StorePath(null, 0, -1, -1, 0);
			}
		}

		public override int GetParent(int index)
		{
			Debugger.Warning("getParent(idx) should not be called for LogicPathFinderOld");
			return 0;
		}

		public int Sign(int value)
			=> value > 0 ? 1 : value >> 31;

		public bool IsReachable(int x, int y)
			=> m_tileMap.GetPathFinderCost(x, y) < (m_costStrategyMode ? 0x7FFFFFFF : 1);
	}
}