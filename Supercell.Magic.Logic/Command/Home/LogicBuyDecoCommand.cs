using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicBuyDecoCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private LogicDecoData m_decoData;

		public LogicBuyDecoCommand()
		{
			// LogicBuyDecoCommand.
		}

		public LogicBuyDecoCommand(int x, int y, LogicDecoData decoData)
		{
			m_x = x;
			m_y = y;
			m_decoData = decoData;
		}

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_decoData = (LogicDecoData)ByteStreamHelper.ReadDataReference(stream, DataType.DECO);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_decoData);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.BUY_DECO;

		public override void Destruct()
		{
			base.Destruct();

			m_x = 0;
			m_y = 0;
			m_decoData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_decoData != null)
			{
				//if (m_decoData.GetVillageType() == level.GetVillageType())
				if (m_decoData.IsEnabledInVillageType(level.GetVillageType()))
				{
					if (level.IsValidPlaceForBuilding(m_x, m_y, m_decoData.GetWidth(), m_decoData.GetHeight(), null))
					{
						LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
						LogicResourceData buildResourceData = m_decoData.GetBuildResource();

						int buildCost = m_decoData.GetBuildCost();

						if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false) && !level.IsDecoCapReached(m_decoData, true))
						{
							if (buildResourceData.IsPremiumCurrency())
							{
								playerAvatar.UseDiamonds(buildCost);
								playerAvatar.GetChangeListener().DiamondPurchaseMade(1, m_decoData.GetGlobalID(), 0, buildCost, level.GetVillageType());
							}
							else
							{
								playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);
							}

							LogicDeco deco = (LogicDeco)LogicGameObjectFactory.CreateGameObject(m_decoData, level, level.GetVillageType());

							deco.SetInitialPosition(m_x << 9, m_y << 9);
							level.GetGameObjectManager().AddGameObject(deco, -1);

							int width = deco.GetWidthInTiles();
							int height = deco.GetHeightInTiles();

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

					return -1;
				}

				return -32;
			}

			return -1;
		}
	}
}