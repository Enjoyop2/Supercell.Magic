using Supercell.Magic.Logic.Command;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class AvailableServerCommandMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24111;

		private LogicCommand m_serverCommand;

		public AvailableServerCommandMessage() : this(0)
		{
			// AvailableServerCommandMessage.
		}

		public AvailableServerCommandMessage(short messageVersion) : base(messageVersion)
		{
			// AvailableServerCommandMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_serverCommand = LogicCommandManager.DecodeCommand(m_stream);
		}

		public override void Encode()
		{
			base.Encode();
			LogicCommandManager.EncodeCommand(m_stream, m_serverCommand);
		}

		public override short GetMessageType()
			=> AvailableServerCommandMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
			m_serverCommand = null;
		}

		public LogicCommand RemoveServerCommand()
		{
			LogicCommand tmp = m_serverCommand;
			m_serverCommand = null;
			return tmp;
		}

		public void SetServerCommand(LogicCommand command)
		{
			m_serverCommand = command;
		}
	}
}