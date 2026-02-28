using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSeenBuilderMenuCommand : LogicCommand
	{
		private int m_villageType;

		public LogicSeenBuilderMenuCommand()
		{
			// LogicSeenBuilderMenuCommand.
		}

		public LogicSeenBuilderMenuCommand(int villageType)
		{
			m_villageType = villageType;
		}

		public override void Decode(ByteStream stream)
		{
			m_villageType = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_villageType);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SEEN_BUILDER_MENU;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_villageType == 0)
			{
				level.GetPlayerAvatar().SetVariableByName("SeenBuilderMenu", 1);
				return 0;
			}

			return -1;
		}
	}
}