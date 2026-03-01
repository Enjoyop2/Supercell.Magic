using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicBuyTrapCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private LogicTrapData m_trapData;

		public LogicBuyTrapCommand()
		{
			// LogicBuyTrapCommand.
		}

		public LogicBuyTrapCommand(int x, int y, LogicTrapData trapData)
		{
			m_x = x;
			m_y = y;
			m_trapData = trapData;
		}

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_trapData = (LogicTrapData)ByteStreamHelper.ReadDataReference(stream, DataType.TRAP);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_trapData);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.BUY_TRAP;

		public override void Destruct()
		{
			base.Destruct();

			m_x = 0;
			m_y = 0;
			m_trapData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_trapData != null)
			{
				//if (m_trapData.GetVillageType() == level.GetVillageType())
				if (m_trapData.IsEnabledInVillageType(level.GetVillageType()))
				{
					if (level.IsValidPlaceForBuilding(m_x, m_y, m_trapData.GetWidth(), m_trapData.GetHeight(), null))
					{
						LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
						LogicResourceData buildResourceData = m_trapData.GetBuildResource();

						int buildCost = m_trapData.GetBuildCost(0);

						if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false) && !level.IsTrapCapReached(m_trapData, true))
						{
							if (level.GetGameMode().GetCalendar().IsEnabled(m_trapData))
							{
								if (buildResourceData.IsPremiumCurrency())
								{
									playerAvatar.UseDiamonds(buildCost);
									playerAvatar.GetChangeListener().DiamondPurchaseMade(1, m_trapData.GetGlobalID(), 0, buildCost, level.GetVillageType());
								}
								else
								{
									playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);
								}

								LogicTrap trap = (LogicTrap)LogicGameObjectFactory.CreateGameObject(m_trapData, level, level.GetVillageType());

								if (m_trapData.GetBuildTime(0) == 0)
								{
									trap.FinishConstruction(false);
								}

								trap.SetInitialPosition(m_x << 9, m_y << 9);
								level.GetGameObjectManager().AddGameObject(trap, -1);

								if (level.IsTrapCapReached(m_trapData, false))
								{
									level.GetGameListener().TrapCapReached(m_trapData);
								}

								int width = trap.GetWidthInTiles();
								int height = trap.GetHeightInTiles();

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

								return 0;
							}
						}
					}

					return -1;
				}

				return -32;
			}

			return -1;
		}
	}
}