using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicLeagueVillage2Data : LogicData
	{
		private int m_trophyLimitLow;
		private int m_trophyLimitHigh;
		private int m_goldReward;
		private int m_elixirReward;
		private int m_bonusGold;
		private int m_bonusElixir;
		private int m_seasonTrophyReset;
		private int m_maxDiamondCost;

		public LogicLeagueVillage2Data(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicLeagueVillage2Data.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_trophyLimitLow = GetIntegerValue("TrophyLimitLow", 0);
			m_trophyLimitHigh = GetIntegerValue("TrophyLimitHigh", 0);
			m_goldReward = GetIntegerValue("GoldReward", 0);
			m_elixirReward = GetIntegerValue("ElixirReward", 0);
			m_bonusGold = GetIntegerValue("BonusGold", 0);
			m_bonusElixir = GetIntegerValue("BonusElixir", 0);
			m_seasonTrophyReset = GetIntegerValue("SeasonTrophyReset", 0);
			m_maxDiamondCost = GetIntegerValue("MaxDiamondCost", 0);
		}

		public int GetTrophyLimitLow()
			=> m_trophyLimitLow;

		public int GetTrophyLimitHigh()
			=> m_trophyLimitHigh;

		public int GetGoldReward()
			=> m_goldReward;

		public int GetElixirReward()
			=> m_elixirReward;

		public int GetBonusGold()
			=> m_bonusGold;

		public int GetBonusElixir()
			=> m_bonusElixir;

		public int GetSeasonTrophyReset()
			=> m_seasonTrophyReset;

		public int GetMaxDiamondCost()
			=> m_maxDiamondCost;
	}
}