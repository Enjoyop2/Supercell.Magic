using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicHelpOpenedCommand : LogicCommand
	{
		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.HELP_OPENED;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			level.SetHelpOpened(true);
			return 0;
		}
	}
}