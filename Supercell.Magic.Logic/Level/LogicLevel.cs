using System;

using Supercell.Magic.Logic.Achievement;
using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Avatar.Change;
using Supercell.Magic.Logic.Battle;
using Supercell.Magic.Logic.Calendar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Cooldown;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Logic.Mission;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Logic.Offer;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Logic.Worker;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Level
{
	public class LogicLevel
	{
		public const int VILLAGE_COUNT = 2;
		public const int EXPERIENCE_VERSION = 1;

		public const int LEVEL_WIDTH = 25600;
		public const int LEVEL_HEIGHT = 25600;
		public const int NPC_LEVEL_WIDTH = 22528;
		public const int NPC_LEVEL_HEIGHT = 22528;

		public const int TILEMAP_SIZE_X = 50;
		public const int TILEMAP_SIZE_Y = 50;

		private readonly LogicTime m_time;
		private LogicGameMode m_gameMode;
		private LogicClientHome m_home;
		private LogicAvatar m_homeOwnerAvatar;
		private LogicAvatar m_visitorAvatar;
		private LogicTileMap m_tileMap;
		private LogicNpcAttack m_npcAttack;
		private LogicRect m_playArea;
		private LogicLeagueData m_leagueData;
		private LogicLeagueData m_visitorLeagueData;
		private LogicLong m_revengeId;

		private readonly LogicGameObjectManager[] m_gameObjectManagers;
		private readonly LogicWorkerManager[] m_workerManagers;
		private LogicOfferManager m_offerManager;
		private LogicAchievementManager m_achievementManager;
		private LogicCooldownManager m_cooldownManager;
		private LogicMissionManager m_missionManager;
		private LogicBattleLog m_battleLog;
		private LogicGameListener m_gameListener;
		private LogicJSONObject m_levelJSON;

		private readonly LogicArrayList<int> m_layoutState;
		private readonly LogicArrayList<int> m_layoutCooldown;
		private readonly LogicArrayList<int> m_layoutStateVillage2;
		private readonly LogicArrayList<string> m_armyNames;
		private LogicArrayList<LogicHeroData> m_placedHeroData;
		private LogicArrayList<LogicCharacter> m_placedHero;
		private LogicArrayList<LogicDataSlot> m_unplacedObjects;

		private LogicArrayList<int> m_newShopBuildings;
		private LogicArrayList<int> m_newShopTraps;
		private LogicArrayList<int> m_newShopDecos;

		private int m_liveReplayUpdateFrequency;

		private int m_loadingVillageType;
		private int m_villageType;
		private int m_warLayout;
		private int m_activeLayout;
		private int m_activeLayoutVillage2;
		private int m_lastLeagueRank;
		private int m_lastAllianceLevel;
		private int m_lastSeasonSeen;
		private int m_lastSeenNews;
		private int m_waveNumber;
		private int m_experienceVersion;
		private int m_warTutorialsSeen;
		private int m_matchType;
		private int m_remainingClockTowerBoostTime;
		private int m_levelWidth;
		private int m_levelHeight;
		private int m_aliveBuildingCount;
		private int m_destructibleBuildingCount;
		private int m_shieldActivatedHours;
		private int m_previousAttackStars;

		private bool m_helpOpened;
		private bool m_warBase;
		private bool m_war;
		private bool m_editModeShown;
		private bool m_npcVillage;
		private bool m_androidClient;
		private bool m_battleStarted;
		private bool m_battleEndPending;
		private bool m_arrangedWarBase;
		private bool m_arrangedWar;
		private bool m_invulnerabilityEnabled;
		private bool m_lastLeagueShuffle;
		private bool m_attackShieldCostOpened;
		private bool m_layoutEditShownErase;
		private bool m_readyForAttack;
		private bool m_ignoreAttack;

		private bool m_feedbackTownHallDestroyed;
		private bool m_feedbackDestruction25;
		private bool m_feedbackDestruction50;
		private bool m_feedbackDestruction75;

		private string m_warTroopRequestMessage;
		private string m_troopRequestMessage;

		public LogicLevel(LogicGameMode gameMode)
		{
			m_gameMode = gameMode;

			m_troopRequestMessage = string.Empty;
			m_warTroopRequestMessage = string.Empty;
			m_lastSeenNews = -1;
			m_loadingVillageType = -1;
			m_readyForAttack = true;

			m_time = new LogicTime();
			m_gameListener = new LogicGameListener();
			m_achievementManager = new LogicAchievementManager(this);
			m_layoutState = new LogicArrayList<int>();
			m_armyNames = new LogicArrayList<string>(4);
			m_gameObjectManagers = new LogicGameObjectManager[LogicLevel.VILLAGE_COUNT];
			m_workerManagers = new LogicWorkerManager[LogicLevel.VILLAGE_COUNT];
			m_tileMap = new LogicTileMap(LogicLevel.TILEMAP_SIZE_X, LogicLevel.TILEMAP_SIZE_Y);

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_workerManagers[i] = new LogicWorkerManager(this);
				m_gameObjectManagers[i] = new LogicGameObjectManager(m_tileMap, this, i);
			}

			m_levelWidth = LogicLevel.LEVEL_WIDTH;
			m_levelHeight = LogicLevel.LEVEL_HEIGHT;
			m_offerManager = new LogicOfferManager(this);
			m_playArea = new LogicRect(3, 3, 47, 47);
			m_cooldownManager = new LogicCooldownManager();
			m_battleLog = new LogicBattleLog(this);
			m_missionManager = new LogicMissionManager(this);
			m_layoutState = new LogicArrayList<int>(8);
			m_layoutCooldown = new LogicArrayList<int>(8);
			m_layoutStateVillage2 = new LogicArrayList<int>(8);
			m_unplacedObjects = new LogicArrayList<LogicDataSlot>();
			m_newShopBuildings = new LogicArrayList<int>();
			m_newShopTraps = new LogicArrayList<int>();
			m_newShopDecos = new LogicArrayList<int>();

			LogicDataTable buildingTable = LogicDataTables.GetTable(DataType.BUILDING);
			LogicDataTable trapTable = LogicDataTables.GetTable(DataType.TRAP);
			LogicDataTable decoTable = LogicDataTables.GetTable(DataType.DECO);

			m_newShopBuildings.EnsureCapacity(buildingTable.GetItemCount());

			for (int i = 0; i < buildingTable.GetItemCount(); i++)
			{
				m_newShopBuildings.Add(0);
			}

			m_newShopBuildings.EnsureCapacity(trapTable.GetItemCount());

			for (int i = 0; i < trapTable.GetItemCount(); i++)
			{
				m_newShopTraps.Add(0);
			}

			m_newShopBuildings.EnsureCapacity(decoTable.GetItemCount());

			for (int i = 0; i < decoTable.GetItemCount(); i++)
			{
				m_newShopDecos.Add(0);
			}

			if (LogicDataTables.GetGlobals().LiveReplayEnabled())
			{
				m_liveReplayUpdateFrequency = LogicTime.GetSecondsInTicks(LogicDataTables.GetGlobals().GetLiveReplayUpdateFrequencySecs());
			}

			for (int i = 0; i < 8; i++)
			{
				m_layoutState.Add(0);
			}

			for (int i = 0; i < 8; i++)
			{
				m_layoutCooldown.Add(0);
			}

			for (int i = 0; i < 8; i++)
			{
				m_layoutStateVillage2.Add(0);
			}

			for (int i = 0; i < 4; i++)
			{
				m_armyNames.Add(string.Empty);
			}
		}

		public LogicGameMode GetGameMode()
			=> m_gameMode;

		public LogicCalendar GetCalendar()
			=> m_gameMode.GetCalendar();

		public LogicConfiguration GetConfiguration()
			=> m_gameMode.GetConfiguration();

		public LogicGameListener GetGameListener()
			=> m_gameListener;

		public void SetGameListener(LogicGameListener listener)
		{
			m_gameListener = listener;
		}

		public bool GetInvulnerabilityEnabled()
			=> m_invulnerabilityEnabled;

		public void SetInvulnerabilityEnabled(bool state)
		{
			m_invulnerabilityEnabled = state;
		}

		public bool IsEditModeShown()
			=> m_editModeShown;

		public bool GetIgnoreAttack()
			=> m_ignoreAttack;

		public void SetEditModeShown()
		{
			m_editModeShown = true;
		}

		public void SetShieldCostPopupShown(bool seen)
		{
			m_attackShieldCostOpened = seen;
		}

		public void SetHelpOpened(bool opened)
		{
			m_helpOpened = opened;
		}

		public void SetAttackShieldCostOpened(bool opened)
		{
			m_attackShieldCostOpened = opened;
		}

		public int GetPreviousAttackStars()
			=> m_previousAttackStars;

		public int GetState()
		{
			if (m_gameMode != null)
				return m_gameMode.GetState();
			return 0;
		}

		public int GetMatchType()
			=> m_matchType;

		public void SetMatchType(int matchType, LogicLong revengeId)
		{
			m_matchType = matchType;
			m_revengeId = revengeId;

			if (matchType == 2)
			{
				m_npcVillage = true;
				m_levelWidth = LogicLevel.NPC_LEVEL_WIDTH;
				m_levelHeight = LogicLevel.NPC_LEVEL_HEIGHT;
			}
		}

		public string GetTroopRequestMessage()
			=> m_troopRequestMessage;

		public void SetTroopRequestMessage(string message)
		{
			m_troopRequestMessage = message;
		}

		public void SetWarTroopRequestMessage(string message)
		{
			m_warTroopRequestMessage = message;
		}

		public int GetRemainingClockTowerBoostTime()
			=> m_remainingClockTowerBoostTime;

		public int GetWarLayout()
			=> m_warLayout;

		public int GetActiveLayout(int villageType)
			=> villageType == 0 ? m_activeLayout : m_activeLayoutVillage2;

		public int GetActiveLayout()
		{
			if (m_loadingVillageType != -1)
			{
				return m_loadingVillageType == 0 ? m_activeLayout : m_activeLayoutVillage2;
			}

			return m_villageType == 0 ? m_activeLayout : m_activeLayoutVillage2;
		}

		public int GetCurrentLayout()
		{
			if (!m_arrangedWar || m_gameMode.GetVisitType() == 5 || !m_arrangedWarBase)
			{
				if (m_matchType != 5 && m_gameMode.GetVisitType() != 4 && m_gameMode.GetVisitType() != 5 && m_gameMode.GetVisitType() != 1 &&
					m_gameMode.GetVillageType() != 1)
				{
					return m_villageType != 0 ? m_activeLayoutVillage2 : m_activeLayout;
				}

				return m_warLayout;
			}

			return 7;
		}

		public void SetActiveLayout(int layout, int villageType)
		{
			if (villageType == 0)
			{
				m_activeLayout = layout;
			}
			else
			{
				m_activeLayoutVillage2 = layout;
			}
		}

		public void SetActiveWarLayout(int layout)
		{
			m_warLayout = layout;
		}

		public void SetLayoutCooldownSecs(int index, int secs)
		{
			if ((index & 0xFFFFFFFE) != 6 && m_villageType == 0)
			{
				m_layoutCooldown[index] = 15 * secs;
			}
		}

		public int GetLayoutCooldown(int index)
			=> m_layoutCooldown[index];

		public int GetLayoutState(int idx, int villageType)
			=> villageType != 1 ? m_layoutState[idx] : m_layoutStateVillage2[idx];

		public void SetLayoutState(int layoutId, int villageType, int state)
		{
			(villageType == 0 ? m_layoutState : m_layoutStateVillage2)[layoutId] = state;
		}

		public int GetTownHallLevel(int villageType)
		{
			LogicBuilding townHall = m_gameObjectManagers[villageType].GetTownHall();

			if (townHall != null)
			{
				return townHall.GetUpgradeLevel();
			}

			return 0;
		}

		public int GetRequiredTownHallLevelForLayout(int layoutId, int villageType)
		{
			if (villageType <= -1)
			{
				villageType = m_villageType;
			}

			if (layoutId > 7)
			{
				Debugger.Warning("unknown layout in getRequiredTownHallLevelForLayout");
				return 10000;
			}

			switch (layoutId)
			{
				case 0:
				case 1:
					return 0;
				case 2:
				case 4:
					if (villageType == 1)
					{
						return LogicDataTables.GetGlobals().GetLayoutTownHallLevelVillage2Slot2();
					}

					return LogicDataTables.GetGlobals().GetLayoutTownHallLevelSlot2();
				case 3:
				case 5:
					if (villageType == 1)
					{
						return LogicDataTables.GetGlobals().GetLayoutTownHallLevelVillage2Slot3();
					}

					return LogicDataTables.GetGlobals().GetLayoutTownHallLevelSlot3();
				default:
					return 0;
			}
		}

		public void SaveLayout(int inputLayoutId, int outputLayoutId)
		{
			int villageType = m_villageType;

			if (outputLayoutId == 6 || outputLayoutId == 7)
			{
				villageType = 0;
			}

			LogicArrayList<LogicGameObject> gameObjects = new LogicArrayList<LogicGameObject>(500);
			LogicGameObjectFilter filter = new LogicGameObjectFilter();

			filter.AddGameObjectType(LogicGameObjectType.BUILDING);
			filter.AddGameObjectType(LogicGameObjectType.TRAP);
			filter.AddGameObjectType(LogicGameObjectType.DECO);

			m_gameObjectManagers[villageType].GetGameObjects(gameObjects, filter);

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicGameObject gameObject = gameObjects[i];
				LogicVector2 layoutPosition = gameObject.GetPositionLayout(inputLayoutId, false);
				LogicVector2 editModePosition = gameObject.GetPositionLayout(inputLayoutId, true);

				gameObject.SetPositionLayoutXY(layoutPosition.m_x, layoutPosition.m_y, outputLayoutId, false);
				gameObject.SetPositionLayoutXY(editModePosition.m_x, editModePosition.m_y, outputLayoutId, true);

				if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
				{
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent(false);

					if (combatComponent != null)
					{
						if (combatComponent.HasAltAttackMode())
						{
							if (combatComponent.UseAltAttackMode(inputLayoutId, false) ^ combatComponent.UseAltAttackMode(outputLayoutId, false))
							{
								combatComponent.ToggleAttackMode(outputLayoutId, false);
							}

							if (combatComponent.UseAltAttackMode(inputLayoutId, true) ^ combatComponent.UseAltAttackMode(outputLayoutId, true))
							{
								combatComponent.ToggleAttackMode(outputLayoutId, true);
							}
						}

						if (combatComponent.GetAttackerItemData().GetTargetingConeAngle() != 0)
						{
							int aimAngle1 = combatComponent.GetAimAngle(inputLayoutId, false);
							int aimAngle2 = combatComponent.GetAimAngle(outputLayoutId, false);

							if (aimAngle1 != aimAngle2)
							{
								combatComponent.ToggleAimAngle(aimAngle1 - aimAngle2, outputLayoutId, false);
							}
						}
					}
				}
				else if (gameObject.GetGameObjectType() == LogicGameObjectType.TRAP)
				{
					LogicTrap trap = (LogicTrap)gameObject;

					if (trap.HasAirMode())
					{
						if (trap.IsAirMode(inputLayoutId, false) ^ trap.IsAirMode(outputLayoutId, false))
						{
							trap.ToggleAirMode(outputLayoutId, false);
						}

						if (trap.IsAirMode(inputLayoutId, true) ^ trap.IsAirMode(outputLayoutId, true))
						{
							trap.ToggleAirMode(outputLayoutId, true);
						}
					}
				}
			}

			gameObjects.Destruct();
			filter.Destruct();
		}

		public int GetBuildingCount(bool includeDestructed, bool includeLocked)
		{
			LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.BUILDING);
			int cnt = 0;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicBuilding building = (LogicBuilding)gameObjects[i];
				LogicBuildingData buildingData = building.GetBuildingData();
				LogicHitpointComponent hitpointComponent = building.GetHitpointComponent();

				if (includeLocked || !building.IsLocked())
				{
					if (hitpointComponent != null)
					{
						if (!buildingData.IsWall())
						{
							if (includeDestructed)
							{
								++cnt;
							}
							else
							{
								if (building.GetHitpointComponent().GetHitpoints() > 0)
								{
									++cnt;
								}
							}
						}
					}
				}
				else
				{
					if (building.IsConstructing())
					{
						if (hitpointComponent != null)
						{
							if (!buildingData.IsWall())
							{
								if (includeDestructed)
								{
									++cnt;
								}
								else
								{
									if (building.GetHitpointComponent().GetHitpoints() > 0)
									{
										++cnt;
									}
								}
							}
						}
					}
				}
			}

			return cnt;
		}

		public int GetTombStoneCount()
		{
			LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.OBSTACLE);
			int cnt = 0;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				if (((LogicObstacle)gameObjects[i]).GetObstacleData().IsTombstone())
				{
					++cnt;
				}
			}

			return cnt;
		}

		public int GetTallGrassCount()
		{
			LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.OBSTACLE);
			int cnt = 0;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				if (((LogicObstacle)gameObjects[i]).GetObstacleData().IsTallGrass())
				{
					++cnt;
				}
			}

			return cnt;
		}

		public void DefenseStateEnded()
		{
			if (m_npcAttack != null)
			{
				m_npcAttack.Destruct();
				m_npcAttack = null;
			}

			SetVisitorAvatar(null);
		}

		public void DefenseStateStarted(LogicAvatar avatar)
		{
			SetVisitorAvatar(avatar);

			if (m_npcAttack != null)
			{
				m_npcAttack.Destruct();
				m_npcAttack = null;
			}

			m_npcAttack = new LogicNpcAttack(this);
			m_aliveBuildingCount = GetBuildingCount(false, true);
			m_destructibleBuildingCount = GetBuildingCount(true, false);

			if (m_battleLog != null)
			{
				m_battleLog.Destruct();
				m_battleLog = null;
			}

			m_battleLog = new LogicBattleLog(this);
			m_battleLog.CalculateAvailableResources(this, m_matchType);

			SetOwnerInformationToBattleLog();
		}

		public int GetUpdatedClockTowerBoostTime()
		{
			LogicBuilding clockTower = m_gameObjectManagers[1].GetClockTower();

			if (clockTower != null && !clockTower.IsConstructing())
			{
				return clockTower.GetRemainingBoostTime();
			}

			return 0;
		}

		public int GetUnplacedObjectCount(LogicData data)
		{
			if (m_unplacedObjects != null)
			{
				int cnt = 0;

				for (int i = 0; i < m_unplacedObjects.Size(); i++)
				{
					if (m_unplacedObjects[i].GetData() == data)
					{
						++cnt;
					}
				}

				return cnt;
			}

			return 0;
		}

		public int GetUnplacedObjectCount(LogicData data, int upgradeLevel)
		{
			if (m_unplacedObjects != null)
			{
				int cnt = 0;

				for (int i = 0; i < m_unplacedObjects.Size(); i++)
				{
					if (m_unplacedObjects[i].GetData() == data && m_unplacedObjects[i].GetCount() == upgradeLevel)
					{
						++cnt;
					}
				}

				return cnt;
			}

			return 0;
		}

		public bool RemoveUnplacedObject(LogicData data, int upgradeLevel)
		{
			if (m_unplacedObjects != null)
			{
				for (int i = 0; i < m_unplacedObjects.Size(); i++)
				{
					LogicDataSlot slot = m_unplacedObjects[i];

					if (slot.GetData() == data && slot.GetCount() == upgradeLevel)
					{
						slot.SetCount(slot.GetCount() - 1);
						m_unplacedObjects.Remove(i);

						return true;
					}
				}
			}

			return false;
		}

		public int GetObjectCount(LogicGameObjectData data, int villageType)
		{
			int cnt = 0;

			if (m_unplacedObjects != null)
			{
				for (int i = 0; i < m_unplacedObjects.Size(); i++)
				{
					if (m_unplacedObjects[i].GetData() == data)
					{
						++cnt;
					}
				}
			}

			return cnt + m_gameObjectManagers[data.GetVillageType()].GetGameObjectCountByData(data);
		}

		public LogicOfferManager GetOfferManager()
			=> m_offerManager;

		public int GetWidth()
			=> m_levelWidth;

		public int GetHeight()
			=> m_levelHeight;

		public int GetWidthInTiles()
			=> m_tileMap.GetSizeX();

		public int GetHeightInTiles()
			=> m_tileMap.GetSizeY();

		public int GetExperienceVersion()
			=> m_experienceVersion;

		public void SetExperienceVersion(int version)
		{
			m_experienceVersion = version;
		}

		public bool GetBattleEndPending()
			=> m_battleEndPending;

		public void EndBattle()
		{
			m_battleEndPending = true;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int villageType)
		{
			m_villageType = villageType;
			m_battleLog.SetVillageType(villageType);

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].ChangeVillageType(i == villageType);
			}
		}

		public void SetLoadingVillageType(int villageType)
		{
			m_loadingVillageType = villageType;
		}

		public string GetArmyName(int armyId)
			=> m_armyNames[armyId];

		public bool IsArrangedWar()
			=> m_arrangedWar;

		public void SetArrangedWar(bool enabled)
		{
			m_arrangedWar = enabled;
		}

		public bool IsArrangedWarBase()
			=> m_arrangedWarBase;

		public void SetArmyName(int armyId, string name)
		{
			if (name.Length > 16)
			{
				name = name.Substring(0, 16);
			}

			m_armyNames[armyId] = name;
		}

		public bool IsReadyForAttack()
			=> m_readyForAttack;

		public LogicBattleLog GetBattleLog()
			=> m_battleLog;

		public LogicTime GetLogicTime()
			=> m_time;

		public LogicRect GetPlayArea()
			=> m_playArea;

		public LogicAchievementManager GetAchievementManager()
			=> m_achievementManager;

		public LogicMissionManager GetMissionManager()
			=> m_missionManager;

		public LogicWorkerManager GetWorkerManager()
			=> m_workerManagers[m_villageType];

		public LogicWorkerManager GetWorkerManagerAt(int index)
			=> m_workerManagers[index];

		public LogicGameObjectManager GetGameObjectManager()
			=> m_gameObjectManagers[m_villageType];

		public LogicGameObjectManager GetGameObjectManagerAt(int index)
			=> m_gameObjectManagers[index];

		public LogicComponentManager GetComponentManager()
			=> m_gameObjectManagers[m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType].GetComponentManager();

		public LogicComponentManager GetComponentManagerAt(int villageType)
			=> m_gameObjectManagers[villageType].GetComponentManager();

		public LogicCooldownManager GetCooldownManager()
			=> m_cooldownManager;

		public LogicTileMap GetTileMap()
			=> m_tileMap;

		public LogicClientAvatar GetPlayerAvatar()
		{
			if (GetState() != 1 && GetState() != 3)
			{
				return (LogicClientAvatar)m_visitorAvatar;
			}

			return (LogicClientAvatar)m_homeOwnerAvatar;
		}

		public LogicAvatar GetHomeOwnerAvatar()
			=> m_homeOwnerAvatar;

		public LogicAvatarChangeListener GetHomeOwnerAvatarChangeListener()
			=> m_homeOwnerAvatar.GetChangeListener();

		public LogicLeagueData GetHomeLeagueData()
		{
			if (m_gameMode.GetState() == 1 &&
				m_homeOwnerAvatar.IsClientAvatar())
			{
				return ((LogicClientAvatar)m_homeOwnerAvatar).GetLeagueTypeData();
			}

			return m_leagueData;
		}

		public LogicAvatar GetVisitorAvatar()
			=> m_visitorAvatar;

		public LogicClientHome GetHome()
			=> m_home;

		public void UpdateLastUsedArmy()
		{
			LogicClientAvatar playerAvatar = GetPlayerAvatar();

			if (m_villageType == 0)
			{
				playerAvatar.SetLastUsedArmy(playerAvatar.GetUnits(), playerAvatar.GetSpells());
			}
		}

		public void SetHome(LogicClientHome home, bool androidClient)
		{
			m_offerManager.Init();

			m_home = home;
			m_levelJSON = (LogicJSONObject)LogicJSONParser.Parse(home.GetHomeJSON());

			LogicJSONBoolean androidClientBoolean = m_levelJSON.GetJSONBoolean("android_client");
			LogicJSONBoolean warBaseBoolean = m_levelJSON.GetJSONBoolean("war_base");

			if (warBaseBoolean != null)
			{
				m_warBase = warBaseBoolean.IsTrue();
			}

			LogicJSONBoolean arrangedWarBoolean = m_levelJSON.GetJSONBoolean("arr_war_base");

			if (arrangedWarBoolean != null)
			{
				m_arrangedWarBase = arrangedWarBoolean.IsTrue();
			}

			LogicJSONNumber activeLayoutNumber = m_levelJSON.GetJSONNumber("active_layout");

			if (activeLayoutNumber != null)
			{
				m_activeLayout = activeLayoutNumber.GetIntValue();
			}

			LogicJSONNumber activeLayout2Number = m_levelJSON.GetJSONNumber("act_l2");

			if (activeLayout2Number != null)
			{
				m_activeLayoutVillage2 = activeLayout2Number.GetIntValue();
			}

			if (m_activeLayout < 0)
			{
				m_activeLayout = 0;
			}

			if (m_activeLayoutVillage2 < 0)
			{
				m_activeLayoutVillage2 = 0;
			}

			LogicJSONNumber warLayoutNumber = m_levelJSON.GetJSONNumber("war_layout");

			if (warLayoutNumber != null)
			{
				m_warLayout = warLayoutNumber.GetIntValue();
			}
			else if (m_warBase)
			{
				m_warLayout = 1;
			}

			if (m_warLayout < 0)
			{
				m_warLayout = 0;
			}

			for (int i = 0; i < m_layoutState.Size(); i++)
			{
				m_layoutState[i] = 0;
			}

			LogicJSONArray layoutStateArray = m_levelJSON.GetJSONArray("layout_state");

			if (layoutStateArray != null)
			{
				int arraySize = layoutStateArray.Size();

				for (int i = 0; i < m_layoutState.Size(); i++)
				{
					if (i >= arraySize)
					{
						break;
					}

					LogicJSONNumber numObject = layoutStateArray.GetJSONNumber(i);

					if (numObject != null)
					{
						int num = numObject.GetIntValue();

						if (num > -1)
						{
							m_layoutState[i] = num;
						}
					}
				}
			}

			for (int i = 0; i < m_layoutStateVillage2.Size(); i++)
			{
				m_layoutStateVillage2[i] = 0;
			}

			LogicJSONArray layoutState2Array = m_levelJSON.GetJSONArray("layout_state2");

			if (layoutState2Array != null)
			{
				int arraySize = layoutState2Array.Size();

				for (int i = 0; i < m_layoutStateVillage2.Size(); i++)
				{
					if (i >= arraySize)
					{
						break;
					}

					LogicJSONNumber numObject = layoutState2Array.GetJSONNumber(i);

					if (numObject != null)
					{
						int num = numObject.GetIntValue();

						if (num > -1)
						{
							m_layoutStateVillage2[i] = num;
						}
					}
				}
			}

			for (int i = 0; i < m_layoutCooldown.Size(); i++)
			{
				m_layoutCooldown[i] = 0;
			}

			LogicJSONArray layoutCooldownArray = m_levelJSON.GetJSONArray("layout_cooldown");

			if (layoutCooldownArray != null)
			{
				int arraySize = layoutCooldownArray.Size();

				for (int i = 0; i < m_layoutCooldown.Size(); i++)
				{
					if (i >= arraySize)
					{
						break;
					}

					LogicJSONNumber numObject = layoutCooldownArray.GetJSONNumber(i);

					if (numObject != null)
					{
						int num = LogicMath.Min(numObject.GetIntValue(), 15 * LogicDataTables.GetGlobals().GetChallengeBaseSaveCooldown());

						if (num > -1)
						{
							m_layoutCooldown[i] = num;
						}
					}
				}
			}

			if (m_unplacedObjects != null)
			{
				m_unplacedObjects.Clear();
			}

			LogicJSONArray unplacedArray = m_levelJSON.GetJSONArray("unplaced");

			if (unplacedArray != null)
			{
				int arraySize = unplacedArray.Size();

				for (int i = 0; i < arraySize; i++)
				{
					LogicDataSlot dataSlot = new LogicDataSlot(null, 0);
					dataSlot.ReadFromJSON(unplacedArray.GetJSONObject(i));
					AddUnplacedObject(dataSlot);
				}
			}

			m_gameMode.GetCalendar().LoadProgress(m_levelJSON);

			if (androidClient)
			{
				m_androidClient = true;
			}
			else
			{
				if (androidClientBoolean != null)
				{
					m_androidClient = androidClientBoolean.IsTrue();
				}
			}

			LogicJSONNumber waveNumObject = m_levelJSON.GetJSONNumber("wave_num");

			if (waveNumObject != null)
			{
				if (GetState() != 1)
				{
					m_waveNumber = waveNumObject.GetIntValue();
				}
			}

			LogicJSONBoolean arrangedWarObject = m_levelJSON.GetJSONBoolean("arrWar");

			if (arrangedWarObject != null)
			{
				m_arrangedWar = arrangedWarObject.IsTrue();
			}

			LogicJSONBoolean warObject = m_levelJSON.GetJSONBoolean("war");

			if (warObject != null)
			{
				m_war = warObject.IsTrue();
			}
			else
			{
				m_war = false;
			}

			LogicJSONBoolean directObject = m_levelJSON.GetJSONBoolean("direct");
			LogicJSONBoolean direct2Object = m_levelJSON.GetJSONBoolean("direct2");

			bool notDirectLevel = directObject == null || !directObject.IsTrue();

			if (!(notDirectLevel || !m_warBase))
			{
				m_matchType = 7;
				m_revengeId = null;
			}
			else if (direct2Object != null && direct2Object.IsTrue())
			{
				m_matchType = 8;
				m_revengeId = null;
			}
			else if (m_warBase || !notDirectLevel)
			{
				m_matchType = (notDirectLevel ? 2 : 0) + 3;
				m_revengeId = null;
			}

			if (LogicDataTables.GetGlobals().LoadVillage2AsSnapshot() && (m_matchType & 0xFFFFFFFE) == 8)
			{
				m_loadingVillageType = 0;

				do
				{
					m_gameObjectManagers[m_loadingVillageType].LoadFromSnapshot(m_levelJSON);
				} while (++m_loadingVillageType < 2);

				m_loadingVillageType = -1;
			}
			else if (m_matchType == 3 || m_matchType == 5 || m_matchType == 7 ||
					 m_gameMode.GetVisitType() == 1 ||
					 m_gameMode.GetVisitType() == 2 ||
					 m_gameMode.GetVisitType() == 3 ||
					 m_gameMode.GetVisitType() == 4 ||
					 m_gameMode.GetVisitType() == 5)
			{
				m_gameObjectManagers[0].LoadFromSnapshot(m_levelJSON);

				if (m_matchType == 5 && LogicDataTables.GetGlobals().ReadyForWarAttackCheck())
				{
					m_readyForAttack = false;
				}
			}
			else
			{
				m_loadingVillageType = 0;

				do
				{
					m_gameObjectManagers[m_loadingVillageType].Load(m_levelJSON);
				} while (++m_loadingVillageType < 2);

				m_loadingVillageType = -1;
				m_cooldownManager.Load(m_levelJSON);
				m_offerManager.Load(m_levelJSON);
			}

			if (!m_npcVillage)
			{
				LogicJSONNumber expVerNumber = m_levelJSON.GetJSONNumber("exp_ver");

				if (expVerNumber != null)
				{
					m_experienceVersion = expVerNumber.GetIntValue();
				}
				else
				{
					m_experienceVersion = 0;
				}

				if (m_gameMode.GetState() != 5)
				{
					while (m_experienceVersion < LogicLevel.EXPERIENCE_VERSION)
					{
						UpdateExperienceVersion(m_experienceVersion);
					}
				}
			}

			m_aliveBuildingCount = GetBuildingCount(false, true);
			m_destructibleBuildingCount = GetBuildingCount(true, false);
		}

		public void SaveToJSON(LogicJSONObject jsonObject)
		{
			if (!m_npcVillage)
			{
				if (m_waveNumber > 0)
				{
					jsonObject.Put("wave_num", new LogicJSONNumber(m_waveNumber));
				}

				if (m_experienceVersion > 0)
				{
					jsonObject.Put("exp_ver", new LogicJSONNumber(m_experienceVersion));
				}

				if (m_androidClient)
				{
					jsonObject.Put("android_client", new LogicJSONBoolean(true));
				}

				if (m_matchType == 5 || m_matchType == 7)
				{
					jsonObject.Put("war", new LogicJSONBoolean(true));
				}

				if (m_matchType == 3 || m_matchType == 7)
				{
					jsonObject.Put("direct", new LogicJSONBoolean(true));
				}

				if (m_matchType == 8)
				{
					jsonObject.Put("direct2", new LogicJSONBoolean(true));
				}

				if (m_arrangedWar)
				{
					jsonObject.Put("arrWar", new LogicJSONBoolean(true));
				}

				jsonObject.Put("active_layout", new LogicJSONNumber(m_activeLayout));
				jsonObject.Put("act_l2", new LogicJSONNumber(m_activeLayoutVillage2));

				if (m_warBase)
				{
					if (LogicDataTables.GetGlobals().RevertBrokenWarLayouts())
					{
						/* if ( sub_1E436C(v22) != 1 )
						   {
							   this.m_warBase = false;
						   }
						*/
					}

					jsonObject.Put("war_layout", new LogicJSONNumber(m_warLayout));
				}

				LogicJSONArray layoutStateArray = new LogicJSONArray();

				for (int i = 0; i < m_layoutState.Size(); i++)
				{
					layoutStateArray.Add(new LogicJSONNumber(m_layoutState[i]));
				}

				jsonObject.Put("layout_state", layoutStateArray);

				LogicJSONArray layoutState2Array = new LogicJSONArray();

				for (int i = 0; i < m_layoutStateVillage2.Size(); i++)
				{
					layoutState2Array.Add(new LogicJSONNumber(m_layoutStateVillage2[i]));
				}

				jsonObject.Put("layout_state2", layoutState2Array);

				LogicJSONArray layoutCooldownArray = new LogicJSONArray();

				for (int i = 0; i < m_layoutCooldown.Size(); i++)
				{
					layoutCooldownArray.Add(new LogicJSONNumber(m_layoutCooldown[i]));
				}

				jsonObject.Put("layout_cooldown", layoutCooldownArray);
			}

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].Save(jsonObject);
			}

			if (!m_npcVillage)
			{
				m_cooldownManager.Save(jsonObject);
				SaveShopNewItems(jsonObject);

				jsonObject.Put("last_league_rank", new LogicJSONNumber(m_lastLeagueRank));
				jsonObject.Put("last_alliance_level", new LogicJSONNumber(m_lastAllianceLevel));
				jsonObject.Put("last_league_shuffle", new LogicJSONNumber(m_lastLeagueShuffle ? 1 : 0));
				jsonObject.Put("last_season_seen", new LogicJSONNumber(m_lastSeasonSeen));
				jsonObject.Put("last_news_seen", new LogicJSONNumber(m_lastSeenNews));

				if (m_troopRequestMessage.Length > 0)
				{
					jsonObject.Put("troop_req_msg", new LogicJSONString(m_troopRequestMessage));
				}

				if (m_warTroopRequestMessage.Length > 0)
				{
					jsonObject.Put("war_req_msg", new LogicJSONString(m_warTroopRequestMessage));
				}

				jsonObject.Put("war_tutorials_seen", new LogicJSONNumber(m_warTutorialsSeen));
				jsonObject.Put("war_base", new LogicJSONBoolean(m_warBase));
				jsonObject.Put("arr_war_base", new LogicJSONBoolean(m_arrangedWarBase));

				LogicJSONArray armyNameArray = new LogicJSONArray();

				for (int i = 0; i < m_armyNames.Size(); i++)
				{
					armyNameArray.Add(new LogicJSONString(m_armyNames[i]));
				}

				jsonObject.Put("army_names", armyNameArray);

				int accountFlags = 0;

				if (m_helpOpened)
				{
					accountFlags |= 1 << 0;
				}

				if (m_editModeShown)
				{
					accountFlags |= 1 << 1;
				}

				if (m_attackShieldCostOpened)
				{
					accountFlags |= 1 << 3;
				}

				jsonObject.Put("account_flags", new LogicJSONNumber(accountFlags));
				jsonObject.Put(GetPersistentBoolVariableName(0), new LogicJSONBoolean(m_layoutEditShownErase));

				if (m_unplacedObjects != null)
				{
					if (m_unplacedObjects.Size() > 0)
					{
						LogicJSONArray unplacedArray = new LogicJSONArray();

						for (int i = 0; i < m_unplacedObjects.Size(); i++)
						{
							LogicJSONObject obj = new LogicJSONObject();
							m_unplacedObjects[i].WriteToJSON(obj);
							unplacedArray.Add(obj);
						}

						jsonObject.Put("unplaced", unplacedArray);
					}
				}

				m_gameMode.GetCalendar().SaveProgress(jsonObject);
			}
		}

		private void SaveShopNewItems(LogicJSONObject jsonObject)
		{
			LogicDataTable buildingTable = LogicDataTables.GetTable(DataType.BUILDING);
			LogicDataTable trapTable = LogicDataTables.GetTable(DataType.TRAP);
			LogicDataTable decoTable = LogicDataTables.GetTable(DataType.DECO);

			int townHallLevelVillage2 = m_homeOwnerAvatar.GetVillage2TownHallLevel();
			int townHallLevel = m_homeOwnerAvatar.GetTownHallLevel();
			int expLevel = m_homeOwnerAvatar.GetExpLevel();

			LogicJSONArray newShopBuildingArray = new LogicJSONArray();

			for (int i = 0; i < m_newShopBuildings.Size(); i++)
			{
				LogicGameObjectData data = (LogicGameObjectData)buildingTable.GetItemAt(i);

				int currentNewItemCount = m_newShopBuildings[i];
				int unlockedShopItemCount = GetShopUnlockCount(data, data.GetVillageType() == 0 ? townHallLevel : townHallLevelVillage2);

				newShopBuildingArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
			}

			jsonObject.Put("newShopBuildings", newShopBuildingArray);

			LogicJSONArray newShopTrapArray = new LogicJSONArray();

			for (int i = 0; i < m_newShopTraps.Size(); i++)
			{
				LogicGameObjectData data = (LogicGameObjectData)trapTable.GetItemAt(i);

				int currentNewItemCount = m_newShopTraps[i];
				int unlockedShopItemCount = GetShopUnlockCount(data, data.GetVillageType() == 0 ? townHallLevel : townHallLevelVillage2);

				newShopTrapArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
			}

			jsonObject.Put("newShopTraps", newShopTrapArray);

			LogicJSONArray newShopDecoArray = new LogicJSONArray();

			for (int i = 0; i < m_newShopDecos.Size(); i++)
			{
				int currentNewItemCount = m_newShopDecos[i];
				int unlockedShopItemCount = GetShopUnlockCount(decoTable.GetItemAt(i), expLevel);

				newShopDecoArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
			}

			jsonObject.Put("newShopDecos", newShopDecoArray);
		}

		public void LoadShopNewItems()
		{
			if (m_levelJSON != null)
			{
				for (int i = 0; i < m_newShopBuildings.Size(); i++)
				{
					m_newShopBuildings[i] = 0;
				}

				for (int i = 0; i < m_newShopTraps.Size(); i++)
				{
					m_newShopTraps[i] = 0;
				}

				for (int i = 0; i < m_newShopDecos.Size(); i++)
				{
					m_newShopDecos[i] = 0;
				}

				LogicDataTable buildingTable = LogicDataTables.GetTable(DataType.BUILDING);
				LogicDataTable trapTable = LogicDataTables.GetTable(DataType.TRAP);
				LogicDataTable decoTable = LogicDataTables.GetTable(DataType.DECO);

				int townHallLevelVillage2 = m_homeOwnerAvatar.GetVillage2TownHallLevel();
				int townHallLevel = m_homeOwnerAvatar.GetTownHallLevel();
				int expLevel = m_homeOwnerAvatar.GetExpLevel();

				LogicJSONArray buildingArray = m_levelJSON.GetJSONArray("newShopBuildings");

				if (buildingArray != null)
				{
					for (int i = 0; i < m_newShopBuildings.Size(); i++)
					{
						LogicGameObjectData data = (LogicGameObjectData)buildingTable.GetItemAt(i);

						int unlockedShopItemCount = GetShopUnlockCount(data, data.GetVillageType() == 0 ? townHallLevel : townHallLevelVillage2);

						if (i < buildingArray.Size())
						{
							unlockedShopItemCount -= buildingArray.GetJSONNumber(i).GetIntValue();

							if (unlockedShopItemCount < 0)
							{
								unlockedShopItemCount = 0;
							}
						}

						m_newShopBuildings[i] = unlockedShopItemCount;
					}
				}

				LogicJSONArray trapArray = m_levelJSON.GetJSONArray("newShopTraps");

				if (trapArray != null)
				{
					for (int i = 0; i < m_newShopTraps.Size(); i++)
					{
						LogicGameObjectData data = (LogicGameObjectData)trapTable.GetItemAt(i);

						int unlockedShopItemCount = GetShopUnlockCount(data, data.GetVillageType() == 0 ? townHallLevel : townHallLevelVillage2);

						if (i < trapArray.Size())
						{
							unlockedShopItemCount -= trapArray.GetJSONNumber(i).GetIntValue();

							if (unlockedShopItemCount < 0)
							{
								unlockedShopItemCount = 0;
							}
						}

						m_newShopTraps[i] = unlockedShopItemCount;
					}
				}

				LogicJSONArray decoArray = m_levelJSON.GetJSONArray("newShopDecos");

				if (decoArray != null)
				{
					for (int i = 0; i < m_newShopDecos.Size(); i++)
					{
						int unlockedShopItemCount = GetShopUnlockCount(decoTable.GetItemAt(i), expLevel);

						if (i < decoArray.Size())
						{
							unlockedShopItemCount -= decoArray.GetJSONNumber(i).GetIntValue();

							if (unlockedShopItemCount < 0)
							{
								unlockedShopItemCount = 0;
							}
						}

						m_newShopDecos[i] = unlockedShopItemCount;
					}
				}
			}
		}

		public bool SetUnlockedShopItemCount(LogicGameObjectData data, int index, int count, int villageType)
		{
			if (data.GetVillageType() == villageType)
			{
				switch (data.GetDataType())
				{
					case DataType.BUILDING:
						m_newShopBuildings[index] = count;
						break;
					case DataType.TRAP:
						m_newShopTraps[index] = count;
						break;
					case DataType.DECO:
						m_newShopDecos[index] = count;
						break;
					default:
						return false;
				}

				return true;
			}

			return false;
		}

		public int GetLastAllianceLevel()
			=> m_lastAllianceLevel;

		public void SetLastAllianceLevel(int value)
		{
			m_lastAllianceLevel = value;
		}

		public void SetLastSeenNews(int lastSeenNews)
		{
			if (m_lastSeenNews < lastSeenNews)
			{
				m_lastSeenNews = lastSeenNews;
			}
		}

		public int GetLastSeenNews()
			=> m_lastSeenNews;

		public int GetLastSeasonSeen()
			=> m_lastSeasonSeen;

		public void SetLastSeasonSeen(int value)
		{
			m_lastSeasonSeen = value;
		}

		public int GetLastLeagueRank()
			=> m_lastLeagueRank;

		public void SetLastLeagueRank(int value)
		{
			m_lastLeagueRank = value;
		}

		public bool IsLastLeagueShuffle()
			=> m_lastLeagueShuffle;

		public void SetLastLeagueShuffle(bool state)
		{
			m_lastLeagueShuffle = state;
		}

		public void RefreshNewShopUnlocksExp()
		{
			int expLevel = m_homeOwnerAvatar.GetExpLevel();

			if (m_homeOwnerAvatar.GetExpLevel() > 0)
			{
				LogicDataTable table = LogicDataTables.GetTable(DataType.DECO);

				for (int i = 0; i < m_newShopDecos.Size(); i++)
				{
					LogicData data = table.GetItemAt(i);

					int totalShopUnlock = GetShopUnlockCount(data, expLevel);
					int shopUnlockCount = totalShopUnlock - GetShopUnlockCount(data, expLevel - 1);

					if (shopUnlockCount > 0)
					{
						m_newShopDecos[i] += shopUnlockCount;
					}
				}
			}
		}

		public void RefreshNewShopUnlocksTH(int villageType)
		{
			RefreshNewShopUnlocks(villageType);
		}

		public void RefreshNewShopUnlocks(int villageType)
		{
			LogicBuilding townHall = m_gameObjectManagers[villageType].GetTownHall();

			if (townHall != null)
			{
				int thUpgradeLevel = townHall.GetUpgradeLevel();

				if (thUpgradeLevel > 0)
				{
					LogicDataTable buildingTable = LogicDataTables.GetTable(DataType.BUILDING);

					for (int i = 0; i < m_newShopBuildings.Size(); i++)
					{
						LogicGameObjectData data = (LogicGameObjectData)buildingTable.GetItemAt(i);

						if (data.IsEnabledInVillageType(villageType))
						{
							int totalShopUnlock = GetShopUnlockCount(data, thUpgradeLevel);
							int previousTotalShopUnlock = GetShopUnlockCount(data, thUpgradeLevel - 1);

							if (totalShopUnlock > previousTotalShopUnlock)
								m_newShopBuildings[i] += totalShopUnlock - previousTotalShopUnlock;
						}
					}

					LogicDataTable trapTable = LogicDataTables.GetTable(DataType.TRAP);

					for (int i = 0; i < m_newShopTraps.Size(); i++)
					{
						LogicGameObjectData data = (LogicGameObjectData)trapTable.GetItemAt(i);

						if (data.IsEnabledInVillageType(villageType))
						{
							int totalShopUnlock = GetShopUnlockCount(data, thUpgradeLevel);
							int previousTotalShopUnlock = GetShopUnlockCount(data, thUpgradeLevel - 1);

							if (totalShopUnlock > previousTotalShopUnlock)
								m_newShopTraps[i] += totalShopUnlock - previousTotalShopUnlock;
						}
					}
				}
			}
		}

		public void RefreshResourceCaps()
		{
			if (m_homeOwnerAvatar != null && m_homeOwnerAvatar.IsClientAvatar())
			{
				LogicClientAvatar clientAvatar = (LogicClientAvatar)m_homeOwnerAvatar;
				LogicDataTable table = LogicDataTables.GetTable(DataType.RESOURCE);

				for (int i = 0, cnt = 0; i < table.GetItemCount(); i++, cnt = 0)
				{
					LogicResourceData data = (LogicResourceData)table.GetItemAt(i);

					for (int j = 0; j < 2; j++)
					{
						if (data.GetWarResourceReferenceData() != null)
						{
							LogicArrayList<LogicComponent> components =
								m_gameObjectManagers[j].GetComponentManager().GetComponents(LogicComponentType.WAR_RESOURCE_STORAGE);

							for (int k = 0; k < components.Size(); k++)
							{
								LogicWarResourceStorageComponent resourceWarStorageComponent = (LogicWarResourceStorageComponent)components[k];

								if (resourceWarStorageComponent.IsEnabled())
								{
									cnt += resourceWarStorageComponent.GetMax(i);
								}
							}
						}
						else
						{
							LogicArrayList<LogicComponent> components =
								m_gameObjectManagers[j].GetComponentManager().GetComponents(LogicComponentType.RESOURCE_STORAGE);

							for (int k = 0; k < components.Size(); k++)
							{
								LogicResourceStorageComponent resourceStorageComponent = (LogicResourceStorageComponent)components[k];

								if (resourceStorageComponent.IsEnabled())
								{
									cnt += resourceStorageComponent.GetMax(i);
								}
							}
						}
					}

					if (!data.IsPremiumCurrency())
					{
						clientAvatar.SetResourceCap(data, cnt);
						clientAvatar.GetChangeListener().CommodityCountChanged(1, data, cnt);
					}
				}
			}
		}

		public void SetHomeOwnerAvatar(LogicAvatar avatar)
		{
			m_homeOwnerAvatar = avatar;

			if (avatar != null)
			{
				avatar.SetLevel(this);

				if (avatar.IsClientAvatar())
				{
					m_lastLeagueRank = ((LogicClientAvatar)avatar).GetLeagueType();
				}

				if (m_battleLog != null)
				{
					if (avatar.GetName() != null)
					{
						m_battleLog.SetDefenderName(avatar.GetName());
					}
				}

				m_gameObjectManagers[0].GetComponentManager().DivideAvatarUnitsToStorages(0);
				m_gameObjectManagers[1].GetComponentManager().DivideAvatarUnitsToStorages(1);

				if (m_matchType == 5)
				{
					throw new NotImplementedException();
				}
				else
				{
					m_gameObjectManagers[0].GetComponentManager().AddAvatarAllianceUnitsToCastle();
				}
			}
		}

		public void SetVisitorAvatar(LogicAvatar avatar)
		{
			if (m_visitorAvatar != avatar && m_visitorAvatar != null)
			{
				m_visitorAvatar.Destruct();
				m_visitorAvatar = null;
			}

			m_visitorAvatar = avatar;

			if (avatar != null)
			{
				avatar.SetLevel(this);

				if (m_battleLog != null && avatar.IsClientAvatar())
				{
					LogicClientAvatar clientAvatar = (LogicClientAvatar)m_visitorAvatar;

					m_visitorLeagueData = clientAvatar.GetLeagueTypeData();
					m_battleLog.SetAttackerStars(clientAvatar.GetStarBonusCounter());
					m_battleLog.SetAttackerHomeId(clientAvatar.GetCurrentHomeId());
					m_battleLog.SetAttackerName(clientAvatar.GetName());

					if (avatar.IsInAlliance())
					{
						m_battleLog.SetAttackerAllianceId(clientAvatar.GetAllianceId());
						m_battleLog.SetAttackerAllianceBadge(clientAvatar.GetAllianceBadgeId());
						m_battleLog.SetAttackerAllianceLevel(clientAvatar.GetAllianceLevel());

						string allianceName = clientAvatar.GetAllianceName();

						if (allianceName != null)
						{
							m_battleLog.SetAttackerAllianceName(allianceName);
						}
					}
					else
					{
						m_battleLog.SetAttackerAllianceBadge(-1);
					}
				}
			}
		}

		public void SetOwnerInformationToBattleLog()
		{
			if (m_homeOwnerAvatar.IsClientAvatar())
			{
				LogicClientAvatar clientAvatar = (LogicClientAvatar)m_homeOwnerAvatar;

				if (clientAvatar.IsInAlliance())
				{
					m_battleLog.SetDefenderAllianceId(clientAvatar.GetAllianceId());

					string allianceName = clientAvatar.GetAllianceName();

					if (allianceName != null)
					{
						m_battleLog.SetDefenderAllianceName(allianceName);
					}

					m_battleLog.SetDefenderAllianceBadge(clientAvatar.GetAllianceBadgeId());
					m_battleLog.SetDefenderAllianceLevel(clientAvatar.GetAllianceLevel());
				}
				else
				{
					m_battleLog.SetDefenderAllianceBadge(-1);
				}

				m_battleLog.SetDefenderHomeId(clientAvatar.GetCurrentHomeId());
			}
		}

		public void SetPersistentBool(int idx, bool value)
		{
			switch (idx)
			{
				case 0:
					m_layoutEditShownErase = value;
					break;
				default:
					Debugger.Warning("setPersistentBool() index out of bounds");
					break;
			}
		}

		public string GetPersistentBoolVariableName(int idx)
		{
			switch (idx)
			{
				case 0:
					return "bool_layout_edit_shown_erase";
				default:
					Debugger.Error("Boolean index out of bounds");
					return null;
			}
		}

		public void AddUnplacedObject(LogicDataSlot obj)
		{
			if (m_unplacedObjects == null)
			{
				m_unplacedObjects = new LogicArrayList<LogicDataSlot>();
			}

			m_unplacedObjects.Add(obj);
		}

		public int GetShopUnlockCount(LogicData data, int arg)
		{
			int unlock = 0;

			switch (data.GetDataType())
			{
				case DataType.BUILDING:
					LogicBuildingData buildingData = (LogicBuildingData)data;

					if (!buildingData.IsLocked())
					{
						unlock = LogicDataTables.GetTownHallLevel(arg).GetUnlockedBuildingCount(buildingData) - buildingData.GetStartingHomeCount();

						if (unlock < 0)
							unlock = 0;
					}

					break;
				case DataType.TRAP:
					unlock = LogicDataTables.GetTownHallLevel(arg).GetUnlockedTrapCount((LogicTrapData)data);
					break;
				case DataType.DECO:
					LogicDecoData decoData = (LogicDecoData)data;

					if (decoData.GetRequiredExpLevel() <= arg && decoData.IsInShop())
					{
						unlock = decoData.GetMaxCount();
					}

					break;
			}

			return unlock;
		}

		public void BattleStarted()
		{
			m_battleStarted = true;

			if (m_matchType == 4 && !LogicDataTables.GetGlobals().RemoveRevengeWhenBattleIsLoaded())
			{
				GetPlayerAvatar().GetChangeListener().RevengeUsed(m_revengeId);
			}

			if (GetState() != 5)
			{
				if (m_matchType <= 8)
				{
					if (m_matchType == 1 ||
						m_matchType == 3 ||
						m_matchType == 4 ||
						m_matchType == 7)
					{
						m_readyForAttack = true;

						m_homeOwnerAvatar.GetChangeListener().AttackStarted();
						GetPlayerAvatar().GetChangeListener().AttackStarted();
					}
					else if (m_matchType == 5 ||
							 m_matchType == 8)
					{
						LogicClientAvatar playerAvatar = GetPlayerAvatar();
						LogicAvatarChangeListener listener = playerAvatar.GetChangeListener();

						if (listener != null)
						{
							listener.BattleFeedback(4, 0);
						}
					}
				}
			}
		}

		public bool IsClockTowerBoostPaused()
		{
			LogicBuilding clockTower = m_gameObjectManagers[1].GetClockTower();

			if (clockTower != null)
			{
				return clockTower.IsBoostPaused();
			}

			return false;
		}

		public bool IsInCombatState()
		{
			int state = GetState();
			return state == 2 || state == 3 || state == 5;
		}

		public bool IsWarBase()
			=> m_warBase;

		public void SetWarBase(bool enabled)
		{
			m_warBase = enabled;
		}

		public bool IsNpcVillage()
			=> m_npcVillage;

		public void SetNpcVillage(bool enabled)
		{
			m_npcVillage = enabled;
		}

		public bool IsBuildingGearUpCapReached(LogicBuildingData data, bool canCallListener)
		{
			int townHallLevel = 0;

			if (m_gameObjectManagers[m_villageType].GetTownHall() != null)
			{
				townHallLevel = m_gameObjectManagers[m_villageType].GetTownHall().GetUpgradeLevel();
			}

			int unlockedGearupCount = LogicDataTables.GetTownHallLevel(townHallLevel).GetUnlockedBuildingGearupCount(data);
			int gearupCount = m_gameObjectManagers[m_villageType].GetGearUpBuildingCount(data);

			if (unlockedGearupCount <= gearupCount)
			{
				if (canCallListener)
				{
					m_gameListener.BuildingCapReached(data);
				}

				return true;
			}

			return false;
		}

		public bool IsBuildingCapReached(LogicBuildingData data, bool canCallListener)
		{
			int townHallLevel = 0;

			if (m_gameObjectManagers[m_villageType].GetTownHall() != null)
			{
				townHallLevel = m_gameObjectManagers[m_villageType].GetTownHall().GetUpgradeLevel();
			}

			bool reached = m_gameObjectManagers[m_villageType].GetGameObjectCountByData(data) >=
						   LogicDataTables.GetTownHallLevel(townHallLevel).GetUnlockedBuildingCount(data);

			if (!reached && canCallListener)
			{
				m_gameListener.BuildingCapReached(data);
			}

			return reached;
		}

		public bool IsTrapCapReached(LogicTrapData data, bool canCallListener)
		{
			int townHallLevel = 0;

			if (m_gameObjectManagers[m_villageType].GetTownHall() != null)
			{
				townHallLevel = m_gameObjectManagers[m_villageType].GetTownHall().GetUpgradeLevel();
			}

			bool reached = m_gameObjectManagers[m_villageType].GetGameObjectCountByData(data) >=
						   LogicDataTables.GetTownHallLevel(townHallLevel).GetUnlockedTrapCount(data);

			if (!reached && canCallListener)
			{
				m_gameListener.TrapCapReached(data);
			}

			return reached;
		}

		public bool IsDecoCapReached(LogicDecoData data, bool canCallListener)
		{
			int townHallLevel = 0;

			if (m_gameObjectManagers[m_villageType].GetTownHall() != null)
			{
				townHallLevel = m_gameObjectManagers[m_villageType].GetTownHall().GetUpgradeLevel();
			}

			bool reached = true;

			if (m_homeOwnerAvatar.GetExpLevel() >= data.GetRequiredExpLevel())
			{
				reached = m_gameObjectManagers[m_villageType].GetGameObjectCountByData(data) >=
						  data.GetMaxCount();
			}

			if (!reached && canCallListener)
			{
				m_gameListener.DecoCapReached(data);
			}

			return reached;
		}

		public bool IsValidPlaceForBuilding(int x, int y, int width, int height, LogicGameObject gameObject)
		{
			if (m_playArea.IsInside(x, y) && m_playArea.IsInside(x + width, y + height))
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						if (!m_tileMap.GetTile(x + i, y + j).IsBuildable(gameObject))
						{
							return false;
						}
					}
				}

				return true;
			}

			return false;
		}

		public bool IsValidPlaceForObstacle(int x, int y, int width, int height, bool edge, bool ignoreTallGrass)
		{
			if (x >= 0 && y >= 0)
			{
				if (width + x <= LogicLevel.TILEMAP_SIZE_X && height + y <= LogicLevel.TILEMAP_SIZE_Y)
				{
					if (edge)
					{
						x -= 1;
						y -= 1;
						width += 2;
						height += 2;
					}

					for (int i = 0; i < width; i++)
					{
						for (int j = 0; j < height; j++)
						{
							LogicTile tile = m_tileMap.GetTile(x + i, y + j);

							if (tile != null)
							{
								if (!ignoreTallGrass)
								{
									if (tile.GetTallGrass() != null)
									{
										return false;
									}
								}

								if (!tile.IsBuildable(null))
								{
									return false;
								}
							}
						}
					}

					return true;
				}
			}

			return false;
		}

		public bool IsValidPlaceForBuildingWithIgnoreList(int x, int y, int width, int height, LogicGameObject[] gameObjects, int count)
		{
			if (m_playArea.IsInside(x, y))
			{
				if (m_playArea.IsInside(x + width, y + height))
				{
					for (int i = 0; i < width; i++)
					{
						for (int j = 0; j < height; j++)
						{
							if (!m_tileMap.GetTile(x + i, y + j).IsBuildableWithIgnoreList(gameObjects, count))
							{
								return false;
							}
						}
					}

					return true;
				}
			}

			return false;
		}

		public bool IsAttackerHeroPlaced(LogicHeroData data)
		{
			if (m_placedHeroData != null)
			{
				for (int i = 0; i < m_placedHeroData.Size(); i++)
				{
					if (m_placedHeroData[i] == data)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void SetAttackerHeroPlaced(LogicHeroData data, LogicCharacter hero)
		{
			if (m_placedHeroData == null)
			{
				m_placedHeroData = new LogicArrayList<LogicHeroData>();
				m_placedHero = new LogicArrayList<LogicCharacter>();
			}

			int index = m_placedHeroData.IndexOf(data);

			if (index == -1)
			{
				m_placedHero.Add(hero);
				m_placedHeroData.Add(data);
			}
			else
			{
				Debugger.Warning("setAttackerHeroPlaced called twice for same hero");
			}
		}

		public int GetTotalAttackerHeroPlaced()
		{
			if (m_placedHeroData != null)
			{
				return m_placedHeroData.Size();
			}

			return 0;
		}

		public bool IsUnitsTrainedVillage2()
		{
			LogicArrayList<LogicComponent> components = m_gameObjectManagers[1].GetComponentManager().GetComponents(LogicComponentType.VILLAGE2_UNIT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicVillage2UnitComponent component = (LogicVillage2UnitComponent)components[i];

				if (component.IsEnabled())
				{
					if (component.GetUnitData() == null || component.GetUnitCount() == 0 || component.GetUnitCount() < component.GetMaxUnitsInCamp(component.GetUnitData()))
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool GetAreaShield(int midX, int midY, LogicVector2 output)
		{
			LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.SPELL);

			int speed = 0x7FFFFFFF;
			int damage = 100;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicSpell spell = (LogicSpell)gameObjects[i];

				if (spell.IsDeployed() && !spell.GetHitsCompleted())
				{
					LogicSpellData spellData = spell.GetSpellData();

					if (spellData.GetShieldProjectileSpeed(spell.GetUpgradeLevel()) != 0)
					{
						int radius = spellData.GetRadius(spellData.GetRadius(spell.GetUpgradeLevel()));
						int distanceX = spell.GetMidX() - midX;
						int distanceY = spell.GetMidY() - midY;

						if (LogicMath.Abs(distanceX) <= radius && LogicMath.Abs(distanceY) <= radius)
						{
							if (distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
							{
								speed = LogicMath.Min(speed, spellData.GetShieldProjectileSpeed(spell.GetUpgradeLevel()));
								damage = 100 - spellData.GetShieldProjectileDamageMod(spell.GetUpgradeLevel());
							}
						}
					}
				}
			}

			if (m_placedHero != null)
			{
				for (int i = 0; i < m_placedHero.Size(); i++)
				{
					LogicCharacter hero = m_placedHero[i];

					if (hero.GetAbilityTime() > 0 && hero.IsAlive())
					{
						LogicHeroData heroData = m_placedHeroData[i];

						int upgLevel = hero.GetUpgradeLevel();
						int abilityShieldProjectileSpeed = heroData.GetAbilityShieldProjectileSpeed(upgLevel);

						if (abilityShieldProjectileSpeed != 0)
						{
							int radius = heroData.GetAbilityRadius();
							int distanceX = hero.GetMidX() - midX;
							int distanceY = hero.GetMidY() - midY;

							if (LogicMath.Abs(distanceX) <= radius && LogicMath.Abs(distanceY) <= radius)
							{
								if (abilityShieldProjectileSpeed < speed && distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
								{
									speed = abilityShieldProjectileSpeed;
									damage = 100 - heroData.GetAbilityShieldProjectileDamageMod(upgLevel);
								}
							}
						}
					}
				}
			}

			output.m_x = speed;
			output.m_y = damage;

			return speed != 0x7FFFFFFF;
		}

		public void AreaPoison(int gameObjectId, int x, int y, int radius, int damage, LogicData preferredTarget, int preferredTargetDamagePecent, LogicEffectData hitEffect,
							   int team, LogicVector2 unk2,
							   int targetType, int damageType, int unk3, bool healing, int heroDamageMultiplier, bool increaseSlowly)
		{
			int villageType = m_loadingVillageType;

			if (villageType < 0)
			{
				villageType = m_villageType;
			}

			LogicArrayList<LogicComponent> components = GetComponentManagerAt(villageType).GetComponents(LogicComponentType.HITPOINT);
			LogicVector2 pushBackPosition = new LogicVector2();

			for (int i = 0; i < components.Size(); i++)
			{
				LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)components[i];
				LogicGameObject parent = hitpointComponent.GetParent();

				if (!parent.IsHidden() && hitpointComponent.GetHitpoints() > 0)
				{
					if (hitpointComponent.GetTeam() == team)
					{
						if (damage > 0 || damage < 0 && parent.IsPreventsHealing())
						{
							continue;
						}
					}
					else if (damage < 0)
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

					if (distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
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

						int totalDamage = LogicCombatComponent.IsPreferredTarget(preferredTarget, parent) ? damage * preferredTargetDamagePecent / 100 : damage;

						if (parent.IsHero())
						{
							totalDamage = (totalDamage < 0 ? LogicDataTables.GetGlobals().GetHeroHealMultiplier() : heroDamageMultiplier) * damage / 100;
						}

						hitpointComponent.SetPoisonDamage(totalDamage, increaseSlowly);

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

		public void AreaDamage(int gameObjectId, int x, int y, int radius, int damage, LogicData preferredTarget, int preferredTargetDamagePecent, LogicEffectData hitEffect,
							   int team, LogicVector2 unk2,
							   int targetType, int damageType, int unk3, bool gravity, bool healing, int heroDamageMultiplier, int maxUnitsHit, LogicGameObject gameObject,
							   int damageTHPercent, int pauseCombatComponentsMs)
		{
			int villageType = m_loadingVillageType;

			if (villageType < 0)
			{
				villageType = m_villageType;
			}

			LogicArrayList<LogicComponent> components = GetComponentManagerAt(villageType).GetComponents(LogicComponentType.HITPOINT);
			LogicVector2 pushBackPosition = new LogicVector2();

			int freezeTimeMS = pauseCombatComponentsMs >> 6;
			int maxHits = maxUnitsHit > 0 ? maxUnitsHit : 0x7FFFFFFF;

			for (int i = 0, j = 0; i < components.Size() && j < maxHits; i++)
			{
				LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)components[i];
				LogicGameObject parent = hitpointComponent.GetParent();

				if (!parent.IsHidden() && hitpointComponent.GetHitpoints() > 0)
				{
					if (hitpointComponent.GetTeam() == team)
					{
						if (damage > 0 || damage < 0 && parent.IsPreventsHealing())
						{
							continue;
						}
					}
					else if (damage < 0)
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

						int totalDamage = LogicCombatComponent.IsPreferredTarget(preferredTarget, parent) ? damage * preferredTargetDamagePecent / 100 : damage;

						if (parent.GetGameObjectType() == LogicGameObjectType.BUILDING && parent.GetData() == LogicDataTables.GetTownHallData())
							totalDamage = damageTHPercent * totalDamage / 100;
						if (parent.IsHero())
							totalDamage = (totalDamage < 0 ? LogicDataTables.GetGlobals().GetHeroHealMultiplier() : heroDamageMultiplier) * damage / 100;

						hitpointComponent.CauseDamage(totalDamage, gameObjectId, gameObject);

						if (pauseCombatComponentsMs > 0)
						{
							if (parent.GetCombatComponent() != null)
								parent.Freeze(freezeTimeMS, 0);
							if (parent.GetMovementComponent() != null)
								parent.Freeze(freezeTimeMS, 0);
						}

						if (healing)
						{
							// Listener.
						}

						if ((distanceX | distanceY) == 0)
						{
							distanceX = 1;

							if (unk2 != null && (unk2.m_x | unk2.m_y) != 0)
							{
								distanceX = -unk2.m_x;
								distanceY = -unk2.m_y;
							}
						}

						++j;

						pushBackPosition.m_x = -distanceX;
						pushBackPosition.m_y = -distanceY;

						pushBackPosition.Normalize(512);

						if (unk3 > 0 && movementComponent != null && !m_invulnerabilityEnabled)
						{
							movementComponent.GetMovementSystem().PushBack(pushBackPosition, damage, unk3, 0, false, gravity);

							if (parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
							{
								LogicBuilding building = (LogicBuilding)parent;

								if (building.GetCombatComponent() != null && building.GetCombatComponent().GetAttackerItemData().IsSelfAsAoeCenter())
								{
									// Listener.
								}
							}
						}
					}
				}
			}
		}

		public void AreaFreeze(int x, int y, int radius, int time, int team)
		{
			for (int i = 0; i < LogicGameObject.GAMEOBJECT_TYPE_COUNT; i++)
			{
				LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects((LogicGameObjectType)i);

				for (int j = 0; j < gameObjects.Size(); j++)
				{
					LogicGameObject gameObject = gameObjects[j];
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent == null || combatComponent.GetUndergroundTime() <= 0)
					{
						LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

						if (hitpointComponent == null || hitpointComponent.GetTeam() != team)
						{
							int distanceX = x - gameObject.GetMidX();
							int distanceY = y - gameObject.GetMidY();

							if (LogicMath.Abs(distanceX) <= radius &&
								LogicMath.Abs(distanceY) <= radius &&
								distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
							{
								int freezeTime = LogicMath.Max(0, time / 4 - (int)(((distanceX * distanceX + distanceY * distanceY) / 250000u) & 0xFFFE));
								int freezeDelay = (int)((distanceX * distanceX + distanceY * distanceY) / 2000000u);

								gameObject.Freeze(freezeTime, freezeDelay);
							}
						}
					}
				}
			}
		}

		public void AreaShield(int x, int y, int radius, int time, int team)
		{
			LogicArrayList<LogicComponent> components = m_gameObjectManagers[m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType]
															.GetComponentManager()
															.GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();
				LogicCombatComponent combatComponent = parent.GetCombatComponent();

				if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == team)
				{
					int distanceX = x - parent.GetMidX();
					int distanceY = y - parent.GetMidY();

					if (LogicMath.Abs(distanceX) <= radius &&
						LogicMath.Abs(distanceY) <= radius &&
						distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
					{
						hitpointComponent.SetInvulnerabilityTime(time);
						parent.SetPreventsHealingTime(0);

						if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
						{
							LogicCharacter character = (LogicCharacter)parent;
							LogicArrayList<LogicCharacter> childrens = character.GetChildTroops();

							if (childrens != null)
							{
								for (int j = 0; j < childrens.Size(); j++)
								{
									LogicGameObject children = childrens[j];

									children.GetHitpointComponent().SetInvulnerabilityTime(time);
									children.SetPreventsHealingTime(0);
								}
							}
						}
					}
				}
			}
		}

		public void AreaShrink(int x, int y, int radius, int speedBoostRatio, int hpRatio, int time, int team)
		{
			for (int i = 0; i < LogicGameObject.GAMEOBJECT_TYPE_COUNT; i++)
			{
				LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects((LogicGameObjectType)i);

				for (int j = 0; j < gameObjects.Size(); j++)
				{
					LogicGameObject gameObject = gameObjects[j];
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (combatComponent == null || combatComponent.GetUndergroundTime() <= 0)
					{
						LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

						if (hitpointComponent == null || hitpointComponent.GetTeam() != team)
						{
							int distanceX = x - gameObject.GetMidX();
							int distanceY = y - gameObject.GetMidY();

							if (LogicMath.Abs(distanceX) <= radius &&
								LogicMath.Abs(distanceY) <= radius &&
								distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
							{
								gameObject.Shrink(time, speedBoostRatio);

								if (hitpointComponent != null && hitpointComponent.GetOriginalHitpoints() == hitpointComponent.GetMaxHitpoints())
								{
									hitpointComponent.SetShrinkHitpoints(time, hpRatio);
								}
							}
						}
					}
				}
			}
		}

		public void AreaInvisibility(int x, int y, int radius, int time, int team)
		{
			for (int i = 0; i < LogicGameObject.GAMEOBJECT_TYPE_COUNT; i++)
			{
				LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects((LogicGameObjectType)i);

				for (int j = 0; j < gameObjects.Size(); j++)
				{
					LogicGameObject gameObject = gameObjects[j];
					LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();
					LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

					if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == team)
					{
						int distanceX = x - gameObject.GetMidX();
						int distanceY = y - gameObject.GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius &&
							distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
						{
							combatComponent.SetUndergroundTime(time / 4);
						}
					}
				}
			}
		}

		public void BoostGameObject(LogicGameObject gameObject, int speedBoost, int speedBoost2, int damageBoostPercentage, int attackSpeedBoost, int boostTime,
									bool boostLinkedToPoison)
		{
			LogicMovementComponent movementComponent = gameObject.GetMovementComponent();
			LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();
			LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

			if (hitpointComponent != null && combatComponent != null)
			{
				int totalDamageBoostPercentage = damageBoostPercentage;

				if (boostLinkedToPoison)
				{
					int rmMS = hitpointComponent.GetPoisonRemainingMS();

					if (60 * rmMS / 1000 >= boostTime)
					{
						boostTime = 60 * rmMS / 1000;
					}
				}

				if (gameObject.IsHero())
				{
					totalDamageBoostPercentage = totalDamageBoostPercentage * LogicDataTables.GetGlobals().GetHeroRageMultiplier() / 100;
				}

				combatComponent.Boost(totalDamageBoostPercentage, attackSpeedBoost, boostTime / 4);

				if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)gameObject;
					LogicArrayList<LogicCharacter> childrens = character.GetChildTroops();

					if (childrens != null)
					{
						for (int i = 0; i < childrens.Size(); i++)
						{
							childrens[i].GetCombatComponent().Boost(damageBoostPercentage, attackSpeedBoost, boostTime / 4);
						}
					}
				}

				if (gameObject.GetData().GetDataType() == DataType.CHARACTER && ((LogicCharacter)gameObject).GetAttackerItemData().GetPreferredTargetData() != null)
				{
					if (movementComponent != null)
					{
						movementComponent.GetMovementSystem().Boost(speedBoost2, boostTime);
					}
				}
				else
				{
					if (gameObject.IsHero())
					{
						speedBoost = (int)(speedBoost * LogicDataTables.GetGlobals().GetHeroRageSpeedMultiplier() / 100L);
					}

					if (movementComponent != null)
					{
						movementComponent.GetMovementSystem().Boost(speedBoost, boostTime);
					}
				}
			}
		}

		public void AreaBoost(int x, int y, int radius, int speedBoost, int speedBoost2, int damageBoostPercentage, int attackSpeedBoost, int damageTime, int team,
							  bool boostLinkedToPoison)
		{
			LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
															.GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();
				LogicCombatComponent combatComponent = parent.GetCombatComponent();

				if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == team && hitpointComponent.GetHitpoints() > 0)
				{
					int distanceX = x - parent.GetMidX();
					int distanceY = y - parent.GetMidY();

					if (LogicMath.Abs(distanceX) <= radius &&
						LogicMath.Abs(distanceY) <= radius &&
						distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
					{
						BoostGameObject(parent, speedBoost, speedBoost2, damageBoostPercentage, attackSpeedBoost, damageTime, boostLinkedToPoison);
					}
				}
			}
		}

		public void AreaBoost(int x, int y, int radius, int damageBoostPercentage, int attackSpeedBoost, int damageTime)
		{
			LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
															.GetComponents(LogicComponentType.COMBAT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicCombatComponent combatComponent = (LogicCombatComponent)components[i];
				LogicGameObject parent = combatComponent.GetParent();

				if (parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
				{
					LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();

					if (hitpointComponent != null && hitpointComponent.GetHitpoints() > 0)
					{
						int distanceX = x - parent.GetMidX();
						int distanceY = y - parent.GetMidY();

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius &&
							distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
						{
							combatComponent.Boost(damageBoostPercentage, attackSpeedBoost, damageTime);
						}
					}
				}
			}
		}

		public void AreaAbilityBoost(LogicCharacter hero, int time)
		{
			if (hero.IsHero())
			{
				LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
																.GetComponents(LogicComponentType.MOVEMENT);
				LogicHeroData data = (LogicHeroData)hero.GetData();

				int upgLevel = hero.GetUpgradeLevel();
				int x = hero.GetX();
				int y = hero.GetY();

				LogicCharacterData abilityAffectsCharacter = data.GetAbilityAffectsCharacter();
				LogicCharacter abilityAffectsHero = data.GetAbilityAffectsHero() ? hero : null;
				LogicCharacterData summonTroop = data.GetAbilitySummonTroop();

				int speedBoost = data.GetAbilitySpeedBoost(upgLevel);
				int speedBoost2 = data.GetAbilitySpeedBoost2(upgLevel);
				int damageBoostPercent = data.GetAbilityDamageBoostPercent(upgLevel);
				int damageBoost = data.GetAbilityDamageBoost(upgLevel);
				int summonTroopCount = data.GetAbilitySummonTroopCount(upgLevel);

				int radius = data.GetAbilityRadius();
				int speedTime = time * 4;

				for (int i = 0; i < components.Size(); i++)
				{
					LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
					LogicGameObject parent = movementComponent.GetParent();
					LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();
					LogicCombatComponent combatComponent = parent.GetCombatComponent();

					if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == 0 && hitpointComponent.GetHitpoints() > 0 &&
						parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)parent;
						LogicCharacterData characterData = character.GetCharacterData();

						if (abilityAffectsCharacter == null || abilityAffectsHero == null || abilityAffectsCharacter == characterData || abilityAffectsHero == parent)
						{
							if (abilityAffectsHero != null || !character.IsHero())
							{
								if (abilityAffectsCharacter != null || characterData.GetDataType() == DataType.HERO)
								{
									if (characterData == data || !character.IsHero())
									{
										int distanceX = x - character.GetMidX();
										int distanceY = y - character.GetMidY();

										if (LogicMath.Abs(distanceX) <= radius &&
											LogicMath.Abs(distanceY) <= radius &&
											distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
										{
											if (speedBoost > 0 && speedBoost2 > 0 && damageBoostPercent > 0)
											{
												if (character.IsHero())
												{
													if (damageBoost != 0)
													{
														combatComponent.Boost(damageBoost, 0, time);
													}
												}
												else
												{
													combatComponent.Boost(damageBoostPercent, 0, time);
												}

												if (characterData.GetDataType() == DataType.CHARACTER &&
													character.GetAttackerItemData().GetPreferredTargetData() != null)
												{
													movementComponent.GetMovementSystem().Boost(speedBoost2, speedTime);
												}
												else
												{
													movementComponent.GetMovementSystem().Boost(speedBoost, speedTime);
												}
											}
											else if (data.GetAbilityStealth())
											{
												if (character.IsHero() && damageBoost != 0)
												{
													combatComponent.Boost(damageBoost, 0, time);
												}

												character.SetStealthTime(time);
											}

											if (summonTroopCount > 0)
											{
												if (character.IsHero())
												{
													character.SpawnEvent(summonTroop, summonTroopCount, m_visitorAvatar.GetUnitUpgradeLevel(summonTroop));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void AreaBoostAlone(LogicCharacter character, int time)
		{
			LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
															.GetComponents(LogicComponentType.MOVEMENT);
			LogicCharacterData data = character.GetCharacterData();

			int x = character.GetX();
			int y = character.GetY();
			int boostRadius = data.GetBoostRadius();
			int boostDamagePerfect = data.GetBoostDamagePerfect();
			int boostAttackSpeed = data.GetBoostAttackSpeed();

			if (data.GetSpecialAbilityType() == LogicCharacterData.SPECIAL_ABILITY_TYPE_RAGE_ALONE && character.GetSpecialAbilityAvailable())
			{
				boostDamagePerfect = data.GetSpecialAbilityAttribute(character.GetUpgradeLevel());
				boostAttackSpeed = data.GetSpecialAbilityAttribute2(character.GetUpgradeLevel());
			}

			int team = character.GetHitpointComponent() != null ? character.GetHitpointComponent().GetTeam() : -1;
			int boostRadiusSquared = boostRadius * boostRadius;
			bool flying = character.IsFlying();
			bool isAlone = true;

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();
				LogicCombatComponent combatComponent = parent.GetCombatComponent();

				if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == team && hitpointComponent.GetHitpoints() > 0 &&
					parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					if (parent != character && !(flying ^ parent.IsFlying()))
					{
						int distanceX = x - parent.GetMidX();
						int distanceY = y - parent.GetMidY();

						if (distanceX * distanceX + distanceY * distanceY < boostRadiusSquared)
						{
							isAlone = false;
						}
					}
				}
			}

			if (isAlone)
			{
				LogicCombatComponent combatComponent = character.GetCombatComponent();

				if (combatComponent != null)
				{
					combatComponent.Boost(boostDamagePerfect, boostAttackSpeed, time);
				}
			}
		}

		public void AreaJump(int x, int y, int radius, int time, int housingSpaceLimit, int team)
		{
			LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
															.GetComponents(LogicComponentType.MOVEMENT);

			for (int i = 0; i < components.Size(); i++)
			{
				LogicMovementComponent movementComponent = (LogicMovementComponent)components[i];
				LogicGameObject parent = movementComponent.GetParent();
				LogicHitpointComponent hitpointComponent = parent.GetHitpointComponent();
				LogicCombatComponent combatComponent = parent.GetCombatComponent();

				if (hitpointComponent != null && combatComponent != null && hitpointComponent.GetTeam() == team && hitpointComponent.GetHitpoints() > 0)
				{
					if (parent.GetGameObjectType() == LogicGameObjectType.CHARACTER)
					{
						LogicCharacter character = (LogicCharacter)parent;
						LogicCharacterData data = character.GetCharacterData();

						if (data.GetHousingSpace() > housingSpaceLimit || data.IsJumper())
						{
							continue;
						}
					}

					int distanceX = x - parent.GetMidX();
					int distanceY = y - parent.GetMidY();

					if (LogicMath.Abs(distanceX) <= radius &&
						LogicMath.Abs(distanceY) <= radius &&
						distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
					{
						movementComponent.EnableJump(time);
					}
				}
			}
		}

		public void AreaPushBack(int x, int y, int radius, int time, int team, int targetType, int pushBackX, int pushBackY, int distance, int housingSpaceLimit)
		{
			LogicArrayList<LogicComponent> components = GetComponentManagerAt(m_loadingVillageType < 0 ? m_villageType : m_loadingVillageType)
															.GetComponents(LogicComponentType.HITPOINT);
			LogicVector2 pushBackPosition = new LogicVector2();

			for (int i = 0; i < components.Size(); i++)
			{
				LogicHitpointComponent hitpointComponent = (LogicHitpointComponent)components[i];
				LogicGameObject parent = hitpointComponent.GetParent();

				if (!parent.IsHidden() && hitpointComponent.GetHitpoints() != 0 && hitpointComponent.GetTeam() != team)
				{
					LogicCombatItemData data = (LogicCombatItemData)parent.GetData();

					if (housingSpaceLimit >= data.GetHousingSpace())
					{
						LogicMovementComponent movementComponent = parent.GetMovementComponent();

						int posX;
						int posY;

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

							posX = parent.GetMidX();
							posY = parent.GetMidY();
						}
						else
						{
							posX = LogicMath.Clamp(x, parent.GetX(), parent.GetX() + (parent.GetWidthInTiles() << 9));
							posY = LogicMath.Clamp(y, parent.GetY(), parent.GetY() + (parent.GetHeightInTiles() << 9));
						}

						int distanceX = x - posX;
						int distanceY = y - posY;

						if (LogicMath.Abs(distanceX) <= radius &&
							LogicMath.Abs(distanceY) <= radius &&
							distanceX * distanceX + distanceY * distanceY < (uint)(radius * radius))
						{
							if (time > 0 && movementComponent != null && !m_invulnerabilityEnabled)
							{
								pushBackPosition.m_x = (int)((2 * distance * pushBackX) & 0xFFFFFE00) / 100;
								pushBackPosition.m_y = (int)((2 * distance * pushBackY) & 0xFFFFFE00) / 100;

								movementComponent.GetMovementSystem().PushTrap(pushBackPosition, 1000, 0, true, true);
								housingSpaceLimit -= data.GetHousingSpace();
							}
						}
					}
				}
			}
		}

		public bool HasFreeWorkers(LogicCommand command, int villageType)
		{
			if (villageType == -1)
			{
				villageType = m_villageType;
			}

			bool hasFreeWorker = m_workerManagers[villageType].GetFreeWorkers() > 0;

			if (!hasFreeWorker)
			{
				m_gameListener.NotEnoughWorkers(command, villageType);
			}

			return hasFreeWorker;
		}

		public void LoadingFinished()
		{
			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].GetComponentManager().DivideAvatarResourcesToStorages();
			}

			RefreshResourceCaps();

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].GetComponentManager().CalculateLoot(true);
			}

			if (m_battleLog != null)
			{
				m_battleLog.CalculateAvailableResources(this, m_matchType);
				SetOwnerInformationToBattleLog();
			}

			if (m_gameMode.GetState() == 2)
			{
				if (m_matchType == 1)
				{
					m_visitorAvatar.CommodityCountChangeHelper(0, LogicDataTables.GetGlobals().GetAttackResource(),
																	-LogicDataTables.GetTownHallLevel(m_visitorAvatar.GetTownHallLevel()).GetAttackCost());
				}
				else if (m_matchType == 8)
				{
					m_visitorAvatar.CommodityCountChangeHelper(0, LogicDataTables.GetGlobals().GetAttackResource(),
																	-LogicDataTables.GetTownHallLevel(m_visitorAvatar.GetTownHallLevel()).GetAttackCostVillage2());
				}
			}

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].LoadingFinished();
			}

			m_missionManager.LoadingFinished();
			LoadShopNewItems();

			if (m_levelJSON != null)
			{
				m_lastLeagueRank = 0;
				m_lastAllianceLevel = 1;

				LogicJSONNumber accountFlagObject = m_levelJSON.GetJSONNumber("account_flags");

				if (accountFlagObject != null)
				{
					int value = accountFlagObject.GetIntValue();

					m_helpOpened = (value & 1) != 0;
					m_editModeShown = ((value >> 1) & 1) != 0;
					m_attackShieldCostOpened = ((value >> 3) & 1) != 0;
				}

				LogicJSONNumber lastLeagueRankObject = m_levelJSON.GetJSONNumber("last_league_rank");

				if (lastLeagueRankObject != null)
				{
					m_lastLeagueRank = lastLeagueRankObject.GetIntValue();
				}

				LogicJSONNumber lastAllianceLevelObject = m_levelJSON.GetJSONNumber("last_alliance_level");

				if (lastAllianceLevelObject != null)
				{
					m_lastAllianceLevel = lastAllianceLevelObject.GetIntValue();
				}

				LogicJSONNumber lastLeagueShuffleObject = m_levelJSON.GetJSONNumber("last_league_shuffle");

				if (lastLeagueShuffleObject != null)
				{
					m_lastLeagueShuffle = lastLeagueShuffleObject.GetIntValue() != 0;
				}

				LogicJSONNumber lastSeasonSeenNumber = m_levelJSON.GetJSONNumber("last_season_seen");

				if (lastSeasonSeenNumber != null)
				{
					m_lastSeasonSeen = lastSeasonSeenNumber.GetIntValue();
				}

				LogicJSONNumber lastNewsSeenNumber = m_levelJSON.GetJSONNumber("last_news_seen");

				if (lastNewsSeenNumber != null)
				{
					m_lastSeenNews = lastNewsSeenNumber.GetIntValue();
				}

				LogicJSONBoolean editModeShown = m_levelJSON.GetJSONBoolean("edit_mode_shown");

				if (editModeShown != null)
				{
					m_editModeShown = editModeShown.IsTrue();
				}

				LogicJSONString troopRequestObject = m_levelJSON.GetJSONString("troop_req_msg");

				if (troopRequestObject != null)
				{
					m_troopRequestMessage = troopRequestObject.GetStringValue();
				}

				LogicJSONString warRequestObject = m_levelJSON.GetJSONString("war_req_msg");

				if (warRequestObject != null)
				{
					m_warTroopRequestMessage = warRequestObject.GetStringValue();
				}

				LogicJSONNumber warTutorialsSeenNumber = m_levelJSON.GetJSONNumber("war_tutorials_seen");

				if (warTutorialsSeenNumber != null)
				{
					m_warTutorialsSeen = warTutorialsSeenNumber.GetIntValue();
				}

				LogicJSONArray armyNameArray = m_levelJSON.GetJSONArray("army_names");

				if (armyNameArray != null)
				{
					int size = LogicMath.Min(armyNameArray.Size(), m_armyNames.Size());

					for (int i = 0; i < size; i++)
					{
						m_armyNames[i] = armyNameArray.GetJSONString(i).GetStringValue();
					}
				}

				LogicJSONBoolean helpOpenedBoolean = m_levelJSON.GetJSONBoolean("help_opened");

				if (helpOpenedBoolean != null)
				{
					m_helpOpened = helpOpenedBoolean.IsTrue();
				}

				LogicJSONBoolean layoutEditShownEraseBoolean = m_levelJSON.GetJSONBoolean(GetPersistentBoolVariableName(0));

				if (layoutEditShownEraseBoolean != null)
				{
					m_layoutEditShownErase = layoutEditShownEraseBoolean.IsTrue();
				}
			}

			m_achievementManager.RefreshStatus();

			if (LogicDataTables.GetGlobals().ValidateTroopUpgradeLevels())
			{
				for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
				{
					m_gameObjectManagers[i].GetComponentManager().ValidateTroopUpgradeLevels();
				}
			}

			if (m_gameMode.GetState() == 2 && m_matchType == 4 && LogicDataTables.GetGlobals().RemoveRevengeWhenBattleIsLoaded())
			{
				GetPlayerAvatar().GetChangeListener().RevengeUsed(m_revengeId);
			}

			m_gameMode.GetCalendar().UpdateUseTroopEvent(m_homeOwnerAvatar, this);
			m_levelJSON = null;
		}

		public void LoadVillageObjects()
		{
			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].LoadVillageObjects();
			}
		}

		public void FastForwardTime(int totalSecs)
		{
			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].FastForwardTime(totalSecs);
			}

			m_offerManager.FastForward(totalSecs);
			m_cooldownManager.FastForwardTime(totalSecs);

			for (int i = 0; i < m_layoutCooldown.Size(); i++)
			{
				if (m_layoutCooldown[i] > 0)
				{
					m_layoutCooldown[i] = LogicMath.Max(0, m_layoutCooldown[i] - 15 * totalSecs);
				}
			}
		}

		public void SubTick()
		{
			int clockTowerBoostTime = 0;

			if (m_gameObjectManagers[1].GetClockTower() != null)
			{
				LogicBuilding clockTower = m_gameObjectManagers[1].GetClockTower();

				if (!clockTower.IsBoostPaused())
				{
					if (!clockTower.IsConstructing())
					{
						clockTowerBoostTime = clockTower.GetRemainingBoostTime();
					}
				}
			}

			m_remainingClockTowerBoostTime = clockTowerBoostTime;

			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				m_gameObjectManagers[i].SubTick();
			}
		}

		public void Tick()
		{
			int state = GetState();

			if (state == 2 && !m_battleStarted && m_battleLog.GetBattleStarted())
			{
				BattleStarted();
			}

			if (state <= 1)
			{
				for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
				{
					m_gameObjectManagers[i].Tick();
				}
			}
			else
			{
				m_gameObjectManagers[m_villageType].Tick();
			}

			m_missionManager.Tick();
			m_achievementManager.Tick();
			m_offerManager.Tick();

			if (m_npcAttack != null)
			{
				m_npcAttack.Tick();
			}

			m_cooldownManager.Tick();
			UpdateBattleShieldStatus(true);

			for (int i = 0; i < m_layoutCooldown.Size(); i++)
			{
				int cooldown = m_layoutCooldown[i];

				if (cooldown > 0)
				{
					m_layoutCooldown[i] = cooldown - 1;
				}
			}
		}

		public void UpdateExperienceVersion(int prevVersion)
		{
			if (prevVersion == 0)
			{
				LogicGameObjectManager gameObjectManager = m_gameObjectManagers[0];

				for (int i = 0, j = gameObjectManager.GetNumGameObjects(); i < j; i++)
				{
					LogicGameObject gameObject = gameObjectManager.GetGameObjectByIndex(i);

					int width = gameObject.GetWidthInTiles();
					int height = gameObject.GetHeightInTiles();

					for (int k = 0; k < 8; k++)
					{
						for (int l = 0; l < 2; l++)
						{
							int x = 0;
							int y = 0;

							if (l != 0 && k == m_activeLayout)
							{
								LogicVector2 pos = gameObject.GetPosition();

								if ((pos.m_x & pos.m_y) >> 9 == -1)
								{
									continue;
								}

								x = pos.m_x >> 9;
								y = pos.m_y >> 9;
							}
							else
							{
								if (gameObject.GetLayoutComponent() == null)
								{
									continue;
								}

								LogicVector2 pos = gameObject.GetPositionLayout(k, l == 0);

								x = pos.m_x;
								y = pos.m_y;

								if ((x & y) == -1)
								{
									continue;
								}
							}

							int updatedX = x + 3;
							int updatedY = y + 3;

							if (gameObject.GetGameObjectType() == LogicGameObjectType.OBSTACLE)
							{
								int corrX = 0;
								int corrY = 0;

								if (x < 3)
								{
									corrX -= 3;
								}

								if (y < 3)
								{
									corrY -= 3;
								}

								if (x + width > 44)
								{
									corrX += 3;
								}

								if (y + height > 44)
								{
									corrY += 3;
								}

								updatedX += corrX;
								updatedY += corrY;
							}

							if (l != 0 && m_activeLayout == k)
							{
								gameObject.SetPositionXY(updatedX << 9, updatedY << 9);
							}
							else
							{
								gameObject.SetPositionLayoutXY(updatedX, updatedY, k, l == 0);
							}
						}
					}
				}

				m_experienceVersion = 1;
			}
		}

		public void UpdateBattleShieldStatus(bool unk)
		{
			if (m_homeOwnerAvatar.IsClientAvatar())
			{
				if (m_gameMode.GetState() != 5)
				{
					if (m_matchType > 7 || m_matchType != 3 && m_matchType != 5 && m_matchType != 7)
					{
						LogicGlobals globals = LogicDataTables.GetGlobals();

						int destructionPercentage = m_battleLog.GetDestructionPercentage();
						int shieldHours = 0;

						if (destructionPercentage >= globals.GetShieldTriggerPercentageHousingSpace())
						{
							shieldHours = globals.GetDestructionToShield(destructionPercentage);
						}

						if (shieldHours > 0 && !unk)
						{
							LogicClientAvatar homeOwnerAvatar = (LogicClientAvatar)m_homeOwnerAvatar;
							LogicLeagueData leagueData = homeOwnerAvatar.GetLeagueTypeData();

							if (leagueData == null)
							{
								leagueData = (LogicLeagueData)LogicDataTables.GetTable(DataType.LEAGUE).GetItemAt(0);
							}

							int villageGuardMins = leagueData.GetVillageGuardInMins();

							if (homeOwnerAvatar.GetAttackShieldReduceCounter() != 0)
							{
								homeOwnerAvatar.SetAttackShieldReduceCounter(0);
								homeOwnerAvatar.GetChangeListener().AttackShieldReduceCounterChanged(0);
							}

							if (homeOwnerAvatar.GetDefenseVillageGuardCounter() != 0)
							{
								homeOwnerAvatar.SetDefenseVillageGuardCounter(0);
								homeOwnerAvatar.GetChangeListener().DefenseVillageGuardCounterChanged(0);
							}

							m_home.GetChangeListener().ShieldActivated(shieldHours * 3600, villageGuardMins * 60);
						}

						if (shieldHours > m_shieldActivatedHours)
						{
							m_gameListener.ShieldActivated(shieldHours);
							m_shieldActivatedHours = shieldHours;
						}
					}
				}
			}
		}

		public int GetDefenseShieldActivatedHours()
			=> m_shieldActivatedHours;

		public void UpdateBattleStatus()
		{
			int state = m_gameMode.GetState();
			Debugger.DoAssert(state == 2 || state == 3 || state == 5, "updateBattleStatus in non combat state.");
			int aliveBuildingCount = GetBuildingCount(false, true);

			if (state == 2 || state == 5)
			{
				if (aliveBuildingCount < m_aliveBuildingCount)
				{
					m_battleLog.SetDestructionPercentage(100 - 100 * aliveBuildingCount / m_destructibleBuildingCount);
				}
			}

			m_aliveBuildingCount = aliveBuildingCount;

			LogicArrayList<LogicGameObject> gameObjects = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.CHARACTER);

			bool battleWaitForDieDamage = m_gameMode.GetConfiguration().GetBattleWaitForDieDamage();
			int damageCharacterCount = 0;

			for (int i = 0; i < gameObjects.Size(); i++)
			{
				LogicCharacter character = (LogicCharacter)gameObjects[i];
				LogicHitpointComponent hitpointComponent = character.GetHitpointComponent();

				if (hitpointComponent != null && hitpointComponent.GetTeam() == 0)
				{
					LogicAttackerItemData data = character.GetAttackerItemData();

					if (data.GetDamage(0, false) > 0 && (hitpointComponent.GetHitpoints() > 0 || battleWaitForDieDamage && character.GetWaitDieDamage()))
					{
						damageCharacterCount += 1;
					}
				}
			}

			int availableUnitCount = 0;
			bool containsSpells = false;
			bool containsAlliancePortals = false;

			if (m_villageType == 1)
			{
				availableUnitCount = m_visitorAvatar.GetUnitsTotalVillage2();

				LogicDataTable dataTable = LogicDataTables.GetTable(DataType.HERO);

				for (int i = 0; i < dataTable.GetItemCount(); i++)
				{
					LogicHeroData data = (LogicHeroData)dataTable.GetItemAt(i);

					if (data.GetVillageType() == m_villageType && m_visitorAvatar.GetHeroState(data) != 0)
					{
						if (m_placedHeroData == null || m_placedHeroData.IndexOf(data) == -1)
						{
							++availableUnitCount;
						}
					}
				}
			}
			else
			{
				availableUnitCount = m_visitorAvatar.GetUnitsTotal() + m_visitorAvatar.GetDamagingSpellsTotal();

				if (LogicDataTables.GetGlobals().FixClanPortalBattleNotEnding() && m_gameObjectManagers[0].GetAlliancePortal() != null)
				{
					availableUnitCount += m_gameObjectManagers[0].GetAlliancePortal().GetBunkerComponent().GetUsedCapacity();
				}
				else
				{
					availableUnitCount += m_visitorAvatar.GetAllianceCastleUsedCapacity();
				}

				LogicDataTable heroTable = LogicDataTables.GetTable(DataType.HERO);

				for (int i = 0; i < heroTable.GetItemCount(); i++)
				{
					LogicHeroData data = (LogicHeroData)heroTable.GetItemAt(i);

					if (data.GetVillageType() == m_villageType && m_visitorAvatar.GetHeroState(data) != 0)
					{
						if (m_placedHeroData == null || m_placedHeroData.IndexOf(data) == -1)
						{
							++availableUnitCount;
						}
					}
				}

				LogicArrayList<LogicGameObject> spells = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.SPELL);

				for (int i = 0; i < spells.Size(); i++)
				{
					LogicSpell spell = (LogicSpell)spells[i];

					if (!spell.GetHitsCompleted() && (spell.GetSpellData().IsDamageSpell() || spell.GetSpellData().GetSummonTroop() != null))
					{
						containsSpells = true;
					}
				}

				LogicArrayList<LogicGameObject> alliancePortals = m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.ALLIANCE_PORTAL);

				for (int i = 0; i < alliancePortals.Size(); i++)
				{
					LogicAlliancePortal alliancePortal = (LogicAlliancePortal)alliancePortals[i];

					if (alliancePortal.GetBunkerComponent().GetTeam() == 0 && !alliancePortal.GetBunkerComponent().IsEmpty())
					{
						containsAlliancePortals = true;
					}
				}
			}

			if ((m_matchType == 5 || m_matchType == 8) && m_gameMode.GetState() != 5)
			{
				UpdateBattleFeedback();
			}

			if (aliveBuildingCount == 0 || !(containsSpells | containsAlliancePortals) && (damageCharacterCount | availableUnitCount) == 0 &&
				(m_gameObjectManagers[m_villageType].GetGameObjects(LogicGameObjectType.PROJECTILE).Size() == 0 ||
				 !m_gameMode.GetConfiguration().GetBattleWaitForProjectileDestruction()) && m_matchType != 6)
			{
				m_battleEndPending = true;
			}
		}

		public void UpdateBattleFeedback()
		{
			LogicAvatarChangeListener avatarChangeListener = GetPlayerAvatar().GetChangeListener();

			if (avatarChangeListener != null)
			{
				if (!m_feedbackTownHallDestroyed)
				{
					if (m_battleLog.GetTownHallDestroyed())
					{
						m_feedbackTownHallDestroyed = true;
						avatarChangeListener.BattleFeedback(3, m_battleLog.GetStars());
					}
				}

				if (!m_feedbackDestruction25)
				{
					if (m_battleLog.GetDestructionPercentage() >= 25)
					{
						m_feedbackDestruction25 = true;
						avatarChangeListener.BattleFeedback(0, m_battleLog.GetStars());
					}
				}

				if (!m_feedbackDestruction50)
				{
					if (m_battleLog.GetDestructionPercentage() >= 50)
					{
						m_feedbackDestruction50 = true;
						avatarChangeListener.BattleFeedback(1, m_battleLog.GetStars());
					}
				}

				if (!m_feedbackDestruction75)
				{
					if (m_battleLog.GetDestructionPercentage() >= 75)
					{
						m_feedbackDestruction75 = true;
						avatarChangeListener.BattleFeedback(2, m_battleLog.GetStars());
					}
				}
			}
		}

		public void ReengageLootCart(int secs)
		{
			if (GetState() == 1)
			{
				LogicGlobals globals = LogicDataTables.GetGlobals();

				if (globals.GetLootCartReengagementMinSeconds() < secs)
				{
					if (globals.GetLootCartReengagementMaxSeconds() <= secs)
					{
						secs = globals.GetLootCartReengagementMaxSeconds();
					}

					int interval = globals.GetLootCartReengagementMaxSeconds() - globals.GetLootCartReengagementMinSeconds();
					int time = 100 * (secs - globals.GetLootCartReengagementMinSeconds()) / interval;

					if (time > 0)
					{
						if (m_homeOwnerAvatar != null)
						{
							if (m_homeOwnerAvatar.GetTownHallLevel() > 0)
							{
								LogicGameObjectManager gameObjectManager = m_gameObjectManagers[0];

								if (gameObjectManager.GetLootCart() == null)
								{
									gameObjectManager.AddLootCart();
								}

								gameObjectManager.GetLootCart().ReengageLootCart(time);
							}
						}
					}
				}
			}
		}

		public void DebugResetWarTutorials()
		{
			m_warTutorialsSeen = 0;
			m_missionManager.DebugResetWarTutorials();
		}

		public void Destruct()
		{
			for (int i = 0; i < LogicLevel.VILLAGE_COUNT; i++)
			{
				if (m_gameObjectManagers[i] != null)
				{
					m_gameObjectManagers[i].Destruct();
					m_gameObjectManagers[i] = null;
				}

				if (m_workerManagers[i] != null)
				{
					m_workerManagers[i].Destruct();
					m_workerManagers[i] = null;
				}
			}

			if (m_placedHero != null)
			{
				m_placedHero.Destruct();
				m_placedHero = null;
				m_placedHeroData = null;
			}

			if (m_tileMap != null)
			{
				m_tileMap.Destruct();
				m_tileMap = null;
			}

			/*if (m_playArea != null)
			{
				m_playArea.Destruct();
				m_playArea = null;
			}*/

			if (m_offerManager != null)
			{
				m_offerManager.Destruct();
				m_offerManager = null;
			}

			if (m_achievementManager != null)
			{
				m_achievementManager.Destruct();
				m_achievementManager = null;
			}

			if (m_cooldownManager != null)
			{
				m_cooldownManager.Destruct();
				m_cooldownManager = null;
			}

			if (m_missionManager != null)
			{
				m_missionManager.Destruct();
				m_missionManager = null;
			}

			if (m_battleLog != null)
			{
				m_battleLog.Destruct();
				m_battleLog = null;
			}

			if (m_gameListener != null)
			{
				m_gameListener.Destruct();
				m_gameListener = null;
			}

			if (m_newShopBuildings != null)
			{
				m_newShopBuildings.Destruct();
				m_newShopBuildings = null;
			}

			if (m_newShopTraps != null)
			{
				m_newShopTraps.Destruct();
				m_newShopTraps = null;
			}

			if (m_newShopDecos != null)
			{
				m_newShopDecos.Destruct();
				m_newShopDecos = null;
			}

			m_layoutState.Destruct();
			m_layoutStateVillage2.Destruct();
			m_layoutCooldown.Destruct();
			m_armyNames.Destruct();

			if (m_unplacedObjects != null)
			{
				for (int i = m_unplacedObjects.Size() - 1; i >= 0; i--)
				{
					m_unplacedObjects[i].Destruct();
					m_unplacedObjects.Remove(i);
				}

				m_unplacedObjects = null;
			}

			if (m_homeOwnerAvatar != null)
			{
				m_homeOwnerAvatar.SetLevel(null);
				m_homeOwnerAvatar = null;
			}

			m_levelJSON = null;
			m_gameMode = null;
			m_home = null;
			m_visitorAvatar = null;
			m_revengeId = null;
		}
	}
}