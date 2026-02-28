using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSwapBuildingEditModeCommand : LogicCommand
	{
		private int m_layoutId;
		private int m_gameObject1;
		private int m_gameObject2;

		public LogicSwapBuildingEditModeCommand()
		{
			// LogicSwapBuildingEditModeCommand.
		}

		public LogicSwapBuildingEditModeCommand(int layoutId, int gameObject1, int gameObject2)
		{
			m_layoutId = layoutId;
			m_gameObject1 = gameObject1;
			m_gameObject2 = gameObject2;
		}

		public override void Decode(ByteStream stream)
		{
			m_layoutId = stream.ReadInt();
			m_gameObject1 = stream.ReadInt();
			m_gameObject2 = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_layoutId);
			encoder.WriteInt(m_gameObject1);
			encoder.WriteInt(m_gameObject2);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SWAP_BUILDING_EDIT_MODE;

		public override int Execute(LogicLevel level)
		{
			if (LogicDataTables.GetGlobals().UseSwapBuildings())
			{
				if (m_gameObject1 != m_gameObject2)
				{
					LogicGameObject gameObject1 = level.GetGameObjectManager().GetGameObjectByID(m_gameObject1);
					LogicGameObject gameObject2 = level.GetGameObjectManager().GetGameObjectByID(m_gameObject2);

					if (gameObject1 != null)
					{
						if (gameObject2 != null)
						{
							LogicGameObjectType gameObjectType1 = gameObject1.GetGameObjectType();

							if (gameObjectType1 == LogicGameObjectType.BUILDING || gameObjectType1 == LogicGameObjectType.TRAP ||
								gameObjectType1 == LogicGameObjectType.DECO)
							{
								LogicGameObjectType gameObjectType2 = gameObject2.GetGameObjectType();

								if (gameObjectType2 == LogicGameObjectType.BUILDING || gameObjectType2 == LogicGameObjectType.TRAP ||
									gameObjectType2 == LogicGameObjectType.DECO)
								{
									int width1 = gameObject1.GetWidthInTiles();
									int width2 = gameObject2.GetWidthInTiles();
									int height1 = gameObject1.GetHeightInTiles();
									int height2 = gameObject2.GetHeightInTiles();

									if (width1 == width2 && height1 == height2)
									{
										if (gameObject1.GetGameObjectType() == LogicGameObjectType.BUILDING)
										{
											LogicBuilding building = (LogicBuilding)gameObject1;

											if (building.IsLocked())
											{
												return -6;
											}

											if (building.IsWall())
											{
												return -7;
											}
										}

										if (gameObject2.GetGameObjectType() == LogicGameObjectType.BUILDING)
										{
											LogicBuilding building = (LogicBuilding)gameObject2;

											if (building.IsLocked())
											{
												return -8;
											}

											if (building.IsWall())
											{
												return -9;
											}
										}

										int x1 = gameObject1.GetPositionLayout(m_layoutId, true).m_x;
										int y1 = gameObject1.GetPositionLayout(m_layoutId, true).m_y;
										int x2 = gameObject2.GetPositionLayout(m_layoutId, true).m_x;
										int y2 = gameObject2.GetPositionLayout(m_layoutId, true).m_y;

										gameObject1.SetPositionLayoutXY(x2, y2, m_layoutId, true);
										gameObject2.SetPositionLayoutXY(x1, y1, m_layoutId, true);

										return 0;
									}

									return -5;
								}

								return -4;
							}

							return -3;
						}

						return -2;
					}

					return -1;
				}

				return -98;
			}

			return -99;
		}
	}
}