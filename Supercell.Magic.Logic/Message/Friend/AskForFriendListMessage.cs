using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Friend
{
	public class AskForFriendListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10504;

		public AskForFriendListMessage() : this(0)
		{
			// AskForFriendListMessage.
		}

		public AskForFriendListMessage(short messageVersion) : base(messageVersion)
		{
			// AskForFriendListMessage.
		}

		public override void Decode()
		{
			base.Decode();
		}

		public override void Encode()
		{
			base.Encode();
		}

		public override short GetMessageType()
			=> AskForFriendListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 3;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}