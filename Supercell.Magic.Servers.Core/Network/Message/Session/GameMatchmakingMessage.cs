using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameMatchmakingMessage : ServerSessionMessage
	{
		public GameMatchmakingType MatchmakingType
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt((int)MatchmakingType);
		}

		public override void Decode(ByteStream stream)
		{
			MatchmakingType = (GameMatchmakingType)stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_MATCHMAKING;

		public enum GameMatchmakingType
		{
			DEFAULT,
			DUEL
		}
	}
}