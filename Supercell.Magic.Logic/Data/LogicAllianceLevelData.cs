using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAllianceLevelData : LogicData
	{
		private int m_expPoints;

		private bool m_visible;

		private int m_troopRequestCooldown;
		private int m_troopDonationLimit;
		private int m_troopDonationRefund;
		private int m_troopDonationUpgrade;
		private int m_warLootCapacityPercent;
		private int m_warLootMultiplierPercent;
		private int m_badgeLevel;

		public LogicAllianceLevelData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicAllianceLevelData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_visible = GetBooleanValue("IsVisible", 0);
			m_expPoints = GetIntegerValue("ExpPoints", 0);

			LogicAllianceLevelData previousLevel = null;

			if (GetInstanceID() > 0)
			{
				previousLevel = (LogicAllianceLevelData)m_table.GetItemAt(GetInstanceID() - 1);
			}

			m_troopRequestCooldown = GetIntegerValue("TroopRequestCooldown", 0);

			if (previousLevel != null)
			{
				if (m_troopRequestCooldown == 0)
				{
					m_troopRequestCooldown = previousLevel.m_troopRequestCooldown;
				}
			}

			m_troopDonationLimit = GetIntegerValue("TroopDonationLimit", 0);

			if (previousLevel != null)
			{
				if (m_troopDonationLimit == 0)
				{
					m_troopDonationLimit = previousLevel.m_troopDonationLimit;
				}
			}

			m_troopDonationRefund = GetIntegerValue("TroopDonationRefund", 0);

			if (previousLevel != null)
			{
				if (m_troopDonationRefund == 0)
				{
					m_troopDonationRefund = previousLevel.m_troopDonationRefund;
				}
			}

			m_troopDonationUpgrade = GetIntegerValue("TroopDonationUpgrade", 0);

			if (previousLevel != null)
			{
				if (m_troopDonationUpgrade == 0)
				{
					m_troopDonationUpgrade = previousLevel.m_troopDonationUpgrade;
				}
			}

			m_warLootCapacityPercent = GetIntegerValue("WarLootCapacityPercent", 0);

			if (previousLevel != null)
			{
				if (m_warLootCapacityPercent == 0)
				{
					m_warLootCapacityPercent = previousLevel.m_warLootCapacityPercent;
				}
			}

			m_warLootMultiplierPercent = GetIntegerValue("WarLootMultiplierPercent", 0);

			if (previousLevel != null)
			{
				if (m_warLootMultiplierPercent == 0)
				{
					m_warLootMultiplierPercent = previousLevel.m_warLootMultiplierPercent;
				}
			}

			m_badgeLevel = GetIntegerValue("BadgeLevel", 0);

			if (previousLevel != null)
			{
				if (m_badgeLevel == 0)
				{
					m_badgeLevel = previousLevel.m_badgeLevel;
				}
			}
		}

		public bool IsVisible()
			=> m_visible;

		public int GetExpPoints()
			=> m_expPoints;

		public int GetTroopRequestCooldown()
			=> m_troopRequestCooldown;

		public int GetTroopDonationLimit()
			=> m_troopDonationLimit;

		public int GetTroopDonationRefund()
			=> m_troopDonationRefund;

		public int GetTroopDonationUpgrade()
			=> m_troopDonationUpgrade;

		public int GetWarLootCapacityPercent()
			=> m_warLootCapacityPercent;

		public int GetWarLootMultiplierPercent()
			=> m_warLootMultiplierPercent;

		public int GetBadgeLevel()
			=> m_badgeLevel;
	}
}