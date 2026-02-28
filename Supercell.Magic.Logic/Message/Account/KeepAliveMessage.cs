using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class KeepAliveMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10108;

		public KeepAliveMessage() : this(0)
		{
			// KeepAliveMessage.
		}

		public KeepAliveMessage(short messageVersion) : base(messageVersion)
		{
			// KeepAliveMessage.
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
			=> KeepAliveMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}