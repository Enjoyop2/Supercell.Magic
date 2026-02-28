using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Battle;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Command.Battle
{
	public sealed class LogicPlaceHeroCommand : LogicCommand
	{
		private int m_x;
		private int m_y;

		private LogicHeroData m_data;

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_data = (LogicHeroData)ByteStreamHelper.ReadDataReference(stream, LogicDataType.HERO);

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
			=> LogicCommandType.PLACE_HERO;

		public override void Destruct()
		{
			base.Destruct();
			m_data = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level.IsReadyForAttack())
			{
				if (m_data != null && !level.IsAttackerHeroPlaced(m_data))
				{
					if (level.GetVillageType() == m_data.GetVillageType())
					{
						int tileX = m_x >> 9;
						int tileY = m_y >> 9;

						if (level.GetTileMap().GetTile(tileX, tileY) != null)
						{
							if (level.GetTileMap().IsPassablePathFinder(m_x >> 8, m_y >> 8))
							{
								if (level.GetTileMap().IsValidAttackPos(tileX, tileY))
								{
									LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

									if (playerAvatar != null)
									{
										if (playerAvatar.IsHeroAvailableForAttack(m_data))
										{
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

											int health = playerAvatar.GetHeroHealth(m_data);
											int upgLevel = playerAvatar.GetUnitUpgradeLevel(m_data);

											level.SetAttackerHeroPlaced(m_data,
																		LogicPlaceHeroCommand.PlaceHero(m_data, level, m_x, m_y,
																										m_data.GetHeroHitpoints(health, upgLevel), upgLevel));

											return 0;
										}
									}

									return -5;
								}

								return -4;
							}

							return -2;
						}

						return -3;
					}

					return -23;
				}

				return -5;
			}

			return -1;
		}

		public static LogicCharacter PlaceHero(LogicHeroData data, LogicLevel level, int x, int y, int hitpoints, int upgLevel)
		{
			LogicCharacter character = (LogicCharacter)LogicGameObjectFactory.CreateGameObject(data, level, level.GetVillageType());

			character.SetUpgradeLevel(upgLevel);
			character.GetHitpointComponent().SetHitpoints(hitpoints);
			character.SetInitialPosition(x, y);

			if (data.IsJumper())
			{
				character.GetMovementComponent().EnableJump(3600000);
				character.GetCombatComponent().RefreshTarget(true);
			}

			level.GetGameObjectManager().AddGameObject(character, -1);
			level.GetGameListener().AttackerPlaced(data);

			LogicBattleLog battleLog = level.GetBattleLog();

			if (battleLog != null)
			{
				battleLog.IncrementDeployedAttackerUnits(data, 1);
				battleLog.SetCombatItemLevel(data, upgLevel);
			}

			return character;
		}

		public override void LoadFromJSON(LogicJSONObject jsonRoot)
		{
			LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("Replay LogicPlaceHeroCommand load failed! Base missing!");
			}

			base.LoadFromJSON(baseObject);

			LogicJSONNumber dataNumber = jsonRoot.GetJSONNumber("d");

			if (dataNumber != null)
			{
				m_data = (LogicHeroData)LogicDataTables.GetDataById(dataNumber.GetIntValue(), LogicDataType.HERO);
			}

			if (m_data == null)
			{
				Debugger.Error("Replay LogicPlaceHeroCommand load failed! Hero is NULL!");
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