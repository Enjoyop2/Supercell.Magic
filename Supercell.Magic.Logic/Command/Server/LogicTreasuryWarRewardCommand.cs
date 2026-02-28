using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicTreasuryWarRewardCommand : LogicServerCommand
	{
		private LogicLong m_warInstanceId;

		private int m_goldCount;
		private int m_elixirCount;
		private int m_darkElixirCount;

		public void SetDatas(int diamondCount)
		{
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			m_goldCount = stream.ReadInt();
			m_elixirCount = stream.ReadInt();
			m_darkElixirCount = stream.ReadInt();
			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_warInstanceId = stream.ReadLong();
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_goldCount);
			encoder.WriteInt(m_elixirCount);
			encoder.WriteInt(m_darkElixirCount);
			encoder.WriteInt(0);

			if (m_warInstanceId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_warInstanceId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.AddWarReward(m_goldCount, m_elixirCount, m_darkElixirCount, 0, m_warInstanceId);
				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TREASURY_WAR_REWARD;
	}
}