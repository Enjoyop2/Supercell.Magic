using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class LeaderboardRequestMessage : ServerRequestMessage
	{

		public int Count
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Count);
		}

		public override void Decode(ByteStream stream)
		{
			Count = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LEADERBOARD_REQUEST;
	}
}