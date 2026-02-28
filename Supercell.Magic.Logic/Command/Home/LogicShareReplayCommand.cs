using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicShareReplayCommand : LogicCommand
	{
		private LogicLong m_battleEntryId;

		private bool m_duelReplay;
		private string m_message;

		public override void Decode(ByteStream stream)
		{
			m_battleEntryId = stream.ReadLong();
			m_duelReplay = stream.ReadBoolean();

			if (stream.ReadBoolean())
			{
				m_message = stream.ReadString(900000);
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_battleEntryId);
			encoder.WriteBoolean(m_duelReplay);

			if (m_message != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteString(m_message);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SHARE_REPLAY;

		public override void Destruct()
		{
			base.Destruct();

			m_message = null;
			m_battleEntryId = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

			if (allianceCastle != null)
			{
				LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

				if (bunkerComponent != null)
				{
					if (bunkerComponent.GetReplayShareCooldownTime() == 0)
					{
						bunkerComponent.StartReplayShareCooldownTime();

						if (m_duelReplay)
						{
							level.GetGameListener().DuelReplayShared(m_battleEntryId);
							level.GetHomeOwnerAvatar().GetChangeListener().ShareDuelReplay(m_battleEntryId, m_message);
						}
						else
						{
							level.GetHomeOwnerAvatar().GetChangeListener().ShareReplay(m_battleEntryId, m_message);
						}

						return 0;
					}
				}
			}

			return -1;
		}
	}
}