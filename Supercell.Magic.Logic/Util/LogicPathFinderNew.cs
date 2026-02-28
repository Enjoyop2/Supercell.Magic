using System;

using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Util
{
	public sealed class LogicPathFinderNew : LogicPathFinder
	{
		public const int SAVED_PATHS = 30;

		private bool m_costStrategyMode;
		private bool m_lineOfSightClear;

		private readonly int m_mapWidth;
		private readonly int m_mapHeight;
		private int m_costStrategy;
		private int m_pathLength;
		private int m_pathStartX;
		private int m_pathStartY;
		private int m_heapLength;
		private int m_savingPathIndex;

		private readonly int[] m_pathState;
		private readonly int[] m_parentBuffer;
		private readonly int[] m_pathBuffer;
		private readonly int[] m_heapBuffer;
		private readonly int[] m_pathCost;

		private LogicVector2 m_pathPointOutput;
		private readonly LogicSavedPath[] m_savedPaths;

		public LogicPathFinderNew(LogicTileMap tileMap) : base(tileMap)
		{
			m_pathStartX = -1;
			m_pathStartY = -1;

			if (tileMap != null)
			{
				m_mapWidth = tileMap.GetSizeX();
				m_mapHeight = tileMap.GetSizeY();
			}
			else
			{
				m_mapWidth = 3;
				m_mapHeight = 4;
			}

			m_savedPaths = new LogicSavedPath[LogicPathFinderNew.SAVED_PATHS];

			for (int i = 0, j = m_mapWidth * 4; i < LogicPathFinderNew.SAVED_PATHS; i++)
			{
				m_savedPaths[i] = new LogicSavedPath(j);
			}

			int size = 4 * m_mapWidth * m_mapHeight;

			m_pathState = new int[size];
			m_heapBuffer = new int[size];
			m_parentBuffer = new int[size];
			m_pathBuffer = new int[size];
			m_pathCost = new int[size];

			m_pathPointOutput = new LogicVector2();

			ResetCostStrategyToDefault();
		}

		public override void Destruct()
		{
			base.Destruct();

			for (int i = 0; i < LogicPathFinderNew.SAVED_PATHS; i++)
			{
				if (m_savedPaths[i] != null)
				{
					m_savedPaths[i].Destruct();
					m_savedPaths[i] = null;
				}
			}

			if (m_pathPointOutput != null)
			{
				m_pathPointOutput.Destruct();
				m_pathPointOutput = null;
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
					m_savingPathIndex = 0;

				m_savedPaths[m_savingPathIndex].StorePath(m_pathBuffer, m_pathLength, startTile, endTile, m_costStrategy);
			}
		}

		public void AStarAddTile(int tileIndex)
		{
			int tileX = tileIndex % m_mapWidth;
			int tileY = tileIndex / m_mapWidth;

			LogicTile tile = m_tileMap.GetTile(tileX, tileY);

			if (tile != null)
			{
				int pathFinderCost1 = tile.GetPathFinderCost(0, 0);
				int pathFinderCost2 = tile.GetPathFinderCost(1, 0);
				int pathFinderCost3 = tile.GetPathFinderCost(0, 1);
				int pathFinderCost4 = tile.GetPathFinderCost(1, 1);

				if (pathFinderCost1 != 0x7FFFFFFF || pathFinderCost2 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX, tileY - 1, tileIndex - m_mapWidth, 100);
				}

				if (pathFinderCost3 != 0x7FFFFFFF || pathFinderCost4 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX, tileY + 1, tileIndex + m_mapWidth, 100);
				}

				if (pathFinderCost1 != 0x7FFFFFFF || pathFinderCost3 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX - 1, tileY, tileIndex - 1, 100);
				}

				if (pathFinderCost2 != 0x7FFFFFFF || pathFinderCost4 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX + 1, tileY, tileIndex + 1, 100);
				}

				if (pathFinderCost1 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX - 1, tileY - 1, tileIndex - m_mapWidth - 1, 141);
				}

				if (pathFinderCost3 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX - 1, tileY + 1, tileIndex + m_mapWidth - 1, 141);
				}

				if (pathFinderCost2 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX + 1, tileY - 1, tileIndex - m_mapWidth + 1, 141);
				}

				if (pathFinderCost4 != 0x7FFFFFFF)
				{
					AStarAddTile(tileIndex, tileX + 1, tileY + 1, tileIndex + m_mapWidth + 1, 141);
				}
			}
		}

		public bool AStarAddTile(int tileIndex, int tileX, int tileY, int pathIdx, int cost)
		{
			if (tileX >= 0 && tileY >= 0 && m_mapWidth > tileX && m_mapHeight > tileY)
			{
				int state = m_pathState[pathIdx];

				if (state != 2)
				{
					int pathFinderLimit = m_costStrategyMode ? 0x7FFFFFFF : 1;
					int pathFinderCost = m_tileMap.GetTilePathFinderCost(tileX, tileY);

					if (pathFinderCost < pathFinderLimit)
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

						int pathCost = tileCost + 10 * (tileDistanceX + tileDistanceY);

						if (state == 1)
						{
							if (m_pathCost[pathIdx] > pathCost)
							{
								m_pathCost[pathIdx] = pathCost;
								m_parentBuffer[pathIdx] = tileIndex;
							}
						}
						else if (state == 0)
						{
							m_pathState[pathIdx] = 1;
							m_pathBuffer[m_pathLength++] = pathIdx;
							m_parentBuffer[pathIdx] = tileIndex;
							m_pathCost[pathIdx] = pathCost;

							Add(pathIdx);
						}

						return true;
					}
				}
			}

			return false;
		}

		public bool IsCollision(int x, int y)
		{
			if (m_tileMap == null)
				return false;

			LogicTile tile = m_tileMap.GetTile(x >> 1, y >> 1);

			if (tile != null)
				return !tile.IsPassablePathFinder((x & 1) + 2 * (y & 1));

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
						break;

					m_heapBuffer[idx] = prevHeap;
					m_heapBuffer[prevHeapIdx] = heap;

					idx = prevHeapIdx;
				} while (idx > 0);
			}
		}

		public int RemoveSmallest()
		{
			if (m_heapLength == 0)
				return -1;

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
						idx2 = idx;
					nextIdx = idx2;
				}

				if (idx1 < m_heapLength)
				{
					if (m_pathCost[m_heapBuffer[nextIdx]] > m_pathCost[m_heapBuffer[idx1]])
						nextIdx = idx1;
				}

				if (nextIdx == idx)
					break;

				m_heapBuffer[idx] = m_heapBuffer[nextIdx];
				m_heapBuffer[nextIdx] = prevHeap;

				idx = nextIdx;
			}

			return result;
		}

		public override int GetParent(int index)
			=> m_parentBuffer[index];

		public override int GetPathLength()
			=> m_pathLength;

		public override void GetPathPoint(LogicVector2 position, int idx)
		{
			if ((uint)idx >= m_pathLength)
			{
				Debugger.Error("illegal path index");
			}

			position.m_x = m_pathBuffer[idx] % m_mapWidth;
			position.m_y = m_pathBuffer[idx] / m_mapWidth;
		}

		public override void GetPathPointSubTile(LogicVector2 position, int idx)
		{
			if ((uint)idx >= m_pathLength)
			{
				Debugger.Error("illegal path index");
			}

			LogicTile tile = m_tileMap.GetTile(position.m_x, position.m_y);

			position.m_x *= 2;
			position.m_y *= 2;

			if (tile.IsPassablePathFinder(3))
			{
				if (tile.IsPassablePathFinder(2))
				{
					if (tile.IsPassablePathFinder(1))
					{
						++position.m_x;
					}
				}

				++position.m_y;
			}
		}

		public void CreatePathPoint(LogicVector2 startPosition)
		{
			int pathLength = m_pathLength;
			int tileMapSubWidth = m_mapWidth * 2;
			int mapSize = 4 * m_mapWidth * m_mapHeight;

			for (int i = 0; i < mapSize; i++)
			{
				m_parentBuffer[i] = -1;
				m_pathCost[i] = -1;
			}

			int pathPoints = 0;

			for (int i = 0; i < pathLength; i++)
			{
				GetPathPoint(m_pathPointOutput, i);

				LogicTile tile = m_tileMap.GetTile(m_pathPointOutput.m_x, m_pathPointOutput.m_y);

				int pathPointIdx = 4 * m_mapWidth * m_pathPointOutput.m_y + 2 * m_pathPointOutput.m_x;

				if (tile.GetPathFinderCostIgnorePos(0, 0) != 0x7FFFFFFF)
				{
					pathPoints += 1;
					m_pathCost[pathPointIdx] = 0x7FFFFFFF;
				}

				if (tile.GetPathFinderCostIgnorePos(1, 0) != 0x7FFFFFFF)
				{
					pathPoints += 1;
					m_pathCost[pathPointIdx | 1] = 0x7FFFFFFF;
				}

				if (tile.GetPathFinderCostIgnorePos(0, 1) != 0x7FFFFFFF)
				{
					pathPoints += 1;
					m_pathCost[pathPointIdx + tileMapSubWidth] = 0x7FFFFFFF;
				}

				if (tile.GetPathFinderCostIgnorePos(1, 1) != 0x7FFFFFFF)
				{
					pathPoints += 1;
					m_pathCost[(pathPointIdx + tileMapSubWidth) | 1] = 0x7FFFFFFF;
				}
			}

			int heapValue = startPosition.m_x + tileMapSubWidth * startPosition.m_y;

			m_heapBuffer[0] = heapValue;
			m_pathCost[heapValue] = 1;

			for (int i = 0, stackSize = 1; i < pathPoints; i++)
			{
				int stackIdx = stackSize - 1;
				int heap = m_heapBuffer[stackSize - 1];

				int minIdx = 0;
				int minCost = 0x7FFFFFFF;

				for (int j = 0; j < stackSize; j++)
				{
					int heap2 = m_heapBuffer[j];

					if (minCost > m_pathCost[heap2])
					{
						heap = heap2;
						minIdx = j;
						minCost = m_pathCost[heap2];
					}
				}

				int cost = m_pathCost[heap];

				if (minIdx < stackIdx)
				{
					Array.Copy(m_heapBuffer, minIdx + 1, m_heapBuffer, minIdx, stackIdx - minIdx);
				}

				m_heapBuffer[stackSize] = -1;

				if (stackSize > 4096)
				{
					Debugger.Warning("LPFNew stack grew too large!");
					break;
				}

				int idx1 = heap - tileMapSubWidth; // - 1y
				int idx2 = heap + tileMapSubWidth; // + 1y
				int idx3 = heap - 1; // - 1x
				int idx4 = heap + 1; // + 1x

				if (idx1 >= 0 && idx1 < mapSize && m_pathCost[idx1] > cost + 100)
				{
					m_pathCost[idx1] = cost + 100;
					m_parentBuffer[idx1] = heap;
					m_heapBuffer[stackIdx++] = idx1;
				}

				if (idx2 >= 0 && idx2 < mapSize && m_pathCost[idx2] > cost + 100)
				{
					m_pathCost[idx2] = cost + 100;
					m_parentBuffer[idx2] = heap;
					m_heapBuffer[stackIdx++] = idx2;
				}

				if (idx3 >= 0 && idx3 < mapSize && m_pathCost[idx3] > cost + 100)
				{
					m_pathCost[idx3] = cost + 100;
					m_parentBuffer[idx3] = heap;
					m_heapBuffer[stackIdx++] = idx3;
				}

				if (idx4 >= 0 && idx4 < mapSize && m_pathCost[idx4] > cost + 100)
				{
					m_pathCost[idx4] = cost + 100;
					m_parentBuffer[idx4] = heap;
					m_heapBuffer[stackIdx++] = idx4;
				}

				idx1 = heap - tileMapSubWidth - 1; // - 1y - 1x
				idx2 = heap - tileMapSubWidth + 1; // - 1y + 1x
				idx3 = heap + tileMapSubWidth - 1; // + 1y - 1x
				idx4 = heap + tileMapSubWidth + 1; // + 1y + 1x

				if (idx1 >= 0 && idx1 < mapSize && m_pathCost[idx1] > cost + 141)
				{
					m_pathCost[idx1] = cost + 141;
					m_parentBuffer[idx1] = heap;
					m_heapBuffer[stackIdx++] = idx1;
				}

				if (idx2 >= 0 && idx2 < mapSize && m_pathCost[idx2] > cost + 141)
				{
					m_pathCost[idx2] = cost + 141;
					m_parentBuffer[idx2] = heap;
					m_heapBuffer[stackIdx++] = idx2;
				}

				if (idx3 >= 0 && idx3 < mapSize && m_pathCost[idx3] > cost + 141)
				{
					m_pathCost[idx3] = cost + 141;
					m_parentBuffer[idx3] = heap;
					m_heapBuffer[stackIdx++] = idx3;
				}

				if (idx4 >= 0 && idx4 < mapSize && m_pathCost[idx4] > cost + 141)
				{
					m_pathCost[idx4] = cost + 141;
					m_parentBuffer[idx4] = heap;
					m_heapBuffer[stackIdx++] = idx4;
				}

				if (stackIdx <= 0)
					break;

				stackSize = stackIdx;
			}
		}

		public override void FindPath(LogicVector2 startPosition, LogicVector2 endPosition, bool clampPathFinderCost)
		{
			m_lineOfSightClear = false;

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
					int startTileIndex = (startPosition.m_x >> 1) + (startPosition.m_y >> 1) * m_mapWidth;
					int endTileIndex = (endPosition.m_x >> 1) + (endPosition.m_y >> 1) * m_mapWidth;

					if (startTileIndex == endTileIndex &&
						IsReachable(startPosition.m_x, startPosition.m_y) &&
						IsReachable(endPosition.m_x, endPosition.m_y) || IsLineOfSightClear(startPosition.m_x, startPosition.m_y, endPosition.m_x, endPosition.m_y))
					{
						m_pathLength = 0;
						m_pathBuffer[m_pathLength++] = endTileIndex;
						m_pathBuffer[m_pathLength++] = startTileIndex;
						m_lineOfSightClear = true;
					}
					else
					{
						AStar(startTileIndex, endTileIndex);

						if (m_pathLength > 0)
						{
							m_pathBuffer[m_pathLength++] = startTileIndex;
						}

						CreatePathPoint(startPosition);
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

		public override bool IsLineOfSightClear()
			=> m_lineOfSightClear;

		public bool IsReachable(int x, int y)
			=> m_tileMap.GetPathFinderCost(x, y) < (m_costStrategyMode ? 0x7FFFFFFF : 1);

		public int Sign(int value)
			=> value > 0 ? 1 : value >> 31;
	}
}