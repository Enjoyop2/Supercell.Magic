using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicLeagueNotificationSeenCommand : LogicCommand
	{
		private int m_lastLeagueRank;
		private int m_lastSeasonSeen;

		public override void Decode(ByteStream stream)
		{
			m_lastLeagueRank = stream.ReadInt();
			m_lastSeasonSeen = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_lastLeagueRank);
			encoder.WriteInt(m_lastSeasonSeen);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.LEAGUE_NOTIFICATION_SEEN;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			level.SetLastLeagueRank(m_lastLeagueRank);
			level.SetLastLeagueShuffle(false);
			level.SetLastSeasonSeen(m_lastSeasonSeen);

			return 0;
		}
	}
}