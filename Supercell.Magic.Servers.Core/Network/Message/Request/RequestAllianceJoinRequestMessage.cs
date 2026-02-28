using Supercell.Magic.Logic.Avatar;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class RequestAllianceJoinRequestMessage : ServerRequestMessage
	{
		public LogicLong AllianceId
		{
			get; set;
		}
		public LogicClientAvatar Avatar
		{
			get; set;
		}

		public string Message
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteString(Message);
			stream.WriteLong(AllianceId);
			Avatar.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			Message = stream.ReadString(900000);
			AllianceId = stream.ReadLong();
			Avatar = new LogicClientAvatar();
			Avatar.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.REQUEST_ALLIANCE_JOIN_REQUEST;
	}
}