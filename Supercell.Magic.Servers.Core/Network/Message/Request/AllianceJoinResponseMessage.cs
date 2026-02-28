using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class AllianceJoinResponseMessage : ServerResponseMessage
	{
		public LogicLong AllianceId
		{
			get; set;
		}
		public string AllianceName
		{
			get; set;
		}
		public int AllianceLevel
		{
			get; set;
		}
		public int AllianceBadgeId
		{
			get; set;
		}
		public bool Created
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
				stream.WriteString(AllianceName);
				stream.WriteVInt(AllianceLevel);
				stream.WriteVInt(AllianceBadgeId);
				stream.WriteBoolean(Created);
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
				AllianceName = stream.ReadString(900000);
				AllianceLevel = stream.ReadVInt();
				AllianceBadgeId = stream.ReadVInt();
				Created = stream.ReadBoolean();
			}
			else
			{
				ErrorReason = (Reason)stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_JOIN_RESPONSE;

		public enum Reason
		{
			GENERIC,
			FULL,
			CLOSED,
			SCORE,
			BANNED,
		}
	}
}