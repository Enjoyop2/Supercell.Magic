using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AskForAllianceBookmarksFullDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14341;

		public AskForAllianceBookmarksFullDataMessage() : this(0)
		{
			// AskForAllianceBookmarksFullDataMessage.
		}

		public AskForAllianceBookmarksFullDataMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAllianceBookmarksFullDataMessage.
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
			=> AskForAllianceBookmarksFullDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}