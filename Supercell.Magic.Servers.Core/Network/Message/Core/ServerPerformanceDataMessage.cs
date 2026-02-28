using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class ServerPerformanceDataMessage : ServerCoreMessage
	{
		public int SessionCount
		{
			get; set;
		}
		public int ClusterCount
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(SessionCount);
			stream.WriteVInt(ClusterCount);
		}

		public override void Decode(ByteStream stream)
		{
			SessionCount = stream.ReadVInt();
			ClusterCount = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SERVER_PERFORMANCE_DATA;
	}
}