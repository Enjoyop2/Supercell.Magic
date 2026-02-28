using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Data
{
	public class LogicResourceData : LogicData
	{
		private string m_resourceIconExportName;
		private string m_hudInstanceName;
		private string m_capFullTID;
		private string m_bundleIconExportName;

		private int m_stealLimitMid;
		private int m_stealLimitBig;
		private int m_textRed;
		private int m_textGreen;
		private int m_textBlue;
		private int m_villageType;

		private bool m_premiumCurrency;

		private LogicEffectData m_collectEffect;
		private LogicEffectData m_stealEffect;
		private LogicResourceData m_warResourceReferenceData;
		private LogicEffectData m_stealEffectMid;
		private LogicEffectData m_stealEffectBig;

		public LogicResourceData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicResourceData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_stealEffect = LogicDataTables.GetEffectByName(GetValue("StealEffect", 0), this);
			m_collectEffect = LogicDataTables.GetEffectByName(GetValue("CollectEffect", 0), this);

			m_resourceIconExportName = GetValue("ResourceIconExportName", 0);
			m_stealLimitMid = GetIntegerValue("StealLimitMid", 0);
			m_stealEffectMid = LogicDataTables.GetEffectByName(GetValue("StealEffectMid", 0), this);
			m_stealLimitBig = GetIntegerValue("StealLimitBig", 0);
			m_stealEffectBig = LogicDataTables.GetEffectByName(GetValue("StealEffectBig", 0), this);
			m_premiumCurrency = GetBooleanValue("PremiumCurrency", 0);
			m_hudInstanceName = GetValue("HudInstanceName", 0);
			m_capFullTID = GetValue("CapFullTID", 0);
			m_textRed = GetIntegerValue("TextRed", 0);
			m_textGreen = GetIntegerValue("TextGreen", 0);
			m_textBlue = GetIntegerValue("TextBlue", 0);
			m_bundleIconExportName = GetValue("BundleIconExportName", 0);
			m_villageType = GetIntegerValue("VillageType", 0);

			if ((uint)m_villageType >= 2)
			{
				Debugger.Error("invalid VillageType");
			}

			string warRefResource = GetValue("WarRefResource", 0);

			if (warRefResource.Length > 0)
			{
				m_warResourceReferenceData = LogicDataTables.GetResourceByName(warRefResource, this);
			}
		}

		public LogicResourceData GetWarResourceReferenceData()
			=> m_warResourceReferenceData;

		public LogicEffectData GetCollectEffect()
			=> m_collectEffect;

		public string GetResourceIconExportName()
			=> m_resourceIconExportName;

		public LogicEffectData GetStealEffect()
			=> m_stealEffect;

		public int GetStealLimitMid()
			=> m_stealLimitMid;

		public LogicEffectData GetStealEffectMid()
			=> m_stealEffectMid;

		public int GetStealLimitBig()
			=> m_stealLimitBig;

		public LogicEffectData GetStealEffectBig()
			=> m_stealEffectBig;

		public bool IsPremiumCurrency()
			=> m_premiumCurrency;

		public string GetHudInstanceName()
			=> m_hudInstanceName;

		public string GetCapFullTID()
			=> m_capFullTID;

		public int GetTextRed()
			=> m_textRed;

		public int GetTextGreen()
			=> m_textGreen;

		public int GetTextBlue()
			=> m_textBlue;

		public string GetBundleIconExportName()
			=> m_bundleIconExportName;

		public int GetVillageType()
			=> m_villageType;
	}
}