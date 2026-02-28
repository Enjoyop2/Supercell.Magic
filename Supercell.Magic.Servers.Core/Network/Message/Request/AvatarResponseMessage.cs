using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class AvatarResponseMessage : ServerResponseMessage
	{
		public LogicClientAvatar LogicClientAvatar
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				LogicClientAvatar.Encode(stream);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				LogicClientAvatar = new LogicClientAvatar();
				LogicClientAvatar.Decode(stream);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.AVATAR_RESPONSE;
	}
}