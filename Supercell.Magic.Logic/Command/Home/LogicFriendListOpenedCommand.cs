using Supercell.Magic.Logic.Level;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicFriendListOpenedCommand : LogicCommand
	{
		public override LogicCommandType GetCommandType()
			=> LogicCommandType.FRIEND_LIST_OPENED;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			level.GetPlayerAvatar().UpdateLastFriendListOpened();
			return 0;
		}
	}
}