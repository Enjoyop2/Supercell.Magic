using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicMoveBuildingCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private int m_gameObjectId;

		public LogicMoveBuildingCommand()
		{
			// LogicMoveBuildingCommand.
		}

		public LogicMoveBuildingCommand(int gameObjectId, int x, int y)
		{
			m_x = x;
			m_y = y;
			m_gameObjectId = gameObjectId;
		}

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_gameObjectId = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			encoder.WriteInt(m_gameObjectId);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.MOVE_BUILDING;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null)
			{
				LogicGameObjectType gameObjectType = gameObject.GetGameObjectType();

				if (gameObjectType == LogicGameObjectType.BUILDING || gameObjectType == LogicGameObjectType.TRAP ||
					gameObjectType == LogicGameObjectType.DECO)
				{
					if (gameObjectType != LogicGameObjectType.BUILDING || ((LogicBuildingData)gameObject.GetData()).GetVillageType() == level.GetVillageType())
					{
						if (gameObjectType == LogicGameObjectType.BUILDING)
						{
							if (((LogicBuilding)gameObject).GetWallIndex() != 0)
							{
								return -21;
							}
						}

						int x = gameObject.GetTileX();
						int y = gameObject.GetTileY();
						int width = gameObject.GetWidthInTiles();
						int height = gameObject.GetHeightInTiles();

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

						if (level.IsValidPlaceForBuilding(m_x, m_y, width, height, gameObject))
						{
							gameObject.SetPositionXY(m_x << 9, m_y << 9);

							if (m_x != x || m_y != y)
							{
								LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

								if (homeOwnerAvatar != null)
								{
									if (homeOwnerAvatar.GetTownHallLevel() >= LogicDataTables.GetGlobals().GetChallengeBaseCooldownEnabledTownHall() &&
										gameObject.GetGameObjectType() != LogicGameObjectType.DECO)
									{
										level.SetLayoutCooldownSecs(level.GetActiveLayout(level.GetVillageType()), LogicDataTables.GetGlobals().GetChallengeBaseSaveCooldown());
									}
								}
							}

							return 0;
						}

						return -3;
					}

					return -32;
				}

				return -1;
			}

			return -2;
		}
	}
}