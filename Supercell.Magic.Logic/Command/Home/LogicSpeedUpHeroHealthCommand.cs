using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSpeedUpHeroHealthCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_villageType;

		public LogicSpeedUpHeroHealthCommand()
		{
			// LogicSpeedUpHeroUpgradeCommand.
		}

		public LogicSpeedUpHeroHealthCommand(int gameObjectId, int villageType)
		{
			m_gameObjectId = gameObjectId;
			m_villageType = villageType;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_villageType = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_villageType);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SPEED_UP_HERO_HEALTH;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = m_villageType <= 1
				? level.GetGameObjectManagerAt(m_villageType).GetGameObjectByID(m_gameObjectId)
				: level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)gameObject;
				LogicHeroBaseComponent heroBaseComponent = building.GetHeroBaseComponent();

				if (heroBaseComponent != null)
				{
					return heroBaseComponent.SpeedUpHealth() ? 0 : -2;
				}
			}

			return -1;
		}
	}
}