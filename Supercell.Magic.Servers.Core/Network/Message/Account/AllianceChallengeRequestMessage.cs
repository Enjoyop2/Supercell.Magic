using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceChallengeRequestMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}
		public string Message
		{
			get; set;
		}
		public byte[] HomeJSON
		{
			get; set;
		}
		public bool WarLayout
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
			stream.WriteString(Message);
			stream.WriteBytes(HomeJSON, HomeJSON.Length);
			stream.WriteBoolean(WarLayout);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			Message = stream.ReadString(900000);
			HomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
			WarLayout = stream.ReadBoolean();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_CHALLENGE_REQUEST;
	}
}