using Supercell.Magic.Logic.Command;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Home
{
	public class LiveReplayHeaderMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24118;

		private int m_serverSubTick;
		private string m_streamHeaderJson;
		private byte[] m_compressedstreamHeaderJson;

		private LogicArrayList<LogicCommand> m_commands;

		public LiveReplayHeaderMessage() : this(0)
		{
			// LiveReplayHeaderMessage.
		}

		public LiveReplayHeaderMessage(short messageVersion) : base(messageVersion)
		{
			// LiveReplayHeaderMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_streamHeaderJson = m_stream.ReadString(900000);
			m_compressedstreamHeaderJson = m_stream.ReadBytes(m_stream.ReadBytesLength(), 900000);
			m_serverSubTick = m_stream.ReadInt();

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
				Debugger.Error(string.Format("LiveReplayHeaderMessage::decode() command count is too high! ({0})", count));
			}

			m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_streamHeaderJson);
			m_stream.WriteBytes(m_compressedstreamHeaderJson, m_compressedstreamHeaderJson.Length);
			m_stream.WriteInt(m_serverSubTick);

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

			m_stream.WriteInt(0);
		}

		public override short GetMessageType()
			=> LiveReplayHeaderMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();

			m_compressedstreamHeaderJson = null;
			m_commands = null;
		}

		public int GetServerSubTick()
			=> m_serverSubTick;

		public void SetServerSubTick(int value)
		{
			m_serverSubTick = value;
		}

		public LogicArrayList<LogicCommand> GetCommands()
			=> m_commands;

		public void SetCommands(LogicArrayList<LogicCommand> commands)
		{
			m_commands = commands;
		}

		public void SetCompressedStreamHeaderJson(byte[] value)
		{
			m_compressedstreamHeaderJson = value;
		}

		public byte[] RemoveCompressedstreamHeaderJson()
		{
			byte[] tmp = m_compressedstreamHeaderJson;
			m_compressedstreamHeaderJson = null;
			return tmp;
		}
	}
}