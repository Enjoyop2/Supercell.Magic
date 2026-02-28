using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class RequestAllianceJoinResponseMessage : ServerResponseMessage
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
			=> ServerMessageType.REQUEST_ALLIANCE_JOIN_RESPONSE;

		public enum Reason
		{
			GENERIC,
			CLOSED,
			ALREADY_SENT,
			NO_SCORE,
			BANNED,
			TOO_MANY_PENDING_REQUESTS,
			NO_DUEL_SCORE,
		}
	}
}