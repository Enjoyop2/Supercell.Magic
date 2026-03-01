using Supercell.Magic.Logic.Message.Avatar.Attack;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class CreateVillage2AttackEntryRequestMessage : ServerRequestMessage
	{

		public LogicLong OwnerId
		{
			get; set;
		}

		public Village2AttackEntry Entry
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(OwnerId);
			stream.WriteVInt((int)Entry.GetAttackEntryType());
			Entry.Encode(stream);
		}

		public override void Decode(ByteStream stream)
		{
			OwnerId = stream.ReadLong();
			Entry = Village2AttackEntryFactory.CreateAttackEntryByType(stream.ReadVInt());
			Entry.Decode(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CREATE_VILLAGE2_ATTACK_ENTRY_REQUEST;
	}
}