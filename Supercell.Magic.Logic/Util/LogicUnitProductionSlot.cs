using Supercell.Magic.Logic.Data;

namespace Supercell.Magic.Logic.Util
{
	public class LogicUnitProductionSlot
	{
		private LogicData m_data;

		private int m_count;
		private bool m_terminate;

		public LogicUnitProductionSlot(LogicData data, int count, bool terminate)
		{
			m_data = data;
			m_count = count;
			m_terminate = terminate;
		}

		public void Destruct()
		{
			m_data = null;
			m_count = 0;
		}

		public LogicData GetData()
			=> m_data;

		public int GetCount()
			=> m_count;

		public void SetCount(int count)
		{
			m_count = count;
		}

		public bool IsTerminate()
			=> m_terminate;

		public void SetTerminate(bool terminate)
		{
			m_terminate = terminate;
		}
	}
}