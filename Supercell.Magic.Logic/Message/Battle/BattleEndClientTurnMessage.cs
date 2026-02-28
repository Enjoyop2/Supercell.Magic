using Supercell.Magic.Logic.Command;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Battle
{
	public class BattleEndClientTurnMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14510;

		private int m_subTick;
		private int m_checksum;

		private LogicArrayList<LogicCommand> m_commands;

		public BattleEndClientTurnMessage() : this(0)
		{
			// BattleEndClientTurnMessage.
		}

		public BattleEndClientTurnMessage(short messageVersion) : base(messageVersion)
		{
			// BattleEndClientTurnMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_subTick = m_stream.ReadInt();
			m_checksum = m_stream.ReadInt();

			int arraySize = m_stream.ReadInt();

			if (arraySize <= 512)
			{
				if (arraySize > 0)
				{
					m_commands = new LogicArrayList<LogicCommand>(arraySize);

					do
					{
						LogicCommand command = LogicCommandManager.DecodeCommand(m_stream);

						if (command == null)
						{
							break;
						}

						m_commands.Add(command);
					} while (--arraySize != 0);
				}
			}
			else
			{
				Debugger.Error(string.Format("BattleEndClientTurn::decode() command count is too high! ({0})", arraySize));
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_subTick);
			m_stream.WriteInt(m_checksum);

			if (m_commands != null)
			{
				m_stream.WriteInt(m_commands.Size());

				for (int i = 0; i < m_commands.Size(); i++)
				{
					LogicCommandManager.EncodeCommand(m_stream, m_commands[i]);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> BattleEndClientTurnMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 27;

		public override void Destruct()
		{
			base.Destruct();
			m_commands = null;
		}

		public int GetSubTick()
			=> m_subTick;

		public void SetSubTick(int value)
		{
			m_subTick = value;
		}

		public int GetChecksum()
			=> m_checksum;

		public void SetChecksum(int value)
		{
			m_checksum = value;
		}

		public LogicArrayList<LogicCommand> GetCommands()
			=> m_commands;

		public void SetCommands(LogicArrayList<LogicCommand> commands)
		{
			m_commands = commands;
		}
	}
}