using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Unit;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicTrainUnitCommand : LogicCommand
	{
		private LogicCombatItemData m_unitData;

		private int m_unitType;
		private int m_trainCount;
		private int m_gameObjectId;
		private int m_slotId;

		public LogicTrainUnitCommand()
		{
			// LogicTrainUnitCommand.
		}

		public LogicTrainUnitCommand(int count, LogicCombatItemData combatItemData, int gameObjectId, int slotId)
		{
			m_trainCount = count;
			m_unitData = combatItemData;
			m_gameObjectId = gameObjectId;
			m_slotId = slotId;
			m_unitType = m_unitData.GetDataType() == LogicDataType.SPELL ? 1 : 0;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_unitType = stream.ReadInt();
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, m_unitType != 0 ? LogicDataType.SPELL : LogicDataType.CHARACTER);
			m_trainCount = stream.ReadInt();
			m_slotId = stream.ReadInt();

			LogicGlobals globals = LogicDataTables.GetGlobals();

			if (!globals.UseDragInTraining() && !globals.UseDragInTrainingFix() && !globals.UseDragInTrainingFix2())
			{
				m_slotId = -1;
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_unitType);
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);
			encoder.WriteInt(m_trainCount);
			encoder.WriteInt(m_slotId);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TRAIN_UNIT;

		public override void Destruct()
		{
			base.Destruct();

			m_gameObjectId = 0;
			m_unitType = 0;
			m_slotId = 0;
			m_trainCount = 0;
			m_unitData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level.GetVillageType() == 0)
			{
				if (!LogicDataTables.GetGlobals().UseNewTraining())
				{
					if (m_gameObjectId == 0)
					{
						return -1;
					}

					// TODO: Implement this.
				}
				else
				{
					return NewTrainingUnit(level);
				}
			}

			return -32;
		}

		public int NewTrainingUnit(LogicLevel level)
		{
			if (LogicDataTables.GetGlobals().UseNewTraining())
			{
				if (m_trainCount <= 100)
				{
					LogicUnitProduction unitProduction = m_unitType == 1
						? level.GetGameObjectManagerAt(0).GetSpellProduction()
						: level.GetGameObjectManagerAt(0).GetUnitProduction();

					if (m_trainCount > 0)
					{
						if (m_unitData != null)
						{
							LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

							bool firstLoopExecuted = false;
							int trainingCost = level.GetGameMode().GetCalendar().GetTrainingCost(m_unitData, playerAvatar.GetUnitUpgradeLevel(m_unitData));

							for (int i = 0; i < m_trainCount; i++)
							{
								if (!unitProduction.CanAddUnitToQueue(m_unitData, false))
								{
									if (firstLoopExecuted)
									{
										break;
									}

									return -40;
								}

								if (!playerAvatar.HasEnoughResources(m_unitData.GetTrainingResource(), trainingCost, true, this, false))
								{
									if (firstLoopExecuted)
									{
										break;
									}

									return -30;
								}

								playerAvatar.CommodityCountChangeHelper(0, m_unitData.GetTrainingResource(), -trainingCost);

								if (m_slotId == -1)
								{
									m_slotId = unitProduction.GetSlotCount();
								}

								unitProduction.AddUnitToQueue(m_unitData, m_slotId, false);
								firstLoopExecuted = true;
							}

							return 0;
						}
					}

					return -50;
				}

				Debugger.Error("LogicTraingUnitCommand - Count is too high");

				return -20;
			}

			return -99;
		}
	}
}