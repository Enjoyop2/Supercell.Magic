using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSpeedUpConstructionCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_villageType;

		public LogicSpeedUpConstructionCommand()
		{
			// LogicSpeedUpConstructionCommand.
		}

		public LogicSpeedUpConstructionCommand(int gameObjectId)
		{
			m_gameObjectId = gameObjectId;
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
			=> LogicCommandType.SPEED_UP_CONSTRUCTION;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_villageType <= 1 && level.GetGameObjectManagerAt(m_villageType) != null)
			{
				LogicGameObject gameObject = level.GetGameObjectManagerAt(m_villageType).GetGameObjectByID(m_gameObjectId);

				if (gameObject != null)
				{
					LogicGameObjectType gameObjectType = gameObject.GetGameObjectType();

					if (gameObjectType == LogicGameObjectType.BUILDING)
					{
						return ((LogicBuilding)gameObject).SpeedUpConstruction() ? 0 : -1;
					}

					if (gameObjectType == LogicGameObjectType.TRAP)
					{
						return ((LogicTrap)gameObject).SpeedUpConstruction() ? 0 : -2;
					}

					if (gameObjectType == LogicGameObjectType.VILLAGE_OBJECT)
					{
						return ((LogicVillageObject)gameObject).SpeedUpCostruction() ? 0 : -1;
					}
				}
			}

			return -3;
		}
	}
}