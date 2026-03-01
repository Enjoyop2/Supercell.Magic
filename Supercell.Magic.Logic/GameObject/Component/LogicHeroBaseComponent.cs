using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicHeroBaseComponent : LogicComponent
	{
		public const int PATROL_PATHS = 8;

		private LogicTimer m_timer;
		private LogicArrayList<LogicVector2> m_patrolPath;

		private readonly LogicHeroData m_hero;

		private int m_healthTime;
		private int m_upgLevel;

		private bool m_sharedHeroCombatData;

		public LogicHeroBaseComponent(LogicGameObject gameObject, LogicHeroData data) : base(gameObject)
		{
			m_hero = data;
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
			m_healthTime += 64;

			int regenTime = 1000;

			if (m_parent.GetRemainingBoostTime() > 0 && !m_parent.IsBoostPaused())
			{
				regenTime /= LogicDataTables.GetGlobals().GetHeroRestBoostMultiplier();
			}

			if (m_parent.GetLevel().GetRemainingClockTowerBoostTime() > 0)
			{
				LogicGameObjectData data = m_parent.GetData();

				if (data.GetDataType() == DataType.BUILDING && data.GetVillageType() == 1)
				{
					regenTime /= LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier();

					if (m_timer != null)
					{
						m_timer.SetFastForward(m_timer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
					}
				}
			}

			if (m_healthTime > regenTime)
			{
				if (m_parent.GetLevel().GetPlayerAvatar().FastForwardHeroHealth(m_hero, 1) && GetParentListener() != null)
				{
					// LOAD EFFECT.
				}

				m_healthTime -= regenTime;
			}

			if (m_timer != null)
			{
				if (m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) == 0)
				{
					FinishUpgrading(true);
				}
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.HERO_BASE;

		public void FinishUpgrading(bool tick)
		{
			if (m_timer != null)
			{
				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

				if (homeOwnerAvatar.GetUnitUpgradeLevel(m_hero) < m_upgLevel || m_upgLevel == 0)
				{
					homeOwnerAvatar.CommodityCountChangeHelper(1, m_hero, 1);
				}

				m_parent.GetLevel().GetWorkerManagerAt(m_parent.GetData().GetVillageType()).DeallocateWorker(m_parent);

				homeOwnerAvatar.SetHeroState(m_hero, 3);
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 3);

				SetFullHealth();

				m_timer.Destruct();
				m_timer = null;
			}
			else
			{
				Debugger.Warning("LogicHeroBaseComponent::finishUpgrading called and m_pHero is NULL");
			}
		}

		public bool IsUpgrading()
			=> m_timer != null;

		public void SetFullHealth()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			homeOwnerAvatar.SetHeroHealth(m_hero, 0);
			homeOwnerAvatar.GetChangeListener().CommodityCountChanged(0, m_hero, 0);
		}

		public int GetRemainingUpgradeSeconds()
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
			if (m_timer != null)
			{
				return m_hero.GetUpgradeTime(m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(m_hero));
			}

			return 0;
		}

		public LogicHeroData GetHeroData()
			=> m_hero;

		public void SetSharedHeroCombatData(bool value)
		{
			m_sharedHeroCombatData = value;
		}

		public override void Save(LogicJSONObject root, int villageType)
		{
			if (m_timer != null && m_hero != null)
			{
				LogicJSONObject jsonObject = new LogicJSONObject();

				jsonObject.Put("level", new LogicJSONNumber(m_upgLevel));
				jsonObject.Put("t", new LogicJSONNumber(m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));

				if (m_timer.GetEndTimestamp() != -1)
				{
					jsonObject.Put("t_end", new LogicJSONNumber(m_timer.GetEndTimestamp()));
				}

				if (m_timer.GetFastForward() > 0)
				{
					jsonObject.Put("t_ff", new LogicJSONNumber(m_timer.GetFastForward()));
				}

				root.Put("hero_upg", jsonObject);
			}
		}

		public override void SaveToSnapshot(LogicJSONObject root, int layoutId)
		{
			if (m_timer != null && m_hero != null)
			{
				LogicJSONObject jsonObject = new LogicJSONObject();

				jsonObject.Put("level", new LogicJSONNumber(m_upgLevel));
				jsonObject.Put("t", new LogicJSONNumber(m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime())));

				root.Put("hero_upg", jsonObject);
			}
		}

		public override void Load(LogicJSONObject root)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			LogicJSONObject jsonObject = root.GetJSONObject("hero_upg");

			if (jsonObject != null)
			{
				LogicJSONNumber levelObject = jsonObject.GetJSONNumber("level");
				LogicJSONNumber timerObject = jsonObject.GetJSONNumber("t");
				LogicJSONNumber timerEndObject = jsonObject.GetJSONNumber("t_end");
				LogicJSONNumber timerFastForwardObject = jsonObject.GetJSONNumber("t_ff");

				if (levelObject != null)
				{
					m_upgLevel = levelObject.GetIntValue();
				}

				if (timerObject != null)
				{
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

					m_parent.GetLevel().GetWorkerManagerAt(m_parent.GetVillageType()).AllocateWorker(m_parent);
				}
			}
		}

		public override void LoadFromSnapshot(LogicJSONObject root)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			LogicJSONObject jsonObject = root.GetJSONObject("hero_upg");

			if (jsonObject != null)
			{
				LogicJSONNumber levelObject = jsonObject.GetJSONNumber("level");

				if (levelObject != null)
				{
					m_upgLevel = levelObject.GetIntValue();
				}
			}
		}

		public override void LoadingFinished()
		{
			if (m_parent.GetLevel().IsInCombatState())
			{
				if (m_parent.GetVillageType() == m_parent.GetLevel().GetVillageType())
				{
					if (m_parent.GetLevel().GetVillageType() == m_parent.GetVillageType())
					{
						m_patrolPath = CreatePatrolPath();
					}
				}
			}

			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			LogicBuilding building = (LogicBuilding)m_parent;

			if (!building.IsLocked() && homeOwnerAvatar.GetHeroState(m_hero) == 0)
			{
				homeOwnerAvatar.SetHeroState(m_hero, 3);
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 3);
			}

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

				if (!building.IsLocked() && homeOwnerAvatar.GetHeroState(m_hero) != 1)
				{
					homeOwnerAvatar.SetHeroState(m_hero, 1);
					homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 1);
				}
			}
			else
			{
				if (!building.IsLocked() && homeOwnerAvatar.GetHeroState(m_hero) == 1)
				{
					homeOwnerAvatar.SetHeroState(m_hero, 3);
					homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 3);
				}
			}

			if (m_hero.HasNoDefence() && !m_parent.GetLevel().IsInCombatState() && homeOwnerAvatar.GetHeroState(m_hero) == 3)
			{
				homeOwnerAvatar.SetHeroState(m_hero, 2);
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 2);
			}

			if (homeOwnerAvatar.GetHeroState(m_hero) == 3)
			{
				if (m_parent.GetLevel().IsInCombatState())
				{
					if (!m_sharedHeroCombatData && !m_hero.HasNoDefence())
					{
						if (m_parent.GetVillageType() == m_parent.GetLevel().GetVillageType())
						{
							AddDefendingHero();
						}
					}
				}
			}

			int heroHealth = homeOwnerAvatar.GetHeroHealth(m_hero);
			int fullRegenerationTime = m_hero.GetFullRegenerationTimeSec(homeOwnerAvatar.GetUnitUpgradeLevel(m_hero));

			if (fullRegenerationTime < heroHealth)
			{
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(0, m_hero, fullRegenerationTime);
				homeOwnerAvatar.SetHeroHealth(m_hero, fullRegenerationTime);
			}
		}

		public override void FastForwardTime(int time)
		{
			int heroHealthTime = time;
			int constructionBoostTime = 0;
			int remainingBoostTime = m_parent.GetRemainingBoostTime();

			if (remainingBoostTime > 0)
			{
				if (!m_parent.IsBoostPaused())
				{
					heroHealthTime += LogicMath.Min(remainingBoostTime, time) * (LogicDataTables.GetGlobals().GetHeroRestBoostMultiplier() - 1);
				}
			}

			int clockTowerBoostTime = m_parent.GetLevel().GetUpdatedClockTowerBoostTime();

			if (clockTowerBoostTime > 0 && m_parent.GetLevel().IsClockTowerBoostPaused())
			{
				LogicGameObjectData data = m_parent.GetData();

				if (data.GetDataType() == DataType.BUILDING && data.GetVillageType() == 1)
				{
					int boost = LogicMath.Min(clockTowerBoostTime, time) * (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1);

					heroHealthTime += boost;
					constructionBoostTime += boost;
				}
			}

			m_parent.GetLevel().GetHomeOwnerAvatar().FastForwardHeroHealth(m_hero, heroHealthTime);

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

				if (constructionBoostTime > 0)
				{
					m_timer.SetFastForward(m_timer.GetFastForward() + 60 * constructionBoostTime);
				}
			}
		}

		public LogicArrayList<LogicVector2> GetPatrolPath()
			=> m_patrolPath;

		public LogicArrayList<LogicVector2> CreatePatrolPath()
		{
			int parentWidth = m_parent.GetWidthInTiles() << 8;
			int parentHeight = m_parent.GetHeightInTiles() << 8;
			int patrolRadius = m_hero.GetPatrolRadius();

			if (patrolRadius * patrolRadius >= parentWidth * parentWidth + parentHeight * parentHeight)
			{
				LogicVector2 tmp1 = new LogicVector2();
				LogicVector2 tmp2 = new LogicVector2();
				LogicVector2 tmp3 = new LogicVector2();
				LogicVector2 tmp4 = new LogicVector2();

				int parentMidX = m_parent.GetMidX();
				int parentMidY = m_parent.GetMidY();

				tmp2.Set(parentMidX, parentMidY);

				LogicArrayList<LogicVector2> wayPoints = new LogicArrayList<LogicVector2>(LogicHeroBaseComponent.PATROL_PATHS);

				for (int i = 0, j = 22; i < LogicHeroBaseComponent.PATROL_PATHS; i++, j += 45)
				{
					tmp1.Set(parentMidX + LogicMath.Cos(j, patrolRadius), parentMidY + LogicMath.Sin(j, patrolRadius));
					LogicHeroBaseComponent.FindPoint(m_parent.GetLevel().GetTileMap(), tmp3, tmp2, tmp1, tmp4);
					wayPoints.Add(new LogicVector2(tmp4.m_x, tmp4.m_y));
				}

				tmp1.Destruct();
				tmp2.Destruct();
				tmp3.Destruct();
				tmp4.Destruct();

				return wayPoints;
			}
			else
			{
				int startX = m_parent.GetX() + (m_parent.GetWidthInTiles() << 9) - 128;
				int startY = m_parent.GetY() + (m_parent.GetWidthInTiles() << 9) - 128;
				int endX = m_parent.GetX() + 128;
				int endY = m_parent.GetY() + 128;

				LogicArrayList<LogicVector2> wayPoints = new LogicArrayList<LogicVector2>(4);

				wayPoints.Add(new LogicVector2(startX, startY));
				wayPoints.Add(new LogicVector2(endX, startY));
				wayPoints.Add(new LogicVector2(endX, endY));
				wayPoints.Add(new LogicVector2(startX, endY));

				return wayPoints;
			}
		}

		public void AddDefendingHero()
		{
			LogicAvatar visitorAvatar = m_parent.GetLevel().GetVisitorAvatar();
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			int randomPatrolPoint = visitorAvatar != null
				? (int)(((visitorAvatar.GetResourceCount(LogicDataTables.GetGoldData()) + 10 * m_hero.GetGlobalID()) & 0x7FFFFFFFu) % m_patrolPath.Size())
				: 0;
			int upgLevel = homeOwnerAvatar.GetUnitUpgradeLevel(m_hero);
			int heroHitpoints = m_hero.GetHeroHitpoints(homeOwnerAvatar.GetHeroHealth(m_hero), upgLevel);

			if (m_hero.HasEnoughHealthForAttack(heroHitpoints, upgLevel))
			{
				LogicVector2 patrolPoint = m_patrolPath[randomPatrolPoint];
				LogicCharacter hero = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(m_hero, m_parent.GetLevel(), m_parent.GetVillageType());

				hero.GetMovementComponent().SetBaseBuilding((LogicBuilding)m_parent);
				hero.GetHitpointComponent().SetTeam(1);
				hero.SetUpgradeLevel(upgLevel);
				hero.GetHitpointComponent().SetHitpoints(heroHitpoints);

				hero.SetInitialPosition(patrolPoint.m_x, patrolPoint.m_y);

				m_parent.GetGameObjectManager().AddGameObject(hero, -1);

				hero.GetCombatComponent().SetSearchRadius(m_hero.GetMaxSearchRadiusForDefender() / 512);

				if (LogicDataTables.GetGlobals().EnableDefendingAllianceTroopJump())
				{
					hero.GetMovementComponent().EnableJump(3600000);
				}
			}
		}

		public bool SpeedUp()
		{
			if (m_timer != null)
			{
				int remainingSecs = m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());
				int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(remainingSecs, 0, m_parent.GetVillageType());

				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

				if (homeOwnerAvatar.IsClientAvatar())
				{
					LogicClientAvatar clientAvatar = (LogicClientAvatar)homeOwnerAvatar;

					if (clientAvatar.HasEnoughDiamonds(speedUpCost, true, m_parent.GetLevel()))
					{
						clientAvatar.UseDiamonds(speedUpCost);
						clientAvatar.GetChangeListener().DiamondPurchaseMade(10, m_hero.GetGlobalID(), clientAvatar.GetUnitUpgradeLevel(m_hero) + 1, speedUpCost,
																			 m_parent.GetLevel().GetVillageType());
						FinishUpgrading(true);

						return true;
					}
				}
			}

			return false;
		}

		public void CancelUpgrade()
		{
			if (m_timer != null)
			{
				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
				int upgradeLevel = homeOwnerAvatar.GetUnitUpgradeLevel(m_hero);
				int upgradeCost = m_hero.GetUpgradeCost(upgradeLevel);
				LogicResourceData upgradeResourceData = m_hero.GetUpgradeResource(upgradeLevel);

				homeOwnerAvatar.CommodityCountChangeHelper(0, upgradeResourceData, LogicDataTables.GetGlobals().GetHeroUpgradeCancelMultiplier() * upgradeCost / 100);

				m_parent.GetLevel().GetWorkerManagerAt(m_parent.GetData().GetVillageType()).DeallocateWorker(m_parent);

				homeOwnerAvatar.SetHeroState(m_hero, 3);
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 3);

				m_timer.Destruct();
				m_timer = null;
			}
			else
			{
				Debugger.Warning("LogicHeroBaseComponent::cancelUpgrade called even upgrade is not on going!");
			}
		}

		public bool CanStartUpgrading(bool callListener)
		{
			if (m_timer == null)
			{
				if (!IsMaxLevel())
				{
					LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

					int requiredTownHallLevel = m_hero.GetRequiredTownHallLevel(homeOwnerAvatar.GetUnitUpgradeLevel(m_hero) + 1);
					int townHallLevel = m_parent.GetLevel().GetTownHallLevel(m_parent.GetLevel().GetVillageType());

					if (townHallLevel >= requiredTownHallLevel)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void StartUpgrading()
		{
			if (CanStartUpgrading(true))
			{
				((LogicBuilding)m_parent).DestructBoost();

				if (m_timer != null)
				{
					m_timer.Destruct();
					m_timer = null;
				}

				LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

				m_parent.GetLevel().GetWorkerManagerAt(m_parent.GetData().GetVillageType()).AllocateWorker(m_parent);

				m_timer = new LogicTimer();
				m_timer.StartTimer(GetTotalSeconds(), m_parent.GetLevel().GetLogicTime(), true,
										m_parent.GetLevel().GetHomeOwnerAvatarChangeListener().GetCurrentTimestamp());
				m_upgLevel = homeOwnerAvatar.GetUnitUpgradeLevel(m_hero) + 1;

				homeOwnerAvatar.SetHeroState(m_hero, 1);
				homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, 1);
			}
		}

		public bool IsMaxLevel()
			=> m_parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(m_hero) >= m_hero.GetUpgradeLevelCount() - 1;

		public int GetSpeedUpHealthCost()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar.IsClientAvatar())
			{
				return homeOwnerAvatar.GetHeroHealCost(m_hero);
			}

			return 0;
		}

		public bool SpeedUpHealth()
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar.IsClientAvatar())
			{
				LogicClientAvatar clientAvatar = (LogicClientAvatar)homeOwnerAvatar;
				int speedUpCost = GetSpeedUpHealthCost();

				if (clientAvatar.HasEnoughDiamonds(speedUpCost, true, m_parent.GetLevel()))
				{
					clientAvatar.UseDiamonds(speedUpCost);
					clientAvatar.GetChangeListener().DiamondPurchaseMade(9, m_hero.GetGlobalID(), clientAvatar.GetUnitUpgradeLevel(m_hero) + 1, speedUpCost,
																		 m_parent.GetLevel().GetVillageType());

					SetFullHealth();

					return true;
				}
			}

			return false;
		}

		public bool SetSleep(bool enabled)
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();
			int state = homeOwnerAvatar.GetHeroState(m_hero);

			if (state != 0)
			{
				int newState = enabled ? 2 : 3;

				if (state != newState)
				{
					homeOwnerAvatar.SetHeroState(m_hero, newState);
					homeOwnerAvatar.GetChangeListener().CommodityCountChanged(2, m_hero, newState);

					return true;
				}
			}

			return false;
		}

		public bool SetHeroMode(int mode)
		{
			LogicAvatar homeOwnerAvatar = m_parent.GetLevel().GetHomeOwnerAvatar();

			if (homeOwnerAvatar.GetHeroMode(m_hero) == mode)
			{
				return false;
			}

			homeOwnerAvatar.SetHeroMode(m_hero, mode);
			homeOwnerAvatar.GetChangeListener().CommodityCountChanged(3, m_hero, mode);

			return true;
		}

		public static bool FindPoint(LogicTileMap tileMap, LogicVector2 pos1, LogicVector2 pos2, LogicVector2 pos3, LogicVector2 pos4)
		{
			pos1.Set(pos2.m_x, pos2.m_y);
			pos1.Substract(pos3);

			int length = pos1.GetLength();

			pos1.m_x = (pos1.m_x << 7) / length;
			pos1.m_y = (pos1.m_y << 7) / length;

			pos4.Set(pos3.m_x, pos3.m_y);

			int radius = LogicMath.Clamp(length / 128, 10, 25);

			for (int i = 0; i < radius; i++)
			{
				if (tileMap.IsPassablePathFinder(pos4.m_x >> 8, pos4.m_y >> 8))
				{
					pos4.m_x = (int)((pos4.m_x & 0xFFFFFF00) | 128);
					pos4.m_y = (int)((pos4.m_y & 0xFFFFFF00) | 128);

					return true;
				}

				pos4.Add(pos1);
			}

			return false;
		}
	}
}