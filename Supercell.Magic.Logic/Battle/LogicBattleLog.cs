using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Battle
{
	public class LogicBattleLog
	{
		private LogicLevel m_level;
		private LogicLong m_attackerHomeId;
		private LogicLong m_defenderHomeId;
		private LogicLong m_attackerAllianceId;
		private LogicLong m_defenderAllianceId;

		private readonly LogicArrayList<LogicDataSlot> m_lootCount;
		private readonly LogicArrayList<LogicDataSlot> m_availableLootCount;
		private readonly LogicArrayList<LogicDataSlot> m_castedSpellCount;
		private readonly LogicArrayList<LogicDataSlot> m_castedUnitCount;
		private readonly LogicArrayList<LogicUnitSlot> m_castedAllianceUnitCount;
		private readonly LogicArrayList<LogicDataSlot> m_unitLevelCount;
		private readonly LogicArrayList<LogicDataSlot> m_costCount;

		private int m_battleTime;
		private int m_villageType;
		private int m_attackerStars;
		private int m_attackerScore;
		private int m_defenderScore;
		private int m_deployedHousingSpace;
		private int m_destructionPercentage;
		private int m_originalAttackerScore;
		private int m_originalDefenderScore;
		private int m_attackerAllianceBadgeId;
		private int m_defenderAllianceBadgeId;
		private int m_armyDeploymentPercentage;
		private int m_lootMultiplierByTownHallDiff;
		private int m_attackerAllianceLevel;
		private int m_defenderAllianceLevel;

		private string m_attackerAllianceName;
		private string m_defenderAllianceName;
		private string m_attackerName;
		private string m_defenderName;

		private bool m_battleEnded;
		private bool m_townhallDestroyed;
		private bool m_allianceUsed;

		public LogicBattleLog(LogicLevel level)
		{
			m_level = level;

			m_attackerHomeId = new LogicLong();
			m_defenderHomeId = new LogicLong();

			m_costCount = new LogicArrayList<LogicDataSlot>();
			m_lootCount = new LogicArrayList<LogicDataSlot>();
			m_availableLootCount = new LogicArrayList<LogicDataSlot>();
			m_unitLevelCount = new LogicArrayList<LogicDataSlot>();
			m_castedSpellCount = new LogicArrayList<LogicDataSlot>();
			m_castedUnitCount = new LogicArrayList<LogicDataSlot>();
			m_castedAllianceUnitCount = new LogicArrayList<LogicUnitSlot>();
		}

		public void Destruct()
		{
			m_level = null;
			m_attackerAllianceName = null;
			m_defenderAllianceName = null;
			m_attackerName = null;
			m_defenderName = null;
			m_attackerHomeId = null;
			m_defenderHomeId = null;
			m_attackerAllianceId = null;
			m_defenderAllianceId = null;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int value)
		{
			m_villageType = value;
		}

		public int GetStars()
			=> (m_destructionPercentage >= 50 ? 1 : 0) + (m_destructionPercentage == 100 ? 1 : 0) + (m_townhallDestroyed ? 1 : 0);

		public bool GetTownHallDestroyed()
			=> m_townhallDestroyed;

		public void SetTownHallDestroyed(bool destroyed)
		{
			m_townhallDestroyed = destroyed;
		}

		public int GetStolenResources(LogicResourceData data)
		{
			for (int i = 0; i < m_lootCount.Size(); i++)
			{
				if (m_lootCount[i].GetData() == data)
				{
					return m_lootCount[i].GetCount();
				}
			}

			return 0;
		}

		public void IncreaseStolenResourceCount(LogicResourceData data, int count)
		{
			if (m_level != null)
			{
				m_level.GetAchievementManager().IncreaseLoot(data, count);
			}

			int idx = -1;

			for (int i = 0; i < m_lootCount.Size(); i++)
			{
				if (m_lootCount[i].GetData() == data)
				{
					idx = i;
					break;
				}
			}

			if (idx != -1)
			{
				m_lootCount[idx].SetCount(m_lootCount[idx].GetCount() + count);
			}
			else
			{
				m_lootCount.Add(new LogicDataSlot(data, count));
			}
		}

		public LogicArrayList<LogicDataSlot> GetCastedSpells()
			=> m_castedSpellCount;

		public LogicArrayList<LogicDataSlot> GetCastedUnits()
			=> m_castedUnitCount;

		public LogicArrayList<LogicUnitSlot> GetCastedAllianceUnits()
			=> m_castedAllianceUnitCount;

		public int GetCostCount(LogicData data)
		{
			for (int i = 0; i < m_costCount.Size(); i++)
			{
				if (m_costCount[i].GetData() == data)
				{
					return m_costCount[i].GetCount();
				}
			}

			return 0;
		}

		public bool GetBattleStarted()
		{
			if (!m_battleEnded)
			{
				int matchType = m_level.GetMatchType();

				if (matchType < 10)
				{
					if (matchType == 3 || matchType == 7 || matchType == 8 || matchType == 9)
					{
						return true;
					}
				}

				return m_castedUnitCount.Size() +
					   m_castedSpellCount.Size() +
					   m_castedAllianceUnitCount.Size() > 0;
			}

			return true;
		}

		public bool HasDeployedUnits()
			=> m_castedUnitCount.Size() +
				   m_castedSpellCount.Size() +
				   m_castedAllianceUnitCount.Size() > 0;

		public bool GetBattleEnded()
			=> m_battleEnded;

		public void SetBattleEnded(int battleTime)
		{
			m_battleTime = battleTime;
			m_battleEnded = true;
		}

		public void SetAttackerHomeId(LogicLong homeId)
		{
			m_attackerHomeId = homeId;
		}

		public LogicLong GetDefenderHomeId()
			=> m_defenderHomeId;

		public void SetDefenderHomeId(LogicLong homeId)
		{
			m_defenderHomeId = homeId;
		}

		public void SetAttackerAllianceId(LogicLong allianceId)
		{
			m_attackerAllianceId = allianceId;
		}

		public void SetDefenderAllianceId(LogicLong allianceId)
		{
			m_defenderAllianceId = allianceId;
		}

		public void SetAttackerAllianceBadge(int badgeId)
		{
			m_attackerAllianceBadgeId = badgeId;
		}

		public void SetDefenderAllianceBadge(int badgeId)
		{
			m_defenderAllianceBadgeId = badgeId;
		}

		public void SetAttackerAllianceLevel(int level)
		{
			m_attackerAllianceLevel = level;
		}

		public void SetDefenderAllianceLevel(int level)
		{
			m_defenderAllianceLevel = level;
		}

		public void SetAttackerAllianceName(string name)
		{
			m_attackerAllianceName = name;
		}

		public void SetDefenderAllianceName(string name)
		{
			m_defenderAllianceName = name;
		}

		public void SetAttackerStars(int star)
		{
			m_attackerStars = star;
		}

		public void SetAttackerScore(int count)
		{
			m_attackerScore = count;
		}

		public void SetDefenderScore(int count)
		{
			m_defenderScore = count;
		}

		public void SetOriginalAttackerScore(int count)
		{
			m_originalAttackerScore = count;
		}

		public void SetOriginalDefenderScore(int count)
		{
			m_originalDefenderScore = count;
		}

		public void SetAttackerName(string name)
		{
			m_attackerName = name;
		}

		public void SetDefenderName(string name)
		{
			m_defenderName = name;
		}

		public void SetAllianceUsed(bool used)
		{
			m_allianceUsed = used;
		}

		public void CalculateAvailableResources(LogicLevel level, int matchType)
		{
			for (int i = m_availableLootCount.Size() - 1; i >= 0; i--)
			{
				m_availableLootCount[i].Destruct();
				m_availableLootCount.Remove(i);
			}

			LogicDataTable resourceTable = LogicDataTables.GetTable(LogicDataType.RESOURCE);

			for (int i = 0; i < resourceTable.GetItemCount(); i++)
			{
				LogicResourceData data = (LogicResourceData)resourceTable.GetItemAt(i);
				LogicResourceData warResourceReferenceData = data.GetWarResourceReferenceData();
				LogicDataSlot dataSlot = null;

				if (warResourceReferenceData != null)
				{
					for (int j = 0; j < m_availableLootCount.Size(); j++)
					{
						if (m_availableLootCount[j].GetData() == warResourceReferenceData)
						{
							dataSlot = m_availableLootCount[j];
							break;
						}
					}

					Debugger.DoAssert(dataSlot != null, "Didn't find the resource slot");
				}
				else
				{
					m_availableLootCount.Add(dataSlot = new LogicDataSlot(data, 0));
				}

				if (matchType == 8 || matchType == 9)
				{
					LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

					if (homeOwnerAvatar != null)
					{
						LogicArrayList<LogicDataSlot> resourceCount = homeOwnerAvatar.GetResources();

						for (int j = 0; j < resourceCount.Size(); j++)
						{
							if (resourceCount[j].GetData() == data)
							{
								dataSlot.SetCount(resourceCount[j].GetCount());
							}
						}
					}
				}
				else
				{
					LogicComponentManager componentManager = level.GetComponentManagerAt(level.GetVillageType());

					if (warResourceReferenceData == null)
					{
						LogicArrayList<LogicComponent> resourceProductionComponents = componentManager.GetComponents(LogicComponentType.RESOURCE_PRODUCTION);
						LogicArrayList<LogicComponent> resourceStorageComponents = componentManager.GetComponents(LogicComponentType.RESOURCE_STORAGE);

						for (int j = 0; j < resourceProductionComponents.Size(); j++)
						{
							LogicResourceProductionComponent resourceProductionComponent = (LogicResourceProductionComponent)resourceProductionComponents[j];
							LogicGameObject gameObject = resourceProductionComponent.GetParent();

							if (gameObject.IsAlive() &&
								resourceProductionComponent.IsEnabled())
							{
								if (resourceProductionComponent.GetResourceData() == data)
								{
									dataSlot.SetCount(dataSlot.GetCount() + resourceProductionComponent.GetStealableResourceCount());
								}
							}
						}

						for (int j = 0; j < resourceStorageComponents.Size(); j++)
						{
							LogicResourceStorageComponent resourceStorageComponent = (LogicResourceStorageComponent)resourceStorageComponents[j];
							LogicGameObject gameObject = resourceStorageComponent.GetParent();

							if (gameObject.IsAlive() &&
								resourceStorageComponent.IsEnabled())
							{
								dataSlot.SetCount(dataSlot.GetCount() + resourceStorageComponent.GetStealableResourceCount(i));
							}
						}
					}
					else
					{
						LogicArrayList<LogicComponent> warResourceStorageComponents = componentManager.GetComponents(LogicComponentType.WAR_RESOURCE_STORAGE);

						for (int j = 0; j < warResourceStorageComponents.Size(); j++)
						{
							LogicWarResourceStorageComponent resourceWarStorageComponent = (LogicWarResourceStorageComponent)warResourceStorageComponents[j];
							LogicGameObject gameObject = resourceWarStorageComponent.GetParent();

							if (gameObject.IsAlive() &&
								resourceWarStorageComponent.IsEnabled())
							{
								dataSlot.SetCount(dataSlot.GetCount() + resourceWarStorageComponent.GetStealableResourceCount(i));
							}
						}
					}
				}
			}
		}

		public void IncrementDeployedAttackerUnits(LogicCombatItemData data, int count)
		{
			int multiplier = data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_HERO
				? LogicDataTables.GetGlobals().GetUnitHousingCostMultiplier()
				: LogicDataTables.GetGlobals().GetHeroHousingCostMultiplier();
			int maxHousingSpace = LogicDataTables.GetTownHallLevel(m_level.GetTownHallLevel(0)).GetMaxHousingSpace();

			if (maxHousingSpace > 0)
			{
				m_armyDeploymentPercentage =
					(100000 * m_deployedHousingSpace / maxHousingSpace + 50) / 100;
			}

			m_deployedHousingSpace += multiplier * data.GetHousingSpace() * count / 100;

			int index = -1;

			for (int i = 0; i < m_castedUnitCount.Size(); i++)
			{
				if (m_castedUnitCount[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_castedUnitCount[index].SetCount(m_castedUnitCount[index].GetCount() + count);
			}
			else
			{
				m_castedUnitCount.Add(new LogicDataSlot(data, count));
			}
		}

		public void IncrementDeployedAllianceUnits(LogicCombatItemData data, int count, int upgLevel)
		{
			int multiplier = data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_HERO
				? LogicDataTables.GetGlobals().GetUnitHousingCostMultiplier()
				: LogicDataTables.GetGlobals().GetHeroHousingCostMultiplier();
			int maxHousingSpace = LogicDataTables.GetTownHallLevel(m_level.GetTownHallLevel(0)).GetMaxHousingSpace();

			if (maxHousingSpace > 0)
			{
				m_armyDeploymentPercentage =
					(100000 * m_deployedHousingSpace / maxHousingSpace + 50) / 100;
			}

			m_deployedHousingSpace += multiplier * data.GetHousingSpace() * count / 100;

			int index = -1;

			for (int i = 0; i < m_castedAllianceUnitCount.Size(); i++)
			{
				if (m_castedAllianceUnitCount[i].GetData() == data && m_castedAllianceUnitCount[i].GetLevel() == upgLevel)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_castedAllianceUnitCount[index].SetCount(m_castedAllianceUnitCount[index].GetCount() + count);
			}
			else
			{
				m_castedAllianceUnitCount.Add(new LogicUnitSlot(data, upgLevel, count));
			}
		}

		public void IncrementCastedSpells(LogicSpellData data, int count)
		{
			m_deployedHousingSpace += LogicDataTables.GetGlobals().GetSpellHousingCostMultiplier() * data.GetHousingSpace() * count / 100;
			m_armyDeploymentPercentage =
				(100000 * m_deployedHousingSpace / LogicDataTables.GetTownHallLevel(m_level.GetTownHallLevel(0)).GetMaxHousingSpace() + 50) / 100;

			int index = -1;

			for (int i = 0; i < m_castedSpellCount.Size(); i++)
			{
				if (m_castedSpellCount[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_castedSpellCount[index].SetCount(m_castedSpellCount[index].GetCount() + count);
			}
			else
			{
				m_castedSpellCount.Add(new LogicDataSlot(data, count));
			}
		}

		public void IncrementDestroyedBuildingCount(LogicBuildingData data)
		{
			LogicBuildingClassData buildingClass = data.GetBuildingClass();

			if (buildingClass.IsTownHall() || buildingClass.IsTownHall2())
			{
				m_townhallDestroyed = true;
			}

			if (m_level != null)
			{
				LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

				if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar())
				{
					if (m_level.GetState() == 2)
					{
						int matchType = m_level.GetMatchType();

						if (matchType != 2 && matchType != 5)
						{
							m_level.GetAchievementManager().BuildingDestroyedInPvP(data);
						}
					}
				}
			}
		}

		public void SetCombatItemLevel(LogicData data, int upgLevel)
		{
			int index = -1;

			for (int i = 0; i < m_unitLevelCount.Size(); i++)
			{
				if (m_unitLevelCount[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_unitLevelCount[index].SetCount(upgLevel);
			}
			else
			{
				m_unitLevelCount.Add(new LogicDataSlot(data, upgLevel));
			}
		}

		public int GetDestructionPercentage()
			=> m_destructionPercentage;

		public int GetDeployedHousingSpace()
			=> m_deployedHousingSpace;

		public void SetDestructionPercentage(int percentage)
		{
			if (m_destructionPercentage <= percentage)
			{
				m_destructionPercentage = percentage;
			}
			else
			{
				Debugger.Warning("LogicBattleLog: m_destructionPercentage decreases");
			}
		}

		public LogicJSONObject GenerateDefenderJSON()
			=> GenerateBattleLogJSON(m_attackerHomeId, m_attackerAllianceId, m_attackerAllianceBadgeId, m_attackerAllianceName,
											  m_defenderAllianceBadgeId, m_attackerAllianceLevel, m_defenderAllianceLevel);

		public LogicJSONObject GenerateAttackerJSON()
			=> GenerateBattleLogJSON(m_defenderHomeId, m_defenderAllianceId, m_defenderAllianceBadgeId, m_defenderAllianceName,
											  m_attackerAllianceBadgeId, m_defenderAllianceLevel, m_attackerAllianceLevel);

		public LogicJSONObject GenerateBattleLogJSON(LogicLong homeId, LogicLong allianceId, int allianceBadgeId, string allianceName, int allianceBadgeId2, int allianceExpLevel,
													 int allianceExpLevel2)
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("villageType", new LogicJSONNumber(m_villageType));

			bool village2Match = true;

			if ((m_level.GetMatchType() & 0xFFFFFFFE) != 8)
			{
				jsonObject.Put("loot", LogicBattleLog.DataSlotArrayToJSONArray(m_lootCount));
				jsonObject.Put("availableLoot", LogicBattleLog.DataSlotArrayToJSONArray(m_availableLootCount));

				village2Match = false;
			}

			jsonObject.Put("units", LogicBattleLog.DataSlotArrayToJSONArray(m_castedUnitCount));

			if (!village2Match)
			{
				if (m_castedAllianceUnitCount != null && m_castedAllianceUnitCount.Size() > 0)
				{
					jsonObject.Put("cc_units", LogicBattleLog.UnitSlotArrayToJSONArray(m_castedAllianceUnitCount));
				}
			}

			if (m_costCount != null && m_costCount.Size() > 0)
			{
				jsonObject.Put("costs", LogicBattleLog.DataSlotArrayToJSONArray(m_costCount));
			}

			if (!village2Match)
			{
				jsonObject.Put("spells", LogicBattleLog.DataSlotArrayToJSONArray(m_castedSpellCount));
			}

			jsonObject.Put("levels", LogicBattleLog.DataSlotArrayToJSONArray(m_unitLevelCount));

			LogicJSONObject statObject = new LogicJSONObject();

			statObject.Put("townhallDestroyed", new LogicJSONBoolean(m_townhallDestroyed));
			statObject.Put("battleEnded", new LogicJSONBoolean(m_battleEnded));

			if (!village2Match)
			{
				statObject.Put("allianceUsed", new LogicJSONBoolean(m_allianceUsed));
			}

			statObject.Put("destructionPercentage", new LogicJSONNumber(m_destructionPercentage));
			statObject.Put("battleTime", new LogicJSONNumber(m_battleTime));

			if (!village2Match)
			{
				statObject.Put("originalAttackerScore", new LogicJSONNumber(m_originalAttackerScore));
				statObject.Put("attackerScore", new LogicJSONNumber(m_attackerScore));
				statObject.Put("originalDefenderScore", new LogicJSONNumber(m_originalDefenderScore));
				statObject.Put("defenderScore", new LogicJSONNumber(m_defenderScore));
			}

			statObject.Put("allianceName", new LogicJSONString(allianceName));

			if (!village2Match)
			{
				statObject.Put("attackerStars", new LogicJSONNumber(m_attackerStars));
			}

			if (m_level.GetMatchType() <= 7)
			{
				statObject.Put("attackerName", new LogicJSONString(m_attackerName));
				statObject.Put("defenderName", new LogicJSONString(m_defenderName));

				int lootMultiplierByTownHallDiff = 100;

				if (LogicDataTables.GetGlobals().UseTownHallLootPenaltyInWar())
				{
					lootMultiplierByTownHallDiff = LogicDataTables.GetGlobals().GetLootMultiplierByTownHallDiff(m_level.GetVisitorAvatar().GetTownHallLevel(),
																												m_level.GetHomeOwnerAvatar().GetTownHallLevel());
				}

				statObject.Put("lootMultiplierByTownHallDiff", new LogicJSONNumber(lootMultiplierByTownHallDiff));
			}

			LogicJSONArray homeIdArray = new LogicJSONArray(2);

			homeIdArray.Add(new LogicJSONNumber(homeId.GetHigherInt()));
			homeIdArray.Add(new LogicJSONNumber(homeId.GetLowerInt()));

			statObject.Put("homeID", homeIdArray);

			if (allianceBadgeId != -2)
			{
				statObject.Put("allianceBadge", new LogicJSONNumber(allianceBadgeId));
			}

			if (allianceBadgeId2 != -2)
			{
				statObject.Put("allianceBadge2", new LogicJSONNumber(allianceBadgeId2));
			}

			if (allianceId != null)
			{
				LogicJSONArray allianceIdArray = new LogicJSONArray(2);

				allianceIdArray.Add(new LogicJSONNumber(allianceId.GetHigherInt()));
				allianceIdArray.Add(new LogicJSONNumber(allianceId.GetLowerInt()));

				statObject.Put("allianceID", allianceIdArray);
			}

			if (!village2Match)
			{
				statObject.Put("deployedHousingSpace", new LogicJSONNumber(m_deployedHousingSpace));
				statObject.Put("armyDeploymentPercentage", new LogicJSONNumber(m_armyDeploymentPercentage));
			}

			if (allianceExpLevel != 0)
			{
				statObject.Put("allianceExp", new LogicJSONNumber(allianceExpLevel));
			}

			if (allianceExpLevel2 != 0)
			{
				statObject.Put("allianceExp2", new LogicJSONNumber(allianceExpLevel2));
			}

			jsonObject.Put("stats", statObject);

			return jsonObject;
		}

		public LogicJSONObject LoadBattleLogFromJSON(LogicJSONObject root)
		{
			LogicJSONNumber villageTypeNumber = root.GetJSONNumber("villageType");

			if (villageTypeNumber != null)
			{
				m_villageType = villageTypeNumber.GetIntValue();
			}

			LogicJSONNode lootNode = root.Get("loot");

			if (lootNode != null && lootNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONDataSlotsToArray((LogicJSONArray)lootNode, m_lootCount);
			}
			else if (m_villageType != 1)
			{
				Debugger.Warning("LogicBattleLog has no loot.");
			}

			LogicJSONNode unitNode = root.Get("units");

			if (unitNode != null && unitNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONDataSlotsToArray((LogicJSONArray)unitNode, m_castedUnitCount);
			}
			else
			{
				Debugger.Warning("LogicBattleLog has no loot.");
			}

			LogicJSONNode allianceUnitNode = root.Get("cc_units");

			if (allianceUnitNode != null && allianceUnitNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONUnitSlotsToArray((LogicJSONArray)allianceUnitNode, m_castedAllianceUnitCount);
			}

			LogicJSONNode costNode = root.Get("costs");

			if (costNode != null && costNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONDataSlotsToArray((LogicJSONArray)costNode, m_costCount);
			}

			LogicJSONNode spellNode = root.Get("spells");

			if (spellNode != null && spellNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONDataSlotsToArray((LogicJSONArray)spellNode, m_costCount);
			}
			else if (m_villageType != 1)
			{
				Debugger.Warning("LogicBattleLog has no spells.");
			}

			LogicJSONNode levelNode = root.Get("levels");

			if (levelNode != null && levelNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				LogicBattleLog.AddJSONDataSlotsToArray((LogicJSONArray)levelNode, m_unitLevelCount);
			}
			else
			{
				Debugger.Warning("LogicBattleLog has no levels.");
			}

			LogicJSONNode statsNode = root.Get("stats");

			if (statsNode != null && statsNode.GetJSONNodeType() == LogicJSONNodeType.OBJECT)
			{
				LogicJSONObject statsObject = (LogicJSONObject)statsNode;
				LogicJSONBoolean townhallDestroyedBoolean = statsObject.GetJSONBoolean("townhallDestroyed");

				if (townhallDestroyedBoolean != null)
				{
					m_townhallDestroyed = townhallDestroyedBoolean.IsTrue();
				}

				LogicJSONBoolean battleEndedBoolean = statsObject.GetJSONBoolean("battleEnded");

				if (battleEndedBoolean != null)
				{
					m_battleEnded = battleEndedBoolean.IsTrue();
				}

				LogicJSONBoolean allianceUsedBoolean = statsObject.GetJSONBoolean("allianceUsed");

				if (allianceUsedBoolean != null)
				{
					m_allianceUsed = allianceUsedBoolean.IsTrue();
				}

				LogicJSONNumber destructionPercentageNumber = statsObject.GetJSONNumber("destructionPercentage");

				if (destructionPercentageNumber != null)
				{
					m_destructionPercentage = destructionPercentageNumber.GetIntValue();
				}

				LogicJSONNumber battleTimeNumber = statsObject.GetJSONNumber("battleTime");

				if (battleTimeNumber != null)
				{
					m_battleTime = battleTimeNumber.GetIntValue();
				}

				LogicJSONNumber attackerScoreNumber = statsObject.GetJSONNumber("attackerScore");

				if (attackerScoreNumber != null)
				{
					m_attackerScore = attackerScoreNumber.GetIntValue();
				}

				LogicJSONNumber defenderScoreNumber = statsObject.GetJSONNumber("defenderScore");

				if (defenderScoreNumber != null)
				{
					m_defenderScore = defenderScoreNumber.GetIntValue();
				}

				LogicJSONNumber originalAttackerScoreNumber = statsObject.GetJSONNumber("originalAttackerScore");

				if (originalAttackerScoreNumber != null)
				{
					m_originalAttackerScore = originalAttackerScoreNumber.GetIntValue();
				}
				else
				{
					m_attackerScore = -1;
				}

				LogicJSONNumber originalDefenderScoreNumber = statsObject.GetJSONNumber("originalDefenderScore");

				if (originalDefenderScoreNumber != null)
				{
					m_originalDefenderScore = originalDefenderScoreNumber.GetIntValue();
				}
				else
				{
					m_originalDefenderScore = -1;
				}

				LoadAttackerNameFromJson(statsObject);
				LoadDefenderNameFromJson(statsObject);

				LogicJSONNumber lootMultiplierByTownHallDiffNumber = statsObject.GetJSONNumber("lootMultiplierByTownHallDiff");

				if (lootMultiplierByTownHallDiffNumber != null)
				{
					m_lootMultiplierByTownHallDiff = lootMultiplierByTownHallDiffNumber.GetIntValue();
				}
				else
				{
					m_lootMultiplierByTownHallDiff = -1;
				}

				LogicJSONNumber deployedHousingSpaceNumber = statsObject.GetJSONNumber("deployedHousingSpace");

				if (deployedHousingSpaceNumber != null)
				{
					m_deployedHousingSpace = deployedHousingSpaceNumber.GetIntValue();
				}

				LogicJSONNumber armyDeploymentPercentageNumber = statsObject.GetJSONNumber("armyDeploymentPercentage");

				if (armyDeploymentPercentageNumber != null)
				{
					m_armyDeploymentPercentage = armyDeploymentPercentageNumber.GetIntValue();
				}

				LogicJSONNumber attackerStarsNumber = statsObject.GetJSONNumber("attackerStars");

				if (attackerStarsNumber != null)
				{
					m_attackerStars = attackerStarsNumber.GetIntValue();
				}

				return statsObject;
			}

			Debugger.Warning("LogicBattleLog has no stats.");

			return null;
		}

		public void LoadAttackerNameFromJson(LogicJSONObject jsonObject)
		{
			LogicJSONString attackerNameObject = jsonObject.GetJSONString("attackerName");

			if (attackerNameObject != null)
			{
				m_attackerName = attackerNameObject.GetStringValue();
			}
			else
			{
				m_attackerName = string.Empty;
			}
		}

		public void LoadDefenderNameFromJson(LogicJSONObject jsonObject)
		{
			LogicJSONString defenderNameObject = jsonObject.GetJSONString("defenderName");

			if (defenderNameObject != null)
			{
				m_defenderName = defenderNameObject.GetStringValue();
			}
			else
			{
				m_defenderName = string.Empty;
			}
		}

		public static void AddJSONDataSlotsToArray(LogicJSONArray jsonArray, LogicArrayList<LogicDataSlot> slot)
		{
			for (int i = 0; i < jsonArray.Size(); i++)
			{
				LogicJSONArray objectArray = jsonArray.GetJSONArray(i);

				if (objectArray != null && objectArray.Size() == 2)
				{
					LogicData data = LogicDataTables.GetDataById(jsonArray.GetJSONNumber(0).GetIntValue());
					int count = objectArray.GetJSONNumber(1).GetIntValue();

					slot.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public static void AddJSONUnitSlotsToArray(LogicJSONArray jsonArray, LogicArrayList<LogicUnitSlot> slot)
		{
			for (int i = 0; i < jsonArray.Size(); i++)
			{
				LogicJSONArray objectArray = jsonArray.GetJSONArray(i);

				if (objectArray != null && objectArray.Size() == 2)
				{
					LogicData data = LogicDataTables.GetDataById(jsonArray.GetJSONNumber(0).GetIntValue());

					int level = objectArray.GetJSONNumber(1).GetIntValue();
					int count = objectArray.GetJSONNumber(2).GetIntValue();

					slot.Add(new LogicUnitSlot(data, level, count));
				}
			}
		}

		public static LogicJSONArray DataSlotArrayToJSONArray(LogicArrayList<LogicDataSlot> dataSlotArray)
		{
			LogicJSONArray jsonArray = new LogicJSONArray(dataSlotArray.Size());

			for (int i = 0; i < dataSlotArray.Size(); i++)
			{
				LogicDataSlot dataSlot = dataSlotArray[i];
				LogicJSONArray objectArray = new LogicJSONArray();

				objectArray.Add(new LogicJSONNumber(dataSlot.GetData().GetGlobalID()));
				objectArray.Add(new LogicJSONNumber(dataSlot.GetCount()));

				jsonArray.Add(objectArray);
			}

			return jsonArray;
		}

		public static LogicJSONArray UnitSlotArrayToJSONArray(LogicArrayList<LogicUnitSlot> dataSlotArray)
		{
			LogicJSONArray jsonArray = new LogicJSONArray(dataSlotArray.Size());

			for (int i = 0; i < dataSlotArray.Size(); i++)
			{
				LogicUnitSlot unitSlot = dataSlotArray[i];
				LogicJSONArray objectArray = new LogicJSONArray();

				objectArray.Add(new LogicJSONNumber(unitSlot.GetData().GetGlobalID()));
				objectArray.Add(new LogicJSONNumber(unitSlot.GetLevel()));
				objectArray.Add(new LogicJSONNumber(unitSlot.GetCount()));

				jsonArray.Add(objectArray);
			}

			return jsonArray;
		}
	}
}