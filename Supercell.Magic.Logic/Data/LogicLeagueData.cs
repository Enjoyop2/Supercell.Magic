using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicLeagueData : LogicData
	{
		private string m_leagueBannerIcon;
		private string m_leagueBannerIconNum;
		private string m_leagueBannerIconHUD;

		private int m_goldReward;
		private int m_elixirReward;
		private int m_darkElixirReward;
		private int m_goldRewardStarBonus;
		private int m_elixirRewardStarBonus;
		private int m_darkElixirRewardStarBonus;
		private int m_placementLimitLow;
		private int m_placementLimitHigh;
		private int m_demoteLimit;
		private int m_promoteLimit;
		private int m_allocateAmount;
		private int m_saverCount;
		private int m_villageGuardInMins;

		private int[] m_bucketPlacementRangeLow;
		private int[] m_bucketPlacementRangeHigh;
		private int[] m_bucketPlacementSoftLimit;
		private int[] m_bucketPlacementHardLimit;

		private bool m_useStarBonus;
		private bool m_ignoredByServer;
		private bool m_demoteEnabled;
		private bool m_promoteEnabled;

		public LogicLeagueData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicLeagueData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_leagueBannerIcon = GetValue("LeagueBannerIcon", 0);
			m_leagueBannerIconNum = GetValue("LeagueBannerIconNum", 0);
			m_leagueBannerIconHUD = GetValue("LeagueBannerIconHUD", 0);
			m_goldReward = GetIntegerValue("GoldReward", 0);
			m_elixirReward = GetIntegerValue("ElixirReward", 0);
			m_darkElixirReward = GetIntegerValue("DarkElixirReward", 0);
			m_useStarBonus = GetBooleanValue("UseStarBonus", 0);
			m_goldRewardStarBonus = GetIntegerValue("GoldRewardStarBonus", 0);
			m_elixirRewardStarBonus = GetIntegerValue("ElixirRewardStarBonus", 0);
			m_darkElixirRewardStarBonus = GetIntegerValue("DarkElixirRewardStarBonus", 0);
			m_placementLimitLow = GetIntegerValue("PlacementLimitLow", 0);
			m_placementLimitHigh = GetIntegerValue("PlacementLimitHigh", 0);
			m_demoteLimit = GetIntegerValue("DemoteLimit", 0);
			m_promoteLimit = GetIntegerValue("PromoteLimit", 0);
			m_ignoredByServer = GetBooleanValue("IgnoredByServer", 0);
			m_demoteEnabled = GetBooleanValue("DemoteEnabled", 0);
			m_promoteEnabled = GetBooleanValue("PromoteEnabled", 0);
			m_allocateAmount = GetIntegerValue("AllocateAmount", 0);
			m_saverCount = GetIntegerValue("SaverCount", 0);
			m_villageGuardInMins = GetIntegerValue("VillageGuardInMins", 0);

			int size = m_row.GetBiggestArraySize();

			m_bucketPlacementRangeLow = new int[size];
			m_bucketPlacementRangeHigh = new int[size];
			m_bucketPlacementSoftLimit = new int[size];
			m_bucketPlacementHardLimit = new int[size];

			for (int i = 0; i < size; i++)
			{
				m_bucketPlacementRangeLow[i] = GetIntegerValue("BucketPlacementRangeLow", i);
				m_bucketPlacementRangeHigh[i] = GetIntegerValue("BucketPlacementRangeHigh", i);
				m_bucketPlacementSoftLimit[i] = GetIntegerValue("BucketPlacementSoftLimit", i);
				m_bucketPlacementHardLimit[i] = GetIntegerValue("BucketPlacementHardLimit", i);
			}
		}

		public int GetBucketPlacementRangeLow(int index)
			=> m_bucketPlacementRangeLow[index];

		public int GetBucketPlacementRangeHigh(int index)
			=> m_bucketPlacementRangeHigh[index];

		public int GetBucketPlacementSoftLimit(int index)
			=> m_bucketPlacementSoftLimit[index];

		public int GetBucketPlacementHardLimit(int index)
			=> m_bucketPlacementHardLimit[index];

		public string GetLeagueBannerIcon()
			=> m_leagueBannerIcon;

		public string GetLeagueBannerIconNum()
			=> m_leagueBannerIconNum;

		public string GetLeagueBannerIconHUD()
			=> m_leagueBannerIconHUD;

		public int GetGoldReward()
			=> m_goldReward;

		public int GetElixirReward()
			=> m_elixirReward;

		public int GetDarkElixirReward()
			=> m_darkElixirReward;

		public bool IsUseStarBonus()
			=> m_useStarBonus;

		public int GetGoldRewardStarBonus()
			=> m_goldRewardStarBonus;

		public int GetElixirRewardStarBonus()
			=> m_elixirRewardStarBonus;

		public int GetDarkElixirRewardStarBonus()
			=> m_darkElixirRewardStarBonus;

		public int GetPlacementLimitLow()
			=> m_placementLimitLow;

		public int GetPlacementLimitHigh()
			=> m_placementLimitHigh;

		public int GetDemoteLimit()
			=> m_demoteLimit;

		public int GetPromoteLimit()
			=> m_promoteLimit;

		public bool IsIgnoredByServer()
			=> m_ignoredByServer;

		public bool IsDemoteEnabled()
			=> m_demoteEnabled;

		public bool IsPromoteEnabled()
			=> m_promoteEnabled;

		public int GetAllocateAmount()
			=> m_allocateAmount;

		public int GetSaverCount()
			=> m_saverCount;

		public int GetVillageGuardInMins()
			=> m_villageGuardInMins;
	}
}