using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session.Change
{
	public class CommodityCountAvatarChange : AvatarChange
	{
		public int Type
		{
			get; set;
		}
		public LogicData Data
		{
			get; set;
		}
		public int Count
		{
			get; set;
		}

		public override void Decode(ByteStream stream)
		{
			Type = stream.ReadVInt();
			Data = ByteStreamHelper.ReadDataReference(stream);
			Count = stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(Type);
			ByteStreamHelper.WriteDataReference(stream, Data);
			stream.WriteVInt(Count);
		}

		public override void ApplyAvatarChange(LogicClientAvatar avatar)
		{
			avatar.SetCommodityCount(Type, Data, Count);
		}

		public override void ApplyAvatarChange(AllianceMemberEntry memberEntry)
		{
		}

		public override AvatarChangeType GetAvatarChangeType()
			=> AvatarChangeType.COMMODITY_COUNT;
	}
}