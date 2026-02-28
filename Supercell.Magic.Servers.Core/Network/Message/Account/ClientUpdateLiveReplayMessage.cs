using Supercell.Magic.Logic.Command;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class ClientUpdateLiveReplayMessage : ServerAccountMessage
	{
		public int SubTick
		{
			get; set;
		}
		public LogicArrayList<LogicCommand> Commands
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(SubTick);

			if (Commands != null)
			{
				stream.WriteVInt(Commands.Size());

				for (int i = 0; i < Commands.Size(); i++)
				{
					LogicCommandManager.EncodeCommand(stream, Commands[i]);
				}
			}
			else
			{
				stream.WriteVInt(-1);
			}
		}

		public override void Decode(ByteStream stream)
		{
			SubTick = stream.ReadVInt();

			int count = stream.ReadVInt();

			if (count >= 0)
			{
				Commands = new LogicArrayList<LogicCommand>(count);

				for (int i = 0; i < count; i++)
				{
					Commands.Add(LogicCommandManager.DecodeCommand(stream));
				}
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CLIENT_UPDATE_LIVE_REPLAY;
	}
}