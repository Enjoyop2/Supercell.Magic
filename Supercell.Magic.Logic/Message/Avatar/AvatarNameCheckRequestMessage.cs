using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarNameCheckRequestMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14600;

		private string m_name;

		public AvatarNameCheckRequestMessage() : this(0)
		{
			// AvatarNameCheckRequestMessage.
		}

		public AvatarNameCheckRequestMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarNameCheckRequestMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_name = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteString(m_name);
		}

		public override short GetMessageType()
			=> AvatarNameCheckRequestMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_name = null;
		}

		public string GetName()
			=> m_name;

		public void SetName(string name)
		{
			m_name = name;
		}
	}
}