using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Data
{
	public class LogicGlobals : LogicDataTable
	{
		private int m_speedUpDiamondCostPerMin;
		private int m_speedUpDiamondCostPerHour;
		private int m_speedUpDiamondCostPerDay;
		private int m_speedUpDiamondCostPerWeek;
		private int m_speedUpDiamondCostPerMinVillage2;
		private int m_speedUpDiamondCostPerHourVillage2;
		private int m_speedUpDiamondCostPerDayVillage2;
		private int m_speedUpDiamondCostPerWeekVillage2;

		private int m_resourceDiamondCost100;
		private int m_resourceDiamondCost1000;
		private int m_resourceDiamondCost10000;
		private int m_resourceDiamondCost100000;
		private int m_resourceDiamondCost1000000;
		private int m_resourceDiamondCost10000000;
		private int m_village2ResourceDiamondCost100;
		private int m_village2ResourceDiamondCost1000;
		private int m_village2ResourceDiamondCost10000;
		private int m_village2resourceDiamondCost100000;
		private int m_village2resourceDiamondCost1000000;
		private int m_village2ResourceDiamondCost10000000;
		private int m_darkElixirDiamondCost1;
		private int m_darkElixirDiamondCost10;
		private int m_darkElixirDiamondCost100;
		private int m_darkElixirDiamondCost1000;
		private int m_darkElixirDiamondCost10000;
		private int m_darkElixirDiamondCost100000;

		private int m_freeUnitHousingCapPercentage;
		private int m_freeHeroHealthCap;
		private int m_startingDiamonds;
		private int m_startingElixir;
		private int m_startingElixir2;
		private int m_startingGold;
		private int m_startingGold2;
		private int m_liveReplayFrequencySecs;
		private int m_challengeBaseSaveCooldown;
		private int m_allianceTroopRequestCooldown;
		private int m_arrangeWarCooldown;
		private int m_clanMailCooldown;
		private int m_replayShareCooldown;
		private int m_elderKickCooldown;
		private int m_challengeCooldown;
		private int m_allianceCreateCost;
		private int m_clockTowerBoostCooldownSecs;
		private int m_clampLongTimeStampsToDays;
		private int m_workerCostSecondBuildCost;
		private int m_workerCostThirdBuildCost;
		private int m_workerCostFourthBuildCost;
		private int m_workerCostFifthBuildCost;
		private int m_challengeBaseCooldownEnabledOnTh;
		private int m_obstacleRespawnSecs;
		private int m_tallGrassRespawnSecs;
		private int m_obstacleMaxCount;
		private int m_resourceProductionLootPercentage;
		private int m_darkElixirProductionLootPercentage;
		private int m_village2MinTownHallLevelForDestructObstacle;
		private int m_attackVillage2PreparationLengthSecs;
		private int m_attackPreparationLengthSecs;
		private int m_attackLengthSecs;
		private int m_village2StartUnitLevel;
		private int m_resourceProductionBoostSecs;
		private int m_barracksBoostSecs;
		private int m_spellFactoryBoostSecs;
		private int m_heroRestBoostSecs;
		private int m_troopTrainingSpeedUpCostTutorial;
		private int m_newTrainingBoostBarracksCost;
		private int m_newTrainingBoostLaboratoryCost;
		private int m_personalBreakLimitSeconds;
		private int m_enablePresetsTownHallLevel;
		private int m_maxAllianceFeedbackMessageLength;
		private int m_maxAllianceMailLength;
		private int m_maxMessageLength;
		private int m_maxTroopDonationCount;
		private int m_maxSpellDonationCount;
		private int m_darkSpellDonationXP;
		private int m_enableNameChangeTownHallLevel;
		private int m_starBonusCooldownMinutes;
		private int m_bunkerSearchTime;
		private int m_clanCastleRadius;
		private int m_clanDefenderSearchRadius;
		private int m_lootCartReengagementMinSecs;
		private int m_lootCartReengagementMaxSecs;
		private int m_warMaxExcludeMembers;
		private int m_minerTargetRandPercentage;
		private int m_minerSpeedRandPercentage;
		private int m_minerHideTime;
		private int m_minerHideTimeRandom;
		private int m_townHallLootPercentage;
		private int m_charVsCharRandomDistanceLimit;
		private int m_charVsCharRadiusForAttacker;
		private int m_targetListSize;
		private int m_chainedProjectileBounceCount;

		private int m_clockTowerBoostMultiplier;
		private int m_resourceProductionBoostMultiplier;
		private int m_spellTrainingCostMultiplier;
		private int m_spellSpeedUpCostMultiplier;
		private int m_heroHealthSpeedUpCostMultipler;
		private int m_troopRequestSpeedUpCostMultiplier;
		private int m_troopTrainingCostMultiplier;
		private int m_speedUpBoostCooldownCostMultiplier;
		private int m_spellHousingCostMultiplier;
		private int m_unitHousingCostMultiplier;
		private int m_heroHousingCostMultiplier;
		private int m_unitHousingCostMultiplierForTotal;
		private int m_spellHousingCostMultiplierForTotal;
		private int m_heroHousingCostMultiplierForTotal;
		private int m_allianceUnitHousingCostMultiplierForTotal;
		private int m_barracksBoostNewMultiplier;
		private int m_barracksBoostMultiplier;
		private int m_spellFactoryBoostNewMultiplier;
		private int m_spellFactoryBoostMultiplier;
		private int m_clockTowerSpeedUpMultiplier;
		private int m_heroRestBoostMultiplier;
		private int m_buildCancelMultiplier;
		private int m_spellCancelMultiplier;
		private int m_trainCancelMultiplier;
		private int m_heroUpgradeCancelMultiplier;
		private int m_village2FirstVictoryTrophies;
		private int m_village2FirstVictoryGold;
		private int m_village2FirstVictoryElixir;
		private int m_duelLootLimitFreeSpeedUps;
		private int m_newbieShieldHours;
		private int m_bookmarksMaxAlliances;
		private int m_layoutSlot2THLevel;
		private int m_layoutSlot3THLevel;
		private int m_layoutSlot2THLevelVillage2;
		private int m_layoutSlot3THLevelVillage2;
		private int m_scoreMultiplierOnAttackLose;
		private int m_eloOffsetDampeningFactor;
		private int m_eloOffsetDampeningLimit;
		private int m_eloOffsetDampeningScoreLimit;
		private int m_starBonusStarCount;
		private int m_lootCartEnabledForTH;
		private int m_shieldTriggerPercentageHousingSpace;
		private int m_defaultDefenseVillageGuard;
		private int m_wallCostBase;
		private int m_hiddenBuildingAppearDestructionPercentage;
		private int m_heroHealMultiplier;
		private int m_heroRageMultiplier;
		private int m_heroRageSpeedMultiplier;
		private int m_warLootPercentage;
		private int m_blockedAttackPositionPenalty;
		private int m_wallBreakerSmartCountLimit;
		private int m_wallBreakerSmartRadius;
		private int m_wallBreakerSmartRetargetLimit;
		private int m_selectedWallTime;
		private int m_forgetTargetTime;
		private int m_skeletonSpellStorageMultiplier;
		private int m_allianceAlertRadius;
		private int m_shrinkSpellDurationSeconds;

		private bool m_useNewTraining;
		private bool m_useTroopWalksOutFromTraining;
		private bool m_useVillageObjects;
		private bool m_useVersusBattle;
		private bool m_moreAccurateTime;
		private bool m_dragInTraining;
		private bool m_dragInTrainingFix;
		private bool m_dragInTrainingFix2;
		private bool m_useNewPathFinder;
		private bool m_liveReplayEnabled;
		private bool m_revertBrokenWarLayouts;
		private bool m_removeRevengeWhenBattleIsLoaded;
		private bool m_completeConstructionOnlyHome;
		private bool m_useNewSpeedUpCalculation;
		private bool m_clampBuildingTimes;
		private bool m_clampUpgradesTimes;
		private bool m_clampAvatarTimersToMax;
		private bool m_stopBoostPauseWhenBoostTimeZeroOnLoad;
		private bool m_fixClanPortalBattleNotEnding;
		private bool m_fixMergeOldBarrackBoostPausing;
		private bool m_saveVillageObjects;
		private bool m_startInLastUsedVillage;
		private bool m_workerForZeroBuildingTime;
		private bool m_adjustEndSubtickUseCurrentTime;
		private bool m_collectAllResourcesAtOnce;
		private bool m_useSwapBuildings;
		private bool m_treasurySizeBasedOnTawnHall;
		private bool m_useTeslaTriggerCommand;
		private bool m_useTrapTriggerCommand;
		private bool m_validateTroopUpgradeLevels;
		private bool m_allowCancelBuildingConstruction;
		private bool m_village2TrainingOnlyUseRegularStorage;
		private bool m_enableTroopDeletion;
		private bool m_enablePresets;
		private bool m_enableNameChange;
		private bool m_enableQuickDonate;
		private bool m_enableQuickDonateWar;
		private bool m_useTownHallLootPenaltyInWar;
		private bool m_allowClanCastleDeployOnObstacles;
		private bool m_skeletonTriggerTesla;
		private bool m_skeletonOpenClanCastle;
		private bool m_castleTroopTargetFilter;
		private bool m_useTroopRequestSpeedUp;
		private bool m_noCooldownFromMoveEditModeActive;
		private bool m_scoringOnlyFromMatchedMode;
		private bool m_eloOffsetDampeningEnabled;
		private bool m_enableLeagues;
		private bool m_revengeGiveLeagueBonus;
		private bool m_revengeGiveStarBonus;
		private bool m_allowStarsOverflowInStarBonus;
		private bool m_loadVillage2AsSnapshot;
		private bool m_readyForWarAttackCheck;
		private bool m_useMoreAccurateLootCap;
		private bool m_enableDefendingAllianceTroopJump;
		private bool m_useWallWeightsForJumpSpell;
		private bool m_jumpWhenHitJumpable;
		private bool m_slideAlongObstacles;
		private bool m_guardPostNotFunctionalUnderUpgrade;
		private bool m_repathDuringFly;
		private bool m_useStickToClosestUnitHealer;
		private bool m_heroUsesAttackPosRandom;
		private bool m_useAttackPosRandomOn1stTarget;
		private bool m_targetSelectionConsidersWallsOnPath;
		private bool m_valkyriePrefers4Buildings;
		private bool m_tighterAttackPosition;
		private bool m_allianceTroopsPatrol;
		private bool m_wallBreakerUseRooms;
		private bool m_rememberOriginalTarget;
		private bool m_ignoreAllianceAlertForNonValidTargets;
		private bool m_restartAttackTimerOnAreaDamageTurrets;
		private bool m_clearAlertStateIfNoTargetFound;
		private bool m_movingUnitsUseSimpleSelect;
		private bool m_morePreciseTargetSelection;
		private bool m_useSmarterHealer;
		private bool m_usePoisonAvoidance;
		private bool m_removeUntriggeredTesla;

		private int[] m_village2TroopHousingBuildCost;
		private int[] m_village2TroopHousingBuildTimeSecs;
		private int[] m_lootMultiplierByTownHallDifference;
		private int[] m_barrackReduceTrainingDivisor;
		private int[] m_darkBarrackReduceTrainingDivisor;
		private int[] m_clockTowerBoostSecs;
		private int[] m_allianceScoreLimit;
		private int[] m_leagueBonusPercentages;
		private int[] m_leagueBonusAltPercentages;
		private int[] m_destructionToShield;
		private int[] m_shieldHours;
		private int[] m_attackShieldReduceHours;
		private int[] m_healStackPercent;

		private LogicResourceData m_allianceCreateResourceData;
		private LogicCharacterData m_village2StartUnit;

		public LogicGlobals(CSVTable table, LogicDataType index) : base(table, index)
		{
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_freeUnitHousingCapPercentage = GetIntValue("FREE_UNIT_HOUSING_CAP_PERCENTAGE");
			m_freeHeroHealthCap = GetIntValue("FREE_HERO_HEALTH_CAP");

			m_speedUpDiamondCostPerMin = GetIntValue("SPEED_UP_DIAMOND_COST_1_MIN");
			m_speedUpDiamondCostPerHour = GetIntValue("SPEED_UP_DIAMOND_COST_1_HOUR");
			m_speedUpDiamondCostPerDay = GetIntValue("SPEED_UP_DIAMOND_COST_24_HOURS");
			m_speedUpDiamondCostPerWeek = GetIntValue("SPEED_UP_DIAMOND_COST_1_WEEK");
			m_speedUpDiamondCostPerMinVillage2 = GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_MIN");
			m_speedUpDiamondCostPerHourVillage2 = GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_HOUR");
			m_speedUpDiamondCostPerDayVillage2 = GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_24_HOURS");
			m_speedUpDiamondCostPerWeekVillage2 = GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_WEEK");

			m_resourceDiamondCost100 = GetIntValue("RESOURCE_DIAMOND_COST_100");
			m_resourceDiamondCost1000 = GetIntValue("RESOURCE_DIAMOND_COST_1000");
			m_resourceDiamondCost10000 = GetIntValue("RESOURCE_DIAMOND_COST_10000");
			m_resourceDiamondCost100000 = GetIntValue("RESOURCE_DIAMOND_COST_100000");
			m_resourceDiamondCost1000000 = GetIntValue("RESOURCE_DIAMOND_COST_1000000");
			m_resourceDiamondCost10000000 = GetIntValue("RESOURCE_DIAMOND_COST_10000000");
			m_village2ResourceDiamondCost100 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_100");
			m_village2ResourceDiamondCost1000 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000");
			m_village2ResourceDiamondCost10000 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000");
			m_village2resourceDiamondCost100000 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_100000");
			m_village2resourceDiamondCost1000000 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000000");
			m_village2ResourceDiamondCost10000000 = GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000000");
			m_darkElixirDiamondCost1 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_1");
			m_darkElixirDiamondCost10 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_10");
			m_darkElixirDiamondCost100 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_100");
			m_darkElixirDiamondCost1000 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_1000");
			m_darkElixirDiamondCost10000 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_10000");
			m_darkElixirDiamondCost100000 = GetIntValue("DARK_ELIXIR_DIAMOND_COST_100000");

			m_startingDiamonds = GetIntValue("STARTING_DIAMONDS");
			m_startingGold = GetIntValue("STARTING_GOLD");
			m_startingElixir = GetIntValue("STARTING_ELIXIR");
			m_startingGold2 = GetIntValue("STARTING_GOLD2");
			m_startingElixir2 = GetIntValue("STARTING_ELIXIR2");
			m_liveReplayFrequencySecs = GetIntValue("LIVE_REPLAY_UPDATE_FREQUENCY_SECONDS");
			m_challengeBaseSaveCooldown = GetIntValue("CHALLENGE_BASE_SAVE_COOLDOWN");
			m_allianceCreateCost = GetIntValue("ALLIANCE_CREATE_COST");
			m_clockTowerBoostCooldownSecs = 60 * GetIntValue("CLOCK_TOWER_BOOST_COOLDOWN_MINS");
			m_clampLongTimeStampsToDays = GetIntValue("CLAMP_LONG_TIME_STAMPS_TO_DAYS");
			m_workerCostSecondBuildCost = GetIntValue("WORKER_COST_2ND");
			m_workerCostThirdBuildCost = GetIntValue("WORKER_COST_3RD");
			m_workerCostFourthBuildCost = GetIntValue("WORKER_COST_4TH");
			m_workerCostFifthBuildCost = GetIntValue("WORKER_COST_5TH");
			m_challengeBaseCooldownEnabledOnTh = GetIntValue("CHALLENGE_BASE_COOLDOWN_ENABLED_ON_TH");
			m_obstacleRespawnSecs = GetIntValue("OBSTACLE_RESPAWN_SECONDS");

			if (m_obstacleRespawnSecs < 3600)
			{
				Debugger.Error("Globals.csv - OBSTACLE_RESPAWN_SECONDS is smaller than 3600");
			}

			m_obstacleMaxCount = GetIntValue("OBSTACLE_COUNT_MAX");
			m_resourceProductionLootPercentage = GetIntValue("RESOURCE_PRODUCTION_LOOT_PERCENTAGE");
			m_darkElixirProductionLootPercentage = GetIntValue("RESOURCE_PRODUCTION_LOOT_PERCENTAGE_DARK_ELIXIR");
			m_village2MinTownHallLevelForDestructObstacle = GetIntValue("VILLAGE2_DO_NOT_ALLOW_CLEAR_OBSTACLE_TH");
			m_attackVillage2PreparationLengthSecs = GetIntValue("ATTACK_PREPARATION_LENGTH_VILLAGE2_SEC");
			m_attackPreparationLengthSecs = GetIntValue("ATTACK_PREPARATION_LENGTH_SEC");
			m_attackLengthSecs = GetIntValue("ATTACK_LENGTH_SEC");
			m_village2StartUnitLevel = GetIntValue("VILLAGE2_START_UNIT_LEVEL");
			m_resourceProductionBoostSecs = 60 * GetIntValue("RESOURCE_PRODUCTION_BOOST_MINS");
			m_barracksBoostSecs = 60 * GetIntValue("BARRACKS_BOOST_MINS");
			m_spellFactoryBoostSecs = 60 * GetIntValue("SPELL_FACTORY_BOOST_MINS");
			m_heroRestBoostSecs = 60 * GetIntValue("HERO_REST_BOOST_MINS");
			m_troopTrainingSpeedUpCostTutorial = GetIntValue("TROOP_TRAINING_SPEED_UP_COST_TUTORIAL");
			m_newTrainingBoostBarracksCost = GetIntValue("NEW_TRAINING_BOOST_BARRACKS_COST");
			m_newTrainingBoostLaboratoryCost = GetIntValue("NEW_TRAINING_BOOST_LABORATORY_COST");
			m_personalBreakLimitSeconds = GetIntValue("PERSONAL_BREAK_LIMIT_SECONDS");
			m_allianceTroopRequestCooldown = GetIntValue("ALLIANCE_TROOP_REQUEST_COOLDOWN");
			m_arrangeWarCooldown = GetIntValue("ARRANGE_WAR_COOLDOWN");
			m_clanMailCooldown = GetIntValue("CLAN_MAIL_COOLDOWN");
			m_replayShareCooldown = GetIntValue("REPLAY_SHARE_COOLDOWN");
			m_elderKickCooldown = GetIntValue("ELDER_KICK_COOLDOWN");
			m_challengeCooldown = GetIntValue("CHALLENGE_COOLDOWN");
			m_enablePresetsTownHallLevel = GetIntValue("ENABLE_PRESETS_TH_LEVEL") - 1;
			m_maxAllianceFeedbackMessageLength = GetIntValue("MAX_ALLIANCE_FEEDBACK_MESSAGE_LENGTH");
			m_maxAllianceMailLength = GetIntValue("MAX_ALLIANCE_MAIL_LENGTH");
			m_maxMessageLength = GetIntValue("MAX_MESSAGE_LENGTH");
			m_tallGrassRespawnSecs = GetIntValue("TALLGRASS_RESPAWN_SECONDS");
			m_enableNameChangeTownHallLevel = GetIntValue("ENABLE_NAME_CHANGE_TH_LEVEL") - 1;
			m_village2FirstVictoryTrophies = GetIntValue("VILLAGE2_FIRST_VICTORY_TROPHIES");
			m_village2FirstVictoryGold = GetIntValue("VILLAGE2_FIRST_VICTORY_GOLD");
			m_village2FirstVictoryElixir = GetIntValue("VILLAGE2_FIRST_VICTORY_ELIXIR");
			m_duelLootLimitFreeSpeedUps = GetIntValue("DUEL_LOOT_LIMIT_FREE_SPEEDUPS");
			m_maxTroopDonationCount = GetIntValue("MAX_TROOP_DONATION_COUNT");
			m_maxSpellDonationCount = GetIntValue("MAX_SPELL_DONATION_COUNT");
			m_darkSpellDonationXP = GetIntValue("DARK_SPELL_DONATION_XP");
			m_starBonusCooldownMinutes = GetIntValue("STAR_BONUS_COOLDOWN_MINUTES");
			m_clanCastleRadius = GetIntValue("CLAN_CASTLE_RADIUS") << 9;
			m_clanDefenderSearchRadius = GetIntValue("CASTLE_DEFENDER_SEARCH_RADIUS");
			m_bunkerSearchTime = GetIntValue("BUNKER_SEARCH_TIME");
			m_newbieShieldHours = GetIntValue("NEWBIE_SHIELD_HOURS");
			m_lootCartReengagementMinSecs = 60 * GetIntValue("LOOT_CART_REENGAGEMENT_MINUTES_MIN");
			m_lootCartReengagementMaxSecs = 60 * GetIntValue("LOOT_CART_REENGAGEMENT_MINUTES_MAX");
			m_warMaxExcludeMembers = GetIntValue("WAR_MAX_EXCLUDE_MEMBERS");
			m_shieldTriggerPercentageHousingSpace = GetIntValue("SHIELD_TRIGGER_PERCENTAGE_HOUSING_SPACE");
			m_defaultDefenseVillageGuard = GetIntValue("DEFAULT_DEFENSE_VILLAGE_GUARD");
			m_minerTargetRandPercentage = GetIntValue("MINER_TARGET_RAND_P");
			m_minerSpeedRandPercentage = GetIntValue("MINER_SPEED_RAND_P");
			m_minerHideTime = GetIntValue("MINER_HIDE_TIME");
			m_minerHideTimeRandom = GetIntValue("MINER_HIDE_TIME_RANDOM");

			if (m_minerHideTimeRandom <= 0)
			{
				m_minerHideTimeRandom = 1;
			}

			m_townHallLootPercentage = GetIntValue("TOWN_HALL_LOOT_PERCENTAGE");
			m_charVsCharRandomDistanceLimit = (GetIntValue("CHAR_VS_CHAR_RANDOM_DIST_LIMIT") << 9) / 100;
			m_charVsCharRadiusForAttacker = GetIntValue("CHAR_VS_CHAR_RADIUS_FOR_ATTACKER") << 9;
			m_hiddenBuildingAppearDestructionPercentage = GetIntValue("HIDDEN_BUILDING_APPEAR_DESTRUCTION_PERCENTAGE");
			m_heroHealMultiplier = GetIntValue("HERO_HEAL_MULTIPLIER");
			m_heroRageMultiplier = GetIntValue("HERO_RAGE_MULTIPLIER");
			m_heroRageSpeedMultiplier = GetIntValue("HERO_RAGE_SPEED_MULTIPLIER");
			m_wallCostBase = GetIntValue("WALL_COST_BASE");

			if (m_wallCostBase > 1500)
			{
				Debugger.Warning("WALL_COST_BASE is too big");
				m_wallCostBase = 1500;
			}
			else if (m_wallCostBase < 100)
			{
				m_wallCostBase = 100;
			}

			if (m_bunkerSearchTime < 100)
			{
				Debugger.Warning("m_bunkerSearchTime too small");
			}

			if (m_townHallLootPercentage != -1 && m_townHallLootPercentage > 100)
			{
				Debugger.Error("globals.csv: Invalid loot percentage!");
			}

			m_clockTowerBoostMultiplier = GetIntValue("CLOCK_TOWER_BOOST_MULTIPLIER");
			m_resourceProductionBoostMultiplier = GetIntValue("RESOURCE_PRODUCTION_BOOST_MULTIPLIER");
			m_spellTrainingCostMultiplier = GetIntValue("SPELL_TRAINING_COST_MULTIPLIER");
			m_spellSpeedUpCostMultiplier = GetIntValue("SPELL_SPEED_UP_COST_MULTIPLIER");
			m_heroHealthSpeedUpCostMultipler = GetIntValue("HERO_HEALTH_SPEED_UP_COST_MULTIPLIER");
			m_troopRequestSpeedUpCostMultiplier = GetIntValue("TROOP_REQUEST_SPEED_UP_COST_MULTIPLIER");
			m_troopTrainingCostMultiplier = GetIntValue("TROOP_TRAINING_COST_MULTIPLIER");
			m_speedUpBoostCooldownCostMultiplier = GetIntValue("SPEEDUP_BOOST_COOLDOWN_COST_MULTIPLIER");
			m_clockTowerSpeedUpMultiplier = GetIntValue("CLOCK_TOWER_SPEEDUP_MULTIPLIER");
			m_barracksBoostMultiplier = GetIntValue("BARRACKS_BOOST_MULTIPLIER");
			m_barracksBoostNewMultiplier = GetIntValue("BARRACKS_BOOST_MULTIPLIER_NEW");
			m_spellFactoryBoostNewMultiplier = GetIntValue("SPELL_FACTORY_BOOST_MULTIPLIER_NEW");
			m_spellFactoryBoostMultiplier = GetIntValue("SPELL_FACTORY_BOOST_MULTIPLIER");
			m_heroRestBoostMultiplier = GetIntValue("HERO_REST_BOOST_MULTIPLIER");
			m_buildCancelMultiplier = GetIntValue("BUILD_CANCEL_MULTIPLIER");
			m_trainCancelMultiplier = GetIntValue("TRAIN_CANCEL_MULTIPLIER");
			m_spellCancelMultiplier = GetIntValue("SPELL_CANCEL_MULTIPLIER");
			m_heroUpgradeCancelMultiplier = GetIntValue("HERO_UPGRADE_CANCEL_MULTIPLIER");
			m_spellHousingCostMultiplier = GetIntValue("SPELL_HOUSING_COST_MULTIPLIER");
			m_unitHousingCostMultiplier = GetIntValue("UNIT_HOUSING_COST_MULTIPLIER");
			m_heroHousingCostMultiplier = GetIntValue("HERO_HOUSING_COST_MULTIPLIER");
			m_unitHousingCostMultiplierForTotal = GetIntValue("UNIT_HOUSING_COST_MULTIPLIER_FOR_TOTAL");
			m_spellHousingCostMultiplierForTotal = GetIntValue("SPELL_HOUSING_COST_MULTIPLIER_FOR_TOTAL");
			m_heroHousingCostMultiplierForTotal = GetIntValue("HERO_HOUSING_COST_MULTIPLIER_FOR_TOTAL");
			m_allianceUnitHousingCostMultiplierForTotal = GetIntValue("ALLIANCE_UNIT_HOUSING_COST_MULTIPLIER_FOR_TOTAL");
			m_bookmarksMaxAlliances = GetIntValue("BOOKMARKS_MAX_ALLIANCES");
			m_layoutSlot2THLevel = GetIntValue("LAYOUT_SLOT_2_TH_LEVEL") - 1;
			m_layoutSlot3THLevel = GetIntValue("LAYOUT_SLOT_3_TH_LEVEL") - 1;
			m_layoutSlot2THLevelVillage2 = GetIntValue("LAYOUT_SLOT_2_TH_LEVEL_VILLAGE2") - 1;
			m_layoutSlot3THLevelVillage2 = GetIntValue("LAYOUT_SLOT_3_TH_LEVEL_VILLAGE2") - 1;
			m_scoreMultiplierOnAttackLose = GetIntValue("SCORE_MULTIPLIER_ON_ATTACK_LOSE");
			m_eloOffsetDampeningFactor = GetIntValue("ELO_OFFSET_DAMPENING_FACTOR");
			m_eloOffsetDampeningLimit = GetIntValue("ELO_OFFSET_DAMPENING_LIMIT");
			m_eloOffsetDampeningScoreLimit = GetIntValue("ELO_OFFSET_DAMPENING_SCORE_LIMIT");
			m_starBonusStarCount = GetIntValue("STAR_BONUS_STAR_COUNT");
			m_lootCartEnabledForTH = GetIntValue("LOOT_CART_ENABLED_FOR_TH");
			m_warLootPercentage = GetIntValue("WAR_LOOT_PERCENTAGE");
			m_blockedAttackPositionPenalty = GetIntValue("BLOCKED_ATTACK_POSITION_PENALTY");
			m_targetListSize = GetIntValue("TARGET_LIST_SIZE");

			if (m_targetListSize <= 2)
			{
				Debugger.Error("TARGET_LIST_SIZE too small");
			}

			m_wallBreakerSmartCountLimit = GetIntValue("WALL_BREAKER_SMART_CNT_LIMIT");
			m_wallBreakerSmartRadius = (GetIntValue("WALL_BREAKER_SMART_RADIUS") << 9) / 100;
			m_wallBreakerSmartRetargetLimit = GetIntValue("WALL_BREAKER_SMART_RETARGET_LIMIT");
			m_selectedWallTime = GetIntValue("SELECTED_WALL_TIME");
			m_skeletonSpellStorageMultiplier = GetIntValue("SKELETON_SPELL_STORAGE_MULTIPLIER");
			m_allianceAlertRadius = (GetIntValue("ALLIANCE_ALERT_RADIUS") << 9) / 100;
			m_forgetTargetTime = GetIntValue("FORGET_TARGET_TIME");

			if (m_forgetTargetTime < 5000)
			{
				Debugger.Warning("FORGET_TARGET_TIME is too small");
				m_forgetTargetTime = 5000;
			}

			m_chainedProjectileBounceCount = GetIntValue("CHAINED_PROJECTILE_BOUNCE_COUNT");
			m_shrinkSpellDurationSeconds = GetIntValue("SHRINK_SPELL_DURATION_SECONDS");

			m_useNewPathFinder = GetBoolValue("USE_NEW_PATH_FINDER");
			m_useTroopWalksOutFromTraining = GetBoolValue("USE_TROOP_WALKS_OUT_FROM_TRAINING");
			m_useVillageObjects = GetBoolValue("USE_VILLAGE_OBJECTS");
			m_useVersusBattle = GetBoolValue("USE_VERSUS_BATTLE");
			m_moreAccurateTime = GetBoolValue("MORE_ACCURATE_TIME");
			m_useNewTraining = GetBoolValue("USE_NEW_TRAINING");
			m_dragInTraining = GetBoolValue("DRAG_IN_TRAINING");
			m_dragInTrainingFix = GetBoolValue("DRAG_IN_TRAINING_FIX");
			m_dragInTrainingFix2 = GetBoolValue("DRAG_IN_TRAINING_FIX2");
			m_revertBrokenWarLayouts = GetBoolValue("REVERT_BROKEN_WAR_LAYOUTS");
			m_liveReplayEnabled = GetBoolValue("LIVE_REPLAY_ENABLED");
			m_removeRevengeWhenBattleIsLoaded = GetBoolValue("REMOVE_REVENGE_WHEN_BATTLE_IS_LOADED");
			m_completeConstructionOnlyHome = GetBoolValue("COMPLETE_CONSTRUCTIONS_ONLY_HOME");
			m_useNewSpeedUpCalculation = GetBoolValue("USE_NEW_SPEEDUP_CALCULATION");
			m_clampBuildingTimes = GetBoolValue("CLAMP_BUILDING_TIMES");
			m_clampUpgradesTimes = GetBoolValue("CLAMP_UPGRADE_TIMES");
			m_clampAvatarTimersToMax = GetBoolValue("CLAMP_AVATAR_TIMERS_TO_MAX");
			m_stopBoostPauseWhenBoostTimeZeroOnLoad = GetBoolValue("STOP_BOOST_PAUSE_WHEN_BOOST_TIME_ZERO_ON_LOAD");
			m_fixClanPortalBattleNotEnding = GetBoolValue("FIX_CLAN_PORTAL_BATTLE_NOT_ENDING");
			m_fixMergeOldBarrackBoostPausing = GetBoolValue("FIX_MERGE_OLD_BARRACK_BOOST_PAUSING");
			m_saveVillageObjects = GetBoolValue("SAVE_VILLAGE_OBJECTS");
			m_workerForZeroBuildingTime = GetBoolValue("WORKER_FOR_ZERO_BUILD_TIME");
			m_adjustEndSubtickUseCurrentTime = GetBoolValue("ADJUST_END_SUBTICK_USE_CURRENT_TIME");
			m_collectAllResourcesAtOnce = GetBoolValue("COLLECT_ALL_RESOURCES_AT_ONCE");
			m_useSwapBuildings = GetBoolValue("USE_SWAP_BUILDINGS");
			m_treasurySizeBasedOnTawnHall = GetBoolValue("TREASURY_SIZE_BASED_ON_TH");
			m_startInLastUsedVillage = GetBoolValue("START_IN_LAST_USED_VILLAGE");
			m_useTeslaTriggerCommand = GetBoolValue("USE_TESLA_TRIGGER_CMD");
			m_useTrapTriggerCommand = GetBoolValue("USE_TRAP_TRIGGER_CMD");
			m_validateTroopUpgradeLevels = GetBoolValue("VALIDATE_TROOP_UPGRADE_LEVELS");
			m_allowCancelBuildingConstruction = GetBoolValue("ALLOW_CANCEL_BUILDING_CONSTRUCTION");
			m_village2TrainingOnlyUseRegularStorage = GetBoolValue("V2_TRAINING_ONLY_USE_REGULAR_STORAGE");
			m_enableTroopDeletion = GetBoolValue("ENABLE_TROOP_DELETION");
			m_enablePresets = GetBoolValue("ENABLE_PRESETS");
			m_useTownHallLootPenaltyInWar = GetBoolValue("USE_TOWNHALL_LOOT_PENALTY_IN_WAR");
			m_enableNameChange = GetBoolValue("ENABLE_NAME_CHANGE");
			m_enableQuickDonate = GetBoolValue("ENABLE_QUICK_DONATE");
			m_enableQuickDonateWar = GetBoolValue("ENABLE_QUICK_DONATE_WAR");
			m_allowClanCastleDeployOnObstacles = GetBoolValue("ALLOW_CLANCASTLE_DEPLOY_ON_OBSTACLES");
			m_skeletonTriggerTesla = GetBoolValue("SKELETON_TRIGGER_TESLA");
			m_skeletonOpenClanCastle = GetBoolValue("SKELETON_OPEN_CC");
			m_castleTroopTargetFilter = GetBoolValue("CASTLE_TROOP_TARGET_FILTER");
			m_useTroopRequestSpeedUp = GetBoolValue("USE_TROOP_REQUEST_SPEED_UP");
			m_noCooldownFromMoveEditModeActive = GetBoolValue("NO_COOLDOWN_FROM_MOVE_EDITMODE_ACTIVE");
			m_scoringOnlyFromMatchedMode = GetBoolValue("SCORING_ONLY_FROM_MM");
			m_eloOffsetDampeningEnabled = GetBoolValue("ELO_OFFSET_DAMPENING_ENABLED");
			m_enableLeagues = GetBoolValue("ENABLE_LEAGUES");
			m_revengeGiveLeagueBonus = GetBoolValue("REVENGE_GIVE_LEAGUE_BONUS");
			m_revengeGiveStarBonus = GetBoolValue("REVENGE_GIVE_STAR_BONUS");
			m_allowStarsOverflowInStarBonus = GetBoolValue("ALLOW_STARS_OVERFLOW_IN_STAR_BONUS");
			m_loadVillage2AsSnapshot = GetBoolValue("LOAD_V2_AS_SNAPSHOT");
			m_readyForWarAttackCheck = GetBoolValue("READY_FOR_WAR_ATTACK_CHECK");
			m_useMoreAccurateLootCap = GetBoolValue("USE_MORE_ACCURATE_LOOT_CAP");
			m_enableDefendingAllianceTroopJump = GetBoolValue("ENABLE_DEFENDING_ALLIANCE_TROOP_JUMP");
			m_useWallWeightsForJumpSpell = GetBoolValue("USE_WALL_WEIGHTS_FOR_JUMP_SPELL");
			m_jumpWhenHitJumpable = GetBoolValue("JUMP_WHEN_HIT_JUMPABLE");
			m_slideAlongObstacles = GetBoolValue("SLIDE_ALONG_OBSTACLES");
			m_guardPostNotFunctionalUnderUpgrade = GetBoolValue("GUARD_POST_NOT_FUNCTIONAL_UNDER_UGPRADE");
			m_repathDuringFly = GetBoolValue("REPATH_DURING_FLY");
			m_useStickToClosestUnitHealer = GetBoolValue("USE_STICK_TO_CLOSEST_UNIT_HEALER");
			m_heroUsesAttackPosRandom = GetBoolValue("HERO_USES_ATTACK_POS_RANDOM");
			m_useAttackPosRandomOn1stTarget = GetBoolValue("USE_ATTACK_POS_RANDOM_ON_1ST_TARGET");
			m_targetSelectionConsidersWallsOnPath = GetBoolValue("TARGET_SELECTION_CONSIDERS_WALLS_ON_PATH");
			m_valkyriePrefers4Buildings = GetBoolValue("VALKYRIE_PREFERS_4_BUILDINGS");
			m_tighterAttackPosition = GetBoolValue("TIGHTER_ATTACK_POSITION");
			m_allianceTroopsPatrol = GetBoolValue("ALLIANCE_TROOPS_PATROL");
			m_wallBreakerUseRooms = GetBoolValue("WALL_BREAKER_USE_ROOMS");
			m_rememberOriginalTarget = GetBoolValue("REMEMBER_ORIGINAL_TARGET");
			m_ignoreAllianceAlertForNonValidTargets = GetBoolValue("IGNORE_ALLIANCE_ALERT_FOR_NON_VALID_TARGETS");
			m_restartAttackTimerOnAreaDamageTurrets = GetBoolValue("RESTART_ATTACK_TIMER_ON_AREA_DAMAGE_TURRETS");
			m_clearAlertStateIfNoTargetFound = GetBoolValue("CLEAR_ALERT_STATE_IF_NO_TARGET_FOUND");
			m_morePreciseTargetSelection = GetBoolValue("MORE_PRECISE_TARGET_SELECTION");
			m_movingUnitsUseSimpleSelect = GetBoolValue("MOVING_UNITS_USE_SIMPLE_SELECT");
			m_useSmarterHealer = GetBoolValue("USE_SMARTER_HEALER");
			m_usePoisonAvoidance = GetBoolValue("USE_POISON_AVOIDANCE");
			m_removeUntriggeredTesla = GetBoolValue("REMOVE_UNTRIGGERED_TESLA");

			m_allianceCreateResourceData = LogicDataTables.GetResourceByName(GetGlobalData("ALLIANCE_CREATE_RESOURCE").GetTextValue(), null);
			m_village2StartUnit = LogicDataTables.GetCharacterByName(GetGlobalData("VILLAGE2_START_UNIT").GetTextValue(), null);

			LogicGlobalData village2TroopHousingBuildCostData = GetGlobalData("TROOP_HOUSING_V2_COST");

			m_village2TroopHousingBuildCost = new int[village2TroopHousingBuildCostData.GetNumberArraySize()];

			for (int i = 0; i < m_village2TroopHousingBuildCost.Length; i++)
			{
				m_village2TroopHousingBuildCost[i] = village2TroopHousingBuildCostData.GetNumberArray(i);
			}

			LogicGlobalData village2TroopHousingBuildTimeSecsData = GetGlobalData("TROOP_HOUSING_V2_BUILD_TIME_SECONDS");

			m_village2TroopHousingBuildTimeSecs = new int[village2TroopHousingBuildTimeSecsData.GetNumberArraySize()];

			for (int i = 0; i < m_village2TroopHousingBuildTimeSecs.Length; i++)
			{
				m_village2TroopHousingBuildTimeSecs[i] = village2TroopHousingBuildTimeSecsData.GetNumberArray(i);
			}

			LogicGlobalData lootMultiplierByTownHallDifferenceObject = GetGlobalData("LOOT_MULTIPLIER_BY_TH_DIFF");

			m_lootMultiplierByTownHallDifference = new int[lootMultiplierByTownHallDifferenceObject.GetNumberArraySize()];

			for (int i = 0; i < m_lootMultiplierByTownHallDifference.Length; i++)
			{
				m_lootMultiplierByTownHallDifference[i] = lootMultiplierByTownHallDifferenceObject.GetNumberArray(i);
			}

			LogicGlobalData barrackReduceTrainingDivisorObject = GetGlobalData("BARRACK_REDUCE_TRAINING_DIVISOR");

			m_barrackReduceTrainingDivisor = new int[barrackReduceTrainingDivisorObject.GetNumberArraySize()];

			for (int i = 0; i < m_barrackReduceTrainingDivisor.Length; i++)
			{
				m_barrackReduceTrainingDivisor[i] = barrackReduceTrainingDivisorObject.GetNumberArray(i);
			}

			LogicGlobalData darkBarrackReduceTrainingDivisorObject = GetGlobalData("DARK_BARRACK_REDUCE_TRAINING_DIVISOR");

			m_darkBarrackReduceTrainingDivisor = new int[darkBarrackReduceTrainingDivisorObject.GetNumberArraySize()];

			for (int i = 0; i < m_darkBarrackReduceTrainingDivisor.Length; i++)
			{
				m_darkBarrackReduceTrainingDivisor[i] = darkBarrackReduceTrainingDivisorObject.GetNumberArray(i);
			}

			LogicGlobalData clockTowerBoostObject = GetGlobalData("CLOCK_TOWER_BOOST_MINS");

			m_clockTowerBoostSecs = new int[clockTowerBoostObject.GetNumberArraySize()];

			for (int i = 0; i < m_clockTowerBoostSecs.Length; i++)
			{
				m_clockTowerBoostSecs[i] = clockTowerBoostObject.GetNumberArray(i) * 60;
			}

			LogicGlobalData allianceScoreLimitObject = GetGlobalData("ALLIANCE_SCORE_LIMIT");

			m_allianceScoreLimit = new int[allianceScoreLimitObject.GetNumberArraySize()];

			for (int i = 0; i < m_allianceScoreLimit.Length; i++)
			{
				m_allianceScoreLimit[i] = allianceScoreLimitObject.GetNumberArray(i);
			}

			LogicGlobalData shieldHoursObject = GetGlobalData("SHIELD_HOURS");

			m_shieldHours = new int[shieldHoursObject.GetNumberArraySize()];

			for (int i = 0; i < m_shieldHours.Length; i++)
			{
				m_shieldHours[i] = shieldHoursObject.GetNumberArray(i);
			}

			LogicGlobalData destructionToShieldObject = GetGlobalData("DESTRUCTION_TO_SHIELD");

			m_destructionToShield = new int[destructionToShieldObject.GetNumberArraySize()];

			for (int i = 0; i < m_destructionToShield.Length; i++)
			{
				m_destructionToShield[i] = destructionToShieldObject.GetNumberArray(i);
			}

			Debugger.DoAssert(m_shieldHours.Length == m_destructionToShield.Length, string.Empty);
			LogicGlobalData attackShieldReduceHoursObject = GetGlobalData("ATTACK_SHIELD_REDUCE_HOURS");

			m_attackShieldReduceHours = new int[attackShieldReduceHoursObject.GetNumberArraySize()];

			for (int i = 0; i < m_attackShieldReduceHours.Length; i++)
			{
				m_attackShieldReduceHours[i] = attackShieldReduceHoursObject.GetNumberArray(i);
			}

			LogicGlobalData healStackPercentObject = GetGlobalData("HEAL_STACK_PERCENT");

			m_healStackPercent = new int[healStackPercentObject.GetNumberArraySize()];

			for (int i = 0; i < healStackPercentObject.GetNumberArraySize(); i++)
			{
				m_healStackPercent[i] = healStackPercentObject.GetNumberArray(i);
			}

			LogicGlobalData leagueBonusPercentageObject = GetGlobalData("LEAGUE_BONUS_PERCENTAGES");

			m_leagueBonusPercentages = new int[leagueBonusPercentageObject.GetNumberArraySize()];
			m_leagueBonusAltPercentages = new int[leagueBonusPercentageObject.GetNumberArraySize()];

			for (int i = 0; i < m_leagueBonusPercentages.Length; i++)
			{
				m_leagueBonusPercentages[i] = leagueBonusPercentageObject.GetNumberArray(i);
				m_leagueBonusAltPercentages[i] = leagueBonusPercentageObject.GetAltNumberArray(i);
			}
		}

		private LogicGlobalData GetGlobalData(string name)
			=> LogicDataTables.GetGlobalByName(name, null);

		private bool GetBoolValue(string name)
			=> GetGlobalData(name).GetBooleanValue();

		private int GetIntValue(string name)
			=> GetGlobalData(name).GetNumberValue();

		public int GetFreeUnitHousingCapPercentage()
			=> m_freeUnitHousingCapPercentage;

		public int GetFreeHeroHealthCap()
			=> m_freeHeroHealthCap;

		public int GetStartingDiamonds()
			=> m_startingDiamonds;

		public int GetStartingGold()
			=> m_startingGold;

		public int GetStartingElixir()
			=> m_startingElixir;

		public int GetStartingGold2()
			=> m_startingGold2;

		public int GetStartingElixir2()
			=> m_startingElixir2;

		public int GetLiveReplayUpdateFrequencySecs()
			=> m_liveReplayFrequencySecs;

		public int GetChallengeBaseSaveCooldown()
			=> m_challengeBaseSaveCooldown;

		public int GetAllianceTroopRequestCooldown()
			=> m_allianceTroopRequestCooldown;

		public int GetArrangeWarCooldown()
			=> m_arrangeWarCooldown;

		public int GetClanMailCooldown()
			=> m_clanMailCooldown;

		public int GetReplayShareCooldown()
			=> m_replayShareCooldown;

		public int GetElderKickCooldown()
			=> m_elderKickCooldown;

		public int GetAllianceCreateCost()
			=> m_allianceCreateCost;

		public int GetClockTowerBoostMultiplier()
			=> m_clockTowerBoostMultiplier;

		public int GetResourceProductionBoostMultiplier()
			=> m_resourceProductionBoostMultiplier;

		public int GetResourceProductionBoostSecs()
			=> m_resourceProductionBoostSecs;

		public int GetSpellFactoryBoostMultiplier()
			=> m_spellFactoryBoostMultiplier;

		public int GetSpellFactoryBoostNewMultiplier()
			=> m_spellFactoryBoostNewMultiplier;

		public int GetSpellFactoryBoostSecs()
			=> m_spellFactoryBoostSecs;

		public int GetBarracksBoostNewMultiplier()
			=> m_barracksBoostNewMultiplier;

		public int GetBarracksBoostMultiplier()
			=> m_barracksBoostMultiplier;

		public int GetBuildCancelMultiplier()
			=> m_buildCancelMultiplier;

		public int GetTrainCancelMultiplier()
			=> m_trainCancelMultiplier;

		public int GetSpellCancelMultiplier()
			=> m_spellCancelMultiplier;

		public int GetHeroUpgradeCancelMultiplier()
			=> m_heroUpgradeCancelMultiplier;

		public int GetBarracksBoostSecs()
			=> m_barracksBoostSecs;

		public int GetClockTowerBoostCooldownSecs()
			=> m_clockTowerBoostCooldownSecs;

		public int GetHeroRestBoostSecs()
			=> m_heroRestBoostSecs;

		public int GetClampLongTimeStampsToDays()
			=> m_clampLongTimeStampsToDays;

		public int GetObstacleRespawnSecs()
			=> m_obstacleRespawnSecs;

		public int GetTallGrassRespawnSecs()
			=> m_tallGrassRespawnSecs;

		public int GetObstacleMaxCount()
			=> m_obstacleMaxCount;

		public int GetNewTrainingBoostBarracksCost()
			=> m_newTrainingBoostBarracksCost;

		public int GetNewTrainingBoostLaboratoryCost()
			=> m_newTrainingBoostLaboratoryCost;

		public int GetUnitHousingCostMultiplierForTotal()
			=> m_unitHousingCostMultiplierForTotal;

		public int GetSpellHousingCostMultiplierForTotal()
			=> m_spellHousingCostMultiplierForTotal;

		public int GetHeroHousingCostMultiplierForTotal()
			=> m_heroHousingCostMultiplierForTotal;

		public int GetAllianceUnitHousingCostMultiplierForTotal()
			=> m_allianceUnitHousingCostMultiplierForTotal;

		public int GetPersonalBreakLimitSeconds()
			=> m_personalBreakLimitSeconds;

		public int GetMaxTroopDonationCount()
			=> m_maxTroopDonationCount;

		public int GetMaxSpellDonationCount()
			=> m_maxSpellDonationCount;

		public int GetDarkSpellDonationXP()
			=> m_darkSpellDonationXP;

		public int GetStarBonusCooldownMinutes()
			=> m_starBonusCooldownMinutes;

		public int GetChallengeCooldown()
			=> m_challengeCooldown;

		public int GetNewbieShieldHours()
			=> m_newbieShieldHours;

		public int GetLayoutTownHallLevelSlot2()
			=> m_layoutSlot2THLevel;

		public int GetLayoutTownHallLevelSlot3()
			=> m_layoutSlot3THLevel;

		public int GetLayoutTownHallLevelVillage2Slot2()
			=> m_layoutSlot2THLevelVillage2;

		public int GetLayoutTownHallLevelVillage2Slot3()
			=> m_layoutSlot3THLevelVillage2;

		public int GetScoreMultiplierOnAttackLose()
			=> m_scoreMultiplierOnAttackLose;

		public int GetEloDampeningFactor()
			=> m_eloOffsetDampeningFactor;

		public int GetEloDampeningLimit()
			=> m_eloOffsetDampeningLimit;

		public int GetEloDampeningScoreLimit()
			=> m_eloOffsetDampeningScoreLimit;

		public int GetShieldTriggerPercentageHousingSpace()
			=> m_shieldTriggerPercentageHousingSpace;

		public int GetDefaultDefenseVillageGuard()
			=> m_defaultDefenseVillageGuard;

		public int GetMinerTargetRandomPercentage()
			=> m_minerTargetRandPercentage;

		public int GetMinerSpeedRandomPercentage()
			=> m_minerSpeedRandPercentage;

		public int GetMinerHideTime()
			=> m_minerHideTime;

		public int GetMinerHideTimeRandom()
			=> m_minerHideTimeRandom;

		public int GetTownHallLootPercentage()
			=> m_townHallLootPercentage;

		public int GetTargetListSize()
			=> m_targetListSize;

		public int GetWallBreakerSmartCountLimit()
			=> m_wallBreakerSmartCountLimit;

		public int GetWallBreakerSmartRadius()
			=> m_wallBreakerSmartRadius;

		public int GetWallBreakerSmartRetargetLimit()
			=> m_wallBreakerSmartRetargetLimit;

		public int GetSelectedWallTime()
			=> m_selectedWallTime;

		public int GetForgetTargetTime()
			=> m_forgetTargetTime;

		public int GetSkeletonSpellStorageMultipler()
			=> m_skeletonSpellStorageMultiplier;

		public int GetAllianceAlertRadius()
			=> m_allianceAlertRadius;

		public int GetHiddenBuildingAppearDestructionPercentage()
			=> m_hiddenBuildingAppearDestructionPercentage;

		public int GetWallCostBase()
			=> m_wallCostBase;

		public int GetHeroHealMultiplier()
			=> m_heroHealMultiplier;

		public int GetHeroRageMultiplier()
			=> m_heroRageMultiplier;

		public int GetHeroRageSpeedMultiplier()
			=> m_heroRageSpeedMultiplier;

		public int GetChainedProjectileBounceCount()
			=> m_chainedProjectileBounceCount;

		public int GetShrinkSpellDurationSeconds()
			=> m_shrinkSpellDurationSeconds;

		public int GetResourceProductionLootPercentage(LogicResourceData data)
		{
			if (LogicDataTables.GetDarkElixirData() == data)
			{
				return m_darkElixirProductionLootPercentage;
			}

			return m_resourceProductionLootPercentage;
		}

		public int GetLootMultiplierByTownHallDiff(int townHallLevel1, int townHallLevel2)
			=> m_lootMultiplierByTownHallDifference[LogicMath.Clamp(townHallLevel1 + 4 - townHallLevel2, 0, m_lootMultiplierByTownHallDifference.Length - 1)];

		public int[] GetBarrackReduceTrainingDevisor()
			=> m_barrackReduceTrainingDivisor;

		public int[] GetDarkBarrackReduceTrainingDevisor()
			=> m_darkBarrackReduceTrainingDivisor;

		public int GetWorkerCost(LogicLevel level)
		{
			int totalWorkers = level.GetWorkerManagerAt(level.GetVillageType()).GetTotalWorkers() + level.GetUnplacedObjectCount(LogicDataTables.GetWorkerData());

			switch (totalWorkers)
			{
				case 1:
					return m_workerCostSecondBuildCost;
				case 2:
					return m_workerCostThirdBuildCost;
				case 3:
					return m_workerCostFourthBuildCost;
				case 4:
					return m_workerCostFifthBuildCost;
				default:
					return m_workerCostFifthBuildCost;
			}
		}

		public int GetChallengeBaseCooldownEnabledTownHall()
			=> m_challengeBaseCooldownEnabledOnTh;

		public int GetSpellTrainingCostMultiplier()
			=> m_spellTrainingCostMultiplier;

		public int GetSpellSpeedUpCostMultiplier()
			=> m_spellSpeedUpCostMultiplier;

		public int GetHeroHealthSpeedUpCostMultipler()
			=> m_heroHealthSpeedUpCostMultipler;

		public int GetTroopRequestSpeedUpCostMultiplier()
			=> m_troopRequestSpeedUpCostMultiplier;

		public int GetTroopTrainingCostMultiplier()
			=> m_troopTrainingCostMultiplier;

		public int GetSpeedUpBoostCooldownCostMultiplier()
			=> m_speedUpBoostCooldownCostMultiplier;

		public int GetClockTowerSpeedUpMultiplier()
			=> m_clockTowerSpeedUpMultiplier;

		public int GetMinVillage2TownHallLevelForDestructObstacle()
			=> m_village2MinTownHallLevelForDestructObstacle;

		public int GetAttackPreparationLengthSecs()
			=> m_attackPreparationLengthSecs;

		public int GetAttackVillage2PreparationLengthSecs()
			=> m_attackVillage2PreparationLengthSecs;

		public int GetAttackLengthSecs()
			=> m_attackLengthSecs;

		public int GetVillage2StartUnitLevel()
			=> m_village2StartUnitLevel;

		public int GetHeroRestBoostMultiplier()
			=> m_heroRestBoostMultiplier;

		public int GetEnablePresetsTownHallLevel()
			=> m_enablePresetsTownHallLevel;

		public int GetMaxAllianceFeedbackMessageLength()
			=> m_maxAllianceFeedbackMessageLength;

		public int GetMaxMessageLength()
			=> m_maxMessageLength;

		public int GetAllianceMailLength()
			=> m_maxAllianceMailLength;

		public int GetUnitHousingCostMultiplier()
			=> m_unitHousingCostMultiplier;

		public int GetHeroHousingCostMultiplier()
			=> m_heroHousingCostMultiplier;

		public int GetSpellHousingCostMultiplier()
			=> m_spellHousingCostMultiplier;

		public int GetEnableNameChangeTownHallLevel()
			=> m_enableNameChangeTownHallLevel;

		public int GetDuelLootLimitFreeSpeedUps()
			=> m_duelLootLimitFreeSpeedUps;

		public int GetBunkerSearchTime()
			=> m_bunkerSearchTime;

		public int GetClanCastleRadius()
			=> m_clanCastleRadius;

		public int GetClanDefenderSearchRadius()
			=> m_clanDefenderSearchRadius;

		public int GetLootCartReengagementMinSeconds()
			=> m_lootCartReengagementMinSecs;

		public int GetLootCartReengagementMaxSeconds()
			=> m_lootCartReengagementMaxSecs;

		public int GetBookmarksMaxAlliances()
			=> m_bookmarksMaxAlliances;

		public int GetStarBonusStarCount()
			=> m_starBonusStarCount;

		public int GetLootCartEnabledTownHall()
			=> m_lootCartEnabledForTH;

		public int GetWarMaxExcludeMembers()
			=> m_warMaxExcludeMembers;

		public int GetCharVersusCharRandomDistanceLimit()
			=> m_charVsCharRandomDistanceLimit;

		public int GetCharVersusCharRadiusForAttacker()
			=> m_charVsCharRadiusForAttacker;

		public int GetWarLootPercentage()
			=> m_warLootPercentage;

		public int GetBlockedAttackPositionPenalty()
			=> m_blockedAttackPositionPenalty;

		public bool CastleTroopTargetFilter()
			=> m_castleTroopTargetFilter;

		public bool MoreAccurateTime()
			=> m_moreAccurateTime;

		public bool UseNewTraining()
			=> m_useNewTraining;

		public bool UseTroopWalksOutFromTraining()
			=> m_useTroopWalksOutFromTraining;

		public bool UseVillageObjects()
			=> m_useVillageObjects;

		public bool UseTownHallLootPenaltyInWar()
			=> m_useTownHallLootPenaltyInWar;

		public bool UseDragInTraining()
			=> m_dragInTraining;

		public bool UseDragInTrainingFix()
			=> m_dragInTrainingFix;

		public bool UseDragInTrainingFix2()
			=> m_dragInTrainingFix2;

		public bool RevertBrokenWarLayouts()
			=> m_revertBrokenWarLayouts;

		public bool LiveReplayEnabled()
			=> m_liveReplayEnabled;

		public bool RemoveRevengeWhenBattleIsLoaded()
			=> m_removeRevengeWhenBattleIsLoaded;

		public bool UseNewPathFinder()
			=> m_useNewPathFinder;

		public bool CompleteConstructionOnlyHome()
			=> m_completeConstructionOnlyHome;

		public bool UseNewSpeedUpCalculation()
			=> m_useNewSpeedUpCalculation;

		public bool ClampBuildingTimes()
			=> m_clampBuildingTimes;

		public bool ClampUpgradeTimes()
			=> m_clampUpgradesTimes;

		public bool ClampAvatarTimersToMax()
			=> m_clampAvatarTimersToMax;

		public bool StopBoostPauseWhenBoostTimeZeroOnLoad()
			=> m_stopBoostPauseWhenBoostTimeZeroOnLoad;

		public bool FixClanPortalBattleNotEnding()
			=> m_fixClanPortalBattleNotEnding;

		public bool FixMergeOldBarrackBoostPausing()
			=> m_fixMergeOldBarrackBoostPausing;

		public bool SaveVillageObjects()
			=> m_saveVillageObjects;

		public bool StartInLastUsedVillage()
			=> m_startInLastUsedVillage;

		public bool WorkerForZeroBuilTime()
			=> m_workerForZeroBuildingTime;

		public bool AdjustEndSubtickUseCurrentTime()
			=> m_adjustEndSubtickUseCurrentTime;

		public bool CollectAllResourcesAtOnce()
			=> m_collectAllResourcesAtOnce;

		public bool UseSwapBuildings()
			=> m_useSwapBuildings;

		public bool TreasurySizeBasedOnTownHall()
			=> m_treasurySizeBasedOnTawnHall;

		public bool UseTeslaTriggerCommand()
			=> m_useTeslaTriggerCommand;

		public bool UseTrapTriggerCommand()
			=> m_useTrapTriggerCommand;

		public bool ValidateTroopUpgradeLevels()
			=> m_validateTroopUpgradeLevels;

		public bool AllowCancelBuildingConstruction()
			=> m_allowCancelBuildingConstruction;

		public bool Village2TrainingOnlyUseRegularStorage()
			=> m_village2TrainingOnlyUseRegularStorage;

		public bool EnableTroopDeletion()
			=> m_enableTroopDeletion;

		public bool EnablePresets()
			=> m_enablePresets;

		public bool EnableNameChange()
			=> m_enableNameChange;

		public bool EnableQuickDonate()
			=> m_enableQuickDonate;

		public bool EnableQuickDonateWar()
			=> m_enableQuickDonateWar;

		public bool AllowClanCastleDeployOnObstacles()
			=> m_allowClanCastleDeployOnObstacles;

		public bool SkeletonTriggerTesla()
			=> m_skeletonTriggerTesla;

		public bool SkeletonOpenClanCastle()
			=> m_skeletonOpenClanCastle;

		public bool UseTroopRequestSpeedUp()
			=> m_useTroopRequestSpeedUp;

		public bool NoCooldownFromMoveEditModeActive()
			=> m_noCooldownFromMoveEditModeActive;

		public bool UseVersusBattle()
			=> m_useVersusBattle;

		public bool ScoringOnlyFromMatchedMode()
			=> m_scoringOnlyFromMatchedMode;

		public bool EloOffsetDampeningEnabled()
			=> m_eloOffsetDampeningEnabled;

		public bool EnableLeagues()
			=> m_enableLeagues;

		public bool RevengeGiveLeagueBonus()
			=> m_revengeGiveLeagueBonus;

		public bool RevengeGiveStarBonus()
			=> m_revengeGiveStarBonus;

		public bool AllowStarsOverflowInStarBonus()
			=> m_allowStarsOverflowInStarBonus;

		public bool LoadVillage2AsSnapshot()
			=> m_loadVillage2AsSnapshot;

		public bool ReadyForWarAttackCheck()
			=> m_readyForWarAttackCheck;

		public bool UseMoreAccurateLootCap()
			=> m_useMoreAccurateLootCap;

		public bool EnableDefendingAllianceTroopJump()
			=> m_enableDefendingAllianceTroopJump;

		public bool UseWallWeightsForJumpSpell()
			=> m_useWallWeightsForJumpSpell;

		public bool JumpWhenHitJumpable()
			=> m_jumpWhenHitJumpable;

		public bool SlideAlongObstacles()
			=> m_slideAlongObstacles;

		public bool GuardPostNotFunctionalUnderUpgrade()
			=> m_guardPostNotFunctionalUnderUpgrade;

		public bool RepathDuringFly()
			=> m_repathDuringFly;

		public bool UseStickToClosestUnitHealer()
			=> m_useStickToClosestUnitHealer;

		public bool HeroUsesAttackPosRandom()
			=> m_heroUsesAttackPosRandom;

		public bool UseAttackPosRandomOn1stTarget()
			=> m_useAttackPosRandomOn1stTarget;

		public bool TargetSelectionConsidersWallsOnPath()
			=> m_targetSelectionConsidersWallsOnPath;

		public bool ValkyriePrefers4Buildings()
			=> m_valkyriePrefers4Buildings;

		public bool TighterAttackPosition()
			=> m_tighterAttackPosition;

		public bool AllianceTroopsPatrol()
			=> m_allianceTroopsPatrol;

		public bool WallBreakerUseRooms()
			=> m_wallBreakerUseRooms;

		public bool RememberOriginalTarget()
			=> m_rememberOriginalTarget;

		public bool IgnoreAllianceAlertForNonValidTargets()
			=> m_ignoreAllianceAlertForNonValidTargets;

		public bool RestartAttackTimerOnAreaDamageTurrets()
			=> m_restartAttackTimerOnAreaDamageTurrets;

		public bool ClearAlertStateIfNoTargetFound()
			=> m_clearAlertStateIfNoTargetFound;

		public bool MorePreciseTargetSelection()
			=> m_morePreciseTargetSelection;

		public bool MovingUnitsUseSimpleSelect()
			=> m_movingUnitsUseSimpleSelect;

		public bool UseSmarterHealer()
			=> m_useSmarterHealer;

		public bool UsePoisonAvoidance()
			=> m_usePoisonAvoidance;

		public bool RemoveUntriggeredTesla()
			=> m_removeUntriggeredTesla;

		public LogicResourceData GetAllianceCreateResourceData()
			=> m_allianceCreateResourceData;

		public LogicCharacterData GetVillage2StartUnitData()
			=> m_village2StartUnit;

		public LogicResourceData GetAttackResource()
			=> LogicDataTables.GetGoldData();

		public int GetVillage2FirstVictoryTrophies()
			=> m_village2FirstVictoryTrophies;

		public int GetVillage2FirstVictoryGold()
			=> m_village2FirstVictoryGold;

		public int GetVillage2FirstVictoryElixir()
			=> m_village2FirstVictoryElixir;

		public int GetFriendlyBattleCost(int townHallLevel)
			=> LogicDataTables.GetTownHallLevel(townHallLevel).GetFriendlyCost();

		public int GetTroopHousingBuildCostVillage2(LogicLevel level)
		{
			LogicBuildingData data = LogicDataTables.GetBuildingByName("Troop Housing2", null);

			if (data != null)
			{
				return m_village2TroopHousingBuildCost[LogicMath.Clamp(level.GetGameObjectManagerAt(1).GetGameObjectCountByData(data),
																			0,
																			m_village2TroopHousingBuildCost.Length - 1)];
			}

			Debugger.Error("Could not find Troop Housing2 data");

			return 0;
		}

		public int GetTroopHousingBuildTimeVillage2(LogicLevel level, int ignoreBuildingCnt)
		{
			LogicBuildingData data = LogicDataTables.GetBuildingByName("Troop Housing2", null);

			if (data != null)
			{
				return m_village2TroopHousingBuildTimeSecs[LogicMath.Clamp(level.GetGameObjectManagerAt(1).GetGameObjectCountByData(data) - ignoreBuildingCnt,
																				0,
																				m_village2TroopHousingBuildTimeSecs.Length - 1)];
			}

			Debugger.Error("Could not find Troop Housing2 data");

			return 0;
		}

		public int GetClockTowerBoostSecs(int upgLevel)
		{
			if (m_clockTowerBoostSecs.Length > upgLevel)
			{
				return m_clockTowerBoostSecs[upgLevel];
			}

			return m_clockTowerBoostSecs[m_clockTowerBoostSecs.Length - 1];
		}

		public int GetTutorialTrainingSpeedUpCost()
			=> m_troopTrainingSpeedUpCostTutorial;

		public int GetHealStackPercent(int idx)
		{
			if (m_healStackPercent.Length != 0)
			{
				if (idx >= m_healStackPercent.Length)
				{
					idx = m_healStackPercent.Length - 1;
				}

				return m_healStackPercent[idx];
			}

			return 100;
		}

		public int GetResourceDiamondCost(int count, LogicResourceData data)
		{
			if (LogicDataTables.GetDarkElixirData() != data)
			{
				int resourceDiamondCost100;
				int resourceDiamondCost1000;
				int resourceDiamondCost10000;
				int resourceDiamondCost100000;
				int resourceDiamondCost1000000;
				int resourceDiamondCost10000000;

				if (data.GetVillageType() == 1)
				{
					resourceDiamondCost100 = m_village2ResourceDiamondCost100;
					resourceDiamondCost1000 = m_village2ResourceDiamondCost1000;
					resourceDiamondCost10000 = m_village2ResourceDiamondCost10000;
					resourceDiamondCost100000 = m_village2resourceDiamondCost100000;
					resourceDiamondCost1000000 = m_village2resourceDiamondCost1000000;
					resourceDiamondCost10000000 = m_village2ResourceDiamondCost10000000;
				}
				else
				{
					resourceDiamondCost100 = m_resourceDiamondCost100;
					resourceDiamondCost1000 = m_resourceDiamondCost1000;
					resourceDiamondCost10000 = m_resourceDiamondCost10000;
					resourceDiamondCost100000 = m_resourceDiamondCost100000;
					resourceDiamondCost1000000 = m_resourceDiamondCost1000000;
					resourceDiamondCost10000000 = m_resourceDiamondCost10000000;
				}

				if (count >= 1)
				{
					if (count >= 100)
					{
						if (count >= 1000)
						{
							if (count >= 10000)
							{
								if (count >= 100000)
								{
									if (count >= 1000000)
									{
										return resourceDiamondCost1000000 + ((resourceDiamondCost10000000 - resourceDiamondCost1000000) * (count / 1000 - 1000) + 4500) / 9000;
									}

									return resourceDiamondCost100000 + ((resourceDiamondCost1000000 - resourceDiamondCost100000) * (count / 100 - 1000) + 4500) / 9000;
								}

								return resourceDiamondCost10000 + ((resourceDiamondCost100000 - resourceDiamondCost10000) * (count / 10 - 1000) + 4500) / 9000;
							}

							return resourceDiamondCost1000 + ((resourceDiamondCost10000 - resourceDiamondCost1000) * (count - 1000) + 4500) / 9000;
						}

						return resourceDiamondCost100 + ((resourceDiamondCost1000 - resourceDiamondCost100) * (count - 100) + 450) / 900;
					}

					return resourceDiamondCost100;
				}

				return 0;
			}

			return GetDarkElixirDiamondCost(count);
		}

		public int GetDarkElixirDiamondCost(int count)
		{
			if (count >= 1)
			{
				if (count >= 10)
				{
					if (count >= 100)
					{
						if (count >= 1000)
						{
							if (count >= 10000)
							{
								return m_darkElixirDiamondCost10000 +
									   ((m_darkElixirDiamondCost100000 - m_darkElixirDiamondCost10000) * (count - 10000) + 45000) / 90000;
							}

							return m_darkElixirDiamondCost1000 + ((m_darkElixirDiamondCost10000 - m_darkElixirDiamondCost1000) * (count - 1000) + 4500) / 9000;
						}

						return m_darkElixirDiamondCost100 + ((m_darkElixirDiamondCost1000 - m_darkElixirDiamondCost100) * (count - 100) + 450) / 900;
					}

					return m_darkElixirDiamondCost10 + ((m_darkElixirDiamondCost100 - m_darkElixirDiamondCost10) * (count - 10) + 45) / 90;
				}

				return m_darkElixirDiamondCost1 + ((m_darkElixirDiamondCost10 - m_darkElixirDiamondCost1) * (count - 1) + 4) / 9;
			}

			return 0;
		}

		public int GetSpeedUpCost(int time, int multiplier, int villageType)
		{
			if (time > 0)
			{
				int speedUpDiamondCostPerMin;
				int speedUpDiamondCostPerHour;
				int speedUpDiamondCostPerDay;
				int speedUpDiamondCostPerWeek;

				if (villageType == 1)
				{
					speedUpDiamondCostPerMin = m_speedUpDiamondCostPerMinVillage2;
					speedUpDiamondCostPerHour = m_speedUpDiamondCostPerHourVillage2;
					speedUpDiamondCostPerDay = m_speedUpDiamondCostPerDayVillage2;
					speedUpDiamondCostPerWeek = m_speedUpDiamondCostPerWeekVillage2;
				}
				else
				{
					speedUpDiamondCostPerMin = m_speedUpDiamondCostPerMin;
					speedUpDiamondCostPerHour = m_speedUpDiamondCostPerHour;
					speedUpDiamondCostPerDay = m_speedUpDiamondCostPerDay;
					speedUpDiamondCostPerWeek = m_speedUpDiamondCostPerWeek;
				}

				int multiplier1 = multiplier;
				int multiplier2 = 100;

				if (m_useNewSpeedUpCalculation)
				{
					multiplier1 = 100;
					multiplier2 = multiplier;
				}

				int cost = speedUpDiamondCostPerMin;

				if (time >= 60)
				{
					if (time >= 3600)
					{
						if (time >= 86400)
						{
							int tmp1 = (speedUpDiamondCostPerWeek - speedUpDiamondCostPerDay) * (time - 86400);

							cost = multiplier2 * speedUpDiamondCostPerDay / 100 + tmp1 / 100 * multiplier2 / 518400;

							if (cost < 0 || tmp1 / 100 > 0x7FFFFFFF / multiplier2)
							{
								cost = multiplier2 * (speedUpDiamondCostPerDay + tmp1 / 518400) / 100;
							}
						}
						else
						{
							cost = multiplier2 * speedUpDiamondCostPerHour / 100 +
								   (speedUpDiamondCostPerDay - speedUpDiamondCostPerHour) * (time - 3600) / 100 * multiplier2 / 82800;
						}
					}
					else
					{
						cost = multiplier2 * speedUpDiamondCostPerMin / 100 + (speedUpDiamondCostPerHour - speedUpDiamondCostPerMin) * (time - 60) * multiplier2 / 354000;
					}
				}
				else if (m_useNewSpeedUpCalculation)
				{
					cost = multiplier2 * speedUpDiamondCostPerMin * time / 6000;
				}

				return LogicMath.Max(cost * multiplier1 / 100, 1);
			}

			return 0;
		}

		public int GetLeagueBonusPercentage(int destructionPercentage)
		{
			if (m_leagueBonusPercentages.Length != 0 && m_leagueBonusAltPercentages.Length != 0)
			{
				for (int i = 0, j = 0, k = 0; i < m_leagueBonusPercentages.Length; i++)
				{
					if (m_leagueBonusPercentages[i] >= destructionPercentage)
					{
						return k + (m_leagueBonusAltPercentages[i] - k) * (destructionPercentage - j) / (m_leagueBonusPercentages[i] - j);
					}

					j = m_leagueBonusPercentages[i];
					k = m_leagueBonusAltPercentages[i];
				}
			}

			return 100;
		}

		public int GetAllianceScoreLimit(int idx)
			=> m_allianceScoreLimit[idx];

		public int GetAllianceScoreLimitCount()
			=> m_allianceScoreLimit.Length;

		public int GetDestructionToShield(int destructionPercentage)
		{
			int shield = 0;

			for (int i = 0; i < m_destructionToShield.Length; i++)
			{
				if (m_destructionToShield[i] <= destructionPercentage)
				{
					shield = m_shieldHours[i];
				}
			}

			return shield;
		}

		public int GetAttackShieldReduceHours(int idx)
		{
			if (idx >= m_attackShieldReduceHours.Length)
			{
				idx = m_attackShieldReduceHours.Length - 1;
			}

			return m_attackShieldReduceHours[idx];
		}
	}
}