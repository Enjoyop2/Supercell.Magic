using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceChallengeLiveReplayIdMessage : ServerAccountMessage
	{
		public LogicLong LiveReplayId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(LiveReplayId);
		}

		public override void Decode(ByteStream stream)
		{
			LiveReplayId = stream.ReadLong();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_CHALLENGE_LIVE_REPLAY_ID;
	}
}