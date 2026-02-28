using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicAllianceExpEarnedCommand : LogicServerCommand
	{
		private int m_allianceExpLevel;
		private bool m_callListener;

		public LogicAllianceExpEarnedCommand()
		{
			// LogicAllianceExpEarnedCommand.
		}

		public LogicAllianceExpEarnedCommand(int expLevel)
		{
			m_allianceExpLevel = expLevel;
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			stream.ReadInt();
			stream.ReadInt();

			m_allianceExpLevel = stream.ReadInt();
			m_callListener = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(0);
			encoder.WriteInt(0);

			encoder.WriteInt(m_allianceExpLevel);
			encoder.WriteBoolean(m_callListener);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null && playerAvatar.IsInAlliance())
			{
				playerAvatar.SetAllianceLevel(m_allianceExpLevel);

				if (m_callListener)
				{
					playerAvatar.GetChangeListener().AllianceLevelChanged(m_allianceExpLevel);
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ALLIANCE_EXP_EARNED;
	}
}