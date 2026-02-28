using Supercell.Magic.Logic.Data;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicBundleOfferData : LogicOfferData
	{
		public LogicBundleOfferData(LogicGemBundleData data)
		{
			m_offerId = data.GetGlobalID();
			m_linkedPackageId = data.GetLinkedPackageId();
			m_shopFrontPageCooldownAfterPurchaseSecs = data.GetShopFrontPageCooldownAfterPurchaseSeconds();
		}
	}
}