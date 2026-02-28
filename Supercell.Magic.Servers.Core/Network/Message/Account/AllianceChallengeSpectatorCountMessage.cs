using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceChallengeSpectatorCountMessage : ServerAccountMessage
	{
		public LogicLong StreamId
		{
			get; set;
		}
		public int Count
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(StreamId);
			stream.WriteVInt(Count);
		}

		public override void Decode(ByteStream stream)
		{
			StreamId = stream.ReadLong();
			Count = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_CHALLENGE_SPECTATOR_COUNT;
	}
}