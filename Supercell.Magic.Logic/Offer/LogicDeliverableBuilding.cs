using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableBuilding : LogicDeliverable
	{
		private LogicBuildingData m_buildingData;

		private int m_buildingLevel;
		private int m_buildingCount;

		public override void Destruct()
		{
			base.Destruct();

			m_buildingData = null;
			m_buildingCount = 0;
			m_buildingLevel = 0;
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);

			LogicJSONHelper.SetLogicData(jsonObject, "building", m_buildingData);

			jsonObject.Put("buildingNumber", new LogicJSONNumber(m_buildingCount));
			jsonObject.Put("buildingLevel", new LogicJSONNumber(m_buildingLevel));
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);

			m_buildingData = (LogicBuildingData)LogicJSONHelper.GetLogicData(jsonObject, "building");
			m_buildingCount = LogicJSONHelper.GetInt(jsonObject, "buildingNumber");
			m_buildingLevel = LogicJSONHelper.GetInt(jsonObject, "buildingLevel");
		}

		public override int GetDeliverableType()
			=> 2;

		public override bool Deliver(LogicLevel level)
		{
			if (CanBeDeliver(level))
			{
				level.AddUnplacedObject(new LogicDataSlot(m_buildingData, m_buildingLevel));
				return true;
			}

			return false;
		}

		public override bool CanBeDeliver(LogicLevel level)
		{
			int placedBuildingCount = level.GetObjectCount(m_buildingData, m_buildingData.GetVillageType());
			int townHallLevel = m_buildingData.GetVillageType() == 1
				? level.GetHomeOwnerAvatar().GetVillage2TownHallLevel()
				: level.GetHomeOwnerAvatar().GetTownHallLevel();
			int unlockedBuildingCount = LogicDataTables.GetTownHallLevel(townHallLevel).GetUnlockedBuildingCount(m_buildingData);

			if (placedBuildingCount >= unlockedBuildingCount || m_buildingCount != 0)
			{
				return m_buildingCount == placedBuildingCount + 1;
			}

			return true;
		}

		public override LogicDeliverableBundle Compensate(LogicLevel level)
		{
			LogicDeliverableBundle logicDeliverableBundle = new LogicDeliverableBundle();

			if (m_buildingData.IsWorkerBuilding())
			{
				logicDeliverableBundle.AddResources(m_buildingData.GetBuildResource(0), LogicDataTables.GetGlobals().GetWorkerCost(level));
			}
			else
			{
				for (int i = 0; i <= m_buildingLevel; i++)
				{
					logicDeliverableBundle.AddResources(m_buildingData.GetBuildResource(i), m_buildingData.GetBuildCost(i, level));
				}
			}

			return logicDeliverableBundle;
		}

		public LogicBuildingData GetBuildingData()
			=> m_buildingData;

		public void SetBuildingData(LogicBuildingData data)
		{
			m_buildingData = data;
		}

		public int GetBuildingLevel()
			=> m_buildingLevel;

		public void SetBuildingLevel(int value)
		{
			m_buildingLevel = value;
		}

		public int GetBuildingCount()
			=> m_buildingCount;

		public void SetBuildingCount(int value)
		{
			m_buildingCount = value;
		}
	}
}