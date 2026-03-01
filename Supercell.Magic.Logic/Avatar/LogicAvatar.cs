using Supercell.Magic.Logic.Avatar.Change;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Avatar
{
	public abstract class LogicAvatar
	{
		protected LogicAvatarChangeListener m_listener;

		protected int m_townHallLevel;
		protected int m_townHallLevelVillage2;
		protected int m_allianceCastleLevel;
		protected int m_allianceCastleTotalCapacity;
		protected int m_allianceCastleUsedCapacity;
		protected int m_allianceCastleTotalSpellCapacity;
		protected int m_allianceCastleUsedSpellCapacity;
		protected int m_allianceUnitVisitCapacity;
		protected int m_allianceUnitSpellVisitCapacity;
		protected int m_allianceBadgeId;
		protected int m_leagueType;
		protected int m_redPackageState;

		protected LogicLevel m_level;

		protected LogicArrayList<LogicDataSlot> m_resourceCount;
		protected LogicArrayList<LogicDataSlot> m_resourceCap;
		protected LogicArrayList<LogicDataSlot> m_unitCount;
		protected LogicArrayList<LogicDataSlot> m_spellCount;
		protected LogicArrayList<LogicDataSlot> m_unitUpgrade;
		protected LogicArrayList<LogicDataSlot> m_spellUpgrade;
		protected LogicArrayList<LogicDataSlot> m_heroUpgrade;
		protected LogicArrayList<LogicDataSlot> m_heroHealth;
		protected LogicArrayList<LogicDataSlot> m_heroState;
		protected LogicArrayList<LogicDataSlot> m_heroMode;
		protected LogicArrayList<LogicDataSlot> m_unitCountVillage2;
		protected LogicArrayList<LogicDataSlot> m_unitCountNewVillage2;
		protected LogicArrayList<LogicDataSlot> m_achievementProgress;
		protected LogicArrayList<LogicDataSlot> m_lootedNpcGold;
		protected LogicArrayList<LogicDataSlot> m_lootedNpcElixir;
		protected LogicArrayList<LogicDataSlot> m_npcStars;
		protected LogicArrayList<LogicDataSlot> m_variables;
		protected LogicArrayList<LogicDataSlot> m_unitPreset1;
		protected LogicArrayList<LogicDataSlot> m_unitPreset2;
		protected LogicArrayList<LogicDataSlot> m_unitPreset3;
		protected LogicArrayList<LogicDataSlot> m_previousArmy;
		protected LogicArrayList<LogicDataSlot> m_eventUnitCounter;
		protected LogicArrayList<LogicDataSlot> m_freeActionCount;

		protected LogicArrayList<LogicUnitSlot> m_allianceUnitCount;

		protected LogicArrayList<LogicData> m_achievementRewardClaimed;
		protected LogicArrayList<LogicData> m_missionCompleted;

		public LogicAvatar()
		{
			m_allianceCastleLevel = -1;
		}

		public virtual void InitBase()
		{
			m_listener = new LogicAvatarChangeListener();
			m_resourceCount = new LogicArrayList<LogicDataSlot>();
			m_resourceCap = new LogicArrayList<LogicDataSlot>();
			m_unitCount = new LogicArrayList<LogicDataSlot>();
			m_spellCount = new LogicArrayList<LogicDataSlot>();
			m_unitUpgrade = new LogicArrayList<LogicDataSlot>();
			m_spellUpgrade = new LogicArrayList<LogicDataSlot>();
			m_heroUpgrade = new LogicArrayList<LogicDataSlot>();
			m_heroHealth = new LogicArrayList<LogicDataSlot>();
			m_heroState = new LogicArrayList<LogicDataSlot>();
			m_heroMode = new LogicArrayList<LogicDataSlot>();
			m_unitCountVillage2 = new LogicArrayList<LogicDataSlot>();
			m_unitCountNewVillage2 = new LogicArrayList<LogicDataSlot>();
			m_achievementProgress = new LogicArrayList<LogicDataSlot>();
			m_lootedNpcGold = new LogicArrayList<LogicDataSlot>();
			m_lootedNpcElixir = new LogicArrayList<LogicDataSlot>();
			m_npcStars = new LogicArrayList<LogicDataSlot>();
			m_variables = new LogicArrayList<LogicDataSlot>();
			m_unitPreset1 = new LogicArrayList<LogicDataSlot>();
			m_unitPreset2 = new LogicArrayList<LogicDataSlot>();
			m_unitPreset3 = new LogicArrayList<LogicDataSlot>();
			m_previousArmy = new LogicArrayList<LogicDataSlot>();
			m_eventUnitCounter = new LogicArrayList<LogicDataSlot>();
			m_allianceUnitCount = new LogicArrayList<LogicUnitSlot>();
			m_achievementRewardClaimed = new LogicArrayList<LogicData>();
			m_missionCompleted = new LogicArrayList<LogicData>();
			m_freeActionCount = new LogicArrayList<LogicDataSlot>();
		}

		public virtual void Destruct()
		{
			if (m_listener != null)
			{
				m_listener.Destruct();
				m_listener = null;
			}

			if (m_resourceCap != null)
			{
				ClearDataSlotArray(m_resourceCap);
				m_resourceCap = null;
			}

			if (m_unitCount != null)
			{
				ClearDataSlotArray(m_unitCount);
				m_unitCount = null;
			}

			if (m_spellCount != null)
			{
				ClearDataSlotArray(m_spellCount);
				m_spellCount = null;
			}

			if (m_unitUpgrade != null)
			{
				ClearDataSlotArray(m_unitUpgrade);
				m_unitUpgrade = null;
			}

			if (m_spellUpgrade != null)
			{
				ClearDataSlotArray(m_spellUpgrade);
				m_spellUpgrade = null;
			}

			if (m_heroUpgrade != null)
			{
				ClearDataSlotArray(m_heroUpgrade);
				m_heroUpgrade = null;
			}

			if (m_heroHealth != null)
			{
				ClearDataSlotArray(m_heroHealth);
				m_heroHealth = null;
			}

			if (m_heroState != null)
			{
				ClearDataSlotArray(m_heroState);
				m_heroState = null;
			}

			if (m_allianceUnitCount != null)
			{
				ClearUnitSlotArray(m_allianceUnitCount);
				m_allianceUnitCount = null;
			}

			if (m_achievementProgress != null)
			{
				ClearDataSlotArray(m_achievementProgress);
				m_achievementProgress = null;
			}

			if (m_npcStars != null)
			{
				ClearDataSlotArray(m_npcStars);
				m_npcStars = null;
			}

			if (m_lootedNpcGold != null)
			{
				ClearDataSlotArray(m_lootedNpcGold);
				m_lootedNpcGold = null;
			}

			if (m_lootedNpcElixir != null)
			{
				ClearDataSlotArray(m_lootedNpcElixir);
				m_lootedNpcElixir = null;
			}

			if (m_heroMode != null)
			{
				ClearDataSlotArray(m_heroMode);
				m_heroMode = null;
			}

			if (m_variables != null)
			{
				ClearDataSlotArray(m_variables);
				m_variables = null;
			}

			if (m_unitPreset1 != null)
			{
				ClearDataSlotArray(m_unitPreset1);
				m_unitPreset1 = null;
			}

			if (m_unitPreset2 != null)
			{
				ClearDataSlotArray(m_unitPreset2);
				m_unitPreset2 = null;
			}

			if (m_unitPreset3 != null)
			{
				ClearDataSlotArray(m_unitPreset3);
				m_unitPreset3 = null;
			}

			if (m_previousArmy != null)
			{
				ClearDataSlotArray(m_previousArmy);
				m_previousArmy = null;
			}

			if (m_eventUnitCounter != null)
			{
				ClearDataSlotArray(m_eventUnitCounter);
				m_eventUnitCounter = null;
			}

			if (m_unitCountVillage2 != null)
			{
				ClearDataSlotArray(m_unitCountVillage2);
				m_unitCountVillage2 = null;
			}

			if (m_unitCountNewVillage2 != null)
			{
				ClearDataSlotArray(m_unitCountNewVillage2);
				m_unitCountNewVillage2 = null;
			}
		}

		public LogicAvatarChangeListener GetChangeListener()
			=> m_listener;

		public void SetChangeListener(LogicAvatarChangeListener listener)
		{
			m_listener = listener;
		}

		public void SetLevel(LogicLevel level)
		{
			m_level = level;
		}

		public virtual bool IsClientAvatar()
			=> false;

		public virtual bool IsNpcAvatar()
			=> false;

		public virtual void GetChecksum(ChecksumHelper checksumHelper)
		{
			checksumHelper.StartObject("LogicAvatar");
			checksumHelper.StartArray("m_pResourceCount");

			for (int i = 0; i < m_resourceCount.Size(); i++)
			{
				m_resourceCount[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pResourceCap");

			for (int i = 0; i < m_resourceCap.Size(); i++)
			{
				m_resourceCap[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pUnitCount");

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				m_unitCount[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pSpellCount");

			for (int i = 0; i < m_spellCount.Size(); i++)
			{
				m_spellCount[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pAllianceUnitCount");

			for (int i = 0; i < m_allianceUnitCount.Size(); i++)
			{
				m_allianceUnitCount[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pUnitUpgrade");

			for (int i = 0; i < m_unitUpgrade.Size(); i++)
			{
				m_unitUpgrade[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pSpellUpgrade");

			for (int i = 0; i < m_spellUpgrade.Size(); i++)
			{
				m_spellUpgrade[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.StartArray("m_pUnitCountVillage2");

			for (int i = 0; i < m_unitCountVillage2.Size(); i++)
			{
				m_unitCountVillage2[i].GetChecksum(checksumHelper);
			}

			checksumHelper.EndArray();
			checksumHelper.WriteValue("m_townHallLevel", m_townHallLevel);
			checksumHelper.WriteValue("m_townHallLevelVillage2", m_townHallLevelVillage2);
			checksumHelper.EndObject();
		}

		public void CommodityCountChangeHelper(int commodityType, LogicData data, int count)
		{
			switch (data.GetDataType())
			{
				case DataType.RESOURCE:
					{
						switch (commodityType)
						{
							case 0:
								{
									int resourceCount = GetResourceCount((LogicResourceData)data);
									int newResourceCount = LogicMath.Max(resourceCount + count, 0);

									if (count > 0)
									{
										int resourceCap = GetResourceCap((LogicResourceData)data);

										if (newResourceCount > resourceCap)
										{
											newResourceCount = resourceCap;
										}

										if (resourceCount >= resourceCap)
										{
											newResourceCount = resourceCap;
										}
									}

									SetResourceCount((LogicResourceData)data, newResourceCount);
									m_listener.CommodityCountChanged(0, data, newResourceCount);
									break;
								}
							case 1:
								{
									int newCount = GetResourceCap((LogicResourceData)data) + count;

									SetResourceCap((LogicResourceData)data, newCount);
									m_listener.CommodityCountChanged(1, data, newCount);
									break;
								}
						}

						break;
					}

				case DataType.CHARACTER:
				case DataType.SPELL:
					switch (commodityType)
					{
						case 0:
							{
								int newCount = LogicMath.Max(GetUnitCount((LogicCombatItemData)data) + count, 0);

								SetUnitCount((LogicCombatItemData)data, newCount);
								m_listener.CommodityCountChanged(0, data, newCount);
								break;
							}
						case 1:
							{
								LogicCombatItemData combatItemData = (LogicCombatItemData)data;

								int newCount = LogicMath.Clamp(GetUnitUpgradeLevel((LogicCombatItemData)data) + count, 0, combatItemData.GetUpgradeLevelCount() - 1);

								SetUnitUpgradeLevel((LogicCombatItemData)data, newCount);
								m_listener.CommodityCountChanged(1, data, newCount);
								break;
							}
						case 7:
							{
								int newCount = LogicMath.Max(GetUnitCountVillage2((LogicCombatItemData)data) + count, 0);

								SetUnitCountVillage2((LogicCombatItemData)data, newCount);
								m_listener.CommodityCountChanged(7, data, newCount);
								break;
							}
						case 8:
							{
								int newCount = LogicMath.Max(GetUnitCountNewVillage2((LogicCombatItemData)data) + count, 0);

								SetUnitCountNewVillage2((LogicCombatItemData)data, newCount);
								m_listener.CommodityCountChanged(8, data, newCount);
								break;
							}
						case 9:
							{
								int newCount = LogicMath.Max(GetFreeActionCount(data) + count, 0);

								SetFreeActionCount(data, newCount);
								m_listener.CommodityCountChanged(9, data, newCount);
								break;
							}
					}

					break;

				case DataType.NPC:
					switch (commodityType)
					{
						case 0:
							{
								int newCount = GetNpcStars((LogicNpcData)data) + count;

								SetNpcStars((LogicNpcData)data, newCount);
								m_listener.CommodityCountChanged(0, data, newCount);
								break;
							}
						case 1:
							{
								int newCount = GetLootedNpcGold((LogicNpcData)data) + count;

								SetLootedNpcGold((LogicNpcData)data, newCount);
								m_listener.CommodityCountChanged(1, data, newCount);
								break;
							}
						case 2:
							{
								int newCount = GetLootedNpcElixir((LogicNpcData)data) + count;

								SetLootedNpcElixir((LogicNpcData)data, newCount);
								m_listener.CommodityCountChanged(2, data, newCount);
								break;
							}
					}

					break;
				case DataType.ACHIEVEMENT:
					int tmp = LogicMath.Max(GetAchievementProgress((LogicAchievementData)data) + count, 0);

					SetAchievementProgress((LogicAchievementData)data, tmp);
					m_listener.CommodityCountChanged(0, data, tmp);
					break;
				case DataType.HERO:
					LogicHeroData heroData = (LogicHeroData)data;

					switch (commodityType)
					{
						case 0:
							{
								int newCount = LogicMath.Clamp(GetHeroHealth(heroData) + count, 0, heroData.GetFullRegenerationTimeSec(GetUnitUpgradeLevel(heroData)));

								SetHeroHealth(heroData, newCount);
								GetChangeListener().CommodityCountChanged(0, data, newCount);
								break;
							}
						case 1:
							{
								int newCount = LogicMath.Clamp(GetUnitUpgradeLevel(heroData) + count, 0, heroData.GetUpgradeLevelCount() - 1);

								SetUnitUpgradeLevel((LogicCombatItemData)data, newCount);
								m_listener.CommodityCountChanged(1, data, newCount);
								break;
							}
					}

					break;
			}
		}

		public void SetCommodityCount(int commodityType, LogicData data, int count)
		{
			switch (data.GetDataType())
			{
				case DataType.RESOURCE:
					switch (commodityType)
					{
						case 0:
							SetResourceCount((LogicResourceData)data, count);
							break;
						case 1:
							SetResourceCap((LogicResourceData)data, count);
							break;
						default:
							Debugger.Error("setCommodityCount - Unknown resource commodity");
							break;
					}

					break;
				case DataType.CHARACTER:
				case DataType.SPELL:
					switch (commodityType)
					{
						case 0:
							SetUnitCount((LogicCombatItemData)data, count);
							break;
						case 1:
							SetUnitUpgradeLevel((LogicCombatItemData)data, count);
							break;
						case 2:
							SetUnitPresetCount((LogicCombatItemData)data, 0, count);
							break;
						case 3:
							SetUnitPresetCount((LogicCombatItemData)data, 1, count);
							break;
						case 4:
							SetUnitPresetCount((LogicCombatItemData)data, 2, count);
							break;
						case 5:
							SetUnitPresetCount((LogicCombatItemData)data, 3, count);
							break;
						case 6:
							SetEventUnitCounterCount((LogicCombatItemData)data, count);
							break;
						case 7:
							SetUnitCountVillage2((LogicCombatItemData)data, count);
							break;
						case 8:
							SetUnitCountNewVillage2((LogicCombatItemData)data, count);
							break;
						case 9:
							SetFreeActionCount(data, count);
							break;
						default:
							Debugger.Error("setCommodityCount - Unknown resource commodity");
							break;
					}

					break;
				case DataType.NPC:
					switch (commodityType)
					{
						case 0:
							SetNpcStars((LogicNpcData)data, count);
							break;
						case 1:
							SetLootedNpcGold((LogicNpcData)data, count);
							break;
						case 2:
							SetLootedNpcElixir((LogicNpcData)data, count);
							break;
					}

					break;
				case DataType.MISSION:
					if (commodityType == 0)
					{
						SetMissionCompleted((LogicMissionData)data, count != 0);
					}

					break;
				case DataType.ACHIEVEMENT:
					switch (commodityType)
					{
						case 0:
							SetAchievementProgress((LogicAchievementData)data, count);
							break;
						case 1:
							SetAchievementRewardClaimed((LogicAchievementData)data, count != 0);
							break;
					}

					break;
				case DataType.HERO:
					switch (commodityType)
					{
						case 0:
							SetHeroHealth((LogicHeroData)data, count);
							break;
						case 1:
							SetUnitUpgradeLevel((LogicHeroData)data, count);
							break;
						case 2:
							SetHeroState((LogicHeroData)data, count);
							break;
						case 3:
							SetHeroMode((LogicHeroData)data, count);
							break;
					}

					break;
				case DataType.VARIABLE:
					if (commodityType == 0)
					{
						SetVariable(data, count);
					}

					break;
			}
		}

		public void ClearDataSlotArray(LogicArrayList<LogicDataSlot> dataSlotArray)
		{
			for (int i = 0; i < dataSlotArray.Size(); i++)
				dataSlotArray[i].Destruct();
			dataSlotArray.Clear();
		}

		public void ClearUnitSlotArray(LogicArrayList<LogicUnitSlot> unitSlotArray)
		{
			for (int i = 0; i < unitSlotArray.Size(); i++)
				unitSlotArray[i].Destruct();
			unitSlotArray.Clear();
		}

		public virtual bool AddDuelReward(int goldCount, int elixirCount, int bonusGoldCount, int bonusElixirCount, LogicLong matchId)
			=> false;

		public virtual bool AddStarBonusReward(int goldCount, int elixirCount, int darkElixirCount)
			=> false;

		public virtual bool AddWarReward(int gold, int elixir, int darkElixir, int unk, LogicLong warInstanceId)
			=> false;

		public virtual void UpdateLootLimitCooldown()
		{
			// UpdateLootLimitCooldown.
		}

		public virtual void UpdateStarBonusLimitCooldown()
		{
			// UpdateStarBonusLimitCooldown.
		}

		public virtual void FastForwardLootLimit(int secs)
		{
			// FastForwardLootLimit.
		}

		public virtual void FastForwardStarBonusLimit(int secs)
		{
			// FastForwardStarBonusLimit.
		}

		public virtual bool IsInAlliance()
			=> false;

		public virtual LogicLong GetAllianceId()
			=> 0;

		public virtual int GetAllianceBadgeId()
			=> 0;

		public virtual LogicAvatarAllianceRole GetAllianceRole()
			=> LogicAvatarAllianceRole.MEMBER;

		public virtual string GetAllianceName()
			=> null;

		public virtual int GetExpLevel()
			=> 1;


		public virtual string GetName()
			=> null;

		public bool IsInExpLevelCap()
			=> GetExpLevel() >= LogicDataTables.GetExperienceLevelCount();

		public bool IsHeroAvailableForAttack(LogicHeroData data)
		{
			if ((GetHeroState(data) & 0xFFFFFFFE) == 2)
			{
				int heroUpgLevel = GetUnitUpgradeLevel(data);
				int heroHealth = data.GetHeroHitpoints(GetHeroHealth(data), heroUpgLevel);

				return data.HasEnoughHealthForAttack(heroHealth, heroUpgLevel);
			}

			return false;
		}

		public int GetHeroHealth(LogicHeroData data)
		{
			int index = -1;

			for (int i = 0; i < m_heroHealth.Size(); i++)
			{
				if (m_heroHealth[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_heroHealth[index].GetCount();
			}

			return 0;
		}

		public void SetHeroHealth(LogicHeroData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_heroHealth.Size(); i++)
			{
				if (m_heroHealth[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_heroHealth[index].SetCount(count);
			}
			else
			{
				m_heroHealth.Add(new LogicDataSlot(data, count));
			}
		}

		public bool FastForwardHeroHealth(LogicHeroData data, int secs)
		{
			int totalSecs = LogicMath.Max(0, secs);
			int health = GetHeroHealth(data);

			if (health != 0)
			{
				health = LogicMath.Max(0, health - totalSecs);

				SetHeroHealth(data, health);
				GetChangeListener().CommodityCountChanged(0, data, health);

				return health == 0;
			}

			return false;
		}

		public int GetFreeActionCount(LogicData data)
		{
			for (int i = 0; i < m_freeActionCount.Size(); i++)
			{
				if (m_freeActionCount[i].GetData() == data)
					return m_freeActionCount[i].GetCount();
			}

			return 0;
		}

		public void SetFreeActionCount(LogicData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_achievementProgress.Size(); i++)
			{
				if (m_achievementProgress[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_freeActionCount[index].SetCount(count);
			}
			else
			{
				m_freeActionCount.Add(new LogicDataSlot(data, count));
			}
		}

		public void SetFreeUnitCount(LogicCombatItemData data, int count, LogicLevel level)
		{
			int cap = data.GetCombatItemType() == 2
				? LogicDataTables.GetGlobals().GetFreeHeroHealthCap()
				: data.GetHousingSpace() + 2 * level.GetComponentManagerAt(data.GetVillageType()).GetTotalMaxHousing(data.GetCombatItemType()) *
				  LogicDataTables.GetGlobals().GetFreeUnitHousingCapPercentage() / (2 * data.GetHousingSpace());
			int currentCount = GetFreeActionCount(data);
			int newCount = LogicMath.Clamp(currentCount + count, 0, cap);

			if (newCount != currentCount)
			{
				SetFreeActionCount(data, newCount);
				m_listener.CommodityCountChanged(9, data, newCount);
			}
		}

		public int GetHeroState(LogicHeroData data)
		{
			int index = -1;

			for (int i = 0; i < m_heroState.Size(); i++)
			{
				if (m_heroState[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_heroState[index].GetCount();
			}

			return 0;
		}

		public void SetHeroState(LogicHeroData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_heroState.Size(); i++)
			{
				if (m_heroState[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_heroState[index].SetCount(count);
			}
			else
			{
				m_heroState.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetHeroMode(LogicHeroData data)
		{
			int index = -1;

			for (int i = 0; i < m_heroMode.Size(); i++)
			{
				if (m_heroMode[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_heroMode[index].GetCount();
			}

			return 0;
		}

		public void SetHeroMode(LogicHeroData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_heroMode.Size(); i++)
			{
				if (m_heroMode[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_heroMode[index].SetCount(count);
			}
			else
			{
				m_heroMode.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetHeroHealCost(LogicHeroData data)
			=> LogicGamePlayUtil.GetSpeedUpCost(GetHeroHealth(data), 2, data.GetVillageType());

		public int GetVariable(LogicData data)
		{
			int index = -1;

			for (int i = 0; i < m_variables.Size(); i++)
			{
				if (m_variables[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_variables[index].GetCount();
			}

			return 0;
		}

		public void SetVariable(LogicData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_variables.Size(); i++)
			{
				if (m_variables[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_variables[index].SetCount(count);
			}
			else
			{
				m_variables.Add(new LogicDataSlot(data, count));
			}

			m_listener.CommodityCountChanged(0, data, count);
		}

		public int GetVariableByName(string name)
		{
			LogicData data = LogicDataTables.GetVariableByName(name, null);

			if (data == null)
			{
				Debugger.Error("getVariableByName() Invalid Name " + name);
			}

			return GetVariable(data);
		}

		public void SetVariableByName(string name, int value)
		{
			LogicData data = LogicDataTables.GetVariableByName(name, null);

			if (data == null)
			{
				Debugger.Error("setVariableByName() Invalid Name " + name);
			}

			SetVariable(data, value);
		}

		public int GetVillageToGoTo()
			=> GetVariableByName("VillageToGoTo");

		public void SetAccountBound()
		{
			SetVariableByName("AccountBound", 1);
		}

		public int GetVillage2BarrackLevel()
			=> GetVariableByName("Village2BarrackLevel");

		public void SetVillage2BarrackLevel(int value)
		{
			SetVariableByName("Village2BarrackLevel", value);
		}

		public bool IsChallengeStarted()
			=> GetVariableByName("ChallengeStarted") != 0;

		public int GetUnusedResourceCap(LogicResourceData data)
			=> LogicMath.Max(GetResourceCap(data) - GetResourceCount(data), 0);

		public bool IsAccountBound()
			=> GetVariableByName("AccountBound") != 0;

		public int GetSecondsSinceLastFriendListOpened()
			=> m_level.GetGameMode().GetStartTime() + m_level.GetLogicTime().GetTotalMS() / 1000 - GetVariableByName("FriendListLastOpened");

		public void UpdateLastFriendListOpened()
		{
			SetVariableByName("FriendListLastOpened", m_level.GetGameMode().GetStartTime() + m_level.GetLogicTime().GetTotalMS() / 1000);
		}

		public int GetResourceCap(LogicResourceData data)
		{
			if (data.IsPremiumCurrency())
			{
				Debugger.Warning("LogicClientAvatar::getResourceCap shouldn't be used for diamonds");
			}
			else
			{
				int index = -1;

				for (int i = 0; i < m_resourceCap.Size(); i++)
				{
					if (m_resourceCap[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_resourceCap[index].GetCount();
				}
			}

			return 0;
		}

		public void SetResourceCap(LogicResourceData data, int count)
		{
			if (data.IsPremiumCurrency())
			{
				Debugger.Warning("LogicClientAvatar::setResourceCap shouldn't be used for diamonds");
			}
			else
			{
				int index = -1;

				for (int i = 0; i < m_resourceCap.Size(); i++)
				{
					if (m_resourceCap[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_resourceCap[index].SetCount(count);
				}
				else
				{
					m_resourceCap.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public LogicArrayList<LogicDataSlot> GetResources()
			=> m_resourceCount;

		public virtual int GetResourceCount(LogicResourceData data)
		{
			if (!data.IsPremiumCurrency())
			{
				int index = -1;

				for (int i = 0; i < m_resourceCount.Size(); i++)
				{
					if (m_resourceCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_resourceCount[index].GetCount();
				}
			}
			else
			{
				Debugger.Warning("LogicAvatar::getResourceCount shouldn't be used for diamonds");
			}

			return 0;
		}

		public void SetResourceCount(LogicResourceData data, int count)
		{
			if (!data.IsPremiumCurrency())
			{
				int index = -1;

				for (int i = 0; i < m_resourceCount.Size(); i++)
				{
					if (m_resourceCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_resourceCount[index].SetCount(count);
				}
				else
				{
					m_resourceCount.Add(new LogicDataSlot(data, count));
				}

				if (m_level != null && m_level.GetState() == 1)
				{
					m_level.GetComponentManagerAt(data.GetVillageType()).DivideAvatarResourcesToStorages();
				}
			}
			else
			{
				Debugger.Warning("LogicAvatar::setResourceCount shouldn't be used for diamonds");
			}
		}

		public bool IsDarkElixirUnlocked()
			=> GetResourceCap(LogicDataTables.GetDarkElixirData()) > 0;

		public int GetDamagingSpellsTotal()
		{
			int cnt = 0;

			LogicDataTable table = LogicDataTables.GetTable(DataType.SPELL);

			for (int i = 0; i < table.GetItemCount(); i++)
			{
				LogicSpellData data = (LogicSpellData)table.GetItemAt(i);

				int idx = -1;

				for (int j = 0; j < m_spellCount.Size(); j++)
				{
					if (m_spellCount[j].GetData() == data)
					{
						idx = j;
						break;
					}
				}

				if (idx != -1 && (data.IsBuildingDamageSpell() || data.GetSummonTroop() != null))
				{
					cnt += m_spellCount[idx].GetCount();
				}
			}

			return cnt;
		}

		public int GetSpellsTotalCapacity()
		{
			int cnt = 0;

			for (int i = 0; i < m_spellCount.Size(); i++)
			{
				LogicDataSlot slot = m_spellCount[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				cnt += data.GetHousingSpace() * slot.GetCount();
			}

			return cnt;
		}

		public int GetUnitsTotalCapacity()
		{
			int cnt = 0;

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				LogicDataSlot slot = m_unitCount[i];
				LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

				cnt += data.GetHousingSpace() * slot.GetCount();
			}

			return cnt;
		}

		public int GetUnitsTotal()
		{
			int count = 0;

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				count += m_unitCount[i].GetCount();
			}

			return count;
		}

		public int GetUnitsTotalVillage2()
		{
			int count = 0;

			for (int i = 0; i < m_unitCountVillage2.Size(); i++)
			{
				count += m_unitCountVillage2[i].GetCount();
			}

			return count;
		}

		public int GetUnitsNewTotalVillage2()
		{
			int count = 0;

			for (int i = 0; i < m_unitCountNewVillage2.Size(); i++)
			{
				count += m_unitCountNewVillage2[i].GetCount();
			}

			return count;
		}

		public LogicArrayList<LogicUnitSlot> GetAllianceUnits()
			=> m_allianceUnitCount;

		public int GetAllianceUnitCount(LogicCombatItemData data, int upgLevel)
		{
			int index = -1;

			for (int i = 0; i < m_allianceUnitCount.Size(); i++)
			{
				if (m_allianceUnitCount[i].GetData() == data && m_allianceUnitCount[i].GetLevel() == upgLevel)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_allianceUnitCount[index].GetCount();
			}

			return 0;
		}

		public void AddAllianceUnit(LogicCombatItemData data, int upgLevel)
		{
			SetAllianceUnitCount(data, upgLevel, GetAllianceUnitCount(data, upgLevel) + 1);
		}

		public void SetAllianceUnitCount(LogicCombatItemData data, int upgLevel, int count)
		{
			int index = -1;

			for (int i = 0; i < m_allianceUnitCount.Size(); i++)
			{
				if (m_allianceUnitCount[i].GetData() == data && m_allianceUnitCount[i].GetLevel() == upgLevel)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
				{
					m_allianceCastleUsedSpellCapacity += (count - m_allianceUnitCount[index].GetCount()) * data.GetHousingSpace();
				}
				else
				{
					m_allianceCastleUsedCapacity += (count - m_allianceUnitCount[index].GetCount()) * data.GetHousingSpace();
				}

				m_allianceUnitCount[index].SetCount(count);
			}
			else
			{
				m_allianceUnitCount.Add(new LogicUnitSlot(data, upgLevel, count));

				if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
				{
					m_allianceCastleUsedSpellCapacity += count * data.GetHousingSpace();
				}
				else
				{
					m_allianceCastleUsedCapacity += count * data.GetHousingSpace();
				}
			}
		}

		public void RemoveAllianceUnit(LogicCombatItemData data, int upgLevel)
		{
			int count = GetAllianceUnitCount(data, upgLevel);

			if (count > 0)
			{
				SetAllianceUnitCount(data, upgLevel, count - 1);
			}
			else
			{
				Debugger.Warning("LogicClientAvatar::removeAllianceUnit called but unit count is already 0");
			}
		}

		public int GetUnitCount(LogicCombatItemData data)
		{
			if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_spellCount.Size(); i++)
				{
					if (m_spellCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_spellCount[index].GetCount();
				}
			}
			else
			{
				int index = -1;

				for (int i = 0; i < m_unitCount.Size(); i++)
				{
					if (m_unitCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_unitCount[index].GetCount();
				}
			}

			return 0;
		}

		public void SetUnitCount(LogicCombatItemData data, int count)
		{
			if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_spellCount.Size(); i++)
				{
					if (m_spellCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_spellCount[index].SetCount(count);
				}
				else
				{
					m_spellCount.Add(new LogicDataSlot(data, count));
				}
			}
			else
			{
				int index = -1;

				for (int i = 0; i < m_unitCount.Size(); i++)
				{
					if (m_unitCount[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_unitCount[index].SetCount(count);
				}
				else
				{
					m_unitCount.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public int GetUnitCountVillage2(LogicCombatItemData data)
		{
			if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_unitCountVillage2.Size(); i++)
				{
					if (m_unitCountVillage2[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_unitCountVillage2[index].GetCount();
				}
			}

			return 0;
		}

		public void SetUnitCountVillage2(LogicCombatItemData data, int count)
		{
			if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_unitCountVillage2.Size(); i++)
				{
					if (m_unitCountVillage2[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_unitCountVillage2[index].SetCount(count);
				}
				else
				{
					m_unitCountVillage2.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public int GetUnitCountNewVillage2(LogicCombatItemData data)
		{
			if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_unitCountNewVillage2.Size(); i++)
				{
					if (m_unitCountNewVillage2[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					return m_unitCountNewVillage2[index].GetCount();
				}
			}

			return 0;
		}

		public void SetUnitCountNewVillage2(LogicCombatItemData data, int count)
		{
			if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
			{
				int index = -1;

				for (int i = 0; i < m_unitCountNewVillage2.Size(); i++)
				{
					if (m_unitCountNewVillage2[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_unitCountNewVillage2[index].SetCount(count);
				}
				else
				{
					m_unitCountNewVillage2.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public int GetUnitUpgradeLevel(LogicCombatItemData data)
		{
			if (!data.UseUpgradeLevelByTownHall())
			{
				if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
				{
					int index = -1;

					for (int i = 0; i < m_unitUpgrade.Size(); i++)
					{
						if (m_unitUpgrade[i].GetData() == data)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						return m_unitUpgrade[index].GetCount();
					}
				}
				else if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_SPELL)
				{
					int index = -1;

					for (int i = 0; i < m_spellUpgrade.Size(); i++)
					{
						if (m_spellUpgrade[i].GetData() == data)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						return m_spellUpgrade[index].GetCount();
					}
				}
				else
				{
					int index = -1;

					for (int i = 0; i < m_heroUpgrade.Size(); i++)
					{
						if (m_heroUpgrade[i].GetData() == data)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						return m_heroUpgrade[index].GetCount();
					}
				}

				return 0;
			}

			return data.GetUpgradeLevelByTownHall(data.GetVillageType() == 1 ? m_townHallLevelVillage2 : m_townHallLevel);
		}

		public void SetUnitUpgradeLevel(LogicCombatItemData data, int count)
		{
			int combatItemType = data.GetCombatItemType();
			int upgradeCount = data.GetUpgradeLevelCount();

			if (combatItemType > 0)
			{
				if (combatItemType == 2)
				{
					if (upgradeCount <= count)
					{
						Debugger.Warning("LogicAvatar::setUnitUpgradeLevel - Level is out of bounds!");
						count = upgradeCount - 1;
					}

					int index = -1;

					for (int i = 0; i < m_heroUpgrade.Size(); i++)
					{
						if (m_heroUpgrade[i].GetData() == data)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						m_heroUpgrade[index].SetCount(count);
					}
					else
					{
						m_heroUpgrade.Add(new LogicDataSlot(data, count));
					}
				}
				else
				{
					if (upgradeCount <= count)
					{
						Debugger.Warning("LogicAvatar::setSpellUpgradeLevel - Level is out of bounds!");
						count = upgradeCount - 1;
					}

					int index = -1;

					for (int i = 0; i < m_spellUpgrade.Size(); i++)
					{
						if (m_spellUpgrade[i].GetData() == data)
						{
							index = i;
							break;
						}
					}

					if (index != -1)
					{
						m_spellUpgrade[index].SetCount(count);
					}
					else
					{
						m_spellUpgrade.Add(new LogicDataSlot(data, count));
					}
				}
			}
			else
			{
				if (upgradeCount <= count)
				{
					Debugger.Warning("LogicAvatar::setUnitUpgradeLevel - Level is out of bounds!");
					count = upgradeCount - 1;
				}

				int index = -1;

				for (int i = 0; i < m_unitUpgrade.Size(); i++)
				{
					if (m_unitUpgrade[i].GetData() == data)
					{
						index = i;
						break;
					}
				}

				if (index != -1)
				{
					m_unitUpgrade[index].SetCount(count);
				}
				else
				{
					m_unitUpgrade.Add(new LogicDataSlot(data, count));
				}
			}
		}

		public int GetNpcStars(LogicNpcData data)
		{
			int index = -1;

			for (int i = 0; i < m_npcStars.Size(); i++)
			{
				if (m_npcStars[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_npcStars[index].GetCount();
			}

			return 0;
		}

		public void SetNpcStars(LogicNpcData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_npcStars.Size(); i++)
			{
				if (m_npcStars[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_npcStars[index].SetCount(count);
			}
			else
			{
				m_npcStars.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetTotalNpcStars()
		{
			int cnt = 0;

			for (int i = 0; i < m_npcStars.Size(); i++)
			{
				cnt += m_npcStars[i].GetCount();
			}

			return cnt;
		}

		public int GetLootedNpcGold(LogicNpcData data)
		{
			int index = -1;

			for (int i = 0; i < m_lootedNpcGold.Size(); i++)
			{
				if (m_lootedNpcGold[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_lootedNpcGold[index].GetCount();
			}

			return 0;
		}

		public void SetLootedNpcGold(LogicNpcData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_lootedNpcGold.Size(); i++)
			{
				if (m_lootedNpcGold[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_lootedNpcGold[index].SetCount(count);
			}
			else
			{
				m_lootedNpcGold.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetLootedNpcElixir(LogicNpcData data)
		{
			int index = -1;

			for (int i = 0; i < m_lootedNpcElixir.Size(); i++)
			{
				if (m_lootedNpcElixir[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_lootedNpcElixir[index].GetCount();
			}

			return 0;
		}

		public void SetLootedNpcElixir(LogicNpcData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_lootedNpcElixir.Size(); i++)
			{
				if (m_lootedNpcElixir[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_lootedNpcElixir[index].SetCount(count);
			}
			else
			{
				m_lootedNpcElixir.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetUnitPresetCount(LogicCombatItemData data, int type)
		{
			LogicArrayList<LogicDataSlot> slots = null;

			switch (type)
			{
				case 0:
					slots = m_previousArmy;
					break;
				case 1:
					slots = m_unitPreset1;
					break;
				case 2:
					slots = m_unitPreset2;
					break;
				case 3:
					slots = m_unitPreset3;
					break;
			}

			int index = -1;

			for (int i = 0; i < slots.Size(); i++)
			{
				if (slots[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return slots[index].GetCount();
			}

			return 0;
		}

		public void SetUnitPresetCount(LogicCombatItemData data, int type, int count)
		{
			LogicArrayList<LogicDataSlot> slots = null;

			switch (type)
			{
				case 0:
					slots = m_previousArmy;
					break;
				case 1:
					slots = m_unitPreset1;
					break;
				case 2:
					slots = m_unitPreset2;
					break;
				case 3:
					slots = m_unitPreset3;
					break;
			}

			int index = -1;

			for (int i = 0; i < slots.Size(); i++)
			{
				if (slots[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				slots[index].SetCount(count);
			}
			else
			{
				slots.Add(new LogicDataSlot(data, count));
			}
		}

		public int GetEventUnitCounterCount(LogicCombatItemData data)
		{
			int index = -1;

			for (int i = 0; i < m_eventUnitCounter.Size(); i++)
			{
				if (m_eventUnitCounter[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_eventUnitCounter[index].GetCount();
			}

			return 0;
		}

		public void SetEventUnitCounterCount(LogicCombatItemData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_eventUnitCounter.Size(); i++)
			{
				if (m_eventUnitCounter[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_eventUnitCounter[index].SetCount(count);
			}
			else
			{
				m_eventUnitCounter.Add(new LogicDataSlot(data, count));
			}
		}

		public virtual void XpGainHelper(int count)
		{
			// XpGainHelper.
		}

		public LogicArrayList<LogicDataSlot> GetUnits()
			=> m_unitCount;

		public LogicArrayList<LogicDataSlot> GetUnitUpgradeLevels()
			=> m_unitUpgrade;

		public LogicArrayList<LogicDataSlot> GetUnitsVillage2()
			=> m_unitCountVillage2;

		public LogicArrayList<LogicDataSlot> GetUnitsNewVillage2()
			=> m_unitCountNewVillage2;

		public LogicArrayList<LogicDataSlot> GetSpells()
			=> m_spellCount;

		public LogicArrayList<LogicDataSlot> GetSpellUpgradeLevels()
			=> m_spellUpgrade;

		public LogicArrayList<LogicDataSlot> GetResourceCaps()
			=> m_resourceCap;

		public void SetMissionCompleted(LogicMissionData data, bool state)
		{
			int index = -1;

			for (int i = 0; i < m_missionCompleted.Size(); i++)
			{
				if (m_missionCompleted[i] == data)
				{
					index = i;
					break;
				}
			}

			if (state)
			{
				if (index == -1)
				{
					m_missionCompleted.Add(data);
				}
			}
			else
			{
				if (index != -1)
				{
					m_missionCompleted.Remove(index);
				}
			}
		}

		public bool IsMissionCompleted(LogicMissionData data)
		{
			int index = -1;

			for (int i = 0; i < m_missionCompleted.Size(); i++)
			{
				if (m_missionCompleted[i] == data)
				{
					index = i;
					break;
				}
			}

			return index != -1;
		}

		public int GetAchievementProgress(LogicAchievementData data)
		{
			int index = -1;

			for (int i = 0; i < m_achievementProgress.Size(); i++)
			{
				if (m_achievementProgress[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return m_achievementProgress[index].GetCount();
			}

			return 0;
		}

		public void SetAchievementProgress(LogicAchievementData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_achievementProgress.Size(); i++)
			{
				if (m_achievementProgress[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_achievementProgress[index].SetCount(count);
			}
			else
			{
				m_achievementProgress.Add(new LogicDataSlot(data, count));
			}
		}

		public void SetAchievementRewardClaimed(LogicAchievementData data, bool claimed)
		{
			int index = -1;

			for (int i = 0; i < m_achievementRewardClaimed.Size(); i++)
			{
				if (m_achievementRewardClaimed[i] == data)
				{
					index = i;
					break;
				}
			}

			if (claimed)
			{
				if (index == -1)
				{
					m_achievementRewardClaimed.Add(data);
				}
			}
			else
			{
				if (index != -1)
				{
					m_achievementRewardClaimed.Remove(index);
				}
			}
		}

		public bool IsAchievementRewardClaimed(LogicAchievementData data)
		{
			int index = -1;

			for (int i = 0; i < m_achievementRewardClaimed.Size(); i++)
			{
				if (m_achievementRewardClaimed[i] == data)
				{
					index = i;
					break;
				}
			}

			return index != -1;
		}

		public bool IsAchievementCompleted(LogicAchievementData data)
		{
			int index = -1;
			int progressCount = 0;

			for (int i = 0; i < m_achievementProgress.Size(); i++)
			{
				if (m_achievementProgress[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				progressCount = m_achievementProgress[index].GetCount();
			}

			return progressCount >= data.GetActionCount();
		}

		public int GetTownHallLevel()
			=> m_townHallLevel;

		public void SetTownHallLevel(int level)
		{
			m_townHallLevel = level;
		}

		public int GetVillage2TownHallLevel()
			=> m_townHallLevelVillage2;

		public void SetVillage2TownHallLevel(int level)
		{
			m_townHallLevelVillage2 = level;
		}

		public int GetAllianceCastleLevel()
			=> m_allianceCastleLevel;

		public void SetAllianceCastleLevel(int level)
		{
			m_allianceCastleLevel = level;

			if (m_allianceCastleLevel == -1)
			{
				m_allianceCastleTotalCapacity = 0;
				m_allianceCastleTotalSpellCapacity = 0;
			}
			else
			{
				LogicBuildingData allianceCastleData = LogicDataTables.GetAllianceCastleData();

				m_allianceCastleTotalCapacity = allianceCastleData.GetUnitStorageCapacity(level);
				m_allianceCastleTotalSpellCapacity = allianceCastleData.GetAltUnitStorageCapacity(level);
			}
		}

		public int GetRedPackageState()
			=> m_redPackageState;

		public int GetRedPackageCount()
		{
			int count = 0;

			for (int i = 4; i < 17; i *= 2)
			{
				if ((m_redPackageState & i) != 0)
					count += 1;
			}

			return count;
		}

		public void SetRedPackageState(int state)
		{
			if (m_redPackageState != state)
			{
				m_redPackageState = state;
				m_listener.REDPackageStateChanged(state);
			}
		}

		public void ResetRedPackageState()
		{
			m_redPackageState &= 252;
		}

		public bool HasAllianceCastle()
			=> m_allianceCastleLevel != -1;

		public int GetAllianceCastleTotalCapacity()
			=> m_allianceCastleTotalCapacity;

		public int GetAllianceCastleTotalSpellCapacity()
			=> m_allianceCastleTotalSpellCapacity;

		public int GetAllianceCastleUsedCapacity()
			=> m_allianceCastleUsedCapacity;

		public int GetAllianceCastleUsedSpellCapacity()
			=> m_allianceCastleUsedSpellCapacity;

		public int GetAllianceCastleFreeCapacity()
			=> m_allianceCastleTotalCapacity - m_allianceCastleUsedCapacity;

		public int GetAllianceCastleFreeSpellCapacity()
			=> m_allianceCastleTotalSpellCapacity - m_allianceCastleUsedSpellCapacity;

		public int GetAttackStrength(int villageType)
		{
			LogicComponentManager componentManager = m_level.GetComponentManagerAt(villageType);
			LogicArrayList<LogicHeroData> unlockedHeroData = new LogicArrayList<LogicHeroData>();
			LogicArrayList<LogicCharacterData> unlockedCharacterData = new LogicArrayList<LogicCharacterData>();
			LogicArrayList<LogicSpellData> unlockedSpellData = new LogicArrayList<LogicSpellData>();

			LogicDataTable heroTable = LogicDataTables.GetTable(DataType.HERO);
			LogicDataTable characterTable = LogicDataTables.GetTable(DataType.CHARACTER);
			LogicDataTable spellTable = LogicDataTables.GetTable(DataType.SPELL);

			int maxBarrackLevel = componentManager.GetMaxBarrackLevel();
			int maxDarkBarrackLevel = componentManager.GetMaxDarkBarrackLevel();
			int maxSpellForgeLevel = componentManager.GetMaxSpellForgeLevel();
			int maxMiniSpellForgeLevel = componentManager.GetMaxMiniSpellForgeLevel();

			for (int i = 0; i < heroTable.GetItemCount(); i++)
			{
				LogicHeroData heroData = (LogicHeroData)heroTable.GetItemAt(i);

				if (componentManager.IsHeroUnlocked(heroData) && heroData.IsProductionEnabled() && heroData.GetVillageType() == villageType)
				{
					unlockedHeroData.Add(heroData);
				}
			}

			for (int i = 0; i < characterTable.GetItemCount(); i++)
			{
				LogicCharacterData characterData = (LogicCharacterData)characterTable.GetItemAt(i);

				if (characterData.GetVillageType() == villageType &&
					characterData.IsUnlockedForBarrackLevel(characterData.GetUnitOfType() == 1 ? maxBarrackLevel : maxDarkBarrackLevel) &&
					characterData.IsProductionEnabled())
				{
					unlockedCharacterData.Add(characterData);
				}
			}

			for (int i = 0; i < spellTable.GetItemCount(); i++)
			{
				LogicSpellData spellData = (LogicSpellData)spellTable.GetItemAt(i);

				if (spellData.GetVillageType() == villageType &&
					spellData.IsUnlockedForProductionHouseLevel(spellData.GetUnitOfType() == 1 ? maxSpellForgeLevel : maxMiniSpellForgeLevel) &&
					spellData.IsProductionEnabled())
				{
					unlockedSpellData.Add(spellData);
				}
			}

			int[] heroUpgradeLevel = new int[unlockedHeroData.Size()];
			int[] characterUpgradeLevel = new int[unlockedCharacterData.Size()];
			int[] spellUpgradeLevel = new int[unlockedSpellData.Size()];

			for (int i = 0; i < unlockedHeroData.Size(); i++)
			{
				heroUpgradeLevel[i] = GetUnitUpgradeLevel(unlockedHeroData[i]);
			}

			for (int i = 0; i < unlockedCharacterData.Size(); i++)
			{
				characterUpgradeLevel[i] = GetUnitUpgradeLevel(unlockedCharacterData[i]);
			}

			for (int i = 0; i < unlockedSpellData.Size(); i++)
			{
				spellUpgradeLevel[i] = GetUnitUpgradeLevel(unlockedSpellData[i]);
			}

			int totalMaxHousing = componentManager.GetTotalMaxHousing(0);
			int spellForgeCapacity = 0;
			int miniSpellForgeCapacity = 0;

			if (spellForgeCapacity != -1)
			{
				spellForgeCapacity = LogicDataTables.GetBuildingByName("Spell Forge", null).GetUnitStorageCapacity(spellForgeCapacity);
			}

			if (miniSpellForgeCapacity != -1)
			{
				miniSpellForgeCapacity = LogicDataTables.GetBuildingByName("Mini Spell Factory", null).GetUnitStorageCapacity(miniSpellForgeCapacity);
			}

			int castleLevel = villageType == 0 ? m_allianceCastleLevel : -1;

			return (int)LogicStrengthUtil.GetAttackStrength(m_townHallLevel, castleLevel, unlockedHeroData, heroUpgradeLevel, unlockedCharacterData, characterUpgradeLevel,
															 totalMaxHousing, unlockedSpellData, spellUpgradeLevel, spellForgeCapacity + miniSpellForgeCapacity);
		}

		public abstract LogicLeagueData GetLeagueTypeData();
		public abstract void SaveToReplay(LogicJSONObject jsonObject);
		public abstract void SaveToDirect(LogicJSONObject jsonObject);
		public abstract void LoadForReplay(LogicJSONObject jsonObject, bool direct);
	}

	public enum LogicAvatarAllianceRole
	{
		MEMBER = 1,
		LEADER,
		ELDER,
		CO_LEADER
	}
}