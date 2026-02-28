using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicEventEntryData : LogicData
	{
		private string m_itemSWF;
		private string m_itemExportName;
		private string m_upcomingItemExportName;
		private string m_titleTID;
		private string m_upcomingTitleTID;
		private string m_buttonTID;
		private string m_buttonAction;
		private string m_buttonActionData;
		private string m_button2TID;
		private string m_button2Action;
		private string m_button2ActionData;
		private string m_buttonLanguage;

		private bool m_loadSWF;

		public LogicEventEntryData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicEventEntryData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_itemSWF = GetValue("ItemSWF", 0);
			m_itemExportName = GetValue("ItemExportName", 0);
			m_upcomingItemExportName = GetValue("UpcomingItemExportName", 0);
			m_loadSWF = GetBooleanValue("LoadSWF", 0);
			m_titleTID = GetValue("TitleTID", 0);
			m_upcomingTitleTID = GetValue("UpcomingTitleTID", 0);
			m_buttonTID = GetValue("ButtonTID", 0);
			m_buttonAction = GetValue("ButtonAction", 0);
			m_buttonActionData = GetValue("ButtonActionData", 0);
			m_button2TID = GetValue("Button2TID", 0);
			m_button2Action = GetValue("Button2Action", 0);
			m_button2ActionData = GetValue("Button2ActionData", 0);
			m_buttonLanguage = GetValue("ButtonLanguage", 0);
		}

		public string GetItemSWF()
			=> m_itemSWF;

		public string GetItemExportName()
			=> m_itemExportName;

		public string GetUpcomingItemExportName()
			=> m_upcomingItemExportName;

		public bool IsLoadSWF()
			=> m_loadSWF;

		public string GetTitleTID()
			=> m_titleTID;

		public string GetUpcomingTitleTID()
			=> m_upcomingTitleTID;

		public string GetButtonTID()
			=> m_buttonTID;

		public string GetButtonAction()
			=> m_buttonAction;

		public string GetButtonActionData()
			=> m_buttonActionData;

		public string GetButton2TID()
			=> m_button2TID;

		public string GetButton2Action()
			=> m_button2Action;

		public string GetButton2ActionData()
			=> m_button2ActionData;

		public string GetButtonLanguage()
			=> m_buttonLanguage;
	}
}