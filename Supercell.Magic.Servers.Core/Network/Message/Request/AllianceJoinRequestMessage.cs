using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class AllianceJoinRequestMessage : ServerRequestMessage
	{
		public LogicLong AllianceId
		{
			get; set;
		}
		public LogicClientAvatar Avatar
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
			stream.WriteLong(AllianceId);
			stream.WriteBoolean(Created);
			stream.WriteBoolean(Invited);

			Avatar.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			AllianceId = stream.ReadLong();
			Created = stream.ReadBoolean();
			Invited = stream.ReadBoolean();

			Avatar = new LogicClientAvatar();
			Avatar.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_JOIN_REQUEST;
	}
}