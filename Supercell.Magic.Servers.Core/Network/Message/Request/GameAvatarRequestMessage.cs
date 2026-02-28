using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameAvatarRequestMessage : ServerRequestMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AccountId);
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_AVATAR_REQUEST;
	}
}