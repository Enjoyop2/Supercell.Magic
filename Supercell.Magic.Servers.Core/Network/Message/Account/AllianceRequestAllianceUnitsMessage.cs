using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceRequestAllianceUnitsMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}
		public string Message
		{
			get; set;
		}
		public int CastleUpgradeLevel
		{
			get; set;
		}
		public int CastleUsedCapacity
		{
			get; set;
		}
		public int CastleTotalCapacity
		{
			get; set;
		}
		public int CastleSpellUsedCapacity
		{
			get; set;
		}
		public int CastleSpellTotalCapacity
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
			stream.WriteString(Message);
			stream.WriteVInt(CastleUpgradeLevel);
			stream.WriteVInt(CastleUsedCapacity);
			stream.WriteVInt(CastleTotalCapacity);
			stream.WriteVInt(CastleSpellUsedCapacity);
			stream.WriteVInt(CastleSpellTotalCapacity);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			Message = stream.ReadString(900000);
			CastleUpgradeLevel = stream.ReadVInt();
			CastleUsedCapacity = stream.ReadVInt();
			CastleTotalCapacity = stream.ReadVInt();
			CastleSpellUsedCapacity = stream.ReadVInt();
			CastleSpellTotalCapacity = stream.ReadVInt();
		}
		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_REQUEST_ALLIANCE_UNITS;
	}
}