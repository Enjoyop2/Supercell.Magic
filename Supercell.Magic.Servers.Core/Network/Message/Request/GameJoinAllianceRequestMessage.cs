using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameJoinAllianceRequestMessage : ServerRequestMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public LogicLong AllianceId
		{
			get; set;
		}
		public LogicLong AvatarStreamId
		{
			get; set;
		}

		public bool Created
		{
			get; set;
		}
		public bool Invited
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AccountId);
			stream.WriteLong(AllianceId);
			stream.WriteBoolean(Created);
			stream.WriteBoolean(Invited);
			stream.WriteBoolean(AvatarStreamId != null);

			if (AvatarStreamId != null)
			{
				stream.WriteLong(AvatarStreamId);
			}
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
			AllianceId = stream.ReadLong();
			Created = stream.ReadBoolean();
			Invited = stream.ReadBoolean();

			if (stream.ReadBoolean())
			{
				AvatarStreamId = stream.ReadLong();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_JOIN_ALLIANCE_REQUEST;
	}
}