using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class CancelMatchmakingMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14103;

		public CancelMatchmakingMessage() : this(0)
		{
			// CancelMatchmakingMessage.
		}

		public CancelMatchmakingMessage(short messageVersion) : base(messageVersion)
		{
			// CancelMatchmakingMessage.
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
			=> CancelMatchmakingMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}