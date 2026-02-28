using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicDuelResourceRewardCommand : LogicServerCommand
	{
		private int m_goldCount;
		private int m_elixirCount;
		private int m_bonusGoldCount;
		private int m_bonusElixirCount;

		private LogicLong m_matchId;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			m_goldCount = stream.ReadInt();
			m_elixirCount = stream.ReadInt();
			m_bonusGoldCount = stream.ReadInt();
			m_bonusElixirCount = stream.ReadInt();
			stream.ReadInt();
			m_matchId = stream.ReadLong();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_goldCount);
			encoder.WriteInt(m_elixirCount);
			encoder.WriteInt(m_bonusGoldCount);
			encoder.WriteInt(m_bonusElixirCount);
			encoder.WriteInt(0);
			encoder.WriteLong(m_matchId);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.AddDuelReward(m_goldCount, m_elixirCount, m_bonusGoldCount, m_bonusElixirCount, m_matchId);
				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DUEL_RESOURCE_REWARD;

		public void SetDatas(int goldCount, int elixirCount, int bonusGoldCount, int bonusElixirCount, LogicLong matchId)
		{
			m_goldCount = goldCount;
			m_elixirCount = elixirCount;
			m_bonusGoldCount = bonusGoldCount;
			m_bonusElixirCount = bonusElixirCount;
			m_matchId = matchId;
		}
	}
}