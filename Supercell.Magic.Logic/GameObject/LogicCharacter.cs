using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicCharacter : LogicGameObject
	{
		private int m_upgradeLevel;
		private int m_abilityTriggerTime; // 156
		private int m_abilityTime; // 160
		private int m_abilityCooldown; // 164
		private int m_abilityAttackCount; // 172
		private int m_summonSpawnCount; // 168
		private int m_troopChildX;
		private int m_troopChildY;
		private int m_duplicateLifeTime; // 228
		private int m_dieTime;
		private int m_spawnTime; // 132 
		private int m_spawnIdleTime; // 136
		private int m_secondaryTroopTeam;
		private int m_autoMergeSize;
		private int m_autoMergeTime;
		private int m_loseHpTime;
		private int m_rageAloneTime; // 232
		private int m_activationTimeState; // 236
		private int m_activationTime; // 240

		private bool m_flying;
		private bool m_duplicate; // 225
		private bool m_abilityUsed; // 211
		private bool m_troopChild;
		private bool m_allianceUnit;
		private bool m_ejected;
		private bool m_hasSpawnDelay; // 209
		private bool m_retributionSpellCreated; // 224

		private LogicSpell m_auraSpell; // 212
		private LogicSpell m_abilitySpell; // 216
		private LogicSpell m_retributionSpell; // 220

		private LogicCharacter m_parent;
		private LogicCharacterData m_summoner; // 152
		private readonly LogicVector2 m_ejectPosition;
		private readonly LogicArrayList<LogicCharacter> m_summonTroops;
		private readonly LogicArrayList<LogicCharacter> m_childrens;

		public LogicCharacter(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			LogicCharacterData characterData = (LogicCharacterData)data;

			m_flying = characterData.IsFlying();

			AddComponent(new LogicHitpointComponent(this, characterData.GetHitpoints(0), 0));
			AddComponent(new LogicCombatComponent(this));
			AddComponent(new LogicMovementComponent(this, characterData.GetSpeed(), characterData.IsFlying(), characterData.IsUnderground()));
			SetUpgradeLevel(0);

			int childTroopCount = characterData.GetChildTroopCount();

			if (childTroopCount > 0)
			{
				m_childrens = new LogicArrayList<LogicCharacter>(childTroopCount);

				for (int i = 0; i < childTroopCount; i++)
				{
					LogicCharacter character = new LogicCharacter(characterData.GetChildTroop(), level, villageType);

					character.SetTroopChild(this, i);
					character.GetCombatComponent().SetTroopChild(true);

					m_childrens.Add(character);
					GetGameObjectManager().AddGameObject(character, -1);
				}
			}

			m_ejectPosition = new LogicVector2();
			m_summonTroops = new LogicArrayList<LogicCharacter>();

			if (characterData.IsUnderground())
			{
				GetCombatComponent().SetUndergroundTime(3600000);
			}
		}

		public LogicCharacterData GetCharacterData()
			=> (LogicCharacterData)m_data;

		public LogicAttackerItemData GetAttackerItemData()
			=> GetCharacterData().GetAttackerItemData(m_upgradeLevel);

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.CHARACTER;

		public override bool IsHero()
			=> GetCharacterData().GetDataType() == LogicDataType.HERO;

		public override bool IsFlying()
			=> m_flying;

		public int GetSecondaryTroopTeam()
			=> m_secondaryTroopTeam;

		public void SetSecondaryTroopTeam(int value)
		{
			m_secondaryTroopTeam = value;
		}

		public bool GetWaitDieDamage()
			=> GetCharacterData().GetDieDamageDelay() > m_dieTime;

		public int GetSpawnDelay()
			=> m_spawnTime;

		public void SetSpawnTime(int time)
		{
			m_spawnTime = time;

			if (time > 0)
			{
				m_hasSpawnDelay = true;
			}
		}

		public int GetSpawnIdleTime()
			=> m_spawnIdleTime;

		public int GetSummonTroopCount()
			=> m_summonTroops.Size();

		public LogicArrayList<LogicCharacter> GetChildTroops()
			=> m_childrens;

		public override void SetInitialPosition(int x, int y)
		{
			base.SetInitialPosition(x, y);

			LogicMovementComponent movementComponent = GetMovementComponent();

			if (movementComponent != null)
			{
				movementComponent.GetMovementSystem().Reset(x, y);
			}

			if (m_childrens != null)
			{
				for (int i = 0; i < m_childrens.Size(); i++)
				{
					m_childrens[i].SetInitialPosition(x, y);
				}
			}
		}

		public override bool ShouldDestruct()
		{
			if (m_level.IsInCombatState())
			{
				int fadingOutTime = 5000;

				if (m_duplicate)
				{
					fadingOutTime += (m_duplicateLifeTime >> 31) & -4999;
				}

				return m_dieTime > fadingOutTime;
			}

			return true;
		}

		public bool HasSpecialAbility()
			=> GetCharacterData().GetSpecialAbilityLevel(m_upgradeLevel) > 0;

		public int GetUpgradeLevel()
			=> m_upgradeLevel;

		public void SetUpgradeLevel(int upgLevel)
		{
			m_upgradeLevel = upgLevel;

			LogicCharacterData data = GetCharacterData();
			LogicHitpointComponent hitpointComponent = GetHitpointComponent();
			LogicCombatComponent combatComponent = GetCombatComponent();

			int hp = data.GetHitpoints(upgLevel);
			int damagePercentage = 100;

			if (data.GetScaleByTH())
			{
				LogicAvatar avatar = m_level.GetHomeOwnerAvatar();

				if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
				{
					avatar = m_level.GetVisitorAvatar();
				}

				int tmp1 = 700 * avatar.GetTownHallLevel() / (LogicDataTables.GetTownHallLevelCount() - 1);

				damagePercentage = tmp1 / 10 + 30;
				hp = damagePercentage * hp / 100;

				if (damagePercentage * hp < 200)
				{
					hp = 1;
				}

				if (tmp1 < -289)
				{
					damagePercentage = 1;
				}
			}

			hitpointComponent.SetMaxHitpoints(hp);
			hitpointComponent.SetHitpoints(data.GetHitpoints(upgLevel));
			hitpointComponent.SetDieEffect(data.GetDieEffect(upgLevel), data.GetDieEffect2(upgLevel));

			if (combatComponent != null)
			{
				combatComponent.SetAttackValues(data.GetAttackerItemData(upgLevel), damagePercentage);
			}

			if (m_childrens != null)
			{
				for (int i = 0; i < m_childrens.Size(); i++)
				{
					m_childrens[i].SetUpgradeLevel(upgLevel);
				}
			}

			if (IsHero())
			{
				LogicHeroData heroData = (LogicHeroData)m_data;
				LogicAvatar avatar = m_level.GetHomeOwnerAvatar();

				if (hitpointComponent.GetTeam() == 0)
				{
					avatar = m_level.GetVisitorAvatar();
				}

				m_flying = heroData.IsFlying(avatar.GetHeroMode(heroData));
				GetMovementComponent().SetFlying(m_flying);
			}

			if (data.GetAutoMergeDistance() > 0)
			{
				m_autoMergeTime = 2000;
			}

			int speed = data.GetSpeed();

			if (data.GetSpecialAbilityLevel(m_upgradeLevel) > 0 &&
				data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPEED_BOOST)
			{
				speed = speed * data.GetSpecialAbilityAttribute(m_upgradeLevel) / 100;
			}

			GetMovementComponent().SetSpeed(speed);
		}

		public LogicCharacter GetParent()
			=> m_parent;

		public void SetTroopChild(LogicCharacter character, int idx)
		{
			m_parent = character;

			if (character != null)
			{
				m_troopChildX = (character.GetCharacterData().GetChildTroopX(idx) << 9) / 100;
				m_troopChildY = (character.GetCharacterData().GetChildTroopY(idx) << 9) / 100;
				m_troopChild = true;
			}
		}

		public void SetDuplicate(bool clone, int lifetime)
		{
			m_duplicate = clone;
			m_duplicateLifeTime = lifetime;
		}

		public bool TileOkForTombstone(LogicTile tile)
		{
			if (tile != null && !tile.IsFullyNotPassable())
			{
				for (int i = 0; i < tile.GetGameObjectCount(); i++)
				{
					LogicGameObject gameObject = tile.GetGameObject(i);

					if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING ||
						gameObject.GetGameObjectType() == LogicGameObjectType.OBSTACLE ||
						gameObject.GetGameObjectType() == LogicGameObjectType.TRAP ||
						gameObject.GetGameObjectType() == LogicGameObjectType.DECO)
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public void Eject(LogicVector2 pos)
		{
			if (!IsFlying())
			{
				if (pos != null)
				{
					m_ejectPosition.Set(pos.m_x, pos.m_y);
				}
				else
				{
					m_ejectPosition.Set(0, 0);
				}

				m_ejected = true;
				GetHitpointComponent().Kill();
			}
		}

		public void SetAllianceUnit()
		{
			m_allianceUnit = true;
		}

		public int GetAbilityCooldown()
			=> m_abilityCooldown;

		public int GetAbilityTime()
			=> m_abilityTime;

		public bool IsAbilityUsed()
			=> m_abilityUsed;

		public void StartAbility()
		{
			if (IsHero())
			{
				LogicHeroData heroData = (LogicHeroData)m_data;

				m_abilityCooldown = 60 * heroData.GetAbilityCooldown() / 4000;
				m_abilityTriggerTime = 5;
				m_abilityTime = 60 * heroData.GetAbilityTime(m_upgradeLevel) / 20000;
				m_summonSpawnCount = 0;

				GetHitpointComponent().CauseDamage(-100 * heroData.GetAbilityHealthIncrease(m_upgradeLevel), 0, this);

				m_abilityAttackCount = GetCombatComponent().GetHitCount() + heroData.GetAbilityAttackCount(m_upgradeLevel);

				if (heroData.GetAbilityDelay() > 0)
				{
					GetCombatComponent().SetAttackDelay(0, heroData.GetAbilityDelay());
					// Listener.
				}

				LogicSpellData abilitySpellData = heroData.GetAbilitySpell(m_upgradeLevel);

				if (abilitySpellData != null)
				{
					m_abilitySpell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(abilitySpellData, m_level, m_villageType);
					m_abilitySpell.SetUpgradeLevel(heroData.GetAbilitySpellLevel(m_upgradeLevel));
					m_abilitySpell.SetInitialPosition(GetX(), GetY());
					m_abilitySpell.AllowDestruction(false);

					GetGameObjectManager().AddGameObject(m_abilitySpell, -1);
				}

				if (heroData.GetActivationTime() > 0)
				{
					m_activationTimeState = 1;
					m_activationTime = heroData.GetActivationTime();

					GetMovementComponent().GetMovementSystem().SetFreezeTime(m_activationTime);
					GetCombatComponent().SetActivationTime(m_activationTime);

					// Listener.
				}

				m_abilityUsed = true;
			}
		}

		public bool GetSpecialAbilityAvailable()
		{
			LogicCharacterData data = GetCharacterData();

			if (data.GetSpecialAbilityLevel(m_upgradeLevel) <= 0)
			{
				return false;
			}

			switch (data.GetSpecialAbilityType())
			{
				case LogicCharacterData.SPECIAL_ABILITY_TYPE_BIG_FIRST_HIT:
					return GetCombatComponent().GetHitCount() <= 0;
				case LogicCharacterData.SPECIAL_ABILITY_TYPE_SPECIAL_PROJECTILE:
					return GetCombatComponent().GetHitCount() < data.GetSpecialAbilityAttribute(m_upgradeLevel);
			}

			return true;
		}

		public bool IsWallBreaker()
		{
			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent != null)
			{
				return combatComponent.IsWallBreaker();
			}

			return false;
		}

		public int GetEjectTime()
		{
			if (m_ejected)
			{
				return m_dieTime + 1;
			}

			return 0;
		}

		public override int GetHitEffectOffset()
			=> GetCharacterData().GetHitEffectOffset();

		public override void DeathEvent()
		{
			LogicHitpointComponent hitpointComponent = GetHitpointComponent();
			LogicCharacterData data = GetCharacterData();

			if (hitpointComponent != null && hitpointComponent.GetTeam() == 1 && !IsHero() && !data.IsSecondaryTroop() &&
				m_level.GetVillageType() == 0 && m_allianceUnit)
			{
				LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

				homeOwnerAvatar.RemoveAllianceUnit(data, m_upgradeLevel);
				homeOwnerAvatar.GetChangeListener().AllianceUnitRemoved(data, m_upgradeLevel);
			}

			if (data.GetSpecialAbilityType() != LogicCharacterData.SPECIAL_ABILITY_TYPE_RESPAWN_AS_CANNON ||
				data.GetSpecialAbilityLevel(m_upgradeLevel) <= 0)
			{
				if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_SPAWN_UNITS)
				{
					if (data.GetSpecialAbilityLevel(m_upgradeLevel) > 0)
					{
						CheckSpawning(null, data.GetSpecialAbilityAttribute(m_upgradeLevel), 0, 0);
					}
				}
				else if (data.GetSecondaryTroop() != null)
				{
					CheckSpawning(null, 0, 0, 0);
				}
			}
			else if (!m_ejected)
			{
				CheckSpawning(LogicDataTables.GetCharacterByName("MovingCannonSecondary", null), 1, data.GetSpecialAbilityAttribute(m_upgradeLevel), 500);
			}

			AddTombstoneIfNeeded();

			if (m_parent != null)
			{
				m_parent.RemoveChildren(this);
				m_parent = null;
			}

			base.DeathEvent();
		}

		public void RemoveChildren(LogicCharacter children)
		{
			if (m_childrens.Size() > 0)
			{
				int idx = m_childrens.IndexOf(children);

				if (idx != -1)
				{
					m_childrens.Remove(idx);

					if (m_childrens.Size() != 0)
					{
						int childTroopLost = GetCharacterData().GetChildTroopCount() - m_childrens.Size();
						GetMovementComponent().GetMovementSystem()
							.SetSpeed(GetCharacterData().GetSpeed() - GetCharacterData().GetSpeedDecreasePerChildTroopLost() * childTroopLost);
					}
					else
					{
						GetHitpointComponent().Kill();
					}
				}
			}
		}

		public void AddTombstoneIfNeeded()
		{
			if (!m_ejected && m_level.GetTombStoneCount() < 40)
			{
				int tileX = GetTileX();
				int tileY = GetTileY();

				LogicTileMap tileMap = m_level.GetTileMap();
				LogicTile tile = tileMap.GetTile(tileX, tileY);

				if (!TileOkForTombstone(tile))
				{
					int minDistance = 0;
					int closestTileX = -1;
					int closestTileY = -1;

					for (int i = -1; i < 2; i++)
					{
						int offsetX = ((i + tileX) << 9) | 256;
						int offsetY = 256 - (tileY << 9);

						for (int j = -1; j < 2; j++, offsetY -= 512)
						{
							tile = tileMap.GetTile(tileX + i, tileY + j);

							if (TileOkForTombstone(tile))
							{
								int distanceX = GetX() - offsetX;
								int distanceY = GetY() + offsetY;
								int distance = distanceX * distanceX + distanceY * distanceY;

								if (minDistance == 0 || distance < minDistance)
								{
									minDistance = distance;
									closestTileX = tileX + i;
									closestTileY = tileY + j;
								}
							}
						}
					}

					if (minDistance == 0)
					{
						return;
					}

					tileX = closestTileX;
					tileY = closestTileY;
				}

				LogicObstacleData tombstoneData = GetCharacterData().GetTombstone();

				if (tombstoneData != null)
				{
					LogicObstacle tombstone = (LogicObstacle)LogicGameObjectFactory.CreateGameObject(tombstoneData, m_level, m_villageType);
					tombstone.SetInitialPosition(tileX << 9, tileY << 9);
					GetGameObjectManager().AddGameObject(tombstone, -1);
				}
			}
		}

		public void CheckSpawning(LogicCharacterData spawnCharacterData, int spawnCount, int spawnUpgradeLevel, int invulnerabilityTime)
		{
			LogicCharacterData data = GetCharacterData();

			if (spawnCharacterData == null)
			{
				spawnCharacterData = data.GetSecondaryTroop();

				if (spawnCharacterData == null)
				{
					spawnCharacterData = data.GetAttackerItemData(m_upgradeLevel).GetSummonTroop();

					if (spawnCharacterData == null)
					{
						return;
					}
				}
			}

			if (spawnCharacterData.IsSecondaryTroop() || IsHero())
			{
				int totalSpawnCount = spawnCount;
				int upgLevel = m_upgradeLevel;

				if (upgLevel >= spawnCharacterData.GetUpgradeLevelCount())
				{
					upgLevel = spawnCharacterData.GetUpgradeLevelCount() - 1;
				}

				if (IsHero())
				{
					if (m_summonSpawnCount >= spawnCount)
					{
						return;
					}

					upgLevel = spawnUpgradeLevel;
					totalSpawnCount = LogicMath.Max(0, LogicMath.Min(3, spawnCount - m_summonSpawnCount));
				}
				else
				{
					if (data.GetSecondaryTroopCount(m_upgradeLevel) != 0)
					{
						totalSpawnCount = data.GetSecondaryTroopCount(m_upgradeLevel);
					}
					else if (spawnCount == 0)
					{
						totalSpawnCount = data.GetAttackerItemData(m_upgradeLevel).GetSummonTroopCount();

						if (m_summonTroops.Size() + totalSpawnCount > data.GetAttackerItemData(m_upgradeLevel).GetSummonLimit())
						{
							totalSpawnCount = data.GetAttackerItemData(m_upgradeLevel).GetSummonLimit() - m_summonTroops.Size();
						}
					}
				}

				if (totalSpawnCount > 0)
				{
					LogicVector2 position = new LogicVector2();
					LogicRandom random = new LogicRandom(m_globalId);

					int team = GetHitpointComponent().GetTeam();
					bool randomizeSecSpawnDist = GetCharacterData().GetRandomizeSecSpawnDist();

					for (int i = 0, j = 0, k = 0; i < totalSpawnCount; i++, j += 360, k += 100)
					{
						int seed = j / totalSpawnCount;

						if (IsHero())
						{
							seed = 360 * (i + m_summonSpawnCount) / LogicMath.Max(1, LogicMath.Min(6, spawnCount));
						}

						int rnd = 59 * m_globalId % 360 + seed;

						if (spawnCharacterData.IsFlying())
						{
							LogicCharacterData parentData = GetCharacterData();

							position.Set(GetX() + LogicMath.GetRotatedX(parentData.GetSecondarySpawnOffset(), 0, rnd),
										 GetY() + LogicMath.GetRotatedY(parentData.GetSecondarySpawnOffset(), 0, rnd));
						}
						else if (spawnCharacterData.GetSpeed() == 0)
						{
							position.Set(GetX(), GetY());
						}
						else
						{
							if (!m_level.GetTileMap().GetNearestPassablePosition(GetX(), GetY(), position, 1536))
							{
								continue;
							}
						}

						LogicCharacter spawnGameObject = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(spawnCharacterData, m_level, m_villageType);

						if (GetCharacterData().GetAttackerItemData(m_upgradeLevel).GetSummonTroop() != null || IsHero())
						{
							m_summonTroops.Add(spawnGameObject);
						}

						spawnGameObject.GetHitpointComponent().SetTeam(team);
						spawnGameObject.SetUpgradeLevel(upgLevel);

						spawnGameObject.SetInitialPosition(position.m_x, position.m_y);

						if (m_duplicate)
						{
							spawnGameObject.m_duplicateLifeTime = m_duplicateLifeTime;
							spawnGameObject.m_duplicate = true;
						}

						if (!IsHero())
						{
							spawnGameObject.m_summoner = (LogicCharacterData)m_data;
						}

						if (invulnerabilityTime > 0)
						{
							spawnGameObject.GetHitpointComponent().SetInvulnerabilityTime(invulnerabilityTime);
						}

						int secondarySpawnDistance = IsHero() ? 768 : GetCharacterData().GetSecondarySpawnDistance();

						if (secondarySpawnDistance > 0)
						{
							if (randomizeSecSpawnDist)
							{
								secondarySpawnDistance = (int)(random.Rand(secondarySpawnDistance) + ((uint)secondarySpawnDistance >> 1));
							}

							position.Set(LogicMath.Cos(rnd, secondarySpawnDistance),
										 LogicMath.Sin(rnd, secondarySpawnDistance));

							int pushBackSpeed = spawnGameObject.GetCharacterData().GetPushbackSpeed();

							if (pushBackSpeed <= 0)
							{
								pushBackSpeed = 1;
							}

							int pushBackTime = 2 * secondarySpawnDistance / (3 * pushBackSpeed);

							if (GetHitpointComponent().GetHitpoints() > 0)
							{
								if (GetAttackerItemData().GetSummonTroop() != null)
								{
									spawnGameObject.SetSpawnTime(pushBackTime);
								}
								else if (IsHero())
								{
									spawnGameObject.SetSpawnTime(pushBackTime + k);
								}
							}

							spawnGameObject.GetMovementComponent().GetMovementSystem().PushTrap(position, pushBackTime, 0, false, false);
						}

						if (team == 1 || spawnGameObject.GetCharacterData().IsJumper())
						{
							spawnGameObject.GetMovementComponent().EnableJump(3600000);
							spawnGameObject.GetCombatComponent().RefreshTarget(true);
						}

						if (team == 1)
						{
							if (LogicDataTables.GetGlobals().AllianceTroopsPatrol())
							{
								spawnGameObject.GetCombatComponent().SetSearchRadius(LogicDataTables.GetGlobals().GetClanCastleRadius() >> 9);

								if (GetMovementComponent().GetBaseBuilding() != null)
								{
									spawnGameObject.GetMovementComponent().SetBaseBuilding(GetMovementComponent().GetBaseBuilding());
								}
							}
						}

						GetGameObjectManager().AddGameObject(spawnGameObject, -1);

						if (IsHero())
						{
							++m_summonSpawnCount;
						}
					}

					position.Destruct();
				}
			}
			else
			{
				Debugger.Warning("checkSpawning: trying to spawn normal troops!");
			}
		}

		public override void SpawnEvent(LogicCharacterData data, int count, int upgLevel)
		{
			CheckSpawning(data, count, upgLevel, 0);
		}

		public override void SubTick()
		{
			base.SubTick();

			LogicCombatComponent combatComponent = GetCombatComponent();
			LogicMovementComponent movementComponent = GetMovementComponent();

			if (combatComponent != null)
			{
				combatComponent.SubTick();
			}

			if (movementComponent != null)
			{
				movementComponent.SubTick();

				LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();
				LogicVector2 movementPosition = movementSystem.GetPosition();

				SetPositionXY(movementPosition.m_x, movementPosition.m_y);
			}
			else if (m_troopChild)
			{
				LogicVector2 tmp = new LogicVector2(m_troopChildX, m_troopChildY);

				tmp.Rotate(m_parent.GetDirection());

				LogicMovementSystem movementSystem = m_parent.GetMovementComponent().GetMovementSystem();
				LogicVector2 position = movementSystem.GetPosition();

				SetPositionXY(tmp.m_x + position.m_x, tmp.m_y + position.m_y);
			}

			if (m_childrens != null)
			{
				for (int i = 0; i < m_childrens.Size(); i++)
				{
					m_childrens[i].SubTick();
				}
			}

			int distanceX = GetX() + (GetWidthInTiles() << 8);
			int distanceY = GetY() + (GetHeightInTiles() << 8);

			if (m_auraSpell != null)
			{
				m_auraSpell.SetPositionXY(distanceX, distanceY);
			}

			if (m_abilitySpell != null)
			{
				m_abilitySpell.SetPositionXY(distanceX, distanceY);
			}

			if (m_retributionSpell != null)
			{
				m_retributionSpell.SetPositionXY(distanceX, distanceY);
			}
		}

		public override void Tick()
		{
			base.Tick();

			LogicCharacterData data = GetCharacterData();

			if (!IsAlive())
			{
				if (!IsHero())
				{
					int dieDamageDelay = GetCharacterData().GetDieDamageDelay();
					int prevDieTime = m_dieTime;

					m_dieTime += 64;

					if (dieDamageDelay >= prevDieTime && dieDamageDelay < m_dieTime && (!m_duplicate || m_duplicateLifeTime >= 0))
					{
						CheckDieDamage(data.GetDieDamage(m_upgradeLevel), data.GetDieDamageRadius());
						m_level.UpdateBattleStatus();
					}
				}

				m_spawnTime = 0;
				m_spawnIdleTime = 0;

				if (m_auraSpell != null)
				{
					GetGameObjectManager().RemoveGameObject(m_auraSpell);
					m_auraSpell = null;
				}

				if (m_abilitySpell != null)
				{
					GetGameObjectManager().RemoveGameObject(m_abilitySpell);
					m_abilitySpell = null;
				}

				if (m_retributionSpell != null)
				{
					GetGameObjectManager().RemoveGameObject(m_retributionSpell);
					m_retributionSpell = null;
				}
			}
			else
			{
				if (data.GetLoseHpPerTick() > 0)
				{
					m_loseHpTime += 64;

					if (m_loseHpTime > data.GetLoseHpInterval())
					{
						LogicHitpointComponent hitpointComponent = GetHitpointComponent();

						if (hitpointComponent != null)
						{
							hitpointComponent.CauseDamage(100 * data.GetLoseHpPerTick(), m_globalId, this);
							// Listener.
						}

						m_loseHpTime = 0;
					}
				}

				if (data.GetAttackCount(m_upgradeLevel) > 0 && GetCombatComponent() != null && GetHitpointComponent() != null &&
					GetCombatComponent().GetHitCount() >= data.GetAttackCount(m_upgradeLevel))
				{
					GetHitpointComponent().Kill();
				}

				m_spawnTime = LogicMath.Max(m_spawnTime - 64, 0);
				m_spawnIdleTime = LogicMath.Max(m_spawnIdleTime - 64, 0);

				if (m_spawnTime == 0 && m_hasSpawnDelay)
				{
					m_spawnIdleTime = LogicMath.Max(10, data.GetSpawnIdle());
					m_hasSpawnDelay = false;
				}

				if (data.GetBoostedIfAlone() || data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_RAGE_ALONE && GetSpecialAbilityAvailable())
				{
					if (++m_rageAloneTime >= 5)
					{
						m_level.AreaBoostAlone(this, 6);
						m_rageAloneTime = 0;
					}
				}

				if (IsHero())
				{
					LogicHeroData heroData = (LogicHeroData)data;

					if (m_abilityTime > 0)
					{
						if (heroData.GetAbilityAttackCount(m_upgradeLevel) > 0 && GetCombatComponent().GetHitCount() >= m_abilityAttackCount)
						{
							Debugger.HudPrint("Hero ability: No more attacks left!");

							m_abilityTime = 0;
							m_abilityTriggerTime = 0;
							m_activationTime = 0;
						}
						else
						{
							if (++m_abilityTriggerTime >= 5)
							{
								m_abilityTime -= 1;
								m_abilityTriggerTime = 0;

								m_level.AreaAbilityBoost(this, 5);
							}
						}
					}

					if (m_abilityCooldown > 0)
					{
						m_abilityCooldown -= 1;
					}

					if (m_abilitySpell != null && m_abilitySpell.GetHitsCompleted())
					{
						GetGameObjectManager().RemoveGameObject(m_abilitySpell);
						m_abilitySpell = null;
					}
				}

				if (m_auraSpell == null || m_auraSpell.GetHitsCompleted())
				{
					if (m_auraSpell != null)
					{
						GetGameObjectManager().RemoveGameObject(m_auraSpell);
						m_auraSpell = null;
					}

					if (data.GetAuraSpell(m_upgradeLevel) != null)
					{
						LogicHitpointComponent hitpointComponent = GetHitpointComponent();

						if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
						{
							m_auraSpell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(data.GetAuraSpell(m_upgradeLevel), m_level, m_villageType);
							m_auraSpell.SetUpgradeLevel(data.GetAuraSpellLevel(m_upgradeLevel));
							m_auraSpell.SetInitialPosition(GetX(), GetY());
							m_auraSpell.AllowDestruction(false);
							m_auraSpell.SetTeam(hitpointComponent.GetTeam());

							GetGameObjectManager().AddGameObject(m_auraSpell, -1);
						}
					}
				}

				if (!m_retributionSpellCreated)
				{
					if (data.GetRetributionSpell(m_upgradeLevel) != null)
					{
						LogicHitpointComponent hitpointComponent = GetHitpointComponent();

						if (hitpointComponent.GetHitpoints() <=
							hitpointComponent.GetMaxHitpoints() * data.GetRetributionSpellTriggerHealth(m_upgradeLevel) / 100)
						{
							m_retributionSpellCreated = true;
							m_retributionSpell =
								(LogicSpell)LogicGameObjectFactory.CreateGameObject(data.GetRetributionSpell(m_upgradeLevel), m_level, m_villageType);
							m_retributionSpell.SetUpgradeLevel(data.GetRetributionSpellLevel(m_upgradeLevel));
							m_retributionSpell.SetPositionXY(GetX(), GetY());
							m_retributionSpell.AllowDestruction(false);
							m_retributionSpell.SetTeam(hitpointComponent.GetTeam());

							GetGameObjectManager().AddGameObject(m_retributionSpell, -1);
						}
					}
				}

				if (m_activationTimeState == 2)
				{
					m_activationTime -= 64;

					if (m_activationTime < 0)
					{
						m_activationTimeState = 0;
						m_activationTime = 0;
					}
				}
				else if (m_activationTimeState == 1)
				{
					m_activationTime -= 64;

					if (m_activationTime < 0)
					{
						m_activationTimeState = 2;
						m_activationTime = ((LogicHeroData)m_data).GetActiveDuration();
					}
				}
			}

			CheckSummons();

			if (IsAlive())
			{
				if (data.GetAutoMergeDistance() > 0)
				{
					m_autoMergeTime = LogicMath.Max(m_autoMergeTime - 64, 0);
				}

				if (data.GetInvisibilityRadius() > 0)
				{
					m_level.AreaInvisibility(GetMidX(), GetMidY(), data.GetInvisibilityRadius(), 4, GetHitpointComponent().GetTeam());
				}

				if (data.GetHealthReductionPerSecond() > 0)
				{
					GetHitpointComponent().CauseDamage(100 * data.GetHealthReductionPerSecond() / 15, 0, this);
				}
			}

			if (m_duplicate)
			{
				if (m_duplicateLifeTime-- <= 0)
				{
					LogicHitpointComponent hitpointComponent = GetHitpointComponent();

					if (hitpointComponent != null)
					{
						hitpointComponent.SetHitpoints(0);
						m_level.UpdateBattleStatus();
					}
				}
			}
		}

		public void CheckDieDamage(int damage, int radius)
		{
			LogicCharacterData data = (LogicCharacterData)m_data;

			if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_DIE_DAMAGE)
			{
				if (data.GetSpecialAbilityLevel(m_upgradeLevel) <= 0)
				{
					return;
				}
			}

			if (damage > 0 && radius > 0)
			{
				LogicHitpointComponent hitpointComponent = GetHitpointComponent();

				if (hitpointComponent != null)
				{
					m_level.AreaDamage(0, GetX(), GetY(), radius, damage, null, 0, null, hitpointComponent.GetTeam(), null, 1, 0, 0, true, false, 100,
											0, this, 100, 0);
				}
			}
		}

		public void CheckSummons()
		{
			for (int i = 0; i < m_summonTroops.Size(); i++)
			{
				LogicCharacter character = m_summonTroops[i];

				if (!character.IsAlive())
				{
					m_summonTroops.Remove(i--);
				}
				else
				{
					if (character.m_spawnTime > 0 && !IsAlive())
					{
						m_summonTroops.Remove(i--);
						m_hasSpawnDelay = false;

						LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

						if (hitpointComponent != null)
						{
							hitpointComponent.SetHitpoints(0);
						}

						m_level.UpdateBattleStatus();
					}
				}
			}
		}

		public void UpdateAutoMerge()
		{
			if (m_autoMergeTime > 0)
			{
				int autoMergeGroupSize = GetCharacterData().GetAutoMergeGroupSize();
				int autoMergeDistance = GetCharacterData().GetAutoMergeDistance();

				if (autoMergeGroupSize > 0)
				{
					LogicArrayList<LogicGameObject> characters = GetGameObjectManager().GetGameObjects(LogicGameObjectType.CHARACTER);
					LogicCharacter closestCharacter = null;

					for (int i = 0; i < characters.Size(); i++)
					{
						LogicCharacter character = (LogicCharacter)characters[i];

						if (character != this)
						{
							if (character.GetData() == GetData())
							{
								if (m_autoMergeSize == 0 && character.m_autoMergeSize >= autoMergeGroupSize)
								{
									if (character.GetHitpointComponent().GetTeam() == GetHitpointComponent().GetTeam() && character.IsAlive())
									{
										if (character.m_autoMergeTime > 0)
										{
											int distanceSquared = GetPosition().GetDistanceSquared(character.GetPosition());

											if (distanceSquared <= autoMergeDistance * autoMergeDistance)
											{
												closestCharacter = character;
											}
										}
									}
								}
							}
						}
					}

					if (closestCharacter != null)
					{
						closestCharacter.m_autoMergeSize += 1;
						closestCharacter.GetCombatComponent().SetMergeDamage(90 * closestCharacter.m_autoMergeSize);
						closestCharacter.GetHitpointComponent()
										.SetMaxHitpoints(closestCharacter.GetCharacterData().GetHitpoints(m_upgradeLevel) * (closestCharacter.m_autoMergeSize + 1));
						closestCharacter.GetHitpointComponent()
										.SetHitpoints(closestCharacter.GetCharacterData().GetHitpoints(m_upgradeLevel) * (closestCharacter.m_autoMergeSize + 1));

						GetGameObjectManager().RemoveGameObject(this);
					}
				}
			}
		}

		public override bool IsStaticObject()
			=> false;

		public override int GetWidthInTiles()
			=> 1;

		public override int GetHeightInTiles()
			=> 1;

		public override int GetMidX()
			=> GetX();

		public override int GetMidY()
			=> GetY();

		public override int GetDirection()
		{
			LogicMovementComponent movementComponent = GetMovementComponent();

			if (movementComponent != null)
			{
				return movementComponent.GetMovementSystem().GetDirection();
			}

			if (m_troopChild && m_parent != null)
			{
				return m_parent.GetDirection();
			}

			return 0;
		}
	}
}