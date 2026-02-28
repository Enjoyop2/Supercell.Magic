using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicChangeChallengeStateCommand : LogicServerCommand
	{
		private LogicLong m_challengeId;
		private LogicLong m_argsId;

		private int m_challengeState;

		public override void Destruct()
		{
			base.Destruct();

			m_challengeId = null;
			m_argsId = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_challengeId = stream.ReadLong();
			m_challengeState = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_argsId = stream.ReadLong();
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_challengeId);
			encoder.WriteInt(m_challengeState);
			encoder.WriteBoolean(m_argsId != null);

			if (m_argsId != null)
			{
				encoder.WriteLong(m_argsId);
			}

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetChallengeId(m_challengeId);
				playerAvatar.SetChallengeState(m_challengeState);

				level.GetGameListener().ChallengeStateChanged(m_challengeId, m_argsId, m_challengeState);

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_CHALLENGE_STATE;
	}
}