using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableBundle : LogicDeliverable
	{
		private LogicArrayList<LogicDeliverable> m_deliverables;

		public LogicDeliverableBundle()
		{
			m_deliverables = new LogicArrayList<LogicDeliverable>();
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_deliverables != null)
			{
				while (m_deliverables.Size() != 0)
				{
					m_deliverables[0].Destruct();
					m_deliverables.Remove(0);
				}

				m_deliverables = null;
			}
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);

			LogicJSONArray jsonArray = new LogicJSONArray(m_deliverables.Size());

			for (int i = 0; i < m_deliverables.Size(); i++)
			{
				LogicJSONObject obj = new LogicJSONObject();
				m_deliverables[i].WriteToJSON(obj);
				jsonArray.Add(obj);
			}

			jsonObject.Put("dels", jsonArray);
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);

			LogicJSONArray jsonArray = jsonObject.GetJSONArray("dels");

			if (jsonArray != null)
			{
				for (int i = 0; i < jsonArray.Size(); i++)
				{
					LogicJSONObject obj = jsonArray.GetJSONObject(i);

					if (obj != null)
					{
						m_deliverables.Add(LogicJSONHelper.GetLogicDeliverable(obj));
					}
				}
			}
		}

		public override int GetDeliverableType()
			=> 5;

		public override bool Deliver(LogicLevel level)
		{
			for (int i = 0; i < m_deliverables.Size(); i++)
			{
				m_deliverables[i].Deliver(level);
			}

			return true;
		}

		public override bool CanBeDeliver(LogicLevel level)
			=> true;

		public override LogicDeliverableBundle Compensate(LogicLevel level)
		{
			Debugger.Warning("LogicDeliverableBundle cannot handle compensations. Use LogicDeliveryHelper instead.");
			return null;
		}

		public void AddResources(LogicResourceData data, int count)
		{
			LogicDeliverableResource instance = null;

			for (int i = 0; i < m_deliverables.Size(); i++)
			{
				LogicDeliverable deliverable = m_deliverables[i];

				if (deliverable.GetDeliverableType() == 1)
				{
					LogicDeliverableResource deliverableResource = (LogicDeliverableResource)deliverable;

					if (deliverableResource.GetResourceData() == data)
					{
						instance = deliverableResource;
						break;
					}
				}
			}

			if (instance != null)
			{
				instance.SetResourceAmount(instance.GetResourceAmount() + count);
			}
			else
			{
				LogicDeliverableResource deliverableResource = new LogicDeliverableResource();

				deliverableResource.SetResourceData(data);
				deliverableResource.SetResourceAmount(count);

				m_deliverables.Add(deliverableResource);
			}
		}

		public void AddDeliverable(LogicDeliverable deliverable)
		{
			m_deliverables.Add(deliverable);
		}

		public int GetDeliverableCount()
			=> m_deliverables.Size();

		public LogicDeliverable GetDeliverable(int index)
			=> m_deliverables[index];
	}
}