using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicMissionData : LogicData
	{
		private int m_missionType;
		private int m_missionCategory;
		private int m_buildBuildingCount;
		private int m_buildBuildingLevel;
		private int m_trainTroopCount;
		private int m_villagers;
		private int m_rewardResourceCount;
		private int m_customData;
		private int m_rewardXp;
		private int m_rewardCharacterCount;
		private int m_delay;
		private int m_villageType;

		private bool m_openAchievements;
		private bool m_showMap;
		private bool m_changeName;
		private bool m_switchSides;
		private bool m_showWarBase;
		private bool m_showStates;
		private bool m_openInfo;
		private bool m_showDonate;
		private bool m_warStates;
		private bool m_forceCamera;
		private bool m_deprecated;
		private bool m_firstStep;

		private string m_action;
		private string m_tutorialText;

		private LogicNpcData m_defendNpcData;
		private LogicNpcData m_attackNpcData;
		private LogicCharacterData m_characterData;
		private LogicBuildingData m_buildBuildingData;
		private LogicVillageObjectData m_fixVillageObjectData;
		private LogicCharacterData m_rewardCharacterData;
		private LogicResourceData m_rewardResourceData;
		private readonly LogicArrayList<LogicMissionData> m_missionDependencies;

		public LogicMissionData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			m_missionType = -1;
			m_missionDependencies = new LogicArrayList<LogicMissionData>();
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			for (int i = 0; i < GetArraySize("Dependencies"); i++)
			{
				LogicMissionData dependency = LogicDataTables.GetMissionByName(GetValue("Dependencies", i), this);

				if (dependency != null)
				{
					m_missionDependencies.Add(dependency);
				}
			}

			m_action = GetValue("Action", 0);
			m_deprecated = GetBooleanValue("Deprecated", 0);
			m_missionCategory = GetIntegerValue("MissionCategory", 0);
			m_fixVillageObjectData = LogicDataTables.GetVillageObjectByName(GetValue("FixVillageObject", 0), this);

			if (m_fixVillageObjectData != null)
			{
				m_buildBuildingLevel = GetIntegerValue("BuildBuildingLevel", 0);
				m_missionType = 13;
			}

			if (string.Equals(m_action, "travel"))
			{
				m_missionType = 14;
			}
			else if (string.Equals(m_action, "upgrade2"))
			{
				m_characterData = LogicDataTables.GetCharacterByName(GetValue("Character", 0), this);
				m_missionType = 17;
			}
			else if (string.Equals(m_action, "duel"))
			{
				m_attackNpcData = LogicDataTables.GetNpcByName(GetValue("AttackNPC", 0), this);
				m_missionType = 18;
			}
			else if (string.Equals(m_action, "duel_end"))
			{
				m_attackNpcData = LogicDataTables.GetNpcByName(GetValue("AttackNPC", 0), this);
				m_missionType = 19;
			}
			else if (string.Equals(m_action, "duel_end2"))
			{
				m_missionType = 20;
			}
			else if (string.Equals(m_action, "show_builder_menu"))
			{
				m_missionType = 21;
			}

			m_buildBuildingData = LogicDataTables.GetBuildingByName(GetValue("BuildBuilding", 0), this);

			if (m_buildBuildingData != null)
			{
				m_buildBuildingCount = GetIntegerValue("BuildBuildingCount", 0);
				m_buildBuildingLevel = GetIntegerValue("BuildBuildingLevel", 0) - 1;
				m_missionType = string.Equals(m_action, "unlock") ? 15 : 5;

				if (m_buildBuildingCount < 0)
				{
					Debugger.Error("missions.csv: BuildBuildingCount is invalid!");
				}
			}
			else
			{
				if (m_missionType == -1)
				{
					m_openAchievements = GetBooleanValue("OpenAchievements", 0);

					if (m_openAchievements)
					{
						m_missionType = 7;
					}
					else
					{
						m_defendNpcData = LogicDataTables.GetNpcByName(GetValue("DefendNPC", 0), this);

						if (m_defendNpcData != null)
						{
							m_missionType = 1;
						}
						else
						{
							m_attackNpcData = LogicDataTables.GetNpcByName(GetValue("AttackNPC", 0), this);

							if (m_attackNpcData != null)
							{
								m_missionType = 2;
								m_showMap = GetBooleanValue("ShowMap", 0);
							}
							else
							{
								m_changeName = GetBooleanValue("ChangeName", 0);

								if (m_changeName)
								{
									m_missionType = 6;
								}
								else
								{
									m_trainTroopCount = GetIntegerValue("TrainTroops", 0);

									if (m_trainTroopCount > 0)
									{
										m_missionType = 4;
									}
									else
									{
										m_switchSides = GetBooleanValue("SwitchSides", 0);

										if (m_switchSides)
										{
											m_missionType = 8;
										}
										else
										{
											m_showWarBase = GetBooleanValue("ShowWarBase", 0);

											if (m_showWarBase)
											{
												m_missionType = 9;
											}
											else
											{
												m_openInfo = GetBooleanValue("OpenInfo", 0);

												if (m_openInfo)
												{
													m_missionType = 11;
												}
												else
												{
													m_showDonate = GetBooleanValue("ShowDonate", 0);

													if (m_showDonate)
													{
														m_missionType = 10;
													}
													else
													{
														m_showStates = GetBooleanValue("WarStates", 0);

														if (m_showStates)
														{
															m_missionType = 12;
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
			}

			m_villagers = GetIntegerValue("Villagers", 0);

			if (m_villagers > 0)
			{
				m_missionType = 16;
			}

			m_forceCamera = GetBooleanValue("ForceCamera", 0);

			if (m_missionType == -1)
			{
				Debugger.Error(string.Format("missions.csv: invalid mission ({0})", GetName()));
			}

			m_rewardResourceData = LogicDataTables.GetResourceByName(GetValue("RewardResource", 0), this);
			m_rewardResourceCount = GetIntegerValue("RewardResourceCount", 0);

			if (m_rewardResourceData != null)
			{
				if (m_rewardResourceCount != 0)
				{
					if (m_rewardResourceCount < 0)
					{
						Debugger.Error("missions.csv: RewardResourceCount is negative!");

						m_rewardResourceData = null;
						m_rewardResourceCount = 0;
					}
				}
				else
				{
					m_rewardResourceData = null;
				}
			}
			else if (m_rewardResourceCount != 0)
			{
				Debugger.Warning("missions.csv: RewardResourceCount defined but RewardResource is not!");
				m_rewardResourceCount = 0;
			}

			m_customData = GetIntegerValue("CustomData", 0);
			m_rewardXp = GetIntegerValue("RewardXP", 0);

			if (m_rewardXp < 0)
			{
				Debugger.Warning("missions.csv: RewardXP is negative!");
				m_rewardXp = 0;
			}

			m_rewardCharacterData = LogicDataTables.GetCharacterByName(GetValue("RewardTroop", 0), this);
			m_rewardCharacterCount = GetIntegerValue("RewardTroopCount", 0);

			if (m_rewardCharacterData != null)
			{
				if (m_rewardCharacterCount != 0)
				{
					if (m_rewardCharacterCount < 0)
					{
						Debugger.Error("missions.csv: RewardTroopCount is negative!");

						m_rewardCharacterData = null;
						m_rewardCharacterCount = 0;
					}
				}
				else
				{
					m_rewardCharacterData = null;
				}
			}
			else if (m_rewardCharacterCount != 0)
			{
				Debugger.Warning("missions.csv: RewardTroopCount defined but RewardTroop is not!");
				m_rewardCharacterCount = 0;
			}

			m_delay = GetIntegerValue("Delay", 0);
			m_villageType = GetIntegerValue("VillageType", 0);
			m_firstStep = GetBooleanValue("FirstStep", 0);
			m_tutorialText = GetValue("TutorialText", 0);

			if (m_tutorialText.Length > 0)
			{
				// BLABLABLA
			}
		}

		public bool IsOpenForAvatar(LogicClientAvatar avatar)
		{
			if (!avatar.IsMissionCompleted(this))
			{
				if (avatar.GetExpLevel() >= 10)
				{
					if ((uint)(m_missionCategory - 1) > 1)
					{
						return false;
					}
				}

				if (!m_deprecated)
				{
					for (int i = 0; i < m_missionDependencies.Size(); i++)
					{
						if (!avatar.IsMissionCompleted(m_missionDependencies[i]))
						{
							return false;
						}
					}

					return true;
				}
			}

			return false;
		}

		public int GetMissionType()
			=> m_missionType;

		public int GetCustomData()
			=> m_customData;

		public LogicCharacterData GetCharacterData()
			=> m_characterData;

		public LogicCharacterData GetRewardCharacterData()
			=> m_rewardCharacterData;

		public LogicVillageObjectData GetFixVillageObjectData()
			=> m_fixVillageObjectData;

		public LogicBuildingData GetBuildBuildingData()
			=> m_buildBuildingData;

		public LogicResourceData GetRewardResourceData()
			=> m_rewardResourceData;

		public LogicNpcData GetAttackNpcData()
			=> m_attackNpcData;

		public LogicNpcData GetDefendNpcData()
			=> m_defendNpcData;

		public int GetRewardResourceCount()
			=> m_rewardResourceCount;

		public int GetRewardCharacterCount()
			=> m_rewardCharacterCount;

		public int GetRewardXp()
			=> m_rewardXp;

		public int GetBuildBuildingLevel()
			=> m_buildBuildingLevel;

		public int GetBuildBuildingCount()
			=> m_buildBuildingCount;

		public int GetTrainTroopCount()
			=> m_trainTroopCount;

		public int GetMissionCategory()
			=> m_missionCategory;

		public int GetVillageType()
			=> m_villageType;
	}
}