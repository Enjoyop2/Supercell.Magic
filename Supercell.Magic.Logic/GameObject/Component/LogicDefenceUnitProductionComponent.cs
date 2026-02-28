using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicDefenceUnitProductionComponent : LogicComponent
	{
		private LogicTimer m_spawnCooldownTimer;
		private LogicArrayList<LogicCharacter> m_defenceTroops;
		private LogicCharacterData[] m_defenceTroopData;

		private int m_defenceTroopUpgradeLevel;
		private int m_defenceTroopCooldownSecs;
		private int m_defenceTroopCount;
		private int m_maxDefenceTroopCount;

		public LogicDefenceUnitProductionComponent(LogicGameObject gameObject) : base(gameObject)
		{
			m_defenceTroops = new LogicArrayList<LogicCharacter>();
			m_defenceTroopData = new LogicCharacterData[2];
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_defenceTroops != null)
			{
				m_defenceTroops.Destruct();
				m_defenceTroops = null;
			}

			if (m_spawnCooldownTimer != null)
			{
				m_spawnCooldownTimer.Destruct();
				m_spawnCooldownTimer = null;
			}

			m_defenceTroopData = null;
		}

		public override void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			for (int i = 0, size = m_defenceTroops.Size(); i < size; i++)
			{
				if (m_defenceTroops[i] == gameObject)
				{
					m_defenceTroops.Remove(i);
					StartSpawnCooldownTimer();

					break;
				}
			}
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.DEFENCE_UNIT_PRODUCTION;

		public override void Tick()
		{
			if (m_parent.IsAlive())
			{
				if (LogicDataTables.GetGlobals().GuardPostNotFunctionalUnderUpgrade())
				{
					if (m_parent.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						LogicBuilding building = (LogicBuilding)m_parent;

						if (building.IsConstructing())
						{
							return;
						}
					}
				}

				if (m_maxDefenceTroopCount > m_defenceTroopCount)
				{
					if (m_spawnCooldownTimer == null || m_spawnCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) <= 0)
					{
						SpawnCharacter(m_parent.GetX(), m_parent.GetY() + (m_parent.GetHeightInTiles() << 8));

						if (m_maxDefenceTroopCount > m_defenceTroops.Size())
						{
							StartSpawnCooldownTimer();
						}
					}
				}
			}
		}

		public void StartSpawnCooldownTimer()
		{
			if (m_spawnCooldownTimer == null)
			{
				m_spawnCooldownTimer = new LogicTimer();
			}

			if (m_spawnCooldownTimer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime()) <= 0)
			{
				m_spawnCooldownTimer.StartTimer(m_defenceTroopCooldownSecs, m_parent.GetLevel().GetLogicTime(), false, -1);
			}
		}

		private void SpawnCharacter(int x, int y)
		{
			int idx = m_defenceTroopCount % 2;

			if (m_defenceTroopData[idx] == null)
			{
				idx = 0;
			}

			LogicBuilding building = (LogicBuilding)m_parent;
			LogicBuildingData buildingData = building.GetBuildingData();

			if (buildingData.IsEnabledInVillageType(m_parent.GetLevel().GetVillageType()) &&
				m_parent.GetLevel().GetState() != 1 &&
				m_parent.GetLevel().GetState() != 4)
			{
				LogicCharacterData data = m_defenceTroopData[idx];
				LogicCharacter character = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(data, m_parent.GetLevel(), m_parent.GetVillageType());

				character.SetInitialPosition(x, y);
				character.SetUpgradeLevel(m_defenceTroopUpgradeLevel - 1);
				character.GetHitpointComponent()?.SetTeam(1);

				if (LogicDataTables.GetGlobals().EnableDefendingAllianceTroopJump())
				{
					character.GetMovementComponent().EnableJump(3600000);
				}

				m_parent.GetGameObjectManager().AddGameObject(character, -1);

				character.GetCombatComponent().SetSearchRadius(LogicDataTables.GetGlobals().GetClanCastleRadius() >> 9);
				character.GetMovementComponent().GetMovementSystem().CreatePatrolArea(m_parent, m_parent.GetLevel(), true, m_defenceTroopCount);

				LogicDefenceUnitProductionComponent defenceUnitProductionComponent = building.GetDefenceUnitProduction();

				if (defenceUnitProductionComponent != null)
				{
					defenceUnitProductionComponent.m_defenceTroops.Add(character);
				}

				++m_defenceTroopCount;
			}
		}

		public void SetDefenceTroops(LogicCharacterData defenceTroopCharacter1, LogicCharacterData defenceTroopCharacter2, int defenceTroopCount, int defenceTroopLevel,
									 int defenseTroopCooldownSecs)
		{
			m_defenceTroopData[0] = defenceTroopCharacter1;
			m_defenceTroopData[1] = defenceTroopCharacter2;
			m_maxDefenceTroopCount = defenceTroopCount;
			m_defenceTroopUpgradeLevel = defenceTroopLevel;
			m_defenceTroopCooldownSecs = defenseTroopCooldownSecs;
		}
	}
}