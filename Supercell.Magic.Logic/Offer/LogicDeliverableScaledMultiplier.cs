using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableScaledMultiplier : LogicDeliverable
	{
		private LogicResourceData m_scaledResourceData;
		private int m_scaledResourceMultiplier;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);

			LogicJSONHelper.SetLogicData(jsonObject, "scaledResource", m_scaledResourceData);
			jsonObject.Put("scaledResourceMultiplier", new LogicJSONNumber(m_scaledResourceMultiplier));
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);

			m_scaledResourceData = (LogicResourceData)LogicJSONHelper.GetLogicData(jsonObject, "scaledResource");
			m_scaledResourceMultiplier = LogicJSONHelper.GetInt(jsonObject, "scaledResourceMultiplier");
		}

		public override int GetDeliverableType()
			=> 7;

		public override bool Deliver(LogicLevel level)
		{
			LogicAvatar avatar = level.GetHomeOwnerAvatar();
			int count = avatar.GetResourceCount(m_scaledResourceData) + m_scaledResourceMultiplier;

			avatar.SetResourceCount(m_scaledResourceData, count);
			avatar.GetChangeListener().CommodityCountChanged(0, m_scaledResourceData, count);

			return true;
		}

		public override bool CanBeDeliver(LogicLevel level)
			=> true;

		public LogicResourceData GetScaledResourceData()
			=> m_scaledResourceData;

		public void SetScaledResourceData(LogicResourceData data)
		{
			m_scaledResourceData = data;
		}

		public int GetScaledResourceMultiplier()
			=> m_scaledResourceMultiplier;

		public void SetScaledResourceMultiplier(int value)
		{
			m_scaledResourceMultiplier = value;
		}
	}
}