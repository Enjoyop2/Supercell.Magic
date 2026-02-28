using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicVillageObject : LogicGameObject
	{
		private LogicTimer m_constructionTimer;

		private bool m_locked;
		private bool m_upgrading;
		private int m_upgLevel;

		public LogicVillageObject(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			// LogicVillageObject.
		}

		public LogicVillageObjectData GetVillageObjectData()
			=> (LogicVillageObjectData)m_data;

		public bool CanUpgrade(bool callListener)
		{
			if (!m_locked)
			{
				LogicVillageObjectData villageObjectData = GetVillageObjectData();

				if (m_upgLevel < villageObjectData.GetUpgradeLevelCount() - 1)
				{
					if (m_level.GetTownHallLevel(m_level.GetVillageType()) >= GetRequiredTownHallLevelForUpgrade())
					{
						return true;
					}

					if (callListener)
					{
						m_level.GetGameListener().TownHallLevelTooLow(GetRequiredTownHallLevelForUpgrade());
					}

					return false;
				}
			}

			return false;
		}

		public void StartUpgrading(bool ignoreListener)
		{
			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			LogicVillageObjectData data = GetVillageObjectData();
			int constructionTime = data.GetBuildTime(GetUpgradeLevel() + 1);

			m_upgrading = true;

			if (constructionTime <= 0)
			{
				FinishConstruction(false, true);
			}
			else
			{
				if (data.IsRequiresBuilder())
				{
					m_level.GetWorkerManagerAt(data.GetVillageType()).AllocateWorker(this);
				}

				EnableComponent(LogicComponentType.RESOURCE_PRODUCTION, false);
				EnableComponent(LogicComponentType.UNIT_PRODUCTION, false);
				EnableComponent(LogicComponentType.UNIT_UPGRADE, false);

				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(constructionTime, m_level.GetLogicTime(), true, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
			}
		}

		public void FinishConstruction(bool ignoreState, bool ignoreListener)
		{
			int state = m_level.GetState();

			if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
			{
				if (m_level.GetHomeOwnerAvatar() != null &&
					m_level.GetHomeOwnerAvatar().IsClientAvatar())
				{
					if (m_constructionTimer != null)
					{
						m_constructionTimer.Destruct();
						m_constructionTimer = null;
					}

					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();
					LogicVillageObjectData data = GetVillageObjectData();

					if (data.IsRequiresBuilder())
					{
						m_level.GetWorkerManagerAt(data.GetVillageType()).DeallocateWorker(this);
					}

					m_locked = false;

					if (m_upgLevel != 0 || m_upgrading)
					{
						int nextUpgLevel = m_upgLevel + 1;

						if (m_upgLevel >= data.GetUpgradeLevelCount() - 1)
						{
							Debugger.Warning("LogicVillageObject - Trying to upgrade to level that doesn't exist! - " + data.GetName());
							nextUpgLevel = data.GetUpgradeLevelCount() - 1;
						}

						int constructionTime = data.GetBuildTime(nextUpgLevel);
						int xpGain = LogicMath.Sqrt(constructionTime);

						m_upgLevel = nextUpgLevel;

						XpGainHelper(xpGain, homeOwnerAvatar, ignoreState);
					}
					else
					{
						int constructionTime = data.GetBuildTime(0);
						int xpGain = LogicMath.Sqrt(constructionTime);

						XpGainHelper(xpGain, homeOwnerAvatar, ignoreState);
					}

					m_upgrading = false;

					if (m_listener != null)
					{
						m_listener.RefreshState();
					}

					if (state == 1)
					{
						m_level.GetAchievementManager().RefreshStatus();
					}
				}
				else
				{
					Debugger.Error("LogicVillageObject::finishCostruction failed - Avatar is null or not client avatar");
				}
			}
		}

		public bool SpeedUpCostruction()
		{
			if (m_constructionTimer != null)
			{
				LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();
				int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()), 0, m_villageType);

				if (playerAvatar.HasEnoughDiamonds(speedUpCost, true, m_level))
				{
					playerAvatar.UseDiamonds(speedUpCost);
					playerAvatar.GetChangeListener().DiamondPurchaseMade(0, m_data.GetGlobalID(), m_upgLevel + (m_upgrading ? 2 : 1), speedUpCost,
																		 m_level.GetVillageType());

					FinishConstruction(false, true);

					return true;
				}
			}

			return false;
		}

		public int GetRequiredTownHallLevelForUpgrade()
			=> GetVillageObjectData().GetRequiredTownHallLevel(LogicMath.Min(m_upgLevel + 1, GetVillageObjectData().GetUpgradeLevelCount() - 1));

		public int GetRemainingConstructionTime()
		{
			if (m_constructionTimer != null)
			{
				return m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}
		}

		public override void FastForwardTime(int secs)
		{
			if (m_constructionTimer != null)
			{
				if (m_constructionTimer.GetEndTimestamp() == -1)
				{
					int remainingTime = m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());

					if (remainingTime > secs)
					{
						base.FastForwardTime(secs);
						m_constructionTimer.StartTimer(remainingTime - secs, m_level.GetLogicTime(), false, -1);
					}
					else
					{
						if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
						{
							base.FastForwardTime(secs);
							m_constructionTimer.StartTimer(0, m_level.GetLogicTime(), false, -1);
						}
						else
						{
							base.FastForwardTime(remainingTime);
							FinishConstruction(true, true);
							base.FastForwardTime(secs - remainingTime);
						}

						return;
					}
				}
				else
				{
					m_constructionTimer.AdjustEndSubtick(m_level);

					if (m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()) == 0)
					{
						if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
						{
							base.FastForwardTime(secs);
							m_constructionTimer.StartTimer(0, m_level.GetLogicTime(), false, -1);
						}
						else
						{
							base.FastForwardTime(0);
							FinishConstruction(true, true);
							base.FastForwardTime(secs);
						}

						return;
					}

					base.FastForwardTime(secs);
				}

				int maxClockTowerFastForward = m_level.GetUpdatedClockTowerBoostTime();

				if (maxClockTowerFastForward > 0 && !m_level.IsClockTowerBoostPaused())
				{
					if (m_data.GetVillageType() == 1)
					{
						m_constructionTimer.SetFastForward(m_constructionTimer.GetFastForward() +
																60 * LogicMath.Min(secs, maxClockTowerFastForward) *
																(LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1));
					}
				}
			}
			else
			{
				base.FastForwardTime(secs);
			}
		}

		public override void SubTick()
		{
			LogicCombatComponent combatComponent = GetCombatComponent(false);

			if (combatComponent != null)
			{
				combatComponent.SubTick();
			}

			if (m_constructionTimer != null)
			{
				if (m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
				{
					FinishConstruction(false, true);
				}
			}
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			if (m_upgLevel != 0 || m_constructionTimer == null || m_upgrading)
			{
				jsonObject.Put("lvl", new LogicJSONNumber(m_upgLevel));
			}
			else
			{
				jsonObject.Put("lvl", new LogicJSONNumber(-1));
			}

			if (m_constructionTimer != null)
			{
				jsonObject.Put("const_t", new LogicJSONNumber(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime())));

				if (m_constructionTimer.GetEndTimestamp() != -1)
				{
					jsonObject.Put("const_t_end", new LogicJSONNumber(m_constructionTimer.GetEndTimestamp()));
				}

				if (m_constructionTimer.GetFastForward() != -1)
				{
					jsonObject.Put("con_ff", new LogicJSONNumber(m_constructionTimer.GetFastForward()));
				}
			}

			if (m_locked)
			{
				jsonObject.Put("locked", new LogicJSONBoolean(true));
			}

			base.Save(jsonObject, villageType);
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			jsonObject.Put("x", new LogicJSONNumber(GetTileX() & 63));
			jsonObject.Put("y", new LogicJSONNumber(GetTileY() & 63));

			if (m_upgLevel != 0 || m_constructionTimer == null || m_upgrading)
			{
				jsonObject.Put("lvl", new LogicJSONNumber(m_upgLevel));
			}
			else
			{
				jsonObject.Put("lvl", new LogicJSONNumber(-1));
			}

			if (m_constructionTimer != null)
			{
				jsonObject.Put("const_t", new LogicJSONNumber(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			base.SaveToSnapshot(jsonObject, layoutId);
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LoadUpgradeLevel(jsonObject);

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			LogicJSONNumber constTimeObject = jsonObject.GetJSONNumber("const_t");

			if (constTimeObject != null)
			{
				int constTime = constTimeObject.GetIntValue();

				if (!LogicDataTables.GetGlobals().ClampBuildingTimes())
				{
					if (m_upgLevel < GetVillageObjectData().GetUpgradeLevelCount() - 1)
					{
						constTime = LogicMath.Min(constTime, GetVillageObjectData().GetBuildTime(m_upgLevel + 1));
					}
				}

				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(constTime, m_level.GetLogicTime(), false, -1);

				LogicJSONNumber constTimeEndObject = jsonObject.GetJSONNumber("const_t_end");

				if (constTimeEndObject != null)
				{
					m_constructionTimer.SetEndTimestamp(constTimeEndObject.GetIntValue());
				}

				LogicJSONNumber conffObject = jsonObject.GetJSONNumber("con_ff");

				if (conffObject != null)
				{
					m_constructionTimer.SetFastForward(conffObject.GetIntValue());
				}

				LogicVillageObjectData villageObjectData = GetVillageObjectData();

				if (villageObjectData.IsRequiresBuilder() && !villageObjectData.IsAutomaticUpgrades())
				{
					m_level.GetWorkerManagerAt(m_villageType).AllocateWorker(this);
				}

				m_upgrading = m_upgLevel != -1;
			}

			m_upgLevel = LogicMath.Clamp(m_upgLevel, 0, GetVillageObjectData().GetUpgradeLevelCount() - 1);

			base.Load(jsonObject);

			SetPositionXY((GetVillageObjectData().GetTileX100() << 9) / 100,
							   (GetVillageObjectData().GetTileY100() << 9) / 100);
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			LoadUpgradeLevel(jsonObject);
			base.LoadFromSnapshot(jsonObject);

			SetPositionXY((GetVillageObjectData().GetTileX100() << 9) / 100,
							   (GetVillageObjectData().GetTileY100() << 9) / 100);
		}

		public void LoadUpgradeLevel(LogicJSONObject jsonObject)
		{
			LogicJSONNumber lvlObject = jsonObject.GetJSONNumber("lvl");

			if (lvlObject != null)
			{
				m_upgLevel = lvlObject.GetIntValue();
				int maxLvl = GetVillageObjectData().GetUpgradeLevelCount();

				if (m_upgLevel >= maxLvl)
				{
					Debugger.Warning(string.Format("LogicVillageObject::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}",
												   lvlObject.GetIntValue(),
												   maxLvl,
												   m_globalId,
												   m_data.GetGlobalID()));
					m_upgLevel = maxLvl - 1;
				}
				else
				{
					if (m_upgLevel < -1)
					{
						Debugger.Error("LogicVillageObject::load() - Loaded an illegal upgrade level!");
					}
				}
			}
			else
			{
				Debugger.Error("LogicVillageObject::load - Upgrade level was not found!");
			}

			if (GetVillageObjectData().IsRequiresBuilder())
			{
				m_level.GetWorkerManagerAt(m_villageType).DeallocateWorker(this);
			}

			LogicJSONBoolean lockedObject = jsonObject.GetJSONBoolean("locked");

			if (lockedObject != null)
			{
				m_locked = lockedObject.IsTrue();
			}
		}

		public override void LoadingFinished()
		{
			base.LoadingFinished();

			if (m_listener != null)
			{
				m_listener.LoadedFromJSON();
			}
		}

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.VILLAGE_OBJECT;

		public override int GetWidthInTiles()
			=> 0;

		public override int GetHeightInTiles()
			=> 0;

		public int GetUpgradeLevel()
			=> m_upgLevel;

		public void SetUpgradeLevel(int upgLevel)
		{
			m_upgLevel = upgLevel;
		}

		public bool IsConstructing()
			=> m_constructionTimer != null;
	}
}