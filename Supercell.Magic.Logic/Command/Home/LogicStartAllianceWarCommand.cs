using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicStartAllianceWarCommand : LogicCommand
	{
		private LogicArrayList<LogicLong> m_excludeMemberList;

		public LogicStartAllianceWarCommand()
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
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.START_ALLIANCE_WAR;

		public override void Destruct()
		{
			base.Destruct();
			m_excludeMemberList = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_excludeMemberList == null || m_excludeMemberList.Size() <= LogicDataTables.GetGlobals().GetWarMaxExcludeMembers())
			{
				LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

				if (homeOwnerAvatar.IsInAlliance())
				{
					if (homeOwnerAvatar.GetAllianceRole() == LogicAvatarAllianceRole.LEADER || homeOwnerAvatar.GetAllianceRole() == LogicAvatarAllianceRole.CO_LEADER)
					{
						homeOwnerAvatar.GetChangeListener().StartWar(m_excludeMemberList);
						return 0;
					}

					return -3;
				}

				return -2;
			}

			return -1;
		}
	}
}