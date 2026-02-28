using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class GameAllowServerCommandMessage : ServerAccountMessage
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
			=> ServerMessageType.GAME_ALLOW_SERVER_COMMAND;
	}
}