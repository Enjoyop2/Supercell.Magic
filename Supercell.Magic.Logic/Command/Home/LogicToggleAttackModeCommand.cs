using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicToggleAttackModeCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_layout;

		private bool m_draftMode;
		private bool m_updateListener;

		public LogicToggleAttackModeCommand()
		{
			// LogicToggleAttackModeCommand.
		}

		public LogicToggleAttackModeCommand(int gameObjectId, int layout, bool draftMode, bool updateListener)
		{
			m_gameObjectId = gameObjectId;
			m_layout = layout;
			m_draftMode = draftMode;
			m_updateListener = updateListener;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_layout = stream.ReadInt();
			m_draftMode = stream.ReadBoolean();
			m_updateListener = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_layout);
			encoder.WriteBoolean(m_draftMode);
			encoder.WriteBoolean(m_updateListener);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TOGGLE_ATTACK_MODE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null)
			{
				if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
				{
					LogicBuilding building = (LogicBuilding)gameObject;
					LogicBuildingData buildingData = building.GetBuildingData();

					if (buildingData.GetGearUpBuildingData() == null || building.GetGearLevel() != 0)
					{
						if (building.GetAttackerItemData().HasAlternativeAttackMode())
						{
							LogicCombatComponent combatComponent = building.GetCombatComponent(false);

							if (combatComponent != null)
							{
								combatComponent.ToggleAttackMode(m_layout, m_draftMode);

								if (m_updateListener)
								{
								}

								return 0;
							}
						}

						return -1;
					}

					return -95;
				}

				if (gameObject.GetGameObjectType() == LogicGameObjectType.TRAP)
				{
					LogicTrap trap = (LogicTrap)gameObject;

					if (trap.HasAirMode())
					{
						trap.ToggleAirMode(m_layout, m_draftMode);

						if (m_updateListener)
						{
						}

						return 0;
					}
				}
			}

			return -1;
		}
	}
}