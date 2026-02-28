using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class CancelChallengeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14125;

		public CancelChallengeMessage() : this(0)
		{
			// CancelChallengeMessage.
		}

		public CancelChallengeMessage(short messageVersion) : base(messageVersion)
		{
			// CancelChallengeMessage.
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
			=> CancelChallengeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}