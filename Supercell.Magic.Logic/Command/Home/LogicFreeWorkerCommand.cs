using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicFreeWorkerCommand : LogicCommand
	{
		private int m_villageType;
		private LogicCommand m_command;

		public LogicFreeWorkerCommand()
		{
			// LogicBuyResourcesCommand.
		}

		public LogicFreeWorkerCommand(LogicCommand resourceCommand, int villageType)
		{
			m_command = resourceCommand;
			m_villageType = villageType;
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_villageType = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_command = LogicCommandManager.DecodeCommand(stream);
			}
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteInt(m_villageType);

			if (m_command != null)
			{
				encoder.WriteBoolean(true);
				LogicCommandManager.EncodeCommand(encoder, m_command);
			}
			else
			{
				encoder.WriteBoolean(false);
			}
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.FREE_WORKER;

		public override void Destruct()
		{
			base.Destruct();

			if (m_command != null)
			{
				m_command.Destruct();
				m_command = null;
			}
		}

		public override int Execute(LogicLevel level)
		{
			int villageType = m_villageType != -1 ? m_villageType : level.GetVillageType();
			int freeWorkers = level.GetWorkerManagerAt(villageType).GetFreeWorkers();

			if (freeWorkers == 0)
			{
				if (level.GetWorkerManagerAt(villageType).FinishTaskOfOneWorker())
				{
					if (m_command != null)
					{
						int commandType = (int)m_command.GetCommandType();

						if (commandType < 1000)
						{
							if (commandType >= 500 && commandType < 700)
							{
								m_command.Execute(level);
							}
						}
					}

					return 0;
				}
			}

			return -1;
		}
	}
}