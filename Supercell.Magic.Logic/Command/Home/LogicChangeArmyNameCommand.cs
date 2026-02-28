using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicChangeArmyNameCommand : LogicCommand
	{
		private int m_armyId;
		private string m_name;

		public LogicChangeArmyNameCommand()
		{
			// LogicChangeArmyNameCommand.
		}

		public LogicChangeArmyNameCommand(int id, string name)
		{
			m_armyId = id;
			m_name = name;
		}

		public override void Decode(ByteStream stream)
		{
			m_armyId = stream.ReadInt();
			m_name = stream.ReadString(900000);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_armyId);
			encoder.WriteString(m_name);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_ARMY_NAME;

		public override void Destruct()
		{
			base.Destruct();
			m_name = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_armyId > -1)
			{
				if (m_armyId <= 3)
				{
					if (m_name.Length <= 16)
					{
						level.SetArmyName(m_armyId, m_name);
						return 0;
					}

					return -4;
				}

				return -2;
			}

			return -1;
		}
	}
}