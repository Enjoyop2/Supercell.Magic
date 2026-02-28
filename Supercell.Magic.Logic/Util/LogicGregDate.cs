namespace Supercell.Magic.Logic.Util
{
	public class LogicGregDate
	{
		private int m_year;
		private int m_month;
		private int m_day;

		private int m_index;

		public LogicGregDate(int year, int month, int day)
		{
			m_year = year;
			m_month = month;
			m_day = day;

			CalculateIndex();
		}

		public LogicGregDate(int index)
		{
			m_index = index;
			CalculateDate();
		}

		public void CalculateIndex()
		{
			int year = m_year;
			int month = m_month;
			int day = m_day;

			if (month <= 2)
			{
				--year;
			}

			m_index = 1461 * (year % 100) / 4
						   + (153 * (month + (month <= 2 ? 9 : -3)) + 2) / 5
						   + day
						   + 146097 * (year / 100) / 4
						   - 719469;
		}

		public void CalculateDate()
		{
			int tmp1 = 4 * m_index + 2877875;
			int tmp2 = (tmp1 % 146097 + (int)((uint)((tmp1 % 146097) >> 31) >> 30)) | 3;
			int tmp3 = 5 * ((tmp2 % 1461 + 4) / 4);

			int y = tmp2 / 1461 + 100 * (tmp1 / 146097);
			int m = tmp3 / 153;
			int d = (tmp3 - 153 * (tmp3 / 153) + 2) / 5;

			if (m < 10)
			{
				m = m + 3;
			}
			else
			{
				m = m - 9;
				y = y + 1;
			}

			m_year = y;
			m_month = m;
			m_day = d;
		}

		public bool Validate()
		{
			int year = m_year;
			int month = m_month;
			int day = m_day;

			if (month <= 2)
			{
				--year;
			}

			int idx = 1461 * (year % 100) / 4
					  + (153 * (month + (month <= 2 ? 9 : -3)) + 2) / 5
					  + day
					  + 146097 * (year / 100) / 4
					  - 719469;

			CalculateDate();

			return m_day == day && m_month == month && m_year == year && m_index == idx;
		}

		public int GetYear()
			=> m_year;

		public int GetMonth()
			=> m_month;

		public int GetDay()
			=> m_day;

		public int GetIndex()
			=> m_index;
	}
}