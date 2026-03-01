using System;

using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

using Debugger = Supercell.Magic.Titan.Debug.Debugger;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicCombatComponent : LogicComponent
	{
		private LogicAttackerItemData m_attackerData;
		private readonly LogicComponentFilter m_enemyFilter; // 48
		private readonly LogicComponentFilter m_groupEnemyFilter; // 616
		private LogicGameObject m_originalTarget; // 44
		private readonly LogicArrayList<LogicGameObject> m_enemyList; // 416
		private readonly LogicRandom m_random; // 484
		private LogicData m_preferredTarget; // 428
		private readonly LogicVector2 m_rangePosition; // 88
		private readonly LogicVector2 m_unk604;
		private readonly LogicVector2 m_targetGroupPosition; // 652
		private readonly LogicVector2 m_unk660;
		private readonly LogicTargetList m_preferredTargetList; // 104
		private readonly LogicTargetList m_targetList; // 192
		private LogicGameObject m_targetGroup; // 612

		private readonly LogicArrayList<LogicGameObject> m_targetGroups; // 640
		private readonly LogicArrayList<LogicGameObject> m_targetGroupEnemyList; // 668
		private readonly LogicArrayList<int> m_targetGroupWeights; // 692
		private readonly LogicArrayList<int> m_groupWeights; // 680
		private readonly LogicArrayList<int> m_targetGroupWeightMultiplier; // 704

		private int[] m_healerTargetWeights; // 488

		private bool m_readyForAttack; // 496
		private readonly bool m_attackMultipleBuildings; // 500
		private bool m_hasAltAttackMode; // 498
		private bool m_attackWithGroups; // 598
		private bool m_groupNoDamageMod; // 597
		private bool m_altMultiTargets; // 499
		private bool m_preferredTargetNotTargeting; // 436
		private bool m_spawnOnAttack; // 503
		private bool m_troopChild;
		private bool m_skeletonSpell; // 599
		private bool m_alerted; // 438
		private bool m_unk497;
		private bool m_unk502;
		private bool m_unk504;
		private bool m_unk596;

		private readonly bool[] m_forceNewTarget; // 505
		private readonly bool[] m_useAltAttackMode; // 510
		private readonly bool[] m_draftUseAltAttackMode;

		private readonly int[] m_boostTime; // 448
		private readonly int[] m_boostDamage; // 456

		private readonly int[] m_attackDelay; // 356
		private readonly int[] m_hitTime; // 284
		private readonly int[] m_burstTime;
		private readonly int[] m_forgetTargetTime;
		private readonly int[] m_attackCooldownOverride; // 304
		private readonly int[] m_hideTime; // 396
		private readonly int[] m_damage2Time; // 724
		private readonly int[] m_damage2X; // 744
		private readonly int[] m_damage2Y; // 764
		private readonly int[] m_aimAngle;
		private readonly int[] m_draftAimAngle;

		private readonly LogicGameObject[] m_targets; // 24

		private int m_ammo;
		private int m_damage;
		private int m_targetGroupsRadius; // 592
		private int m_prepareSpeed; // 344
		private int m_preferredTargetDamageMod;
		private int m_wakeUpTime;
		private int m_undergroundTime; // 492
		private int m_hitCount; // 280
		private int m_activationTime;
		private int m_targetCount; // 20
		private int m_searchRadius; // 72
		private int m_damageTime; // 16
		private int m_damageLevelEffect; // 80
		private int m_summonCooldownTime; // 352
		private int m_totalTargets; // 444
		private int m_ammoReloadingTotalTime; // 476
		private int m_ammoReloadingTime; // 480
		private int m_mergeDamage;
		private int m_deployedHousingSpace; // 788
		private int m_slowTime; // 464
		private int m_slowDamage; // 468
		private int m_attackSpeedBoost; // 716
		private int m_attackSpeedBoostTime; // 720
		private int m_alertTime; // 440
		private int m_originalForgetTargetTime; // 600

		public LogicCombatComponent(LogicGameObject gameObject) : base(gameObject)
		{
			m_targetCount = 1;

			m_targets = new LogicGameObject[8];

			m_boostTime = new int[2];
			m_boostDamage = new int[2];

			m_forceNewTarget = new bool[8];
			m_attackDelay = new int[8];
			m_hitTime = new int[8];
			m_burstTime = new int[8];
			m_forgetTargetTime = new int[8];
			m_attackCooldownOverride = new int[8];
			m_hideTime = new int[8];
			m_damage2Time = new int[8];
			m_damage2X = new int[8];
			m_damage2Y = new int[8];

			for (int i = 0; i < 8; i++)
			{
				m_damage2X[i] = -1;
				m_damage2Y[i] = -1;
			}

			m_aimAngle = new int[8];
			m_draftAimAngle = new int[8];
			m_useAltAttackMode = new bool[8];
			m_draftUseAltAttackMode = new bool[8];

			m_unk502 = true;

			m_rangePosition = new LogicVector2();
			m_unk604 = new LogicVector2(-1, -1);
			m_targetGroupPosition = new LogicVector2(-1, -1);
			m_unk660 = new LogicVector2(-1, -1);
			m_preferredTargetList = new LogicTargetList();
			m_targetList = new LogicTargetList();
			m_enemyFilter = new LogicComponentFilter();
			m_groupEnemyFilter = new LogicComponentFilter();
			m_random = new LogicRandom();

			m_targetGroups = new LogicArrayList<LogicGameObject>();
			m_targetGroupEnemyList = new LogicArrayList<LogicGameObject>();
			m_targetGroupWeights = new LogicArrayList<int>();
			m_groupWeights = new LogicArrayList<int>();
			m_targetGroupWeightMultiplier = new LogicArrayList<int>();

			m_enemyFilter.SetComponentType(LogicComponentType.HITPOINT);
			m_groupEnemyFilter.SetComponentType(LogicComponentType.HITPOINT);

			if (gameObject.GetHitpointComponent() == null)
			{
				Debugger.Error("LogicCombatComponent::constructor - Enemy filter works only if Hitpoint component is initialized!");
			}

			m_enemyFilter.PassEnemyOnly(gameObject);
			m_enemyList = new LogicArrayList<LogicGameObject>(20);

			if (m_parent.GetData().GetDataType() == DataType.CHARACTER)
			{
				LogicCharacterData characterData = (LogicCharacterData)m_parent.GetData();

				if (characterData.IsSecondaryTroop())
				{
					m_totalTargets = 1;
				}

				m_attackMultipleBuildings = characterData.GetAttackMultipleBuildings();
			}

			m_preferredTargetDamageMod = 100;
			m_random.SetIteratedRandomSeed(5512);
		}

		public override void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			base.RemoveGameObjectReferences(gameObject);

			for (int i = 0; i < m_targetCount; i++)
			{
				if (m_targets[0] == gameObject || m_originalTarget == gameObject)
				{
					m_targets[i] = null;
					m_hitTime[i] = 0;

					if (m_burstTime[i] != 0)
					{
						m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
					}

					m_burstTime[i] = 0;
					m_originalTarget = null;

					if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)m_parent;

						if (character.GetCharacterData().IsUnderground())
						{
							m_hideTime[0] = LogicDataTables.GetGlobals().GetMinerHideTime() + m_parent.Rand(0) % LogicDataTables.GetGlobals().GetMinerHideTimeRandom();
						}
					}
				}
			}

			for (int i = 0; i < m_enemyList.Size(); i++)
			{
				if (m_enemyList[i] == gameObject)
				{
					m_enemyList.Remove(i--);
				}
			}

			for (int i = 0; i < m_targetGroups.Size(); i++)
			{
				if (m_targetGroups[i] == gameObject)
				{
					m_targetGroups.Remove(i--);
				}
			}

			if (m_targetGroup == gameObject)
			{
				m_targetGroup = null;
			}
		}

		public void SetPreferredTarget(LogicData data, int preferredTargetDamageMod, bool preferredTargetNotTargeting)
		{
			m_preferredTarget = data;
			m_preferredTargetDamageMod = preferredTargetDamageMod;
			m_preferredTargetNotTargeting = preferredTargetNotTargeting;
		}

		public void SetAttackValues(LogicAttackerItemData data, int damagePercentage)
		{
			m_attackerData = data;
			m_prepareSpeed = data.GetPrepareSpeed();
			m_damage = damagePercentage * data.GetDamage(0, data.GetMultiTargets(false)) / 100;
			m_preferredTarget = data.GetPreferredTargetData();
			m_preferredTargetDamageMod = 100 * data.GetPreferredTargetDamageMod();
			m_preferredTargetNotTargeting = data.GetPreferredTargetNoTargeting();

			m_unk502 = true;

			m_summonCooldownTime = data.GetSummonCooldown() / 4;
			m_wakeUpTime = data.GetWakeUpSpeed();

			m_spawnOnAttack = data.GetSpawnOnAttack();

			if (m_attackerData.GetAttackSpeed() < 64)
			{
				Debugger.Error(m_parent.GetData().GetName() + " has too fast attack speed!");
			}

			if (m_attackerData.GetFightWithGroups())
			{
				if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					m_attackWithGroups = true;
					m_targetGroupsRadius = m_attackerData.GetTargetGroupsRadius();
					m_groupNoDamageMod = true;
				}
			}

			if (m_attackerData.HasAlternativeAttackMode())
			{
				m_hasAltAttackMode = true;
				m_altMultiTargets = m_attackerData.GetMultiTargets(true);
				m_targetCount = m_altMultiTargets ? m_attackerData.GetAltNumMultiTargets() : 1;
			}

			if (m_attackerData.GetAmmoCount() > 0)
			{
				int attackSpeed = m_attackerData.GetAttackSpeed();
				int unk = 1;

				if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
				{
					attackSpeed = m_attackerData.GetAltAttackSpeed();
				}

				if (attackSpeed >= 64)
				{
					unk = attackSpeed / 64;
				}

				m_ammoReloadingTotalTime = unk;
				m_ammoReloadingTime = unk;
			}

			if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0)
			{
				m_enemyFilter.PassEnemyOnly(m_parent);
			}
			else
			{
				if (m_attackerData.GetDamage2() > 0)
				{
					m_enemyFilter.PassEnemyOnly(m_parent);
				}
				else
				{
					m_enemyFilter.PassFriendlyOnly(m_parent);
				}
			}
		}

		public void SetAttackDelay(int idx, int time)
		{
			m_attackDelay[idx] = time;

			m_targets[idx] = null;
			m_originalTarget = null;
		}

		public LogicData GetPreferredTarget()
			=> m_preferredTarget;

		public void SetActivationTime(int value)
		{
			m_activationTime = value;
		}

		public void SetTroopChild(bool child)
		{
			m_troopChild = child;
		}

		public void SetSearchRadius(int radius)
		{
			m_searchRadius = radius;
		}

		public int Rand(int rnd)
			=> m_parent.Rand(rnd);

		public LogicGameObject GetTarget(int idx)
			=> m_targets[idx];

		public bool IsTargetValid(LogicGameObject gameObject)
		{
			if (gameObject != null)
			{
				LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

				if (combatComponent != null && combatComponent.m_undergroundTime > 0)
				{
					if (m_damage > 0 || m_attackerData.GetDamage2() > 0 || m_attackerData.GetShockwavePushStrength() != 0)
					{
						return false;
					}
				}

				LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

				if (m_damage > 0 || m_attackerData.GetDamage2() > 0 || m_attackerData.GetShockwavePushStrength() != 0)
				{
					if (hitpointComponent != null && hitpointComponent.IsEnemy(m_parent))
					{
						if (m_parent.GetMovementComponent() == null)
						{
							return IsInRange(gameObject);
						}

						return true;
					}
				}
				else
				{
					if (hitpointComponent != null && hitpointComponent.GetTeam() == m_parent.GetHitpointComponent().GetTeam())
					{
						return gameObject.IsAlive();
					}
				}
			}

			return false;
		}

		public LogicAttackerItemData GetAttackerItemData()
			=> m_attackerData;

		public bool HasAltAttackMode()
			=> m_hasAltAttackMode;

		public bool UseAltAttackMode(int layoutId, bool draft)
		{
			if (draft)
			{
				return m_draftUseAltAttackMode[layoutId];
			}

			return m_useAltAttackMode[layoutId];
		}

		public bool IsInRange(LogicGameObject gameObject)
		{
			int activeLayout = m_parent.GetLevel().GetCurrentLayout();
			int aimAngle = 0;

			if (m_attackerData.GetTargetingConeAngle() != 0)
			{
				aimAngle = m_aimAngle[activeLayout];
			}

			CalculateDistance(gameObject, m_rangePosition);

			int distance = m_rangePosition.m_x;
			int angle = m_rangePosition.m_y;

			if (m_parent.IsFlying() ||
				gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				int minAttackRange = m_attackerData.GetMinAttackRange();

				if (distance < minAttackRange * minAttackRange)
					return false;

				int attackRange = GetAttackRange(activeLayout, false);

				if (distance > (attackRange + 256) * (attackRange + 256))
					return false;
			}
			else
			{
				int attackRange = GetAttackRange(activeLayout, false);

				if (distance >= attackRange * attackRange)
					return false;
			}

			if (m_attackerData.GetTargetingConeAngle() > 0)
			{
				int attackAngle = LogicMath.GetAngleBetween(angle, LogicMath.NormalizeAngle180(aimAngle));

				if (attackAngle > m_attackerData.GetTargetingConeAngle() / 2)
					return false;
			}

			return true;
		}

		public bool IsInLine(LogicGameObject gameObject)
		{
			if (gameObject != null)
			{
				if (gameObject.IsWall() || m_parent.GetGameObjectType() != LogicGameObjectType.CHARACTER)
				{
					return true;
				}

				LogicCharacter parent = (LogicCharacter)m_parent;

				if (!parent.GetCharacterData().GetAttackOverWalls())
				{
					return !m_parent.GetLevel().GetTileMap()
								.GetWallInPassableLine(m_parent.GetMidX(), m_parent.GetMidY(), gameObject.GetMidX(), gameObject.GetMidY(), new LogicVector2());
				}
			}

			return true;
		}

		public void CalculateDistance(LogicGameObject gameObject, LogicVector2 position)
		{
			int midX = m_parent.GetMidX();
			int midY = m_parent.GetMidY();
			int distance;
			int angle = 0;

			if (m_parent.IsFlying() ||
				gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				distance = m_parent.GetDistanceSquaredTo(gameObject);

				if (m_attackerData.GetTargetingConeAngle() > 0)
				{
					angle = LogicMath.NormalizeAngle180(LogicMath.GetAngle(gameObject.GetMidX() - midX, gameObject.GetMidY() - midY));
				}
			}
			else
			{
				int passableSubtilesAtEdge = gameObject.PassableSubtilesAtEdge() << 8;

				int goX = gameObject.GetX();
				int goY = gameObject.GetY();

				int goClampedX = LogicMath.Clamp(midX, goX + passableSubtilesAtEdge, goX + (gameObject.GetWidthInTiles() << 9) - passableSubtilesAtEdge);
				int goClampedY = LogicMath.Clamp(midY, goY + passableSubtilesAtEdge, goY + (gameObject.GetHeightInTiles() << 9) - passableSubtilesAtEdge);

				distance = (goClampedX - midX) * (goClampedX - midX) + (goClampedY - midY) * (goClampedY - midY);

				if (m_attackerData.GetTargetingConeAngle() > 0)
				{
					angle = LogicMath.NormalizeAngle180(LogicMath.GetAngle(goClampedX - midX, goClampedY - midY));
				}
			}

			position.m_x = distance;
			position.m_y = angle;
		}

		public int GetAttackRange(int layout, bool draft)
		{
			if (draft)
			{
				if (m_draftUseAltAttackMode[layout])
				{
					return m_attackerData.GetAttackRange(true);
				}
			}
			else if (m_useAltAttackMode[layout])
			{
				return m_attackerData.GetAttackRange(true);
			}

			int attackRange = m_attackerData.GetAttackRange(false);

			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;
				LogicCharacterData data = character.GetCharacterData();

				if (character.GetSpecialAbilityAvailable() && data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE)
				{
					attackRange += data.GetSpecialAbilityAttribute2(character.GetUpgradeLevel()) * m_attackerData.GetAttackRange(false) / 100;
				}
			}

			return attackRange;
		}

		public bool IsWallBreaker()
			=> !m_preferredTargetNotTargeting && m_preferredTarget != null && m_preferredTarget.GetDataType() == DataType.BUILDING_CLASS &&
				   ((LogicBuildingClassData)m_preferredTarget).IsWall();

		public void SearchTarget(int idx, LogicGameObject prevTarget)
		{
			if (m_attackWithGroups && SearchTargetWithGroups(idx, prevTarget))
				return;

			if (m_unk504)
			{
				SelectTarget(((LogicCharacter)m_parent).GetParent().GetCombatComponent().m_targets[idx], idx);
				return;
			}

			if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0 ||
				!LogicDataTables.GetGlobals().UseSmarterHealer() && !LogicDataTables.GetGlobals().UseStickToClosestUnitHealer())
			{
				if (m_attackerData.GetPreferredTargetNoTargeting())
				{
					LogicGameObject searchTargetNoTargeting = SearchTargetNoTargeting();

					if (searchTargetNoTargeting != null && IsTargetValid(searchTargetNoTargeting))
					{
						SelectTarget(searchTargetNoTargeting, idx);
						return;
					}
				}

				bool wallBreaker = false;

				if (IsWallBreaker())
				{
					LogicGameObject wall = SelectWall();

					if (wall != null && IsTargetValid(wall))
					{
						SelectTarget(wall, idx);
						return;
					}

					wallBreaker = true;
				}

				m_targetList.Clear();
				m_preferredTargetList.Clear();
				m_enemyList.Clear();

				m_parent.GetGameObjectManager().GetGameObjects(m_enemyList, m_enemyFilter);

				LogicArrayList<LogicGameObject> troopChildTargets = new LogicArrayList<LogicGameObject>();
				LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

				for (int i = 0, cnt = m_enemyList.Size(); i < cnt; i++)
				{
					LogicGameObject gameObject = m_enemyList[i];

					if (!gameObject.IsHidden())
					{
						if (gameObject.IsStealthy())
						{
							if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0)
							{
								continue;
							}
						}

						if (CanAttackHeightCheck(gameObject))
						{
							if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
							{
								LogicCharacter character = (LogicCharacter)gameObject;
								LogicCombatComponent combatComponent;

								if (character.GetSpawnDelay() > 0 || character.GetSpawnIdleTime() > 0 ||
									(combatComponent = character.GetCombatComponent()) != null && combatComponent.m_undergroundTime > 0)
								{
									continue;
								}

								if (character.GetChildTroops() != null)
								{
									continue;
								}
							}

							bool containsTarget = false;

							for (int j = 0; j < m_targetCount; j++)
							{
								if (idx != j)
								{
									if (m_targets[j] == gameObject)
									{
										containsTarget = true;
									}
								}
							}

							if (containsTarget)
							{
								continue;
							}

							int distance;

							if (!LogicDataTables.GetGlobals().MorePreciseTargetSelection() ||
								LogicDataTables.GetGlobals().MovingUnitsUseSimpleSelect() && movementComponent != null)
							{
								int distanceX = (gameObject.GetMidX() - m_parent.GetMidX()) >> 9;
								int distanceY = (gameObject.GetMidY() - m_parent.GetMidY()) >> 9;

								distance = distanceX * distanceX + distanceY * distanceY;
							}
							else
							{
								CalculateDistance(gameObject, m_rangePosition);
								distance = m_rangePosition.m_x;
							}

							if (LogicDataTables.GetGlobals().GetMinerTargetRandomPercentage() > 0 && m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
							{
								LogicCharacter character = (LogicCharacter)gameObject;
								LogicCharacterData data = character.GetCharacterData();

								if (data.IsUnderground())
								{
									if (m_hitCount > 0)
									{
										distance -= distance / 100 * m_random.Rand(LogicDataTables.GetGlobals().GetMinerTargetRandomPercentage());
									}
								}
							}

							if (m_searchRadius > 0 && movementComponent != null)
							{
								LogicBuilding heroBaseBuilding = movementComponent.GetBaseBuilding();

								if (heroBaseBuilding != null)
								{
									m_rangePosition.m_x = gameObject.GetMidX() - heroBaseBuilding.GetMidX();
									m_rangePosition.m_y = gameObject.GetMidY() - heroBaseBuilding.GetMidY();

									if (m_rangePosition.GetLengthSquared() > (m_searchRadius << 9) * (m_searchRadius << 9))
									{
										continue;
									}
								}

								if (!m_parent.IsHero())
								{
									m_rangePosition.m_x = gameObject.GetMidX() - m_parent.GetMidX();
									m_rangePosition.m_y = gameObject.GetMidY() - m_parent.GetMidY();

									int castleDefenderSearchRadius = LogicDataTables.GetGlobals().GetClanDefenderSearchRadius();

									if (m_rangePosition.GetLengthSquared() > (castleDefenderSearchRadius << 9) * (castleDefenderSearchRadius << 9))
									{
										continue;
									}
								}
							}

							if ((m_attackerData.GetMinAttackRange() > 0 || m_attackerData.GetTargetingConeAngle() > 0) && !IsInRange(gameObject))
							{
								continue;
							}

							if (m_damage <= 0 && m_attackerData.GetShockwavePushStrength() == 0 && m_attackerData.GetDamage2() <= 0)
							{
								if (gameObject != m_parent && gameObject.IsAlive() && (wallBreaker || !IsWall(gameObject)))
								{
									LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

									if (hitpointComponent != null)
									{
										if (!hitpointComponent.HasFullHitpoints() || hitpointComponent.IsDamagedRecently())
										{
											m_preferredTargetList.AddCandidate(gameObject, distance);
											continue;
										}
									}

									m_targetList.AddCandidate(gameObject, distance);
								}

								continue;
							}

							if (wallBreaker || !IsWall(gameObject))
							{
								if (m_troopChild)
								{
									if (IsTargetValid(gameObject))
									{
										troopChildTargets.Add(gameObject);
									}

									continue;
								}

								if (IsPreferredTargetForMe(gameObject, distance))
								{
									m_preferredTargetList.AddCandidate(gameObject, distance);
									continue;
								}

								m_targetList.AddCandidate(gameObject, distance);
							}
						}
					}
				}

				LogicGameObject target = m_preferredTargetList.EvaluateTargets(movementComponent);

				if (target != null && movementComponent == null && !IsInRange(target))
				{
					target = null;
				}

				if (troopChildTargets.Size() <= 0)
				{
					if (target == null)
					{
						if (m_preferredTarget != null && m_preferredTarget.GetDataType() == DataType.BUILDING)
						{
							m_preferredTarget = ((LogicBuildingData)m_preferredTarget).GetBuildingClass();
							return;
						}

						target = m_targetList.EvaluateTargets(movementComponent);
					}
				}
				else
				{
					target = troopChildTargets[m_random.Rand(troopChildTargets.Size())];
				}

				SelectTarget(target, idx);
				return;
			}

			LogicGameObject selectedTarget = LogicDataTables.GetGlobals().UseSmarterHealer() ? SearchSmartHealerTarget() : SearchHealerTargetUsingStick();

			if (selectedTarget != null && IsTargetValid(selectedTarget))
			{
				SelectTarget(selectedTarget, idx);
			}
		}

		public LogicGameObject SearchTargetNoTargeting()
		{
			m_targetList.Clear();
			m_preferredTargetList.Clear();
			m_enemyList.Clear();

			m_parent.GetGameObjectManager().GetGameObjects(m_enemyList, m_enemyFilter);

			LogicMovementComponent movementComponent = m_parent.GetMovementComponent();
			LogicGameObject closestTarget = null;

			for (int i = 0, minDistance = 0; i < m_enemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_enemyList[i];

				if (!gameObject.IsHidden())
				{
					if (gameObject.IsStealthy())
					{
						if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0)
						{
							continue;
						}
					}

					if (CanAttackHeightCheck(gameObject))
					{
						int distance;

						if (!LogicDataTables.GetGlobals().MorePreciseTargetSelection() ||
							LogicDataTables.GetGlobals().MovingUnitsUseSimpleSelect() && movementComponent != null)
						{
							int distanceX = (gameObject.GetMidX() - m_parent.GetMidX()) >> 9;
							int distanceY = (gameObject.GetMidY() - m_parent.GetMidY()) >> 9;

							distance = distanceX * distanceX + distanceY * distanceY;
						}
						else
						{
							CalculateDistance(gameObject, m_rangePosition);
							distance = m_rangePosition.m_x;
						}

						if (distance < minDistance || closestTarget == null)
						{
							minDistance = distance;
							closestTarget = gameObject;
						}
					}
				}
			}

			return closestTarget;
		}

		public int IntArrayListToChecksum(LogicArrayList<int> arrayList)
		{
			int checksum = 0;

			for (int i = 0; i < arrayList.Size(); i++)
			{
				checksum += arrayList[i];
			}

			return checksum;
		}

		public bool SearchTargetWithGroups(int idx, LogicGameObject prevTarget)
		{
			if (m_originalForgetTargetTime == 0)
			{
				RefreshTargetGroups(true);
				m_originalForgetTargetTime = LogicDataTables.GetGlobals().GetForgetTargetTime();
			}

			m_targetGroupEnemyList.Clear();
			m_groupWeights.Clear();

			m_targetGroupEnemyList.EnsureCapacity(m_enemyList.Size());
			m_groupWeights.EnsureCapacity(m_enemyList.Size());

			int attackRange = GetAttackRange(m_parent.GetLevel().GetCurrentLayout(), false);
			int attackRangeSquared = attackRange * attackRange;
			int originalTargetDistanceSquared = (attackRange - 512) * (attackRange - 512);
			int maxGroupWeight = 0;

			LogicGameObject groupTarget = null;

			for (int i = 0; i < m_targetGroups.Size(); i++)
			{
				LogicGameObject gameObject = m_targetGroups[i];
				LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

				if (combatComponent != null)
				{
					int groupWeight = 1;

					if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)gameObject;

						groupWeight = m_attackWithGroups
							? character.GetCharacterData().GetFriendlyGroupWeight()
							: character.GetCharacterData().GetEnemyGroupWeight();
					}

					LogicGameObject target = combatComponent.m_targets[0];

					if (target != null)
					{
						if (!target.IsWall() || (target = combatComponent.m_originalTarget) != null &&
							(target.GetDistanceSquaredTo(combatComponent.m_targets[0]) <= originalTargetDistanceSquared ||
							 target.GetDistanceSquaredTo(m_parent) <= attackRangeSquared))
						{
							int gameObjectIdx = m_targetGroupEnemyList.IndexOf(target);

							if (gameObjectIdx == -1)
							{
								m_targetGroupEnemyList.Add(target);
								m_groupWeights.Add(groupWeight);
							}
							else
							{
								groupWeight += m_groupWeights[gameObjectIdx];
								m_groupWeights[gameObjectIdx] = groupWeight;
							}

							if (groupWeight > maxGroupWeight)
							{
								maxGroupWeight = groupWeight;
								groupTarget = target;
							}
						}
					}
				}
			}

			if (groupTarget == null)
			{
				for (int i = 0; i < m_targetGroups.Size(); i++)
				{
					LogicGameObject gameObject = m_targetGroups[i];
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent != null)
					{
						int groupWeight = 1;

						if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)gameObject;

							groupWeight = m_attackWithGroups
								? character.GetCharacterData().GetFriendlyGroupWeight()
								: character.GetCharacterData().GetEnemyGroupWeight();
						}

						LogicGameObject target = combatComponent.m_targets[0];

						if (target != null)
						{
							int gameObjectIdx = m_targetGroupEnemyList.IndexOf(target);

							if (gameObjectIdx == -1)
							{
								m_targetGroupEnemyList.Add(target);
								m_groupWeights.Add(groupWeight);
							}
							else
							{
								groupWeight += m_groupWeights[gameObjectIdx];
								m_groupWeights[gameObjectIdx] = groupWeight;
							}

							if (groupWeight > maxGroupWeight)
							{
								maxGroupWeight = groupWeight;
								groupTarget = target;
							}
						}
					}
				}

				if (groupTarget == null)
				{
					if (prevTarget != null && m_alertTime <= 0 && prevTarget.IsAlive() && IsInRange(prevTarget))
					{
						groupTarget = prevTarget;
					}
				}
			}

			if (groupTarget != null)
			{
				SelectTarget(groupTarget, idx);
				return true;
			}

			return false;
		}

		public LogicGameObject SearchSmartHealerTarget()
		{
			if (m_healerTargetWeights == null)
			{
				m_healerTargetWeights = new int[625];
			}

			Array.Clear(m_healerTargetWeights, 0, 625);

			m_enemyList.Clear();
			m_parent.GetGameObjectManager().GetGameObjects(m_enemyList, m_enemyFilter);

			for (int i = 0; i < m_enemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_enemyList[i];

				if (gameObject.IsAlive() && gameObject.GetData() != m_parent.GetData() && CanAttackHeightCheck(gameObject))
				{
					uint distanceOffsetX = (uint)(gameObject.GetMidX() >> 10);
					uint distanceOffsetY = (uint)(gameObject.GetMidY() >> 10);

					int offset = (int)(distanceOffsetX + 25 * distanceOffsetY);

					if (distanceOffsetX >= 25 || distanceOffsetY >= 25)
					{
						offset = -1;
					}

					if (offset >= 0)
					{
						int housingSpaceWeights = 0;

						if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							housingSpaceWeights = ((LogicCharacter)gameObject).GetCharacterData().GetHousingSpace() / 2 + 1;
						}

						int weight = m_healerTargetWeights[offset] + (housingSpaceWeights << 16);

						LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

						if (hitpointComponent != null)
						{
							if (!hitpointComponent.HasFullHitpoints() || hitpointComponent.IsDamagedRecently())
							{
								weight += housingSpaceWeights;
							}
						}

						m_healerTargetWeights[offset] = weight;
					}
				}
			}

			int currentTargetOffset = -1;

			LogicGameObject currentTarget = m_targets[0];
			LogicGameObject selectTarget = currentTarget;

			if (currentTarget != null)
			{
				uint distanceOffsetX = (uint)(currentTarget.GetMidX() >> 10);
				uint distanceOffsetY = (uint)(currentTarget.GetMidY() >> 10);

				currentTargetOffset = (int)(distanceOffsetX + 25 * distanceOffsetY);

				if (distanceOffsetX >= 25 || distanceOffsetY >= 25)
				{
					currentTargetOffset = -1;
				}
			}

			int minAttackRange = LogicMath.Max(0, LogicMath.GetRadius(-7168, 512) - m_attackerData.GetAttackRange(false));

			if (currentTargetOffset < 0)
			{
				minAttackRange = -((int)((minAttackRange >> 9) + ((uint)((minAttackRange >> 9) * (minAttackRange >> 9)) >> 2)));
			}
			else
			{
				minAttackRange = GetHealerTargetCost(currentTargetOffset) -
								 LogicCombatComponent.GetHealerAttackRange(m_parent.GetMidX(), m_parent.GetMidY(), m_attackerData.GetAttackRange(false),
																		   currentTargetOffset);
			}

			int maxAttackRange = LogicMath.Max(0, LogicMath.GetRadius(-25088, 512) - m_attackerData.GetAttackRange(false));

			maxAttackRange = -((int)((maxAttackRange >> 9) + ((uint)((maxAttackRange >> 9) * (maxAttackRange >> 9)) >> 2)));
			minAttackRange += minAttackRange > 0 ? minAttackRange / 2 + 1 : 1;

			int minOffset = -1;
			int maxOffset = -1;

			for (int i = 0, midX = m_parent.GetMidX(), midY = m_parent.GetMidY(); i < 625; i++)
			{
				int healerAttackRange = LogicCombatComponent.GetHealerAttackRange(midX, midY, m_attackerData.GetAttackRange(false), i);
				int healerTargetCost = GetHealerTargetCost(i);
				int weights = LogicMath.Min(m_healerTargetWeights[i] >> 16, 20);

				if (healerTargetCost > 0)
				{
					int tmp = healerTargetCost - healerAttackRange;

					if (tmp > minAttackRange)
					{
						minOffset = i;
						minAttackRange = tmp;
					}
				}

				if (weights > 1)
				{
					int tmp = weights - healerAttackRange;

					if (tmp > maxAttackRange)
					{
						maxOffset = i;
						maxAttackRange = tmp;
					}
				}
			}

			if (minOffset < 0)
			{
				if (selectTarget != null)
				{
					return selectTarget;
				}

				if (maxOffset < 0)
				{
					return null;
				}

				minOffset = maxOffset;
			}

			int closestDistance = 0x7FFFFFFF;

			for (int i = 0, x = ((minOffset % 25) << 10) | 512, y = ((minOffset / 25) << 10) | 512; i < m_enemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_enemyList[i];

				if (gameObject.IsAlive() && CanAttackHeightCheck(gameObject) && gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					m_rangePosition.m_x = x - gameObject.GetMidX();
					m_rangePosition.m_y = y - gameObject.GetMidY();

					int distanceSquared = m_rangePosition.GetLengthSquared();

					if (distanceSquared < closestDistance)
					{
						closestDistance = distanceSquared;
						selectTarget = gameObject;
					}
				}
			}

			return selectTarget;
		}

		public int GetHealerTargetCost(int weights)
		{
			int x = weights % 25;
			int y = weights / 25;

			uint weights1X = (uint)x;
			uint weights1Y = (uint)(((y << 10) - 1024) >> 10);
			int weights1 = (int)(weights1X + 25 * weights1Y);

			if (weights1X >= 25 || weights1Y >= 25)
			{
				weights1 = -1;
			}

			uint weights2X = (uint)x;
			uint weights2Y = (uint)(((y << 10) + 1024) >> 10);
			int weights2 = (int)(weights2X + 25 * weights2Y);

			if (weights2X >= 25 || weights2Y >= 25)
			{
				weights2 = -1;
			}

			uint weights3X = (uint)(((x << 10) - 1024) >> 10);
			uint weights3Y = (uint)y;
			int weights3 = (int)(weights3X + 25 * weights3Y);

			if (weights3X >= 25 || weights3Y >= 25)
			{
				weights3 = -1;
			}

			uint weights4X = (uint)(((x << 10) + 1024) >> 10);
			uint weights4Y = (uint)y;
			int weights4 = (int)(weights4X + 25 * weights4Y);

			if (weights4X >= 25 || weights4Y >= 25)
			{
				weights4 = -1;
			}

			int cost = (ushort)m_healerTargetWeights[weights];

			if (weights1 >= 0)
			{
				cost += (ushort)m_healerTargetWeights[weights1] >> 2;
			}

			if (weights2 >= 0)
			{
				cost += (m_healerTargetWeights[weights2] >> 2) & 0x3FFF;
			}

			if (weights3 >= 0)
			{
				cost += (m_healerTargetWeights[weights3] >> 2) & 0x3FFF;
			}

			if (weights4 >= 0)
			{
				cost += (m_healerTargetWeights[weights4] >> 2) & 0x3FFF;
			}

			return LogicMath.Min(cost / 2, 20);
		}

		public static int GetHealerAttackRange(int x, int y, int attackRange, int currentTargetWeights)
		{
			int radius = LogicMath.GetRadius((((currentTargetWeights % 25) << 10) | 512) - x, (((currentTargetWeights / 25) << 10) | 512) - y);
			int range = LogicMath.Max(0, radius - attackRange);

			return (int)((range >> 9) + ((uint)((range >> 9) * (range >> 9)) >> 2));
		}

		public LogicGameObject SearchHealerTargetUsingStick()
		{
			m_enemyList.Clear();
			m_parent.GetGameObjectManager().GetGameObjects(m_enemyList, m_enemyFilter);

			LogicGameObject closestTarget = null;

			for (int i = 0, minDistance = 0x7FFFFFFF; i < m_enemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_enemyList[i];

				if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER && gameObject.IsAlive() && CanAttackHeightCheck(gameObject))
				{
					if (gameObject.GetData() != m_parent.GetData() && IsTargetValid(gameObject) && gameObject.GetHitpointComponent() != null)
					{
						m_rangePosition.m_x = m_parent.GetMidX() - gameObject.GetMidX();
						m_rangePosition.m_y = m_parent.GetMidY() - gameObject.GetMidY();

						int lengthSquared = m_rangePosition.GetLengthSquared();

						if (lengthSquared < minDistance)
						{
							minDistance = lengthSquared;
							closestTarget = gameObject;
						}
					}
				}
			}

			return closestTarget;
		}

		public void SelectTarget(LogicGameObject gameObject, int idx)
		{
			if (LogicDataTables.GetGlobals().ClearAlertStateIfNoTargetFound())
			{
				if (idx == 0 && m_alertTime > 0)
				{
					if (gameObject == null || gameObject.GetGameObjectType() != LogicGameObjectType.CHARACTER)
					{
						m_alertTime = 0;
					}
				}
			}

			m_targets[idx] = gameObject;
			m_originalTarget = null;
			m_damageTime = 0;

			if (m_unk502)
			{
				m_damageLevelEffect = 0;
			}

			if (m_targets[idx] != null)
			{
				m_parent.IsHero();
			}

			m_forgetTargetTime[idx] = LogicDataTables.GetGlobals().GetForgetTargetTime();
		}

		public bool IsPreferredTargetForMe(LogicGameObject gameObject, int distance)
		{
			if (m_preferredTarget != null)
			{
				return LogicCombatComponent.IsPreferredTarget(m_preferredTarget, gameObject);
			}

			if (m_alertTime <= 0 || gameObject.GetGameObjectType() != LogicGameObjectType.CHARACTER)
			{
				return false;
			}

			int charVsCharRadius = !LogicDataTables.GetGlobals().MorePreciseTargetSelection() ||
								   LogicDataTables.GetGlobals().MovingUnitsUseSimpleSelect() && m_parent.GetMovementComponent() != null
				? LogicDataTables.GetGlobals().GetCharVersusCharRadiusForAttacker() / 512
				: LogicDataTables.GetGlobals().GetCharVersusCharRadiusForAttacker();

			LogicMovementComponent movementComponent = gameObject.GetMovementComponent();

			if (movementComponent != null)
			{
				if (charVsCharRadius * charVsCharRadius > distance)
				{
					return !movementComponent.GetPatrolEnabled();
				}
			}

			return false;
		}

		public bool GetTrackAirTargets(int layout, bool draft)
			=> m_attackerData.GetTrackAirTargets(draft ? m_draftUseAltAttackMode[layout] : m_useAltAttackMode[layout]);

		public bool GetTrackGroundTargets(int layout, bool draft)
			=> m_attackerData.GetTrackGroundTargets(draft ? m_draftUseAltAttackMode[layout] : m_useAltAttackMode[layout]);

		public int GetTotalTargets()
			=> m_totalTargets;

		public int GetHitCount()
			=> m_hitCount;

		public bool GetAttackMultipleBuildings()
			=> m_attackMultipleBuildings;

		public int GetMaxAmmo()
			=> m_attackerData.GetAmmoCount();

		public void SetUndergroundTime(int ms)
		{
			m_undergroundTime = ms;
		}

		public int GetUndergroundTime()
			=> m_undergroundTime;

		public int GetAmmoCount()
			=> m_ammo;

		public bool UseAmmo()
			=> m_attackerData.GetAmmoCount() > 0;

		public void LoadAmmo()
		{
			int maxAmmo = m_attackerData.GetAmmoCount();

			if (maxAmmo > 0)
			{
				m_ammo = maxAmmo;
			}
		}

		public void RemoveAmmo()
		{
			m_ammo = 0;
		}

		public void ToggleAttackMode(int layout, bool draft)
		{
			if (m_hasAltAttackMode)
			{
				bool[] array = m_useAltAttackMode;

				if (draft)
				{
					array = m_draftUseAltAttackMode;
				}

				array[layout] ^= true;
			}
		}

		public void ToggleAimAngle(int count, int layout, bool draft)
		{
			if (m_attackerData.GetTargetingConeAngle() != 0)
			{
				int[] array = m_aimAngle;

				if (draft)
				{
					array = m_draftAimAngle;
				}

				int angle = array[layout] + count;

				if (angle >= 360)
				{
					angle -= 360;
				}

				if (angle < 0)
				{
					angle += 360;
				}

				array[layout] = angle;
			}
		}

		public int GetAimAngle(int layout, bool draft)
		{
			if (m_attackerData.GetTargetingConeAngle() != 0)
			{
				if (draft)
				{
					return m_draftAimAngle[layout];
				}

				return m_aimAngle[layout];
			}

			return 0;
		}

		public void ForceNewTarget()
		{
			if (m_preferredTarget == null || m_preferredTarget.GetDataType() != DataType.BUILDING_CLASS ||
				!((LogicBuildingClassData)m_preferredTarget).IsWall())
			{
				m_forceNewTarget[0] = true;
			}
		}

		public void UpdateSelectedTargetGroup()
		{
			LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

			if (movementComponent != null)
			{
				if (m_unk660.m_x == -1 && m_unk660.m_y == -1)
				{
					m_unk660.m_x = m_parent.GetX();
					m_unk660.m_y = m_parent.GetY();
				}

				long totalWeight = 0;
				long totalWeightX = 0;
				long totalWeightY = 0;

				for (int i = 0; i < m_targetGroups.Size(); i++)
				{
					LogicGameObject gameObject = m_targetGroups[i];
					int groupWeight;

					if (m_targetGroups[i].GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)gameObject;

						groupWeight = m_attackWithGroups
							? character.GetCharacterData().GetFriendlyGroupWeight()
							: character.GetCharacterData().GetEnemyGroupWeight();
					}
					else
					{
						groupWeight = 1;
					}

					totalWeightX += groupWeight * gameObject.GetMidX();
					totalWeightY += groupWeight * gameObject.GetMidY();
					totalWeight += groupWeight;
				}

				if (totalWeight > 0)
				{
					LogicVector2 pos1 = new LogicVector2(m_unk604.m_x, m_unk604.m_y);

					int posX = (int)(totalWeightX / totalWeight);
					int posY = (int)(totalWeightY / totalWeight);

					m_unk604.Set(posX, posY);
					m_targetGroupPosition.Set(posX, posY);

					LogicVector2 pos2 = new LogicVector2(m_unk660.m_x - m_unk604.m_x, m_unk660.m_y - m_unk604.m_y);

					pos2.Normalize(3072);

					m_targetGroupPosition.Add(pos2);
					m_targetGroupPosition.Set(
						(m_targetGroupPosition.m_x + m_parent.GetMidX()) / 2,
						(m_targetGroupPosition.m_y + m_parent.GetMidY()) / 2);

					if (m_unk596)
					{
						totalWeightX = 0;
						totalWeightY = 0;
						totalWeight = 0;

						for (int i = 0; i < m_targetGroups.Size(); i++)
						{
							LogicGameObject gameObject = m_targetGroups[i];
							int groupWeight;

							if (m_targetGroups[i].GetGameObjectType() == LogicGameObjectType.CHARACTER)
							{
								LogicCharacter character = (LogicCharacter)gameObject;

								groupWeight = m_attackWithGroups
									? character.GetCharacterData().GetFriendlyGroupWeight()
									: character.GetCharacterData().GetEnemyGroupWeight();
							}
							else
							{
								groupWeight = 1;
							}

							LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

							if (combatComponent != null)
							{
								LogicGameObject target = combatComponent.m_targets[0];

								if (target != null)
								{
									pos2.Set(target.GetMidX() - gameObject.GetMidX(), target.GetMidY() - gameObject.GetMidY());
									pos2.Normalize(512);

									totalWeightX += groupWeight * pos2.m_x;
									totalWeightY += groupWeight * pos2.m_y;
									totalWeight += groupWeight;
								}
							}
						}

						if (totalWeight > 0)
						{
							posX = (int)(totalWeightX / totalWeight);
							posY = (int)(totalWeightY / totalWeight);
							pos2.Set(posX, posY);
							pos2.Normalize(512);

							m_unk604.Substract(pos2);
						}
					}
				}
			}
			else
			{
				int attackSpeed = m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()]
					? m_attackerData.GetAltAttackSpeed()
					: m_attackerData.GetAttackSpeed();

				if (m_hitTime[0] <= attackSpeed)
				{
					int midX = m_parent.GetMidX();
					int midY = m_parent.GetMidY();

					LogicGameObject selectTarget = null;

					for (int i = 0, selectTargetWeight = 0, selectTargetDistance = 0; i < m_targetGroups.Size(); i++)
					{
						LogicGameObject gameObject = m_targetGroups[i];

						if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)gameObject;

							int groupWeight = m_attackWithGroups
								? character.GetCharacterData().GetFriendlyGroupWeight()
								: character.GetCharacterData().GetEnemyGroupWeight();
							int distanceX = character.GetMidX() - midX;
							int distanceY = character.GetMidY() - midY;
							int distanceSquared = distanceX * distanceX + distanceY * distanceY;

							if (groupWeight <= selectTargetWeight)
							{
								if (distanceSquared < selectTargetDistance && groupWeight == selectTargetWeight)
								{
									selectTargetDistance = distanceSquared;
									selectTarget = character;
								}
							}
							else
							{
								selectTarget = character;
								selectTargetDistance = distanceSquared;
								selectTargetWeight = groupWeight;
							}
						}
					}

					if (selectTarget != null)
					{
						m_targetGroup = selectTarget;

						int posX = selectTarget.GetX() + (selectTarget.GetWidthInTiles() << 8);
						int posY = selectTarget.GetY() + (selectTarget.GetWidthInTiles() << 8);

						m_unk604.Set(posX, posY);
						m_targetGroupPosition.Set(posX, posY);
					}
				}
				else
				{
					if (m_targetGroup != null)
					{
						m_unk604.m_x = m_targetGroup.GetX() + (m_targetGroup.GetWidthInTiles() << 8);
						m_unk604.m_y = m_targetGroup.GetY() + (m_targetGroup.GetWidthInTiles() << 8);
					}
				}
			}
		}

		public bool GetAttackFinished()
			=> m_readyForAttack;

		public void StopAttack()
		{
			for (int i = 0; i < m_targetCount; i++)
			{
				m_hitTime[i] = 0;

				if (m_burstTime[i] != 0)
				{
					m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
				}

				m_burstTime[i] = 0;
			}

			m_readyForAttack = false;
			m_unk497 = false;
		}

		public void RefreshTarget(bool destructTarget)
		{
			if (LogicDataTables.GetGlobals().UseStickToClosestUnitHealer())
			{
				if (m_damage > 0)
				{
					m_enemyFilter.PassEnemyOnly(m_parent);
					goto REFRESH;
				}

				if (m_attackerData.GetShockwavePushStrength() == 0 && m_attackerData.GetDamage2() <= 0)
				{
					if (m_targets[0] != null)
					{
						if (IsTargetValid(m_targets[0]))
						{
							return;
						}
					}
				}
			}

			if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0)
			{
				m_enemyFilter.PassEnemyOnly(m_parent);
			}
			else
			{
				m_enemyFilter.PassFriendlyOnly(m_parent);
			}

		REFRESH:

			if (m_unk596)
			{
				RefreshTargetGroups(m_groupNoDamageMod);
			}
			else
			{
				if (!m_attackerData.GetTargetGroups())
				{
					for (int i = 0; i < m_targetCount; i++)
					{
						if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)m_parent;

							if (character.GetSpawnDelay() > 0 || character.GetSpawnIdleTime() > 0)
							{
								m_targets[i] = null;
								m_originalTarget = null;

								continue;
							}
						}

						bool damageUnit = false;
						bool notPassablePosition = false;

						if (LogicDataTables.GetGlobals().UseStickToClosestUnitHealer())
						{
							if (m_targets[i] != null && m_forgetTargetTime[i] == 0 && m_parent.GetMovementComponent() != null && !IsWallBreaker())
							{
								damageUnit = m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0;
							}
						}
						else
						{
							if (m_targets[i] != null && m_forgetTargetTime[i] == 0 && m_parent.GetMovementComponent() != null && !IsWallBreaker())
							{
								damageUnit = true;
							}
						}

						LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

						if (movementComponent != null)
						{
							notPassablePosition = movementComponent.IsInNotPassablePosition();
						}

						LogicGameObject originalTarget = m_originalTarget ?? m_targets[i];

						if (!notPassablePosition && m_attackWithGroups)
						{
							if (m_targets[i] != null && m_originalForgetTargetTime == 0 && m_hitTime[i] <= 64)
							{
								RefreshTarget_Internal1(i, true);
							}
							else
							{
								if (m_targetGroups.Size() != 0)
								{
									if (m_hitTime[i] <= 64)
									{
										bool isBreak = false;

										for (int j = 0; j < m_targetGroups.Size(); j++)
										{
											LogicCombatComponent combatComponent = m_targetGroups[j].GetCombatComponent();

											if (combatComponent != null)
											{
												if (combatComponent.m_targets[0] == m_targets[i] ||
													combatComponent.m_originalTarget == m_targets[i])
												{
													isBreak = true;
													break;
												}
											}
										}

										if (!isBreak)
											damageUnit |= m_hitTime[i] <= 64;
									}
								}
							}
						}

						if (damageUnit || destructTarget)
						{
							RefreshTarget_Internal1(i, true);
						}

						if (m_forceNewTarget[i])
						{
							RefreshTarget_Internal1(i, false);
						}

						if (m_targets[i] != null)
						{
							bool unk3 = true;

							if (IsTargetValid(m_targets[i]))
							{
								if (m_targets[i].IsStealthy())
								{
									if (m_damage <= 0 && m_attackerData.GetShockwavePushStrength() <= 0 && m_attackerData.GetDamage2() <= 0)
									{
										unk3 = false;
									}
								}
								else
								{
									unk3 = false;
								}
							}

							bool originalTargetNotValid = m_originalTarget != null && !IsTargetValid(m_originalTarget);

							if (originalTargetNotValid || unk3)
							{
								// Listener

								m_targets[i] = null;
								m_originalTarget = null;

								if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
								{
									LogicCharacter character = (LogicCharacter)m_parent;
									LogicCharacterData data = character.GetCharacterData();

									if (data.IsUnderground())
									{
										m_hideTime[i] = LogicDataTables.GetGlobals().GetMinerHideTime() +
															 m_parent.Rand(0) % LogicDataTables.GetGlobals().GetMinerHideTimeRandom();
									}

									if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0)
									{
										if (!character.IsHero())
										{
											m_attackDelay[i] = LogicMath.Abs(m_parent.Rand(0)) % 800 + 200;
										}
									}
								}
							}
						}

						if (!LogicDataTables.GetGlobals().UseStickToClosestUnitHealer())
						{
							if (m_hideTime[i] != 0)
							{
								continue;
							}

							if (m_targets[i] != null)
							{
								if (m_damage > 0 || m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.GetDamage2() > 0)
								{
									continue;
								}
							}
						}
						else if (m_hideTime[i] != 0 || m_targets[i] != null)
						{
							continue;
						}

						if (m_attackDelay[i] != 0)
						{
							if (!m_troopChild || notPassablePosition)
							{
								continue;
							}
						}
						else if (notPassablePosition)
						{
							continue;
						}

						if (LogicDataTables.GetGlobals().RestartAttackTimerOnAreaDamageTurrets() || m_attackerData.GetDamageRadius() == 0 || movementComponent != null)
						{
							m_hitTime[i] = 0;

							if (m_burstTime[i] != 0)
							{
								m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
							}

							m_burstTime[i] = 0;
						}

						m_originalTarget = null;
						SearchTarget(i, originalTarget);

						if (m_targets[i] != null)
						{
							if (IsTargetValid(m_targets[i]))
							{
								if (movementComponent != null)
								{
									movementComponent.NewTargetFound();
									// Listener
								}

								m_totalTargets += 1;
							}
							else
							{
								m_targets[i] = null;
							}
						}

						if (movementComponent != null && m_targets[0] == null)
						{
							movementComponent.NoTargetFound();
						}

						if (m_targets[0] == null)
						{
							m_hitTime[i] = 0;

							if (m_burstTime[i] != 0)
							{
								m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
							}

							m_burstTime[i] = 0;
						}

						m_attackDelay[i] = 500;
					}
				}
				else
				{
					if (m_burstTime[0] == 0 && (m_targetGroups.Size() <= 0 && m_attackCooldownOverride[0] <= 0 && m_attackDelay[0] == 0 || m_forceNewTarget[0]))
					{
						RefreshTargetGroups(m_damage <= 0 && m_attackerData.GetShockwavePushStrength() == 0 && m_attackerData.GetDamage2() <= 0);

						m_forceNewTarget[0] = false;
						m_attackDelay[0] = 500;
						m_hitTime[0] = 0;
						m_readyForAttack = false;

						if (m_burstTime[0] != 0)
						{
							m_attackCooldownOverride[0] = m_attackerData.GetAttackCooldownOverride();
						}

						m_burstTime[0] = 0;
					}
				}
			}
		}

		private void RefreshTarget_Internal1(int i, bool forceNewTarget)
		{
			if (forceNewTarget)
			{
				m_forceNewTarget[i] = true;
			}

			// Listener.

			m_forceNewTarget[i] = false;
			m_targets[i] = null;
			m_originalTarget = null;
			m_targetGroup = null;

			if (forceNewTarget)
			{
				m_attackDelay[i] = 0;
			}
			else
			{
				m_attackDelay[i] = LogicMath.Abs(m_parent.Rand(0));
				m_attackDelay[i] = m_attackDelay[i] % 800;
			}
		}

		public LogicVector2 GetTargetGroupPosition()
		{
			if (m_targetGroups.Size() > 0 || m_burstTime[0] > 0)
				return m_targetGroupPosition;
			return null;
		}

		public bool GetUnk596()
			=> m_unk596;

		public bool RefreshTargetGroups(bool noDamage)
		{
			int radius = m_unk596 || m_attackWithGroups ? m_targetGroupsRadius : m_attackerData.GetTargetGroupsRadius();
			int groupTileCount = 2 * radius / 5;

			if (groupTileCount <= 0)
				groupTileCount = 1;

			int subGroupCount = 2 * radius / groupTileCount;
			int groupCount = 25600 / groupTileCount;

			if (25600u % groupTileCount > groupTileCount / 3u)
				groupCount += 1;

			int groupCountSquared = groupCount * groupCount;

			m_targetGroupWeights.Clear();
			m_targetGroupWeights.EnsureCapacity(groupCountSquared);

			for (int i = 0; i < groupCountSquared; i++)
			{
				m_targetGroupWeights.Add(0);
			}

			m_targetGroups.Clear();
			m_targetGroups.EnsureCapacity(20);

			if (noDamage)
				m_groupEnemyFilter.PassFriendlyOnly(m_parent);
			else
				m_groupEnemyFilter.PassEnemyOnly(m_parent);

			m_targetGroupEnemyList.Clear();
			m_parent.GetGameObjectManager().GetGameObjects(m_targetGroupEnemyList, m_groupEnemyFilter);

			int maxGroupWeight = 0;
			int targetGroupsRangeSquared = m_attackerData.GetTargetGroupsRange() * m_attackerData.GetTargetGroupsRange();

			if (m_targetGroupEnemyList.Size() == 0)
				return false;

			for (int i = 0; i < m_targetGroupEnemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_targetGroupEnemyList[i];

				if (gameObject == m_parent || gameObject.IsHidden() || (gameObject.IsStealthy() && !noDamage) || !gameObject.IsAlive() || !CanAttackHeightCheck(gameObject))
					continue;

				if (noDamage)
				{
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent != null && combatComponent.m_damage <= 0 && combatComponent.m_attackerData.GetShockwavePushStrength() == 0 &&
						combatComponent.m_attackerData.GetDamage2() <= 0)
						continue;
				}

				int groupWeight = 1;

				if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)gameObject;

					if (character.GetSpawnDelay() > 0 || character.GetSpawnIdleTime() > 0 || character.GetChildTroops() != null)
						continue;

					groupWeight = m_attackWithGroups
						? character.GetCharacterData().GetFriendlyGroupWeight()
						: character.GetCharacterData().GetEnemyGroupWeight();
				}

				if (m_parent.GetMovementComponent() != null || IsInRange(gameObject))
				{
					int midX = gameObject.GetMidX();
					int midY = gameObject.GetMidY();

					if ((midX | midY) >= 0)
					{
						if (targetGroupsRangeSquared != 0)
						{
							int parentMidX = m_parent.GetMidX();
							int parentMidY = m_parent.GetMidY();
							int distanceX = parentMidX - midX;
							int distanceY = parentMidY - midY;

							if (distanceX * distanceX + distanceY * distanceY > targetGroupsRangeSquared)
								continue;
						}

						int groupX = midX / groupTileCount;
						int groupY = midY / groupTileCount;

						if (groupX < groupCount && groupY < groupCount)
						{
							int offset = groupX + groupCount * groupY;
							int weight = m_targetGroupWeights[offset] + groupWeight;

							m_targetGroupWeights[offset] = weight;

							if (weight > maxGroupWeight)
							{
								maxGroupWeight = weight;
							}
						}
					}
				}
			}

			if (maxGroupWeight == 0)
				return false;

			int tmp1 = subGroupCount * groupTileCount / -2;
			int tmp2 = tmp1 + (groupTileCount >> 1);

			if (m_targetGroupWeightMultiplier.Size() == 0)
			{
				m_targetGroupWeightMultiplier.EnsureCapacity(subGroupCount * subGroupCount);

				LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

				if (movementComponent != null)
				{
					for (int x = 0, subX = 0; x < subGroupCount; x++)
					{
						int offset = tmp2 + groupTileCount * x;
						int offsetSquared = offset * offset;

						for (int y = 0, subY = tmp2; y < subGroupCount; y++)
						{
							int sqrt = LogicMath.Sqrt(offsetSquared + subY * subY);
							int clamp = LogicMath.Clamp(radius - sqrt, -(groupTileCount >> 1), groupTileCount >> 1);

							m_targetGroupWeightMultiplier.Add(subX + y, 100 * ((groupTileCount >> 1) + clamp) / (groupTileCount & 0x7FFFFFFE));

							subY += groupTileCount;
						}

						subX += subGroupCount;
					}
				}
				else
				{
					for (int i = 0, subX = 0; i < subGroupCount; i++)
					{
						int tmp = tmp2 + i * groupTileCount;
						int offset = tmp;

						if (tmp != 0)
							offset = tmp - (groupTileCount >> 1);
						if (tmp < 0)
							offset = tmp + (groupTileCount >> 1);

						int offsetSquared = offset * offset;

						for (int j = 0, subY = tmp1; j < subGroupCount; j++)
						{
							int offset2 = subY + (groupTileCount >> 1);

							if (offset2 < 0)
								offset2 = 2 * (groupTileCount >> 1) + subY;
							else if (offset2 != 0)
								offset2 = subY;

							int value = radius - LogicMath.Sqrt(offsetSquared + offset2 * offset2);

							if (value < 0)
								value = 0;

							m_targetGroupWeightMultiplier.Add(subX + j, 100 * value / radius);

							subY += groupTileCount;
						}

						subX += subGroupCount;
					}
				}
			}

			int bestGroupX = 0;
			int bestGroupY = 0;
			int count = groupCount - subGroupCount;

			long bestGroupWeightMultiplied = 0;

			for (int i = 0; i <= count; i++)
			{
				int offset = tmp1 - i * groupTileCount;

				for (int j = 0; j <= count; j++)
				{
					long groupWeight = 0;
					long groupWeightMultiplied = 0;

					for (int k = 0, subX = 0; k < subGroupCount; k++)
					{
						int offset2 = j + (i + k) * groupCount;

						for (int l = 0; l < subGroupCount; l++)
						{
							int weight = m_targetGroupWeights[offset2 + l];
							int multiplier = m_targetGroupWeightMultiplier[subX + l];

							groupWeightMultiplied += weight * multiplier;
							groupWeight += weight;
						}

						subX += subGroupCount;
					}

					if (groupWeight >= m_attackerData.GetTargetGroupsMinWeight())
					{
						if (bestGroupWeightMultiplied < groupWeightMultiplied * 1000)
						{
							int midX = m_parent.GetMidX();
							int midY = m_parent.GetMidY();
							int offset2 = tmp1 - j * groupTileCount;

							int sqrt = LogicMath.Sqrt((offset2 + midX) * (offset2 + midX) + (offset + midY) * (offset + midY)) >> 8;
							int sqrt20000 = LogicMath.Sqrt(20000);
							uint multiplier = (uint)(1000 * (sqrt20000 - sqrt) / sqrt20000 * (1000 * (sqrt20000 - sqrt) / sqrt20000) / 1000u);

							groupWeightMultiplied *= multiplier;

							if (groupWeightMultiplied < 1)
								groupWeightMultiplied = 1;
						}

						if (bestGroupWeightMultiplied < groupWeightMultiplied)
						{
							bestGroupWeightMultiplied = groupWeightMultiplied;
							bestGroupX = j;
							bestGroupY = i;
						}
					}
				}
			}

			int minX = bestGroupX * groupTileCount;
			int maxX = bestGroupX * groupTileCount + subGroupCount * groupTileCount;
			int minY = bestGroupY * groupTileCount;
			int maxY = bestGroupY * groupTileCount + subGroupCount * groupTileCount;

			int totalWeight = 0;

			for (int i = 0; i < m_targetGroupEnemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_targetGroupEnemyList[i];

				if (gameObject == m_parent || gameObject.IsHidden() || (gameObject.IsStealthy() && !noDamage) || !gameObject.IsAlive() || !CanAttackHeightCheck(gameObject))
					continue;

				if (noDamage)
				{
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent != null && combatComponent.m_damage <= 0 && combatComponent.m_attackerData.GetShockwavePushStrength() <= 0 &&
						combatComponent.m_attackerData.GetDamage2() <= 0)
						continue;
				}

				int groupWeight = 1;

				if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)gameObject;

					if (character.GetSpawnDelay() > 0 || character.GetSpawnIdleTime() > 0 || character.GetChildTroops() != null)
						continue;

					groupWeight = m_attackWithGroups
						? character.GetCharacterData().GetFriendlyGroupWeight()
						: character.GetCharacterData().GetEnemyGroupWeight();
				}

				int midX = gameObject.GetMidX();
				int midY = gameObject.GetMidY();

				if (midX >= minX && midX <= maxX && midY >= minY && midY <= maxY)
				{
					if (m_parent.GetMovementComponent() != null || IsInRange(gameObject))
					{
						m_targetGroups.Add(gameObject);
						totalWeight += groupWeight;
					}
				}
			}

			if (totalWeight < m_attackerData.GetTargetGroupsMinWeight())
			{
				m_targetGroups.Clear();
				return false;
			}

			UpdateSelectedTargetGroup();
			return m_targetGroups.Size() != 0;
		}

		public LogicGameObject SelectWall()
		{
			if (m_parent.GetHitpointComponent().GetTeam() != 1)
			{
				LogicArrayList<LogicGameObject> buildings = m_parent.GetGameObjectManager().GetGameObjects(LogicGameObjectType.BUILDING);
				LogicGameObject wall = GetBestWallToBreak(buildings);

				if (wall == null)
				{
					LogicGameObject closestWall = null;

					for (int i = 0, minDistance = 0; i < buildings.Size(); i++)
					{
						LogicBuilding building = (LogicBuilding)buildings[i];

						if (building.IsWall())
						{
							m_rangePosition.m_x = building.GetMidX() - m_parent.GetMidX();
							m_rangePosition.m_y = building.GetMidY() - m_parent.GetMidY();

							int lengthSquared = m_rangePosition.GetLengthSquared();

							if ((closestWall == null || lengthSquared < minDistance) && building.IsConnectedWall())
							{
								minDistance = lengthSquared;
								closestWall = building;
							}
						}
					}

					wall = closestWall;
				}

				if (wall != null)
				{
					if (wall.IsAlive())
					{
						return wall;
					}

					int wallTileX = wall.GetTileX();
					int wallTileY = wall.GetTileY();

					int distanceX = wallTileX - m_parent.GetTileX();
					int distanceY = wallTileY - m_parent.GetTileY();

					if ((distanceX | distanceY) != 0)
					{
						m_rangePosition.m_x = distanceX;
						m_rangePosition.m_y = distanceY;

						m_rangePosition.Normalize(10);

						return FindNextWallInLine(wallTileX, wallTileY, wallTileX + m_rangePosition.m_x, wallTileY + m_rangePosition.m_y);
					}
				}
			}

			return null;
		}

		public bool IsWall(LogicGameObject gameObject)
		{
			if (gameObject != null)
			{
				LogicGameObjectData data = gameObject.GetData();

				if (data.GetDataType() == DataType.BUILDING)
				{
					return ((LogicBuildingData)data).IsWall();
				}
			}

			return false;
		}

		public bool IsHealer()
			=> m_damage >> 31 != 0;

		public bool CanAttackHeightCheck(LogicGameObject gameObject)
		{
			int layout = m_parent.GetLevel().GetCurrentLayout();
			bool flying = gameObject.IsFlying();

			if (!m_useAltAttackMode[layout] || !m_altMultiTargets)
			{
				if (GetTrackAirTargets(layout, false) || !flying)
				{
					return flying || GetTrackGroundTargets(layout, false);
				}

				return false;
			}

			return true;
		}

		public bool IsAlerted()
			=> m_alertTime > 0;

		public void StartAllianceAlert(LogicGameObject gameObject, LogicGameObject target)
		{
			if (m_preferredTarget == null)
			{
				if (!LogicDataTables.GetGlobals().UseStickToClosestUnitHealer() ||
					m_damage > 0 ||
					m_attackerData.GetShockwavePushStrength() != 0 ||
					m_attackerData.GetDamage2() > 0)
				{
					if ((m_parent == target || m_alertTime <= 0) && (!LogicDataTables.GetGlobals().IgnoreAllianceAlertForNonValidTargets() ||
																			   IsTargetValid(gameObject) && CanAttackHeightCheck(gameObject)))
					{
						for (int i = 0; i < m_targetCount; i++)
						{
							LogicGameObject tmp = m_targets[i];

							if (tmp != null)
							{
								if (tmp.GetGameObjectType() == LogicGameObjectType.CHARACTER)
								{
									return;
								}
							}
						}

						m_alertTime = 1000;
						m_alerted = true;
					}
				}
			}
		}

		public void ObstacleToDestroy(LogicGameObject obstacle)
		{
			if (m_targets[0] == null || !IsInRange(m_targets[0]) || !IsInLine(m_targets[0]))
			{
				if (IsTargetValid(obstacle))
				{
					if (LogicDataTables.GetGlobals().RememberOriginalTarget() && m_originalTarget == null)
					{
						m_originalTarget = m_targets[0];
					}

					m_targets[0] = obstacle;

					if (obstacle.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						((LogicBuilding)obstacle).StartSelectedWallTime();
					}
				}
				else
				{
					m_attackDelay[0] = 2500;
					m_targets[0] = null;
				}
			}
		}

		public LogicGameObject GetBestWallToBreak(LogicArrayList<LogicGameObject> gameObjects)
		{
			if (LogicDataTables.GetGlobals().GetWallBreakerSmartCountLimit() != 0)
			{
				if (m_totalTargets < LogicDataTables.GetGlobals().GetWallBreakerSmartRetargetLimit())
				{
					LogicArrayList<LogicGameObject> characters = m_parent.GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER);

					int wallBreakerCount = 0;

					for (int i = 0; i < characters.Size(); i++)
					{
						LogicCharacter character = (LogicCharacter)characters[i];
						LogicCombatComponent combatComponent = character.GetCombatComponent();

						if (combatComponent != null && combatComponent.m_preferredTarget != null && combatComponent.m_preferredTarget.GetDataType() == DataType.BUILDING_CLASS &&
							((LogicBuildingClassData)combatComponent.m_preferredTarget).IsWall())
						{
							if (character.IsAlive())
							{
								wallBreakerCount += 1;
							}
						}
					}

					if (wallBreakerCount <= LogicDataTables.GetGlobals().GetWallBreakerSmartCountLimit())
					{
						int smartRadius = LogicDataTables.GetGlobals().GetWallBreakerSmartRadius();
						int smartRadiusSquared = smartRadius * smartRadius;

						LogicArrayList<int> goLength = new LogicArrayList<int>(50);
						LogicArrayList<int> goIdx = new LogicArrayList<int>(50);

						for (int i = 0; i < gameObjects.Size(); i++)
						{
							LogicGameObject gameObject = gameObjects[i];

							if (!gameObject.IsWall() &&
								!gameObject.IsHidden())
							{
								if (gameObject.IsAlive())
								{
									m_rangePosition.m_x = gameObject.GetMidX() - m_parent.GetMidX();
									m_rangePosition.m_y = gameObject.GetMidY() - m_parent.GetMidY();

									int length = m_rangePosition.GetLengthSquared();

									if (length <= smartRadiusSquared && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
									{
										if (goLength.Size() >= 50)
										{
											goLength.Remove(49);
											goIdx.Remove(49);
										}

										bool added = false;

										for (int j = 0; j < goLength.Size(); j++)
										{
											if (goLength[j] > length)
											{
												goLength.Add(j, length);
												goIdx.Add(j, i);

												added = true;

												break;
											}
										}

										if (!added)
										{
											goLength.Add(length);
											goIdx.Add(i);
										}
									}
								}
							}
						}

						if (LogicDataTables.GetGlobals().WallBreakerUseRooms())
						{
							LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

							int roomIdx = tileMap.GetTile(m_parent.GetTileX(), m_parent.GetTileY())?.GetRoomIdx() ?? -1;

							for (int i = 0; i < goIdx.Size(); i++)
							{
								LogicGameObject gameObject = gameObjects[goIdx[i]];

								if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									int goRoomIdx = tileMap.GetTile(gameObject.GetTileX(), gameObject.GetTileY())?.GetRoomIdx() ?? -1;

									if (goRoomIdx != roomIdx)
									{
										LogicMovementComponent movementComponent = m_parent.GetMovementComponent();
										LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();

										movementComponent.MoveTo(gameObject);

										LogicGameObject wall = movementSystem.GetWall();

										if (wall != null)
										{
											return wall;
										}
									}
								}
							}
						}
						else
						{
							for (int i = 0; i < goIdx.Size(); i++)
							{
								LogicGameObject gameObject = gameObjects[goIdx[i]];

								if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									LogicMovementComponent movementComponent = m_parent.GetMovementComponent();
									LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();

									movementComponent.MoveTo(gameObject);

									LogicGameObject wall = movementSystem.GetWall();

									if (wall != null)
									{
										return wall;
									}
								}
							}
						}
					}
				}
			}

			return null;
		}

		public LogicGameObject FindNextWallInLine(int startX, int startY, int endX, int endY)
		{
			int moveStepX = endX > startX ? 1 : -1;
			int moveStepY = endY > startY ? 1 : -1;

			int distanceX = LogicMath.Abs(endX - startX);
			int distanceY = LogicMath.Abs(endY - startY);
			int direction = distanceX - distanceY;

			int subTileDistanceX = distanceX * 2;
			int subTileDistanceY = distanceY * 2;

			LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

			for (int i = distanceX + distanceY, posX = startX, posY = startY; i >= 0; i--)
			{
				LogicTile tile = tileMap.GetTile(posX, posY);

				if (tile == null)
				{
					break;
				}

				for (int k = 0; k < tile.GetGameObjectCount(); k++)
				{
					LogicGameObject gameObject = tile.GetGameObject(k);

					if (gameObject.IsWall() && gameObject.IsAlive())
					{
						return gameObject;
					}
				}

				if (direction > 0)
				{
					direction -= subTileDistanceY;
					posX += moveStepX;
				}
				else
				{
					direction += subTileDistanceX;
					posY += moveStepY;
				}
			}

			return null;
		}

		public void Boost(int damage, int attackSpeedBoost, int time)
		{
			if (damage < 0)
			{
				m_slowDamage = LogicMath.Min(LogicMath.Max(-100, damage), m_slowDamage);
				m_slowTime = time;
			}
			else
			{
				int idx = m_boostDamage[0] != 0 ? 1 : 0;

				m_boostDamage[idx] = LogicMath.Max(damage, m_boostDamage[idx]);
				m_boostTime[idx] = time;
			}

			m_attackSpeedBoost = attackSpeedBoost;
			m_attackSpeedBoostTime = time;
		}

		public bool IsBoosted()
			=> m_boostTime[0] > 0;

		public bool IsSlowed()
			=> m_slowTime > 0;

		public override void SubTick()
		{
			if (m_activationTime <= 0)
			{
				if (m_attackerData.GetTargetGroups() || m_unk596 || m_attackWithGroups)
				{
					LogicVector2 position = new LogicVector2();
					position.Set(m_unk604.m_x, m_unk604.m_y);
					UpdateSelectedTargetGroup();

					if (m_targetGroups.Size() > 0)
					{
						int targetGroupsRadius = m_unk596 || m_attackWithGroups ? m_targetGroupsRadius : m_attackerData.GetTargetGroupsRadius();

						if (m_parent.GetMovementComponent() == null)
						{
							LogicVector2 position2 = new LogicVector2();

							position2.Set(m_unk604.m_x, m_unk604.m_y);
							position2.Substract(position);

							int length = position2.Normalize(30);

							if (length <= 2 * targetGroupsRadius && length > 30)
							{
								m_unk604.Set(position.m_x, position.m_y);
								m_unk604.Add(position2);
							}
						}

						// Listener.

						if (m_unk596)
						{
							LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

							if (movementComponent == null || !movementComponent.IsInNotPassablePosition())
							{
								if (movementComponent != null)
								{
									LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();
									LogicVector2 pathEndPosition = new LogicVector2(movementSystem.GetPathEndPosition().m_x, movementSystem.GetPathEndPosition().m_y);

									int distance = pathEndPosition.GetDistanceSquaredTo(m_unk604.m_x, m_unk604.m_y);

									if (distance > 0x10000)
									{
										if (m_parent.GetLevel().GetTileMap().GetNearestPassablePosition(m_unk604.m_x, m_unk604.m_y, m_unk604, 512))
										{
											movementComponent.MoveTo(m_unk604.m_x, m_unk604.m_y);
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				m_activationTime -= 16;
			}
		}

		public override void Tick()
		{
			if (m_activationTime <= 0)
			{
				if (m_altMultiTargets)
				{
					m_targetCount = m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()] ? m_attackerData.GetAltNumMultiTargets() : 1;
				}

				if (!m_parent.IsAlive() || m_parent.IsHidden() || m_attackerData.GetAmmoCount() > 0 && m_ammo == 0 || m_parent.IsFrozen())
				{
					for (int i = 0; i < m_targetCount; i++)
					{
						m_targets[i] = null;
						m_hitTime[i] = 0;
						m_burstTime[i] = 0;
						m_damage2Time[i] = 0;

						// Listener.

						if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)m_parent;

							if (character.GetCharacterData().IsUnderground())
							{
								m_hideTime[i] = LogicDataTables.GetGlobals().GetMinerHideTime() +
													 m_parent.Rand(0) % LogicDataTables.GetGlobals().GetMinerHideTimeRandom();
							}
						}
					}

					m_originalTarget = null;
					m_prepareSpeed = m_attackerData.GetPrepareSpeed();
					m_targetGroups.Clear();

					// Listener.

					m_unk604.Set(0, 0);
					m_targetGroupPosition.Set(0, 0);

					m_targetGroup = null;

					// Listener.
				}
				else
				{
					if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING && m_attackerData.GetWakeUpSpace() > 0)
					{
						int prevDeployedHousingSpace = m_deployedHousingSpace;
						m_deployedHousingSpace = m_parent.GetLevel().GetBattleLog().GetDeployedHousingSpace();

						if (m_deployedHousingSpace >= m_attackerData.GetWakeUpSpace())
						{
							if (prevDeployedHousingSpace < m_attackerData.GetWakeUpSpace())
							{
								// Listener.
							}

							m_wakeUpTime = LogicMath.Max(m_wakeUpTime - 64, 0);

							if (m_wakeUpTime > 0)
							{
								return;
							}
						}
						else
						{
							return;
						}
					}

					RefreshTarget(false);

					int boostDefenceMS = ((m_attackSpeedBoost << 6) + 6400) / 100;

					m_damageTime += 64;
					m_alertTime = LogicMath.Max(m_alertTime - 64, 0);
					m_summonCooldownTime = LogicMath.Max(m_summonCooldownTime - boostDefenceMS, 0);
					m_originalForgetTargetTime = LogicMath.Max(m_originalForgetTargetTime - 64, 0);

					int targetCount = 0;

					for (int i = 0; i < m_targetCount; i++)
					{
						m_forgetTargetTime[i] = LogicMath.Max(m_forgetTargetTime[i] - 64, 0);
						m_attackDelay[i] = LogicMath.Max(m_attackDelay[i] - 64, 0);
						m_attackCooldownOverride[i] = LogicMath.Max(m_attackCooldownOverride[i] - boostDefenceMS, 0);
						m_damage2Time[i] = LogicMath.Max(m_damage2Time[i] - boostDefenceMS, 0);

						if (m_targets[i] != null)
						{
							++targetCount;
						}
					}

					if (m_targetGroups.Size() > 0)
					{
						for (int i = 0; i < m_targetGroups.Size(); i++)
						{
							LogicGameObject gameObject = m_targetGroups[i];

							if (!gameObject.IsAlive() || gameObject.IsHidden() ||
								(m_damage > 0 || m_attackerData.GetDamage2() > 0 || m_attackerData.GetShockwavePushStrength() != 0) && !m_attackWithGroups &&
								gameObject.IsStealthy() || gameObject.GetMovementComponent() == null && !IsInRange(gameObject))
							{
								// Listener.
								m_targetGroups.Remove(i--);

								if (gameObject == m_targetGroup)
								{
									m_targetGroup = null;
								}
							}
						}

						if (m_targetGroups.Size() == 0)
						{
							// Listener.
						}
					}

					if ((targetCount != 0 || m_attackerData.GetTargetGroups()) &&
						(!m_attackerData.GetTargetGroups() || m_targetGroups.Size() > 0 || m_burstTime[0] > 0))
					{
						m_prepareSpeed = LogicMath.Max(m_prepareSpeed - 64, 0);
					}
					else
					{
						m_prepareSpeed = LogicMath.Min(m_prepareSpeed + 64, m_attackerData.GetPrepareSpeed());
					}

					bool attackFinished = m_readyForAttack;
					m_readyForAttack = false;

					if (m_attackerData.GetAmmoCount() > 0)
					{
						if (m_ammoReloadingTime > 0)
						{
							--m_ammoReloadingTime;
						}
					}

					bool destructTarget = true;

					for (int i = 0; i < m_targetCount; i++)
					{
						if (m_hideTime[i] > 0)
						{
							m_hideTime[i] -= 64;

							if (m_hideTime[i] < 0)
							{
								m_hideTime[i] = 0;
							}
						}

						if (m_targets[i] != null)
						{
							bool validTarget = false;

							LogicGameObject target = m_targets[i];

							if (IsInRange(target))
							{
								validTarget = true;

								if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									validTarget = ((LogicBuilding)m_parent).IsValidTarget(target);
								}
							}

							if (m_attackMultipleBuildings)
							{
								if (!target.IsWall() && target.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

									if (movementComponent != null)
									{
										validTarget = movementComponent.GetMovementSystem().NotMoving() & validTarget;
									}
								}
							}

							bool isInLine = false;

							if (validTarget)
							{
								isInLine = IsInLine(target);

								if (!attackFinished && isInLine)
								{
									m_hitTime[i] = m_attackerData.GetNewTargetAttackDelay();
								}
							}

							if (!isInLine && m_hitTime[i] <= 0)
							{
								m_hitTime[i] = 0;

								if (m_burstTime[i] != 0)
								{
									m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
								}

								m_burstTime[i] = 0;

								goto NEXT;
							}

							m_forgetTargetTime[i] = LogicDataTables.GetGlobals().GetForgetTargetTime();
							m_readyForAttack = true;

							if (m_prepareSpeed != 0 || m_attackCooldownOverride[i] != 0)
							{
								destructTarget = false;
								goto NEXT;
							}

							if (m_attackerData.GetPreAttackEffect() != null)
							{
								// Listener.
							}

							m_hitTime[i] += boostDefenceMS;
							int attackSpeed = m_attackerData.GetAttackSpeed();

							if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
							{
								attackSpeed = m_attackerData.GetAltAttackSpeed();
							}

							if (m_hitTime[i] < attackSpeed)
							{
								destructTarget = false;
								goto NEXT;
							}

							int burstCount = m_attackerData.GetBurstCount();
							int burstDelay = m_attackerData.GetBurstDelay();

							if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
							{
								burstCount = m_attackerData.GetAltBurstCount();
								burstDelay = m_attackerData.GetAltBurstDelay();
							}

							if (burstCount <= 0 || burstDelay <= 0)
							{
								Hit(i);
							}
							else
							{
								m_hitTime[i] = attackSpeed;

								int prevBurstId = (burstDelay + m_burstTime[i] - 1) / burstDelay % (burstCount + 1);
								m_burstTime[i] += boostDefenceMS;
								int burstId = (burstDelay + m_burstTime[i] - 1) / burstDelay % (burstCount + 1);

								if (burstId <= prevBurstId)
								{
									goto NEXT;
								}

								Hit(i);

								if (burstId != burstCount)
								{
									goto NEXT;
								}
							}

							m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
							m_hitTime[i] = isInLine ? LogicMath.Min(m_hitTime[i] - attackSpeed, attackSpeed) : 0;
						}
						else
						{
							if (!m_attackerData.GetTargetGroups())
							{
								m_hitTime[i] = 0;

								if (m_burstTime[i] != 0)
								{
									m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
								}

								m_burstTime[i] = 0;
							}
							else
							{
								if (m_targetGroups.Size() > 0 || m_burstTime[0] > 0 || m_hitTime[i] >= m_attackerData.GetAttackSpeed())
								{
									m_forgetTargetTime[i] = LogicDataTables.GetGlobals().GetForgetTargetTime();
									m_readyForAttack = true;

									if (m_prepareSpeed != 0 || m_attackCooldownOverride[i] != 0)
									{
										goto NEXT;
									}

									if (m_attackerData.GetPreAttackEffect() != null)
									{
										if (m_hitTime[i] < 64)
										{
											// Listener.
										}
									}

									m_hitTime[i] += boostDefenceMS;

									if (m_hitTime[i] < m_attackerData.GetAttackSpeed())
									{
										destructTarget = false;
										goto NEXT;
									}

									int burstCount = m_attackerData.GetBurstCount();
									int burstDelay = m_attackerData.GetBurstDelay();

									if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
									{
										burstCount = m_attackerData.GetAltBurstCount();
										burstDelay = m_attackerData.GetAltBurstDelay();
									}

									if (burstCount <= 0)
									{
										Hit(i);
									}
									else
									{
										int attackerSpeed = m_attackerData.GetAttackSpeed();

										if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
										{
											attackerSpeed = m_attackerData.GetAltAttackSpeed();
										}

										m_hitTime[i] = attackerSpeed;

										int prevBurstId = (burstDelay + m_burstTime[i] - 1) / burstDelay;
										m_burstTime[i] += boostDefenceMS;
										int burstId = (burstDelay + m_burstTime[i] - 1) / burstDelay;

										if (burstId <= prevBurstId)
										{
											goto NEXT;
										}

										Hit(i);

										if (burstId != burstCount)
										{
											goto NEXT;
										}
									}

									m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();

									int attackSpeed = m_attackerData.GetAttackSpeed();

									if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
									{
										attackSpeed = m_attackerData.GetAltAttackSpeed();
									}

									m_hitTime[i] = LogicMath.Min(m_hitTime[i] - attackSpeed, attackSpeed);
									m_burstTime[i] = 0;
									m_targetGroups.Clear();

									// Listener.

									m_unk604.Set(-1, -1);
									m_targetGroupPosition.Set(-1, -1);

									m_targetGroup = null;
								}
								else
								{
									m_hitTime[i] = 0;

									if (m_burstTime[i] != 0)
									{
										m_attackCooldownOverride[i] = m_attackerData.GetAttackCooldownOverride();
									}

									m_burstTime[i] = 0;

									m_unk604.Set(-1, -1);
									m_targetGroupPosition.Set(-1, -1);
								}
							}
						}

					NEXT:

						if (m_damage2Time[i] != 0 || m_damage2X[i] < 0)
						{
							continue;
						}

						if (m_attackerData.GetDamage2Radius() > 0)
						{
							int targetType = GetTrackAirTargets(m_parent.GetLevel().GetCurrentLayout(), false)
								? GetTrackGroundTargets(m_parent.GetLevel().GetCurrentLayout(), false) ? 2 : 0
								: 1;

							if (m_attackerData.GetDamage2Min() <= 0 || m_attackerData.GetDamage2Min() == m_attackerData.GetDamage2())
							{
								m_parent.GetLevel().AreaDamage(m_parent.GetGlobalID(), m_damage2X[i], m_damage2Y[i], m_attackerData.GetDamage2Radius(),
																	m_attackerData.GetDamage2(), m_preferredTarget, m_preferredTargetDamageMod, null,
																	m_parent.GetHitpointComponent().GetTeam(), null, targetType, 0, m_attackerData.GetPushBack(), true,
																	false,
																	100, 0, m_parent, 100, 0);
							}
						}
					}

					if (destructTarget)
					{
						if (m_alerted)
						{
							m_targets[0] = null;
							m_originalTarget = null;
							m_attackDelay[0] = 0;
							m_alerted = false;
						}
					}

					if (m_attackerData.GetSummonTroop() != null && !m_attackerData.GetSpawnOnAttack())
					{
						int summonCooldown = m_attackerData.GetSummonCooldown();

						if (m_summonCooldownTime == 0)
						{
							m_summonCooldownTime = summonCooldown;
							m_unk497 = false;
						}

						if (m_summonCooldownTime < summonCooldown / 8 && m_targets[0] != null)
						{
							int spawnSummonCount = ((LogicCharacter)m_parent).GetSummonTroopCount();

							if (spawnSummonCount >= m_attackerData.GetSummonLimit())
							{
								m_summonCooldownTime = 0;
							}
							else
							{
								m_attackDelay[0] = summonCooldown / 6;
								m_targets[0] = null;
								m_originalTarget = null;
								m_unk497 = true;

								m_parent.SpawnEvent(null, 0, 0);
							}
						}
					}

					if (m_boostTime[0] > 0)
					{
						m_boostTime[0] -= 1;

						if (m_boostTime[0] == 0)
						{
							m_boostTime[0] = m_boostTime[1];
							m_boostDamage[0] = m_boostDamage[1];

							m_boostTime[1] = 0;
							m_boostDamage[1] = 0;
						}
					}

					if (m_slowTime > 0)
					{
						m_slowTime -= 1;

						if (m_slowTime == 0)
						{
							m_slowDamage = 0;
						}
					}

					if (m_attackSpeedBoostTime > 0)
					{
						m_attackSpeedBoostTime -= 1;

						if (m_attackSpeedBoostTime == 0)
						{
							m_attackSpeedBoost = 0;
						}
					}

					if (m_undergroundTime > 0)
					{
						m_undergroundTime -= 1;

						LogicMovementComponent movementComponent = m_parent.GetMovementComponent();

						if (movementComponent != null)
						{
							LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();

							if (movementSystem.NotMoving())
							{
								m_undergroundTime = 0;
								movementComponent.CheckTriggers();
							}
						}

						if (m_undergroundTime <= 0)
						{
							// Listener.
						}
					}
				}
			}
		}

		public void AttackedBy(LogicGameObject gameObject)
		{
			if (m_parent.GetHitpointComponent().GetTeam() != 1 && m_alertTime <= 0 && GetComponentType() == LogicComponentType.COMBAT &&
				gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicArrayList<LogicGameObject> characters = m_parent.GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER);

				m_rangePosition.m_x = m_parent.GetMidX();
				m_rangePosition.m_y = m_parent.GetMidY();

				int allianceAlertRadiusSquared = LogicDataTables.GetGlobals().GetAllianceAlertRadius() * LogicDataTables.GetGlobals().GetAllianceAlertRadius();

				for (int i = 0; i < characters.Size(); i++)
				{
					LogicCharacter character = (LogicCharacter)characters[i];

					if (m_rangePosition.GetDistanceSquared(character.GetPosition()) < allianceAlertRadiusSquared)
					{
						LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

						if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
						{
							LogicCombatComponent combatComponent = character.GetCombatComponent();

							if (combatComponent != null)
							{
								combatComponent.StartAllianceAlert(gameObject, m_parent);
							}
						}
					}
				}
			}
		}

		public void Hit(int idx)
		{
			if (m_attackerData.GetAmmoCount() > 0 && m_ammo <= 0)
			{
				return;
			}

			LogicGameObject target = m_targets[idx];

			if (m_attackerData.GetTargetGroups() && m_parent.GetMovementComponent() == null && m_targetGroup != null)
			{
				target = m_targetGroup;
			}

			// Listener.

			if (m_spawnOnAttack)
			{
				m_parent.SpawnEvent(null, 0, 0);
				HitCompleted();

				return;
			}

			bool altAttackMode = m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()];
			bool flyingTarget = target != null && target.IsFlying();

			int damage = GetDamage();

			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;

				if (character.GetSpecialAbilityAvailable())
				{
					LogicCharacterData data = character.GetCharacterData();

					if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE)
					{
						altAttackMode = true;
					}

					if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_BIG_FIRST_HIT)
					{
						damage = damage * data.GetSpecialAbilityAttribute(character.GetUpgradeLevel()) / 100;
					}
				}
			}

			// Listener.

			if (m_attackerData.GetProjectile(altAttackMode) == null)
			{
				if (damage >= 0 || !m_parent.IsPreventsHealing())
				{
					if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						LogicVector2 shield = new LogicVector2(target.GetMidX(), target.GetMidY());

						if (m_parent.GetLevel().GetAreaShield(target.GetMidX(), target.GetMidY(), shield))
						{
							damage = shield.m_y * damage / 100;
						}
					}

					if (m_attackerData.GetDamageRadius() > 0)
					{
						target.GetListener().PlayEffect(m_attackerData.GetHitEffect2());

						int midX;
						int midY;

						if (m_attackerData.IsSelfAsAoeCenter())
						{
							midX = m_parent.GetMidX();
							midY = m_parent.GetMidY();
						}
						else
						{
							midX = target.GetMidX();
							midY = target.GetMidY();
						}

						m_parent.GetLevel().AreaDamage(m_parent.GetGlobalID(), midX, midY, m_attackerData.GetDamageRadius(), damage, m_preferredTarget,
															m_preferredTargetDamageMod, null, m_parent.GetHitpointComponent().GetTeam(), null, flyingTarget ? 0 : 1, 0,
															m_attackerData.GetPushBack(), true, false, 100, 0, m_parent, 100, 0);
					}
					else
					{
						if (LogicCombatComponent.IsPreferredTarget(m_preferredTarget, target))
						{
							damage = m_preferredTargetDamageMod * damage / 100;
						}

						if (damage < 0)
						{
							LogicGameObjectData targetData = target.GetData();

							if (targetData.GetDataType() == DataType.HERO)
							{
								damage = LogicDataTables.GetGlobals().GetHeroHealMultiplier() * damage / 100;
							}
						}

						if (m_skeletonSpell)
						{
							LogicGameObjectData targetData = target.GetData();

							if (targetData.GetDataType() == DataType.BUILDING)
							{
								LogicBuildingData buildingData = (LogicBuildingData)targetData;

								if (buildingData.GetMaxStoredGold(0) > 0 ||
									buildingData.GetMaxStoredElixir(0) > 0 ||
									buildingData.GetMaxStoredDarkElixir(0) > 0 ||
									buildingData.IsTownHall() ||
									buildingData.IsTownHallVillage2())
								{
									damage = LogicDataTables.GetGlobals().GetSkeletonSpellStorageMultipler() * damage / 100;
								}
							}
						}

						// Listener.

						LogicHitpointComponent targetHitpointComponent = target.GetHitpointComponent();

						targetHitpointComponent.CauseDamage(damage, m_parent.GetGlobalID(), m_parent);

						if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()] &&
							m_altMultiTargets &&
							targetHitpointComponent.GetHitpoints() == 0)
						{
							m_hideTime[idx] = m_attackerData.GetAlternatePickNewTargetDelay();
						}

						if (targetHitpointComponent.GetHitpoints() == 0 && m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)m_parent;
							LogicCharacterData data = character.GetCharacterData();

							if (data.IsUnderground())
							{
								m_hideTime[idx] = LogicDataTables.GetGlobals().GetMinerHideTime() +
													   m_parent.Rand(0) % LogicDataTables.GetGlobals().GetMinerHideTimeRandom();
							}
						}

						if (m_attackerData.GetPreventsHealing() && targetHitpointComponent.GetInvulnerabilityTime() <= 0)
						{
							target.SetPreventsHealingTime(30);
						}

						if (m_attackerData.GetChainAttackDistance() > 0)
						{
							m_targets[1] = null;
							m_targets[2] = null;
							m_targets[3] = null;
							m_targets[4] = null;

							LogicGameObject chainAttackTarget = GetChainAttackTarget(target.GetX(), target.GetY(), m_attackerData.GetChainAttackDistance());

							if (chainAttackTarget != null)
							{
								m_targets[1] = chainAttackTarget;

								// Listener.

								chainAttackTarget.GetHitpointComponent().CauseDamage(damage, m_parent.GetGlobalID(), m_parent);
							}
						}
					}
				}

				if (m_attackerData.GetDamage2() != 0)
				{
					m_damage2Time[idx] = m_attackerData.GetDamage2Delay();
					m_damage2X[idx] = target.GetMidX();
					m_damage2Y[idx] = target.GetMidY();
				}
			}
			else
			{
				LogicProjectileData projectileData = m_attackerData.GetProjectile(altAttackMode);

				if (m_attackerData.GetRageProjectile() != null)
				{
					if (m_parent.IsHero())
					{
						if (m_parent.IsStealthy())
						{
							projectileData = m_attackerData.GetRageProjectile();
						}
					}
					else
					{
						if (m_boostTime[0] > 0)
						{
							projectileData = m_attackerData.GetRageProjectile();
						}
					}
				}

				int burstCount = 1;

				if (m_attackerData.GetBurstCount() > 0 && m_attackerData.GetBurstDelay() == 0)
				{
					burstCount = m_attackerData.GetBurstCount();
				}

				if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
				{
					burstCount = 1;

					if (m_attackerData.GetAltBurstCount() > 0 && m_attackerData.GetAltBurstDelay() == 0)
					{
						burstCount = m_attackerData.GetAltBurstCount();
					}
				}

				for (int i = 0; i < burstCount; i++)
				{
					LogicProjectile projectile =
						(LogicProjectile)LogicGameObjectFactory.CreateGameObject(projectileData, m_parent.GetLevel(), m_parent.GetVillageType());

					projectile.SetInitialPosition(m_parent, m_parent.GetMidX(), m_parent.GetMidY());
					projectile.SetBounceCount(m_attackerData.GetProjectileBounces());

					if (i >= burstCount - m_attackerData.GetDummyProjectileCount())
					{
						projectile.SetDummyProjectile(true);
					}

					int team = m_parent.GetHitpointComponent()?.GetTeam() ?? -1;

					if (target != null || m_targetGroups.Size() <= 0 && m_burstTime[0] <= 0)
					{
						if (m_attackerData.GetShockwavePushStrength() != 0 || m_attackerData.IsPenetratingProjectile())
						{
							int attackRange = GetAttackRange(m_parent.GetLevel().GetCurrentLayout(), false) + m_attackerData.GetPenetratingExtraRange() + 256;

							LogicVector2 penetrating = new LogicVector2(attackRange, 0);

							penetrating.Rotate(m_parent.GetDirection());

							if (m_attackerData.GetShockwavePushStrength() != 0)
							{
								projectile.SetTargetPos(m_parent.GetMidX(), m_parent.GetMidY(), m_parent.GetMidX() + penetrating.m_x,
														m_parent.GetMidY() + penetrating.m_y, m_attackerData.GetMinAttackRange(), attackRange + 256,
														m_parent.GetDirection(), m_attackerData.GetShockwavePushStrength(), m_attackerData.GetShockwaveArcLength(),
														m_attackerData.GetShockwaveExpandRadius(), team, flyingTarget);
							}
							else if (m_attackerData.IsPenetratingProjectile())
							{
								projectile.SetTargetPos(m_parent.GetMidX() + penetrating.m_x, m_parent.GetMidY() + penetrating.m_y, team, flyingTarget);
								projectile.SetPenetratingRadius(m_attackerData.GetPenetratingRadius());
							}
						}
						else if (projectileData.GetTrackTarget())
						{
							projectile.SetTarget(m_parent.GetMidX(), m_parent.GetMidY(), m_hitCount, target, projectileData.GetRandomHitPosition());
						}
						else
						{
							int midX = target.GetMidX();
							int midY = target.GetMidY();

							if (projectileData.GetTargetPosRandomRadius() != 0)
							{
								int randomRadius = projectileData.GetTargetPosRandomRadius();

								if (m_attackerData.GetDummyProjectileCount() > 0 && i == 0)
								{
									randomRadius = 0;
								}

								midX += m_random.Rand(2 * randomRadius) - randomRadius;
								midY += m_random.Rand(2 * randomRadius) - randomRadius;
							}

							projectile.SetTargetPos(midX, midY, team, flyingTarget);
						}
					}
					else
					{
						projectile.SetTargetPos(m_unk604.m_x, m_unk604.m_y, team, flyingTarget);
					}

					projectile.SetInitialPosition(m_parent, m_parent.GetMidX(), m_parent.GetMidY());

					if (target != null)
					{
						if (damage < 0 && target.IsPreventsHealing())
						{
							damage = 0;
						}
					}

					bool specialProjectile = false;

					if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)m_parent;
						LogicCharacterData characterData = character.GetCharacterData();

						if (characterData.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE && character.GetSpecialAbilityAvailable())
						{
							damage = damage * characterData.GetSpecialAbilityAttribute3(character.GetUpgradeLevel()) / 100;
							projectile.SetHitEffect(characterData.GetSpecialAbilityEffect(character.GetUpgradeLevel()), null);
							specialProjectile = true;
						}
					}

					projectile.SetDamage(damage);
					projectile.SetPreferredTargetDamageMod(m_preferredTarget, m_preferredTargetDamageMod);
					projectile.SetDamageRadius(m_attackerData.GetDamageRadius());
					projectile.SetPushBack(m_attackerData.GetPushBack(), true);

					if (!specialProjectile || projectile.GetHitEffect() == null)
					{
						projectile.SetHitEffect(m_attackerData.GetHitEffect(), m_attackerData.GetHitEffect2());
					}

					projectile.SetSpeedMod(m_attackerData.GetSpeedMod());
					projectile.SetStatusEffectTime(m_attackerData.GetStatusEffectTime());
					projectile.SetMyTeam(team);

					m_parent.GetGameObjectManager().AddGameObject(projectile, -1);

					if (target != null && m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)m_parent;
						LogicCharacterData data = character.GetCharacterData();

						int chainShootingDistance = data.GetChainShootingDistance();

						int distanceX = target.GetMidX() - m_parent.GetMidX();
						int distanceY = target.GetMidY() - m_parent.GetMidY();

						int distance = LogicMath.Sqrt(distanceX * distanceX + distanceY * distanceY);

						if (chainShootingDistance > 0 && distance > 0)
						{
							int chainedProjectileBounceCount = LogicDataTables.GetGlobals().GetChainedProjectileBounceCount();
							int chainShootingDistanceTile = (chainShootingDistance << 9) / 100;

							if (chainedProjectileBounceCount > 1)
							{
								distanceX = 255 * distanceX / distance;
								distanceY = 255 * distanceY / distance;

								int offsetX = chainShootingDistanceTile * distanceX / 255;
								int offsetY = chainShootingDistanceTile * distanceY / 255;

								int posX = target.GetMidX() + offsetX;
								int posY = target.GetMidY() + offsetY;

								for (int j = 0; j < chainedProjectileBounceCount - 1; j++)
								{
									projectile.SetBouncePosition(new LogicVector2(posX, posY));

									posX += offsetX;
									posY += offsetY;
								}
							}
						}
					}
				}
			}

			if (m_attackerData.GetHitSpell() != null)
			{
				LogicSpell spell =
					(LogicSpell)LogicGameObjectFactory.CreateGameObject(m_attackerData.GetHitSpell(), m_parent.GetLevel(), m_parent.GetVillageType());

				spell.SetUpgradeLevel(m_attackerData.GetHitSpellLevel());
				spell.SetInitialPosition(target.GetMidX(), target.GetMidY());
				spell.SetTeam(m_parent.GetHitpointComponent().GetTeam());

				m_parent.GetGameObjectManager().AddGameObject(spell, -1);
			}

			if (m_attackerData.GetAmmoCount() > 0 && m_ammoReloadingTime <= 0)
			{
				m_ammo -= 1;
				m_ammoReloadingTime = m_ammoReloadingTotalTime;

				if (m_ammo == 0)
				{
					// Listener.
				}
			}

			if (target != null)
			{
				LogicCombatComponent combatComponent = target.GetCombatComponent();

				if (combatComponent != null)
				{
					combatComponent.AttackedBy(m_parent);
				}
			}

			HitCompleted();

			if (m_troopChild)
			{
				ForceNewTarget();
			}

			if (!m_unk502)
			{
				if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)m_parent;

					if (character.GetParent() != null)
					{
						character.GetParent().GetCombatComponent().HitCompleted();
					}
				}
			}
		}

		public void HitCompleted()
		{
			m_hitCount += 1;

			if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;
				LogicCharacterData data = character.GetCharacterData();

				if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE)
				{
					if (m_targets[0] != null && !IsInRange(m_targets[0]))
					{
						StopAttack();
						RefreshTarget(true);
					}
				}
			}
		}

		public LogicGameObject GetChainAttackTarget(int midX, int midY, int attackDistance)
		{
			m_enemyList.Clear();
			m_parent.GetGameObjectManager().GetGameObjects(m_enemyList, m_enemyFilter);

			int minDistance = 0x7FFFFFFF;
			LogicGameObject closestGameObject = null;

			for (int i = 0; i < m_enemyList.Size(); i++)
			{
				LogicGameObject gameObject = m_enemyList[i];

				if (!gameObject.IsHidden() && CanAttackHeightCheck(gameObject))
				{
					if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)gameObject;

						if (character.GetSpawnDelay() > 0 || character.GetSpawnIdleTime() > 0 || character.GetCombatComponent()?.GetUndergroundTime() > 0 ||
							character.GetChildTroops() != null)
						{
							continue;
						}
					}

					bool isTarget = false;

					for (int j = 0; j < 5; j++)
					{
						if (gameObject == m_targets[j])
						{
							isTarget = true;
						}
					}

					if (!isTarget)
					{
						int distanceX = (gameObject.GetMidX() - midX) >> 9;
						int distanceY = (gameObject.GetMidY() - midY) >> 9;
						int distance = distanceX * distanceX + distanceY * distanceY;

						if (distance < minDistance)
						{
							minDistance = distance;
							closestGameObject = gameObject;
						}
					}
				}
			}

			if (minDistance < attackDistance * attackDistance)
			{
				return closestGameObject;
			}

			return null;
		}

		public int GetDamage()
		{
			int damage = m_damage;

			if (m_altMultiTargets)
			{
				if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
				{
					damage = m_attackerData.GetDamage(0, m_attackerData.GetMultiTargets(true));
					goto CONTINUE;
				}
			}
			else if (m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
			{
				damage = m_attackerData.GetAltDamage(0, m_attackerData.GetMultiTargets(true));
				goto CONTINUE;
			}

			if (m_attackerData.IsIncreasingDamage())
			{
				int damageType = GetDamageLevel();

				if (damageType != 0)
				{
					damage = m_attackerData.GetDamage(damageType, false);
				}
			}

		CONTINUE:

			int slowDamage = 0;
			int boostDamage = 0;
			int mergeDamage = 0;

			if (m_boostTime[0] > 0)
			{
				boostDamage = (int)((long)m_boostDamage[0] * damage / 100L);
			}

			if (m_slowTime > 0)
			{
				slowDamage = (int)((long)m_slowDamage * damage / 100L);
			}

			if (m_mergeDamage > 0)
			{
				mergeDamage = (int)((long)m_mergeDamage * damage / 100L);
			}

			return damage + boostDamage + slowDamage + mergeDamage;
		}

		public int GetDamageLevel()
		{
			int damageTime = 0;

			if (m_unk502)
			{
				damageTime = m_damageTime;
			}
			else if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)m_parent;
				LogicGameObject gameObject = character.GetParent() == null ? m_parent : character.GetParent();

				damageTime = gameObject.GetCombatComponent().m_hitCount;
			}

			if (!m_attackerData.IsIncreasingDamage() || m_altMultiTargets && m_useAltAttackMode[m_parent.GetLevel().GetCurrentLayout()])
			{
				return 0;
			}

			int damageLevel = 0;

			if (damageTime >= m_attackerData.GetSwitchTimeLv2())
			{
				damageLevel = 1;
			}

			if (damageTime >= m_attackerData.GetSwitchTimeLv3())
			{
				damageLevel = 2;
			}

			return damageLevel;
		}

		public void SetSkeletonSpell()
		{
			m_skeletonSpell = true;
		}

		public int GetWakeUpTime()
			=> m_wakeUpTime;

		public int GetDeployedHousingSpace()
			=> m_deployedHousingSpace;

		public void SetMergeDamage(int damage)
		{
			m_mergeDamage = damage;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.COMBAT;

		public override void Load(LogicJSONObject jsonObject)
		{
			if (m_hasAltAttackMode)
			{
				for (int i = 0; i < 8; i++)
				{
					LogicJSONBoolean attackModeObject = jsonObject.GetJSONBoolean(GetLayoutVariableNameAttackMode(i, false));

					if (attackModeObject != null)
					{
						m_useAltAttackMode[i] = attackModeObject.IsTrue();
					}

					LogicJSONBoolean draftAttackModeObject = jsonObject.GetJSONBoolean(GetLayoutVariableNameAttackMode(i, true));

					if (draftAttackModeObject != null)
					{
						m_draftUseAltAttackMode[i] = draftAttackModeObject.IsTrue();
					}
				}
			}

			if (m_attackerData.GetAmmoCount() > 0)
			{
				LogicJSONNumber ammoObject = jsonObject.GetJSONNumber("ammo");
				m_ammo = ammoObject != null ? ammoObject.GetIntValue() : 0;
			}

			if (m_attackerData.GetTargetingConeAngle() > 0)
			{
				for (int i = 0; i < 8; i++)
				{
					LogicJSONNumber aimAngleObject = jsonObject.GetJSONNumber(GetLayoutVariableNameAimAngle(i, false));

					if (aimAngleObject != null)
					{
						m_aimAngle[i] = aimAngleObject.GetIntValue();
					}

					LogicJSONNumber draftAimAngleObject = jsonObject.GetJSONNumber(GetLayoutVariableNameAimAngle(i, true));

					if (draftAimAngleObject != null)
					{
						m_draftAimAngle[i] = draftAimAngleObject.GetIntValue();
					}
				}
			}
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			if (m_hasAltAttackMode)
			{
				for (int i = 0; i < 8; i++)
				{
					LogicJSONBoolean attackModeObject = jsonObject.GetJSONBoolean(GetLayoutVariableNameAttackMode(i, false));

					if (attackModeObject != null)
					{
						m_useAltAttackMode[i] = attackModeObject.IsTrue();
					}
				}
			}

			if (m_attackerData.GetAmmoCount() > 0)
			{
				m_ammo = m_attackerData.GetAmmoCount();
			}

			if (m_attackerData.GetTargetingConeAngle() > 0)
			{
				for (int i = 0; i < 8; i++)
				{
					LogicJSONNumber aimAngleObject = jsonObject.GetJSONNumber(GetLayoutVariableNameAimAngle(i, false));

					if (aimAngleObject != null)
					{
						m_aimAngle[i] = aimAngleObject.GetIntValue();
					}
				}
			}
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			if (m_hasAltAttackMode)
			{
				for (int i = 0; i < 8; i++)
				{
					jsonObject.Put(GetLayoutVariableNameAttackMode(i, false), new LogicJSONBoolean(m_useAltAttackMode[i]));
					jsonObject.Put(GetLayoutVariableNameAttackMode(i, true), new LogicJSONBoolean(m_draftUseAltAttackMode[i]));
				}
			}

			if (m_ammo > 0)
			{
				jsonObject.Put("ammo", new LogicJSONNumber(m_ammo));
			}

			if (m_attackerData.GetTargetingConeAngle() > 0)
			{
				for (int i = 0; i < 8; i++)
				{
					jsonObject.Put(GetLayoutVariableNameAimAngle(i, false), new LogicJSONNumber(m_aimAngle[i]));
					jsonObject.Put(GetLayoutVariableNameAimAngle(i, true), new LogicJSONNumber(m_draftAimAngle[i]));
				}
			}
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			if (m_hasAltAttackMode)
			{
				for (int i = 0; i < 8; i++)
				{
					jsonObject.Put(GetLayoutVariableNameAttackMode(i, false), new LogicJSONBoolean(m_useAltAttackMode[layoutId]));
				}
			}

			if (m_ammo > 0)
			{
				jsonObject.Put("ammo", new LogicJSONNumber(m_ammo));
			}

			if (m_attackerData.GetTargetingConeAngle() > 0)
			{
				for (int i = 0; i < 8; i++)
				{
					jsonObject.Put(GetLayoutVariableNameAimAngle(i, false), new LogicJSONNumber(m_aimAngle[layoutId]));
				}
			}
		}

		public string GetLayoutVariableNameAttackMode(int idx, bool draftMode)
		{
			if (draftMode)
			{
				switch (idx)
				{
					case 0:
						return "attack_mode_draft";
					case 1:
						return "attack_mode_d1";
					case 2:
						return "attack_mode_d2";
					case 3:
						return "attack_mode_d3";
					case 4:
						return "attack_mode_d4";
					case 5:
						return "attack_mode_d5";
					case 6:
						return "attack_mode_dchal";
					case 7:
						return "attack_mode_draft_arrw";
					default:
						Debugger.Error("Layout index out of bounds");
						return "attack_mode_draft";
				}
			}

			switch (idx)
			{
				case 0:
					return "attack_mode";
				case 1:
					return "attack_mode1";
				case 2:
					return "attack_mode2";
				case 3:
					return "attack_mode3";
				case 4:
					return "attack_mode4";
				case 5:
					return "attack_mode5";
				case 6:
					return "attack_mode_chal";
				case 7:
					return "attack_mode_arrw";
				default:
					Debugger.Error("Layout index out of bounds");
					return "attack_mode";
			}
		}

		public string GetLayoutVariableNameAimAngle(int idx, bool draftMode)
		{
			if (draftMode)
			{
				switch (idx)
				{
					case 0:
						return "aim_angle_draft";
					case 1:
						return "aim_angle_d1";
					case 2:
						return "aim_angle_d2";
					case 3:
						return "aim_angle_d3";
					case 4:
						return "aim_angle_d4";
					case 5:
						return "aim_angle_d5";
					case 6:
						return "aim_angle_dchal";
					case 7:
						return "aim_angle_draft_arrw";
					default:
						Debugger.Error("Layout index out of bounds");
						return "aim_angle_draft";
				}
			}

			switch (idx)
			{
				case 0:
					return "aim_angle";
				case 1:
					return "aim_angle1";
				case 2:
					return "aim_angle2";
				case 3:
					return "aim_angle3";
				case 4:
					return "aim_angle4";
				case 5:
					return "aim_angle5";
				case 6:
					return "aim_angle5_chal";
				case 7:
					return "aim_angle_arrw";
				default:
					Debugger.Error("Layout index out of bounds");
					return "aim_angle";
			}
		}

		public static bool IsPreferredTarget(LogicData target, LogicGameObject gameObject)
		{
			if (target != null && gameObject != null)
			{
				LogicGameObjectData data = gameObject.GetData();

				if (target.GetDataType() != DataType.BUILDING_CLASS || data.GetDataType() != DataType.BUILDING)
				{
					if (target.GetDataType() != DataType.CHARACTER || data.GetDataType() != DataType.CHARACTER ||
						((LogicCharacterData)data).IsSecondaryTroop() ||
						((LogicCharacter)gameObject).GetSecondaryTroopTeam() == 0)
					{
						return data == target;
					}
				}
				else
				{
					LogicBuildingData buildingData = (LogicBuildingData)data;

					if (buildingData.GetBuildingClass() == target)
					{
						return true;
					}

					if (buildingData.GetSecondaryTargetingClass() != null)
					{
						return buildingData.GetSecondaryTargetingClass() == target;
					}
				}
			}

			return false;
		}
	}
}