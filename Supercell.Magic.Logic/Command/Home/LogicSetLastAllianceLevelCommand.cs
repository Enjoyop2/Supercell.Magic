using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSetLastAllianceLevelCommand : LogicCommand
	{
		private int m_allianceLevel;

		public override void Decode(ByteStream stream)
		{
			m_allianceLevel = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_allianceLevel);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SET_LAST_ALLIANCE_LEVEL;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			level.SetLastAllianceLevel(m_allianceLevel);
			return 0;
		}
	}
}