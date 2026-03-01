using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public class LogicUnitProductionComponent : LogicComponent
	{
		private LogicTimer m_timer;
		private readonly LogicArrayList<LogicDataSlot> m_slots;

		private bool m_mode;
		private int m_productionType;

		public LogicUnitProductionComponent(LogicGameObject gameObject) : base(gameObject)
		{
			m_slots = new LogicArrayList<LogicDataSlot>();
			SetProductionType(gameObject);
		}

		public void SetProductionType(LogicGameObject gameObject)
		{
			m_productionType = 0;

			if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				m_productionType = ((LogicBuilding)gameObject).GetBuildingData().IsForgesSpells() ? 1 : 0;
			}
		}

		public int GetProductionType()
			=> m_productionType;

		public int GetRemainingSeconds()
		{
			if (m_timer != null)
			{
				return m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public LogicCombatItemData GetCurrentlyTrainedUnit()
		{
			if (m_slots.Size() > 0)
			{
				return (LogicCombatItemData)m_slots[0].GetData();
			}

			return null;
		}

		public bool ContainsUnit(LogicCombatItemData data)
		{
			for (int i = 0; i < m_slots.Size(); i++)
			{
				if (m_slots[i].GetData() == data)
				{
					return true;
				}
			}

			return false;
		}

		public override void Load(LogicJSONObject root)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			for (int i = m_slots.Size() - 1; i >= 0; i--)
			{
				m_slots[i].Destruct();
				m_slots.Remove(i);
			}

			LogicJSONObject jsonObject = root.GetJSONObject("unit_prod");

			if (jsonObject != null)
			{
				LogicJSONNumber modeObject = jsonObject.GetJSONNumber("m");

				if (modeObject != null)
				{
					m_mode = true;
				}

				LogicJSONNumber unitTypeObject = jsonObject.GetJSONNumber("unit_type");

				if (unitTypeObject != null)
				{
					m_productionType = unitTypeObject.GetIntValue();
				}

				LogicJSONArray slotArray = jsonObject.GetJSONArray("slots");

				if (slotArray != null)
				{
					for (int i = 0; i < slotArray.Size(); i++)
					{
						LogicJSONObject slotObject = slotArray.GetJSONObject(i);

						if (slotObject != null)
						{
							LogicJSONNumber idObject = slotObject.GetJSONNumber("id");

							if (idObject != null)
							{
								LogicData data = LogicDataTables.GetDataById(idObject.GetIntValue(),
																			 m_productionType == 0 ? DataType.CHARACTER : DataType.SPELL);

								if (data != null)
								{
									LogicJSONNumber countObject = slotObject.GetJSONNumber("cnt");

									if (countObject != null)
									{
										if (countObject.GetIntValue() > 0)
										{
											m_slots.Add(new LogicDataSlot(data, countObject.GetIntValue()));
										}
									}
								}
							}
						}
					}
				}

				if (m_slots.Size() > 0)
				{
					LogicJSONNumber timeObject = jsonObject.GetJSONNumber("t");

					if (timeObject != null)
					{
						m_timer = new LogicTimer();
						m_timer.StartTimer(timeObject.GetIntValue(), m_parent.GetLevel().GetLogicTime(), false, -1);
					}
					else
					{
						LogicCombatItemData data = (LogicCombatItemData)m_slots[0].GetData();
						LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
						int upgLevel = homeOwnerAvatar != null ? homeOwnerAvatar.GetUnitUpgradeLevel(data) : 0;

						m_timer = new LogicTimer();
						m_timer.StartTimer(data.GetTrainingTime(upgLevel, m_parent.GetLevel(), 0), m_parent.GetLevel().GetLogicTime(), false, -1);
					}
				}
			}
			else
			{
				m_productionType = 0;

				if (m_parent.GetVillageType() == 0)
				{
					Debugger.Warning("LogicUnitProductionComponent::load - Component wasn't found from the JSON");
				}
			}
		}

		public override void Save(LogicJSONObject root, int villageType)
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("m", new LogicJSONNumber(1));
			jsonObject.Put("unit_type", new LogicJSONNumber(m_productionType));

			if (m_timer != null)
			{
				jsonObject.Put("t", new LogicJSONNumber(m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));
			}

			if (m_slots.Size() > 0)
			{
				LogicJSONArray slotArray = new LogicJSONArray();

				for (int i = 0; i < m_slots.Size(); i++)
				{
					LogicDataSlot slot = m_slots[i];
					LogicJSONObject slotObject = new LogicJSONObject();

					slotObject.Put("id", new LogicJSONNumber(slot.GetData().GetGlobalID()));
					slotObject.Put("cnt", new LogicJSONNumber(slot.GetCount()));

					slotArray.Add(slotObject);
				}

				jsonObject.Put("slots", slotArray);
			}

			root.Put("unit_prod", jsonObject);
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.UNIT_PRODUCTION;
	}
}