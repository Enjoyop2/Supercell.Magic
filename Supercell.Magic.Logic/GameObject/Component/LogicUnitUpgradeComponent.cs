using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicUnitUpgradeComponent : LogicComponent
	{
		private LogicTimer m_timer;
		private LogicCombatItemData m_unit;
		private int m_unitType;

		public LogicUnitUpgradeComponent(LogicGameObject gameObject) : base(gameObject)
		{
			// LogicUnitUpgradeComponent.
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}
		}

		public override void Tick()
		{
			if (m_timer != null)
			{
				if (m_parent.GetLevel().GetRemainingClockTowerBoostTime() > 0)
				{
					LogicGameObjectData data = m_parent.GetData();

					if (data.GetDataType() == DataType.BUILDING && data.GetVillageType() == 1)
					{
						m_timer.SetFastForward(m_timer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
					}
				}

				if (m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) == 0)
				{
					FinishUpgrading(true);
				}
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.UNIT_UPGRADE;

		public void FinishUpgrading(bool tick)
		{
			if (m_unit != null)
			{
				m_parent.GetLevel().GetHomeOwnerAvatar().CommodityCountChangeHelper(1, m_unit, 1);

				if (m_unit.GetVillageType() == 1 && m_unit.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
				{
					m_parent.GetLevel().GetGameObjectManagerAt(1).RefreshArmyCampSize();
				}

				if (m_parent.GetLevel().GetState() == 1)
				{
					m_parent.GetLevel().GetGameListener()
						.UnitUpgradeCompleted(m_unit, m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(m_unit), tick);
				}

				m_unit = null;
			}
			else
			{
				Debugger.Warning("LogicUnitUpgradeComponent::finishUpgrading called and m_pUnit is NULL");
			}

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}
		}

		public int GetRemainingSeconds()
		{
			if (m_timer != null)
			{
				return m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetRemainingMS()
		{
			if (m_timer != null)
			{
				return m_timer.GetRemainingMS(m_parent.GetLevel().GetLogicTime());
			}

			return 0;
		}

		public int GetTotalSeconds()
		{
			if (m_unit != null)
			{
				return m_unit.GetUpgradeTime(m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(m_unit));
			}

			return 0;
		}

		public override void Save(LogicJSONObject root, int villageType)
		{
			if (m_timer != null && m_unit != null)
			{
				LogicJSONObject jsonObject = new LogicJSONObject();

				jsonObject.Put("unit_type", new LogicJSONNumber(m_unitType));
				jsonObject.Put("t", new LogicJSONNumber(m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));
				jsonObject.Put("id", new LogicJSONNumber(m_unit.GetGlobalID()));

				if (m_timer.GetEndTimestamp() != -1)
				{
					jsonObject.Put("t_end", new LogicJSONNumber(m_timer.GetEndTimestamp()));
				}

				if (m_timer.GetFastForward() > 0)
				{
					jsonObject.Put("t_ff", new LogicJSONNumber(m_timer.GetFastForward()));
				}

				root.Put("unit_upg", jsonObject);
			}
		}

		public override void Load(LogicJSONObject root)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			m_unit = null;

			LogicJSONObject jsonObject = root.GetJSONObject("unit_upg");

			if (jsonObject != null)
			{
				LogicJSONNumber unitTypeObject = jsonObject.GetJSONNumber("unit_type");
				LogicJSONNumber idObject = jsonObject.GetJSONNumber("id");
				LogicJSONNumber timerObject = jsonObject.GetJSONNumber("t");
				LogicJSONNumber timerEndObject = jsonObject.GetJSONNumber("t_end");
				LogicJSONNumber timerFastForwardObject = jsonObject.GetJSONNumber("t_ff");

				m_unitType = unitTypeObject != null ? unitTypeObject.GetIntValue() : 0;

				if (idObject != null)
				{
					if (timerObject != null)
					{
						LogicData data = LogicDataTables.GetDataById(idObject.GetIntValue(), m_unitType == 0 ? DataType.CHARACTER : DataType.SPELL);

						if (data != null)
						{
							m_unit = (LogicCombatItemData)data;

							m_timer = new LogicTimer();
							m_timer.StartTimer(timerObject.GetIntValue(), m_parent.GetLevel().GetLogicTime(), false, -1);

							if (timerEndObject != null)
							{
								m_timer.SetEndTimestamp(timerEndObject.GetIntValue());
							}

							if (timerFastForwardObject != null)
							{
								m_timer.SetFastForward(timerFastForwardObject.GetIntValue());
							}
						}
					}
				}
			}
		}

		public override void LoadingFinished()
		{
			if (m_timer != null)
			{
				int remainingSecs = m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
				int totalSecs = GetTotalSeconds();

				if (LogicDataTables.GetGlobals().ClampUpgradeTimes())
				{
					if (remainingSecs > totalSecs)
					{
						m_timer.StartTimer(totalSecs, m_parent.GetLevel().GetLogicTime(), true,
												m_parent.GetLevel().GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					}
				}
				else
				{
					m_timer.StartTimer(LogicMath.Min(remainingSecs, totalSecs), m_parent.GetLevel().GetLogicTime(), false, -1);
				}
			}
		}

		public override void FastForwardTime(int time)
		{
			if (m_timer != null)
			{
				if (m_timer.GetEndTimestamp() == -1)
				{
					m_timer.StartTimer(m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) - time, m_parent.GetLevel().GetLogicTime(), false, -1);
				}
				else
				{
					m_timer.AdjustEndSubtick(m_parent.GetLevel());
				}

				int clockTowerBoostTime = m_parent.GetLevel().GetUpdatedClockTowerBoostTime();

				if (clockTowerBoostTime > 0 && m_parent.GetLevel().IsClockTowerBoostPaused())
				{
					LogicGameObjectData data = m_parent.GetData();

					if (data.GetDataType() == DataType.BUILDING)
					{
						if (data.GetVillageType() == 1)
						{
							m_timer.SetFastForward(m_timer.GetFastForward() +
														60 * LogicMath.Min(time, clockTowerBoostTime) * (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1));
						}
					}
				}
			}
		}

		public bool SpeedUp()
		{
			if (m_unit != null)
			{
				int remainingSecs = m_timer != null ? m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) : 0;
				int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(remainingSecs, 0, m_parent.GetVillageType());

				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

				if (m_parent.GetLevel().GetHomeOwnerAvatar().IsClientAvatar())
				{
					LogicClientAvatar clientAvatar = (LogicClientAvatar)homeOwnerAvatar;

					if (clientAvatar.HasEnoughDiamonds(speedUpCost, true, m_parent.GetLevel()))
					{
						clientAvatar.UseDiamonds(speedUpCost);
						clientAvatar.GetChangeListener().DiamondPurchaseMade(4, m_unit.GetGlobalID(), clientAvatar.GetUnitUpgradeLevel(m_unit) + 1, speedUpCost,
																			 m_parent.GetLevel().GetVillageType());
						FinishUpgrading(true);

						return true;
					}
				}
			}

			return false;
		}

		public LogicCombatItemData GetCurrentlyUpgradedUnit()
			=> m_unit;

		public void CancelUpgrade()
		{
			if (m_unit != null)
			{
				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
				int upgLevel = homeOwnerAvatar.GetUnitUpgradeLevel(m_unit);

				homeOwnerAvatar.CommodityCountChangeHelper(0, m_unit.GetUpgradeResource(upgLevel), m_unit.GetUpgradeCost(upgLevel));

				m_unit = null;
			}

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}
		}

		public bool CanStartUpgrading(LogicCombatItemData data)
		{
			if (data != null && m_unit == null)
			{
				if (m_parent.GetLevel().GetGameMode().GetCalendar().IsProductionEnabled(data))
				{
					if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_HERO)
					{
						if (m_parent.GetVillageType() == data.GetVillageType())
						{
							int upgLevel = m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(data);

							if (data.GetUpgradeLevelCount() - 1 > upgLevel)
							{
								int maxProductionHouseLevel;

								if (data.GetVillageType() == 1)
								{
									maxProductionHouseLevel = m_parent.GetComponentManager().GetMaxBarrackLevel();
								}
								else
								{
									LogicComponentManager componentManager = m_parent.GetComponentManager();

									if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
									{
										maxProductionHouseLevel = data.GetUnitOfType() != 1 ? componentManager.GetMaxDarkBarrackLevel() : componentManager.GetMaxBarrackLevel();
									}
									else
									{
										maxProductionHouseLevel =
											data.GetUnitOfType() == 1 ? componentManager.GetMaxSpellForgeLevel() : componentManager.GetMaxMiniSpellForgeLevel();
									}
								}

								if (maxProductionHouseLevel >= data.GetRequiredProductionHouseLevel())
								{
									LogicBuilding building = (LogicBuilding)m_parent;

									if (!building.IsLocked())
									{
										return building.GetUpgradeLevel() >= data.GetRequiredLaboratoryLevel(upgLevel + 1);
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		public void StartUpgrading(LogicCombatItemData data)
		{
			if (CanStartUpgrading(data))
			{
				m_unit = data;

				if (m_timer != null)
				{
					m_timer.Destruct();
					m_timer = null;
				}

				m_timer = new LogicTimer();
				m_timer.StartTimer(GetTotalSeconds(), m_parent.GetLevel().GetLogicTime(), true,
										m_parent.GetLevel().GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
				m_unitType = m_unit.GetCombatItemType();
			}
		}
	}
}