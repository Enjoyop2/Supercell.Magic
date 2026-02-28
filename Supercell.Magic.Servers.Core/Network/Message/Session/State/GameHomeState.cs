using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameHomeState : GameState
	{
		public int MaintenanceTime
		{
			get; set;
		}
		public int LayoutId
		{
			get; set;
		}
		public int MapId
		{
			get; set;
		}

		public LogicArrayList<LogicServerCommand> ServerCommands
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteVInt(MaintenanceTime);
			stream.WriteVInt(LayoutId);
			stream.WriteVInt(MapId);

			for (int i = 0; i < ServerCommands.Size(); i++)
			{
				LogicCommandManager.EncodeCommand(stream, ServerCommands[i]);
			}
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			MaintenanceTime = stream.ReadVInt();
			LayoutId = stream.ReadVInt();
			MapId = stream.ReadVInt();
			ServerCommands = new LogicArrayList<LogicServerCommand>();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				ServerCommands.Add((LogicServerCommand)LogicCommandManager.DecodeCommand(stream));
			}
		}

		public override GameStateType GetGameStateType()
			=> GameStateType.HOME;
	}
}