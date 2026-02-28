using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicShieldData : LogicData
	{
		private int m_diamondsCost;
		private int m_cooldownSecs;
		private int m_scoreLimit;

		private int m_timeHours;
		private int m_guardTimeHours;

		public LogicShieldData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicShieldData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_diamondsCost = GetIntegerValue("Diamonds", 0);
			m_cooldownSecs = GetIntegerValue("CooldownS", 0);
			m_scoreLimit = GetIntegerValue("LockedAboveScore", 0);
			m_timeHours = GetIntegerValue("TimeH", 0);
			m_guardTimeHours = GetIntegerValue("GuardTimeH", 0);
		}

		public int GetDiamondsCost()
			=> m_diamondsCost;

		public int GetCooldownSecs()
			=> m_cooldownSecs;

		public int GetScoreLimit()
			=> m_scoreLimit;

		public int GetTimeHours()
			=> m_timeHours;

		public int GetGuardTimeHours()
			=> m_guardTimeHours;
	}
}