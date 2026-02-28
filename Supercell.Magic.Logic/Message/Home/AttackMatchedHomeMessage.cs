using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class AttackMatchedHomeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14123;

		public AttackMatchedHomeMessage() : this(0)
		{
			// AttackMatchedHomeMessage.
		}

		public AttackMatchedHomeMessage(short messageVersion) : base(messageVersion)
		{
			// AttackMatchedHomeMessage.
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
			=> AttackMatchedHomeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}