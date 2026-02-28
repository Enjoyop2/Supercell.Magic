using Supercell.Magic.Logic.Command.Battle;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicTriggerComponent : LogicComponent
	{
		private bool m_airTrigger;
		private bool m_groundTrigger;
		private readonly bool m_healerTrigger;

		private readonly int m_triggerRadius;
		private readonly int m_minTriggerHousingLimit;

		private readonly bool m_triggeredByRadius;
		private bool m_triggered;
		private bool m_cmdTriggered;

		public LogicTriggerComponent(LogicGameObject gameObject, int triggerRadius, bool airTrigger, bool groundTrigger, bool healerTrigger, int minTriggerHousingLimit) :
			base(gameObject)
		{
			m_triggerRadius = triggerRadius;
			m_airTrigger = airTrigger;
			m_groundTrigger = groundTrigger;
			m_healerTrigger = healerTrigger;
			m_minTriggerHousingLimit = minTriggerHousingLimit;

			int tmp = ((LogicMath.Min(m_parent.GetWidthInTiles(), m_parent.GetHeightInTiles()) << 9) + 1024) >> 1;

			m_triggeredByRadius = tmp < triggerRadius;
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.TRIGGER;

		public override void Tick()
		{
			if (m_triggeredByRadius)
			{
				LogicLevel level = m_parent.GetLevel();

				if (level.IsInCombatState())
				{
					if (!m_triggered)
					{
						if (((level.GetLogicTime().GetTick() / 4) & 7) == 0)
						{
							LogicArrayList<LogicComponent> components = m_parent.GetComponentManager().GetComponents(LogicComponentType.MOVEMENT);

							for (int i = 0; i < components.Size(); i++)
							{
								LogicGameObject gameObject = components[i].GetParent();

								if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
								{
									LogicCharacter character = (LogicCharacter)gameObject;
									LogicMovementComponent movementComponent = character.GetMovementComponent();

									bool triggerDisabled = false;

									if (movementComponent != null)
									{
										LogicMovementSystem movementSystem = movementComponent.GetMovementSystem();

										if (movementSystem != null && movementSystem.IsPushed())
										{
											triggerDisabled = movementSystem.IgnorePush();
										}
									}

									if (!triggerDisabled && character.GetCharacterData().GetTriggersTraps())
									{
										ObjectClose(character);
									}
								}
							}
						}
					}
				}
			}
		}

		public void ObjectClose(LogicGameObject gameObject)
		{
			LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

			if (hitpointComponent == null || hitpointComponent.GetTeam() != 1)
			{
				if (gameObject.GetGameObjectType() == LogicGameObjectType.CHARACTER)
				{
					LogicCharacter character = (LogicCharacter)gameObject;
					LogicCharacterData data = character.GetCharacterData();

					if (data.GetHousingSpace() < m_minTriggerHousingLimit)
					{
						return;
					}
				}

				LogicCombatComponent combatComponent = gameObject.GetCombatComponent();

				if (combatComponent == null || combatComponent.GetUndergroundTime() <= 0)
				{
					if ((!gameObject.IsFlying() || m_airTrigger) && (gameObject.IsFlying() || m_groundTrigger))
					{
						if (m_healerTrigger || combatComponent == null || !combatComponent.IsHealer())
						{
							int distanceX = gameObject.GetX() - m_parent.GetMidX();
							int distanceY = gameObject.GetY() - m_parent.GetMidY();

							if (LogicMath.Abs(distanceX) <= m_triggerRadius &&
								LogicMath.Abs(distanceY) <= m_triggerRadius &&
								distanceX * distanceX + distanceY * distanceY < (uint)(m_triggerRadius * m_triggerRadius))
							{
								Trigger();
							}
						}
					}
				}
			}
		}

		private void Trigger()
		{
			if (!m_triggered)
			{
				Debugger.Print(string.Format("LogicTriggerComponent::trigger() -> {0}", m_parent.GetLevel().GetLogicTime().GetTick()));

				if (LogicDataTables.GetGlobals().UseTrapTriggerCommand())
				{
					if (!m_cmdTriggered)
					{
						if (m_parent.GetLevel().GetState() != 5)
						{
							LogicTriggerComponentTriggeredCommand triggerComponentTriggeredCommand = new LogicTriggerComponentTriggeredCommand(m_parent);
							triggerComponentTriggeredCommand.SetExecuteSubTick(m_parent.GetLevel().GetLogicTime().GetTick() + 1);
							m_parent.GetLevel().GetGameMode().GetCommandManager().AddCommand(triggerComponentTriggeredCommand);
						}

						m_cmdTriggered = true;
					}
				}
				else
				{
					m_triggered = true;
				}
			}
		}

		public void SetAirTrigger(bool value)
		{
			m_airTrigger = value;
		}

		public void SetGroundTrigger(bool value)
		{
			m_groundTrigger = value;
		}

		public bool IsTriggeredByRadius()
			=> m_triggeredByRadius;

		public bool IsTriggered()
			=> m_triggered;

		public void SetTriggered()
		{
			m_triggered = true;
		}
	}
}