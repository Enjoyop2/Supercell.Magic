using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameStateDataMessage : ServerSessionMessage
	{
		public GameState State
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt((int)State.GetGameStateType());
			State.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			State = GameStateFactory.CreateByType((GameStateType)stream.ReadVInt());
			State.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_STATE_DATA;
	}
}