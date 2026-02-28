using Supercell.Magic.Logic.Data;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicCalendarBuildingDestroyedSpawnUnit
	{
		private readonly LogicBuildingData m_buildingData;
		private readonly LogicCharacterData m_characterData;

		private readonly int m_count;

		public LogicCalendarBuildingDestroyedSpawnUnit(LogicBuildingData buildingData, LogicCharacterData unitData, int count)
		{
			m_buildingData = buildingData;
			m_characterData = unitData;
			m_count = count;
		}

		public LogicBuildingData GetBuildingData()
			=> m_buildingData;

		public LogicCharacterData GetCharacterData()
			=> m_characterData;

		public int GetCount()
			=> m_count;
	}
}