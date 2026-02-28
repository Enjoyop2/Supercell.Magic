using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class LeaveAllianceMemberMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LEAVE_ALLIANCE_MEMBER;
	}
}