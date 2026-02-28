namespace Supercell.Magic.Logic.Offer
{
	public class LogicOfferData
	{
		protected int m_offerId;
		protected int m_linkedPackageId;
		protected int m_shopFrontPageCooldownAfterPurchaseSecs;

		public int GetId()
			=> m_offerId;

		public int GetLinkedPackageId()
			=> m_linkedPackageId;

		public int GetShopFrontPageCooldownAfterPurchaseSeconds()
			=> m_shopFrontPageCooldownAfterPurchaseSecs;
	}
}