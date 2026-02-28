using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicUnlockBuildingCommand : LogicCommand
	{
		private int m_gameObjectId;

		public LogicUnlockBuildingCommand()
		{
			// LogicUnlockBuildingCommand.
		}

		public LogicUnlockBuildingCommand(int gameObjectId)
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
			=> LogicCommandType.UNLOCK_BUILDING;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)gameObject;

				if (building.IsLocked())
				{
					if (building.GetUpgradeLevel() == 0 && building.CanUnlock(true))
					{
						LogicBuildingData buildingData = building.GetBuildingData();

						if (buildingData.GetConstructionTime(0, level, 0) == 0 || level.HasFreeWorkers(this, -1))
						{
							LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
							LogicResourceData buildResource = buildingData.GetBuildResource(0);
							int buildCost = buildingData.GetBuildCost(0, level);

							if (playerAvatar.HasEnoughResources(buildResource, buildCost, true, this, false))
							{
								playerAvatar.CommodityCountChangeHelper(0, buildResource, -buildCost);
								building.StartConstructing(true);
								building.GetListener().RefreshState();

								return 0;
							}
						}
					}
				}
			}

			return -1;
		}
	}
}