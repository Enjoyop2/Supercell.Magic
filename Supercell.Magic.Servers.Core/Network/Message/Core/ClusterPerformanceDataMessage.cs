using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class ClusterPerformanceDataMessage : ServerCoreMessage
	{
		public int Id
		{
			get; set;
		}
		public int SessionCount
		{
			get; set;
		}
		public int Ping
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Id);
			stream.WriteVInt(SessionCount);
			stream.WriteVInt(Ping);
		}

		public override void Decode(ByteStream stream)
		{
			Id = stream.ReadVInt();
			SessionCount = stream.ReadVInt();
			Ping = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CLUSTER_PERFORMANCE_DATA;
	}
}