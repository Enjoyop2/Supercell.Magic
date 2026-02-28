using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class CreateAllianceResponseMessage : ServerResponseMessage
	{
		public LogicLong AllianceId
		{
			get; set;
		}
		public Reason ErrorReason
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				stream.WriteLong(AllianceId);
			}
			else
			{
				stream.WriteVInt((int)ErrorReason);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				AllianceId = stream.ReadLong();
			}
			else
			{
				ErrorReason = (Reason)stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_ALLIANCE_RESPONSE;

		public enum Reason
		{
			GENERIC,
			INVALID_NAME,
			INVALID_DESCRIPTION,
			NAME_TOO_SHORT,
			NAME_TOO_LONG
		}
	}
}