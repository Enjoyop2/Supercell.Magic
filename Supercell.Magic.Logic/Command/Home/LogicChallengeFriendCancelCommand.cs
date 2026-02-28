using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicChallengeFriendCancelCommand : LogicCommand
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
			=> LogicCommandType.CHALLENGE_FRIEND_CANCEL;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (LogicDataTables.GetGlobals().UseVersusBattle())
			{
				level.GetGameListener().CancelFriendlyVersusBattle();
				return 0;
			}

			return -2;
		}
	}
}