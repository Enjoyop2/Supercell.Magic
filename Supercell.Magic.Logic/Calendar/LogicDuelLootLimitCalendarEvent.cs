using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicDuelLootLimitCalendarEvent : LogicCalendarEvent
	{
		private int m_duelLootLimitCooldownInMinutes;
		private int m_duelBonusPercentWin;
		private int m_duelBonusPercentLose;
		private int m_duelBonusPercentDraw;
		private int m_duelBonusMaxDiamondCostPercent;

		public override void Load(LogicJSONObject jsonObject)
		{
			base.Load(jsonObject);

			m_duelLootLimitCooldownInMinutes = LogicJSONHelper.GetInt(jsonObject, "lootLimitCooldownInMinutes", 1320);
			m_duelBonusPercentWin = LogicJSONHelper.GetInt(jsonObject, "duelBonusPercentWin", 100);
			m_duelBonusPercentLose = LogicJSONHelper.GetInt(jsonObject, "duelBonusPercentLose", 0);
			m_duelBonusPercentDraw = LogicJSONHelper.GetInt(jsonObject, "duelBonusPercentDraw", 0);
			m_duelBonusMaxDiamondCostPercent = LogicJSONHelper.GetInt(jsonObject, "duelBonusMaxDiamondCostPercent", 100);
		}

		public override LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = base.Save();

			jsonObject.Put("lootLimitCooldownInMinutes", new LogicJSONNumber(m_duelLootLimitCooldownInMinutes));
			jsonObject.Put("duelBonusPercentWin", new LogicJSONNumber(m_duelBonusPercentWin));
			jsonObject.Put("duelBonusPercentLose", new LogicJSONNumber(m_duelBonusPercentLose));
			jsonObject.Put("duelBonusPercentDraw", new LogicJSONNumber(m_duelBonusPercentDraw));
			jsonObject.Put("duelBonusMaxDiamondCostPercent", new LogicJSONNumber(m_duelBonusMaxDiamondCostPercent));

			return jsonObject;
		}

		public override int GetCalendarEventType()
			=> LogicCalendarEvent.EVENT_TYPE_DUEL_LOOT_LIMIT;

		public int GetDuelLootLimitCooldownInMinutes()
			=> m_duelLootLimitCooldownInMinutes;

		public void SetDuelLootLimitCooldownInMinutes(int value)
		{
			m_duelLootLimitCooldownInMinutes = value;
		}

		public int GetDuelBonusPercentWin()
			=> m_duelBonusPercentWin;

		public void SetDuelBonusPercentWin(int value)
		{
			m_duelBonusPercentWin = value;
		}

		public int GetDuelBonusPercentLose()
			=> m_duelBonusPercentLose;

		public void SetDuelBonusPercentLose(int value)
		{
			m_duelBonusPercentLose = value;
		}

		public int GetDuelBonusPercentDraw()
			=> m_duelBonusPercentDraw;

		public void SetDuelBonusPercentDraw(int value)
		{
			m_duelBonusPercentDraw = value;
		}

		public int GetDuelBonusMaxDiamondCostPercent()
			=> m_duelBonusMaxDiamondCostPercent;

		public void SetDuelBonusMaxDiamondCostPercent(int value)
		{
			m_duelBonusMaxDiamondCostPercent = value;
		}
	}
}