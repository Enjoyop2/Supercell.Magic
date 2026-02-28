using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicChangeLeagueCommand : LogicServerCommand
	{
		private LogicLong m_leagueInstanceId;
		private int m_leagueType;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			if (stream.ReadBoolean())
			{
				m_leagueInstanceId = stream.ReadLong();
			}

			m_leagueType = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			if (m_leagueInstanceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_leagueInstanceId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteInt(m_leagueType);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetLeagueType(m_leagueType);

				if (m_leagueType != 0)
				{
					playerAvatar.SetLeagueInstanceId(m_leagueInstanceId.Clone());
				}
				else
				{
					playerAvatar.SetLeagueInstanceId(null);
					playerAvatar.SetAttackWinCount(0);
					playerAvatar.SetAttackLoseCount(0);
					playerAvatar.SetDefenseWinCount(0);
					playerAvatar.SetDefenseLoseCount(0);
				}

				playerAvatar.GetChangeListener().LeagueChanged(m_leagueType, m_leagueInstanceId);
				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_LEAGUE;

		public void SetDatas(LogicLong id, int leagueType)
		{
			m_leagueInstanceId = id;
			m_leagueType = leagueType;
		}
	}
}