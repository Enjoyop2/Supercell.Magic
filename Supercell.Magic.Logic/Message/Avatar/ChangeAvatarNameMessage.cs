using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class ChangeAvatarNameMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10212;

		private string m_avatarName;
		private bool m_nameSetByUser;

		public ChangeAvatarNameMessage() : this(0)
		{
			// ChangeAvatarNameMessage.
		}

		public ChangeAvatarNameMessage(short messageVersion) : base(messageVersion)
		{
			// ChangeAvatarNameMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_avatarName = m_stream.ReadString(900000);
			m_nameSetByUser = m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_avatarName);
			m_stream.WriteBoolean(m_nameSetByUser);
		}

		public override short GetMessageType()
			=> ChangeAvatarNameMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarName = null;
		}

		public string RemoveAvatarName()
		{
			string tmp = m_avatarName;
			m_avatarName = null;
			return tmp;
		}

		public void SetAvatarName(string name)
		{
			m_avatarName = name;
		}

		public bool GetNameSetByUser()
			=> m_nameSetByUser;

		public void SetNameSetByUser(bool set)
		{
			m_nameSetByUser = set;
		}
	}
}