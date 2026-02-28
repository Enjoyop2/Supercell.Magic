using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicWarTroopRequestMessageCommand : LogicCommand
	{
		private string m_message;

		public override void Decode(ByteStream stream)
		{
			m_message = stream.ReadString(900000);
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteString(m_message);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.WAR_TROOP_REQUEST_MESSAGE;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
		}

		public override int Execute(LogicLevel level)
		{
			level.SetWarTroopRequestMessage(m_message);

			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.GetChangeListener().WarTroopRequestMessageChanged(m_message);
			}

			return 0;
		}
	}
}