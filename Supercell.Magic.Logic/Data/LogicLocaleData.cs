using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicLocaleData : LogicData
	{
		private string m_fileName;
		private string m_localizedName;
		private string m_usedSystemFont;
		private string m_helpshiftSDKLanguage;
		private string m_helpshiftSDKLanguageAndroid;
		private string m_boomboxUrl;
		private string m_boomboxStagingUrl;
		private string m_helpshiftLanguageTagOverride;

		private int m_sortOrder;

		private bool m_hasEvenSpaceCharacters;
		private bool m_isRTL;
		private bool m_testLanguage;
		private bool m_boomboxEnabled;

		public LogicLocaleData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicLocaleData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_fileName = GetValue("FileName", 0);
			m_localizedName = GetValue("LocalizedName", 0);
			m_hasEvenSpaceCharacters = GetBooleanValue("HasEvenSpaceCharacters", 0);
			m_isRTL = GetBooleanValue("isRTL", 0);
			m_usedSystemFont = GetValue("UsedSystemFont", 0);
			m_helpshiftSDKLanguage = GetValue("HelpshiftSDKLanguage", 0);
			m_helpshiftSDKLanguageAndroid = GetValue("HelpshiftSDKLanguageAndroid", 0);
			m_sortOrder = GetIntegerValue("SortOrder", 0);
			m_testLanguage = GetBooleanValue("TestLanguage", 0);
			m_boomboxEnabled = GetBooleanValue("BoomboxEnabled", 0);
			m_boomboxUrl = GetValue("BoomboxUrl", 0);
			m_boomboxStagingUrl = GetValue("BoomboxStagingUrl", 0);
			m_helpshiftLanguageTagOverride = GetValue("HelpshiftLanguageTagOverride", 0);
		}

		public string GetFileName()
			=> m_fileName;

		public string GetLocalizedName()
			=> m_localizedName;

		public bool IsHasEvenSpaceCharacters()
			=> m_hasEvenSpaceCharacters;

		public bool IsRTL()
			=> m_isRTL;

		public string GetUsedSystemFont()
			=> m_usedSystemFont;

		public string GetHelpshiftSDKLanguage()
			=> m_helpshiftSDKLanguage;

		public string GetHelpshiftSDKLanguageAndroid()
			=> m_helpshiftSDKLanguageAndroid;

		public int GetSortOrder()
			=> m_sortOrder;

		public bool IsTestLanguage()
			=> m_testLanguage;

		public bool IsBoomboxEnabled()
			=> m_boomboxEnabled;

		public string GetBoomboxUrl()
			=> m_boomboxUrl;

		public string GetBoomboxStagingUrl()
			=> m_boomboxStagingUrl;

		public string GetHelpshiftLanguageTagOverride()
			=> m_helpshiftLanguageTagOverride;
	}
}