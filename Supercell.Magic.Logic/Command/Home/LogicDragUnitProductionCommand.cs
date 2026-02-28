using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Unit;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicDragUnitProductionCommand : LogicCommand
	{
		private bool m_spellProduction;
		private int m_slotIdx;
		private int m_dragIdx;

		public LogicDragUnitProductionCommand()
		{
			// LogicDragUnitProductionCommand.
		}

		public LogicDragUnitProductionCommand(bool spellProduction, int slotIdx, int dragIdx)
		{
			m_spellProduction = spellProduction;
			m_slotIdx = slotIdx;
			m_dragIdx = dragIdx;
		}

		public override void Decode(ByteStream stream)
		{
			m_spellProduction = stream.ReadBoolean();
			m_slotIdx = stream.ReadInt();
			m_dragIdx = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteBoolean(m_spellProduction);
			encoder.WriteInt(m_slotIdx);
			encoder.WriteInt(m_dragIdx);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DRAG_UNIT_PRODUCTION;

		public override int Execute(LogicLevel level)
		{
			if (LogicDataTables.GetGlobals().UseNewTraining())
			{
				if (!LogicDataTables.GetGlobals().UseDragInTraining() &&
					!LogicDataTables.GetGlobals().UseDragInTrainingFix() &&
					!LogicDataTables.GetGlobals().UseDragInTrainingFix2())
				{
					return -51;
				}

				LogicUnitProduction unitProduction = m_spellProduction ? level.GetGameObjectManager().GetSpellProduction() : level.GetGameObjectManager().GetUnitProduction();

				if (unitProduction.GetSlotCount() > m_slotIdx)
				{
					if (unitProduction.GetSlotCount() >= m_dragIdx)
					{
						if (m_slotIdx >= 0)
						{
							if (m_dragIdx >= 0)
							{
								return unitProduction.DragSlot(m_slotIdx, m_dragIdx) ? 0 : -5;
							}

							return -4;
						}

						return -3;
					}

					return -2;
				}

				return -1;
			}

			return -50;
		}
	}
}