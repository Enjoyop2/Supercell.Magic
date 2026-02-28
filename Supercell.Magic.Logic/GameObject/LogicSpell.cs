using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicSpell : LogicGameObject
	{
		private int m_upgradeLevel;
		private int m_deployEffect2Cooldown;
		private readonly int m_playEffectOffsetX;
		private readonly int m_playEffectOffsetY;
		private int m_deployTime;
		private int m_chargingTime;
		private int m_hitTime;
		private int m_chargingCount;
		private int m_hitCount;
		private int m_team;
		private int m_duplicateHousingSpace; // 204
		private int m_duplicateCharacterOffset; // 196
		private int m_duplicateCharacterPositionOffset; // 200
		private int m_duplicateCharacterUpgradeLevel; // 192

		private readonly LogicVector2 m_randomOffset;
		private LogicCharacterData m_duplicateCharacterData; // 188
		private LogicArrayList<LogicGameObject> m_duplicateCharacters; // 184
		private LogicArrayList<LogicGameObject> m_duplicableCharacters; // 180

		private bool m_hitsCompleted;
		private bool m_deployed;
		private bool m_preDeployEffectPlayed;
		private bool m_allowDestruction;

		public LogicSpell(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			m_allowDestruction = true;
			m_randomOffset = new LogicVector2();
			m_duplicateHousingSpace = -1;
			m_playEffectOffsetX = 100;
			m_playEffectOffsetY = 100;
		}

		public override void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			if (m_duplicableCharacters != null)
			{
				int idx = m_duplicableCharacters.IndexOf(gameObject);

				if (idx != -1)
				{
					m_duplicableCharacters.Remove(idx);
				}
			}
		}

		public LogicSpellData GetSpellData()
			=> (LogicSpellData)m_data;

		public void AllowDestruction(bool value)
		{
			m_allowDestruction = value;
		}

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.SPELL;

		public int GetUpgradeLevel()
			=> m_upgradeLevel;

		public void SetUpgradeLevel(int upgLevel)
		{
			m_upgradeLevel = upgLevel;
		}

		public void CalculateRandomOffset(int randCount)
		{
			m_randomOffset.m_x = 0;
			m_randomOffset.m_y = 0;

			int randomRadius = GetSpellData().GetRandomRadius(m_upgradeLevel);

			if (randomRadius > 0)
			{
				for (int i = 0; i < randCount; i++)
				{
					int prevX = m_randomOffset.m_x;
					int prevY = m_randomOffset.m_y;

					int yPosSeed = 7 * randCount + 9;
					int xEnableSeed = 5 * randCount + 3;
					int yEnableSeed = 11 * randCount + 32;

					for (int j = 0; j < 100; j++)
					{
						m_randomOffset.m_x = (Rand(randCount + j) % randomRadius) * (2 * (Rand(xEnableSeed) & 1) - 1);
						m_randomOffset.m_y = (Rand(yPosSeed) % randomRadius) * (2 * (Rand(yEnableSeed) & 1) - 1);

						if (LogicMath.Abs(prevY - prevX + m_randomOffset.m_x - m_randomOffset.m_y) > (int)(randomRadius / 3u))
						{
							break;
						}

						yPosSeed += 7;
						xEnableSeed += 4;
						yEnableSeed += 15;
					}
				}
			}
		}

		public void SpawnObstacle(int x, int y, int radius)
		{
			int tileX = x >> 9;
			int tileY = y >> 9;

			if (!TileOkForSpawn(m_level.GetTileMap().GetTile(tileX, tileY)))
			{
				int minSquare = 0;
				int minTileX = -1;
				int minTileY = -1;

				for (int i = -radius; i <= radius; i++)
				{
					int posX = tileX + i;
					int midX = (posX << 9) | 256;
					int midY = (radius << 9) - 256 - ((y >> 9) << 9);

					for (int j = -radius; j <= radius; j++)
					{
						int posY = tileY + j;

						if (TileOkForSpawn(m_level.GetTileMap().GetTile(posX, posY)))
						{
							int goX = GetX();
							int goY = GetY();

							int square = (goX - midX) * (goX - midX) + (goY + midY) * (goY + midY);

							if (minSquare == 0 || square < minSquare)
							{
								minSquare = square;
								minTileX = posX;
								minTileY = posY;
							}
						}

						midY -= 512;
					}
				}

				if (minSquare == 0)
				{
					return;
				}

				tileX = minTileX;
				tileY = minTileY;
			}

			LogicObstacleData data = GetSpellData().GetSpawnObstacle();

			if (data != null)
			{
				LogicGameObject gameObject = LogicGameObjectFactory.CreateGameObject(data, m_level, GetVillageType());
				gameObject.SetInitialPosition(tileX << 9, tileY << 9);
				GetGameObjectManager().AddGameObject(gameObject, -1);
			}
		}

		public bool TileOkForSpawn(LogicTile tile)
		{
			if (tile != null && !tile.IsFullyNotPassable())
			{
				for (int i = 0; i < tile.GetGameObjectCount(); i++)
				{
					LogicGameObjectType gameObjectType = tile.GetGameObject(i).GetGameObjectType();

					if (gameObjectType == LogicGameObjectType.BUILDING || gameObjectType == LogicGameObjectType.OBSTACLE ||
						gameObjectType == LogicGameObjectType.TRAP || gameObjectType == LogicGameObjectType.DECO)
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public bool GetHitsCompleted()
			=> m_hitsCompleted;

		public bool IsDeployed()
			=> m_deployed;

		public void SetTeam(int team)
		{
			m_team = team;
		}

		public void SelectDuplicableCharacters()
		{
			if (m_duplicableCharacters == null)
			{
				m_duplicableCharacters = new LogicArrayList<LogicGameObject>(20);
			}

			if (m_duplicateCharacters == null)
			{
				m_duplicateCharacters = new LogicArrayList<LogicGameObject>(20);
			}

			int radius = GetSpellData().GetRadius(m_upgradeLevel);

			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();

				if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)parent;
					LogicCharacterData characterData = character.GetCharacterData();
					LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

					if (hitpointComponent != null && hitpointComponent.GetTeam() == 0 && character.IsAlive() && !character.IsHero() &&
						characterData.GetHousingSpace() <= m_duplicateHousingSpace)
					{
						int distanceX = character.GetPosition().m_x - GetMidX();
						int distanceY = character.GetPosition().m_y - GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius &&
							distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
						{
							int idx = -1;

							for (int j = 0, size = m_duplicableCharacters.Size(); j < size; j++)
							{
								if (m_duplicableCharacters[j] == character)
								{
									idx = j;
									break;
								}
							}

							if (idx == -1)
							{
								m_duplicateCharacterData = characterData;
								m_duplicateCharacterUpgradeLevel = character.GetUpgradeLevel();

								m_duplicableCharacters.Add(character);

								// Listener.
							}
						}
					}
				}
			}
		}

		public LogicCharacter CreateDuplicateCharacter(LogicCharacterData data, int upgLevel, int x, int y)
		{
			LogicCharacter character = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(data, m_level, m_villageType);

			character.SetUpgradeLevel(upgLevel);
			character.SetDuplicate(true, GetSpellData().GetDuplicateLifetime(m_upgradeLevel) / 64 + 1);
			character.SetInitialPosition(x, y);

			if (data.IsJumper())
			{
				character.GetMovementComponent().EnableJump(3600000);
				character.GetCombatComponent().RefreshTarget(true);
			}

			if (data.IsUnderground())
			{
				LogicCombatComponent combatComponent = character.GetCombatComponent();

				combatComponent.SetUndergroundTime(3600000);
				combatComponent.RefreshTarget(true);
			}

			if (LogicDataTables.IsSkeleton(data))
			{
				LogicCombatComponent combatComponent = character.GetCombatComponent();

				if (combatComponent != null)
				{
					combatComponent.SetSkeletonSpell();
				}
			}

			GetGameObjectManager().AddGameObject(character, -1);
			// Listener.
			return character;
		}

		public bool DuplicateCharacter()
		{
			if (m_duplicateHousingSpace < 0)
			{
				m_duplicateHousingSpace = GetSpellData().GetDuplicateHousing(m_upgradeLevel);
			}

			if (m_duplicableCharacters != null)
			{
				if (m_duplicableCharacters.Size() > 0)
				{
					int minHousingSpace = ((LogicCharacter)m_duplicableCharacters[0]).GetCharacterData().GetHousingSpace();

					for (int i = 0; i < m_duplicableCharacters.Size(); i++)
					{
						LogicCharacter character = (LogicCharacter)m_duplicableCharacters[(i + m_duplicateCharacterOffset) % m_duplicableCharacters.Size()];
						LogicCharacterData data = character.GetCharacterData();

						int housingSpace = data.GetHousingSpace();

						if (minHousingSpace > housingSpace)
						{
							minHousingSpace = housingSpace;
						}

						if (DuplicateCharacter(data, character.GetUpgradeLevel()))
						{
							return true;
						}
					}

					return false;
				}

				return DuplicateCharacter(m_duplicateCharacterData, m_duplicateCharacterUpgradeLevel);
			}

			return m_duplicateCharacterData != null && DuplicateCharacter(m_duplicateCharacterData, m_duplicateCharacterUpgradeLevel);
		}

		public bool DuplicateCharacter(LogicCharacterData data, int upgLevel)
		{
			if (data != null)
			{
				int tick = m_level.GetLogicTime().GetTick();
				int offset = 75 * GetSpellData().GetRadius(m_upgradeLevel) / 100 * (tick % 100) / 100;

				int posX = GetX() + ((offset * LogicMath.Sin(tick * 21 + 7 * m_duplicateCharacterPositionOffset)) >> 10);
				int posY = GetY() + ((offset * LogicMath.Cos(tick * 21 + 7 * m_duplicateCharacterPositionOffset)) >> 10);

				bool posNotFound = false;

				if (!data.IsFlying())
				{
					posNotFound = !LogicGamePlayUtil.FindGoodDuplicatePosAround(m_level, posX, posY, out int outputX, out int outputY, 10);

					posX = outputX;
					posY = outputY;
				}

				if (!posNotFound)
				{
					if (m_duplicateHousingSpace >= data.GetHousingSpace())
					{
						m_duplicateHousingSpace -= data.GetHousingSpace();
						m_duplicateCharacters.Add(CreateDuplicateCharacter(data, upgLevel, posX, posY));

						++m_duplicateCharacterOffset;
						++m_duplicateCharacterPositionOffset;

						return true;
					}
				}
			}

			return false;
		}

		public void SpawnSummon(int x, int y)
		{
			LogicSpellData data = GetSpellData();
			LogicCharacterData summonData = data.GetSummonTroop();
			LogicVector2 position = new LogicVector2();

			int summonCount = data.GetUnitsToSpawn(m_upgradeLevel);
			int spawnDuration = data.GetSpawnDuration(m_upgradeLevel);
			int totalSpawnDuration = -(spawnDuration * data.GetSpawnFirstGroupSize());

			for (int i = 0, k = 0, angle = y + 7 * x; i < summonCount; i++, k += 7, angle += 150, totalSpawnDuration += spawnDuration)
			{
				if (!summonData.IsFlying())
				{
					if (!m_level.GetTileMap().GetNearestPassablePosition(GetX(), GetY(), position, 1536))
					{
						return;
					}
				}
				else
				{
					position.m_x = x + LogicMath.GetRotatedX(summonData.GetSecondarySpawnOffset(), 0, angle);
					position.m_y = y + LogicMath.GetRotatedY(summonData.GetSecondarySpawnOffset(), 0, angle);
				}

				LogicCharacter summon = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(summonData, m_level, m_villageType);

				summon.GetHitpointComponent().SetTeam(0);
				summon.SetInitialPosition(position.m_x, position.m_y);

				LogicRandom random = new LogicRandom(k + m_globalId);

				int rnd = ((random.Rand(150) << 9) + 38400) / 100;

				position.Set(LogicMath.Cos(angle, rnd),
							 LogicMath.Sin(angle, rnd));

				int pushBackSpeed = summonData.GetPushbackSpeed() > 0 ? summonData.GetPushbackSpeed() : 1;
				int pushBackTime = 2 * rnd / (3 * pushBackSpeed);
				int spawnDelay = pushBackTime + totalSpawnDuration / summonCount;

				if (data.GetSpawnFirstGroupSize() > 0)
				{
					spawnDelay = LogicMath.Max(200, spawnDelay);
				}

				summon.SetSpawnTime(spawnDelay);
				summon.GetMovementComponent().GetMovementSystem().PushTrap(position, pushBackTime, 0, false, false);

				if (summon.GetCharacterData().IsJumper())
				{
					summon.GetMovementComponent().EnableJump(3600000);
					summon.GetCombatComponent().RefreshTarget(true);
				}

				LogicCombatComponent combatComponent = summon.GetCombatComponent();

				if (combatComponent != null)
				{
					combatComponent.SetSkeletonSpell();
				}

				GetGameObjectManager().AddGameObject(summon, -1);
			}
		}

		public void ApplyDamagePermil(int x, int y, int unk1, int team, int unk2, int targetType, int damageType, int unk3, bool healing)
		{
			LogicSpellData spellData = GetSpellData();

			int radius = spellData.GetRadius(m_upgradeLevel);
			int troopDamagePermil = spellData.GetTroopDamagePermil(m_upgradeLevel);
			int buildingDamagePermil = spellData.GetBuildingDamagePermil(m_upgradeLevel);
			int executeHealthPermil = spellData.GetExecuteHealthPermil(m_upgradeLevel);
			int damagePermilMin = spellData.GetDamagePermilMin(m_upgradeLevel);
			int preferredTargetDamageMod = spellData.GetPreferredTargetDamageMod();
			int preferredDamagePermilMin = spellData.GetPreferredDamagePermilMin(m_upgradeLevel);

			LogicData preferredTarget = spellData.GetPreferredTarget();

			LogicVector2 pushBackPosition = new LogicVector2();
			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.HITPOINT);

			int tmp = troopDamagePermil + 2 * buildingDamagePermil;

			for (int i = 0; i < components.Size(); i++)
			{
				LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)components[i];
				LogicGameObject parent = hitpointComponent.GetParent();

				if (!parent.IsHidden() && hitpointComponent.GetHitpoints() != 0)
				{
					if (hitpointComponent.GetTeam() == team)
					{
						if (tmp > 0 || tmp < 0 && parent.IsPreventsHealing())
						{
							continue;
						}
					}
					else if (tmp < 0)
					{
						continue;
					}

					if (damageType == 2 && parent.GetGameObjectType() != LogicGameObjectType.CHARACTER)
					{
						continue;
					}

					int parentX;
					int parentY;

					LogicMovementComponent movementComponent = parent.GetMovementComponent();

					if (movementComponent != null || parent.IsFlying())
					{
						if (parent.IsFlying())
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

						parentX = parent.GetMidX();
						parentY = parent.GetMidY();
					}
					else
					{
						int posX = parent.GetX();
						int posY = parent.GetY();

						parentX = LogicMath.Clamp(x, posX, posX + (parent.GetWidthInTiles() << 9));
						parentY = LogicMath.Clamp(y, posY, posY + (parent.GetHeightInTiles() << 9));
					}

					int distanceX = x - parentX;
					int distanceY = y - parentY;

					if (LogicMath.Abs(distanceX) <= radius &&
						LogicMath.Abs(distanceY) <= radius &&
						distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
					{
						if (damageType == 1 && parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
						{
							LogicBuilding building = (LogicBuilding)parent;

							if (building.GetResourceStorageComponentComponent() != null &&
								!building.GetBuildingData().IsTownHall() &&
								!building.GetBuildingData().IsTownHallVillage2())
							{
								parent.SetDamageTime(10);
								continue;
							}
						}

						if (parent.GetGameObjectType() == LogicGameObjectType.BUILDING || parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							int dataDamagePermil = parent.GetGameObjectType() == LogicGameObjectType.BUILDING ? buildingDamagePermil : troopDamagePermil;

							if (dataDamagePermil != 0)
							{
								int permil = 10 * hitpointComponent.GetMaxHitpoints() * dataDamagePermil / 10000;

								if (10 * hitpointComponent.GetMaxHitpoints() * dataDamagePermil <= -10000)
								{
									if (parent.IsHero())
									{
										permil = LogicDataTables.GetGlobals().GetHeroHealMultiplier() * permil / 100;
									}
								}

								bool isPreferredTarget = LogicCombatComponent.IsPreferredTarget(preferredTarget, parent);

								int numberOfHits = spellData.GetNumberOfHits(m_upgradeLevel);
								int completePermil = hitpointComponent.GetDamagePermilCount() / spellData.GetNumberOfHits(m_upgradeLevel);
								int calculateDamage = isPreferredTarget
									? permil / (completePermil + 1) + preferredTargetDamageMod * hitpointComponent.GetMaxHitpoints() / (100 * numberOfHits) * completePermil *
									  completePermil
									: permil / (2 * completePermil + 1);
								int permilMin = isPreferredTarget ? preferredDamagePermilMin : damagePermilMin;
								int damage = hitpointComponent.GetMaxHitpoints() * permilMin / 10000;

								if (calculateDamage >= damage)
								{
									damage = calculateDamage;
								}

								if (executeHealthPermil > 0 && 1000 * (hitpointComponent.GetHitpoints() - damage) <= executeHealthPermil)
								{
									damage = hitpointComponent.GetHitpoints();
								}

								hitpointComponent.CauseDamagePermil(damage);

								if (healing)
								{
									// Listener.
								}

								if ((distanceX | distanceX) == 0)
								{
									distanceX = 1;
								}

								pushBackPosition.m_x = -distanceX;
								pushBackPosition.m_y = -distanceY;

								pushBackPosition.Normalize(512);

								if (unk3 > 0 && movementComponent != null)
								{
									movementComponent.GetMovementSystem().PushBack(pushBackPosition, damage, unk3, 0, false, true);
								}
							}
						}
					}
				}
			}
		}

		public void ApplyExtraHealthPermil(int x, int y, int radius, int team, int extraHealthPermil, int extraHealthMin, int extraHealthMax, int time, int targetType)
		{
			LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.HITPOINT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)components[i];
				LogicGameObject parent = hitpointComponent.GetParent();

				if (!parent.IsHidden() && hitpointComponent.GetHitpoints() != 0 && hitpointComponent.GetTeam() == team)
				{
					LogicMovementComponent movementComponent = parent.GetMovementComponent();

					if (movementComponent != null)
					{
						if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)parent;

							if (character.GetCharacterData().GetAuraSpell(character.GetUpgradeLevel()) == m_data)
							{
								continue;
							}
						}

						if (parent.IsFlying())
						{
							if (targetType != 1)
							{
								continue;
							}
						}
						else if (targetType == 0)
						{
							continue;
						}

						int distanceX = x - parent.GetMidX();
						int distanceY = y - parent.GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius &&
							distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
						{
							int hp = hitpointComponent.GetOriginalHitpoints() +
									 LogicMath.Clamp(extraHealthPermil * hitpointComponent.GetOriginalHitpoints() / 1000, 100 * extraHealthMin, 100 * extraHealthMax);

							if (hp >= hitpointComponent.GetMaxHitpoints())
							{
								hitpointComponent.SetExtraHealth(hp, time);
							}
						}
					}
				}
			}
		}

		public override void Tick()
		{
			base.Tick();

			if (m_deployEffect2Cooldown > 0)
			{
				m_deployEffect2Cooldown -= 64;

				if (m_deployEffect2Cooldown <= 0)
				{
					GetListener().PlayEffect(GetSpellData().GetDeployEffect2(m_upgradeLevel), m_playEffectOffsetX, m_playEffectOffsetY);
					m_deployEffect2Cooldown = 0;
				}
			}
		}

		public override void SubTick()
		{
			base.SubTick();

			LogicSpellData data = GetSpellData();

			if (!m_preDeployEffectPlayed)
			{
				GetListener().PlayEffect(data.GetPreDeployEffect(m_upgradeLevel));
				m_preDeployEffectPlayed = true;
			}

			if (++m_deployTime >= data.GetDeployTimeMS() * 60 / 1000)
			{
				if (!m_deployed)
				{
					GetListener().PlayEffect(m_team != 0 && data.GetEnemyDeployEffect(m_upgradeLevel) != null
													  ? data.GetEnemyDeployEffect(m_upgradeLevel)
													  : data.GetDeployEffect(m_upgradeLevel), m_playEffectOffsetX, m_playEffectOffsetY);

					m_deployEffect2Cooldown = data.GetDeployEffect2Delay();

					if (m_deployEffect2Cooldown <= 0)
					{
						GetListener().PlayEffect(data.GetDeployEffect2(m_upgradeLevel), m_playEffectOffsetX, m_playEffectOffsetY);
						m_deployEffect2Cooldown = 0;
					}

					m_deployed = true;
				}

				if (++m_chargingTime >= (data.GetChargingTimeMS() * 60 / 1000) +
					m_chargingCount * (data.GetTimeBetweenHitsMS(m_upgradeLevel) * 60 / 1000) &&
					m_chargingCount < data.GetNumberOfHits(m_upgradeLevel))
				{
					CalculateRandomOffset(m_chargingCount);
					GetListener().PlayTargetedEffect(data.GetChargingEffect(m_upgradeLevel), this, m_randomOffset);
					++m_chargingCount;
				}

				if (++m_hitTime >= (data.GetHitTimeMS() * 60 / 1000) +
					m_hitCount * (data.GetTimeBetweenHitsMS(m_upgradeLevel) * 60 / 1000) &&
					m_hitCount < data.GetNumberOfHits(m_upgradeLevel))
				{
					CalculateRandomOffset(m_hitCount);
					GetListener().PlayTargetedEffect(data.GetHitEffect(m_upgradeLevel), this, m_randomOffset);

					int randomRadiusX = 0;
					int randomRadiusY = 0;

					if (!data.GetRandomRadiusAffectsOnlyGfx())
					{
						randomRadiusX = m_randomOffset.m_x;
						randomRadiusY = m_randomOffset.m_y;
					}

					int damage = data.GetDamage(m_upgradeLevel);

					if (damage != 0 && data.IsScaleByTownHall())
					{
						int scaledDamage = damage * (700 * m_level.GetPlayerAvatar().GetTownHallLevel() / (LogicDataTables.GetTownHallLevelCount() - 1) / 10 + 30) / 100;

						damage = 1;

						if (scaledDamage > 0)
						{
							damage = scaledDamage;
						}
					}

					if (damage != 0 && data.GetRadius(m_upgradeLevel) > 0)
					{
						int areaDamageX = randomRadiusX + GetMidX();
						int areaDamageY = randomRadiusY + GetMidY();
						int preferredTargetDamagePercent = 100 * data.GetPreferredTargetDamageMod();

						if (data.GetTroopsOnly())
						{
							m_level.AreaDamage(0, areaDamageX, areaDamageY, data.GetRadius(m_upgradeLevel), damage, data.GetPreferredTarget(),
													preferredTargetDamagePercent, null, m_team, null, 2, 2, 0, true, damage < 0, data.GetHeroDamageMultiplier(),
													data.GetMaxUnitsHit(m_upgradeLevel), null, data.GetDamageTHPercent(), data.GetPauseCombatComponentMs());
						}
						else
						{
							m_level.AreaDamage(0, areaDamageX, areaDamageY, data.GetRadius(m_upgradeLevel), damage, data.GetPreferredTarget(),
													preferredTargetDamagePercent, null, m_team, null, 2, 1, 0, true, damage < 0, data.GetHeroDamageMultiplier(),
													data.GetMaxUnitsHit(m_upgradeLevel), null, data.GetDamageTHPercent(), data.GetPauseCombatComponentMs());
						}
					}

					if (data.GetDuplicateHousing(m_upgradeLevel) != 0 && data.GetRadius(m_upgradeLevel) > 0)
					{
						SelectDuplicableCharacters();
						DuplicateCharacter();
					}

					if ((data.GetBuildingDamagePermil(m_upgradeLevel) != 0 || data.GetTroopDamagePermil(m_upgradeLevel) != 0) && data.GetRadius(m_upgradeLevel) > 0)
					{
						ApplyDamagePermil(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), 0, m_team, 0, 2, 1, 0,
											   data.GetTroopDamagePermil(m_upgradeLevel) < 0);
					}

					if (data.GetPoisonDamage(m_upgradeLevel) != 0 && data.GetRadius(m_upgradeLevel) > 0)
					{
						int areaDamageX = randomRadiusX + GetMidX();
						int areaDamageY = randomRadiusY + GetMidY();
						int poisonDamage = data.GetPoisonDamage(m_upgradeLevel);

						if (data.GetTroopsOnly())
						{
							m_level.AreaPoison(0, areaDamageX, areaDamageY, data.GetRadius(m_upgradeLevel), data.GetPoisonDamage(m_upgradeLevel), null,
													0, null, m_team, null, data.GetPoisonAffectAir() ? 2 : 1, 2, 0, poisonDamage < 0, data.GetHeroDamageMultiplier(),
													data.GetPoisonIncreaseSlowly());
						}
						else
						{
							m_level.AreaPoison(0, areaDamageX, areaDamageY, data.GetRadius(m_upgradeLevel), data.GetPoisonDamage(m_upgradeLevel), null,
													0, null, m_team, null, data.GetPoisonAffectAir() ? 2 : 1, 1, 0, poisonDamage < 0, data.GetHeroDamageMultiplier(),
													data.GetPoisonIncreaseSlowly());
						}
					}

					if (data.GetSpeedBoost(m_upgradeLevel) != 0 || data.GetAttackSpeedBoost(m_upgradeLevel) != 0)
					{
						m_level.AreaBoost(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
											   data.GetSpeedBoost(m_upgradeLevel), data.GetSpeedBoost2(m_upgradeLevel), data.GetDamageBoostPercent(m_upgradeLevel),
											   data.GetAttackSpeedBoost(m_upgradeLevel), 60 * data.GetBoostTimeMS(m_upgradeLevel) / 1000,
											   data.GetBoostDefenders() ? m_team != 1 ? 1 : 0 : m_team, data.GetBoostLinkedToPoison());
					}

					if (data.GetJumpBoostMS(m_upgradeLevel) != 0)
					{
						if (m_team == 0)
						{
							m_level.AreaJump(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
												  data.GetJumpBoostMS(m_upgradeLevel), data.GetJumpHousingLimit(m_upgradeLevel), m_team);

							if (m_hitCount == 0)
							{
								if (LogicDataTables.GetGlobals().UseWallWeightsForJumpSpell())
								{
									int numberOfHits = data.GetNumberOfHits(m_upgradeLevel);
									int timeBetweenHitsMS = data.GetTimeBetweenHitsMS(m_upgradeLevel);
									int radius = data.GetRadius(m_upgradeLevel);
									int jumpTime = numberOfHits * timeBetweenHitsMS - LogicDataTables.GetGlobals().GetForgetTargetTime();

									LogicArrayList<LogicGameObject> buildings = GetGameObjectManager().GetGameObjects(LogicGameObjectType.BUILDING);

									for (int i = 0; i < buildings.Size(); i++)
									{
										LogicBuilding building = (LogicBuilding)buildings[i];

										if (building.IsWall() && building.IsAlive())
										{
											int distanceX = GetMidX() - building.GetMidX();
											int distanceY = GetMidY() - building.GetMidY();

											if (LogicMath.Abs(distanceX) < radius &&
												LogicMath.Abs(distanceY) < radius &&
												distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
											{
												building.SetHitWallDelay(jumpTime);
											}
										}
									}

									m_level.GetTileMap().GetPathFinder().InvalidateCache();

									LogicArrayList<LogicComponent> components = GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

									for (int i = 0; i < components.Size(); i++)
									{
										LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
										LogicGameObject parent = movementComponent.GetParent();
										LogicCombatComponent combatComponent = parent.GetCombatComponent();

										if (combatComponent != null && combatComponent.GetTarget(0) != null)
										{
											if (combatComponent.GetTarget(0).IsWall())
											{
												combatComponent.ForceNewTarget();
											}
										}
									}
								}
							}
						}
					}

					if (data.GetShrinkReduceSpeedRatio() != 0 || data.GetShrinkHitpointsRatio() != 0)
					{
						m_level.AreaShrink(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
												data.GetShrinkReduceSpeedRatio(),
												data.GetShrinkHitpointsRatio(), 1000 * LogicDataTables.GetGlobals().GetShrinkSpellDurationSeconds() / 64, m_team);
					}

					if (data.GetFreezeTimeMS(m_upgradeLevel) != 0)
					{
						m_level.AreaFreeze(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
												60 * data.GetFreezeTimeMS(m_upgradeLevel) / 1000, m_team);
					}

					if (data.GetBuildingDamageBoostPercent(m_upgradeLevel) != 0)
					{
						m_level.AreaBoost(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
											   data.GetBuildingDamageBoostPercent(m_upgradeLevel), 0, 60 * data.GetBoostTimeMS(m_upgradeLevel) / 1000);
					}

					if (data.GetSummonTroop() != null)
					{
						SpawnSummon(randomRadiusX + GetMidX(), randomRadiusY + GetMidY());
					}

					if (data.GetSpawnObstacle() != null)
					{
						SpawnObstacle(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), 5);
					}

					if (data.GetExtraHealthPermil(m_upgradeLevel) != 0)
					{
						ApplyExtraHealthPermil(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel), m_team,
													data.GetExtraHealthPermil(m_upgradeLevel), data.GetExtraHealthMin(m_upgradeLevel),
													data.GetExtraHealthMax(m_upgradeLevel),
													data.GetHitTimeMS() + 64, 2);
					}

					if (data.GetInvulnerabilityTime(m_upgradeLevel) != 0)
					{
						m_level.AreaShield(randomRadiusX + GetMidX(), randomRadiusY + GetMidY(), data.GetRadius(m_upgradeLevel),
												data.GetInvulnerabilityTime(m_upgradeLevel), m_team);
					}

					if (++m_hitCount >= data.GetNumberOfHits(m_upgradeLevel))
					{
						m_hitsCompleted = true;
						m_level.UpdateBattleStatus();
					}
				}
			}
		}

		public override bool ShouldDestruct()
		{
			if (!m_allowDestruction)
			{
				return false;
			}

			if (m_duplicateHousingSpace != 0)
			{
				if (m_level.IsInCombatState())
				{
					return m_hitCount >= GetSpellData().GetNumberOfHits(m_upgradeLevel);
				}
			}

			return true;
		}

		public override int GetMidX()
			=> GetX();

		public override int GetMidY()
			=> GetY();

		public override bool IsStaticObject()
			=> false;
	}
}