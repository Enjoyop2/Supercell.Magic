using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSendArrangedWarRequestCommand : LogicCommand
	{
		private LogicArrayList<LogicLong> m_excludeMemberList;
		private LogicLong m_enemyAllianceId;

		public LogicSendArrangedWarRequestCommand()
		{
			m_excludeMemberList = new LogicArrayList<LogicLong>();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			int count = stream.ReadInt();

			if (count > 0)
			{
				m_excludeMemberList = new LogicArrayList<LogicLong>();
				m_excludeMemberList.EnsureCapacity(count);

				if (count > 50)
				{
					Debugger.Error(string.Format("Number of excluded players ({0}) is too high.", count));
				}

				for (int i = 0; i < count; i++)
				{
					m_excludeMemberList.Add(stream.ReadLong());
				}
			}

			m_enemyAllianceId = stream.ReadLong();

			stream.ReadInt();
			stream.ReadInt();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			if (m_excludeMemberList != null)
			{
				encoder.WriteInt(m_excludeMemberList.Size());

				for (int i = 0; i < m_excludeMemberList.Size(); i++)
				{
					encoder.WriteLong(m_excludeMemberList[i]);
				}
			}

			encoder.WriteLong(m_enemyAllianceId);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SEND_ARRANGED_WAR_REQUEST;

		public override void Destruct()
		{
			base.Destruct();

			m_excludeMemberList = null;
			m_enemyAllianceId = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_excludeMemberList == null || m_excludeMemberList.Size() <= LogicDataTables.GetGlobals().GetWarMaxExcludeMembers())
			{
				if (m_enemyAllianceId != null)
				{
					LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

					if (homeOwnerAvatar.IsInAlliance())
					{
						if (homeOwnerAvatar.GetAllianceRole() == LogicAvatarAllianceRole.LEADER || homeOwnerAvatar.GetAllianceRole() == LogicAvatarAllianceRole.CO_LEADER)
						{
							LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

							if (allianceCastle != null)
							{
								LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

								if (bunkerComponent != null && bunkerComponent.GetArrangedWarCooldownTime() == 0)
								{
									bunkerComponent.StartArrangedWarCooldownTime();
									homeOwnerAvatar.GetChangeListener().StartArrangedWar(m_excludeMemberList, m_enemyAllianceId, 0, 0, 0);

									return 0;
								}

								return -5;
							}

							return -4;
						}

						return -3;
					}

					return -2;
				}

				return -3;
			}

			return -1;
		}
	}
}