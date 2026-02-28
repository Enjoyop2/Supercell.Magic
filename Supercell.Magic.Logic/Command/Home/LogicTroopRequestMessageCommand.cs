using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicTroopRequestMessageCommand : LogicCommand
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
			=> LogicCommandType.TROOP_REQUEST_MESSAGE;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
		}

		public override int Execute(LogicLevel level)
		{
			level.SetTroopRequestMessage(m_message);
			return 0;
		}
	}
}