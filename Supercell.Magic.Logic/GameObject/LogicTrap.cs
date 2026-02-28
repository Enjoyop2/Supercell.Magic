using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicTrap : LogicGameObject
	{
		private LogicTimer m_constructionTimer;

		private bool m_upgrading;
		private bool m_disarmed;

		private int m_upgLevel;
		private int m_numSpawns;
		private int m_fadeTime;
		private int m_spawnInitDelay;
		private int m_hitTime; // 132
		private int m_hitCount; // 136
		private int m_actionTime; // 128

		private readonly bool[] m_useAirMode;
		private readonly bool[] m_draftUseAirMode;

		private readonly int[] m_direction;
		private readonly int[] m_draftDirection;

		public LogicTrap(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			m_fadeTime = -1;
			m_hitTime = -1;

			m_useAirMode = new bool[8];
			m_draftUseAirMode = new bool[8];
			m_direction = new int[8];
			m_draftDirection = new int[8];

			LogicTrapData trapData = (LogicTrapData)data;

			AddComponent(new LogicTriggerComponent(this, trapData.GetTriggerRadius(), trapData.GetAirTrigger(), trapData.GetGroundTrigger(), trapData.GetHealerTrigger(),
														trapData.GetMinTriggerHousingLimit()));
			AddComponent(new LogicLayoutComponent(this));

			m_numSpawns = trapData.GetNumSpawns(0);
			m_spawnInitDelay = trapData.GetSpawnInitialDelayMS() / 64;
		}

		public LogicTrapData GetTrapData()
			=> (LogicTrapData)m_data;

		public override void Destruct()
		{
			base.Destruct();

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}
		}

		public override void GetChecksum(ChecksumHelper checksum, bool includeGameObjects)
		{
			checksum.StartObject("LogicTrap");

			base.GetChecksum(checksum, includeGameObjects);

			if (m_constructionTimer != null)
			{
				checksum.WriteValue("remainingMS", m_constructionTimer.GetRemainingMS(m_level.GetLogicTime()));
			}

			checksum.EndObject();
		}

		public override bool ShouldDestruct()
		{
			if (m_fadeTime >= 1000)
			{
				int trapCount = GetGameObjectManager().GetGameObjectCountByData(GetData());
				int townHallLevel = m_level.GetTownHallLevel(m_level.GetVillageType());

				LogicTownhallLevelData townhallLevelData = LogicDataTables.GetTownHallLevel(townHallLevel);

				return townhallLevelData.GetUnlockedTrapCount(GetTrapData()) < trapCount;
			}

			return false;
		}

		public void FinishConstruction(bool ignoreState)
		{
			int state = m_level.GetState();

			if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
			{
				LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

				if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar())
				{
					LogicTrapData data = GetTrapData();

					if (m_constructionTimer != null)
					{
						m_constructionTimer.Destruct();
						m_constructionTimer = null;
					}

					m_level.GetWorkerManagerAt(m_data.GetVillageType()).DeallocateWorker(this);

					if (m_upgLevel != 0 || m_upgrading)
					{
						if (m_upgLevel >= data.GetUpgradeLevelCount() - 1)
						{
							Debugger.Warning("LogicTrap - Trying to upgrade to level that doesn't exist! - " + data.GetName());
							m_upgLevel = data.GetUpgradeLevelCount() - 1;
						}
						else
						{
							m_upgLevel += 1;
						}
					}

					if (!ignoreState && !m_disarmed)
					{
						if (GetListener() != null)
						{
							// Listener.
						}
					}

					XpGainHelper(LogicGamePlayUtil.TimeToExp(data.GetBuildTime(m_upgLevel)), homeOwnerAvatar, ignoreState);

					if (m_disarmed)
					{
						// Listener.
					}

					m_fadeTime = 0;
					m_disarmed = false;
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
					Debugger.Warning("LogicTrap::finishCostruction failed - Avatar is null or not client avatar");
				}
			}
		}

		public bool IsFadingOut()
			=> m_fadeTime > 0;

		public bool IsDisarmed()
			=> m_disarmed;

		public void RepairTrap()
		{
			m_disarmed = false;
			m_fadeTime = 0;
		}

		public void DisarmTrap()
		{
			m_disarmed = true;
			m_fadeTime = 0;
		}

		public void SetUpgradeLevel(int upgLevel)
		{
			LogicTrapData data = GetTrapData();

			m_upgLevel = LogicMath.Clamp(upgLevel, 0, data.GetUpgradeLevelCount() - 1);
			m_numSpawns = data.GetNumSpawns(m_upgLevel);
		}

		public void CreateProjectile(LogicProjectileData data)
		{
			LogicTrapData trapData = GetTrapData();

			LogicVector2 position = new LogicVector2();
			LogicArrayList<LogicGameObject> characters = GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER);

			LogicGameObject closestGameObject = null;

			for (int i = 0, minDistance = 0; i < characters.Size(); i++)
			{
				LogicCharacter character = (LogicCharacter)characters[i];
				LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

				if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
				{
					if (character.IsFlying() && character.IsAlive())
					{
						int housingSpace = character.GetCharacterData().GetHousingSpace();

						if (housingSpace >= trapData.GetMinTriggerHousingLimit() && character.GetChildTroops() == null)
						{
							if (trapData.GetHealerTrigger() || character.GetCombatComponent() == null || !character.GetCombatComponent().IsHealer())
							{
								position.m_x = character.GetPosition().m_x - GetMidX();
								position.m_y = character.GetPosition().m_y - GetMidY();

								int lengthSquared = position.GetLengthSquared();

								if (minDistance == 0 || lengthSquared < minDistance)
								{
									minDistance = lengthSquared;
									closestGameObject = character;
								}
							}
						}
					}
				}
			}

			position.Destruct();

			if (closestGameObject != null)
			{
				LogicProjectile projectile = (LogicProjectile)LogicGameObjectFactory.CreateGameObject(data, m_level, m_villageType);

				projectile.SetInitialPosition(null, GetMidX(), GetMidY());
				projectile.SetTarget(GetMidX(), GetMidY(), 0, closestGameObject, data.GetRandomHitPosition());
				projectile.SetDamage(trapData.GetDamage(m_upgLevel));
				projectile.SetDamageRadius(trapData.GetDamageRadius(m_upgLevel));
				projectile.SetPushBack(trapData.GetPushback(), !trapData.GetDoNotScalePushByDamage());
				projectile.SetMyTeam(1);
				projectile.SetHitEffect(trapData.GetDamageEffect(), null);

				GetGameObjectManager().AddGameObject(projectile, -1);
			}
		}

		public void EjectCharacters()
		{
			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

			int ejectHousingSpace = GetTrapData().GetEjectHousingLimit(m_upgLevel);
			int radius = GetTrapData().GetTriggerRadius();

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();

				if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)parent;

					if (character.GetHitpointComponent() != null && character.GetHitpointComponent().GetTeam() == 0 &&
						character.GetCharacterData().GetHousingSpace() <= ejectHousingSpace)
					{
						int distanceX = character.GetX() - GetMidX();
						int distanceY = character.GetY() - GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius)
						{
							if (character.GetCombatComponent() == null || character.GetCombatComponent().GetUndergroundTime() <= 0)
							{
								int distanceSquared = distanceX * distanceX + distanceY * distanceY;

								if (distanceSquared < (uint)(radius * radius))
								{
									character.Eject(null);
									ejectHousingSpace -= character.GetCharacterData().GetHousingSpace();
								}
							}
						}
					}
				}
			}
		}

		public void ThrowCharacters()
		{
			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

			int ejectHousingSpace = GetTrapData().GetEjectHousingLimit(m_upgLevel);
			int radius = GetTrapData().GetTriggerRadius();

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();

				if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)parent;

					if (character.GetHitpointComponent() != null && character.GetHitpointComponent().GetTeam() == 0 &&
						character.GetCharacterData().GetHousingSpace() <= ejectHousingSpace)
					{
						int distanceX = character.GetX() - GetMidX();
						int distanceY = character.GetY() - GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius)
						{
							if (character.GetCombatComponent() == null || character.GetCombatComponent().GetUndergroundTime() <= 0)
							{
								int distanceSquared = distanceX * distanceX + distanceY * distanceY;

								if (distanceSquared < (uint)(radius * radius))
								{
									int activeLayout = m_level.GetActiveLayout();
									int direction = activeLayout <= 7 ? m_direction[activeLayout] : 0;
									int pushBackX = 0;
									int pushBackY = 0;

									switch (direction)
									{
										case 0:
											pushBackX = 256;
											break;
										case 1:
											pushBackY = 256;
											break;
										case 2:
											pushBackX = -256;
											break;
										case 3:
											pushBackY = -256;
											break;
									}

									m_level.AreaPushBack(GetMidX(), GetMidY(), 600, 1000, 1, 1, pushBackX, pushBackY, GetTrapData().GetThrowDistance(),
															  ejectHousingSpace);
								}
							}
						}
					}
				}
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
							FinishConstruction(true);
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
							FinishConstruction(true);
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

		public override bool IsPassable()
			=> true;

		public override void Tick()
		{
			base.Tick();

			if (m_constructionTimer != null)
			{
				if (m_level.GetRemainingClockTowerBoostTime() > 0 &&
					m_data.GetVillageType() == 1)
				{
					m_constructionTimer.SetFastForward(m_constructionTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
				}

				if (m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
				{
					FinishConstruction(false);
				}
			}

			if (m_disarmed)
			{
				if (m_fadeTime >= 0)
				{
					m_fadeTime = LogicMath.Min(m_fadeTime + 64, 1000);
				}
			}

			LogicTriggerComponent triggerComponent = GetTriggerComponent();

			if (triggerComponent.IsTriggered() && !m_disarmed && !m_upgrading)
			{
				LogicTrapData data = GetTrapData();

				if (m_numSpawns > 0)
				{
					if (m_spawnInitDelay != 0)
					{
						m_spawnInitDelay -= 1;
					}
					else
					{
						SpawnUnit(1);
						m_numSpawns -= 1;
						m_spawnInitDelay = GetTrapData().GetTimeBetweenSpawnsMS() / 64;
					}
				}

				if (m_actionTime >= 0)
				{
					m_actionTime += 64;
				}

				if (m_hitTime >= 0)
				{
					m_hitTime += 64;
				}

				if (m_actionTime > data.GetActionFrame())
				{
					m_hitTime = data.GetHitDelayMS();
					m_actionTime = -1;
				}
				else if (m_hitTime > data.GetHitDelayMS())
				{
					if (data.GetSpell() != null)
					{
						LogicSpell spell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(data.GetSpell(), m_level, m_villageType);

						spell.SetUpgradeLevel(0);
						spell.SetInitialPosition(GetMidX(), GetMidY());
						spell.SetTeam(1);

						GetGameObjectManager().AddGameObject(spell, -1);
					}
					else if (data.GetProjectile(m_upgLevel) != null)
					{
						CreateProjectile(data.GetProjectile(m_upgLevel));
					}
					else if (data.GetDamageMod() != 0)
					{
						m_level.AreaBoost(GetMidX(), GetMidY(), data.GetDamageRadius(m_upgLevel), -data.GetSpeedMod(), -data.GetSpeedMod(), data.GetDamageMod(),
											   0, data.GetDurationMS() / 16, 0, false);
					}
					else if (data.GetEjectVictims())
					{
						if (data.GetThrowDistance() <= 0)
						{
							EjectCharacters();
						}
						else
						{
							ThrowCharacters();
						}
					}
					else
					{
						bool defaultMode = true;

						if (data.GetSpawnedCharAir() != null && data.GetSpawnedCharGround() != null || data.HasAlternativeMode())
						{
							int activeLayout = m_level.GetActiveLayout();

							if (activeLayout <= 7)
							{
								defaultMode = m_useAirMode[activeLayout] ^ true;
							}
						}

						m_level.AreaDamage(0, GetMidX(), GetMidY(), data.GetDamageRadius(m_upgLevel), data.GetDamage(m_upgLevel),
												data.GetPreferredTarget(),
												data.GetPreferredTargetDamageMod(), data.GetDamageEffect(), 1, null, defaultMode ? 1 : 0, 0, 100, true, false, 100, 0, this, 100,
												0);
					}

					m_hitTime = 0;
					m_hitCount += 1;

					if (m_hitCount >= data.GetHitCount() && m_numSpawns == 0)
					{
						m_fadeTime = 1;
						m_hitTime = -1;
						m_disarmed = true;
						m_numSpawns = data.GetNumSpawns(m_upgLevel);
						m_spawnInitDelay = data.GetSpawnInitialDelayMS() / 64;
					}
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

			if (m_disarmed && GetTrapData().GetVillageType() != 1)
			{
				jsonObject.Put("needs_repair", new LogicJSONBoolean(true));
			}

			LogicTrapData data = GetTrapData();

			if (data.HasAlternativeMode() || data.GetSpawnedCharAir() != null && data.GetSpawnedCharGround() != null)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						jsonObject.Put(layoutComponent.GetLayoutVariableNameAirMode(i, false), new LogicJSONBoolean(m_useAirMode[i]));
						jsonObject.Put(layoutComponent.GetLayoutVariableNameAirMode(i, true), new LogicJSONBoolean(m_draftUseAirMode[i]));
					}
				}
			}

			if (data.GetDirectionCount() > 0)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						jsonObject.Put(layoutComponent.GetLayoutVariableNameTrapDirection(i, false), new LogicJSONNumber(m_direction[i]));
						jsonObject.Put(layoutComponent.GetLayoutVariableNameTrapDirection(i, true), new LogicJSONNumber(m_draftDirection[i]));
					}
				}
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

			if (m_constructionTimer != null)
			{
				jsonObject.Put("const_t", new LogicJSONNumber(m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime())));
			}

			LogicTrapData data = GetTrapData();

			if (data.HasAlternativeMode() || data.GetSpawnedCharAir() != null && data.GetSpawnedCharGround() != null)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						jsonObject.Put(layoutComponent.GetLayoutVariableNameAirMode(i, false), new LogicJSONBoolean(m_useAirMode[i]));
					}
				}
			}

			if (data.GetDirectionCount() > 0)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						jsonObject.Put(layoutComponent.GetLayoutVariableNameTrapDirection(i, false), new LogicJSONNumber(m_direction[i]));
					}
				}
			}

			base.SaveToSnapshot(jsonObject, layoutId);
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LoadUpgradeLevel(jsonObject);

			LogicTrapData data = GetTrapData();

			if (data.HasAlternativeMode() || data.GetSpawnedCharAir() != null && data.GetSpawnedCharGround() != null)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						LogicJSONBoolean airModeObject = jsonObject.GetJSONBoolean(layoutComponent.GetLayoutVariableNameAirMode(i, false));

						if (airModeObject != null)
						{
							m_useAirMode[i] = airModeObject.IsTrue();
						}

						LogicJSONBoolean draftAirModeObject = jsonObject.GetJSONBoolean(layoutComponent.GetLayoutVariableNameAirMode(i, true));

						if (draftAirModeObject != null)
						{
							m_draftUseAirMode[i] = draftAirModeObject.IsTrue();
						}
					}
				}

				LogicTriggerComponent triggerComponent = GetTriggerComponent();

				int layoutId = m_level.GetCurrentLayout();
				bool airMode = m_useAirMode[layoutId];

				triggerComponent.SetAirTrigger(airMode);
				triggerComponent.SetGroundTrigger(!airMode);
			}

			if (data.GetDirectionCount() > 0)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						LogicJSONNumber trapDistanceObject = jsonObject.GetJSONNumber(layoutComponent.GetLayoutVariableNameTrapDirection(i, false));

						if (trapDistanceObject != null)
						{
							m_direction[i] = trapDistanceObject.GetIntValue();
						}

						LogicJSONNumber draftTrapDistanceObject = jsonObject.GetJSONNumber(layoutComponent.GetLayoutVariableNameTrapDirection(i, true));

						if (draftTrapDistanceObject != null)
						{
							m_draftDirection[i] = draftTrapDistanceObject.GetIntValue();
						}
					}
				}
			}

			m_level.GetWorkerManagerAt(m_villageType).DeallocateWorker(this);

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
					if (m_upgLevel < data.GetUpgradeLevelCount() - 1)
					{
						constTime = LogicMath.Min(constTime, data.GetBuildTime(m_upgLevel + 1));
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

				m_level.GetWorkerManagerAt(m_villageType).AllocateWorker(this);
				m_upgrading = m_upgLevel != -1;
			}

			LogicJSONBoolean disarmed = jsonObject.GetJSONBoolean("needs_repair");

			if (disarmed != null)
			{
				m_disarmed = disarmed.IsTrue();
			}

			SetUpgradeLevel(m_upgLevel);
			base.Load(jsonObject);
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			if (m_data.GetVillageType() == 1)
			{
				Load(jsonObject);
				return;
			}

			LogicTrapData data = GetTrapData();

			LoadUpgradeLevel(jsonObject);
			m_level.GetWorkerManagerAt(m_villageType).DeallocateWorker(this);

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			if (data.HasAlternativeMode() || data.GetSpawnedCharAir() != null && data.GetSpawnedCharGround() != null)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						LogicJSONBoolean airModeObject = jsonObject.GetJSONBoolean(layoutComponent.GetLayoutVariableNameAirMode(i, false));

						if (airModeObject != null)
						{
							m_useAirMode[i] = airModeObject.IsTrue();
						}
					}
				}

				LogicTriggerComponent triggerComponent = GetTriggerComponent();

				bool airMode = m_useAirMode[m_level.GetWarLayout()];

				triggerComponent.SetAirTrigger(airMode);
				triggerComponent.SetGroundTrigger(!airMode);
			}

			if (data.GetDirectionCount() > 0)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

				if (layoutComponent != null)
				{
					for (int i = 0; i < 8; i++)
					{
						LogicJSONNumber trapDistanceObject = jsonObject.GetJSONNumber(layoutComponent.GetLayoutVariableNameTrapDirection(i, false));

						if (trapDistanceObject != null)
						{
							m_direction[i] = trapDistanceObject.GetIntValue();
						}
					}
				}
			}

			m_level.GetWorkerManagerAt(m_data.GetVillageType()).DeallocateWorker(this);

			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			SetUpgradeLevel(m_upgLevel);
			base.LoadFromSnapshot(jsonObject);
		}

		public void LoadUpgradeLevel(LogicJSONObject jsonObject)
		{
			LogicJSONNumber lvlObject = jsonObject.GetJSONNumber("lvl");
			LogicTrapData data = GetTrapData();

			if (lvlObject != null)
			{
				m_upgLevel = lvlObject.GetIntValue();
				int maxLvl = data.GetUpgradeLevelCount();

				if (m_upgLevel >= maxLvl)
				{
					Debugger.Warning(string.Format("LogicTrap::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}",
												   lvlObject.GetIntValue(),
												   maxLvl,
												   m_globalId,
												   data.GetGlobalID()));
					m_upgLevel = maxLvl - 1;
				}
				else
				{
					if (m_upgLevel < -1)
					{
						Debugger.Error("LogicTrap::load() - Loaded an illegal upgrade level!");
					}
				}
			}
		}

		public override void LoadingFinished()
		{
			base.LoadingFinished();

			if (LogicDataTables.GetGlobals().ClampBuildingTimes())
			{
				if (m_constructionTimer != null)
				{
					if (m_upgLevel < GetTrapData().GetUpgradeLevelCount() - 1)
					{
						int remainingSecs = m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());
						int totalSecs = GetTrapData().GetBuildTime(m_upgLevel + 1);

						if (remainingSecs > totalSecs)
						{
							m_constructionTimer.StartTimer(totalSecs, m_level.GetLogicTime(), true,
																m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
						}
					}
				}
			}

			if (m_listener != null)
			{
				m_listener.LoadedFromJSON();
			}
		}

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.TRAP;

		public override int GetWidthInTiles()
			=> GetTrapData().GetWidth();

		public override int GetHeightInTiles()
			=> GetTrapData().GetHeight();

		public int GetUpgradeLevel()
			=> m_upgLevel;

		public bool IsConstructing()
			=> m_constructionTimer != null;

		public bool IsUpgrading()
			=> m_constructionTimer != null && m_upgrading;

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

		public void StartUpgrading()
		{
			if (m_constructionTimer != null)
			{
				m_constructionTimer.Destruct();
				m_constructionTimer = null;
			}

			LogicTrapData data = GetTrapData();

			m_upgrading = true;
			int buildTime = data.GetBuildTime(m_upgLevel + 1);

			if (buildTime <= 0)
			{
				FinishConstruction(false);
			}
			else
			{
				m_level.GetWorkerManagerAt(data.GetVillageType()).AllocateWorker(this);

				m_constructionTimer = new LogicTimer();
				m_constructionTimer.StartTimer(buildTime, m_level.GetLogicTime(), true, m_level.GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
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

					LogicTrapData data = GetTrapData();
					LogicResourceData buildResourceData = data.GetBuildResource();
					int buildCost = data.GetBuildCost(upgLevel);
					int refundedCount = LogicMath.Max(LogicDataTables.GetGlobals().GetBuildCancelMultiplier() * buildCost / 100, 0);

					homeOwnerAvatar.CommodityCountChangeHelper(0, buildResourceData, refundedCount);

					m_level.GetWorkerManagerAt(m_data.GetVillageType()).DeallocateWorker(this);

					if (upgLevel != 0)
					{
						if (m_listener != null)
						{
							m_listener.RefreshState();
						}
					}
					else
					{
						GetGameObjectManager().RemoveGameObject(this);
					}
				}
			}
		}

		public int GetRequiredTownHallLevelForUpgrade()
			=> GetTrapData().GetRequiredTownHallLevel(LogicMath.Min(m_upgLevel + 1, GetTrapData().GetUpgradeLevelCount() - 1));

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

		public bool IsMaxUpgradeLevel()
		{
			LogicTrapData trapData = GetTrapData();
			if (trapData.GetVillageType() != 1 || GetRequiredTownHallLevelForUpgrade() < m_level.GetGameMode().GetConfiguration().GetMaxTownHallLevel())
			{
				return m_upgLevel >= trapData.GetUpgradeLevelCount() - 1;
			}

			return true;
		}

		public bool SpeedUpConstruction()
		{
			if (m_constructionTimer != null)
			{
				LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();

				int remainingSecs = m_constructionTimer.GetRemainingSeconds(m_level.GetLogicTime());
				int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(remainingSecs, 0, m_villageType);

				if (playerAvatar.HasEnoughDiamonds(speedUpCost, true, m_level))
				{
					playerAvatar.UseDiamonds(speedUpCost);
					playerAvatar.GetChangeListener().DiamondPurchaseMade(4, m_data.GetGlobalID(), m_upgLevel + (m_upgrading ? 2 : 1), speedUpCost,
																		 m_level.GetVillageType());

					FinishConstruction(false);

					return true;
				}
			}

			return false;
		}

		public void SpawnUnit(int count)
		{
			LogicTrapData data = GetTrapData();
			LogicCharacterData spawnData = m_useAirMode[m_level.GetActiveLayout(m_villageType)] ? data.GetSpawnedCharAir() : data.GetSpawnedCharGround();

			if (spawnData != null)
			{
				LogicVector2 position = new LogicVector2();

				for (int i = 0, j = 59, k = 0, l = 0; i < count; i++, j += 59, k += 128, l += 360)
				{
					int random = l / data.GetNumSpawns(m_upgLevel) + j * m_numSpawns % 360;
					int randomX = (byte)(k & 0x80) ^ 0x180;
					int posX = GetMidX() + LogicMath.GetRotatedX(randomX, 0, random);
					int posY = GetMidY() + LogicMath.GetRotatedY(randomX, 0, random);

					if (spawnData.IsFlying())
					{
						position.m_x = posX;
						position.m_y = posY;
					}
					else
					{
						if (!m_level.GetTileMap().GetNearestPassablePosition(posX, posY, position, 1536))
						{
							continue;
						}
					}

					LogicCharacter character = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(spawnData, m_level, m_villageType);

					character.GetHitpointComponent().SetTeam(1);
					character.GetMovementComponent().EnableJump(3600000);
					character.SetInitialPosition(position.m_x, position.m_y);
					character.SetSpawnTime(200);

					GetGameObjectManager().AddGameObject(character, -1);
				}

				position.Destruct();
			}
		}

		public bool IsAirMode(int layout, bool draft)
		{
			if (draft)
			{
				return m_draftUseAirMode[layout];
			}

			return m_useAirMode[layout];
		}

		public void ToggleAirMode(int layout, bool draft)
		{
			bool[] array = m_useAirMode;

			if (draft)
			{
				array = m_draftUseAirMode;
			}

			array[layout] ^= true;
		}

		public void ToggleDirection(int layout, bool draft)
		{
			int[] array = m_direction;

			if (draft)
			{
				array = m_draftDirection;
			}

			int direction = 0;

			if (array[layout] + 1 < GetTrapData().GetDirectionCount())
			{
				direction = array[layout] + 1;
			}

			array[layout] = direction;
		}

		public void SetDirection(int layout, bool draft, int count)
		{
			int[] array = m_direction;

			if (draft)
			{
				array = m_draftDirection;
			}

			array[layout] = count;
		}

		public int GetDirection(int layout, bool draft)
		{
			if (layout <= 7)
			{
				if (draft)
				{
					return m_draftDirection[layout];
				}

				return m_direction[layout];
			}

			return 0;
		}

		public bool HasAirMode()
		{
			LogicTrapData data = (LogicTrapData)m_data;

			if (data.GetSpawnedCharAir() == null || data.GetSpawnedCharGround() == null)
			{
				return data.HasAlternativeMode();
			}

			return true;
		}
	}
}