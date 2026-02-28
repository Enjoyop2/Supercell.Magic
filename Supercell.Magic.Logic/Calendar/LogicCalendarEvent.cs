using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicCalendarEvent
	{
		public const int EVENT_TYPE_BASE = 0;
		public const int EVENT_TYPE_OFFER = 1;
		public const int EVENT_TYPE_DUEL_LOOT_LIMIT = 4;

		private int m_id;
		private int m_version;

		private int m_startTime;
		private int m_endTime;
		private int m_visibleTime;
		private int m_visibleEndTime;

		private int m_inboxEntryId;

		private int m_newTrainingBoostBarracksCost;
		private int m_newTrainingBoostSpellCost;
		private int m_allianceXpMultiplier;
		private int m_starBonusMultiplier;
		private int m_allianceWarWinLootMultiplier;
		private int m_allianceWarDrawLootMultiplier;
		private int m_allianceWarLooseLootMultiplier;

		private string m_clashBoxEntryName;
		private string m_notificationTid;
		private string m_image;
		private string m_sc;
		private string m_localization;

		private LogicArrayList<LogicCalendarFunction> m_functions;
		private readonly LogicArrayList<LogicDataSlot> m_buildingBoostCost;
		private LogicArrayList<LogicDataSlot> m_troopDiscount;
		private LogicArrayList<LogicData> m_enabledData;
		private LogicArrayList<LogicDataSlot> m_freeTroops;
		private LogicArrayList<LogicCalendarUseTroop> m_useTroops;

		private LogicCalendarBuildingDestroyedSpawnUnit m_buildingDestroyedSpawnUnit;
		private LogicEventEntryData m_eventEntryData;
		private LogicCalendarTargeting m_targeting;
		private LogicCalendarErrorHandler m_errorHandler;

		public LogicCalendarEvent()
		{
			m_errorHandler = new LogicCalendarErrorHandler();

			m_functions = new LogicArrayList<LogicCalendarFunction>();
			m_buildingBoostCost = new LogicArrayList<LogicDataSlot>();
			m_troopDiscount = new LogicArrayList<LogicDataSlot>();
			m_enabledData = new LogicArrayList<LogicData>();
			m_freeTroops = new LogicArrayList<LogicDataSlot>();
			m_useTroops = new LogicArrayList<LogicCalendarUseTroop>();

			m_allianceXpMultiplier = 1;
			m_starBonusMultiplier = 1;
			m_allianceWarWinLootMultiplier = 1;
			m_allianceWarDrawLootMultiplier = 1;
			m_allianceWarLooseLootMultiplier = 1;
		}

		public virtual void Destruct()
		{
			if (m_functions != null)
			{
				for (int i = m_functions.Size() - 1; i >= 0; i--)
				{
					m_functions[i].Destruct();
					m_functions.Remove(i);
				}

				m_functions = null;
			}

			if (m_buildingBoostCost != null)
			{
				while (m_buildingBoostCost.Size() > 0)
				{
					m_buildingBoostCost[0].Destruct();
					m_buildingBoostCost.Remove(0);
				}

				m_troopDiscount = null;
			}

			if (m_troopDiscount != null)
			{
				while (m_troopDiscount.Size() > 0)
				{
					m_troopDiscount[0].Destruct();
					m_troopDiscount.Remove(0);
				}

				m_troopDiscount = null;
			}

			if (m_freeTroops != null)
			{
				while (m_freeTroops.Size() > 0)
				{
					m_freeTroops[0].Destruct();
					m_freeTroops.Remove(0);
				}

				m_freeTroops = null;
			}

			m_enabledData = null;
			m_useTroops = null;

			m_clashBoxEntryName = null;
			m_notificationTid = null;
			m_image = null;
			m_sc = null;
			m_localization = null;

			m_eventEntryData = null;
			m_targeting = null;
			m_errorHandler = null;
		}

		public void SetErrorHandler(LogicCalendarErrorHandler errorHandler)
		{
			m_errorHandler = errorHandler;
		}

		public void Init(LogicJSONObject jsonObject)
		{
			Load(jsonObject);
			ApplyFunctions();
		}

		public virtual void Load(LogicJSONObject jsonObject)
		{
			if (jsonObject == null)
			{
				m_errorHandler.Error(this, "Json cannot be null");
			}

			m_id = LogicJSONHelper.GetInt(jsonObject, "id", -1);
			m_version = LogicJSONHelper.GetInt(jsonObject, "version", 0);

			m_newTrainingBoostBarracksCost = LogicDataTables.GetGlobals().GetNewTrainingBoostBarracksCost();
			m_newTrainingBoostSpellCost = LogicDataTables.GetGlobals().GetNewTrainingBoostLaboratoryCost();

			m_startTime = LogicCalendarEvent.ConvertStringToTimestamp(LogicJSONHelper.GetString(jsonObject, "startTime"), false);
			m_endTime = LogicCalendarEvent.ConvertStringToTimestamp(LogicJSONHelper.GetString(jsonObject, "endTime"), true);

			if (m_startTime >= m_endTime)
			{
				m_errorHandler.ErrorField(this, "endTime", "End time must be after start time.");
			}

			LogicJSONString visibleTimeString = jsonObject.GetJSONString("visibleTime");

			if (visibleTimeString != null)
			{
				m_visibleTime = LogicCalendarEvent.ConvertStringToTimestamp(visibleTimeString.GetStringValue(), false);

				if (m_visibleTime > m_startTime)
				{
					m_errorHandler.ErrorField(this, "visibleTime", "Visible time must be before or at start time.");
				}
			}
			else
			{
				m_visibleTime = 0;
			}

			m_clashBoxEntryName = jsonObject.GetJSONString("clashBoxEntryName").GetStringValue();

			LogicJSONString eventEntryNameString = jsonObject.GetJSONString("eventEntryName");

			m_eventEntryData = LogicDataTables.GetEventEntryByName(eventEntryNameString.GetStringValue(), null);

			if (eventEntryNameString.GetStringValue().Length > 0)
			{
				if (m_eventEntryData == null)
				{
					m_errorHandler.ErrorField(this, "eventEntryName", string.Format("Invalid event entry name: {0}.", eventEntryNameString.GetStringValue()));
				}

				if (m_visibleTime == 0)
				{
					m_errorHandler.ErrorField(this, "visibleTime", "Visible time must be set if event entry name is set.");
				}
			}

			if (m_visibleTime != 0)
			{
				if (m_eventEntryData == null)
				{
					m_errorHandler.ErrorField(this, "eventEntryName", "Event entry name must be set if visible time is set.");
				}
			}

			m_inboxEntryId = LogicJSONHelper.GetInt(jsonObject, "inboxEntryId", -1);
			m_notificationTid = LogicJSONHelper.GetString(jsonObject, "notificationTid");
			m_image = LogicJSONHelper.GetString(jsonObject, "image");
			m_sc = LogicJSONHelper.GetString(jsonObject, "sc");
			m_localization = LogicJSONHelper.GetString(jsonObject, "localization");

			LogicJSONObject targetingObject = jsonObject.GetJSONObject("targeting");

			if (targetingObject != null)
			{
				m_targeting = new LogicCalendarTargeting(jsonObject);
			}

			LogicJSONArray functionArray = jsonObject.GetJSONArray("functions");

			if (functionArray != null)
			{
				for (int i = 0; i < functionArray.Size(); i++)
				{
					m_functions.Add(new LogicCalendarFunction(this, i, functionArray.GetJSONObject(i), m_errorHandler));
				}
			}
		}

		public virtual LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("type", new LogicJSONNumber(GetCalendarEventType()));
			jsonObject.Put("id", new LogicJSONNumber(m_id));
			jsonObject.Put("version", new LogicJSONNumber(m_version));
			jsonObject.Put("visibleTime", new LogicJSONString(LogicCalendarEvent.ConvertTimestampToString(m_visibleTime)));
			jsonObject.Put("startTime", new LogicJSONString(LogicCalendarEvent.ConvertTimestampToString(m_startTime)));
			jsonObject.Put("endTime", new LogicJSONString(LogicCalendarEvent.ConvertTimestampToString(m_endTime)));
			jsonObject.Put("clashBoxEntryName", new LogicJSONString(m_clashBoxEntryName));
			jsonObject.Put("eventEntryName", new LogicJSONString(m_eventEntryData != null ? m_eventEntryData.GetName() : string.Empty));
			jsonObject.Put("inboxEntryId", new LogicJSONNumber(m_inboxEntryId));
			jsonObject.Put("notificationTid", new LogicJSONString(m_notificationTid));
			jsonObject.Put("image", new LogicJSONString(m_image));
			jsonObject.Put("sc", new LogicJSONString(m_sc));
			jsonObject.Put("localization", new LogicJSONString(m_localization));

			LogicJSONArray functionArray = new LogicJSONArray(m_functions.Size());

			for (int i = 0; i < m_functions.Size(); i++)
			{
				functionArray.Add(m_functions[i].Save());
			}

			jsonObject.Put("functions", functionArray);

			if (m_targeting != null)
			{
				LogicJSONObject targetingObject = new LogicJSONObject();
				m_targeting.Save(jsonObject);
				jsonObject.Put("targeting", targetingObject);
			}

			return jsonObject;
		}

		public void ApplyFunctions()
		{
			for (int i = 0; i < m_functions.Size(); i++)
			{
				m_functions[i].ApplyToEvent(this);
			}
		}

		private static int ConvertStringToTimestamp(string time, bool round)
		{
			int spliter = time.IndexOf("T");

			Debugger.DoAssert(spliter == 8, "Unable to convert time. ISO8601 expected.");
			LogicGregDate date = new LogicGregDate(LogicStringUtil.ConvertToInt(time, 0, 4),
												   LogicStringUtil.ConvertToInt(time, 4, 6),
												   LogicStringUtil.ConvertToInt(time, 6, 8));

			date.Validate();

			int totalSecs = date.GetIndex() * 86400;
			string dayTime = time.Substring(spliter + 1);

			if (dayTime.Length < 2)
			{
				if (round)
				{
					return totalSecs + 82800;
				}

				return totalSecs;
			}

			totalSecs += 3600 * LogicStringUtil.ConvertToInt(dayTime, 0, 2);

			if (dayTime.Length < 4)
			{
				if (round)
				{
					return totalSecs + 3540;
				}

				return totalSecs;
			}

			totalSecs += 60 * LogicStringUtil.ConvertToInt(dayTime, 2, 4);

			if (dayTime.Length < 6)
			{
				if (round)
				{
					return totalSecs + 59;
				}

				return totalSecs;
			}

			return totalSecs + LogicStringUtil.ConvertToInt(dayTime, 4, 6);
		}

		private static string ConvertTimestampToString(int timestamp)
		{
			LogicGregDate gregDate = new LogicGregDate(timestamp / 86400);
			return string.Format("{0:D4}{1:D2}{2:D2}T{3:D2}{4:D2}{5:D2}.000Z", gregDate.GetYear(), gregDate.GetMonth(), gregDate.GetDay(),
								 timestamp % 86400 / 3600,
								 timestamp % 86400 % 3600 / 60,
								 timestamp % 86400 % 3600 % 60);
		}

		public virtual int GetCalendarEventType()
			=> LogicCalendarEvent.EVENT_TYPE_BASE;

		public int GetId()
			=> m_id;

		public void SetId(int value)
		{
			m_id = value;
		}

		public int GetVersion()
			=> m_version;

		public void SetVersion(int value)
		{
			m_version = value;
		}

		public int GetStartTime()
			=> m_startTime;

		public void SetStartTime(int value)
		{
			m_startTime = value;
		}

		public int GetEndTime()
			=> m_endTime;

		public void SetEndTime(int value)
		{
			m_endTime = value;
		}

		public int GetVisibleTime()
			=> m_visibleTime;

		public void SetVisibleTime(int value)
		{
			m_visibleTime = value;
		}

		public int GetVisibleEndTime()
			=> m_visibleEndTime;

		public void SetVisibleEndTime(int value)
		{
			m_visibleEndTime = value;
		}

		public int GetInboxEntryId()
			=> m_inboxEntryId;

		public void SetInboxEntryId(int value)
		{
			m_inboxEntryId = value;
		}

		public string GetClashBoxEntryName()
			=> m_clashBoxEntryName;

		public void SetClashBoxEntryName(string value)
		{
			m_clashBoxEntryName = value;
		}

		public string GetNotificationTid()
			=> m_notificationTid;

		public void SetNotificationTid(string value)
		{
			m_notificationTid = value;
		}

		public string GetImage()
			=> m_image;

		public void SetImage(string value)
		{
			m_image = value;
		}

		public string GetSc()
			=> m_sc;

		public void SetSc(string value)
		{
			m_sc = value;
		}

		public string GetLocalization()
			=> m_localization;

		public void SetLocalization(string value)
		{
			m_localization = value;
		}

		public LogicEventEntryData GetEventEntryData()
			=> m_eventEntryData;

		public void SetEventEntryData(LogicEventEntryData value)
		{
			m_eventEntryData = value;
		}

		public int GetNewTrainingBoostBarracksCost()
			=> m_newTrainingBoostBarracksCost;

		public void SetNewTrainingBoostBarracksCost(int value)
		{
			m_newTrainingBoostBarracksCost = value;
		}

		public int GetNewTrainingBoostSpellCost()
			=> m_newTrainingBoostSpellCost;

		public void SetNewTrainingBoostSpellCost(int value)
		{
			m_newTrainingBoostSpellCost = value;
		}

		public void AddBuildingBoost(LogicData data, int count)
		{
			m_buildingBoostCost.Add(new LogicDataSlot(data, count));
		}

		public int GetBuildingBoostCost(LogicBuildingData data, int upgLevel)
		{
			for (int i = 0; i < m_buildingBoostCost.Size(); i++)
			{
				LogicDataSlot slot = m_buildingBoostCost[i];

				if (slot.GetData() == data)
				{
					return slot.GetCount();
				}
			}

			return data.GetBoostCost(upgLevel);
		}

		public void AddTroopDiscount(LogicData data, int count)
		{
			m_troopDiscount.Add(new LogicDataSlot(data, count));
		}

		public int GetTrainingCost(LogicCombatItemData data, int upgLevel)
		{
			int trainingCost = data.GetTrainingCost(upgLevel);

			for (int i = 0; i < m_troopDiscount.Size(); i++)
			{
				LogicDataSlot slot = m_troopDiscount[i];

				if (slot.GetData() == data)
				{
					return (slot.GetCount() * trainingCost + 99) / 100;
				}
			}

			return trainingCost;
		}

		public void AddEnabledData(LogicData data)
		{
			m_enabledData.Add(data);
		}

		public bool IsEnabled(LogicData data)
		{
			if (data.IsEnableByCalendar())
			{
				if (m_enabledData.IndexOf(data) != -1)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		public void AddFreeTroop(LogicCombatItemData data, int count)
		{
			if (!data.IsProductionEnabled())
			{
				Debugger.Error(data.GetName() + " cannot be produced!");
			}

			m_freeTroops.Add(new LogicDataSlot(data, count));
		}

		public void AddUseTroop(LogicCombatItemData data, int count, int ratioOfHousing, int rewardDiamonds, int rewardXp)
		{
			LogicCalendarUseTroop calendarUseTroop = new LogicCalendarUseTroop(data);

			calendarUseTroop.AddParameter(count);
			calendarUseTroop.AddParameter(ratioOfHousing);
			calendarUseTroop.AddParameter(rewardDiamonds);
			calendarUseTroop.AddParameter(rewardXp);

			m_useTroops.Add(calendarUseTroop);
		}

		public void AddBuildingDestroyedSpawnUnit(LogicBuildingData data, LogicCharacterData spawnData, int count)
		{
			SetBuildingDestroyedSpawnUnit(new LogicCalendarBuildingDestroyedSpawnUnit(data, spawnData, count));
		}

		public void SetBuildingDestroyedSpawnUnit(LogicCalendarBuildingDestroyedSpawnUnit buildingDestroyedSpawnUnit)
		{
			m_buildingDestroyedSpawnUnit = buildingDestroyedSpawnUnit;
		}

		public int GetAllianceXpMultiplier()
			=> m_allianceXpMultiplier;

		public void SetAllianceXpMultiplier(int value)
		{
			m_allianceXpMultiplier = value;
		}

		public int GetStarBonusMultiplier()
			=> m_starBonusMultiplier;

		public void SetStarBonusMultiplier(int value)
		{
			m_starBonusMultiplier = value;
		}

		public int GetAllianceWarWinLootMultiplier()
			=> m_allianceWarWinLootMultiplier;

		public void SetAllianceWarWinLootMultiplier(int value)
		{
			m_allianceWarWinLootMultiplier = value;
		}

		public int GetAllianceWarDrawLootMultiplier()
			=> m_allianceWarDrawLootMultiplier;

		public void SetAllianceWarDrawLootMultiplier(int value)
		{
			m_allianceWarDrawLootMultiplier = value;
		}

		public int GetAllianceWarLooseLootMultiplier()
			=> m_allianceWarLooseLootMultiplier;

		public void SetAllianceWarLooseLootMultiplier(int value)
		{
			m_allianceWarLooseLootMultiplier = value;
		}

		public bool IsEqual(LogicCalendarEvent calendarEvent)
		{
			if (m_id == calendarEvent.m_id)
			{
				return m_version == calendarEvent.m_version;
			}

			return false;
		}

		public void StartUseTroopEvent(LogicAvatar homeOwnerAvatar, LogicLevel level)
		{
			if (homeOwnerAvatar != null)
			{
				for (int i = 0; i < m_useTroops.Size(); i++)
				{
					LogicCalendarUseTroop calendarUseTroop = m_useTroops[i];
					LogicCombatItemData data = calendarUseTroop.GetData();

					int housingSpace;
					int totalMaxHousing;
					int unitCount;

					if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
					{
						housingSpace = data.GetHousingSpace() * 2;
						totalMaxHousing = data.GetHousingSpace() + 2 * (level.GetComponentManagerAt(data.GetVillageType()).GetTotalMaxHousing(data.GetCombatItemType()) *
																		calendarUseTroop.GetParameter(1) / 100);
						unitCount = totalMaxHousing / housingSpace;
					}
					else
					{
						LogicBuildingData troopHousingData = LogicDataTables.GetBuildingByName("Troop Housing", null);
						LogicBuildingData barrackData = LogicDataTables.GetBuildingByName("Barrack", null);
						LogicBuildingData darkElixirBarrackData = LogicDataTables.GetBuildingByName("Dark Elixir Barrack", null);

						int townHallLevel = homeOwnerAvatar.GetTownHallLevel();
						int maxUpgradeLevelForTH = troopHousingData.GetMaxUpgradeLevelForTownHallLevel(townHallLevel);
						int unitStorageCapacity = troopHousingData.GetUnitStorageCapacity(maxUpgradeLevelForTH);

						housingSpace = data.GetHousingSpace();

						if (data.GetUnitOfType() == 1 && barrackData.GetRequiredTownHallLevel(data.GetRequiredProductionHouseLevel()) <= townHallLevel ||
							data.GetUnitOfType() == 2 && darkElixirBarrackData.GetRequiredTownHallLevel(data.GetRequiredProductionHouseLevel()) <= townHallLevel)
						{
							int totalHousing = (int)((long)LogicDataTables.GetTownHallLevel(townHallLevel).GetUnlockedBuildingCount(troopHousingData) *
													  calendarUseTroop.GetParameter(1) *
													  unitStorageCapacity);
							unitCount = (int)((housingSpace * 0.5f + totalHousing / 100) / housingSpace);
						}
						else
						{
							LogicBuildingData allianceCastleData = LogicDataTables.GetBuildingByName("Alliance Castle", null);

							totalMaxHousing = allianceCastleData.GetUnitStorageCapacity(allianceCastleData.GetMaxUpgradeLevelForTownHallLevel(townHallLevel));
							unitCount = totalMaxHousing / housingSpace;
						}
					}

					int eventCounter = LogicMath.Max(1, unitCount) << 16;

					homeOwnerAvatar.SetCommodityCount(6, data, eventCounter);
					homeOwnerAvatar.GetChangeListener().CommodityCountChanged(6, data, eventCounter);

					Debugger.HudPrint("EVENT: Use troop/spell event started!");
				}
			}
		}

		public LogicCalendarBuildingDestroyedSpawnUnit GetBuildingDestroyedSpawnUnit()
			=> m_buildingDestroyedSpawnUnit;

		public LogicArrayList<LogicCalendarUseTroop> GetUseTroops()
			=> m_useTroops;

		public LogicArrayList<LogicCalendarFunction> GetFunctions()
			=> m_functions;
	}
}