using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameCreateAllianceInvitationResponseMessage : ServerResponseMessage
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
			=> ServerMessageType.GAME_CREATE_ALLIANCE_INVITATION_RESPONSE;

		public enum Reason
		{
			GENERIC,
			NO_CASTLE,
			ALREADY_IN_ALLIANCE,
			ALREADY_HAS_AN_INVITE,
			HAS_TOO_MANY_INVITES,
		}
	}
}