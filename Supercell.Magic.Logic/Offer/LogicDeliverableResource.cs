using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableResource : LogicDeliverable
	{
		private LogicResourceData m_resourceData;
		private int m_resourceAmount;

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);

			LogicJSONHelper.SetLogicData(jsonObject, "resource", m_resourceData);
			jsonObject.Put("resourceAmount", new LogicJSONNumber(m_resourceAmount));
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);

			m_resourceData = (LogicResourceData)LogicJSONHelper.GetLogicData(jsonObject, "resource");
			m_resourceAmount = LogicJSONHelper.GetInt(jsonObject, "resourceAmount");
		}

		public override int GetDeliverableType()
			=> 1;

		public override bool Deliver(LogicLevel level)
		{
			LogicAvatar avatar = level.GetHomeOwnerAvatar();
			int count = avatar.GetResourceCount(m_resourceData) + m_resourceAmount;

			avatar.SetResourceCount(m_resourceData, count);
			avatar.GetChangeListener().CommodityCountChanged(0, m_resourceData, count);

			return true;
		}

		public LogicResourceData GetResourceData()
			=> m_resourceData;

		public void SetResourceData(LogicResourceData data)
		{
			m_resourceData = data;
		}

		public int GetResourceAmount()
			=> m_resourceAmount;

		public void SetResourceAmount(int value)
		{
			m_resourceAmount = value;
		}
	}
}