using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicProjectile : LogicGameObject
	{
		public const int MAX_BOUNCES = 4;

		private int m_minAttackRange;
		private int m_maxAttackRange;
		private int m_bounceCount;
		private int m_damage; // 176
		private int m_damageTime;
		private int m_damageRadius;
		private int m_groupsId;
		private int m_randomHitRange;
		private int m_shockwaveAngle;
		private int m_shockwavePushStrength;
		private int m_shockwaveArcLength;
		private int m_shockwaveExpandRadius;
		private int m_myTeam;
		private int m_penetratingRadius;
		private int m_preferredTargetDamageMod;
		private int m_pushBack; // 212
		private int m_speedMod; // 216
		private int m_statusEffectTime; // 220
		private int m_travelTime; // 268
		private int m_areaShieldSpeed;
		private int m_areaShieldDelay;

		private bool m_gravity; // 210
		private bool m_bounceProjectile; // 322
		private bool m_targetGroups; // 322
		private bool m_flyingTarget; // 209
		private bool m_penetrating; // 264
		private bool m_dummy; // 323
		private bool m_targetReached; // 208

		private LogicGameObject m_groups;
		private LogicGameObject m_target; // 132
		private LogicData m_preferredTarget;
		private LogicEffectData m_hitEffect;
		private LogicEffectData m_hitEffect2;
		private readonly LogicGameObject[] m_bounceTargets;
		private readonly LogicVector2[] m_bouncePositions;

		private readonly LogicVector2 m_targetPosition; // 136
		private readonly LogicVector2 m_unk144;
		private readonly LogicVector2 m_unk152;
		private readonly LogicVector2 m_unk160;
		private readonly LogicVector2 m_unk168;
		private readonly LogicVector2 m_unk248;
		private readonly LogicVector2 m_unk276;

		public LogicProjectile(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			m_myTeam = -1;
			m_targetPosition = new LogicVector2();
			m_unk144 = new LogicVector2();
			m_unk152 = new LogicVector2();
			m_unk160 = new LogicVector2();
			m_unk168 = new LogicVector2();
			m_unk248 = new LogicVector2();
			m_unk276 = new LogicVector2();

			m_bounceTargets = new LogicGameObject[LogicProjectile.MAX_BOUNCES];
			m_bouncePositions = new LogicVector2[LogicProjectile.MAX_BOUNCES];
		}

		public LogicProjectileData GetProjectileData()
			=> (LogicProjectileData)m_data;

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.PROJECTILE;

		public override void Destruct()
		{
			base.Destruct();

			for (int i = 0; i < LogicProjectile.MAX_BOUNCES; i++)
			{
				m_bouncePositions[i]?.Destruct();
				m_bouncePositions[i] = null;
			}

			m_targetPosition.Destruct();
			m_unk144.Destruct();
			m_unk152.Destruct();
			m_unk160.Destruct();
			m_unk168.Destruct();
			m_unk248.Destruct();
			m_unk276.Destruct();
		}

		public override void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			if (m_target == gameObject)
			{
				m_target = null;
			}

			if (m_groups == gameObject)
			{
				m_groups = null;

				if (m_targetGroups)
				{
					m_target = null;
					m_targetGroups = false;
				}
			}
		}

		public override int GetWidthInTiles()
			=> 0;

		public override int GetHeightInTiles()
			=> 0;

		public override bool ShouldDestruct()
			=> m_targetReached && m_damageTime == -0x80000000;

		public override bool IsUnbuildable()
			=> true;

		public bool IsDummyProjectile()
			=> m_dummy;

		public void SetDummyProjectile(bool dummy)
		{
			m_dummy = dummy;
		}

		public LogicEffectData GetHitEffect()
			=> m_hitEffect;

		public void SetHitEffect(LogicEffectData hitEffect, LogicEffectData hitEffect2)
		{
			m_hitEffect = hitEffect;
			m_hitEffect2 = hitEffect2;
		}

		public void SetDamage(int damage)
		{
			m_damage = damage;
		}

		public void SetPushBack(int force, bool enabled)
		{
			m_pushBack = force;
			m_gravity = enabled;
		}

		public void SetSpeedMod(int speed)
		{
			m_speedMod = speed;
		}

		public void SetStatusEffectTime(int time)
		{
			m_statusEffectTime = time;
		}

		public void SetBounceCount(int value)
		{
			m_bounceCount = value;

			if (value > LogicProjectile.MAX_BOUNCES)
			{
				Debugger.Warning("LogicProjectile::setBounceCount() called with too high value, clamping to MAX_BOUNCES");
				m_bounceCount = LogicProjectile.MAX_BOUNCES;
			}
		}

		public void SetInitialPosition(LogicGameObject groups, int x, int y)
		{
			m_groups = groups;
			m_groupsId = groups != null ? groups.GetGlobalID() : 0;
			m_unk144.m_x = m_targetPosition.m_x - 8 * x;
			m_unk144.m_y = m_targetPosition.m_y - 8 * y;

			m_unk144.Normalize((GetProjectileData().GetStartOffset() << 9) / 100);
			SetInitialPosition(m_unk144.m_x + x, m_unk144.m_y + y);

			m_unk160.m_x = 0;
			m_unk160.m_y = 0;
			m_unk248.m_x = GetX();
			m_unk248.m_y = GetY();
			m_unk276.m_x = GetX() * 8;
			m_unk276.m_y = GetY() * 8;
			m_unk152.m_x = m_targetPosition.m_x - m_unk276.m_x;
			m_unk152.m_y = m_targetPosition.m_y - m_unk276.m_y;

			m_targetGroups = false;

			if (m_groups != null && m_groups.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicCombatComponent combatComponent = m_groups.GetCombatComponent();

				if (combatComponent != null && combatComponent.GetAttackerItemData().GetTargetGroups())
				{
					m_targetGroups = true;
				}
			}
		}

		public void SetTarget(int x, int y, int randomHitRange, LogicGameObject target, bool randomHitPosition)
		{
			m_target = target;
			m_targetPosition.m_x = target.GetMidX() * 8;
			m_targetPosition.m_y = target.GetMidY() * 8;

			if (target.GetGameObjectType() == LogicGameObjectType.CHARACTER)
			{
				LogicCharacter character = (LogicCharacter)target;

				if (character.IsFlying())
				{
					LogicCombatComponent combatComponent = target.GetCombatComponent();

					m_randomHitRange = combatComponent != null && combatComponent.IsHealer() ? 200 : 1000;
					m_flyingTarget = true;
				}

				if (randomHitPosition)
				{
					LogicVector2 pos = new LogicVector2(m_targetPosition.m_x >> 3, m_targetPosition.m_y >> 3);

					int distance = pos.GetDistance(GetPosition());

					m_unk168.m_x = m_targetPosition.m_x - 8 * GetMidX();
					m_unk168.m_y = m_targetPosition.m_y - 8 * GetMidY();

					m_unk168.Rotate(90);
					m_unk168.Normalize(64);

					int rnd = ((distance / 10) & Rand(randomHitRange)) - distance / 20;

					m_unk168.m_x = m_unk168.m_x * rnd / 64;
					m_unk168.m_y = m_unk168.m_y * rnd / 64;

					pos.Destruct();
				}
			}
			else
			{
				int range = target.IsWall() ? 1016 : 2040;

				m_unk168.m_x = 8 * x - m_targetPosition.m_x;
				m_unk168.m_y = 8 * y - m_targetPosition.m_y;

				m_unk168.Normalize(((target.GetWidthInTiles() - target.PassableSubtilesAtEdge()) << 12) / 3);

				m_unk168.m_x += (range & Rand(randomHitRange)) * (2 * (Rand(randomHitRange + 1) & 1) - 1);
				m_unk168.m_y += (range & Rand(randomHitRange + 2)) * (2 * (Rand(randomHitRange + 3) & 1) - 1);

				m_targetPosition.Add(m_unk168);

				m_randomHitRange = 150;
			}
		}

		public void SetTargetPos(int x, int y, int team, bool flyingTarget)
		{
			m_targetPosition.m_x = x * 8;
			m_targetPosition.m_y = y * 8;
			m_myTeam = team;
			m_randomHitRange = flyingTarget ? 1000 : 0;
			m_flyingTarget = flyingTarget;
		}

		public void SetTargetPos(int startX, int startY, int x, int y, int minAttackRange, int maxAttackRange, int shockwaveAngle, int shockwavePushStrength,
								 int shockwaveArcLength,
								 int shockwaveExpandRadius, int team, bool flyingTarget)
		{
			m_targetPosition.m_x = x * 8;
			m_targetPosition.m_y = y * 8;
			m_myTeam = team;
			m_randomHitRange = flyingTarget ? 1000 : 0;
			m_flyingTarget = flyingTarget;
			m_unk248.m_x = startX;
			m_unk248.m_y = startY;
			m_shockwaveAngle = shockwaveAngle;
			m_shockwaveArcLength = shockwaveArcLength;
			m_shockwaveExpandRadius = shockwaveExpandRadius;
			m_shockwavePushStrength = shockwavePushStrength;
			m_minAttackRange = minAttackRange;
			m_maxAttackRange = maxAttackRange;
		}

		public void SetMyTeam(int team)
		{
			m_myTeam = team;
		}

		public void SetPenetratingRadius(int radius)
		{
			m_penetrating = true;
			m_penetratingRadius = radius;
		}

		public void SetDamageRadius(int radius)
		{
			m_damageRadius = radius;
		}

		public void SetPreferredTargetDamageMod(LogicData data, int preferredTargetDamageMod)
		{
			m_preferredTarget = data;
			m_preferredTargetDamageMod = preferredTargetDamageMod;
		}

		public int GetTargetX()
			=> m_targetPosition.m_x >> 3;

		public int GetTargetY()
			=> m_targetPosition.m_y >> 3;

		public void SetBouncePosition(LogicVector2 pos)
		{
			m_bounceProjectile = true;

			for (int i = 0; i < m_bouncePositions.Length; i++)
			{
				if (m_bouncePositions[i] == null)
				{
					m_bouncePositions[i] = pos;
					break;
				}
			}
		}

		public int GetShockwaveArcLength()
		{
			LogicVector2 pos = new LogicVector2(GetMidX() - (m_targetPosition.m_x >> 3), GetMidY() - (m_targetPosition.m_y >> 3));

			int length = pos.GetLength();
			int shockwaveLength = m_maxAttackRange - length;

			if (shockwaveLength >= m_minAttackRange)
			{
				int arcLength = (m_shockwaveArcLength << 9) / 100;
				int expandedArcLength = LogicMath.Clamp(shockwaveLength * arcLength / ((m_shockwaveExpandRadius << 9) / 100), 0, arcLength);

				if (expandedArcLength < 0)
				{
					expandedArcLength = arcLength;
				}

				if (expandedArcLength > arcLength)
				{
					expandedArcLength = arcLength;
				}

				int calculateArcLength = 18000 * expandedArcLength / (314 * shockwaveLength);

				if (calculateArcLength < 180)
				{
					return calculateArcLength;
				}

				return 180;
			}

			return 0;
		}

		public void UpdateDamage(int percentage)
		{
			m_damageTime -= 16;

			if (m_damageTime <= 0)
			{
				m_damageTime = -0x80000000;
				int damage = m_damage * percentage / 100;

				if (m_target != null && m_target.GetHitpointComponent() != null)
				{
					UpdateTargetDamage(m_target, damage);
				}

				if (m_target == null)
				{
					if (!m_penetrating)
					{
						if (m_damageRadius > 0 && m_shockwavePushStrength == 0)
						{
							m_level.AreaDamage(m_groupsId, m_targetPosition.m_x >> 3, m_targetPosition.m_y >> 3, m_damageRadius, damage,
													m_preferredTarget,
													m_preferredTargetDamageMod, m_hitEffect, m_myTeam, m_unk160, m_flyingTarget ? 0 : 1, 0,
													m_pushBack,
													m_gravity, false, 100, 0, m_groups, 100, 0);

							if (m_speedMod != 0)
							{
								if (m_statusEffectTime > 0)
								{
									m_level.AreaBoost(m_targetPosition.m_x >> 3, m_targetPosition.m_y >> 3, m_damageRadius, m_speedMod, m_speedMod, 0,
														   0,
														   m_statusEffectTime >> 4, 0, false);
								}
							}

							if (GetProjectileData().GetSlowdownDefensePercent() > 0)
							{
								m_level.AreaBoost(m_targetPosition.m_x >> 3, m_targetPosition.m_y >> 3, m_damageRadius, 0,
													   -GetProjectileData().GetSlowdownDefensePercent(), 120);
							}
						}
					}
				}
			}
		}

		public void UpdateTargetDamage(LogicGameObject target, int damage)
		{
			if (target != null && !m_dummy && target.GetHitpointComponent() != null)
			{
				int totalDamage = damage;

				if (LogicCombatComponent.IsPreferredTarget(m_preferredTarget, target))
				{
					totalDamage = damage * m_preferredTargetDamageMod / 100;
				}

				if (totalDamage >= 0 || target.GetData().GetDataType() == LogicDataType.HERO &&
					(totalDamage = totalDamage * LogicDataTables.GetGlobals().GetHeroHealMultiplier() / 100) > 0 || !target.IsPreventsHealing())
				{
					if (m_damageRadius <= 0)
					{
						target.GetHitpointComponent().CauseDamage(totalDamage, m_groupsId, m_groups);
					}
					else
					{
						m_level.AreaDamage(m_groupsId, target.GetMidX(), target.GetMidY(), m_damageRadius, damage, m_preferredTarget,
												m_preferredTargetDamageMod, m_hitEffect, m_myTeam, m_unk160, m_flyingTarget ? 0 : 1, 0, m_pushBack,
												m_gravity, false, 100, 0, m_groups, 100, 0);
					}
				}

				int slowdownDefensePercent = GetProjectileData().GetSlowdownDefensePercent();

				if (slowdownDefensePercent > 0 && target.GetGameObjectType() == LogicGameObjectType.BUILDING)
				{
					LogicCombatComponent combatComponent = target.GetCombatComponent();

					if (combatComponent != null)
					{
						combatComponent.Boost(100, -slowdownDefensePercent, 120);
					}
				}
			}
		}

		public void UpdateBounces()
		{
			LogicArrayList<LogicGameObject> gameObjects = GetGameObjectManager().GetGameObjects(LogicGameObjectType.BUILDING);

			int closestBuildingDistance = 0x7FFFFFFF;
			int closestWallDistance = 0x7FFFFFFF;

			LogicBuilding closestBuilding = null;
			LogicBuilding closestWall = null;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicBuilding building = (LogicBuilding)gameObjects[i];

				if (building != m_target && building.IsAlive())
				{
					LogicHitpointComponent hitpointComponent = building.GetHitpointComponent();

					if (hitpointComponent != null && hitpointComponent.IsEnemyForTeam(m_myTeam) && !building.IsHidden() && !building.IsWall())
					{
						int distanceSquared = GetDistanceSquaredTo(building);

						if (distanceSquared <= 26214400 && distanceSquared < (building.IsWall() ? closestWallDistance : closestBuildingDistance))
						{
							int idx = -1;

							for (int j = m_bounceCount; j < LogicProjectile.MAX_BOUNCES; j++)
							{
								if (m_bounceTargets[j] == building)
								{
									idx = j;
									break;
								}
							}

							if (idx == -1)
							{
								if (m_level.GetTileMap().GetWallInPassableLine(GetMidX(), GetMidY(), building.GetMidX(), building.GetMidY(), new LogicVector2()))
								{
									if (building.IsWall())
									{
										closestWallDistance = distanceSquared;
										closestWall = building;
									}
									else
									{
										closestBuildingDistance = distanceSquared;
										closestBuilding = building;
									}
								}
							}
						}
					}
				}
			}

			LogicBuilding nextTarget = closestBuilding ?? closestWall;

			if (nextTarget != null)
			{
				m_bounceCount -= 1;
				m_targetReached = false;
				m_damage /= 2;

				SetTarget(GetMidX(), GetMidY(), m_bounceCount, nextTarget, false);
				SetInitialPosition(m_groups, GetMidX(), GetMidY());
			}
		}

		public void UpdateShockwavePush(int team, int targetType)
		{
			LogicVector2 position = new LogicVector2(GetMidX() - m_unk248.m_x, GetMidY() - m_unk248.m_y);
			int length = position.GetLength();

			if (length >= m_minAttackRange)
			{
				int maxRangeDistance = length - m_maxAttackRange;
				int maxRadius = length;
				int minRadius = length - 512;

				if (minRadius < m_minAttackRange)
				{
					minRadius = m_minAttackRange;
				}

				uint minRadiusSquared = (uint)(minRadius * minRadius);
				uint maxRadiusSquared = (uint)(maxRadius * maxRadius);

				int boostSpeed = m_speedMod * maxRangeDistance / m_maxAttackRange;
				int boostTime = m_statusEffectTime * maxRangeDistance / (16 * m_maxAttackRange);
				int shockwaveArcLength = GetShockwaveArcLength();

				LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);
				LogicVector2 pushBackPosition = new LogicVector2();

				for (int i = 0; i < components.Size(); i++)
				{
					LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
					LogicGameObject parent = movementComponent.GetParent();
					LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();

					if (!parent.IsHidden())
					{
						if (hitpointComponent == null || hitpointComponent.GetTeam() != team)
						{
							if (hitpointComponent != null && hitpointComponent.GetParent().IsFlying())
							{
								if (targetType == 1)
								{
									continue;
								}
							}
							else if (targetType == 0)
							{
								continue;
							}

							int distanceX = parent.GetMidX() - m_unk248.m_x;
							int distanceY = parent.GetMidY() - m_unk248.m_y;

							if (LogicMath.Abs(distanceX) <= maxRadius &&
								LogicMath.Abs(distanceY) <= maxRadius)
							{
								int distance = distanceX * distanceX + distanceY * distanceY;

								if (distance <= maxRadiusSquared && distance >= minRadiusSquared)
								{
									if ((distanceX | distanceY) == 0)
									{
										distanceX = 1;
									}

									pushBackPosition.Set(distanceX, distanceY);

									int pushBackLength = pushBackPosition.Normalize(512);
									int angle =
										LogicMath.Abs(LogicMath.NormalizeAngle180(LogicMath.NormalizeAngle180(pushBackPosition.GetAngle()) -
																				  LogicMath.NormalizeAngle180(m_shockwaveAngle)));

									if (angle < shockwaveArcLength / 2)
									{
										int pushBack = 100 * (m_maxAttackRange + 256 - pushBackLength) / 512;

										if (pushBack > m_shockwavePushStrength)
										{
											pushBack = m_shockwavePushStrength;
										}

										movementComponent.GetMovementSystem().ManualPushBack(pushBackPosition, pushBack, 750, m_globalId);

										if (boostSpeed != 0)
										{
											movementComponent.GetMovementSystem().Boost(boostSpeed, boostTime);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void UpdatePenetrating(int damageMultiplier)
		{
			LogicVector2 pos1 = new LogicVector2((m_targetPosition.m_x >> 3) - m_unk248.m_x, (m_targetPosition.m_y >> 3) - m_unk248.m_y);

			pos1.Normalize(512);

			LogicVector2 pos2 = new LogicVector2(-pos1.m_y, pos1.m_x);

			int distance = ((200 - m_areaShieldDelay) * (8 * GetSpeed() - 8 * m_areaShieldSpeed) / 200 + 8 * m_areaShieldSpeed) >> 3;

			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0, damage = damageMultiplier * m_damage / 100; i < components.Size(); i++)
			{
				LogicMovementComponent component = (LogicMovementComponent)components[i];
				LogicGameObject parent = component.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();

				if (!parent.IsHidden() && hitpointComponent.GetTeam() != m_myTeam && hitpointComponent.GetHitpoints() > 0)
				{
					int distanceX = parent.GetMidX() - GetMidX();
					int distanceY = parent.GetMidY() - GetMidY();

					if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						distanceX += parent.GetWidthInTiles() << 8;
						distanceY += parent.GetHeightInTiles() << 8;
					}

					if ((!component.IsFlying() || m_flyingTarget) &&
						LogicMath.Abs(distanceX) <= m_penetratingRadius &&
						LogicMath.Abs(distanceY) <= m_penetratingRadius &&
						distanceX * distanceX + distanceY * distanceY <= (uint)(m_penetratingRadius * m_penetratingRadius))
					{
						LogicVector2 position = new LogicVector2();

						if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER && hitpointComponent.GetMaxHitpoints() <= damage)
						{
							int rnd = (byte)Rand(parent.GetGlobalID());

							if (rnd > 170u)
							{
								position.Set((pos1.m_x >> 2) + pos2.m_x, (pos1.m_y >> 2) + pos2.m_y);
							}
							else
							{
								if (rnd > 85)
								{
									position.Set(pos1.m_x, pos1.m_y);
								}
								else
								{
									position.Set((pos1.m_x >> 2) - pos2.m_x, (pos1.m_y >> 2) - pos2.m_y);
								}
							}

							if (hitpointComponent.GetInvulnerabilityTime() <= 0)
							{
								((LogicCharacter)parent).Eject(position);
							}

							position.Destruct();
						}
						else
						{
							position.Set(pos1.m_x, pos1.m_y);
							position.Normalize(distance);

							if (parent.GetMovementComponent().GetMovementSystem().ManualPushTrap(position, 150, m_globalId) || parent.IsHero())
							{
								UpdateTargetDamage(parent, damage);
							}
						}
					}
				}
			}

			pos1.Destruct();
			pos2.Destruct();
		}

		public override void SubTick()
		{
			base.SubTick();

			m_areaShieldSpeed = 0;

			bool isInAreaShield = false;
			int damagePercentage = 100;

			if (m_myTeam == 1)
			{
				LogicVector2 areaShield = new LogicVector2();

				if (m_level.GetAreaShield(GetMidX(), GetMidY(), areaShield))
				{
					m_areaShieldSpeed = areaShield.m_x;

					isInAreaShield = true;
					damagePercentage = 0;
				}
			}

			if (m_targetReached)
			{
				if (m_damageTime > 0)
				{
					UpdateDamage(damagePercentage);
				}
			}
			else
			{
				if (m_targetGroups)
				{
					if (m_target != null && m_groups != null)
					{
						LogicCombatComponent combatComponent = m_groups.GetCombatComponent();

						if (combatComponent != null && !combatComponent.IsInRange(m_target))
						{
							m_target = null;
						}
					}
				}

				if (isInAreaShield)
				{
					m_areaShieldDelay = LogicMath.Min(m_areaShieldDelay + 16, 200);
				}
				else if (m_areaShieldDelay > 0)
				{
					m_areaShieldDelay = LogicMath.Max(m_areaShieldDelay - 4, 0);
				}

				if (m_areaShieldDelay == 0)
				{
					if (m_target != null && m_target.GetMovementComponent() != null)
					{
						m_targetPosition.Set(m_target.GetMidX() * 8, m_target.GetMidY() * 8);
						m_targetPosition.Add(m_unk168);
					}
				}
				else if (m_target != null && m_target.GetMovementComponent() != null)
				{
					int x = m_unk168.m_x + m_target.GetMidX() * 8;
					int y = m_unk168.m_y + m_target.GetMidY() * 8;

					LogicVector2 tmp1 = new LogicVector2(x - m_unk276.m_x, y - m_unk276.m_y);
					LogicVector2 tmp2 = new LogicVector2(m_unk152.m_x, m_unk152.m_y);

					int length1 = tmp1.Normalize(512);
					int length2 = tmp2.Normalize(512);

					int angle1 = tmp1.GetAngle();
					int angle2 = tmp2.GetAngle();

					if (LogicMath.Abs(LogicMath.NormalizeAngle180(angle1 - angle2)) <= 30)
					{
						m_targetPosition.m_x += LogicMath.Clamp(x - m_targetPosition.m_x, length1 / -500, length1 / 500);
						m_targetPosition.m_y += LogicMath.Clamp(y - m_targetPosition.m_y, length1 / -500, length1 / 500);
					}
					else
					{
						m_target = null;
					}
				}

				m_unk144.m_x = m_targetPosition.m_x - m_unk276.m_x;
				m_unk144.m_y = m_targetPosition.m_y - m_unk276.m_y;

				int distance = (200 - m_areaShieldDelay) * (8 * GetSpeed() - 8 * m_areaShieldSpeed) / 200 + 8 * m_areaShieldSpeed;

				if (distance * distance >= m_unk144.GetDistanceSquaredTo(0, 0))
				{
					TargetReached(damagePercentage);
				}
				else
				{
					m_unk152.m_x = m_unk144.m_x;
					m_unk152.m_y = m_unk144.m_y;

					m_unk144.Normalize(distance);

					m_unk276.m_x += m_unk144.m_x;
					m_unk276.m_y += m_unk144.m_y;

					SetPositionXY(m_unk276.m_x >> 3, m_unk276.m_y >> 3);

					m_unk160.m_x = m_unk144.m_x >> 3;
					m_unk160.m_y = m_unk144.m_y >> 3;
				}

				if (m_shockwavePushStrength > 0)
				{
					UpdateShockwavePush(m_myTeam, m_flyingTarget ? 0 : 1);
				}

				if (m_penetrating)
				{
					UpdatePenetrating(damagePercentage);
				}

				m_travelTime += 16;
			}
		}

		public void TargetReached(int damagePercent)
		{
			m_damageTime = GetProjectileData().GetDamageDelay();
			UpdateDamage(damagePercent);
			m_targetReached = true;

			if (!m_dummy)
			{
				if (m_hitEffect != null)
				{
					if (m_target != null)
					{
						LogicHitpointComponent hitpointComponent = m_target.GetHitpointComponent();

						if (hitpointComponent != null)
						{
							if (!m_bounceProjectile)
							{
								// Listener.
							}
						}
					}
					else if (!m_penetrating && m_shockwavePushStrength == 0)
					{
						// Listener.
					}
				}

				if (m_hitEffect2 != null)
				{
					if (m_target != null)
					{
						LogicHitpointComponent hitpointComponent = m_target.GetHitpointComponent();

						if (hitpointComponent != null)
						{
							if (!m_bounceProjectile)
							{
								// Listener.
							}
						}
					}
					else if (!m_penetrating && m_shockwavePushStrength == 0)
					{
						// Listener.
					}
				}

				if (m_target != null)
				{
					if (m_bounceCount > 0)
					{
						m_bounceTargets[m_bounceCount - 1] = m_target;
						UpdateBounces();
					}
				}

				LogicSpellData hitSpell = GetProjectileData().GetHitSpell();

				if (hitSpell != null)
				{
					LogicSpell spell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(hitSpell, m_level, m_villageType);

					spell.SetUpgradeLevel(GetProjectileData().GetHitSpellLevel());
					spell.SetInitialPosition(GetMidX(), GetMidY());
					spell.SetTeam(1);

					GetGameObjectManager().AddGameObject(spell, -1);
				}

				if (m_bounceProjectile)
				{
					int idx = -1;

					for (int i = 0; i < LogicProjectile.MAX_BOUNCES; i++)
					{
						if (m_bouncePositions[i] != null)
						{
							idx = i;
							break;
						}
					}

					if (idx != -1)
					{
						LogicVector2 bouncePosition = m_bouncePositions[idx];

						m_bouncePositions[idx] = null;
						m_target = null;

						LogicEffectData bounceEffect = GetProjectileData().GetBounceEffect();

						if (bounceEffect != null)
						{
							m_listener.PlayEffect(bounceEffect);
						}

						m_targetPosition.m_x = 8 * bouncePosition.m_x;
						m_targetPosition.m_y = 8 * bouncePosition.m_y;

						m_randomHitRange = m_flyingTarget ? 1000 : 0;

						// Listener.

						m_targetReached = false;
						m_travelTime = 0;

						bouncePosition.Destruct();
					}
					else
					{
						m_target = null;
					}
				}

				if (m_targetReached)
				{
					LogicEffectData destroyedEffect = GetProjectileData().GetDestroyedEffect();

					if (destroyedEffect != null)
					{
						// Listener.
					}
				}
			}
		}

		public int GetSpeed()
		{
			LogicProjectileData projectileData = GetProjectileData();

			if (projectileData.GetFixedTravelTime() != 0)
			{
				LogicVector2 position = new LogicVector2();

				position.m_x = (m_targetPosition.m_x - m_unk276.m_x) >> 3;
				position.m_y = (m_targetPosition.m_y - m_unk276.m_y) >> 3;

				int remMS = projectileData.GetFixedTravelTime() - m_travelTime;
				int speed = position.GetLength();

				if (remMS <= 0)
				{
					remMS = 1000;
					speed = projectileData.GetSpeed();
				}

				return 16 * speed / remMS;
			}

			return (int)(16L * projectileData.GetSpeed() / 1000L);
		}
	}
}