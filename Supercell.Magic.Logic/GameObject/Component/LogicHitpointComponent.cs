using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Listener;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicHitpointComponent : LogicComponent
	{
		private bool m_regenerationEnabled;

		private int m_team;
		private int m_hp;
		private int m_maxHp;
		private int m_originalHp;
		private int m_maxRegenerationTime;
		private int m_lastDamageTime;
		private int m_regenTime;
		private int m_shrinkTime;
		private int m_extraHpTime;
		private int m_poisonInitTime;
		private int m_poisonDamage;
		private int m_poisonTime;
		private int m_invulnerabilityTime; // 76
		private int m_damagePermilCount;

		private readonly int[] m_healingTime; // 80
		private readonly int[] m_healingId; // 112

		private LogicEffectData m_dieEffect;
		private LogicEffectData m_dieEffect2;

		public LogicHitpointComponent(LogicGameObject gameObject, int hp, int team) : base(gameObject)
		{
			m_team = team;

			m_healingTime = new int[8];
			m_healingId = new int[8];

			m_lastDamageTime = 60;
			m_hp = 100 * hp;
			m_maxHp = 100 * hp;
			m_originalHp = 100 * hp;
		}

		public void SetMaxRegenerationTime(int time)
		{
			m_maxRegenerationTime = time;
		}

		public void EnableRegeneration(bool state)
		{
			m_regenerationEnabled = state;
		}

		public void SetPoisonDamage(int damage, bool increaseSlowly)
		{
			int time = 8;

			if (m_poisonTime >= 80)
			{
				time = 24;

				if (m_poisonTime >= 320)
				{
					time = 136;

					if (m_poisonTime >= 1000)
					{
						time = 0;
					}
				}
			}

			if (m_poisonDamage != 0)
			{
				if (m_poisonDamage >= damage)
				{
					if (m_poisonDamage > damage)
					{
						time = damage * time / m_poisonDamage;
					}
				}
				else
				{
					m_poisonTime = m_poisonDamage * m_poisonTime / damage;
					m_poisonDamage = damage;
				}
			}
			else
			{
				m_poisonDamage = damage;
			}

			m_poisonTime = increaseSlowly ? LogicMath.Min(time + m_poisonTime, 1000) : 1000;
			m_poisonInitTime = 640;
		}

		public void CauseDamage(int damage, int gameObjectId, LogicGameObject gameObject)
		{
			if (damage >= 0 || m_hp != 0)
			{
				if (m_parent == null)
				{
					if (damage > 0 && m_invulnerabilityTime > 0)
					{
						return;
					}
				}
				else
				{
					LogicCombatComponent combatComponent = m_parent.GetCombatComponent();

					if (combatComponent != null)
					{
						if (combatComponent.GetUndergroundTime() > 0 && damage > 0)
						{
							damage = 0;
						}
					}

					if (!m_parent.GetLevel().GetInvulnerabilityEnabled())
					{
						if (damage > 0 && m_invulnerabilityTime > 0)
						{
							return;
						}
					}
					else
					{
						damage = 0;
					}

					if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)m_parent;
						LogicArrayList<LogicCharacter> childTroops = character.GetChildTroops();

						if (childTroops != null && childTroops.Size() > 0 || character.GetSpawnDelay() > 0)
						{
							return;
						}
					}
				}

				if (gameObjectId != 0 && damage < 0)
				{
					int prevHealingIdx = -1;
					int healingIdx = -1;

					for (int i = 0; i < 8; i++)
					{
						if (m_healingId[i] == gameObjectId)
						{
							prevHealingIdx = i;
						}
						else if (healingIdx == -1)
						{
							healingIdx = i;

							if (m_healingTime[i] > 0)
							{
								healingIdx = -1;
							}
						}
					}

					if (healingIdx < prevHealingIdx && prevHealingIdx != -1 && healingIdx != -1)
					{
						m_healingId[healingIdx] = gameObjectId;
						m_healingTime[healingIdx] = 1000;
						m_healingId[prevHealingIdx] = 0;
						m_healingTime[prevHealingIdx] = 0;
					}
					else if (prevHealingIdx == -1)
					{
						if (healingIdx != -1)
						{
							m_healingId[healingIdx] = gameObjectId;
							m_healingTime[healingIdx] = 1000;
						}
						else
						{
							healingIdx = 8;
						}
					}
					else
					{
						healingIdx = prevHealingIdx;
						m_healingTime[prevHealingIdx] = 1000;
					}

					damage = damage * LogicDataTables.GetGlobals().GetHealStackPercent(healingIdx) / 100;
				}

				int prevHp = (m_hp + 99) / 100;
				int prevAccurateHp = m_hp;
				m_hp = LogicMath.Clamp(m_hp - damage, 0, m_maxHp);
				int hp = (m_hp + 99) / 100;

				if (prevHp > hp)
				{
					LogicResourceStorageComponent resourceStorageComponent =
						(LogicResourceStorageComponent)m_parent.GetComponent(LogicComponentType.RESOURCE_STORAGE);
					LogicResourceProductionComponent resourceProductionComponent =
						(LogicResourceProductionComponent)m_parent.GetComponent(LogicComponentType.RESOURCE_PRODUCTION);
					LogicWarResourceStorageComponent warResourceStorageComponent =
						(LogicWarResourceStorageComponent)m_parent.GetComponent(LogicComponentType.WAR_RESOURCE_STORAGE);

					if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						LogicBuilding building = (LogicBuilding)m_parent;

						if (!building.GetBuildingData().IsLootOnDestruction() || prevAccurateHp > 0 && (m_hp == 0 || (uint)m_hp >= 0xFFFFFF3A))
						{
							if (resourceStorageComponent != null)
								resourceStorageComponent.ResourcesStolen(prevHp - hp, prevHp);
							if (resourceProductionComponent != null)
								resourceProductionComponent.ResourcesStolen(prevHp - hp, prevHp);
							if (warResourceStorageComponent != null)
								warResourceStorageComponent.ResourcesStolen(prevHp - hp, prevHp);
						}
					}

					if (m_parent.IsWall())
						m_parent.RefreshPassable();
					m_lastDamageTime = 0;
				}

				UpdateHeroHealthToAvatar(hp);

				if (damage <= 0)
				{
					if (damage < 0)
					{
						// Listener
					}
				}
				else
				{
					if (m_parent.GetMovementComponent() != null)
						m_parent.GetMovementComponent().SetPatrolFreeze();
				}

				if (prevAccurateHp > 0 && m_hp == 0)
				{
					m_parent.DeathEvent();
					m_parent.GetLevel().UpdateBattleStatus();

					if (m_parent.IsWall())
						WallRemoved();
				}
			}
		}

		public void Kill()
		{
			m_invulnerabilityTime = 0;
			CauseDamage(m_maxHp, 0, null);
		}

		public void CauseDamagePermil(int hp)
		{
			CauseDamage(hp, 0, null);
			m_damagePermilCount += 1;
		}

		public int GetDamagePermilCount()
			=> m_damagePermilCount;

		public void UpdateHeroHealthToAvatar(int hitpoint)
		{
			LogicAvatar avatar = m_team == 1 ? m_parent.GetLevel().GetHomeOwnerAvatar() : m_parent.GetLevel().GetVisitorAvatar();
			LogicHeroData data = null;

			int upgLevel = 0;

			if (m_parent.IsHero())
			{
				LogicCharacter character = (LogicCharacter)m_parent;

				data = (LogicHeroData)character.GetCharacterData();
				upgLevel = character.GetUpgradeLevel();
			}
			else if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)m_parent;
				LogicHeroBaseComponent heroBaseComponent = building.GetHeroBaseComponent();

				if (heroBaseComponent == null)
				{
					return;
				}

				LogicBuildingData buildingData = building.GetBuildingData();

				if (!buildingData.GetShareHeroCombatData())
				{
					return;
				}

				LogicCombatComponent combatComponent = building.GetCombatComponent();

				if (combatComponent == null || !combatComponent.IsEnabled())
				{
					return;
				}

				data = buildingData.GetHeroData();
				upgLevel = avatar.GetUnitUpgradeLevel(data);
			}

			if (data != null)
			{
				int secs = LogicMath.Min(data.GetSecondsToFullHealth(hitpoint, upgLevel), data.GetFullRegenerationTimeSec(upgLevel));

				if (avatar != null)
				{
					avatar.GetChangeListener().CommodityCountChanged(0, data, secs);
					avatar.SetHeroHealth(data, secs);
				}
			}
		}

		public void WallRemoved()
		{
			LogicArrayList<LogicComponent> components = m_parent.GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicCombatComponent combatComponent = components[i].GetParent().GetCombatComponent();

				if (combatComponent != null && combatComponent.GetTarget(0) != null)
				{
					LogicGameObject target = combatComponent.GetTarget(0);

					if (target.IsWall())
					{
						combatComponent.ForceNewTarget();
					}
				}
			}
		}

		public void SetMaxHitpoints(int maxHp)
		{
			m_maxHp = 100 * maxHp;
			m_hp = LogicMath.Clamp(m_hp, 0, m_maxHp);
			m_originalHp = m_maxHp;
		}

		public void SetHitpoints(int hp)
		{
			m_hp = LogicMath.Clamp(100 * hp, 0, m_maxHp);
		}

		public bool IsEnemy(LogicGameObject gameObject)
		{
			LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)gameObject.GetComponent(LogicComponentType.HITPOINT);

			if (hitpointComponent != null && hitpointComponent.GetTeam() != m_team)
			{
				return m_hp > 0;
			}

			return false;
		}

		public bool IsEnemyForTeam(int team)
			=> m_team != team && m_hp > 0;

		public int GetTeam()
			=> m_team;

		public void SetTeam(int team)
		{
			m_team = team;

			if (team != 0)
			{
				if (m_parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)m_parent;
					LogicCombatComponent combatComponent = character.GetCombatComponent();

					if (combatComponent != null)
					{
						if (character.GetCharacterData().IsUnderground())
						{
							combatComponent.SetUndergroundTime(0);
						}

						LogicMovementComponent movementComponent = character.GetMovementComponent();

						if (movementComponent != null)
						{
							movementComponent.SetUnderground(false);
						}
					}
				}
			}
		}

		public int GetInvulnerabilityTime()
			=> m_invulnerabilityTime;

		public void SetInvulnerabilityTime(int ms)
		{
			m_invulnerabilityTime = ms;
		}

		public bool HasFullHitpoints()
			=> m_hp == m_maxHp;

		public void SetDieEffect(LogicEffectData data1, LogicEffectData data2)
		{
			m_dieEffect = data1;
			m_dieEffect = data2;
		}

		public bool IsDamagedRecently()
			=> m_lastDamageTime < 30;

		public override void Tick()
		{
			if (m_hp < m_maxHp && m_regenerationEnabled)
			{
				int rebuildEffectHp = m_maxHp / 5;
				int prevHp = m_hp;

				if (m_maxRegenerationTime <= 0)
				{
					m_hp = m_maxHp;
					m_regenTime = 0;
				}
				else
				{
					m_regenTime += 64;
					int tmp = LogicMath.Max(1000 * m_maxRegenerationTime / (m_maxHp / 100), 1);
					m_hp += 100 * (m_regenTime / tmp);

					if (m_hp >= m_maxHp)
					{
						m_hp = m_maxHp;
						m_regenTime = 0;
					}
					else
					{
						m_regenTime %= tmp;
					}
				}

				if (prevHp < rebuildEffectHp && m_hp >= rebuildEffectHp)
				{
					LogicGameObjectListener listener = GetParentListener();

					if (listener != null)
					{
						// Listener.
					}
				}
			}

			if (m_extraHpTime > 0)
			{
				m_extraHpTime -= 64;

				if (m_extraHpTime <= 0)
				{
					m_extraHpTime = 0;

					if (m_hp > 0)
					{
						if (m_originalHp != m_maxHp)
						{
							m_hp = (int)(m_originalHp * (long)m_hp / m_maxHp);
							m_maxHp = m_originalHp;
						}

						m_extraHpTime = -1;
					}
				}
			}

			if (m_poisonInitTime <= 0)
			{
				if (m_poisonTime > 0)
				{
					m_poisonTime -= 10;

					if (m_poisonTime <= 0)
					{
						m_poisonTime = 0;
						m_poisonDamage = 0;
					}
				}
			}
			else
			{
				m_poisonInitTime = LogicMath.Max(m_poisonInitTime - 64, 0);
			}

			if (m_poisonTime > 0)
			{
				CauseDamage((int)(((m_poisonDamage * m_poisonTime / 1000L) * 64L) / 1000L), 0, null);
			}

			m_invulnerabilityTime = LogicMath.Max(m_invulnerabilityTime - 64, 0);

			for (int i = 0; i < 8; i++)
			{
				m_healingTime[i] -= 64;

				if (m_healingTime[i] <= 0)
				{
					m_healingTime[i] = 0;
					m_healingId[i] = 0;
				}
			}

			m_lastDamageTime += 1;

			if (m_shrinkTime > 0)
			{
				m_shrinkTime -= 1;

				if (m_shrinkTime == 0)
				{
					Debugger.HudPrint("HP TO ORIGINAL");

					m_hp = LogicMath.Min(m_originalHp * (100 * m_hp / m_maxHp) / 100, m_originalHp);
					m_maxHp = m_originalHp;
				}
			}
		}

		public override void FastForwardTime(int time)
		{
			if (m_regenerationEnabled)
			{
				m_regenTime += 64;

				if (m_maxRegenerationTime <= time)
				{
					m_hp = m_maxHp;
				}
				else
				{
					m_hp += m_maxHp * time / m_maxRegenerationTime;

					if (m_hp > m_maxHp)
					{
						m_hp = m_maxHp;
					}
				}
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.HITPOINT;

		public override void Load(LogicJSONObject jsonObject)
		{
			LoadHitpoint(jsonObject);
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			LoadHitpoint(jsonObject);

			m_hp = m_maxHp;
			m_regenerationEnabled = false;
		}

		public void LoadHitpoint(LogicJSONObject jsonObject)
		{
			LogicJSONNumber hpNumber = jsonObject.GetJSONNumber("hp");
			LogicJSONBoolean regenBoolean = jsonObject.GetJSONBoolean("reg");

			if (hpNumber != null)
			{
				m_hp = m_parent.GetLevel().GetState() != 2 ? LogicMath.Clamp(hpNumber.GetIntValue(), 0, m_maxHp) : m_maxHp;
			}
			else
			{
				m_hp = m_maxHp;
			}

			m_regenerationEnabled = regenBoolean != null && regenBoolean.IsTrue();
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			if (m_hp < m_maxHp)
			{
				jsonObject.Put("hp", new LogicJSONNumber(m_hp));
				jsonObject.Put("reg", new LogicJSONBoolean(m_regenerationEnabled));
			}
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			if (m_hp < m_maxHp)
			{
				jsonObject.Put("hp", new LogicJSONNumber(m_hp));
				jsonObject.Put("reg", new LogicJSONBoolean(m_regenerationEnabled));
			}
		}

		public int GetHitpoints()
			=> m_hp;

		public int GetMaxHitpoints()
			=> m_maxHp;

		public int GetOriginalHitpoints()
			=> m_originalHp;

		public void SetExtraHealth(int hp, int time)
		{
			if (m_hp > 0)
			{
				if (time == -1 || m_maxHp <= hp)
				{
					if (m_maxHp != hp)
					{
						m_hp = (int)((long)hp * m_hp / m_maxHp);
						m_maxHp = hp;
					}

					m_extraHpTime = time;
				}
			}
		}

		public void SetShrinkHitpoints(int time, int hp)
		{
			m_shrinkTime = time;
			m_maxHp = hp * m_originalHp / 100;
			m_hp = LogicMath.Min(m_hp, m_maxHp);
		}

		public int GetPoisonRemainingMS()
			=> m_poisonInitTime + (m_poisonTime << 6) / 10;
	}
}