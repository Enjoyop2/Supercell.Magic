using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicObstacle : LogicGameObject
	{
		private LogicTimer m_clearTimer;

		private int m_lootMultiplyVersion;
		private int m_fadeTime;

		public LogicObstacle(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			LogicObstacleData obstacleData = GetObstacleData();

			if (obstacleData.GetSpawnObstacle() != null)
			{
				AddComponent(new LogicSpawnerComponent(this, obstacleData.GetSpawnObstacle(), obstacleData.GetSpawnRadius(), obstacleData.GetSpawnIntervalSeconds(),
														  obstacleData.GetSpawnCount(), obstacleData.GetMaxSpawned(), obstacleData.GetMaxLifetimeSpawns()));
			}

			if (obstacleData.IsLootCart())
			{
				LogicLootCartComponent logicLootCartComponent = new LogicLootCartComponent(this);
				LogicDataTable resourceTable = LogicDataTables.GetTable(DataType.RESOURCE);
				LogicBuilding townHall = GetGameObjectManager().GetTownHall();

				LogicArrayList<int> capacityCount = new LogicArrayList<int>();

				for (int i = 0, cap = 0; i < resourceTable.GetItemCount(); i++, cap = 0)
				{
					LogicResourceData resourceData = (LogicResourceData)resourceTable.GetItemAt(i);

					if (townHall != null)
					{
						if (!resourceData.IsPremiumCurrency() && resourceData.GetWarResourceReferenceData() == null)
						{
							cap = LogicDataTables.GetTownHallLevel(townHall.GetUpgradeLevel()).GetCartLootCap(resourceData);
						}
					}

					capacityCount.Add(cap);
				}

				logicLootCartComponent.SetCapacityCount(capacityCount);

				AddComponent(logicLootCartComponent);
			}
		}

		public LogicObstacleData GetObstacleData()
			=> (LogicObstacleData)m_data;

		public LogicLootCartComponent GetLootCartComponent()
			=> (LogicLootCartComponent)GetComponent(LogicComponentType.LOOT_CART);

		public override void Destruct()
		{
			base.Destruct();

			if (m_clearTimer != null)
			{
				m_clearTimer.Destruct();
				m_clearTimer = null;
			}

			m_fadeTime = 0;
		}

		public override bool IsPassable()
			=> GetObstacleData().IsPassable();

		public override void FastForwardTime(int time)
		{
			base.FastForwardTime(time);

			if (m_clearTimer != null)
			{
				int remainingSeconds = m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime());

				if (remainingSeconds <= time)
				{
					if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
					{
						m_clearTimer.StartTimer(0, m_level.GetLogicTime(), false, -1);
					}
					else
					{
						ClearingFinished(true);
					}
				}
				else
				{
					m_clearTimer.StartTimer(remainingSeconds - time, m_level.GetLogicTime(), false, -1);
				}
			}
		}

		public override void Tick()
		{
			base.Tick();

			if (m_clearTimer != null)
			{
				if (m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime()) > 0 && m_level.GetRemainingClockTowerBoostTime() > 0 &&
					GetObstacleData().GetVillageType() == 1)
				{
					m_clearTimer.SetFastForward(m_clearTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
				}
			}

			if (m_fadeTime < 1)
			{
				if (m_clearTimer != null)
				{
					if (m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
					{
						ClearingFinished(false);
					}
				}
			}
			else
			{
				m_fadeTime = LogicMath.Min(m_fadeTime + 64, GetFadingOutTime());
			}
		}

		public override bool ShouldDestruct()
			=> m_fadeTime >= GetFadingOutTime();

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			base.Save(jsonObject, villageType);

			if (m_clearTimer != null)
			{
				jsonObject.Put("clear_t", new LogicJSONNumber(m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime())));
				jsonObject.Put("clear_ff", new LogicJSONNumber(m_clearTimer.GetFastForward()));
			}

			if (m_lootMultiplyVersion != 1)
			{
				jsonObject.Put("lmv", new LogicJSONNumber(m_lootMultiplyVersion));
			}
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			jsonObject.Put("x", new LogicJSONNumber(GetTileX() & 63));
			jsonObject.Put("y", new LogicJSONNumber(GetTileY() & 63));
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			base.Load(jsonObject);

			LogicJSONNumber clearTimeObject = jsonObject.GetJSONNumber("clear_t");

			if (clearTimeObject != null)
			{
				if (m_clearTimer != null)
				{
					m_clearTimer.Destruct();
					m_clearTimer = null;
				}

				m_clearTimer = new LogicTimer();
				m_clearTimer.StartTimer(clearTimeObject.GetIntValue(), m_level.GetLogicTime(), false, -1);
				m_level.GetWorkerManagerAt(m_villageType).AllocateWorker(this);
			}

			LogicJSONNumber clearFastForwardObject = jsonObject.GetJSONNumber("clear_ff");

			if (clearFastForwardObject != null)
			{
				if (m_clearTimer != null)
				{
					m_clearTimer.SetFastForward(clearFastForwardObject.GetIntValue());
				}
			}

			LogicJSONNumber lootMultiplyVersionObject = jsonObject.GetJSONNumber("loot_multiply_ver");

			if (lootMultiplyVersionObject == null)
			{
				lootMultiplyVersionObject = jsonObject.GetJSONNumber("lmv");

				if (lootMultiplyVersionObject == null)
				{
					m_lootMultiplyVersion = 1;
					return;
				}
			}

			m_lootMultiplyVersion = lootMultiplyVersionObject.GetIntValue();
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			base.LoadFromSnapshot(jsonObject);
		}

		public override void LoadingFinished()
		{
			base.LoadingFinished();

			if (m_listener != null)
			{
				m_listener.LoadedFromJSON();
			}
		}

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.OBSTACLE;

		public override bool IsUnbuildable()
			=> GetObstacleData().IsTombstone() || GetObstacleData().IsLootCart();

		public override int GetWidthInTiles()
			=> GetObstacleData().GetWidth();

		public override int GetHeightInTiles()
			=> GetObstacleData().GetHeight();

		public bool IsTombstone()
			=> GetObstacleData().IsTombstone();

		public int GetTombGroup()
			=> GetObstacleData().GetTombGroup();

		public int GetFadeTime()
			=> m_fadeTime;

		public int GetFadingOutTime()
		{
			LogicObstacleData data = GetObstacleData();

			if (!data.IsTallGrass())
			{
				return data.IsLootCart() ? 4000 : 2000;
			}

			return 1000;
		}

		public int GetLootMultiplyVersion()
			=> m_lootMultiplyVersion;

		public void SetLootMultiplyVersion(int version)
		{
			m_lootMultiplyVersion = version;
		}

		public int GetRemainingClearingTime()
		{
			if (m_clearTimer != null)
			{
				return m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime());
			}

			return 0;
		}

		public int GetRemainingClearingTimeMS()
		{
			if (m_clearTimer != null)
			{
				return m_clearTimer.GetRemainingMS(m_level.GetLogicTime());
			}

			return 0;
		}

		public bool IsFadingOut()
			=> m_fadeTime > 0;

		public bool CanStartClearing()
			=> m_clearTimer == null && m_fadeTime == 0;

		public bool IsClearingOnGoing()
			=> m_clearTimer != null;

		public void StartClearing()
		{
			if (m_clearTimer == null && m_fadeTime == 0)
			{
				if (GetObstacleData().GetClearTime() != 0)
				{
					m_level.GetWorkerManagerAt(m_villageType).AllocateWorker(this);

					m_clearTimer = new LogicTimer();
					m_clearTimer.StartTimer(GetObstacleData().GetClearTime(), m_level.GetLogicTime(), false, -1);

					if (m_listener != null)
					{
						// Listener.
					}
				}
				else
				{
					ClearingFinished(false);
				}
			}
		}

		public void CancelClearing()
		{
			m_level.GetWorkerManagerAt(m_data.GetVillageType()).DeallocateWorker(this);

			if (m_clearTimer != null)
			{
				m_clearTimer.Destruct();
				m_clearTimer = null;
			}
		}

		public void ClearingFinished(bool ignoreState)
		{
			int state = m_level.GetState();

			if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
			{
				if (m_level.GetHomeOwnerAvatar().IsClientAvatar())
				{
					LogicClientAvatar homeOwnerAvatar = (LogicClientAvatar)m_level.GetHomeOwnerAvatar();
					LogicObstacleData obstacleData = GetObstacleData();
					LogicResourceData lootResourceData = obstacleData.GetLootResourceData();

					int lootCount = obstacleData.GetLootCount();

					if (obstacleData.IsLootCart())
					{
						LogicLootCartComponent lootCartComponent = (LogicLootCartComponent)GetComponent(LogicComponentType.LOOT_CART);

						if (lootCartComponent != null)
						{
							LogicDataTable resourceTable = LogicDataTables.GetTable(DataType.RESOURCE);

							bool empty = true;

							for (int i = 0; i < resourceTable.GetItemCount(); i++)
							{
								LogicResourceData resourceData = (LogicResourceData)resourceTable.GetItemAt(i);

								if (!resourceData.IsPremiumCurrency() && resourceData.GetWarResourceReferenceData() == null)
								{
									int resourceCount = lootCartComponent.GetResourceCount(i);
									int rewardCount = LogicMath.Min(homeOwnerAvatar.GetUnusedResourceCap(resourceData), resourceCount);
									int remainingCount = resourceCount - rewardCount;

									if (rewardCount > 0)
									{
										homeOwnerAvatar.CommodityCountChangeHelper(0, resourceData, rewardCount);
										lootCartComponent.SetResourceCount(i, remainingCount);
									}

									if (remainingCount > 0)
									{
										empty = false;
									}
								}
							}

							if (!empty)
							{
								return;
							}
						}
					}

					if (!obstacleData.IsTombstone() && !obstacleData.IsLootCart())
					{
						m_level.GetAchievementManager().ObstacleCleared();
					}

					m_level.GetWorkerManagerAt(m_villageType).DeallocateWorker(this);
					XpGainHelper(LogicGamePlayUtil.TimeToExp(obstacleData.GetClearTime()), homeOwnerAvatar, ignoreState || state == 1);

					if (lootResourceData != null && lootCount > 0)
					{
						if (homeOwnerAvatar != null)
						{
							if (lootResourceData.IsPremiumCurrency())
							{
								int lootMultipler = 1;

								if (m_lootMultiplyVersion >= 2)
								{
									lootMultipler = obstacleData.GetLootMultiplierVersion2();
								}

								int diamondsCount = obstacleData.GetName().Equals("Bonus Gembox")
									? lootCount * lootMultipler
									: m_level.GetGameObjectManagerAt(m_villageType).IncreaseObstacleClearCounter(lootMultipler);

								if (diamondsCount > 0)
								{
									homeOwnerAvatar.SetDiamonds(homeOwnerAvatar.GetDiamonds() + diamondsCount);
									homeOwnerAvatar.SetFreeDiamonds(homeOwnerAvatar.GetFreeDiamonds() + diamondsCount);
									homeOwnerAvatar.GetChangeListener().FreeDiamondsAdded(diamondsCount, 6);
								}
							}
							else
							{
								int gainCount = LogicMath.Min(homeOwnerAvatar.GetUnusedResourceCap(lootResourceData), lootCount);

								if (gainCount > 0)
								{
									homeOwnerAvatar.CommodityCountChangeHelper(0, lootResourceData, gainCount);
								}
							}
						}
						else
						{
							Debugger.Error("LogicObstacle::clearingFinished - Home owner avatar is NULL!");
						}
					}

					if (obstacleData.IsEnabledInVillageType(m_level.GetVillageType()))
					{
						// ?
					}

					if (m_clearTimer != null)
					{
						m_clearTimer.Destruct();
						m_clearTimer = null;
					}

					m_fadeTime = 1;
				}
			}
		}

		public bool SpeedUpClearing()
		{
			if (m_clearTimer != null)
			{
				LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();
				int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(m_clearTimer.GetRemainingSeconds(m_level.GetLogicTime()), 0, m_villageType);

				if (playerAvatar.HasEnoughDiamonds(speedUpCost, true, m_level))
				{
					playerAvatar.UseDiamonds(speedUpCost);
					playerAvatar.GetChangeListener().DiamondPurchaseMade(3, m_data.GetGlobalID(), 0, speedUpCost, m_level.GetVillageType());

					ClearingFinished(false);
					return true;
				}
			}

			return false;
		}

		public void ReengageLootCart(int secs)
		{
			LogicObstacleData obstacleData = GetObstacleData();
			LogicLootCartComponent lootCartComponent = (LogicLootCartComponent)GetComponent(LogicComponentType.LOOT_CART);
			LogicBuilding townHall = m_level.GetGameObjectManagerAt(0).GetTownHall();

			Debugger.DoAssert(obstacleData.IsLootCart(), string.Empty);
			Debugger.DoAssert(lootCartComponent != null, string.Empty);
			Debugger.DoAssert(townHall != null, string.Empty);

			LogicDataTable resourceTable = LogicDataTables.GetTable(DataType.RESOURCE);

			for (int i = 0; i < resourceTable.GetItemCount(); i++)
			{
				LogicResourceData resourceData = (LogicResourceData)resourceTable.GetItemAt(i);
				LogicTownhallLevelData townhallLevelData = LogicDataTables.GetTownHallLevel(townHall.GetUpgradeLevel());

				int cap = secs * townhallLevelData.GetCartLootReengagement(resourceData) / 100;

				if (cap > lootCartComponent.GetResourceCount(i))
				{
					lootCartComponent.SetResourceCount(i, cap);
				}
			}
		}
	}
}