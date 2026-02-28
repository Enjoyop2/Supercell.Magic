using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceShareReplayMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}
		public LogicLong ReplayId
		{
			get; set;
		}

		public string Message
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
			stream.WriteLong(ReplayId);
			stream.WriteString(Message);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			ReplayId = stream.ReadLong();
			Message = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_SHARE_REPLAY;
	}
}