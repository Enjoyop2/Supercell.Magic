using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicNewsData : LogicData
	{
		private string m_type;
		private string m_buttonTID;
		private string m_actionType;
		private string m_actionParameter1;
		private string m_actionParameter2;
		private string m_nativeAndroidURL;
		private string m_includedCountries;
		private string m_excludedCountries;
		private string m_excludedLoginCountries;
		private string m_itemSWF;
		private string m_itemExportName;
		private string m_iconSWF;
		private string m_iconExportName;
		private string m_minOS;
		private string m_maxOS;
		private string m_buttonTID2;
		private string m_action2Type;
		private string m_action2Parameter1;
		private string m_action2Parameter2;

		private int m_id;
		private int m_iconFrame;
		private int m_minTownHall;
		private int m_maxTownHall;
		private int m_minLevel;
		private int m_maxLevel;
		private int m_maxDiamonds;
		private int m_notifyMinLevel;
		private int m_avatarIdModulo;
		private int m_moduloMin;
		private int m_moduloMax;

		private bool m_enabled;
		private bool m_enabledIOS;
		private bool m_enabledAndroid;
		private bool m_enabledKunlun;
		private bool m_enabledTencent;
		private bool m_enabledLowEnd;
		private bool m_enabledHighEnd;
		private bool m_showAsNew;
		private bool m_centerText;
		private bool m_loadResources;
		private bool m_loadInLowEnd;
		private bool m_animateIcon;
		private bool m_centerIcon;
		private bool m_clickToDismiss;
		private bool m_notifyAlways;
		private bool m_collapsed;

		public LogicNewsData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicNewsData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_id = GetIntegerValue("ID", 0);
			m_enabled = GetBooleanValue("Enabled", 0);
			m_enabledIOS = GetBooleanValue("EnabledIOS", 0);
			m_enabledAndroid = GetBooleanValue("EnabledAndroid", 0);
			m_enabledKunlun = GetBooleanValue("EnabledKunlun", 0);
			m_enabledTencent = GetBooleanValue("EnabledTencent", 0);
			m_enabledLowEnd = GetBooleanValue("EnabledLowEnd", 0);
			m_enabledHighEnd = GetBooleanValue("EnabledHighEnd", 0);
			m_type = GetValue("Type", 0);
			m_showAsNew = GetBooleanValue("ShowAsNew", 0);
			m_buttonTID = GetValue("ButtonTID", 0);
			m_actionType = GetValue("ActionType", 0);
			m_actionParameter1 = GetValue("ActionParameter1", 0);
			m_actionParameter2 = GetValue("ActionParameter2", 0);
			m_nativeAndroidURL = GetValue("NativeAndroidURL", 0);
			m_includedCountries = GetValue("IncludedCountries", 0);
			m_excludedCountries = GetValue("ExcludedCountries", 0);
			m_excludedLoginCountries = GetValue("ExcludedLoginCountries", 0);
			m_centerText = GetBooleanValue("CenterText", 0);
			m_loadResources = GetBooleanValue("LoadResources", 0);
			m_loadInLowEnd = GetBooleanValue("LoadInLowEnd", 0);
			m_itemSWF = GetValue("ItemSWF", 0);
			m_itemExportName = GetValue("ItemExportName", 0);
			m_iconSWF = GetValue("IconSWF", 0);
			m_iconExportName = GetValue("IconExportName", 0);
			m_iconFrame = GetIntegerValue("IconFrame", 0);
			m_animateIcon = GetBooleanValue("AnimateIcon", 0);
			m_centerIcon = GetBooleanValue("CenterIcon", 0);
			m_minTownHall = GetIntegerValue("MinTownHall", 0);
			m_maxTownHall = GetIntegerValue("MaxTownHall", 0);
			m_minLevel = GetIntegerValue("MinLevel", 0);
			m_maxLevel = GetIntegerValue("MaxLevel", 0);
			m_maxDiamonds = GetIntegerValue("MaxDiamonds", 0);
			m_clickToDismiss = GetBooleanValue("ClickToDismiss", 0);
			m_notifyAlways = GetBooleanValue("NotifyAlways", 0);
			m_notifyMinLevel = GetIntegerValue("NotifyMinLevel", 0);
			m_avatarIdModulo = GetIntegerValue("AvatarIdModulo", 0);
			m_moduloMin = GetIntegerValue("ModuloMin", 0);
			m_moduloMax = GetIntegerValue("ModuloMax", 0);
			m_collapsed = GetBooleanValue("Collapsed", 0);
			m_minOS = GetValue("MinOS", 0);
			m_maxOS = GetValue("MaxOS", 0);
			m_buttonTID2 = GetValue("ButtonTID2", 0);
			m_action2Type = GetValue("Action2Type", 0);
			m_action2Parameter1 = GetValue("Action2Parameter1", 0);
			m_action2Parameter2 = GetValue("Action2Parameter2", 0);
		}

		public int GetID()
			=> m_id;

		public bool IsEnabled()
			=> m_enabled;

		public bool IsEnabledIOS()
			=> m_enabledIOS;

		public bool IsEnabledAndroid()
			=> m_enabledAndroid;

		public bool IsEnabledKunlun()
			=> m_enabledKunlun;

		public bool IsEnabledTencent()
			=> m_enabledTencent;

		public bool IsEnabledLowEnd()
			=> m_enabledLowEnd;

		public bool IsEnabledHighEnd()
			=> m_enabledHighEnd;

		public string GetType()
			=> m_type;

		public bool IsShowAsNew()
			=> m_showAsNew;

		public string GetButtonTID()
			=> m_buttonTID;

		public string GetActionType()
			=> m_actionType;

		public string GetActionParameter1()
			=> m_actionParameter1;

		public string GetActionParameter2()
			=> m_actionParameter2;

		public string GetNativeAndroidURL()
			=> m_nativeAndroidURL;

		public string GetIncludedCountries()
			=> m_includedCountries;

		public string GetExcludedCountries()
			=> m_excludedCountries;

		public string GetExcludedLoginCountries()
			=> m_excludedLoginCountries;

		public bool IsCenterText()
			=> m_centerText;

		public bool IsLoadResources()
			=> m_loadResources;

		public bool IsLoadInLowEnd()
			=> m_loadInLowEnd;

		public string GetItemSWF()
			=> m_itemSWF;

		public string GetItemExportName()
			=> m_itemExportName;

		public string GetIconSWF()
			=> m_iconSWF;

		public int GetIconFrame()
			=> m_iconFrame;

		public bool IsAnimateIcon()
			=> m_animateIcon;

		public bool IsCenterIcon()
			=> m_centerIcon;

		public int GetMinTownHall()
			=> m_minTownHall;

		public int GetMaxTownHall()
			=> m_maxTownHall;

		public int GetMinLevel()
			=> m_minLevel;

		public int GetMaxLevel()
			=> m_maxLevel;

		public int GetMaxDiamonds()
			=> m_maxDiamonds;

		public bool IsClickToDismiss()
			=> m_clickToDismiss;

		public bool IsNotifyAlways()
			=> m_notifyAlways;

		public int GetNotifyMinLevel()
			=> m_notifyMinLevel;

		public int GetAvatarIdModulo()
			=> m_avatarIdModulo;

		public int GetModuloMin()
			=> m_moduloMin;

		public int GetModuloMax()
			=> m_moduloMax;

		public bool IsCollapsed()
			=> m_collapsed;

		public string GetMinOS()
			=> m_minOS;

		public string GetMaxOS()
			=> m_maxOS;

		public string GetButtonTID2()
			=> m_buttonTID2;

		public string GetAction2Type()
			=> m_action2Type;

		public string GetAction2Parameter1()
			=> m_action2Parameter1;

		public string GetAction2Parameter2()
			=> m_action2Parameter2;
	}
}