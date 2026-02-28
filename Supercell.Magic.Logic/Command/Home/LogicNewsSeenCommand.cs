using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicNewsSeenCommand : LogicCommand
	{
		private int m_lastSeenNews;

		public LogicNewsSeenCommand()
		{
			// LogicNewsSeenCommand.
		}

		public LogicNewsSeenCommand(int lastSeenNews)
		{
			m_lastSeenNews = lastSeenNews;
		}

		public override void Decode(ByteStream stream)
		{
			m_lastSeenNews = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_lastSeenNews);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.NEWS_SEEN;

		public override int Execute(LogicLevel level)
		{
			level.SetLastSeenNews(m_lastSeenNews);
			return 0;
		}
	}
}