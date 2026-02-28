using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class CreateAllianceRequestMessage : ServerRequestMessage
	{
		public string AllianceName
		{
			get; set;
		}
		public string AllianceDescription
		{
			get; set;
		}

		public AllianceType AllianceType
		{
			get; set;
		}
		public int AllianceBadgeId
		{
			get; set;
		}
		public int RequiredScore
		{
			get; set;
		}
		public int RequiredDuelScore
		{
			get; set;
		}
		public int WarFrequency
		{
			get; set;
		}

		public bool PublicWarLog
		{
			get; set;
		}
		public bool ArrangedWarEnabled
		{
			get; set;
		}

		public LogicData OriginData
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteString(AllianceName);
			stream.WriteString(AllianceDescription);
			stream.WriteVInt((int)AllianceType);
			stream.WriteVInt(AllianceBadgeId);
			stream.WriteVInt(RequiredScore);
			stream.WriteVInt(RequiredDuelScore);
			stream.WriteVInt(WarFrequency);
			stream.WriteBoolean(PublicWarLog);
			stream.WriteBoolean(ArrangedWarEnabled);
			ByteStreamHelper.WriteDataReference(stream, OriginData);
		}

		public override void Decode(ByteStream stream)
		{
			AllianceName = stream.ReadString(900000);
			AllianceDescription = stream.ReadString(900000);
			AllianceType = (AllianceType)stream.ReadVInt();
			AllianceBadgeId = stream.ReadVInt();
			RequiredScore = stream.ReadVInt();
			RequiredDuelScore = stream.ReadVInt();
			WarFrequency = stream.ReadVInt();
			PublicWarLog = stream.ReadBoolean();
			ArrangedWarEnabled = stream.ReadBoolean();
			OriginData = ByteStreamHelper.ReadDataReference(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_ALLIANCE_REQUEST;
	}
}