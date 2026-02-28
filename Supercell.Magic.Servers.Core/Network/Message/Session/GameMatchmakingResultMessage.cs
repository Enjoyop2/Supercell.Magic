using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameMatchmakingResultMessage : ServerSessionMessage
	{
		public LogicLong EnemyId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (EnemyId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(EnemyId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (stream.ReadBoolean())
			{
				EnemyId = stream.ReadLong();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_MATCHMAKING_RESULT;
	}
}