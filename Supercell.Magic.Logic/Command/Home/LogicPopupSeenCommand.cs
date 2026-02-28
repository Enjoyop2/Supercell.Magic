using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicPopupSeenCommand : LogicCommand
	{
		private int m_popupType;
		private bool m_seen;

		public override void Decode(ByteStream stream)
		{
			m_popupType = stream.ReadInt();
			m_seen = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_popupType);
			encoder.WriteBoolean(m_seen);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.POPUP_SEEN;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			switch (m_popupType)
			{
				case 0:
					level.SetHelpOpened(m_seen);
					break;
				case 1:
					level.SetEditModeShown();
					break;
				case 2:
					level.SetShieldCostPopupShown(m_seen);
					break;
				case 3:
					break;
				default:
					return -1;
			}

			return 0;
		}
	}
}