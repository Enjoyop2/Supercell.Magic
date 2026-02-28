using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class RemoveAvatarStreamMessage : ServerAccountMessage
	{
		public override void Encode(ByteStream stream)
		{
		}

		public override void Decode(ByteStream stream)
		{
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.REMOVE_AVATAR_STREAM;
	}
}