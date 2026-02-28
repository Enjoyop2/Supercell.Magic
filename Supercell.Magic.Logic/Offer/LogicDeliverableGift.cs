using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableGift : LogicDeliverableBundle
	{
		private int m_giftLimit;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);
			jsonObject.Put("giftLimit", new LogicJSONNumber(m_giftLimit));
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);
			m_giftLimit = LogicJSONHelper.GetInt(jsonObject, "giftLimit");
		}

		public override int GetDeliverableType()
			=> 6;

		public override bool Deliver(LogicLevel level)
		{
			level.GetHomeOwnerAvatar().GetChangeListener().DeliverableAllianceGift(this);
			return true;
		}

		public override bool CanBeDeliver(LogicLevel level)
			=> level.GetHomeOwnerAvatar().IsInAlliance();

		public int GetGiftLimit()
			=> m_giftLimit;

		public void SetGiftLimit(int value)
		{
			m_giftLimit = value;
		}
	}
}