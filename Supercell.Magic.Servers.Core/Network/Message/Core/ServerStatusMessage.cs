using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class ServerStatusMessage : ServerCoreMessage
	{
		public ServerStatusType Type
		{
			get; set;
		}
		public int Time
		{
			get; set;
		}
		public int NextTime
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt((int)Type);
			stream.WriteVInt(Time);
			stream.WriteVInt(NextTime);
		}

		public override void Decode(ByteStream stream)
		{
			Type = (ServerStatusType)stream.ReadVInt();
			Time = stream.ReadVInt();
			NextTime = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SERVER_STATUS;
	}
}