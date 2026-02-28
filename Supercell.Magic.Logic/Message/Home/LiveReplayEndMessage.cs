using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class LiveReplayEndMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24126;

		public LiveReplayEndMessage() : this(0)
		{
			// LiveReplayEndMessage.
		}

		public LiveReplayEndMessage(short messageVersion) : base(messageVersion)
		{
			// LiveReplayEndMessage.
		}

		public override short GetMessageType()
			=> LiveReplayEndMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}