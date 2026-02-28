using System;

namespace Supercell.Magic.Logic.Util
{
	public class LogicSavedPath
	{
		private int[] m_path;
		private int m_size;
		private int m_length;
		private int m_startTile;
		private int m_endTile;
		private int m_strategy;
		private int m_extractCount;

		public LogicSavedPath(int size)
		{
			m_path = new int[size];
			m_size = size;
		}

		public void Destruct()
		{
			m_path = null;
			m_size = 0;
			m_length = 0;
			m_startTile = 0;
			m_endTile = -1;
			m_strategy = 0;
		}

		public int GetLength()
			=> m_length;

		public void StorePath(int[] path, int length, int startTile, int endTile, int costStrategy)
		{
			if (m_size >= length)
			{
				if (length > 0)
				{
					Array.Copy(path, m_path, length);
				}

				m_extractCount = 0;
				m_startTile = startTile;
				m_endTile = endTile;
				m_length = length;
				m_strategy = costStrategy;
			}
		}

		public void ExtractPath(int[] path)
		{
			++m_extractCount;
			Array.Copy(m_path, path, m_length);
		}

		public bool IsEqual(int startTile, int endTile, int costStrategy)
			=> m_startTile == startTile && m_endTile == endTile && m_strategy == costStrategy;
	}
}