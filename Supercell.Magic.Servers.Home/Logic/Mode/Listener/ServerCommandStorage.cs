using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Listener;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Mode;

using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Home.Logic.Mode.Listener
{
	public class ServerCommandStorage : LogicCommandManagerListener
	{
		private readonly GameMode m_gameMode;
		private readonly LogicGameMode m_logicGameMode;
		private readonly LogicArrayList<LogicServerCommand> m_bufferedServerCommands;
		private readonly LogicArrayList<LogicServerCommand> m_executedServerCommands;

		public ServerCommandStorage(GameMode gameMode, LogicGameMode logicGameMode)
		{
			m_gameMode = gameMode;
			m_logicGameMode = logicGameMode;
			m_bufferedServerCommands = new LogicArrayList<LogicServerCommand>();
			m_executedServerCommands = new LogicArrayList<LogicServerCommand>();
		}

		public override void Destruct()
		{
			base.Destruct();
			m_bufferedServerCommands.Clear();
		}

		public override void CommandExecuted(LogicCommand command)
		{
			if (command.IsServerCommand())
			{
				m_bufferedServerCommands.Remove(m_bufferedServerCommands.IndexOf((LogicServerCommand)command));
				m_executedServerCommands.Add((LogicServerCommand)command);
			}
		}

		public void AddServerCommand(LogicServerCommand serverCommand)
		{
			m_bufferedServerCommands.Add(serverCommand);
		}

		public bool GetAwaitingExecutionOfCommandType(LogicCommandType type)
		{
			for (int i = 0; i < m_bufferedServerCommands.Size(); i++)
			{
				if (m_bufferedServerCommands[i].GetCommandType() == type)
					return true;
			}

			return false;
		}

		public LogicArrayList<LogicServerCommand> RemoveExecutedServerCommands()
		{
			LogicArrayList<LogicServerCommand> arrayList = new LogicArrayList<LogicServerCommand>();
			arrayList.AddAll(m_executedServerCommands);
			m_executedServerCommands.Clear();
			return arrayList;
		}

		public void CheckExecutableServerCommands(int endSubTick, LogicArrayList<LogicCommand> commands)
		{
			for (int i = 0; i < commands.Size(); i++)
			{
				LogicCommand command = commands[i];

				if (command.IsServerCommand())
				{
					if (m_logicGameMode.GetState() != 1)
					{
						commands.Remove(i--);
						continue;
					}

					LogicServerCommand serverCommand = (LogicServerCommand)command;
					LogicServerCommand bufferedServerCommand = null;

					for (int j = 0; j < m_bufferedServerCommands.Size(); j++)
					{
						LogicServerCommand tmp = m_bufferedServerCommands[j];

						if (tmp.GetId() == serverCommand.GetId())
						{
							bufferedServerCommand = tmp;
						}
					}

					if (bufferedServerCommand == null || bufferedServerCommand.GetCommandType() != serverCommand.GetCommandType() ||
						bufferedServerCommand.GetExecuteSubTick() != -1 && bufferedServerCommand.GetExecuteSubTick() >= m_logicGameMode.GetLevel().GetLogicTime().GetTick())
					{
						commands.Remove(i--);
						continue;
					}

					bufferedServerCommand.SetExecuteSubTick(serverCommand.GetExecuteSubTick());
					commands[i] = bufferedServerCommand;
				}
			}
		}
	}
}