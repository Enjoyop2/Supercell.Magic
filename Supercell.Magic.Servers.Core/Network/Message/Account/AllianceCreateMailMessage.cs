using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceCreateMailMessage : ServerAccountMessage
	{
		public LogicLong MemberId
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
			stream.WriteString(Message);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			Message = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_CREATE_MAIL;
	}
}