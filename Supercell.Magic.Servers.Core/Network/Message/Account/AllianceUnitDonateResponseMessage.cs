using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceUnitDonateResponseMessage : ServerAccountMessage
	{
		public LogicLong MemberId
		{
			get; set;
		}
		public LogicLong StreamId
		{
			get; set;
		}
		public LogicCombatItemData Data
		{
			get; set;
		}

		public int UpgradeLevel
		{
			get; set;
		}
		public bool QuickDonate
		{
			get; set;
		}
		public bool Success
		{
			get; set;
		}

		public string MemberName
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(MemberId);
			stream.WriteLong(StreamId);

			ByteStreamHelper.WriteDataReference(stream, Data);

			stream.WriteVInt(UpgradeLevel);
			stream.WriteBoolean(QuickDonate);
			stream.WriteBoolean(Success);
			stream.WriteString(MemberName);
		}

		public override void Decode(ByteStream stream)
		{
			MemberId = stream.ReadLong();
			StreamId = stream.ReadLong();
			Data = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream);
			UpgradeLevel = stream.ReadVInt();
			QuickDonate = stream.ReadBoolean();
			Success = stream.ReadBoolean();
			MemberName = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_UNIT_DONATE_RESPONSE;
	}
}