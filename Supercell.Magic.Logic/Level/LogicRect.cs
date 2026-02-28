namespace Supercell.Magic.Logic.Level
{
	public sealed class LogicRect
	{
		private readonly int m_startX;
		private readonly int m_startY;
		private readonly int m_endX;
		private readonly int m_endY;

		public LogicRect(int startX, int startY, int endX, int endY)
		{
			m_startX = startX;
			m_startY = startY;
			m_endX = endX;
			m_endY = endY;
		}

		public void Destruct()
		{
			// Destruct.
		}

		public int GetStartX()
			=> m_startX;

		public int GetStartY()
			=> m_startY;

		public int GetEndX()
			=> m_endX;

		public int GetEndY()
			=> m_endY;

		public bool IsInside(int x, int y)
		{
			if (m_startX <= x)
			{
				if (m_startY <= y)
				{
					return m_endX >= x && m_endY >= y;
				}
			}

			return false;
		}

		public bool IsInside(LogicRect rect)
		{
			if (m_startX <= rect.m_startX)
			{
				if (m_startY <= rect.m_startY)
				{
					return m_endX > rect.m_endX && m_endY > rect.m_endY;
				}
			}

			return false;
		}
	}
}