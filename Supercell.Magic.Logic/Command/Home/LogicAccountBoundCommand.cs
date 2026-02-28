using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicAccountBoundCommand : LogicCommand
	{
		private int m_bound;

		public override void Decode(ByteStream stream)
		{
			m_bound = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_bound);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ACCOUNT_BOUND;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			level.GetPlayerAvatar().SetAccountBound();
			level.GetAchievementManager().RefreshStatus();

			return 0;
		}
	}
}