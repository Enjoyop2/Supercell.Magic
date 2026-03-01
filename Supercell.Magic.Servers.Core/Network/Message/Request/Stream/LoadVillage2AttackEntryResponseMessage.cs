
using Supercell.Magic.Logic.Message.Avatar.Attack;

using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class LoadVillage2AttackEntryResponseMessage : ServerResponseMessage
	{
		public Village2AttackEntry Entry
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (base.Success)
			{
				stream.WriteVInt((int)Entry.GetAttackEntryType());
				Entry.Encode(stream);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (base.Success)
			{
				Entry = Village2AttackEntryFactory.CreateAttackEntryByType(stream.ReadVInt());
				Entry.Decode(stream);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LOAD_VILLAGE2_ATTACK_ENTRY_RESPONSE;
	}
}
