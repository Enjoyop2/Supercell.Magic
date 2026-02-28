using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameCreateAllianceInvitationRequestMessage : ServerRequestMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public AllianceInvitationAvatarStreamEntry Entry
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AccountId);
			Entry.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
			Entry = new AllianceInvitationAvatarStreamEntry();
			Entry.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_CREATE_ALLIANCE_INVITATION_REQUEST;
	}
}