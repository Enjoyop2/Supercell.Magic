using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicPlaceUnplacedObjectCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private int m_upgradeLevel;

		private LogicGameObjectData m_gameObjectData;

		public LogicPlaceUnplacedObjectCommand()
		{
			// LogicPlaceUnplacedObjectCommand.
		}

		public LogicPlaceUnplacedObjectCommand(int x, int y, int upgradeLevel, LogicGameObjectData gameObjectData)
		{
			m_x = x;
			m_y = y;
			m_upgradeLevel = upgradeLevel;
			m_gameObjectData = gameObjectData;
		}

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_gameObjectData = (LogicGameObjectData)ByteStreamHelper.ReadDataReference(stream);
			m_upgradeLevel = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_gameObjectData);
			encoder.WriteInt(m_upgradeLevel);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.PLACE_UNPLACED_OBJECT;

		public override void Destruct()
		{
			base.Destruct();
			m_gameObjectData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_gameObjectData != null && level.GetUnplacedObjectCount(m_gameObjectData) > 0)
			{
				if (level.GetVillageType() == m_gameObjectData.GetVillageType())
				{
					DataType dataType = m_gameObjectData.GetDataType();

					if (dataType == DataType.BUILDING)
					{
						LogicBuildingData buildingData = (LogicBuildingData)m_gameObjectData;

						if (level.IsValidPlaceForBuilding(m_x, m_y, buildingData.GetWidth(), buildingData.GetHeight(), null))
						{
							if (!level.RemoveUnplacedObject(m_gameObjectData, m_upgradeLevel))
							{
								return -63;
							}

							LogicBuilding building = (LogicBuilding)LogicGameObjectFactory.CreateGameObject(m_gameObjectData, level, level.GetVillageType());

							building.SetPositionXY(m_x << 9, m_y << 9);
							level.GetGameObjectManager().AddGameObject(building, -1);
							building.FinishConstruction(false, true);
							building.SetUpgradeLevel(m_upgradeLevel);
						}

						return 0;
					}

					if (dataType == DataType.TRAP)
					{
						LogicTrapData trapData = (LogicTrapData)m_gameObjectData;

						if (level.IsValidPlaceForBuilding(m_x, m_y, trapData.GetWidth(), trapData.GetHeight(), null))
						{
							if (!level.RemoveUnplacedObject(m_gameObjectData, m_upgradeLevel))
							{
								return -64;
							}

							LogicTrap trap = (LogicTrap)LogicGameObjectFactory.CreateGameObject(m_gameObjectData, level, level.GetVillageType());

							trap.SetPositionXY(m_x << 9, m_y << 9);
							trap.FinishConstruction(false);
							trap.SetUpgradeLevel(m_upgradeLevel);
							level.GetGameObjectManager().AddGameObject(trap, -1);
						}

						return 0;
					}

					if (dataType == DataType.DECO)
					{
						LogicDecoData decoData = (LogicDecoData)m_gameObjectData;

						if (level.IsValidPlaceForBuilding(m_x, m_y, decoData.GetWidth(), decoData.GetHeight(), null))
						{
							if (!level.RemoveUnplacedObject(m_gameObjectData, m_upgradeLevel))
							{
								return -65;
							}

							LogicDeco deco = (LogicDeco)LogicGameObjectFactory.CreateGameObject(m_gameObjectData, level, level.GetVillageType());

							deco.SetPositionXY(m_x << 9, m_y << 9);
							level.RemoveUnplacedObject(m_gameObjectData, m_upgradeLevel);
							level.GetGameObjectManager().AddGameObject(deco, -1);
						}

						return 0;
					}

					return -3;
				}

				return -35;
			}

			return 0;
		}
	}
}