using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Server;


using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class HomeServerCommandAllowedMessage : ServerSessionMessage
	{
		public LogicServerCommand ServerCommand
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			LogicCommandManager.EncodeCommand(stream, ServerCommand);
		}

		public override void Decode(ByteStream stream)
		{
			ServerCommand = (LogicServerCommand)LogicCommandManager.DecodeCommand(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.HOME_SERVER_COMMAND_ALLOWED;
	}
}