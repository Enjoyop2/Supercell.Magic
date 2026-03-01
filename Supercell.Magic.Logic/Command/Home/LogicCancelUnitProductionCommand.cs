using System;

using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Unit;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicCancelUnitProductionCommand : LogicCommand
	{
		private LogicCombatItemData m_unitData;

		private int m_unitType;
		private int m_unitCount;
		private int m_gameObjectId;
		private int m_slotId;

		public LogicCancelUnitProductionCommand()
		{
			// LogicCancelConstructionCommand.
		}

		public LogicCancelUnitProductionCommand(int count, LogicCombatItemData combatItemData, int gameObjectId, int slotId)
		{
			m_unitCount = count;
			m_unitData = combatItemData;
			m_gameObjectId = gameObjectId;
			m_slotId = slotId;

			m_unitType = m_unitData.GetCombatItemType();
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_unitType = stream.ReadInt();
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, m_unitType != 0 ? DataType.SPELL : DataType.CHARACTER);
			m_unitCount = stream.ReadInt();
			m_slotId = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_unitType);
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);
			encoder.WriteInt(m_unitCount);
			encoder.WriteInt(m_slotId);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CANCEL_UNIT_PRODUCTION;

		public override void Destruct()
		{
			base.Destruct();

			m_gameObjectId = 0;
			m_unitType = 0;
			m_slotId = 0;
			m_unitCount = 0;
			m_unitData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (!LogicDataTables.GetGlobals().UseNewTraining())
			{
				throw new NotImplementedException(); // TODO: Implement this.
			}

			return NewTrainingUnit(level);
		}

		public int NewTrainingUnit(LogicLevel level)
		{
			if (LogicDataTables.GetGlobals().UseNewTraining())
			{
				if (m_unitData != null)
				{
					LogicUnitProduction unitProduction = m_unitData.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_SPELL
						? level.GetGameObjectManager().GetSpellProduction()
						: level.GetGameObjectManager().GetUnitProduction();

					if (!unitProduction.IsLocked())
					{
						if (m_unitCount > 0)
						{
							if (m_unitData.GetDataType() == unitProduction.GetUnitProductionType())
							{
								LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
								LogicResourceData trainingResourceData = m_unitData.GetTrainingResource();
								int trainingCost = level.GetGameMode().GetCalendar().GetTrainingCost(m_unitData, playerAvatar.GetUnitUpgradeLevel(m_unitData));
								int refundCount = LogicMath.Max(trainingCost * (m_unitData.GetDataType() != DataType.CHARACTER
																	? LogicDataTables.GetGlobals().GetSpellCancelMultiplier()
																	: LogicDataTables.GetGlobals().GetTrainCancelMultiplier()) / 100, 0);

								while (unitProduction.RemoveUnit(m_unitData, m_slotId))
								{
									playerAvatar.CommodityCountChangeHelper(0, trainingResourceData, refundCount);

									if (--m_unitCount <= 0)
									{
										break;
									}
								}

								return 0;
							}
						}

						return -1;
					}

					return -23;
				}

				return -1;
			}

			return -99;
		}
	}
}