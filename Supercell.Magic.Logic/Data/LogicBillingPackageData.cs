using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicBillingPackageData : LogicData
	{
		private string m_shopItemExportName;
		private string m_offerItemExportName;
		private string m_tencentID;

		private int m_diamonds;
		private int m_usd;
		private int m_order;
		private int m_rmb;
		private int m_lenovoID;

		private bool m_disabled;
		private bool m_existsApple;
		private bool m_existsAndroid;
		private bool m_red;
		private bool m_kunlunOnly;
		private bool m_isOfferPackage;

		public LogicBillingPackageData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicBillingPackageData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_disabled = GetBooleanValue("Disabled", 0);
			m_existsApple = GetBooleanValue("ExistsApple", 0);
			m_existsAndroid = GetBooleanValue("ExistsAndroid", 0);
			m_diamonds = GetIntegerValue("Diamonds", 0);
			m_usd = GetIntegerValue("USD", 0);
			m_shopItemExportName = GetValue("ShopItemExportName", 0);
			m_offerItemExportName = GetValue("OfferItemExportName", 0);
			m_order = GetIntegerValue("Order", 0);
			m_red = GetBooleanValue("RED", 0);
			m_rmb = GetIntegerValue("RMB", 0);
			m_kunlunOnly = GetBooleanValue("KunlunOnly", 0);
			m_lenovoID = GetIntegerValue("LenovoID", 0);
			m_tencentID = GetValue("TencentID", 0);
			m_isOfferPackage = GetBooleanValue("isOfferPackage", 0);
		}

		public bool Disabled()
			=> m_disabled;

		public bool ExistsApple()
			=> m_existsApple;

		public bool ExistsAndroid()
			=> m_existsAndroid;

		public int GetDiamonds()
			=> m_diamonds;

		public int GetUSD()
			=> m_usd;

		public string GetShopItemExportName()
			=> m_shopItemExportName;

		public string GetOfferItemExportName()
			=> m_offerItemExportName;

		public int GetOrder()
			=> m_order;

		public bool IsRED()
			=> m_red;

		public int GetRMB()
			=> m_rmb;

		public bool IsKunlunOnly()
			=> m_kunlunOnly;

		public int GetLenovoID()
			=> m_lenovoID;

		public string GetTencentID()
			=> m_tencentID;

		public bool IsOfferPackage()
			=> m_isOfferPackage;
	}
}