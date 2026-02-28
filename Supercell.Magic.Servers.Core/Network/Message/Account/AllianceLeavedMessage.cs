using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceLeavedMessage : ServerAccountMessage
	{
		public LogicLong AllianceId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AllianceId);
		}

		public override void Decode(ByteStream stream)
		{
			AllianceId = stream.ReadLong();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_LEAVED;
	}
}