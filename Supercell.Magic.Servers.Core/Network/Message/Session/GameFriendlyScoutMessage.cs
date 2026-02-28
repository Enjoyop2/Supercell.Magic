using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameFriendlyScoutMessage : ServerSessionMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public LogicLong StreamId
		{
			get; set;
		}

		public byte[] HomeJSON
		{
			get; set;
		}
		public int MapId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AccountId);
			stream.WriteLong(StreamId);
			stream.WriteBytes(HomeJSON, HomeJSON.Length);
			stream.WriteVInt(MapId);
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
			StreamId = stream.ReadLong();
			HomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
			MapId = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_FRIENDLY_SCOUT;
	}
}