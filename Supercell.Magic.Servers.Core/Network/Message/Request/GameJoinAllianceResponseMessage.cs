using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameJoinAllianceResponseMessage : ServerResponseMessage
	{
		public Reason ErrorReason
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (!Success)
			{
				stream.WriteVInt((int)ErrorReason);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (!Success)
			{
				ErrorReason = (Reason)stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_JOIN_ALLIANCE_RESPONSE;

		public enum Reason
		{
			NO_CASTLE,
			ALREADY_IN_ALLIANCE,
			GENERIC,
			FULL,
			CLOSED,
			SCORE,
			BANNED,
		}
	}
}