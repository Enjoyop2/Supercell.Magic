using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic
{
	public class LogicConfiguration
	{
		private LogicJSONObject m_json;
		private LogicObstacleData m_specialObstacle;
		private LogicArrayList<int> m_milestoneScoreChangeForLosing;
		private LogicArrayList<int> m_percentageScoreChangeForLosing;
		private LogicArrayList<int> m_milestoneStrengthRangeForScore;
		private LogicArrayList<int> m_percentageStrengthRangeForScore;

		private bool m_battleWaitForDieDamage;
		private bool m_battleWaitForProjectileDestruction;

		private int m_maxTownHallLevel;
		private int m_duelLootLimitCooldownInMinutes;
		private int m_duelBonusLimitWinsPerDay;
		private int m_duelBonusPercentWin;
		private int m_duelBonusPercentLose;
		private int m_duelBonusPercentDraw;
		private int m_duelBonusMaxDiamondCostPercent;

		private string m_giftPackExtension;

		public LogicConfiguration()
		{
		}

		public LogicJSONObject GetJson()
			=> m_json;

		public void Load(LogicJSONObject jsonObject)
		{
			m_json = jsonObject;

			if (jsonObject != null)
			{
				LogicJSONObject village1Object = jsonObject.GetJSONObject("Village1");
				Debugger.DoAssert(village1Object != null, "pVillage1 = NULL!");

				LogicJSONString specialObstacleObject = village1Object.GetJSONString("SpecialObstacle");

				if (specialObstacleObject != null)
				{
					m_specialObstacle = LogicDataTables.GetObstacleByName(specialObstacleObject.GetStringValue(), null);
				}

				LogicJSONObject village2Object = jsonObject.GetJSONObject("Village2");
				Debugger.DoAssert(village2Object != null, "pVillage2 = NULL!");

				m_maxTownHallLevel = LogicJSONHelper.GetInt(village2Object, "TownHallMaxLevel");

				LogicJSONArray scoreChangeForLosingArray = village2Object.GetJSONArray("ScoreChangeForLosing");
				Debugger.DoAssert(scoreChangeForLosingArray != null, "ScoreChangeForLosing array is null");

				m_milestoneScoreChangeForLosing = new LogicArrayList<int>(scoreChangeForLosingArray.Size());
				m_percentageScoreChangeForLosing = new LogicArrayList<int>(scoreChangeForLosingArray.Size());

				for (int i = 0; i < scoreChangeForLosingArray.Size(); i++)
				{
					LogicJSONObject obj = scoreChangeForLosingArray.GetJSONObject(i);

					if (obj != null)
					{
						LogicJSONNumber milestoneObject = obj.GetJSONNumber("Milestone");
						LogicJSONNumber percentageObject = obj.GetJSONNumber("Percentage");

						if (milestoneObject != null && percentageObject != null)
						{
							m_milestoneScoreChangeForLosing.Add(milestoneObject.GetIntValue());
							m_percentageScoreChangeForLosing.Add(percentageObject.GetIntValue());
						}
					}
				}

				LogicJSONArray strengthRangeForScoreArray = village2Object.GetJSONArray("StrengthRangeForScore");
				Debugger.DoAssert(strengthRangeForScoreArray != null, "StrengthRangeForScore array is null");

				m_milestoneStrengthRangeForScore = new LogicArrayList<int>(strengthRangeForScoreArray.Size());
				m_percentageStrengthRangeForScore = new LogicArrayList<int>(strengthRangeForScoreArray.Size());

				for (int i = 0; i < strengthRangeForScoreArray.Size(); i++)
				{
					LogicJSONObject obj = strengthRangeForScoreArray.GetJSONObject(i);

					if (obj != null)
					{
						LogicJSONNumber milestoneObject = obj.GetJSONNumber("Milestone");
						LogicJSONNumber percentageObject = obj.GetJSONNumber("Percentage");

						if (milestoneObject != null && percentageObject != null)
						{
							m_milestoneStrengthRangeForScore.Add(milestoneObject.GetIntValue());
							m_percentageStrengthRangeForScore.Add(percentageObject.GetIntValue());
						}
					}
				}

				LogicJSONObject killSwitchesObject = jsonObject.GetJSONObject("KillSwitches");
				Debugger.DoAssert(killSwitchesObject != null, "pKillSwitches = NULL!");

				m_battleWaitForProjectileDestruction = LogicJSONHelper.GetBool(killSwitchesObject, "BattleWaitForProjectileDestruction");
				m_battleWaitForDieDamage = LogicJSONHelper.GetBool(killSwitchesObject, "BattleWaitForDieDamage");

				LogicJSONObject globalsObject = jsonObject.GetJSONObject("Globals");
				Debugger.DoAssert(globalsObject != null, "pGlobals = NULL!");

				m_giftPackExtension = LogicJSONHelper.GetString(globalsObject, "GiftPackExtension");

				m_duelLootLimitCooldownInMinutes = LogicJSONHelper.GetInt(globalsObject, "DuelLootLimitCooldownInMinutes");
				m_duelBonusLimitWinsPerDay = LogicJSONHelper.GetInt(globalsObject, "DuelBonusLimitWinsPerDay");
				m_duelBonusPercentWin = LogicJSONHelper.GetInt(globalsObject, "DuelBonusPercentWin");
				m_duelBonusPercentLose = LogicJSONHelper.GetInt(globalsObject, "DuelBonusPercentLose");
				m_duelBonusPercentDraw = LogicJSONHelper.GetInt(globalsObject, "DuelBonusPercentDraw");
				m_duelBonusMaxDiamondCostPercent = LogicJSONHelper.GetInt(globalsObject, "DuelBonusMaxDiamondCostPercent");
			}
			else
			{
				Debugger.Error("pConfiguration = NULL!");
			}
		}


		public bool GetBattleWaitForProjectileDestruction()
			=> m_battleWaitForProjectileDestruction;

		public bool GetBattleWaitForDieDamage()
			=> m_battleWaitForDieDamage;

		public int GetMaxTownHallLevel()
			=> m_maxTownHallLevel;

		public int GetDuelBonusLimitWinsPerDay()
			=> m_duelBonusLimitWinsPerDay;

		public int GetDuelLootLimitCooldownInMinutes()
			=> m_duelLootLimitCooldownInMinutes;

		public int GetDuelBonusMaxDiamondCostPercent()
			=> m_duelBonusMaxDiamondCostPercent;

		public LogicObstacleData GetSpecialObstacleData()
			=> m_specialObstacle;

		public int GetDuelBonusPercentWin()
			=> m_duelBonusPercentWin;


		public int GetDuelBonusPercentLose()
			=> m_duelBonusPercentLose;


		public int GetDuelBonusPercentDraw()
			=> m_duelBonusPercentDraw;
	}
}