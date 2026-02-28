using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicChangeNameChangeStateCommand : LogicServerCommand
	{
		private int m_state;

		public LogicChangeNameChangeStateCommand()
		{
			// LogicChangeNameStateCommand.
		}

		public LogicChangeNameChangeStateCommand(int state)
		{
			m_state = state;
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			m_state = stream.ReadInt();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);
			encoder.WriteInt(m_state);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				level.GetPlayerAvatar().SetNameChangeState(m_state);
				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_NAME_CHANGE_STATE;
	}
}