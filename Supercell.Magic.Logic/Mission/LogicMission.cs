using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Mission
{
	public class LogicMission
	{
		private LogicLevel m_level;
		private LogicMissionData m_data;

		private int m_progress;
		private int m_requireProgress;

		private bool m_finished;

		public LogicMission(LogicMissionData data, LogicLevel level)
		{
			if (data == null)
			{
				Debugger.Error("LogicMission::constructor - pData is NULL!");
			}

			m_data = data;
			m_level = level;
			m_requireProgress = 1;

			switch (data.GetMissionType())
			{
				case 1:
				case 16:
				case 17:
				case 18:
				case 19:
					m_requireProgress = 2;
					break;
				case 0:
				case 5:
					m_requireProgress = data.GetBuildBuildingCount();
					break;
				case 4:
					m_requireProgress = data.GetTrainTroopCount();
					break;
			}

			if (data.GetMissionCategory() == 1)
			{
				m_requireProgress = 0;
			}
		}

		public void Destruct()
		{
			m_data = null;
			m_level = null;
			m_progress = 0;
			m_requireProgress = 0;
		}

		public int GetMissionType()
			=> m_data.GetMissionType();

		public LogicMissionData GetMissionData()
			=> m_data;

		public int GetProgress()
			=> m_progress;

		public void RefreshProgress()
		{
			LogicGameObjectManager gameObjectManager = m_level.GetGameObjectManager();

			switch (m_data.GetMissionType())
			{
				case 0:
				case 5:
					m_progress = 0;

					if (m_level.GetState() == 1)
					{
						LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(LogicGameObjectType.BUILDING);

						for (int i = 0; i < gameObjects.Size(); i++)
						{
							LogicBuilding building = (LogicBuilding)gameObjects[i];

							if (building.GetBuildingData() == m_data.GetBuildBuildingData() && (!building.IsConstructing() || building.IsUpgrading()) &&
								building.GetUpgradeLevel() >= m_data.GetBuildBuildingLevel())
							{
								++m_progress;
							}
						}
					}

					break;
				case 4:
					m_progress = m_level.GetPlayerAvatar().GetUnitsTotalCapacity();
					break;
				case 6:
					if (m_level.GetPlayerAvatar().GetNameSetByUser())
					{
						m_progress = 1;
					}

					break;
				case 13:
					m_progress = 0;

					if (m_level.GetState() == 1)
					{
						LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(LogicGameObjectType.VILLAGE_OBJECT);

						for (int i = 0; i < gameObjects.Size(); i++)
						{
							LogicVillageObject villageObject = (LogicVillageObject)gameObjects[i];

							if (villageObject.GetVillageObjectData() == m_data.GetFixVillageObjectData() &&
								villageObject.GetUpgradeLevel() >= m_data.GetBuildBuildingLevel())
							{
								++m_progress;
							}
						}
					}

					break;
				case 14:
					m_progress = 0;

					if (m_level.GetState() == 1 && m_level.GetVillageType() == 1)
					{
						++m_progress;
					}

					break;
				case 15:
					m_progress = 0;

					if (m_level.GetState() == 1)
					{
						LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(LogicGameObjectType.BUILDING);

						for (int i = 0; i < gameObjects.Size(); i++)
						{
							LogicBuilding building = (LogicBuilding)gameObjects[i];

							if (building.GetBuildingData() == m_data.GetBuildBuildingData() && !building.IsLocked())
							{
								++m_progress;
							}
						}
					}

					break;
				case 17:
					m_progress = 0;

					if (m_level.GetState() == 1 && m_level.GetVillageType() == 1)
					{
						if (m_level.GetPlayerAvatar().GetUnitUpgradeLevel(m_data.GetCharacterData()) > 0)
						{
							m_progress = 2;
						}
					}

					break;
			}

			if (m_progress >= m_requireProgress)
			{
				m_progress = m_requireProgress;
				Finished();
			}
		}

		public void StateChangeConfirmed()
		{
			switch (m_data.GetMissionType())
			{
				case 1:
					if (m_progress == 0)
					{
						m_level.GetGameMode().StartDefendState(LogicNpcAvatar.GetNpcAvatar(m_data.GetDefendNpcData()));
						m_progress = 1;
					}

					break;
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 20:
				case 21:
					m_progress = 1;
					Finished();

					break;
				case 16:
					if (m_progress == 0)
					{
						// ?
					}

					m_progress += 1;
					break;
				case 19:
					if (m_progress == 1)
					{
						LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();
						int duelScoreGain = LogicDataTables.GetGlobals().GetVillage2FirstVictoryTrophies();

						playerAvatar.AddDuelReward(LogicDataTables.GetGlobals().GetVillage2FirstVictoryGold(), LogicDataTables.GetGlobals().GetVillage2FirstVictoryElixir(), 0, 0,
												   null);
						playerAvatar.SetDuelScore(playerAvatar.GetDuelScore() + LogicDataTables.GetGlobals().GetVillage2FirstVictoryTrophies());
						playerAvatar.GetChangeListener().DuelScoreChanged(playerAvatar.GetAllianceId(), duelScoreGain, -1, false);

						m_progress = 2;
						Finished();
					}

					break;
			}
		}

		public void Finished()
		{
			LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();

			if (!playerAvatar.IsMissionCompleted(m_data))
			{
				playerAvatar.SetMissionCompleted(m_data, true);
				playerAvatar.GetChangeListener().CommodityCountChanged(0, m_data, 1);

				AddRewardUnits();

				LogicResourceData rewardResourceData = m_data.GetRewardResourceData();

				if (rewardResourceData != null)
				{
					playerAvatar.AddMissionResourceReward(rewardResourceData, m_data.GetRewardResourceCount());
				}

				int rewardXp = m_data.GetRewardXp();

				if (rewardXp > 0)
				{
					playerAvatar.XpGainHelper(rewardXp);
				}
			}

			m_finished = true;
		}

		public bool IsOpenTutorialMission()
		{
			if (m_data.GetVillageType() == m_level.GetVillageType())
			{
				if (m_data.GetMissionCategory() == 2)
				{
					LogicAvatar homeOwnerAvatar = m_level.GetHomeOwnerAvatar();

					if (homeOwnerAvatar == null || !homeOwnerAvatar.IsNpcAvatar() || m_level.GetVillageType() != 1)
					{
						LogicGameObjectManager gameObjectManager = m_level.GetGameObjectManagerAt(0);
						LogicVillageObject shipyard = gameObjectManager.GetShipyard();

						if (shipyard == null || shipyard.GetUpgradeLevel() != 0)
						{
							int missionType = m_data.GetMissionType();

							if ((missionType == 16 || missionType == 14) && m_level.GetState() == 1 && m_level.GetVillageType() == 0)
							{
								if (gameObjectManager.GetShipyard().IsConstructing())
								{
									return false;
								}
							}

							return m_data.GetMissionCategory() != 1;
						}

						return false;
					}

					return true;
				}

				return m_data.GetMissionCategory() != 1;
			}

			return false;
		}

		public void Tick()
		{
			int missionType = m_data.GetMissionType();

			switch (missionType)
			{
				case 1:
					if (m_level.GetState() == 1 && m_progress == 1)
					{
						Finished();
					}

					break;
				case 2:
					if (m_level.GetHomeOwnerAvatar().IsNpcAvatar())
					{
						if (m_level.GetState() == 2)
						{
							Finished();
							m_level.GetGameListener().ShowTroopPlacementTutorial(m_data.GetCustomData());
						}
					}

					break;
				case 18:
					if (m_progress == 0)
					{
						if (m_level.GetHomeOwnerAvatar().IsNpcAvatar() && m_level.GetState() == 2)
						{
							m_progress = 1;
							m_level.GetGameListener().ShowTroopPlacementTutorial(m_data.GetCustomData());
						}
					}
					else if (m_progress == 1)
					{
						if (m_level.GetHomeOwnerAvatar().IsNpcAvatar() && m_level.GetState() == 2)
						{
							if (m_level.GetBattleLog().GetBattleEnded())
							{
								m_progress = 2;
								Finished();
							}
						}
					}

					break;
				case 19:
					if (m_level.GetState() == 1 && m_progress == 0)
					{
						m_progress = 1;
					}

					break;
			}
		}

		public bool IsFinished()
			=> m_finished;

		public void AddRewardUnits()
		{
			LogicCharacterData characterData = m_data.GetRewardCharacterData();

			if (characterData != null)
			{
				int characterCount = m_data.GetRewardCharacterCount();

				if (characterCount > 0)
				{
					LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();
					LogicComponentFilter filter = new LogicComponentFilter();

					for (int i = 0; i < characterCount; i++)
					{
						filter.RemoveAllIgnoreObjects();

						while (true)
						{
							LogicUnitStorageComponent component =
								(LogicUnitStorageComponent)m_level.GetComponentManagerAt(m_level.GetVillageType()).GetClosestComponent(0, 0, filter);

							if (component != null)
							{
								if (component.CanAddUnit(characterData))
								{
									playerAvatar.CommodityCountChangeHelper(0, characterData, 1);
									component.AddUnit(characterData);

									if (m_level.GetState() == 1 || m_level.GetState() == 3)
									{
										if (component.GetParentListener() != null)
										{
											component.GetParentListener().ExtraCharacterAdded(characterData, null);
										}
									}

									break;
								}

								filter.AddIgnoreObject(component.GetParent());
							}
							else
							{
								break;
							}
						}
					}
				}
			}
		}
	}
}