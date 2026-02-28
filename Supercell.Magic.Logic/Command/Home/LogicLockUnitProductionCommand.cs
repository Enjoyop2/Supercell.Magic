using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Unit;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicLockUnitProductionCommand : LogicCommand
	{
		private bool m_disabled;
		private int m_index;

		public LogicLockUnitProductionCommand()
		{
			// LogicLockUnitProductionCommand.
		}

		public LogicLockUnitProductionCommand(bool disabled, int index)
		{
			m_disabled = disabled;
			m_index = index;
		}

		public override void Decode(ByteStream stream)
		{
			m_disabled = stream.ReadBoolean();
			m_index = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteBoolean(m_disabled);
			encoder.WriteInt(m_index);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.LOCK_UNIT_PRODUCTION;

		public override int Execute(LogicLevel level)
		{
			if (level.GetVillageType() == 0)
			{
				LogicUnitProduction unitProduction = null;

				switch (m_index)
				{
					case 1:
						unitProduction = level.GetGameObjectManagerAt(0).GetUnitProduction();
						break;
					case 2:
						unitProduction = level.GetGameObjectManagerAt(0).GetSpellProduction();
						break;
				}

				if (unitProduction == null)
				{
					return -1;
				}

				unitProduction.SetLocked(m_disabled);

				return 0;
			}

			return -32;
		}
	}
}