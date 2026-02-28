using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicRotateBuildingCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_layout;
		private int m_baseLayout;

		private bool m_draftLayout;
		private bool m_baseDraftLayout;
		private bool m_updateListener;

		public LogicRotateBuildingCommand()
		{
			// LogicRotateBuildingCommand.
		}

		public LogicRotateBuildingCommand(int gameObjectId, int layout, bool draftLayout, bool updateListener, int baseLayout, bool baseDraftLayout)
		{
			m_gameObjectId = gameObjectId;
			m_layout = layout;
			m_draftLayout = draftLayout;
			m_updateListener = updateListener;
			m_baseLayout = baseLayout;
			m_baseDraftLayout = baseDraftLayout;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_layout = stream.ReadInt();
			m_draftLayout = stream.ReadBoolean();
			m_updateListener = stream.ReadBoolean();
			m_baseLayout = stream.ReadInt();
			m_baseDraftLayout = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_layout);
			encoder.WriteBoolean(m_draftLayout);
			encoder.WriteBoolean(m_updateListener);
			encoder.WriteInt(m_baseLayout);
			encoder.WriteBoolean(m_baseDraftLayout);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ROTATE_BUILDING;

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
					LogicCombatComponent combatComponent = building.GetCombatComponent(false);

					if (combatComponent != null && combatComponent.GetAttackerItemData().GetTargetingConeAngle() != 0)
					{
						if (m_baseLayout == -1)
						{
							combatComponent.ToggleAimAngle(buildingData.GetAimRotateStep(), m_layout, m_draftLayout);
						}
						else
						{
							int draftAngle = combatComponent.GetAimAngle(m_baseLayout, m_baseDraftLayout);
							int currentAngle = combatComponent.GetAimAngle(m_layout, m_draftLayout);

							combatComponent.ToggleAimAngle(draftAngle - currentAngle, m_layout, m_draftLayout);
						}

						return 0;
					}
				}
				else if (gameObject.GetGameObjectType() == LogicGameObjectType.TRAP)
				{
					LogicTrap trap = (LogicTrap)gameObject;

					if (trap.GetTrapData().GetDirectionCount() > 0)
					{
						if (m_baseLayout == -1)
						{
							trap.ToggleDirection(m_layout, m_draftLayout);
						}
						else
						{
							trap.SetDirection(m_layout, m_draftLayout, trap.GetDirection(m_baseLayout, m_baseDraftLayout));
						}

						return 0;
					}

					return -21;
				}
			}

			return -1;
		}
	}
}