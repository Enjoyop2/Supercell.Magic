using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Unit
{
	public class LogicUnitProduction
	{
		public const int TUTORIAL_MAX_CAPACITY = 20;

		private LogicLevel m_level;
		private LogicTimer m_timer;
		private LogicTimer m_boostTimer;
		private readonly LogicArrayList<LogicUnitProductionSlot> m_slots;

		private readonly int m_villageType;
		private readonly LogicDataType m_unitProductionType;
		private int m_nextProduction;

		private bool m_locked;
		private bool m_boostPause;

		public LogicUnitProduction(LogicLevel level, LogicDataType unitProductionType, int villageType)
		{
			m_level = level;
			m_villageType = villageType;
			m_unitProductionType = unitProductionType;

			m_slots = new LogicArrayList<LogicUnitProductionSlot>();
		}

		public void Destruct()
		{
			for (int i = m_slots.Size() - 1; i >= 0; i--)
			{
				m_slots[i].Destruct();
				m_slots.Remove(i);
			}

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			m_level = null;
		}

		public LogicDataType GetUnitProductionType()
			=> m_unitProductionType;

		public bool IsLocked()
			=> m_locked;

		public void SetLocked(bool state)
		{
			m_locked = state;
		}

		public bool IsBoostPaused()
			=> m_boostPause;

		public void SetBoostPause(bool state)
		{
			m_boostPause = state;
		}

		public int GetRemainingBoostTimeSecs()
		{
			if (m_boostTimer != null)
			{
				return m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetMaxBoostTimeSecs()
		{
			if (m_unitProductionType == LogicDataType.SPELL)
			{
				return LogicDataTables.GetGlobals().GetSpellFactoryBoostSecs();
			}

			if (m_unitProductionType == LogicDataType.CHARACTER)
			{
				return LogicDataTables.GetGlobals().GetBarracksBoostSecs();
			}

			return 0;
		}


		public int GetRemainingSeconds()
		{
			if (m_timer != null)
			{
				return m_timer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetRemainingMS()
		{
			if (m_timer != null)
			{
				return m_timer.GetRemainingMS(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetTotalSeconds()
		{
			LogicUnitProductionSlot slot = null;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitProductionSlot tmp = m_slots[i];

				if (!tmp.IsTerminate())
				{
					slot = tmp;
					break;
				}
			}

			if (slot != null)
			{
				LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				return data.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(data), m_level, 0);
			}

			return 0;
		}

		public int GetTotalRemainingSeconds()
		{
			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
			LogicComponentManager componentManager = m_level.GetComponentManagerAt(m_villageType);

			int totalMaxHousing = componentManager.GetTotalMaxHousing(m_unitProductionType != LogicDataType.CHARACTER ? 1 : 0);
			int totalUsedCapacity = m_unitProductionType == LogicDataType.CHARACTER ? homeOwnerAvatar.GetUnitsTotalCapacity() : homeOwnerAvatar.GetSpellsTotalCapacity();
			int freeCapacity = totalMaxHousing - totalUsedCapacity;
			int remainingSecs = 0;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitProductionSlot slot = m_slots[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();
				int housingSpace = data.GetHousingSpace();
				int count = slot.GetCount();

				if (count > 0)
				{
					if (i == 0)
					{
						if (!slot.IsTerminate() && freeCapacity - housingSpace >= 0)
						{
							if (m_timer != null)
							{
								remainingSecs += m_timer.GetRemainingSeconds(m_level.GetLogicTime());
							}
						}

						freeCapacity -= housingSpace;
						count -= 1;
					}

					for (int j = 0; j < count; j++)
					{
						if (!slot.IsTerminate() && freeCapacity - housingSpace >= 0)
						{
							remainingSecs += data.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(data), m_level, 0);
						}

						freeCapacity -= housingSpace;
					}
				}
			}

			return remainingSecs;
		}

		public bool IsTutorialCapacityFull()
			=> m_level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + GetTotalCount() >= LogicUnitProduction.TUTORIAL_MAX_CAPACITY;

		public int GetMaxTrainCount()
			=> m_level.GetComponentManagerAt(m_villageType).GetTotalMaxHousing(m_unitProductionType != LogicDataType.CHARACTER ? 1 : 0) * 2;

		public int GetTutorialMax()
			=> LogicUnitProduction.TUTORIAL_MAX_CAPACITY;

		public int GetTutorialCount()
			=> m_level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + GetTotalCount();

		public int GetSlotCount()
			=> m_slots.Size();

		public int GetTrainingCount(int idx)
			=> m_slots[idx].GetCount();

		public LogicCombatItemData GetCurrentlyTrainedUnit()
		{
			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (!m_slots[i].IsTerminate())
				{
					return (LogicCombatItemData)m_slots[i].GetData();
				}
			}

			return null;
		}

		public int GetCurrentlyTrainedIndex()
		{
			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (!m_slots[i].IsTerminate())
				{
					return i;
				}
			}

			return -1;
		}

		public int GetWaitingForSpaceUnitCount(LogicCombatItemData data)
		{
			int count = 0;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitProductionSlot slot = m_slots[i];

				if (slot.GetData() == data)
				{
					if (slot.IsTerminate())
					{
						count += slot.GetCount();
					}
				}
			}

			return count;
		}

		public LogicUnitProductionSlot GetCurrentlyTrainedSlot()
		{
			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (!m_slots[i].IsTerminate())
				{
					return m_slots[i];
				}
			}

			return null;
		}

		public LogicCombatItemData GetWaitingForSpaceUnit()
		{
			if (m_slots.Size() > 0)
			{
				LogicUnitProductionSlot slot = m_slots[0];

				if (slot.IsTerminate() || m_timer != null && m_timer.GetRemainingSeconds(m_level.GetLogicTime()) == 0)
				{
					return (LogicCombatItemData)slot.GetData();
				}
			}

			return null;
		}

		public int GetBoostMultiplier()
		{
			if (m_unitProductionType == LogicDataType.SPELL)
			{
				if (LogicDataTables.GetGlobals().UseNewTraining())
				{
					return LogicDataTables.GetGlobals().GetSpellFactoryBoostNewMultiplier();
				}

				return LogicDataTables.GetGlobals().GetSpellFactoryBoostMultiplier();
			}

			if (m_unitProductionType != LogicDataType.CHARACTER)
			{
				return 1;
			}

			if (LogicDataTables.GetGlobals().UseNewTraining())
			{
				return LogicDataTables.GetGlobals().GetBarracksBoostNewMultiplier();
			}

			return LogicDataTables.GetGlobals().GetBarracksBoostMultiplier();
		}

		public bool ProductionCompleted(bool speedUp)
		{
			bool success = false;

			if (!m_locked)
			{
				LogicComponentFilter filter = new LogicComponentFilter();

				filter.SetComponentType(0);

				while (true)
				{
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
					LogicComponentManager componentManager = m_level.GetComponentManagerAt(m_villageType);
					LogicCombatItemData productionData = GetWaitingForSpaceUnit();

					if (speedUp)
					{
						if (m_slots.Size() <= 0)
						{
							return false;
						}

						productionData = (LogicCombatItemData)m_slots[0].GetData();
					}

					if (productionData == null)
					{
						filter.Destruct();
						return false;
					}

					bool productionTerminate = m_slots[0].IsTerminate();
					LogicBuildingData buildingProductionData = productionData.GetProductionHouseData();
					LogicGameObjectManager gameObjectManager = m_level.GetGameObjectManagerAt(m_villageType);
					LogicBuilding productionHouse = gameObjectManager.GetHighestBuilding(buildingProductionData);

					if (LogicDataTables.GetGlobals().UseTroopWalksOutFromTraining())
					{
						int gameObjectCount = gameObjectManager.GetNumGameObjects();

						for (int i = 0; i < gameObjectCount; i++)
						{
							LogicGameObject gameObject = gameObjectManager.GetGameObjectByIndex(i);

							if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
							{
								LogicBuilding building = (LogicBuilding)gameObject;
								LogicUnitProductionComponent unitProductionComponent = building.GetUnitProductionComponent();

								if (unitProductionComponent != null)
								{
									if (unitProductionComponent.GetProductionType() == productionData.GetCombatItemType())
									{
										if (building.GetBuildingData().GetProducesUnitsOfType() == productionData.GetUnitOfType() &&
											!building.IsUpgrading() &&
											!building.IsConstructing())
										{
											if (productionData.IsUnlockedForProductionHouseLevel(building.GetUpgradeLevel()))
											{
												if (productionHouse != null)
												{
													int seed = m_level.GetPlayerAvatar().GetExpPoints();

													if (building.Rand(seed) % 1000 > 750)
													{
														productionHouse = building;
													}
												}
												else
												{
													productionHouse = building;
												}
											}
										}
									}
								}
							}
						}
					}

					if (productionHouse != null)
					{
						LogicUnitStorageComponent unitStorageComponent =
							(LogicUnitStorageComponent)componentManager.GetClosestComponent(productionHouse.GetX(), productionHouse.GetY(), filter);

						if (unitStorageComponent != null)
						{
							if (unitStorageComponent.CanAddUnit(productionData))
							{
								homeOwnerAvatar.CommodityCountChangeHelper(0, productionData, 1);
								unitStorageComponent.AddUnit(productionData);

								if (productionTerminate)
								{
									RemoveUnit(productionData, -1);
								}
								else
								{
									StartProducingNextUnit();
								}

								success = true;

								if (m_slots.Size() > 0 && m_slots[0].IsTerminate() && m_slots[0].GetCount() > 0)
								{
									continue;
								}

								break;
							}

							filter.AddIgnoreObject(unitStorageComponent.GetParent());
						}
						else
						{
							if (m_timer != null && m_timer.GetRemainingSeconds(m_level.GetLogicTime()) == 0)
							{
								success = TrainingFinished();
							}

							break;
						}
					}
					else
					{
						break;
					}
				}

				filter.Destruct();

				if (success)
				{
					m_nextProduction = 0;
				}
				else
				{
					m_nextProduction = 2000;
				}
			}

			return success;
		}

		public void RemoveTrainedUnit(LogicCombatItemData data)
		{
			int idx = -1;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (m_slots[i].GetData() == data)
				{
					if (m_slots[i].IsTerminate())
					{
						idx = i;
						break;
					}
				}
			}

			if (idx != -1)
			{
				LogicUnitProductionSlot slot = m_slots[idx];

				if (slot.GetCount() > 1)
				{
					slot.SetCount(slot.GetCount() - 1);
				}
				else
				{
					m_slots.Remove(idx);

					slot.Destruct();
					slot = null;
				}

				MergeSlots();

				if (GetWaitingForSpaceUnit() != null)
				{
					ProductionCompleted(false);
				}
			}
		}

		public bool RemoveUnit(LogicCombatItemData data, int index)
		{
			LogicUnitProductionSlot slot = null;
			bool removed = false;

			if (index > -1 &&
				m_slots.Size() > index &&
				m_slots[index].GetData() == data)
			{
				slot = m_slots[index];
			}
			else
			{
				index = -1;

				for (int i = 0; i < m_slots.Size(); i++)
				{
					LogicUnitProductionSlot tmp = m_slots[i];

					if (tmp.GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index == -1)
				{
					return false;
				}

				slot = m_slots[index];
			}

			int count = slot.GetCount();

			if (count > 0)
			{
				removed = true;
				slot.SetCount(count - 1);

				if (count == 1)
				{
					int prodIdx = GetCurrentlyTrainedIndex();

					if (prodIdx == index)
					{
						if (m_timer != null)
						{
							m_timer.Destruct();
							m_timer = null;
						}
					}

					m_slots[index].Destruct();
					m_slots.Remove(index);
				}
			}

			if (m_slots.Size() > 0)
			{
				LogicUnitProductionSlot productionSlot = GetCurrentlyTrainedSlot();

				if (productionSlot == null || m_timer != null)
				{
					if (!removed)
					{
						return false;
					}

					MergeSlots();
				}
				else
				{
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
					LogicCombatItemData productionData = (LogicCombatItemData)productionSlot.GetData();

					m_timer = new LogicTimer();
					m_timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), m_level, 0), m_level.GetLogicTime(),
											false,
											-1);

					if (removed)
					{
						MergeSlots();
					}
				}
			}
			else
			{
				if (!removed)
				{
					return false;
				}

				MergeSlots();
			}

			return true;
		}

		public void SpeedUp()
		{
			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
			LogicComponentManager componentManager = m_level.GetComponentManagerAt(m_villageType);

			int totalMaxHousing = componentManager.GetTotalMaxHousing(m_unitProductionType != LogicDataType.CHARACTER ? 1 : 0);
			int totalUsedCapacity = m_unitProductionType == LogicDataType.CHARACTER ? homeOwnerAvatar.GetUnitsTotalCapacity() : homeOwnerAvatar.GetSpellsTotalCapacity();
			int freeCapacity = totalMaxHousing - totalUsedCapacity;

			bool armyCampFull = false;

			while (!armyCampFull && m_slots.Size() > 0)
			{
				LogicUnitProductionSlot slot = m_slots[0];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				int count = slot.GetCount();

				if (count <= 0)
				{
					break;
				}

				armyCampFull = true;

				do
				{
					freeCapacity -= data.GetHousingSpace();

					if (freeCapacity >= 0)
					{
						ProductionCompleted(true);
						armyCampFull = false;
					}
				} while (--count > 0);
			}
		}

		public LogicUnitProductionSlot GetUnit(int index)
		{
			if (index > -1 && m_slots.Size() > index)
			{
				return m_slots[index];
			}

			return null;
		}

		public void MergeSlots()
		{
			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

			if (m_slots.Size() > 0)
			{
				if (m_slots.Size() > 1)
				{
					for (int i = 1; i < m_slots.Size(); i++)
					{
						LogicUnitProductionSlot slot1 = m_slots[i];
						LogicUnitProductionSlot slot2 = m_slots[i - 1];

						if (slot1.GetData() == slot2.GetData())
						{
							if (slot1.IsTerminate() == slot2.IsTerminate())
							{
								m_slots.Remove(i--);

								slot2.SetCount(slot2.GetCount() + slot1.GetCount());
								slot1.Destruct();
								slot1 = null;
							}
						}
					}
				}
			}

			LogicComponentManager componentManager = m_level.GetComponentManagerAt(m_villageType);

			int usedCapacity = m_unitProductionType == LogicDataType.SPELL ? homeOwnerAvatar.GetSpellsTotalCapacity() : homeOwnerAvatar.GetUnitsTotalCapacity();
			int totalCapacity = componentManager.GetTotalMaxHousing(m_unitProductionType != LogicDataType.CHARACTER ? 1 : 0);
			int freeCapacity = totalCapacity - usedCapacity;

			for (int i = 0, j = freeCapacity; i < m_slots.Size(); i++)
			{
				LogicUnitProductionSlot slot = m_slots[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				int count = slot.GetCount();
				int housingSpace = data.GetHousingSpace() * count;

				if (j < housingSpace)
				{
					if (count > 1)
					{
						int maxInProduction = j / data.GetHousingSpace();

						if (maxInProduction > 0)
						{
							int inQueue = count - maxInProduction;

							if (inQueue > 0)
							{
								slot.SetCount(maxInProduction);
								m_slots.Add(i + 1, new LogicUnitProductionSlot(data, inQueue, slot.IsTerminate()));
							}
						}
					}

					break;
				}

				j -= housingSpace;
			}
		}

		public void StartProducingNextUnit()
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			if (m_slots.Size() > 0)
			{
				LogicCombatItemData unitData = GetCurrentlyTrainedUnit();

				if (unitData != null)
				{
					RemoveUnit(unitData, -1);
				}
			}
		}

		public bool TrainingFinished()
		{
			bool success = false;

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			if (m_slots.Size() > 0)
			{
				LogicUnitProductionSlot prodSlot = GetCurrentlyTrainedSlot();
				int prodIdx = GetCurrentlyTrainedIndex();

				if (prodSlot != null)
				{
					if (prodSlot.GetCount() == 1)
					{
						prodSlot.SetTerminate(true);
					}
					else
					{
						prodSlot.SetCount(prodSlot.GetCount() - 1);

						LogicUnitProductionSlot previousSlot = m_slots[LogicMath.Max(prodIdx - 1, 0)];

						if (previousSlot != null &&
							previousSlot.IsTerminate() &&
							previousSlot.GetData().GetGlobalID() == prodSlot.GetData().GetGlobalID())
						{
							previousSlot.SetCount(previousSlot.GetCount() + 1);
						}
						else
						{
							m_slots.Add(prodIdx, new LogicUnitProductionSlot(prodSlot.GetData(), 1, true));
						}
					}
				}

				if (m_slots.Size() > 0)
				{
					LogicCombatItemData nextProductionData = GetCurrentlyTrainedUnit();

					if (nextProductionData != null && m_timer == null)
					{
						m_timer = new LogicTimer();
						m_timer.StartTimer(nextProductionData.GetTrainingTime(m_level.GetHomeOwnerAvatar().GetUnitUpgradeLevel(nextProductionData), m_level, 0),
												m_level.GetLogicTime(), false, -1);
						success = true;
					}
				}
			}

			MergeSlots();

			return success;
		}

		public bool DragSlot(int slotIdx, int dragIdx)
		{
			m_locked = false;

			if (slotIdx > -1 && slotIdx < m_slots.Size())
			{
				LogicCombatItemData productionData = GetCurrentlyTrainedUnit();
				LogicUnitProductionSlot slot = m_slots[slotIdx];

				m_slots.Remove(slotIdx);

				if (slot != null)
				{
					if (slotIdx <= dragIdx)
					{
						dragIdx -= 1;
					}

					if (dragIdx >= 0 && dragIdx <= m_slots.Size())
					{
						m_slots.Add(dragIdx, slot);
						MergeSlots();

						LogicCombatItemData prodData = GetCurrentlyTrainedUnit();
						int prodIdx = GetCurrentlyTrainedIndex();

						if (productionData != prodData && (dragIdx >= prodIdx || prodIdx == slotIdx || prodIdx == dragIdx + 1))
						{
							if (m_timer != null)
							{
								m_timer.Destruct();
								m_timer = null;
							}

							LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

							m_timer = new LogicTimer();
							m_timer.StartTimer(prodData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(prodData), m_level, 0),
													m_level.GetLogicTime(), false, -1);
						}
					}

					return true;
				}
			}

			return false;
		}

		public int AddUnitToQueue(LogicCombatItemData data)
		{
			if (data != null)
			{
				if (CanAddUnitToQueue(data, false))
				{
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

					for (int i = m_slots.Size() - 1; i >= 0; i--)
					{
						LogicUnitProductionSlot tmp = m_slots[i];

						if (tmp != null)
						{
							if (tmp.GetData() == data)
							{
								tmp.SetCount(tmp.GetCount() + 1);
								MergeSlots();

								return i;
							}

							break;
						}
					}

					m_slots.Add(new LogicUnitProductionSlot(data, 1, false));
					MergeSlots();

					if (m_slots.Size() > 0)
					{
						LogicCombatItemData productionData = GetCurrentlyTrainedUnit();

						if (productionData != null && m_timer == null)
						{
							m_timer = new LogicTimer();
							m_timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), m_level, 0),
													m_level.GetLogicTime(), false, -1);
						}
					}
				}
			}
			else
			{
				Debugger.Error("LogicUnitProduction - Trying to add NULL character!");
			}

			return -1;
		}

		public int AddUnitToQueue(LogicCombatItemData data, int index, bool ignoreCapacity)
		{
			if (data != null)
			{
				if (CanAddUnitToQueue(data, ignoreCapacity))
				{
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
					LogicCombatItemData productionData = GetCurrentlyTrainedUnit();

					m_slots.Add(index, new LogicUnitProductionSlot(data, 1, false));
					MergeSlots();

					if (productionData != null)
					{
						if (GetCurrentlyTrainedUnit() == data || GetCurrentlyTrainedIndex() != index)
						{
							return index;
						}
					}
					else
					{
						productionData = GetCurrentlyTrainedUnit();
					}

					if (m_timer != null)
					{
						m_timer.Destruct();
						m_timer = null;
					}

					m_timer = new LogicTimer();
					m_timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), m_level, 0),
											m_level.GetLogicTime(), false, -1);

					return index;
				}
			}
			else
			{
				Debugger.Error("LogicUnitProduction - Trying to add NULL character!");
			}

			return -1;
		}

		public bool CanAddUnitToQueue(LogicCombatItemData data, bool ignoreCapacity)
		{
			if (data != null)
			{
				if (data.GetDataType() == m_unitProductionType)
				{
					LogicGameObjectManager gameObjectManager = m_level.GetGameObjectManagerAt(0);
					LogicBuilding productionHouse = gameObjectManager.GetHighestBuilding(data.GetProductionHouseData());

					if (productionHouse != null)
					{
						if (!data.IsUnlockedForProductionHouseLevel(productionHouse.GetUpgradeLevel()))
						{
							return false;
						}

						if (data.GetUnitOfType() != productionHouse.GetBuildingData().GetProducesUnitsOfType())
						{
							return false;
						}
					}

					if (m_level.GetMissionManager().IsTutorialFinished() ||
						m_level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + GetTotalCount() < LogicUnitProduction.TUTORIAL_MAX_CAPACITY)
					{
						if (ignoreCapacity)
						{
							return true;
						}

						LogicAvatar avatar = m_level.GetHomeOwnerAvatar();
						LogicComponentManager componentManager = m_level.GetComponentManagerAt(m_villageType);
						int totalMaxHousing = componentManager.GetTotalMaxHousing(m_unitProductionType != LogicDataType.CHARACTER ? 1 : 0) * 2;
						int totalUsedCapacity = GetTotalCount() + data.GetHousingSpace() + (m_unitProductionType == LogicDataType.CHARACTER
													? avatar.GetUnitsTotalCapacity()
													: avatar.GetSpellsTotalCapacity());

						return totalMaxHousing >= totalUsedCapacity;
					}
				}
				else
				{
					Debugger.Error("Trying to add wrong unit type to UnitProduction");
				}
			}
			else
			{
				Debugger.Error("Trying to add NULL troop to UnitProduction");
			}

			return false;
		}

		public bool CanBeBoosted()
		{
			if (!m_boostPause && GetBoostMultiplier() > 0)
				return GetBoostCost() > 0;
			return false;
		}

		public int GetBoostCost()
		{
			if (m_unitProductionType == LogicDataType.CHARACTER)
				return m_level.GetGameMode().GetCalendar().GetUnitProductionBoostCost();
			return m_level.GetGameMode().GetCalendar().GetSpellProductionBoostCost();
		}

		public void Boost()
		{
			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			m_boostTimer = new LogicTimer();
			m_boostTimer.StartTimer(GetMaxBoostTimeSecs(), m_level.GetLogicTime(), false, -1);
		}

		public void StopBoost()
		{
			if (m_boostTimer != null && !m_boostPause)
			{
				m_boostPause = true;
			}
		}

		public int GetTotalCount()
		{
			int count = 0;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitProductionSlot slot = m_slots[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				count += data.GetHousingSpace() * slot.GetCount();
			}

			return count;
		}

		public void SubTick()
		{
			if (m_boostTimer != null && m_boostPause)
			{
				m_boostTimer.StartTimer(m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime()), m_level.GetLogicTime(), false, -1);
			}
		}

		public void Tick()
		{
			if (GetRemainingBoostTimeSecs() > 0)
			{
				if (m_timer != null)
				{
					if (!IsBoostPaused())
					{
						m_timer.FastForwardSubticks(4 * GetBoostMultiplier() - 4);
					}
				}
			}

			bool productionCompleted = false;

			if (m_timer != null)
			{
				productionCompleted = m_timer.GetRemainingSeconds(m_level.GetLogicTime()) == 0;
			}

			if (m_nextProduction > 0)
			{
				m_nextProduction = productionCompleted ? 0 : LogicMath.Max(m_nextProduction - 64, 0);
			}

			if (m_boostTimer != null && m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			if ((productionCompleted || GetWaitingForSpaceUnit() != null) && m_nextProduction == 0)
			{
				ProductionCompleted(false);
			}
		}

		public void FastForwardTime(int secs)
		{
			if (m_boostTimer != null && !m_boostPause)
			{
				int remainingSecs = m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime());

				if (remainingSecs <= secs)
				{
					m_boostTimer.Destruct();
					m_boostTimer = null;
				}
				else
				{
					m_boostTimer.StartTimer(remainingSecs - secs, m_level.GetLogicTime(), false, -1);
				}
			}

			if (GetRemainingBoostTimeSecs() > 0)
			{
				if (GetBoostMultiplier() >= 2 && !IsBoostPaused())
				{
					secs = LogicMath.Min(secs, GetRemainingBoostTimeSecs()) * (GetBoostMultiplier() - 1) + secs;
				}

				if (m_timer != null)
				{
					if (!IsBoostPaused())
					{
						m_timer.FastForwardSubticks(4 * GetBoostMultiplier() - 4);
					}
				}
			}

			do
			{
				if (secs <= 0)
				{
					break;
				}

				LogicUnitProductionSlot productionSlot = GetCurrentlyTrainedSlot();

				if (productionSlot == null)
				{
					break;
				}

				if (m_timer == null)
				{
					LogicCombatItemData productionData = (LogicCombatItemData)productionSlot.GetData();

					m_timer = new LogicTimer();
					m_timer.StartTimer(productionData.GetTrainingTime(m_level.GetHomeOwnerAvatar().GetUnitUpgradeLevel(productionData), m_level, 0),
											m_level.GetLogicTime(), false, -1);
				}

				int remainingSecs = m_timer.GetRemainingSeconds(m_level.GetLogicTime());

				if (secs < remainingSecs)
				{
					m_timer.StartTimer(remainingSecs - secs, m_level.GetLogicTime(), false, -1);
					break;
				}

				secs -= remainingSecs;
				m_timer.StartTimer(0, m_level.GetLogicTime(), false, -1);
			} while (ProductionCompleted(false));
		}

		public void LoadingFinished()
		{
			LogicCombatItemData unitData = GetWaitingForSpaceUnit();

			if (unitData != null)
			{
				ProductionCompleted(false);
			}
		}

		public void UnitRemoved()
		{
			LogicCombatItemData unitData = GetWaitingForSpaceUnit();

			if (unitData != null)
			{
				ProductionCompleted(false);
			}
		}

		public void Load(LogicJSONObject root)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			for (int i = m_slots.Size() - 1; i >= 0; i--)
			{
				m_slots[i].Destruct();
				m_slots.Remove(i);
			}

			LogicJSONObject jsonObject = root.GetJSONObject("unit_prod");

			if (jsonObject != null)
			{
				LogicJSONArray slotArray = jsonObject.GetJSONArray("slots");

				if (slotArray != null)
				{
					for (int i = 0; i < slotArray.Size(); i++)
					{
						LogicJSONObject slotObject = slotArray.GetJSONObject(i);

						if (slotObject != null)
						{
							LogicJSONNumber dataObject = slotObject.GetJSONNumber("id");

							if (dataObject != null)
							{
								LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue());

								if (data != null)
								{
									LogicJSONNumber countObject = slotObject.GetJSONNumber("cnt");
									LogicJSONBoolean termineObject = slotObject.GetJSONBoolean("t");

									if (countObject != null)
									{
										if (countObject.GetIntValue() > 0)
										{
											LogicUnitProductionSlot slot = new LogicUnitProductionSlot(data, countObject.GetIntValue(), false);

											if (termineObject != null)
											{
												slot.SetTerminate(termineObject.IsTrue());
											}

											m_slots.Add(slot);
										}
									}
								}
							}
						}
					}
				}

				if (m_slots.Size() > 0)
				{
					LogicUnitProductionSlot slot = GetCurrentlyTrainedSlot();

					if (slot != null)
					{
						LogicJSONNumber timeObject = jsonObject.GetJSONNumber("t");

						if (timeObject != null)
						{
							m_timer = new LogicTimer();
							m_timer.StartTimer(timeObject.GetIntValue(), m_level.GetLogicTime(), false, -1);
						}
						else
						{
							LogicCombatItemData combatItemData = (LogicCombatItemData)slot.GetData();
							LogicAvatar avatar = m_level.GetHomeOwnerAvatar();
							int upgradeLevel = 0;

							if (avatar != null)
							{
								upgradeLevel = avatar.GetUnitUpgradeLevel(combatItemData);
							}

							m_timer = new LogicTimer();
							m_timer.StartTimer(combatItemData.GetTrainingTime(upgradeLevel, m_level, 0), m_level.GetLogicTime(), false, -1);

							Debugger.Print("LogicUnitProduction::load null timer, restart: " + m_timer.GetRemainingSeconds(m_level.GetLogicTime()));
						}
					}
				}

				LogicJSONNumber boostTimeObject = jsonObject.GetJSONNumber("boost_t");

				if (boostTimeObject != null)
				{
					m_boostTimer = new LogicTimer();
					m_boostTimer.StartTimer(boostTimeObject.GetIntValue(), m_level.GetLogicTime(), false, -1);
				}

				LogicJSONBoolean boostPauseObject = jsonObject.GetJSONBoolean("boost_pause");

				if (boostPauseObject != null)
				{
					m_boostPause = boostPauseObject.IsTrue();
				}
			}
			else
			{
				Debugger.Warning("LogicUnitProduction::load - Component wasn't found from the JSON");
			}
		}

		public void Save(LogicJSONObject root)
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			if (m_timer != null)
			{
				jsonObject.Put("t", new LogicJSONNumber(m_timer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			if (m_slots.Size() > 0)
			{
				LogicJSONArray slotArray = new LogicJSONArray();

				for (int i = 0; i < m_slots.Size(); i++)
				{
					LogicUnitProductionSlot slot = m_slots[i];

					if (slot != null)
					{
						LogicJSONObject slotObject = new LogicJSONObject();

						slotObject.Put("id", new LogicJSONNumber(slot.GetData().GetGlobalID()));
						slotObject.Put("cnt", new LogicJSONNumber(slot.GetCount()));

						if (slot.IsTerminate())
						{
							slotObject.Put("t", new LogicJSONBoolean(true));
						}

						slotArray.Add(slotObject);
					}
				}

				jsonObject.Put("slots", slotArray);
			}

			if (m_boostTimer != null)
			{
				jsonObject.Put("boost_t", new LogicJSONNumber(m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			if (m_boostPause)
			{
				jsonObject.Put("boost_pause", new LogicJSONBoolean(true));
			}

			root.Put("unit_prod", jsonObject);
		}
	}
}