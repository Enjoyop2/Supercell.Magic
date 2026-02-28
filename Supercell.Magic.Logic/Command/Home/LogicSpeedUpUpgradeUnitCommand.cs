using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSpeedUpUpgradeUnitCommand : LogicCommand
	{
		private int m_gameObjectId;

		public LogicSpeedUpUpgradeUnitCommand()
		{
			// LogicSpeedUpUpgradeUnitCommand.
		}

		public LogicSpeedUpUpgradeUnitCommand(int gameObjectId)
		{
			m_gameObjectId = gameObjectId;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SPEED_UP_UPGRADE_UNIT;

		public override void Destruct()
		{
			base.Destruct();
			m_gameObjectId = 0;
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)gameObject;
				LogicUnitUpgradeComponent unitUpgradeComponent = building.GetUnitUpgradeComponent();

				if (unitUpgradeComponent.GetCurrentlyUpgradedUnit() != null)
				{
					return unitUpgradeComponent.SpeedUp() ? 0 : -2;
				}
			}

			return -1;
		}
	}
}