using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicMoveBuildingEditModeCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_layoutId;
		private int m_x;
		private int m_y;

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_gameObjectId = stream.ReadInt();
			m_layoutId = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_layoutId);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.MOVE_BUILDING_EDIT_MODE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_layoutId != 6)
			{
				if (m_layoutId != 7)
				{
					LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

					if (gameObject != null)
					{
						LogicGameObjectType gameObjectType = gameObject.GetGameObjectType();

						if (gameObjectType == LogicGameObjectType.BUILDING ||
							gameObjectType == LogicGameObjectType.TRAP ||
							gameObjectType == LogicGameObjectType.DECO)
						{
							LogicRect playArea = level.GetPlayArea();

							if (playArea.IsInside(m_x, m_y) && playArea.IsInside(m_x + gameObject.GetWidthInTiles(), m_y + gameObject.GetHeightInTiles()) ||
								m_x == -1 ||
								m_y == -1)
							{
								if (gameObjectType == LogicGameObjectType.BUILDING)
								{
									LogicBuilding building = (LogicBuilding)gameObject;

									if (building.GetWallIndex() != 0)
									{
										return -23;
									}
								}

								gameObject.SetPositionLayoutXY(m_x, m_y, m_layoutId, true);

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

								return 0;
							}

							return -2; // EditModeOutsideMap
						}

						return -1;
					}

					return -3;
				}

				return -6;
			}

			return -5;
		}
	}
}