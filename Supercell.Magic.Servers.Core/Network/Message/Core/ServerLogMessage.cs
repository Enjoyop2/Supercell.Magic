using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class ServerLogMessage : ServerCoreMessage
	{
		public int LogType
		{
			get; set;
		}
		public string Message
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(LogType);
			stream.WriteString(Message);
		}

		public override void Decode(ByteStream stream)
		{
			LogType = stream.ReadVInt();
			Message = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SERVER_LOG;
	}
}