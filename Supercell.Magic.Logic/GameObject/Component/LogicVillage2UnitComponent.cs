using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Listener;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicVillage2UnitComponent : LogicComponent
	{
		private LogicDataSlot m_unit;
		private LogicTimer m_productionTimer;

		private bool m_noBarrack;
		private int m_productionType;

		public LogicVillage2UnitComponent(LogicGameObject gameObject) : base(gameObject)
		{
		}

		public void TrainUnit(LogicCombatItemData combatItemData)
		{
			if (m_unit != null)
			{
				m_unit.Destruct();
				m_unit = null;
			}

			if (m_productionTimer != null)
			{
				m_productionTimer.Destruct();
				m_productionTimer = null;
			}

			m_unit = new LogicDataSlot(combatItemData, 0);
			m_productionTimer = new LogicTimer();
			m_productionTimer.StartTimer(GetTrainingTime(combatItemData), m_parent.GetLevel().GetLogicTime(), false, -1);
		}

		public void RemoveUnits()
		{
			if (m_unit != null)
			{
				m_unit.Destruct();
				m_unit = null;
			}

			if (m_productionTimer != null)
			{
				m_productionTimer.Destruct();
				m_productionTimer = null;
			}
		}

		public void SetUnit(LogicCombatItemData combatItemData, int count)
		{
			if (m_unit != null)
			{
				m_unit.Destruct();
				m_unit = null;
			}

			m_unit = new LogicDataSlot(combatItemData, count);
		}

		public int GetTrainingTime(LogicCombatItemData data)
			=> data.GetTrainingTime(m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(data), m_parent.GetLevel(), 0);

		public int GetTotalSecs()
		{
			if (m_unit != null)
			{
				return GetTrainingTime((LogicCombatItemData)m_unit.GetData());
			}

			return 0;
		}

		public int GetMaxUnitsInCamp(LogicCharacterData data)
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null)
			{
				return data.GetUnitsInCamp(homeOwnerAvatar.GetUnitUpgradeLevel(data));
			}

			Debugger.Error("AVATAR = NULL");

			return 0;
		}

		public bool IsEmpty()
		{
			if (m_unit != null)
			{
				return m_unit.GetCount() <= 0;
			}

			return true;
		}

		public int GetRemainingSecs()
		{
			if (m_productionTimer != null)
			{
				int remainingSecs = m_productionTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
				int trainingTime = m_unit != null ? GetTrainingTime((LogicCombatItemData)m_unit.GetData()) : 0;

				return LogicMath.Min(remainingSecs, trainingTime);
			}

			return 0;
		}

		public void ProductionCompleted()
		{
			m_unit.SetCount(GetMaxUnitsInCamp((LogicCharacterData)m_unit.GetData()));

			if (m_productionTimer != null)
			{
				m_productionTimer.Destruct();
				m_productionTimer = null;
			}

			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			LogicCombatItemData combatItemData = (LogicCombatItemData)m_unit.GetData();

			int unitCount = m_parent.GetLevel().GetMissionManager().IsVillage2TutorialOpen()
				? m_unit.GetCount() - homeOwnerAvatar.GetUnitCountVillage2(combatItemData)
				: m_unit.GetCount();

			homeOwnerAvatar.CommodityCountChangeHelper(7, m_unit.GetData(), unitCount);

			if (m_parent.GetLevel().GetGameListener() != null)
			{
				// ?
			}

			int state = m_parent.GetLevel().GetState();

			if (state == 1)
			{
				if (m_parent.GetListener() != null)
				{
					// ?
				}
			}
		}

		public void RefreshArmyCampSize(bool unk)
		{
			if (m_unit != null && m_productionTimer == null)
			{
				int maxUnits = GetMaxUnitsInCamp((LogicCharacterData)m_unit.GetData());
				int unitCount = m_unit.GetCount();

				if (maxUnits > unitCount)
				{
					LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
					homeOwnerAvatar.CommodityCountChangeHelper(7, m_unit.GetData(), maxUnits - unitCount);
					m_unit.SetCount(maxUnits);

					if (m_parent.GetLevel().GetState() == 1)
					{
						LogicGameObjectListener listener = m_parent.GetListener();

						if (listener != null)
						{
							for (int i = unitCount; i < maxUnits; i++)
							{
								if (i > 25)
								{
									return;
								}

								// TODO: Implement listener.
							}
						}
					}
				}
			}
		}

		public int GetRemainingMS()
		{
			if (m_productionTimer != null)
			{
				return m_productionTimer.GetRemainingMS(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public LogicCharacterData GetUnitData()
		{
			if (m_unit != null)
			{
				return (LogicCharacterData)m_unit.GetData();
			}

			return null;
		}

		public LogicCharacterData GetCurrentlyTrainedUnit()
		{
			if (m_unit != null && m_unit.GetCount() == 0)
			{
				return (LogicCharacterData)m_unit.GetData();
			}

			return null;
		}

		public int GetUnitCount()
		{
			if (m_unit != null)
			{
				return m_unit.GetCount();
			}

			return 0;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.VILLAGE2_UNIT;

		public override void FastForwardTime(int secs)
		{
			LogicArrayList<LogicComponent> components = m_parent.GetComponentManager().GetComponents(LogicComponentType.VILLAGE2_UNIT);

			int barrackCount = m_parent.GetGameObjectManager().GetBarrackCount();
			int barrackFoundCount = 0;

			for (int i = 0; i < barrackCount; i++)
			{
				LogicBuilding building = (LogicBuilding)m_parent.GetGameObjectManager().GetBarrack(i);

				if (building != null && !building.IsConstructing())
				{
					barrackFoundCount += 1;
				}
			}

			if (components.Size() <= 0 || barrackFoundCount != 0 && components[0] == this)
			{
				LogicLevel level = m_parent.GetLevel();

				int clockTowerBoostTime = level.GetUpdatedClockTowerBoostTime();
				int boostTime = 0;

				if (clockTowerBoostTime > 0 &&
					!level.IsClockTowerBoostPaused())
				{
					LogicGameObjectData data = m_parent.GetData();

					if (data.GetDataType() == DataType.BUILDING)
					{
						if (data.GetVillageType() == 1)
						{
							boostTime = LogicMath.Min(secs, clockTowerBoostTime) * (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1);
						}
					}
				}

				int remainingSecs = secs + boostTime;

				for (int i = 0; i < components.Size(); i++)
				{
					remainingSecs = LogicMath.Max(0, remainingSecs - ((LogicVillage2UnitComponent)components[i]).FastForwardProduction(remainingSecs));
				}
			}
		}

		public int FastForwardProduction(int secs)
		{
			if (secs > 0)
			{
				if (m_unit != null && m_unit.GetCount() == 0)
				{
					if (m_productionTimer == null)
					{
						m_productionTimer = new LogicTimer();
						m_productionTimer.StartTimer(GetTrainingTime((LogicCharacterData)m_unit.GetData()), m_parent.GetLevel().GetLogicTime(), false, -1);
					}

					int remainingSecs = m_productionTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

					if (remainingSecs - secs <= 0)
					{
						ProductionCompleted();
						return remainingSecs;
					}

					m_productionTimer.StartTimer(remainingSecs - secs, m_parent.GetLevel().GetLogicTime(), false, -1);
					return secs;
				}
			}

			return 0;
		}

		public override void Tick()
		{
			LogicArrayList<LogicComponent> components = m_parent.GetComponentManager().GetComponents(LogicComponentType.VILLAGE2_UNIT);

			int barrackCount = m_parent.GetGameObjectManager().GetBarrackCount();
			int barrackFoundCount = 0;

			for (int i = 0; i < barrackCount; i++)
			{
				LogicBuilding building = (LogicBuilding)m_parent.GetGameObjectManager().GetBarrack(i);

				if (building != null && !building.IsConstructing())
				{
					barrackFoundCount += 1;
				}
			}

			m_noBarrack = barrackFoundCount == 0;

			for (int i = 0; i < components.Size(); i++)
			{
				LogicVillage2UnitComponent component = (LogicVillage2UnitComponent)components[i];

				if (barrackFoundCount != 0)
				{
					if (this == component)
					{
						break;
					}
				}

				if (component != null && component.m_unit != null)
				{
					if (component.m_unit.GetData() != null && component.m_unit.GetCount() == 0)
					{
						if (component.GetRemainingSecs() > 0)
						{
							if (m_productionTimer != null)
							{
								m_productionTimer.StartTimer(m_productionTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()),
																  m_parent.GetLevel().GetLogicTime(), false, -1);
							}

							return;
						}
					}
				}
			}

			if (m_productionTimer != null)
			{
				if (m_parent.GetLevel().GetRemainingClockTowerBoostTime() > 0)
				{
					if (m_parent.GetData().GetDataType() == DataType.BUILDING && m_parent.GetData().GetVillageType() == 1)
					{
						m_productionTimer.FastForwardSubticks(4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
					}
				}
			}

			// TODO: Implement listener.

			if (m_productionTimer != null && m_productionTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) == 0)
			{
				if (m_unit != null)
				{
					ProductionCompleted();
				}
			}
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject unitProductionObject = jsonObject.GetJSONObject("up2");

			if (unitProductionObject != null)
			{
				LogicJSONNumber timerObject = unitProductionObject.GetJSONNumber("t");

				if (timerObject != null)
				{
					int time = timerObject.GetIntValue();

					if (m_productionTimer != null)
					{
						m_productionTimer.Destruct();
						m_productionTimer = null;
					}

					m_productionTimer = new LogicTimer();
					m_productionTimer.StartTimer(time, m_parent.GetLevel().GetLogicTime(), false, -1);
				}

				LogicJSONArray unitArray = unitProductionObject.GetJSONArray("unit");

				if (unitArray != null)
				{
					LogicJSONNumber dataObject = unitArray.GetJSONNumber(0);
					LogicJSONNumber cntObject = unitArray.GetJSONNumber(1);

					if (dataObject != null)
					{
						if (cntObject != null)
						{
							LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue(),
																		 m_productionType != 0 ? DataType.SPELL : DataType.CHARACTER);

							if (data == null)
							{
								Debugger.Error("LogicVillage2UnitComponent::load - Character data is NULL!");
							}

							m_unit = new LogicDataSlot(data, cntObject.GetIntValue());
						}
					}
				}
			}
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			LogicJSONObject unitProductionObject = new LogicJSONObject();

			if (m_unit != null)
			{
				LogicJSONArray unitArray = new LogicJSONArray();

				unitArray.Add(new LogicJSONNumber(m_unit.GetData().GetGlobalID()));
				unitArray.Add(new LogicJSONNumber(m_unit.GetCount()));

				unitProductionObject.Put("unit", unitArray);
			}

			if (m_productionTimer != null)
			{
				unitProductionObject.Put("t", new LogicJSONNumber(m_productionTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));
			}

			jsonObject.Put("up2", unitProductionObject);
		}

		public override void LoadingFinished()
		{
			RefreshArmyCampSize(false);
		}
	}
}