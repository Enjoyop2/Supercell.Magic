using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableDecoration : LogicDeliverable
	{
		private LogicDecoData m_decoData;

		public override void Destruct()
		{
			base.Destruct();
			m_decoData = null;
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);
			LogicJSONHelper.SetLogicData(jsonObject, "decoration", m_decoData);
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);
			m_decoData = (LogicDecoData)LogicJSONHelper.GetLogicData(jsonObject, "decoration");
		}

		public override int GetDeliverableType()
			=> 3;

		public override bool Deliver(LogicLevel level)
		{
			if (CanBeDeliver(level))
			{
				level.AddUnplacedObject(new LogicDataSlot(m_decoData, 0));
				return true;
			}

			return false;
		}

		public override bool CanBeDeliver(LogicLevel level)
			=> level.GetObjectCount(m_decoData, m_decoData.GetVillageType()) < m_decoData.GetMaxCount();

		public override LogicDeliverableBundle Compensate(LogicLevel level)
		{
			LogicDeliverableBundle logicDeliverableBundle = new LogicDeliverableBundle();
			logicDeliverableBundle.AddResources(m_decoData.GetBuildResource(), m_decoData.GetBuildCost());
			return logicDeliverableBundle;
		}

		public LogicDecoData GetDecorationData()
			=> m_decoData;

		public void SetDecorationData(LogicDecoData data)
		{
			m_decoData = data;
		}
	}
}