using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Worker
{
	public class LogicWorkerManager
	{
		private LogicLevel m_level;
		private LogicArrayList<LogicGameObject> m_constructions;

		private int m_workerCount;

		public LogicWorkerManager(LogicLevel level)
		{
			m_level = level;
			m_constructions = new LogicArrayList<LogicGameObject>();
		}

		public void Destruct()
		{
			if (m_constructions != null)
			{
				m_constructions.Destruct();
				m_constructions = null;
			}

			m_level = null;
			m_workerCount = 0;
		}

		public int GetFreeWorkers()
			=> m_workerCount - m_constructions.Size();

		public int GetTotalWorkers()
			=> m_workerCount;

		public void AllocateWorker(LogicGameObject gameObject)
		{
			if (m_constructions.IndexOf(gameObject) != -1)
			{
				Debugger.Warning("LogicWorkerManager::allocateWorker called twice for same target!");
				return;
			}

			m_constructions.Add(gameObject);
		}

		public void DeallocateWorker(LogicGameObject gameObject)
		{
			int index = m_constructions.IndexOf(gameObject);

			if (index != -1)
			{
				m_constructions.Remove(index);
			}
		}

		public void RemoveGameObjectReference(LogicGameObject gameObject)
		{
			DeallocateWorker(gameObject);
		}

		public void IncreaseWorkerCount()
		{
			++m_workerCount;
		}

		public void DecreaseWorkerCount()
		{
			if (m_workerCount-- <= 0)
			{
				Debugger.Error("LogicWorkerManager - Total worker count below 0");
			}
		}

		public LogicGameObject GetShortestTaskGO()
		{
			LogicGameObject gameObject = null;

			for (int i = 0, minRemaining = -1, tmpRemaining = 0; i < m_constructions.Size(); i++, tmpRemaining = 0)
			{
				LogicGameObject tmp = m_constructions[i];

				switch (m_constructions[i].GetGameObjectType())
				{
					case LogicGameObjectType.BUILDING:
						LogicBuilding building = (LogicBuilding)tmp;

						if (building.IsConstructing())
						{
							tmpRemaining = building.GetRemainingConstructionTime();
						}
						else
						{
							LogicHeroBaseComponent heroBaseComponent = building.GetHeroBaseComponent();

							if (heroBaseComponent == null)
							{
								Debugger.Warning("LogicWorkerManager - Worker allocated to building with remaining construction time 0");
							}
							else
							{
								if (heroBaseComponent.IsUpgrading())
								{
									tmpRemaining = heroBaseComponent.GetRemainingUpgradeSeconds();
								}
								else
								{
									Debugger.Warning("LogicWorkerManager - Worker allocated to altar/herobase without hero upgrading");
								}
							}
						}

						break;
					case LogicGameObjectType.OBSTACLE:
						LogicObstacle obstacle = (LogicObstacle)tmp;

						if (obstacle.IsClearingOnGoing())
						{
							tmpRemaining = obstacle.GetRemainingClearingTime();
						}
						else
						{
							Debugger.Warning("LogicWorkerManager - Worker allocated to obstacle with remaining clearing time 0");
						}

						break;
					case LogicGameObjectType.TRAP:
						LogicTrap trap = (LogicTrap)tmp;

						if (trap.IsConstructing())
						{
							tmpRemaining = trap.GetRemainingConstructionTime();
						}
						else
						{
							Debugger.Warning("LogicWorkerManager - Worker allocated to trap with remaining construction time 0");
						}

						break;
					case LogicGameObjectType.VILLAGE_OBJECT:
						LogicVillageObject villageObject = (LogicVillageObject)tmp;

						if (villageObject.IsConstructing())
						{
							tmpRemaining = villageObject.GetRemainingConstructionTime();
						}
						else
						{
							Debugger.Error("LogicWorkerManager - Worker allocated to building with remaining construction time 0 (vilobj)");
						}

						break;
				}

				if (gameObject == null || minRemaining > tmpRemaining)
				{
					gameObject = tmp;
					minRemaining = tmpRemaining;
				}
			}

			return gameObject;
		}

		public bool FinishTaskOfOneWorker()
		{
			LogicGameObject gameObject = GetShortestTaskGO();

			if (gameObject != null)
			{
				switch (gameObject.GetGameObjectType())
				{
					case LogicGameObjectType.BUILDING:
						LogicBuilding building = (LogicBuilding)gameObject;

						if (building.IsConstructing())
						{
							return building.SpeedUpConstruction();
						}

						if (building.GetHeroBaseComponent() != null)
						{
							return building.GetHeroBaseComponent().SpeedUp();
						}

						break;
					case LogicGameObjectType.OBSTACLE:
						LogicObstacle obstacle = (LogicObstacle)gameObject;

						if (obstacle.IsClearingOnGoing())
						{
							return obstacle.SpeedUpClearing();
						}

						break;
					case LogicGameObjectType.TRAP:
						LogicTrap trap = (LogicTrap)gameObject;

						if (trap.IsConstructing())
						{
							return trap.SpeedUpConstruction();
						}

						break;
					case LogicGameObjectType.VILLAGE_OBJECT:
						LogicVillageObject villageObject = (LogicVillageObject)gameObject;

						if (villageObject.IsConstructing())
						{
							return villageObject.SpeedUpCostruction();
						}

						break;
				}
			}

			return false;
		}
	}
}