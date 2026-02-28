using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Calendar;
using Supercell.Magic.Logic.Command.Battle;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicBuilding : LogicGameObject
	{
		private int m_gear;
		private int m_upgLevel;
		private int m_wallIndex;
		private int m_wallBlockX;
		private int m_direction;
		private int m_selectedWallTime;
		private int m_hitWallDelay;
		private int m_dieTime;

		private bool m_wallPoint;
		private bool m_locked;
		private bool m_hidden;
		private bool m_hasAreaOfEffectSpell;
		private bool m_gearing;
		private bool m_upgrading;
		private bool m_boostPause;

		private LogicTimer m_constructionTimer;
		private LogicTimer m_boostCooldownTimer;
		private LogicTimer m_boostTimer;
		private LogicAttackerItemData m_shareHeroCombatData;
		private LogicSpell m_auraSpell; // 188
		private LogicSpell m_areaOfEffectSpell; // 160
		private LogicGameObjectFilter m_filter;

		public LogicBuilding(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			LogicBuildingData buildingData = GetBuildingData();

			m_locked = buildingData.IsLocked();

			if (buildingData.GetHitpoints(0) > 0)
			{
				LogicHitpointComponent hitpointComponent = new LogicHitpointComponent(this, buildingData.GetHitpoints(0), 1);
				hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(0));
				AddComponent(hitpointComponent);
			}

			LogicAttackerItemData attackerItemData = buildingData.GetAttackerItemData(0);

			if (buildingData.GetHeroData() != null)
			{
				LogicHeroBaseComponent heroBaseComponent = new LogicHeroBaseComponent(this, buildingData.GetHeroData());

				AddComponent(heroBaseComponent);

				if (buildingData.GetShareHeroCombatData())
				{
					m_shareHeroCombatData = buildingData.GetHeroData().GetAttackerItemData(0).Clone();
					m_shareHeroCombatData.AddAttackRange(buildingData.GetWidth() * 72704 / 200);

					heroBaseComponent.SetSharedHeroCombatData(true);
					attackerItemData = m_shareHeroCombatData;
				}
			}

			if (buildingData.IsLaboratory())
			{
				LogicUnitUpgradeComponent unitUpgradeComponent = new LogicUnitUpgradeComponent(this);
				unitUpgradeComponent.SetEnabled(false);
				AddComponent(unitUpgradeComponent);
			}

			if (buildingData.GetVillage2Housing() > 0)
			{
				LogicVillage2UnitComponent village2UnitComponent = new LogicVillage2UnitComponent(this);
				village2UnitComponent.SetEnabled(false);
				AddComponent(village2UnitComponent);
			}

			if (buildingData.GetUnitProduction(0) > 0)
			{
				LogicUnitProductionComponent unitProductionComponent = new LogicUnitProductionComponent(this);
				unitProductionComponent.SetEnabled(false);
				AddComponent(unitProductionComponent);
			}

			if (buildingData.GetUnitStorageCapacity(0) > 0)
			{
				if (buildingData.IsAllianceCastle())
				{
					AddComponent(new LogicBunkerComponent(this, 0));
				}
				else
				{
					AddComponent(new LogicUnitStorageComponent(this, 0));
				}
			}

			if (attackerItemData.GetDamage(0, false) > 0 ||
				attackerItemData.GetDamage2() > 0 ||
				buildingData.GetAreaOfEffectSpell() != null ||
				attackerItemData.GetShockwavePushStrength() > 0 ||
				attackerItemData.GetHitSpell() != null)
			{
				LogicCombatComponent combatComponent = new LogicCombatComponent(this);

				combatComponent.SetAttackValues(attackerItemData, 100);
				combatComponent.SetEnabled(false);

				AddComponent(combatComponent);
			}

			if (buildingData.GetProduceResource() != null)
			{
				LogicResourceProductionComponent resourceProductionComponent = new LogicResourceProductionComponent(this, buildingData.GetProduceResource());
				resourceProductionComponent.SetEnabled(false);
				AddComponent(resourceProductionComponent);
			}

			if (buildingData.StoresResources())
			{
				if (buildingData.IsAllianceCastle())
				{
					AddComponent(new LogicWarResourceStorageComponent(this));
				}
				else
				{
					AddComponent(new LogicResourceStorageComponent(this));
				}
			}

			if (buildingData.GetDefenceTroopCharacter(0) != null)
			{
				AddComponent(new LogicDefenceUnitProductionComponent(this));
			}

			AddComponent(new LogicLayoutComponent(this));

			InitHidden(buildingData);
			InitAoeSpell(buildingData);
		}

		public LogicBuildingData GetBuildingData()
			=> (LogicBuildingData)m_data;

		public void InitHidden(LogicBuildingData data)
		{
			if (data.IsHidden())
			{
				if (m_level.IsInCombatState())
				{
					m_hidden = true;
					m_filter = new LogicGameObjectFilter();
					m_filter.AddGameObjectType(LogicGameObjectType.CHARACTER);
					m_filter.PassEnemyOnly(this);
				}
			}
		}

		public void InitAoeSpell(LogicBuildingData data)
		{
			if (data.GetAreaOfEffectSpell() != null)
			{
				if (m_level.IsInCombatState())
				{
					m_hasAreaOfEffectSpell = true;

					if (m_filter == null)
					{
						m_filter = new LogicGameObjectFilter();
						m_filter.AddGameObjectType(LogicGameObjectType.CHARACTER);
						m_filter.PassEnemyOnly(this);
					}
				}
			}
		}

		public LogicHeroBaseComponent GetHeroBaseComponent()
			=> (LogicHeroBaseComponent)GetComponent(LogicComponentType.HERO_BASE);

		public LogicUnitProductionComponent GetUnitProductionComponent()
			=> (LogicUnitProductionComponent)GetComponent(LogicComponentType.UNIT_PRODUCTION);

		public LogicUnitStorageComponent GetUnitStorageComponent()
			=> (LogicUnitStorageComponent)GetComponent(LogicComponentType.UNIT_STORAGE);

		public LogicResourceStorageComponent GetResourceStorageComponentComponent()
			=> (LogicResourceStorageComponent)GetComponent(LogicComponentType.RESOURCE_STORAGE);

		public LogicUnitUpgradeComponent GetUnitUpgradeComponent()
			=> (LogicUnitUpgradeComponent)GetComponent(LogicComponentType.UNIT_UPGRADE);

		public LogicWarResourceStorageComponent GetWarResourceStorageComponent()
			=> (LogicWarResourceStorageComponent)GetComponent(LogicComponentType.WAR_RESOURCE_STORAGE);

		public LogicVillage2UnitComponent GetVillage2UnitComponent()
			=> (LogicVillage2UnitComponent)GetComponent(LogicComponentType.VILLAGE2_UNIT);

		public int GetRemainingConstructionTime()
		{
			if (m_constructionTimer != null)
			{
				return m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetRemainingConstructionTimeMS()
		{
			if (m_constructionTimer != null)
			{
				return m_constructionTimer.GetRemainingMS(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetRemainingBoostCooldownTime()
		{
			if (m_boostCooldownTimer != null)
			{
				return m_boostCooldownTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetGearLevel()
			=> m_gear;

		public void SetGearLevel(int value)
		{
			m_gear = value;
		}

		public LogicAttackerItemData GetAttackerItemData()
			=> GetBuildingData().GetAttackerItemData(m_upgLevel);

		public override int GetRemainingBoostTime()
		{
			if (m_boostTimer != null)
			{
				return m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public override int GetMaxFastForwardTime()
		{
			if (m_constructionTimer != null && !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
			{
				return LogicMath.Max(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()), 1);
			}

			LogicVillage2UnitComponent village2UnitComponent = (LogicVillage2UnitComponent)GetComponent(LogicComponentType.VILLAGE2_UNIT);

			if (village2UnitComponent != null && village2UnitComponent.GetCurrentlyTrainedUnit() != null)
			{
				int remainingSecs = village2UnitComponent.GetRemainingSecs();

				if (remainingSecs > 0)
				{
					return remainingSecs;
				}

				return -1;
			}

			LogicUnitProductionComponent unitProductionComponent = GetUnitProductionComponent();

			if (unitProductionComponent != null)
			{
				if (GetRemainingBoostTime() <= 0 || GetBoostMultiplier() <= 0 || m_boostPause)
				{
					// TODO: Implement this.
				}
			}

			return -1;
		}

		public int GetMaxBoostTime()
		{
			if (GetComponent(LogicComponentType.RESOURCE_PRODUCTION) != null)
			{
				return LogicDataTables.GetGlobals().GetResourceProductionBoostSecs();
			}

			if (GetComponent(LogicComponentType.UNIT_PRODUCTION) != null)
			{
				if (GetUnitProductionComponent().GetProductionType() == 0)
				{
					return LogicDataTables.GetGlobals().GetBarracksBoostSecs();
				}

				return LogicDataTables.GetGlobals().GetSpellFactoryBoostSecs();
			}

			if (GetComponent(LogicComponentType.HERO_BASE) != null)
			{
				LogicHeroBaseComponent heroBaseComponent = (LogicHeroBaseComponent)GetComponent(LogicComponentType.HERO_BASE);

				if (!heroBaseComponent.IsUpgrading())
				{
					return LogicDataTables.GetGlobals().GetHeroRestBoostSecs();
				}
			}

			if (GetBuildingData().IsClockTower())
			{
				return LogicDataTables.GetGlobals().GetClockTowerBoostSecs(m_upgLevel);
			}

			return 0;
		}

		public override void DeathEvent()
		{
			base.DeathEvent();

			LogicBuildingData data = GetBuildingData();

			// DestructEffect.

			m_level.GetBattleLog().IncrementDestroyedBuildingCount(data);
			m_level.GetTileMap().GetPathFinder().InvalidateCache();
			m_level.GetVisitorAvatar().XpGainHelper(data.GetDestructionXP(m_upgLevel));

			LogicCalendarBuildingDestroyedSpawnUnit buildingDestroyedSpawnUnit = m_level.GetCalendar().GetBuildingDestroyedSpawnUnit();

			if (buildingDestroyedSpawnUnit != null && buildingDestroyedSpawnUnit.GetBuildingData() == m_data)
			{
				LogicCharacterData characterData = buildingDestroyedSpawnUnit.GetCharacterData();

				for (int i = 0, c = buildingDestroyedSpawnUnit.GetCount(), offsetX = 0, offsetY = 0; i < c; i++, offsetX += 721, offsetY += 1051)
				{
					LogicCharacter character = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(characterData, m_level, GetVillageType());

					character.SetInitialPosition(GetMidX() + offsetX % 512, GetMidY() + offsetY % 512);

					LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

					if (hitpointComponent != null)
					{
						hitpointComponent.SetTeam(1);
						hitpointComponent.SetInvulnerabilityTime(64);
					}

					if (LogicDataTables.GetGlobals().EnableDefendingAllianceTroopJump())
					{
						GetMovementComponent().EnableJump(3600000);
					}

					m_level.GetGameObjectManager().AddGameObject(character, -1);
					GetCombatComponent().SetSearchRadius(LogicDataTables.GetGlobals().GetClanCastleRadius() >> 9);
					GetMovementComponent().GetMovementSystem().CreatePatrolArea(this, m_level, true, i);
				}
			}
		}

		public bool IsConstructing()
			=> m_constructionTimer != null;

		public bool IsUpgrading()
			=> m_constructionTimer != null && m_upgrading;

		public bool IsGearing()
			=> m_constructionTimer != null && m_gearing;

		public bool IsLocked()
			=> m_locked;

		public void SetLocked(bool locked)
		{
			m_locked = locked;
		}

		public void Lock()
		{
			m_locked = true;
			SetUpgradeLevel(0);
		}

		public override bool IsBuilding()
			=> true;

		public override bool IsBoostPaused()
			=> m_boostPause;

		public void SetBoostPause(bool state)
		{
			m_boostPause = state;
		}

		public int GetUpgradeLevel()
			=> m_upgLevel;

		public int GetWallIndex()
			=> m_wallIndex;

		public override bool IsWall()
			=> GetBuildingData().IsWall();

		public override bool IsPassable()
		{
			if (!LogicDataTables.GetGlobals().RemoveUntriggeredTesla() && !LogicDataTables.GetGlobals().UseTeslaTriggerCommand() || !GetBuildingData().IsHidden())
			{
				int state = m_level.GetState();

				if (state != 1 && state != 4)
				{
					LogicHitpointComponent hitpointComponent = GetHitpointComponent();

					if (hitpointComponent != null && hitpointComponent.GetHitpoints() <= 0)
					{
						return true;
					}
				}

				return false;
			}

			return true;
		}

		public bool IsConnectedWall()
		{
			if (IsWall())
			{
				int connectedCount = 0;
				int tileX = GetTileX();
				int tileY = GetTileY();

				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						if ((i | j) != 0)
						{
							LogicTile tile = m_level.GetTileMap().GetTile(tileX + i, tileY + j);

							if (tile != null)
							{
								for (int k = 0; k < tile.GetGameObjectCount(); k++)
								{
									if (tile.GetGameObject(k).IsWall())
									{
										connectedCount += 1;
									}
								}
							}
						}
					}
				}

				return connectedCount > 1;
			}

			return false;
		}

		public override int GetWidthInTiles()
			=> GetBuildingData().GetWidth();

		public override int GetHeightInTiles()
			=> GetBuildingData().GetHeight();

		public override int PassableSubtilesAtEdge()
		{
			if (!IsWall())
			{
				return LogicMath.Max(1, GetBuildingData().GetWidth() - GetBuildingData().GetBuildingW());
			}

			return 0;
		}

		public override int PathFinderCost()
		{
			if (IsWall() && IsAlive())
			{
				LogicHitpointComponent hitpointComponent = GetHitpointComponent();

				if (hitpointComponent != null)
				{
					if (m_hitWallDelay <= 0)
					{
						int hp = hitpointComponent.GetHitpoints() / 100;
						int maxHp = hitpointComponent.GetMaxHitpoints() / 100;
						int wallCostBase = LogicDataTables.GetGlobals().GetWallCostBase();
						int hpMultiplier = 4000 - wallCostBase;

						if (m_selectedWallTime > 0)
						{
							hpMultiplier = (3 * hpMultiplier) >> 2;
						}

						return wallCostBase + Rand(0) % 256 + hp * hpMultiplier / maxHp;
					}

					return 100;
				}
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

			if (m_boostCooldownTimer != null)
			{
				m_boostCooldownTimer.Destruct();
				m_boostCooldownTimer = null;
			}

			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}
		}

		public override void Tick()
		{
			base.Tick();

			if (m_hitWallDelay > 0)
			{
				m_hitWallDelay = LogicMath.Max(m_hitWallDelay - 64, 0);

				if (m_hitWallDelay == 0)
				{
					RefreshPassable();
				}
			}

			if (m_selectedWallTime > 0)
			{
				m_selectedWallTime = LogicMath.Max(m_selectedWallTime - 64, 0);

				if (m_selectedWallTime == 0)
				{
					RefreshPassable();
				}
			}

			if (m_constructionTimer != null)
			{
				if (m_level.GetRemainingClockTowerBoostTime() > 0 && GetBuildingData().GetVillageType() == 1)
				{
					m_constructionTimer.SetFastForward(m_constructionTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
				}

				if (m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
				{
					FinishConstruction(false, true);
				}
			}

			if (m_boostTimer != null)
			{
				if (m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
				{
					m_boostTimer.Destruct();
					m_boostTimer = null;

					if (GetBuildingData().IsClockTower())
					{
						m_boostCooldownTimer = new LogicTimer();
						m_boostCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetClockTowerBoostCooldownSecs(), m_level.GetLogicTime(), false, -1);

						if (m_level.GetGameListener() != null)
						{
							// LogicGameListener.
						}
					}

					if (m_listener != null)
					{
						m_listener.RefreshState();
					}
				}
			}

			LogicHitpointComponent hitpointComponent = GetHitpointComponent();

			if (hitpointComponent != null)
			{
				hitpointComponent.EnableRegeneration(m_level.GetState() == 1);
			}

			if (m_level.IsInCombatState())
			{
				if (m_hidden)
				{
					UpdateHidden();
				}

				if (m_hasAreaOfEffectSpell)
				{
					UpdateAreaOfEffectSpell();
				}

				if (!IsAlive())
				{
					LogicBuildingData data = GetBuildingData();

					int dieDamageDelay = data.GetDieDamageDelay();
					int prevDieDamage = m_dieTime;

					m_dieTime += 64;

					if (dieDamageDelay >= prevDieDamage && dieDamageDelay < m_dieTime)
					{
						UpdateDieDamage(data.GetDieDamage(m_upgLevel), data.GetDieDamageRadius());
					}
				}

				UpdateAuraSpell();
			}
		}

		public override void SubTick()
		{
			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent != null)
			{
				combatComponent.SubTick();

				if (combatComponent.GetTarget(0) != null)
				{
					LogicBuildingData buildingData = GetBuildingData();

					int angleToTarget = GetAngleToTarget();

					if (angleToTarget <= 0)
					{
						if (angleToTarget < 0)
						{
							m_direction -= 16 * buildingData.GetTurnSpeed();
						}
					}
					else
					{
						m_direction += 16 * buildingData.GetTurnSpeed();
					}

					if (angleToTarget * GetAngleToTarget() < 0)
					{
						m_direction = 1000 * (GetDirection() + GetAngleToTarget());
					}

					if (m_direction < 360000)
					{
						if (m_direction < 0)
						{
							m_direction += 360000;
						}
					}
					else
					{
						m_direction -= 360000;
					}

					LogicAttackerItemData attackerItemData = combatComponent.GetAttackerItemData();

					if (attackerItemData.GetTargetingConeAngle() > 0)
					{
						int aimAngle = 1000 * combatComponent.GetAimAngle(m_level.GetActiveLayout(m_level.GetVillageType()), false);
						int newDirection = m_direction - aimAngle;

						if (newDirection < 180000)
						{
							if (newDirection < -180000)
							{
								newDirection += 360000;
							}
						}
						else
						{
							newDirection -= 360000;
						}

						m_direction = aimAngle + LogicMath.Clamp(newDirection, -500 * attackerItemData.GetTargetingConeAngle(),
																	  500 * attackerItemData.GetTargetingConeAngle());

						if (m_direction < 360000)
						{
							if (m_direction < 0)
							{
								m_direction += 360000;
							}
						}
						else
						{
							m_direction -= 360000;
						}
					}
				}
			}

			if (m_boostCooldownTimer != null && m_boostPause)
			{
				m_boostCooldownTimer.StartTimer(m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime()), m_level.GetLogicTime(), false, -1);
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

			if (m_gearing)
			{
				jsonObject.Put("gearing", new LogicJSONBoolean(true));
			}

			if (m_constructionTimer != null)
			{
				jsonObject.Put("const_t", new LogicJSONNumber(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime())));

				if (m_constructionTimer.GetEndTimestamp() != -1)
				{
					jsonObject.Put("const_t_end", new LogicJSONNumber(m_constructionTimer.GetEndTimestamp()));
				}

				if (m_constructionTimer.GetFastForward() > 0)
				{
					jsonObject.Put("con_ff", new LogicJSONNumber(m_constructionTimer.GetFastForward()));
				}
			}

			if (m_locked)
			{
				jsonObject.Put("locked", new LogicJSONBoolean(true));
			}

			if (m_boostTimer != null)
			{
				jsonObject.Put("boost_t", new LogicJSONNumber(m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			if (m_boostPause)
			{
				jsonObject.Put("boost_pause", new LogicJSONBoolean(true));
			}

			if (GetRemainingBoostCooldownTime() > 0)
			{
				jsonObject.Put("bcd", new LogicJSONNumber(GetRemainingBoostCooldownTime()));
			}

			if (m_gear > 0)
			{
				jsonObject.Put("gear", new LogicJSONNumber(m_gear));
			}

			if (IsWall())
			{
				jsonObject.Put("wI", new LogicJSONNumber(m_wallIndex));

				if (m_wallPoint)
				{
					jsonObject.Put("wP", new LogicJSONNumber(1));
				}

				jsonObject.Put("wX", new LogicJSONNumber(m_wallBlockX));
			}

			base.Save(jsonObject, villageType);
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			if (m_upgLevel != 0 || m_constructionTimer == null || m_upgrading)
			{
				jsonObject.Put("lvl", new LogicJSONNumber(m_upgLevel));
			}
			else
			{
				jsonObject.Put("lvl", new LogicJSONNumber(-1));
			}

			if (m_gearing)
			{
				jsonObject.Put("gearing", new LogicJSONBoolean(true));
			}

			if (m_constructionTimer != null)
			{
				jsonObject.Put("const_t", new LogicJSONNumber(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			if (m_locked)
			{
				jsonObject.Put("locked", new LogicJSONBoolean(true));
			}

			if (m_gear > 0)
			{
				jsonObject.Put("gear", new LogicJSONNumber(m_gear));
			}

			base.SaveToSnapshot(jsonObject, layoutId);
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LoadUpgradeLevel(jsonObject);

			LogicJSONNumber constTimeObject = jsonObject.GetJSONNumber("const_t");

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			LogicJSONBoolean gearingObject = jsonObject.GetJSONBoolean("gearing");

			if (gearingObject != null)
			{
				m_gearing = gearingObject.IsTrue();
			}

			if (constTimeObject != null)
			{
				int secs = constTimeObject.GetIntValue();

				if (!LogicDataTables.GetGlobals().ClampBuildingTimes())
				{
					if (m_upgLevel < GetBuildingData().GetUpgradeLevelCount() - 1)
					{
						secs = LogicMath.Min(secs, GetBuildingData().GetConstructionTime(m_upgLevel + 1, m_level, 0));
					}
				}

				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(secs, m_level.GetLogicTime(), false, -1);

				LogicJSONNumber constTimeEndObject = jsonObject.GetJSONNumber("const_t_end");

				if (constTimeEndObject != null)
				{
					m_constructionTimer.SetEndTimestamp(constTimeEndObject.GetIntValue());
				}

				LogicJSONNumber conFFObject = jsonObject.GetJSONNumber("con_ff");

				if (conFFObject != null)
				{
					m_constructionTimer.SetFastForward(conFFObject.GetIntValue());
				}

				if (m_gearing)
				{
					m_level.GetWorkerManagerAt(1).AllocateWorker(this);
				}
				else
				{
					m_level.GetWorkerManagerAt(m_villageType).AllocateWorker(this);
					m_upgrading = m_upgLevel != -1;
				}
			}

			LogicJSONNumber boostTimeObject = jsonObject.GetJSONNumber("boost_t");

			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			if (boostTimeObject != null)
			{
				m_boostTimer = new LogicTimer();
				m_boostTimer.StartTimer(boostTimeObject.GetIntValue(), m_level.GetLogicTime(), false, -1);
			}

			LogicJSONNumber boostCooldownObject = jsonObject.GetJSONNumber("bcd");

			if (boostCooldownObject != null)
			{
				m_boostCooldownTimer = new LogicTimer();
				m_boostCooldownTimer.StartTimer(boostCooldownObject.GetIntValue(), m_level.GetLogicTime(), false, -1);
			}

			LogicJSONBoolean boostPauseObject = jsonObject.GetJSONBoolean("boost_pause");

			if (boostPauseObject != null)
			{
				m_boostPause = boostPauseObject.IsTrue();
			}

			if (m_boostTimer == null)
			{
				if (LogicDataTables.GetGlobals().StopBoostPauseWhenBoostTimeZeroOnLoad())
				{
					m_boostPause = false;
				}
			}

			if (IsWall())
			{
				LogicJSONNumber wallIndexObject = jsonObject.GetJSONNumber("wI");

				if (wallIndexObject != null)
				{
					m_wallIndex = wallIndexObject.GetIntValue();
				}

				LogicJSONNumber wallXObject = jsonObject.GetJSONNumber("wX");

				if (wallXObject != null)
				{
					m_wallBlockX = wallXObject.GetIntValue();
				}

				LogicJSONNumber wallPositionObject = jsonObject.GetJSONNumber("wP");

				if (wallPositionObject != null)
				{
					m_wallPoint = wallPositionObject.GetIntValue() != 0;
				}
			}

			if (LogicDataTables.GetGlobals().FixMergeOldBarrackBoostPausing())
			{
				if (LogicDataTables.GetGlobals().UseNewTraining())
				{
					if (GetBuildingData().GetUnitProduction(0) > 0)
					{
						if (m_boostTimer != null)
						{
							m_boostTimer.Destruct();
							m_boostTimer = null;

							if (m_boostCooldownTimer != null)
							{
								m_boostCooldownTimer.Destruct();
								m_boostCooldownTimer = null;
							}
						}
					}
				}
			}

			SetUpgradeLevel(LogicMath.Clamp(m_upgLevel, 0, GetBuildingData().GetUpgradeLevelCount() - 1));
			base.Load(jsonObject);
		}

		public void LoadUpgradeLevel(LogicJSONObject jsonObject)
		{
			LogicJSONNumber lvlObject = jsonObject.GetJSONNumber("lvl");

			if (lvlObject != null)
			{
				m_upgLevel = lvlObject.GetIntValue();
				int maxUpgLevel = GetBuildingData().GetUpgradeLevelCount();

				if (m_upgLevel >= maxUpgLevel)
				{
					Debugger.Warning(string.Format("LogicBuilding::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}", m_upgLevel, maxUpgLevel,
												   m_globalId, m_data.GetGlobalID()));
					m_upgLevel = maxUpgLevel - 1;
				}
				else
				{
					if (m_upgLevel < -1)
					{
						Debugger.Error("LogicBuilding::load() - Loaded an illegal upgrade level!");
					}
				}
			}
			else
			{
				Debugger.Error("LogicBuilding::load - Upgrade level was not found!");
				m_upgLevel = 0;
			}

			m_level.GetWorkerManagerAt(1).DeallocateWorker(this);
			m_level.GetWorkerManagerAt(m_villageType).DeallocateWorker(this);

			LogicJSONNumber gearObject = jsonObject.GetJSONNumber("gear");

			if (gearObject != null)
			{
				m_gear = gearObject.GetIntValue();
			}

			LogicJSONBoolean lockedObject = jsonObject.GetJSONBoolean("locked");

			if (lockedObject != null)
			{
				m_locked = lockedObject.IsTrue();
			}
			else
			{
				m_locked = false;
			}
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			if (m_data.GetVillageType() == 1)
			{
				Load(jsonObject);
				return;
			}

			LoadUpgradeLevel(jsonObject);
			SetUpgradeLevel(LogicMath.Clamp(m_upgLevel, 0, GetBuildingData().GetUpgradeLevelCount() - 1));
			base.LoadFromSnapshot(jsonObject);
		}

		public override void StopBoost()
		{
			if (m_boostTimer != null && CanStopBoost() && !m_boostPause)
			{
				m_boostPause = true;
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

		public override void FastForwardBoost(int secs)
		{
			base.FastForwardBoost(secs);

			if (GetBuildingData().IsClockTower())
			{
				if (m_boostCooldownTimer != null)
				{
					int remainingSecs = m_boostCooldownTimer.GetRemainingSeconds(m_level.GetLogicTime());

					if (remainingSecs <= secs)
					{
						m_boostCooldownTimer.Destruct();
						m_boostCooldownTimer = null;

						if (m_listener != null)
						{
							m_listener.RefreshState();
						}
					}
					else
					{
						m_boostCooldownTimer.StartTimer(remainingSecs - secs, m_level.GetLogicTime(), false, -1);
					}
				}
			}

			if (m_boostTimer != null)
			{
				if (!m_boostPause)
				{
					int remainingSecs = m_boostTimer.GetRemainingSeconds(m_level.GetLogicTime());

					if (remainingSecs <= secs)
					{
						m_boostTimer.Destruct();
						m_boostTimer = null;

						if (GetBuildingData().IsClockTower())
						{
							int passedSecs = secs - remainingSecs;

							if (passedSecs < 0)
							{
								Debugger.Warning("boost timer run out during FF -> start cooldown, but timeToFF < 0");
							}

							m_boostCooldownTimer = new LogicTimer();
							m_boostCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetClockTowerBoostCooldownSecs() - passedSecs, m_level.GetLogicTime(), false,
																 -1);
						}

						if (m_listener != null)
						{
							m_listener.RefreshState();
						}
					}
					else
					{
						m_boostTimer.StartTimer(remainingSecs - secs, m_level.GetLogicTime(), false, -1);
					}
				}

				if (m_boostTimer != null)
				{
					if (GetBuildingData().IsClockTower())
					{
						m_boostPause = false;
					}
				}
			}
		}

		public override int GetStrengthWeight()
			=> GetBuildingData().GetStrengthWeight(m_upgLevel);

		public bool IsValidTarget(LogicGameObject target)
		{
			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent != null && combatComponent.GetAttackerItemData().GetTargetingConeAngle() != 0)
			{
				return LogicMath.Abs(GetAngleToTarget(target)) <= combatComponent.GetAttackerItemData().GetTargetingConeAngle() / 2;
			}

			if (GetBuildingData().IsNeedsAim())
			{
				return LogicMath.Abs(GetAngleToTarget(target)) < 5;
			}

			return true;
		}

		public int GetAngleToTarget()
		{
			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent == null || combatComponent.GetTarget(0) == null)
			{
				return 0;
			}

			return GetAngleToTarget(combatComponent.GetTarget(0));
		}

		public int GetAngleToTarget(LogicGameObject gameObject)
		{
			int x = gameObject.GetMidX() - GetMidX();
			int y = gameObject.GetMidY() - GetMidY();

			if (x != 0 || y != 0)
			{
				return LogicMath.NormalizeAngle180(LogicMath.GetAngle(x, y) - GetDirection());
			}

			return 0;
		}

		public void UpdateHidden()
		{
			if (((m_level.GetLogicTime().GetTick() / 4) & 7) == 0)
			{
				if (m_constructionTimer != null)
				{
					m_hidden = false;
				}

				LogicBuildingData data = GetBuildingData();
				LogicGameObjectManager gameObjectManager = GetGameObjectManager();
				LogicGameObject gameObject = gameObjectManager.GetClosestGameObject(GetMidX(), GetMidY(), m_filter);

				bool isInArea = false;

				if (gameObject != null)
				{
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent == null || combatComponent.GetUndergroundTime() <= 0)
					{
						if (LogicDataTables.GetGlobals().SkeletonTriggerTesla() || !LogicDataTables.IsSkeleton((LogicCharacterData)m_data))
						{
							isInArea = gameObject.GetPosition().GetDistanceSquaredTo(GetMidX(), GetMidY()) < data.GetTriggerRadius() * data.GetTriggerRadius();
						}
					}
				}

				if (isInArea || m_level.GetBattleLog().GetDestructionPercentage() > LogicDataTables.GetGlobals().GetHiddenBuildingAppearDestructionPercentage())
				{
					if (LogicDataTables.GetGlobals().UseTeslaTriggerCommand())
					{
						if (m_level.GetState() != 5)
						{
							LogicTriggerTeslaCommand triggerTeslaCommand = new LogicTriggerTeslaCommand(this);
							triggerTeslaCommand.SetExecuteSubTick(m_level.GetLogicTime().GetTick() + 1);
							m_level.GetGameMode().GetCommandManager().AddCommand(triggerTeslaCommand);
						}
					}
					else
					{
						Trigger();
					}
				}
			}
		}

		public void UpdateAreaOfEffectSpell()
		{
			if (IsAlive())
			{
				LogicGameObject gameObject = GetGameObjectManager().GetClosestGameObject(GetMidX(), GetMidY(), m_filter);

				if (gameObject != null)
				{
					bool altAttackMode = GetCombatComponent(false).UseAltAttackMode(m_level.GetActiveLayout(m_level.GetVillageType()), false);

					int distanceSquared = gameObject.GetPosition().GetDistanceSquaredTo(GetMidX(), GetMidY());
					int radius =
						(m_shareHeroCombatData ?? GetBuildingData().GetAttackerItemData(m_upgLevel)).GetAttackRange(altAttackMode);

					if ((uint)distanceSquared <= radius * radius)
					{
						if (m_areaOfEffectSpell == null)
						{
							m_areaOfEffectSpell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(GetBuildingData().GetAreaOfEffectSpell(), m_level,
																											m_villageType);
							m_areaOfEffectSpell.SetUpgradeLevel(m_upgLevel);
							m_areaOfEffectSpell.SetInitialPosition(GetMidX(), GetMidY());

							GetGameObjectManager().AddGameObject(m_areaOfEffectSpell, -1);
						}

						return;
					}
				}
			}

			if (m_areaOfEffectSpell != null)
			{
				m_upgrading = true;
				m_areaOfEffectSpell = null;
			}
		}

		public void UpdateAuraSpell()
		{
			if (IsAlive())
			{
				LogicBuildingData data = GetBuildingData();

				if (data.GetShareHeroCombatData())
				{
					LogicHeroBaseComponent heroBaseComponent = GetHeroBaseComponent();

					if (heroBaseComponent != null)
					{
						LogicHeroData heroData = heroBaseComponent.GetHeroData();
						LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

						int heroUpgLevel = homeOwnerAvatar.GetUnitUpgradeLevel(heroData);

						if (homeOwnerAvatar.IsHeroAvailableForAttack(heroData))
						{
							LogicSpellData auraSpell = heroData.GetAuraSpell(heroUpgLevel);

							if (auraSpell != null)
							{
								if (m_auraSpell != null)
								{
									if (!m_auraSpell.GetHitsCompleted())
									{
										return;
									}

									GetGameObjectManager().RemoveGameObject(m_auraSpell);
									m_auraSpell = null;
								}

								if (m_level.GetBattleLog().GetBattleStarted())
								{
									LogicCombatComponent combatComponent = GetCombatComponent();

									if (combatComponent != null && combatComponent.GetDeployedHousingSpace() >= combatComponent.GetAttackerItemData().GetWakeUpSpace() &&
										combatComponent.GetWakeUpTime() == 0)
									{
										m_auraSpell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(auraSpell, m_level, m_villageType);
										m_auraSpell.SetUpgradeLevel(heroData.GetAuraSpellLevel(heroUpgLevel));
										m_auraSpell.SetInitialPosition(GetMidX(), GetMidY());
										m_auraSpell.AllowDestruction(false);
										m_auraSpell.SetTeam(GetHitpointComponent().GetTeam());

										GetGameObjectManager().AddGameObject(m_auraSpell, -1);
									}
								}
							}
						}
					}
				}
			}
			else if (m_auraSpell != null)
			{
				GetGameObjectManager().RemoveGameObject(m_auraSpell);
				m_auraSpell = null;
			}
		}

		public void UpdateDieDamage(int damage, int radius)
		{
			if (damage > 0 && radius > 0 && GetHitpointComponent() != null && m_constructionTimer == null)
			{
				m_level.AreaDamage(0, GetMidX(), GetMidY(), radius, damage, null, 0, null, GetHitpointComponent().GetTeam(), null, 1, 0, 0, true, false, 100, 0,
										this, 100, 0);
				// Listener.
			}
		}

		public bool IsMaxUpgradeLevel()
		{
			LogicBuildingData buildingData = GetBuildingData();

			if (buildingData.IsTownHallVillage2())
			{
				return m_upgLevel >= m_level.GetGameMode().GetConfiguration().GetMaxTownHallLevel() - 1;
			}

			if (buildingData.GetVillageType() == 1)
			{
				if (GetRequiredTownHallLevelForUpgrade() >= m_level.GetGameMode().GetConfiguration().GetMaxTownHallLevel())
				{
					return true;
				}
			}

			return m_upgLevel >= buildingData.GetUpgradeLevelCount() - 1;
		}

		public int GetRequiredTownHallLevelForUpgrade()
			=> GetBuildingData().GetRequiredTownHallLevel(LogicMath.Min(m_upgLevel + 1, GetBuildingData().GetUpgradeLevelCount() - 1));

		public int GetBoostMultiplier()
		{
			if (GetComponent(LogicComponentType.RESOURCE_PRODUCTION) != null)
			{
				return LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier();
			}

			if (GetComponent(LogicComponentType.UNIT_PRODUCTION) == null)
			{
				if (GetComponent(LogicComponentType.HERO_BASE) != null)
				{
					return LogicDataTables.GetGlobals().GetHeroRestBoostMultiplier();
				}

				if (GetBuildingData().IsClockTower())
				{
					return LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier();
				}
			}
			else
			{
				LogicUnitProductionComponent unitProductionComponent = (LogicUnitProductionComponent)GetComponent(LogicComponentType.UNIT_PRODUCTION);

				if (unitProductionComponent.GetProductionType() == 1)
				{
					if (LogicDataTables.GetGlobals().UseNewTraining())
					{
						return LogicDataTables.GetGlobals().GetSpellFactoryBoostMultiplier();
					}

					return LogicDataTables.GetGlobals().GetSpellFactoryBoostMultiplier();
				}

				if (unitProductionComponent.GetProductionType() == 0)
				{
					if (LogicDataTables.GetGlobals().UseNewTraining())
					{
						return LogicDataTables.GetGlobals().GetBarracksBoostNewMultiplier();
					}

					return LogicDataTables.GetGlobals().GetBarracksBoostMultiplier();
				}
			}

			return 1;
		}

		public bool CanUnlock(bool canCallListener)
		{
			if (m_constructionTimer != null || m_upgLevel != 0 || !m_locked)
			{
				return false;
			}

			bool canUnlock = m_level.GetTownHallLevel(m_level.GetVillageType()) >= GetBuildingData().GetRequiredTownHallLevel(0);

			if (!canUnlock)
			{
				m_level.GetGameListener().TownHallLevelTooLow(GetRequiredTownHallLevelForUpgrade());
			}

			return canUnlock;
		}

		public bool CanUpgrade(bool canCallListener)
		{
			if (m_constructionTimer == null && !IsMaxUpgradeLevel())
			{
				if (m_level.GetTownHallLevel(m_villageType) >= GetRequiredTownHallLevelForUpgrade())
				{
					return true;
				}

				if (canCallListener)
				{
					m_level.GetGameListener().TownHallLevelTooLow(GetRequiredTownHallLevelForUpgrade());
				}
			}

			return false;
		}

		public bool CanSell()
			=> false;

		public bool CanBeBoosted()
		{
			if (m_boostCooldownTimer != null && m_boostCooldownTimer.GetRemainingSeconds(m_level.GetLogicTime()) > 0)
			{
				return false;
			}

			if (m_data.GetVillageType() == 1)
			{
				if (GetBuildingData().IsClockTower())
				{
					return true;
				}
			}
			else
			{
				if (m_boostTimer == null)
				{
					int maxBoostTime = GetMaxBoostTime();

					if (maxBoostTime > 0)
					{
						return m_level.GetGameMode().GetCalendar().GetBuildingBoostCost(GetBuildingData(), m_upgLevel) > 0 || GetBuildingData().IsFreeBoost();
					}
				}
			}

			return false;
		}

		public bool CanStopBoost()
			=> GetComponent(LogicComponentType.UNIT_PRODUCTION) != null || GetComponent(LogicComponentType.HERO_BASE) != null ||
				   GetBuildingData().IsClockTower();

		public int GetBoostCost()
			=> m_level.GetGameMode().GetCalendar().GetBuildingBoostCost(GetBuildingData(), m_upgLevel);

		public LogicResourceData GetSellResource()
			=> GetBuildingData().GetBuildResource(m_upgLevel);

		public int GetSelectedWallTime()
			=> m_selectedWallTime;

		public void StartSelectedWallTime()
		{
			int selectedWallTime = LogicDataTables.GetGlobals().GetSelectedWallTime();

			if (selectedWallTime > 0 && IsWall())
			{
				if (m_selectedWallTime == 0)
				{
					m_selectedWallTime = selectedWallTime;
					RefreshPassable();
					m_level.GetTileMap().GetPathFinder().InvalidateCache();
				}

				m_selectedWallTime = selectedWallTime;
			}
		}

		public int GetHitWallDelay()
			=> m_hitWallDelay;

		public void SetHitWallDelay(int time)
		{
			if (time > 0 && IsWall())
			{
				if (m_hitWallDelay == 0)
				{
					m_hitWallDelay = time;
					RefreshPassable();
				}

				m_hitWallDelay = time;
			}
		}

		public void OnSell()
		{
			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

			if (homeOwnerAvatar.IsClientAvatar())
			{
				if (GetComponent(LogicComponentType.RESOURCE_STORAGE) != null)
				{
					EnableComponent(LogicComponentType.RESOURCE_STORAGE, false);
					m_level.RefreshResourceCaps();
				}

				if (GetComponent(LogicComponentType.UNIT_STORAGE) != null)
				{
					LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent)GetComponent(LogicComponentType.UNIT_STORAGE);

					for (int i = 0; i < unitStorageComponent.GetUnitTypeCount(); i++)
					{
						homeOwnerAvatar.CommodityCountChangeHelper(0, unitStorageComponent.GetUnitType(i), -unitStorageComponent.GetUnitCount(i));
					}
				}
			}
		}

		public int GetSellPrice()
		{
			if (m_constructionTimer == null)
			{
				return GetBuildingData().GetBuildCost(m_upgLevel, m_level) / 5;
			}

			if (!m_gearing)
			{
				return GetBuildingData().GetBuildCost(m_upgLevel, m_level);
			}

			return GetBuildingData().GetBuildCost(m_upgLevel + 1, m_level) + GetBuildingData().GetBuildCost(m_upgLevel, m_level) / 5;
		}

		public bool SpeedUpConstruction()
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

		public bool SpeedUpBoostCooldown()
		{
			if (m_boostCooldownTimer != null)
			{
				int cooldownSecs = m_boostCooldownTimer.GetRemainingSeconds(m_level.GetLogicTime());

				if (cooldownSecs > 0)
				{
					LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();
					int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(cooldownSecs, GetBuildingData().IsClockTower() ? 6 : 5, m_villageType);

					if (playerAvatar.HasEnoughDiamonds(speedUpCost, true, m_level))
					{
						playerAvatar.UseDiamonds(speedUpCost);
						playerAvatar.GetChangeListener().DiamondPurchaseMade(17, m_data.GetGlobalID(), m_upgLevel, speedUpCost, m_level.GetVillageType());

						m_boostCooldownTimer.Destruct();
						m_boostCooldownTimer = null;

						Boost();

						return true;
					}
				}
			}

			return false;
		}

		public void StartConstructing(bool updateListener)
		{
			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			int constructionTime = GetBuildingData().GetConstructionTime(m_upgLevel, m_level, 1);

			if (constructionTime <= 0)
			{
				FinishConstruction(updateListener, updateListener);
			}
			else
			{
				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(constructionTime, m_level.GetLogicTime(), true, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());

				m_level.GetWorkerManagerAt(GetBuildingData().GetVillageType()).AllocateWorker(this);
			}

			if (m_villageType == 1 && m_locked)
			{
				// this.m_level.GetGameListener.???
			}
		}

		public void DestructBoost()
		{
			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;

				if (GetBuildingData().IsClockTower())
				{
					m_boostCooldownTimer = new LogicTimer();
					m_boostCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetClockTowerBoostCooldownSecs(), m_level.GetLogicTime(), false, -1);
				}
			}
		}

		public void StartUpgrading(bool updateListener, bool gearup)
		{
			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			DestructBoost();

			int constructionTime;

			if (gearup)
			{
				constructionTime = GetBuildingData().GetGearUpTime(m_upgLevel);
				m_gearing = true;
			}
			else
			{
				constructionTime = GetBuildingData().GetConstructionTime(m_upgLevel + 1, m_level, 0);
				m_upgrading = true;
			}

			if (constructionTime <= 0)
			{
				FinishConstruction(false, updateListener);
			}
			else
			{
				m_level.GetWorkerManagerAt(m_gearing ? 1 : GetBuildingData().GetVillageType()).AllocateWorker(this);

				if (GetComponent(LogicComponentType.RESOURCE_PRODUCTION) != null)
				{
					GetResourceProductionComponent().CollectResources(false);
				}

				EnableComponent(LogicComponentType.RESOURCE_PRODUCTION, false);
				EnableComponent(LogicComponentType.UNIT_PRODUCTION, false);
				EnableComponent(LogicComponentType.UNIT_UPGRADE, false);

				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(constructionTime, m_level.GetLogicTime(), true, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
			}
		}

		public void FinishConstruction(bool ignoreState, bool updateListener)
		{
			int state = m_level.GetState();

			if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
			{
				if (m_level.GetHomeOwnerAvatar() != null)
				{
					if (m_level.GetHomeOwnerAvatar().IsClientAvatar())
					{
						LogicClientAvatar homeOwnerAvatar = (LogicClientAvatar)m_level.GetHomeOwnerAvatar();

						if (m_constructionTimer != null)
						{
							m_constructionTimer.Destruct();
							m_constructionTimer = null;
						}

						m_level.GetWorkerManagerAt(m_gearing ? 1 : GetBuildingData().GetVillageType()).DeallocateWorker(this);
						m_locked = false;

						if (m_gearing)
						{
							m_gear += 1;

							LogicCombatComponent combatComponent = GetCombatComponent(false);

							if (combatComponent != null)
							{
								combatComponent.ToggleAttackMode(m_level.GetActiveLayout(), false);
							}
						}
						else
						{
							if (m_upgLevel != 0 || m_upgrading)
							{
								int newUpgLevel = m_upgLevel + 1;

								if (m_upgLevel >= GetBuildingData().GetUpgradeLevelCount() - 1)
								{
									Debugger.Warning("LogicBuilding - Trying to upgrade to level that doesn't exist! - " + GetBuildingData().GetName());
									newUpgLevel = GetBuildingData().GetUpgradeLevelCount() - 1;
								}

								int constructionTime = GetBuildingData().GetConstructionTime(newUpgLevel, m_level, 0);
								int xpGain = LogicGamePlayUtil.TimeToExp(constructionTime);
								SetUpgradeLevel(newUpgLevel);
								XpGainHelper(xpGain, homeOwnerAvatar, ignoreState);
							}
							else
							{
								int constructionTime = GetBuildingData().GetConstructionTime(0, m_level, 0);
								int xpGain = LogicGamePlayUtil.TimeToExp(constructionTime);
								SetUpgradeLevel(m_upgLevel);
								XpGainHelper(xpGain, homeOwnerAvatar, ignoreState);

								LogicCombatComponent combatComponent = GetCombatComponent();

								if (combatComponent != null)
								{
									if (combatComponent.UseAmmo())
									{
										combatComponent.LoadAmmo();
									}
								}

								if (GetComponent(LogicComponentType.HERO_BASE) != null)
								{
									LogicHeroData heroData = GetBuildingData().GetHeroData();

									if (heroData != null)
									{
										int heroState = heroData.HasNoDefence() ? 2 : 3;

										homeOwnerAvatar.SetHeroState(heroData, heroState);
										homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, heroData, heroState);
									}
									else
									{
										Debugger.Warning("No hero data in herobase/altar building");
									}
								}
							}
						}

						if (GetComponent(LogicComponentType.RESOURCE_PRODUCTION) != null)
						{
							((LogicResourceProductionComponent)GetComponent(LogicComponentType.RESOURCE_PRODUCTION)).RestartTimer();
						}

						m_upgrading = false;
						m_gearing = false;

						if (m_listener != null)
						{
							m_listener.RefreshState();
						}

						if (state == 1)
						{
							m_level.GetAchievementManager().RefreshStatus();
						}

						LogicBuildingClassData buildingClassData = GetBuildingData().GetBuildingClass();

						if (buildingClassData.IsTownHall() || buildingClassData.IsTownHall2())
						{
							m_level.RefreshNewShopUnlocksTH(m_data.GetVillageType());

							if (buildingClassData.IsTownHall2())
							{
								m_level.GetGameObjectManagerAt(1).Village2TownHallFixed();
							}
						}

						return;
					}
				}

				Debugger.Warning("LogicBuilding::finishCostruction failed - Avatar is null or not client avatar");
			}
		}

		public void CancelConstruction()
		{
			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar())
			{
				if (m_constructionTimer != null)
				{
					m_constructionTimer.Destruct();
					m_constructionTimer = null;

					int upgLevel = m_upgLevel;

					if (m_upgrading)
					{
						SetUpgradeLevel(m_upgLevel);
						upgLevel += 1;
					}

					LogicBuildingData data = GetBuildingData();
					LogicResourceData buildResourceData = data.GetBuildResource(upgLevel);

					int buildCost = data.GetBuildCost(upgLevel, m_level);

					if (m_gearing)
					{
						buildResourceData = data.GetGearUpResource();
						buildCost = data.GetGearUpCost(m_upgLevel);
					}

					homeOwnerAvatar.CommodityCountChangeHelper(0, buildResourceData, LogicMath.Max(LogicDataTables.GetGlobals().GetBuildCancelMultiplier() * buildCost / 100, 0));

					if (m_gearing)
					{
						m_level.GetWorkerManagerAt(1).DeallocateWorker(this);
					}
					else
					{
						m_level.GetWorkerManagerAt(m_data.GetVillageType()).DeallocateWorker(this);

						if (upgLevel == 0)
						{
							GetGameObjectManager().RemoveGameObject(this);
							return;
						}
					}

					if (m_listener != null)
					{
						m_listener.RefreshState();
					}
				}
			}
		}

		public void SetWallObjectId(int wallIdx, int wallBlockX, bool wallPoint)
		{
			if (!IsWall())
			{
				Debugger.Error("setWallObjectId called for non Wall");
			}

			m_wallIndex = wallIdx;
			m_wallPoint = wallPoint;
			m_wallBlockX = wallBlockX;
		}

		public void Boost()
		{
			if (m_boostTimer != null)
			{
				m_boostTimer.Destruct();
				m_boostTimer = null;
			}

			m_boostTimer = new LogicTimer();
			m_boostTimer.StartTimer(GetMaxBoostTime(), m_level.GetLogicTime(), false, -1);

			if (GetBuildingData().IsClockTower())
			{
				LogicGameListener gameListener = m_level.GetGameListener();

				if (gameListener != null)
				{
					// Listener.
				}
			}
		}

		public void SetUpgradeLevel(int level)
		{
			LogicBuildingData buildingData = (LogicBuildingData)m_data;

			m_upgLevel = LogicMath.Clamp(level, 0, buildingData.GetUpgradeLevelCount() - 1);

			if (m_level.GetHomeOwnerAvatar() != null)
			{
				if (buildingData.IsAllianceCastle() && !m_locked)
				{
					m_level.GetHomeOwnerAvatar().GetChangeListener().SetAllianceCastleLevel(m_upgLevel);
					m_level.GetHomeOwnerAvatar().SetAllianceCastleLevel(m_upgLevel);

					LogicBuilding building = m_level.GetGameObjectManagerAt(0).GetAllianceCastle();

					if (building != null)
					{
						building.SetTreasurySize();
					}
				}
				else if (buildingData.IsTownHall())
				{
					m_level.GetHomeOwnerAvatar().GetChangeListener().SetTownHallLevel(m_upgLevel);
					m_level.GetHomeOwnerAvatar().SetTownHallLevel(m_upgLevel);

					LogicBuilding building = m_level.GetGameObjectManagerAt(0).GetAllianceCastle();

					if (building != null)
					{
						building.SetTreasurySize();
					}
				}
				else if (buildingData.IsTownHallVillage2())
				{
					m_level.GetHomeOwnerAvatar().GetChangeListener().SetVillage2TownHallLevel(m_upgLevel);
					m_level.GetHomeOwnerAvatar().SetVillage2TownHallLevel(m_upgLevel);
				}
				else if (buildingData.IsBarrackVillage2())
				{
					m_level.GetHomeOwnerAvatar().SetVillage2BarrackLevel(m_upgLevel);
				}
			}

			if (m_upgLevel != 0 || m_upgrading || m_constructionTimer == null)
			{
				bool enable = m_constructionTimer == null;

				EnableComponent(LogicComponentType.UNIT_PRODUCTION, enable);
				EnableComponent(LogicComponentType.UNIT_UPGRADE, enable);
				EnableComponent(LogicComponentType.COMBAT, enable);
				EnableComponent(LogicComponentType.VILLAGE2_UNIT, enable);
				EnableComponent(LogicComponentType.RESOURCE_PRODUCTION, enable);

				LogicUnitStorageComponent unitStorageComponent = GetUnitStorageComponent();

				if (unitStorageComponent != null)
				{
					unitStorageComponent.SetMaxCapacity(buildingData.GetUnitStorageCapacity(m_upgLevel));
				}

				LogicBunkerComponent bunkerComponent = GetBunkerComponent();

				if (bunkerComponent != null)
				{
					bunkerComponent.SetMaxCapacity(buildingData.GetUnitStorageCapacity(m_upgLevel));
				}

				LogicDefenceUnitProductionComponent defenceUnitProductionComponent = GetDefenceUnitProduction();

				if (defenceUnitProductionComponent != null)
				{
					defenceUnitProductionComponent.SetDefenceTroops(buildingData.GetDefenceTroopCharacter(m_upgLevel), buildingData.GetDefenceTroopCharacter2(m_upgLevel),
													 buildingData.GetDefenceTroopCount(m_upgLevel), buildingData.GetDefenceTroopLevel(m_upgLevel), 1);
				}

				LogicHitpointComponent hitpointComponent = GetHitpointComponent();

				if (hitpointComponent != null)
				{
					if (m_locked)
					{
						hitpointComponent.SetMaxHitpoints(0);
						hitpointComponent.SetHitpoints(0);
						hitpointComponent.SetMaxRegenerationTime(100);
					}
					else
					{
						hitpointComponent.SetMaxHitpoints(buildingData.GetHitpoints(m_upgLevel));
						hitpointComponent.SetHitpoints(buildingData.GetHitpoints(m_upgLevel));
						hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(m_upgLevel));
					}
				}

				LogicCombatComponent combatComponent = GetCombatComponent();
				LogicAttackerItemData attackerItemData = buildingData.GetAttackerItemData(m_upgLevel);

				if (buildingData.GetHeroData() != null && buildingData.GetShareHeroCombatData())
				{
					m_shareHeroCombatData = buildingData.GetHeroData().GetAttackerItemData(0).Clone();
					m_shareHeroCombatData.AddAttackRange(buildingData.GetWidth() * 72704 / 200);

					attackerItemData = m_shareHeroCombatData;
				}

				if (combatComponent != null)
				{
					combatComponent.SetAttackValues(attackerItemData, 100);
				}

				LogicResourceProductionComponent resourceProductionComponent = GetResourceProductionComponent();

				if (resourceProductionComponent != null)
				{
					resourceProductionComponent.SetProduction(buildingData.GetResourcePer100Hours(m_upgLevel), buildingData.GetResourceMax(m_upgLevel));
				}

				LogicResourceStorageComponent resourceStorageComponent = GetResourceStorageComponentComponent();

				if (resourceStorageComponent != null)
				{
					resourceStorageComponent.SetMaxArray(buildingData.GetMaxStoredResourceCounts(m_upgLevel));
					resourceStorageComponent.SetMaxPercentageArray(buildingData.GetMaxPercentageStoredResourceCounts(m_upgLevel));
				}

				SetTreasurySize();
			}
		}

		public void SetTreasurySize()
		{
			LogicBuildingData data = GetBuildingData();
			LogicWarResourceStorageComponent component = GetWarResourceStorageComponent();

			if (data.IsAllianceCastle() && LogicDataTables.GetGlobals().TreasurySizeBasedOnTownHall())
			{
				LogicTownhallLevelData townhallLevelData = LogicDataTables.GetTownHallLevel(m_level.GetTownHallLevel(0));

				if (townhallLevelData != null)
				{
					component.SetMaxArray(townhallLevelData.GetTreasuryCaps());
					return;
				}
			}

			if (component != null)
			{
				component.SetMaxArray(data.GetMaxStoredResourceCounts(m_upgLevel));
			}
		}

		public override void LoadingFinished()
		{
			base.LoadingFinished();

			if (LogicDataTables.GetGlobals().ClampBuildingTimes())
			{
				if (m_constructionTimer != null)
				{
					LogicBuildingData buildingData = GetBuildingData();

					int remainingSecs = m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());
					int totalSecs = 0;

					if (m_gearing)
					{
						totalSecs = buildingData.GetGearUpTime(m_upgLevel);
					}
					else
					{
						int upgLevel = m_upgrading ? m_upgLevel : -1;

						if (upgLevel < buildingData.GetUpgradeLevelCount() - 1)
						{
							totalSecs = buildingData.GetConstructionTime(upgLevel + 1, m_level, 0);
						}
					}

					if (remainingSecs > totalSecs)
					{
						m_constructionTimer.StartTimer(totalSecs, m_level.GetLogicTime(), true, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
					}
				}
			}

			LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

			if (homeOwnerAvatar != null)
			{
				LogicBuildingData buildingData = GetBuildingData();

				if (buildingData.IsAllianceCastle() && !m_locked)
				{
					if (homeOwnerAvatar.GetAllianceCastleLevel() != m_upgLevel)
					{
						m_level.GetHomeOwnerAvatar().GetChangeListener().SetAllianceCastleLevel(m_upgLevel);
						m_level.GetHomeOwnerAvatar().SetAllianceCastleLevel(m_upgLevel);

						LogicBuilding building = m_level.GetGameObjectManagerAt(0).GetAllianceCastle();

						if (building != null)
						{
							building.SetTreasurySize();
						}
					}
				}
				else if (buildingData.IsTownHall())
				{
					if (homeOwnerAvatar.GetTownHallLevel() != m_upgLevel)
					{
						m_level.GetHomeOwnerAvatar().GetChangeListener().SetTownHallLevel(m_upgLevel);
						m_level.GetHomeOwnerAvatar().SetTownHallLevel(m_upgLevel);

						LogicBuilding building = m_level.GetGameObjectManagerAt(0).GetAllianceCastle();

						if (building != null)
						{
							building.SetTreasurySize();
						}
					}
				}
				else if (buildingData.IsTownHallVillage2())
				{
					if (homeOwnerAvatar.GetVillage2TownHallLevel() != m_upgLevel)
					{
						m_level.GetHomeOwnerAvatar().GetChangeListener().SetVillage2TownHallLevel(m_upgLevel);
						m_level.GetHomeOwnerAvatar().SetVillage2TownHallLevel(m_upgLevel);
					}
				}
			}

			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent != null)
			{
				if (combatComponent.GetAttackerItemData().GetTargetingConeAngle() <= 0)
				{
					LogicBuilding townHall = GetGameObjectManager().GetTownHall();

					if (townHall != null)
					{
						int distanceX = GetMidX() - townHall.GetMidX();
						int distanceY = GetMidY() - townHall.GetMidY();

						m_direction = 1000 * LogicMath.GetAngle(distanceX, distanceY);
					}
				}
				else
				{
					m_direction = 1000 * combatComponent.GetAimAngle(m_level.GetActiveLayout(m_level.GetVillageType()), false);
				}

				LogicBuildingData buildingData = GetBuildingData();
				LogicAttackerItemData attackerItemData = buildingData.GetAttackerItemData(m_upgLevel);
				LogicHeroData heroData = buildingData.GetHeroData();

				if (heroData != null && buildingData.GetShareHeroCombatData())
				{
					int heroUpgLevel = homeOwnerAvatar.GetUnitUpgradeLevel(heroData);

					m_shareHeroCombatData = heroData.GetAttackerItemData(heroUpgLevel).Clone();
					m_shareHeroCombatData.AddAttackRange(buildingData.GetWidth() * 72704 / 200);

					attackerItemData = m_shareHeroCombatData;

					if (homeOwnerAvatar.IsHeroAvailableForAttack(heroData))
					{
						if (!m_locked && m_level.IsInCombatState())
						{
							LogicHitpointComponent hitpointComponent = GetHitpointComponent();

							hitpointComponent.SetMaxHitpoints(heroData.GetHitpoints(heroUpgLevel));
							hitpointComponent.SetHitpoints(heroData.GetHitpoints(heroUpgLevel));
							hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(m_upgLevel));
						}
					}
					else
					{
						combatComponent.SetEnabled(false);
					}
				}

				combatComponent.SetAttackValues(attackerItemData, 100);
			}
		}

		public override void GetChecksum(ChecksumHelper checksum, bool includeGameObjects)
		{
			checksum.StartObject("LogicBuilding");

			base.GetChecksum(checksum, includeGameObjects);

			if (GetComponent(LogicComponentType.RESOURCE_STORAGE) != null)
			{
				GetComponent(LogicComponentType.RESOURCE_STORAGE).GetChecksum(checksum);
			}

			if (GetComponent(LogicComponentType.RESOURCE_PRODUCTION) != null)
			{
				GetComponent(LogicComponentType.RESOURCE_PRODUCTION).GetChecksum(checksum);
			}

			checksum.EndObject();
		}

		public override int GetDirection()
			=> m_direction / 1000;

		public override bool IsHidden()
			=> m_hidden;

		public void Trigger()
		{
			m_hidden = false;

			if (GetCombatComponent() != null && !m_hidden)
			{
				int attackRange = GetCombatComponent().GetAttackRange(0, false);

				LogicGameObjectManager gameObjectManager = GetGameObjectManager();
				LogicVector2 position = new LogicVector2(GetMidX(), GetMidY());

				LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(LogicGameObjectType.CHARACTER);

				for (int i = 0; i < gameObjects.Size(); i++)
				{
					LogicCharacter character = (LogicCharacter)gameObjects[i];
					LogicCombatComponent combatComponent = character.GetCombatComponent();

					int distanceSquared = position.GetDistanceSquared(character.GetPosition());

					if (combatComponent != null && (distanceSquared < attackRange * attackRange ||
													LogicCombatComponent.IsPreferredTarget(combatComponent.GetPreferredTarget(), this)))
					{
						LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

						if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
						{
							combatComponent.ForceNewTarget();
						}
					}
				}

				position.Destruct();
			}
		}

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.BUILDING;
	}
}