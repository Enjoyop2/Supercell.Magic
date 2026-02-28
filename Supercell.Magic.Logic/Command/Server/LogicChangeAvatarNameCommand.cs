using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicChangeAvatarNameCommand : LogicServerCommand
	{
		private string m_avatarName;
		private int m_nameChangeState;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarName = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_avatarName = stream.ReadString(900000);
			m_nameChangeState = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteString(m_avatarName);
			encoder.WriteInt(m_nameChangeState);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetName(m_avatarName);
				playerAvatar.SetNameSetByUser(true);
				playerAvatar.SetNameChangeState(m_nameChangeState);

				level.GetGameListener().NameChanged(m_avatarName);

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_AVATAR_NAME;

		public void SetAvatarName(string avatarName)
		{
			m_avatarName = avatarName;
		}

		public void SetAvatarNameChangeState(int state)
		{
			m_nameChangeState = state;
		}
	}
}