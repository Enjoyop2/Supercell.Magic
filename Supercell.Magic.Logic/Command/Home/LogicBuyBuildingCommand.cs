using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicBuyBuildingCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private LogicBuildingData m_buildingData;

		public LogicBuyBuildingCommand()
		{
			// LogicBuyBuildingCommand.
		}

		public LogicBuyBuildingCommand(int x, int y, LogicBuildingData buildingData)
		{
			m_x = x;
			m_y = y;
			m_buildingData = buildingData;
		}

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_buildingData = (LogicBuildingData)ByteStreamHelper.ReadDataReference(stream, DataType.BUILDING);
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_buildingData);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.BUY_BUILDING;

		public override void Destruct()
		{
			base.Destruct();
			m_x = 0;
			m_y = 0;
			m_buildingData = null;
		}

		public override int Execute(LogicLevel level)
		{
			int villageType = level.GetVillageType();

			if (m_buildingData.GetVillageType() == villageType)
			{
				if (m_buildingData.GetWallBlockCount() <= 1 && m_buildingData.GetBuildingClass().CanBuy())
				{
					if (level.IsValidPlaceForBuilding(m_x, m_y, m_buildingData.GetWidth(), m_buildingData.GetHeight(), null) &&
						!level.IsBuildingCapReached(m_buildingData, true))
					{
						if (level.GetCalendar().IsEnabled(m_buildingData))
						{
							LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
							LogicResourceData buildResourceData = m_buildingData.GetBuildResource(0);

							int buildResourceCost = m_buildingData.GetBuildCost(0, level);

							if (playerAvatar.HasEnoughResources(buildResourceData, buildResourceCost, true, this, false))
							{
								if (m_buildingData.IsWorkerBuilding() ||
									m_buildingData.GetConstructionTime(0, level, 0) <= 0 && !LogicDataTables.GetGlobals().WorkerForZeroBuilTime() ||
									level.HasFreeWorkers(this, -1))
								{
									if (buildResourceData.IsPremiumCurrency())
									{
										playerAvatar.UseDiamonds(buildResourceCost);
										playerAvatar.GetChangeListener().DiamondPurchaseMade(1, m_buildingData.GetGlobalID(), 0, buildResourceCost, villageType);
									}
									else
									{
										playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildResourceCost);
									}

									LogicBuilding building = (LogicBuilding)LogicGameObjectFactory.CreateGameObject(m_buildingData, level, villageType);

									building.SetInitialPosition(m_x << 9, m_y << 9);
									level.GetGameObjectManager().AddGameObject(building, -1);
									building.StartConstructing(false);

									if (m_buildingData.IsWall() && level.IsBuildingCapReached(m_buildingData, false))
									{
										level.GetGameListener().BuildingCapReached(m_buildingData);
									}

									int width = building.GetWidthInTiles();
									int height = building.GetHeightInTiles();

									for (int i = 0; i < width; i++)
									{
										for (int j = 0; j < height; j++)
										{
											LogicObstacle tallGrass = level.GetTileMap().GetTile(m_x + i, m_y + j).GetTallGrass();

											if (tallGrass != null)
											{
												level.GetGameObjectManager().RemoveGameObject(tallGrass);
											}
										}
									}
								}
							}

							return 0;
						}
					}
				}

				return -33;
			}

			return -32;
		}
	}
}