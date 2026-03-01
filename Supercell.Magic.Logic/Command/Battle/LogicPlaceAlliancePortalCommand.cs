using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Battle
{
	public sealed class LogicPlaceAlliancePortalCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private LogicAlliancePortalData m_data;

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_data = (LogicAlliancePortalData)ByteStreamHelper.ReadDataReference(stream, DataType.ALLIANCE_PORTAL);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_data);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.PLACE_ALLIANCE_PORTAL;

		public override void Destruct()
		{
			base.Destruct();
			m_data = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level.IsReadyForAttack())
			{
				if (level.GetVillageType() == 0)
				{
					if (LogicDataTables.GetGlobals().AllowClanCastleDeployOnObstacles())
					{
						if (!level.GetTileMap().IsValidAttackPos(m_x >> 9, m_y >> 9))
						{
							return -2;
						}
					}
					else
					{
						LogicTile tile = level.GetTileMap().GetTile(m_x >> 9, m_y >> 9);

						if (tile == null)
						{
							return -4;
						}

						if (tile.GetPassableFlag() == 0)
						{
							return -3;
						}
					}

					LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

					if (playerAvatar != null)
					{
						if (m_data != null)
						{
							LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);

							if (gameObjectManager.GetGameObjectCountByData(m_data) <= 0 && playerAvatar.GetAllianceCastleUsedCapacity() > 0)
							{
								LogicAlliancePortal alliancePortal = (LogicAlliancePortal)LogicGameObjectFactory.CreateGameObject(m_data, level, level.GetVillageType());
								LogicBunkerComponent bunkerComponent = alliancePortal.GetBunkerComponent();

								alliancePortal.SetInitialPosition(m_x, m_y);

								if (bunkerComponent != null)
								{
									bunkerComponent.SetMaxCapacity(playerAvatar.GetAllianceCastleTotalCapacity());

									if (level.GetBattleLog() != null)
									{
										if (!level.GetBattleLog().HasDeployedUnits() && level.GetTotalAttackerHeroPlaced() == 0)
										{
											level.UpdateLastUsedArmy();
										}
									}

									if (level.GetGameMode().IsInAttackPreparationMode())
									{
										level.GetGameMode().EndAttackPreparation();
									}

									bunkerComponent.RemoveAllUnits();

									LogicArrayList<LogicUnitSlot> allianceUnits = playerAvatar.GetAllianceUnits();

									for (int i = 0; i < allianceUnits.Size(); i++)
									{
										LogicUnitSlot slot = allianceUnits[i];
										LogicCombatItemData data = (LogicCombatItemData)slot.GetData();

										if (data != null)
										{
											int count = slot.GetCount();

											if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
											{
												for (int j = 0; j < count; j++)
												{
													if (bunkerComponent.GetUnusedCapacity() >= data.GetHousingSpace())
													{
														bunkerComponent.AddUnitImpl(data, slot.GetLevel());
													}
												}
											}
										}
										else
										{
											Debugger.Error("LogicPlaceAlliancePortalCommand::execute - NULL alliance character");
										}
									}
								}

								gameObjectManager.AddGameObject(alliancePortal, -1);

								return 0;
							}
						}
					}

					return -5;
				}
			}

			return -1;
		}

		public override void LoadFromJSON(LogicJSONObject jsonRoot)
		{
			LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("Replay LogicPlaceAlliancePortalCommand load failed! Base missing!");
			}

			base.LoadFromJSON(baseObject);

			LogicJSONNumber dataNumber = jsonRoot.GetJSONNumber("d");

			if (dataNumber != null)
			{
				m_data = (LogicAlliancePortalData)LogicDataTables.GetDataById(dataNumber.GetIntValue(), DataType.ALLIANCE_PORTAL);
			}

			if (m_data == null)
			{
				Debugger.Error("Replay LogicPlaceAlliancePortalCommand load failed! Data is NULL!");
			}

			m_x = jsonRoot.GetJSONNumber("x").GetIntValue();
			m_y = jsonRoot.GetJSONNumber("y").GetIntValue();
		}

		public override LogicJSONObject GetJSONForReplay()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("base", base.GetJSONForReplay());

			if (m_data != null)
			{
				jsonObject.Put("d", new LogicJSONNumber(m_data.GetGlobalID()));
			}

			jsonObject.Put("x", new LogicJSONNumber(m_x));
			jsonObject.Put("y", new LogicJSONNumber(m_y));

			return jsonObject;
		}
	}
}