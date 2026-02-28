using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicCancelUpgradeUnitCommand : LogicCommand
	{
		private int m_gameObjectId;

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CANCEL_UPGRADE_UNIT;

		public override void Destruct()
		{
			base.Destruct();
			m_gameObjectId = 0;
		}

		public override int Execute(LogicLevel level)
			=> -1;
	}
}