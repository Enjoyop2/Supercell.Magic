using Supercell.Magic.Logic.Command;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Home
{
	public class LiveReplayDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24119;

		private int m_serverSubTick;
		private int m_viewerCount;
		private int m_enemyViewerCount;

		private LogicArrayList<LogicCommand> m_commands;

		public LiveReplayDataMessage() : this(0)
		{
			// LiveReplayDataMessage.
		}

		public LiveReplayDataMessage(short messageVersion) : base(messageVersion)
		{
			// LiveReplayDataMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_serverSubTick = m_stream.ReadVInt();
			m_viewerCount = m_stream.ReadVInt();
			m_enemyViewerCount = m_stream.ReadVInt();

			int count = m_stream.ReadInt();

			if (count <= 512)
			{
				if (count > 0)
				{
					m_commands = new LogicArrayList<LogicCommand>(count);

					for (int i = 0; i < count; i++)
					{
						LogicCommand command = LogicCommandManager.DecodeCommand(m_stream);

						if (command != null)
						{
							m_commands.Add(command);
						}
					}
				}
			}
			else
			{
				Debugger.Error(string.Format("LiveReplayDataMessage::decode() command count is too high! ({0})", count));
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteVInt(m_serverSubTick);
			m_stream.WriteVInt(m_viewerCount);
			m_stream.WriteVInt(m_enemyViewerCount);

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
				m_stream.WriteInt(0);
			}
		}

		public override short GetMessageType()
			=> LiveReplayDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_commands = null;
		}

		public int GetServerSubTick()
			=> m_serverSubTick;

		public void SetServerSubTick(int value)
		{
			m_serverSubTick = value;
		}

		public void SetViewerCount(int value)
		{
			m_viewerCount = value;
		}

		public int GetViewerCount()
			=> m_viewerCount;

		public void SetEnemyViewerCount(int value)
		{
			m_enemyViewerCount = value;
		}

		public int GetEnemyViewerCount()
			=> m_enemyViewerCount;

		public LogicArrayList<LogicCommand> GetCommands()
			=> m_commands;

		public void SetCommands(LogicArrayList<LogicCommand> commands)
		{
			m_commands = commands;
		}
	}
}