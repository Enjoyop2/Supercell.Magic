using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSetPersistentBoolCommand : LogicCommand
	{
		private int m_index;
		private bool m_value;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_index = stream.ReadInt();
			m_value = stream.ReadBoolean();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteInt(m_index);
			encoder.WriteBoolean(m_value);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SET_PERSISTENT_BOOL;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			switch (m_index)
			{
				case 0:
					level.SetPersistentBool(0, m_value);
					return 0;
				default:
					return -1;
			}
		}
	}
}