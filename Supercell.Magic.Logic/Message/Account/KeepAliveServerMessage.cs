using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class KeepAliveServerMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20108;

		public KeepAliveServerMessage() : this(0)
		{
			// KeepAliveServerMessage.
		}

		public KeepAliveServerMessage(short messageVersion) : base(messageVersion)
		{
			// KeepAliveServerMessage.
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
			=> KeepAliveServerMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}