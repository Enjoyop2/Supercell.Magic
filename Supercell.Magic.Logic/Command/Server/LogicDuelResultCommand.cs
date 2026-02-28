using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicDuelResultCommand : LogicServerCommand
	{
		private int m_scoreGain;
		private int m_resultType;

		private bool m_attacker;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			m_scoreGain = stream.ReadInt();
			m_attacker = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_scoreGain);
			encoder.WriteBoolean(m_attacker);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetDuelScore(playerAvatar.GetDuelScore() + m_scoreGain);

				switch (m_resultType)
				{
					case 0:
						playerAvatar.SetDuelLoseCount(playerAvatar.GetDuelLoseCount() + 1);
						break;
					case 1:
						playerAvatar.SetDuelWinCount(playerAvatar.GetDuelWinCount() + 1);
						break;
					case 2:
						playerAvatar.SetDuelDrawCount(playerAvatar.GetDuelDrawCount() + 1);
						break;
				}

				level.GetAchievementManager().RefreshStatus();

				LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

				if (homeOwnerAvatar.GetChangeListener() != null)
				{
					homeOwnerAvatar.GetChangeListener().DuelScoreChanged(homeOwnerAvatar.GetAllianceId(), m_scoreGain, m_resultType, true);
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DUEL_RESULT;

		public void SetData(int scoreGain, int resultType, bool attacker)
		{
			m_scoreGain = scoreGain;
			m_resultType = resultType;
			m_attacker = attacker;
		}
	}
}