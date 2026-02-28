using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicCalendarTargeting
	{
		private int m_minTownHallLevel;
		private int m_maxTownHallLevel;
		private int m_minGemsLevel;
		private int m_maxGemsLevel;

		public LogicCalendarTargeting(LogicJSONObject jsonObject)
		{
			Load(jsonObject);
		}

		public void Load(LogicJSONObject jsonObject)
		{
			Debugger.DoAssert(jsonObject != null, "Unable to load targeting");

			m_minTownHallLevel = LogicJSONHelper.GetInt(jsonObject, "minTownHallLevel") & 0x7FFFFFFF;
			m_maxTownHallLevel = LogicJSONHelper.GetInt(jsonObject, "maxTownHallLevel") & 0x7FFFFFFF;
			m_minGemsLevel = LogicJSONHelper.GetInt(jsonObject, "minGemsPurchased") & 0x7FFFFFFF;
			m_maxGemsLevel = LogicJSONHelper.GetInt(jsonObject, "maxGemsPurchased") & 0x7FFFFFFF;
		}

		public void Save(LogicJSONObject jsonObject)
		{
			Debugger.DoAssert(jsonObject != null, "Unable to save targeting");

			jsonObject.Put("minTownHallLevel", new LogicJSONNumber(m_minTownHallLevel));
			jsonObject.Put("maxTownHallLevel", new LogicJSONNumber(m_maxTownHallLevel));
			jsonObject.Put("minGemsPurchased", new LogicJSONNumber(m_minGemsLevel));
			jsonObject.Put("maxGemsPurchased", new LogicJSONNumber(m_maxGemsLevel));
		}
	}
}