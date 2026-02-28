using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicMoveMultipleBuildingsEditModeCommand : LogicCommand
	{
		private readonly LogicArrayList<int> m_gameObjectIds;
		private readonly LogicArrayList<int> m_xPositions;
		private readonly LogicArrayList<int> m_yPositions;

		private int m_layoutId;

		public LogicMoveMultipleBuildingsEditModeCommand()
		{
			m_gameObjectIds = new LogicArrayList<int>();
			m_xPositions = new LogicArrayList<int>();
			m_yPositions = new LogicArrayList<int>();
		}

		public override void Decode(ByteStream stream)
		{
			m_layoutId = stream.ReadInt();

			for (int i = LogicMath.Min(500, stream.ReadInt()); i > 0; i--)
			{
				m_xPositions.Add(stream.ReadInt());
				m_yPositions.Add(stream.ReadInt());
				m_gameObjectIds.Add(stream.ReadInt());
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			int count = m_gameObjectIds.Size();

			encoder.WriteInt(m_layoutId);
			encoder.WriteInt(count);

			for (int i = LogicMath.Min(500, count); i > 0; i--)
			{
				encoder.WriteInt(m_xPositions[i]);
				encoder.WriteInt(m_yPositions[i]);
				encoder.WriteInt(m_gameObjectIds[i]);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.MOVE_MULTIPLE_BUILDINGS_EDIT_MODE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			int count = m_gameObjectIds.Size();

			if (count > 0)
			{
				bool validGameObjects = true;

				if (m_xPositions.Size() == count && m_xPositions.Size() == count && count <= 500)
				{
					LogicGameObject[] gameObjects = new LogicGameObject[count];

					for (int i = 0; i < count; i++)
					{
						LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectIds[i]);

						if (gameObject != null)
						{
							LogicGameObjectType gameObjectType = gameObject.GetGameObjectType();

							if (gameObjectType != LogicGameObjectType.BUILDING &&
								gameObjectType != LogicGameObjectType.TRAP &&
								gameObjectType != LogicGameObjectType.DECO)
							{
								validGameObjects = false;
							}

							gameObjects[i] = gameObject;
						}
						else
						{
							validGameObjects = false;
						}
					}

					if (validGameObjects)
					{
						for (int i = 0; i < count; i++)
						{
							LogicGameObject gameObject = gameObjects[i];

							if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING && validGameObjects)
							{
								LogicBuilding baseWallBlock = (LogicBuilding)gameObject;

								if (baseWallBlock.GetWallIndex() != 0)
								{
									int x = m_xPositions[i];
									int y = m_yPositions[i];
									int minX = 0;
									int minY = 0;
									int maxX = 0;
									int maxY = 0;
									int minWallBlockX = 0;
									int minWallBlockY = 0;
									int maxWallBlockX = 0;
									int maxWallBlockY = 0;
									int wallBlockCnt = 0;

									bool success = true;

									for (int j = 0; j < count; j++)
									{
										LogicGameObject obj = gameObjects[j];

										if (obj.GetGameObjectType() == LogicGameObjectType.BUILDING)
										{
											LogicBuilding wallBlock = (LogicBuilding)obj;

											if (wallBlock.GetWallIndex() == baseWallBlock.GetWallIndex())
											{
												int tmp1 = x - m_xPositions[j];
												int tmp2 = y - m_yPositions[j];

												if ((x & m_xPositions[j]) != -1)
												{
													success = false;
												}

												minX = LogicMath.Min(minX, tmp1);
												minY = LogicMath.Min(minY, tmp2);
												maxX = LogicMath.Max(maxX, tmp1);
												maxY = LogicMath.Max(maxY, tmp2);

												int wallBlockX = wallBlock.GetBuildingData().GetWallBlockX(wallBlockCnt);
												int wallBlockY = wallBlock.GetBuildingData().GetWallBlockY(wallBlockCnt);

												minWallBlockX = LogicMath.Min(minWallBlockX, wallBlockX);
												minWallBlockY = LogicMath.Min(minWallBlockY, wallBlockY);
												maxWallBlockX = LogicMath.Max(maxWallBlockX, wallBlockX);
												maxWallBlockY = LogicMath.Max(maxWallBlockY, wallBlockY);

												++wallBlockCnt;
											}
										}
									}

									if (baseWallBlock.GetBuildingData().GetWallBlockCount() == wallBlockCnt)
									{
										int wallBlockSizeX = maxWallBlockX - minWallBlockX;
										int wallBlockSizeY = maxWallBlockY - minWallBlockY;
										int lengthX = maxX - minX;
										int lengthY = maxY - minY;

										if (wallBlockSizeX != lengthX || wallBlockSizeY != lengthY)
										{
											if (!success && wallBlockSizeX != lengthX != (wallBlockSizeY != lengthY))
											{
												validGameObjects = false;
											}
										}
									}
								}
							}
						}
					}

					if (validGameObjects)
					{
						for (int i = 0; i < count; i++)
						{
							int x = m_xPositions[i];
							int y = m_yPositions[i];

							LogicGameObject gameObject = gameObjects[i];

							int width = gameObject.GetWidthInTiles();
							int height = gameObject.GetHeightInTiles();

							int tmp1 = x + width;
							int tmp2 = y + height;

							for (int j = 0; j < count; j++)
							{
								LogicGameObject gameObject2 = gameObjects[j];

								if (gameObject2 != gameObject)
								{
									int x2 = m_xPositions[j];
									int y2 = m_yPositions[j];

									if (x2 != -1 && y2 != -1)
									{
										int width2 = gameObject2.GetWidthInTiles();
										int height2 = gameObject2.GetHeightInTiles();
										int tmp3 = x2 + width2;
										int tmp4 = y2 + height2;

										if (tmp1 > x2 && tmp2 > y2 && x < tmp3 && y < tmp4)
										{
											return 0;
										}
									}
								}
							}
						}

						bool moved = false;

						for (int i = 0; i < count; i++)
						{
							int x = m_xPositions[i];
							int y = m_yPositions[i];

							LogicGameObject gameObject = gameObjects[i];
							LogicVector2 position = gameObject.GetPositionLayout(m_layoutId, true);

							if (position.m_x != -1 && position.m_y != -1)
							{
								if (x != position.m_x && y != position.m_y)
								{
									moved = true;
								}
							}

							gameObject.SetPositionLayoutXY(x, y, m_layoutId, true);

							LogicGlobals globals = LogicDataTables.GetGlobals();

							if (!globals.NoCooldownFromMoveEditModeActive())
							{
								if (level.GetActiveLayout(level.GetVillageType()) == m_layoutId)
								{
									LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

									if (homeOwnerAvatar.GetExpLevel() >= globals.GetChallengeBaseCooldownEnabledTownHall())
									{
										level.SetLayoutCooldownSecs(m_layoutId, globals.GetChallengeBaseSaveCooldown());
									}
								}
							}
						}

						if (moved)
						{
							LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

							if (homeOwnerAvatar.GetExpLevel() >= LogicDataTables.GetGlobals().GetChallengeBaseCooldownEnabledTownHall())
							{
								level.SetLayoutCooldownSecs(m_layoutId, LogicDataTables.GetGlobals().GetChallengeBaseSaveCooldown());
							}
						}

						return 0;
					}
				}
			}

			return -1;
		}
	}
}