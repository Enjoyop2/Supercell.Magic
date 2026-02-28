using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameSpectateLiveReplayMessage : ServerSessionMessage
	{
		public LogicLong LiveReplayId
		{
			get; set;
		}
		public bool IsEnemy
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(LiveReplayId);
			stream.WriteBoolean(IsEnemy);
		}

		public override void Decode(ByteStream stream)
		{
			LiveReplayId = stream.ReadLong();
			IsEnemy = stream.ReadBoolean();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_SPECTATE_LIVE_REPLAY;
	}
}