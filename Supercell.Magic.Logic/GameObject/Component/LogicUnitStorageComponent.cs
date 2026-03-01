using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public class LogicUnitStorageComponent : LogicComponent
	{
		private int m_storageType;
		private int m_maxCapacity;

		private LogicArrayList<LogicUnitSlot> m_slots;

		public LogicUnitStorageComponent(LogicGameObject gameObject, int capacity) : base(gameObject)
		{
			m_slots = new LogicArrayList<LogicUnitSlot>();
			m_maxCapacity = capacity;
			SetStorageType(gameObject);
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_slots != null)
			{
				for (int i = m_slots.Size() - 1; i >= 0; i--)
				{
					m_slots[i].Destruct();
					m_slots.Remove(i);
				}

				m_slots = null;
			}
		}

		public int GetStorageType()
			=> m_storageType;

		public void SetStorageType(LogicGameObject gameObject)
		{
			m_storageType = 0;

			if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				m_storageType = ((LogicBuilding)gameObject).GetBuildingData().IsForgesSpells() ? 1 : 0;
			}
		}

		public int GetMaxCapacity()
			=> m_maxCapacity;

		public void SetMaxCapacity(int capacity)
		{
			m_maxCapacity = capacity;
		}

		public int GetUnusedCapacity()
			=> LogicMath.Max(m_maxCapacity - GetUsedCapacity(), 0);

		public int GetUsedCapacity()
		{
			int usedCapacity = 0;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitSlot unitSlot = m_slots[i];
				LogicCombatItemData combatItemData = (LogicCombatItemData)unitSlot.GetData();

				usedCapacity += combatItemData.GetHousingSpace() * unitSlot.GetCount();
			}

			return usedCapacity;
		}

		public bool CanAddUnit(LogicCombatItemData data)
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (!homeOwnerAvatar.IsNpcAvatar())
			{
				if (GetComponentType() != 0)
				{
					if (m_storageType == data.GetCombatItemType())
					{
						return m_maxCapacity >= data.GetHousingSpace() + GetUsedCapacity();
					}
				}
				else
				{
					LogicComponentManager componentManager = m_parent.GetComponentManager();

					int totalUsedHousing = componentManager.GetTotalUsedHousing(m_storageType);
					int totalMaxHousing = componentManager.GetTotalMaxHousing(m_storageType);

					if (data.GetCombatItemType() == m_storageType)
					{
						if (GetUsedCapacity() < m_maxCapacity)
						{
							return totalMaxHousing >= totalUsedHousing + data.GetHousingSpace();
						}
					}
				}

				return false;
			}

			return true;
		}

		public void AddUnit(LogicCombatItemData data)
		{
			AddUnitImpl(data, -1);
		}

		public void AddUnitImpl(LogicCombatItemData data, int upgLevel)
		{
			if (data != null)
			{
				if (CanAddUnit(data))
				{
					int index = -1;

					for (int i = 0; i < m_slots.Size(); i++)
					{
						LogicUnitSlot slot = m_slots[i];

						if (slot.GetData() == data && slot.GetLevel() == upgLevel)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						m_slots[index].SetCount(m_slots[index].GetCount() + 1);
					}
					else
					{
						m_slots.Add(new LogicUnitSlot(data, upgLevel, 1));
					}
				}
				else
				{
					Debugger.Warning("LogicUnitStorageComponent::addUnitImpl called and storage is full");
				}
			}
			else
			{
				Debugger.Warning("LogicUnitStorageComponent::addUnitImpl called and storage is full");
			}
		}

		public void RemoveAllUnits()
		{
			for (int i = m_slots.Size() - 1; i >= 0; i--)
			{
				m_slots[i].Destruct();
				m_slots.Remove(i);
			}
		}

		public void RemoveUnits(LogicCombatItemData data, int count)
		{
			RemoveUnitsImpl(data, -1, count);
		}

		public void RemoveUnits(LogicCombatItemData data, int upgLevel, int count)
		{
			RemoveUnitsImpl(data, upgLevel, count);
		}

		private void RemoveUnitsImpl(LogicCombatItemData data, int upgLevel, int count)
		{
			if (data != null)
			{
				int index = -1;

				for (int i = 0; i < m_slots.Size(); i++)
				{
					LogicUnitSlot slot = m_slots[i];

					if (slot.GetData() == data && slot.GetLevel() == upgLevel)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					LogicUnitSlot slot = m_slots[index];

					if (slot.GetCount() - count <= 0)
					{
						m_slots[index].Destruct();
						m_slots.Remove(index);
					}
					else
					{
						slot.SetCount(slot.GetCount() - count);
					}

					Debugger.Print("LogicUnitStorageComponent::removeUnitsImpl remove " + count + " units");
				}
				else
				{
					Debugger.Error("LogicUnitStorageComponent::removeUnitsImpl No units with the given type found");
				}
			}
			else
			{
				Debugger.Error("LogicUnitStorageComponent::removeUnits called with CharacterData NULL");
			}
		}

		public int GetUnitTypeCount()
			=> m_slots.Size();

		public LogicCombatItemData GetUnitType(int idx)
			=> (LogicCombatItemData)m_slots[idx].GetData();

		public int GetUnitCountByData(LogicCombatItemData data)
		{
			int count = 0;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitSlot unitSlot = m_slots[i];

				if (unitSlot.GetData() == data)
				{
					count += unitSlot.GetCount();
				}
			}

			return count;
		}

		public int GetUnitCount(int idx)
			=> m_slots[idx].GetCount();

		public int GetUnitLevel(int idx)
			=> m_slots[idx].GetLevel();

		public int GetUnitTypeIndex(LogicCombatItemData data)
		{
			int index = -1;

			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (m_slots[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			return index;
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			LogicJSONArray unitArray = new LogicJSONArray();

			for (int i = 0; i < m_slots.Size(); i++)
			{
				LogicUnitSlot slot = m_slots[i];

				if (slot.GetData() != null && slot.GetCount() > 0)
				{
					if (slot.GetLevel() != -1)
					{
						Debugger.Error("Invalid unit level.");
					}

					LogicJSONArray unitObject = new LogicJSONArray(2);
					unitObject.Add(new LogicJSONNumber(slot.GetData().GetGlobalID()));
					unitObject.Add(new LogicJSONNumber(slot.GetCount()));
					unitArray.Add(unitObject);
				}
			}

			jsonObject.Put("units", unitArray);
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONArray unitArray = jsonObject.GetJSONArray("units");

			if (unitArray != null)
			{
				if (m_slots.Size() > 0)
				{
					Debugger.Error("LogicUnitStorageComponent::load - Unit array size > 0!");
				}

				for (int i = 0, size = unitArray.Size(); i < size; i++)
				{
					LogicJSONArray unitObject = unitArray.GetJSONArray(i);

					if (unitObject != null)
					{
						LogicJSONNumber dataObject = unitObject.GetJSONNumber(0);
						LogicJSONNumber countObject = unitObject.GetJSONNumber(1);

						if (dataObject != null)
						{
							if (countObject != null)
							{
								LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue(),
																			 m_storageType != 0 ? DataType.SPELL : DataType.CHARACTER);

								if (data == null)
								{
									Debugger.Error("LogicUnitStorageComponent::load - Character data is NULL!");
								}

								m_slots.Add(new LogicUnitSlot(data, -1, countObject.GetIntValue()));
							}
						}
					}
				}
			}
		}

		public bool IsEmpty()
		{
			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (m_slots[i].GetCount() > 0)
				{
					return false;
				}
			}

			return true;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.UNIT_STORAGE;
	}
}