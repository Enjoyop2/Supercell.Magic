using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicServerCommand : LogicCommand
	{
		private int m_id;

		public LogicServerCommand()
		{
			m_id = -1;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_id = -1;
		}

		public int GetId()
			=> m_id;

		public void SetId(int id)
		{
			m_id = id;
		}

		public override void Decode(ByteStream stream)
		{
			m_id = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_id);
			base.Encode(encoder);
		}

		public sealed override bool IsServerCommand()
			=> true;
	}
}