using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Util
{
	public class LogicPathFinder
	{
		protected LogicTileMap m_tileMap;

		public LogicPathFinder(LogicTileMap tileMap)
		{
			m_tileMap = tileMap;
		}

		public virtual void Destruct()
		{
			m_tileMap = null;
		}

		public virtual void SetCostStrategy(bool enabled, int quality)
		{
			// SetCostStrategy.
		}

		public virtual void ResetCostStrategyToDefault()
		{
			// ResetCostStrategyToDefault.
		}

		public virtual void FindPath(LogicVector2 startPosition, LogicVector2 endPosition, bool clampPathFinderCost)
		{
			// FindPath.
		}

		public virtual int GetPathLength()
			=> 0;

		public virtual void GetPathPoint(LogicVector2 position, int idx)
		{
			// GetPathPoint.
		}

		public virtual void GetPathPointSubTile(LogicVector2 position, int idx)
		{
			// GetPathPointSubTile.
		}

		public virtual void InvalidateCache()
		{
			// InvalidateCache.
		}

		public virtual int GetParent(int index)
			=> 0;

		public virtual bool IsLineOfSightClear()
			=> false;

		public LogicTileMap GetTileMap()
			=> m_tileMap;
	}
}