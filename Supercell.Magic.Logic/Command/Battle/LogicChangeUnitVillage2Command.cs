using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Battle
{
	public sealed class LogicChangeUnitVillage2Command : LogicCommand
	{
		private LogicCharacterData m_oldUnitData;
		private LogicCharacterData m_newUnitData;

		public override void Decode(ByteStream stream)
		{
			m_newUnitData = (LogicCharacterData)ByteStreamHelper.ReadDataReference(stream, DataType.CHARACTER);
			m_oldUnitData = (LogicCharacterData)ByteStreamHelper.ReadDataReference(stream, DataType.CHARACTER);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			ByteStreamHelper.WriteDataReference(encoder, m_newUnitData);
			ByteStreamHelper.WriteDataReference(encoder, m_oldUnitData);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_UNIT_VILLAGE_2;

		public override void Destruct()
		{
			base.Destruct();

			m_oldUnitData = null;
			m_newUnitData = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (level.GetVillageType() == 1)
			{
				LogicGameMode gameMode = level.GetGameMode();

				if (!gameMode.IsInAttackPreparationMode())
				{
					if (gameMode.GetState() != 5)
					{
						return -9;
					}
				}

				if (m_oldUnitData != null && m_newUnitData != null && gameMode.GetCalendar().IsProductionEnabled(m_newUnitData))
				{
					if (!m_newUnitData.IsUnlockedForBarrackLevel(playerAvatar.GetVillage2BarrackLevel()))
					{
						if (gameMode.GetState() != 7)
						{
							return -7;
						}
					}

					int oldUnitCount = playerAvatar.GetUnitCountVillage2(m_oldUnitData);
					int oldUnitsInCamp = m_oldUnitData.GetUnitsInCamp(playerAvatar.GetUnitUpgradeLevel(m_oldUnitData));

					if (oldUnitCount >= oldUnitsInCamp)
					{
						int newUnitCount = playerAvatar.GetUnitCountVillage2(m_newUnitData);
						int newUnitsInCamp = m_newUnitData.GetUnitsInCamp(playerAvatar.GetUnitUpgradeLevel(m_newUnitData));

						playerAvatar.SetUnitCountVillage2(m_oldUnitData, oldUnitCount - oldUnitsInCamp);
						playerAvatar.SetUnitCountVillage2(m_newUnitData, newUnitCount + newUnitsInCamp);

						LogicArrayList<LogicDataSlot> unitsNew = playerAvatar.GetUnitsNewVillage2();

						for (int i = 0; i < unitsNew.Size(); i++)
						{
							LogicDataSlot slot = unitsNew[i];

							if (slot.GetCount() > 0)
							{
								playerAvatar.CommodityCountChangeHelper(8, slot.GetData(), -slot.GetCount());
							}
						}

						return 0;
					}

					return -23;
				}

				return -7;
			}

			return -10;
		}

		public override void LoadFromJSON(LogicJSONObject jsonRoot)
		{
			LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("Replay LogicChangeUnitVillage2Command load failed! Base missing!");
			}

			base.LoadFromJSON(baseObject);

			LogicJSONNumber newDataNumber = jsonRoot.GetJSONNumber("n");

			if (newDataNumber != null)
			{
				m_newUnitData = (LogicCharacterData)LogicDataTables.GetDataById(newDataNumber.GetIntValue(), DataType.CHARACTER);
			}

			LogicJSONNumber oldDataNumber = jsonRoot.GetJSONNumber("o");

			if (oldDataNumber != null)
			{
				m_oldUnitData = (LogicCharacterData)LogicDataTables.GetDataById(oldDataNumber.GetIntValue(), DataType.CHARACTER);
			}
		}

		public override LogicJSONObject GetJSONForReplay()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("base", base.GetJSONForReplay());

			if (m_newUnitData != null)
			{
				jsonObject.Put("n", new LogicJSONNumber(m_newUnitData.GetGlobalID()));
			}

			if (m_oldUnitData != null)
			{
				jsonObject.Put("o", new LogicJSONNumber(m_oldUnitData.GetGlobalID()));
			}

			return jsonObject;
		}
	}
}