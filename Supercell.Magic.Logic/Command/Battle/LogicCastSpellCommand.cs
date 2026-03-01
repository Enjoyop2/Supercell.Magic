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
	public sealed class LogicCastSpellCommand : LogicCommand
	{
		private int m_x;
		private int m_y;
		private int m_upgLevel;
		private bool m_allianceSpell;

		private LogicSpellData m_data;

		public override void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
			m_data = (LogicSpellData)ByteStreamHelper.ReadDataReference(stream, DataType.SPELL);
			m_allianceSpell = stream.ReadBoolean();
			m_upgLevel = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_x);
			encoder.WriteInt(m_y);
			ByteStreamHelper.WriteDataReference(encoder, m_data);
			encoder.WriteBoolean(m_allianceSpell);
			encoder.WriteInt(m_upgLevel);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CAST_SPELL;

		public override void Destruct()
		{
			base.Destruct();
			m_data = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level.IsReadyForAttack())
			{
				int tileX = m_x >> 9;
				int tileY = m_y >> 9;

				if (level.GetTileMap().GetTile(tileX, tileY) != null)
				{
					if (level.GetVillageType() == 0)
					{
						LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

						if (playerAvatar != null)
						{
							int unitCount = m_allianceSpell ? playerAvatar.GetAllianceUnitCount(m_data, m_upgLevel) : playerAvatar.GetUnitCount(m_data);

							if (unitCount > 0)
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

								LogicCastSpellCommand.CastSpell(playerAvatar, m_data, m_allianceSpell, m_upgLevel, level, m_x, m_y);

								return 0;
							}
						}

						return -3;
					}

					Debugger.Error("not available for village");

					return -23;
				}

				return -3;
			}

			return -1;
		}

		public static LogicSpell CastSpell(LogicAvatar avatar, LogicSpellData spellData, bool allianceSpell, int upgLevel, LogicLevel level, int x, int y)
		{
			if (allianceSpell)
			{
				avatar.RemoveAllianceUnit(spellData, upgLevel);
			}
			else
			{
				avatar.CommodityCountChangeHelper(0, spellData, -1);
			}

			if (!allianceSpell)
			{
				upgLevel = avatar.GetUnitUpgradeLevel(spellData);
			}

			LogicSpell spell = (LogicSpell)LogicGameObjectFactory.CreateGameObject(spellData, level, level.GetVillageType());

			spell.SetUpgradeLevel(upgLevel);
			spell.SetInitialPosition(x, y);
			level.GetGameObjectManager().AddGameObject(spell, -1);
			level.GetGameListener().AttackerPlaced(spellData);

			LogicBattleLog battleLog = level.GetBattleLog();

			if (battleLog != null)
			{
				battleLog.IncrementCastedSpells(spellData, 1);
				battleLog.SetCombatItemLevel(spellData, upgLevel);
			}

			return spell;
		}

		public override void LoadFromJSON(LogicJSONObject jsonRoot)
		{
			LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("Replay LogicCastSpellCommand load failed! Base missing!");
			}

			base.LoadFromJSON(baseObject);

			LogicJSONNumber dataNumber = jsonRoot.GetJSONNumber("d");

			if (dataNumber != null)
			{
				m_data = (LogicSpellData)LogicDataTables.GetDataById(dataNumber.GetIntValue(), DataType.SPELL);
			}

			if (m_data == null)
			{
				Debugger.Error("Replay LogicCastSpellCommand load failed! Data is NULL!");
			}

			m_x = jsonRoot.GetJSONNumber("x").GetIntValue();
			m_y = jsonRoot.GetJSONNumber("y").GetIntValue();

			LogicJSONNumber dataLevelNumber = jsonRoot.GetJSONNumber("dl");

			if (dataLevelNumber != null)
			{
				m_allianceSpell = true;
				m_upgLevel = dataLevelNumber.GetIntValue();
			}
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

			if (m_allianceSpell)
			{
				jsonObject.Put("dl", new LogicJSONNumber(m_upgLevel));
			}

			return jsonObject;
		}
	}
}