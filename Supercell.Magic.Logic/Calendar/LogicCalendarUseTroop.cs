using Supercell.Magic.Logic.Data;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicCalendarUseTroop
	{
		private readonly LogicCombatItemData m_data;
		private readonly LogicArrayList<int> m_parameters;

		public LogicCalendarUseTroop(LogicCombatItemData data)
		{
			m_data = data;
			m_parameters = new LogicArrayList<int>();
		}

		public LogicCombatItemData GetData()
			=> m_data;

		public void AddParameter(int parameter)
		{
			m_parameters.Add(parameter);
		}

		public int GetParameter(int idx)
			=> m_parameters[idx];
	}
}