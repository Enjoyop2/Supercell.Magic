using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicClaimAchievementRewardCommand : LogicCommand
	{
		private LogicAchievementData m_achievementData;

		public LogicClaimAchievementRewardCommand()
		{
			// LogicClaimAchievementRewardCommand.
		}

		public LogicClaimAchievementRewardCommand(LogicAchievementData achievementData)
		{
			m_achievementData = achievementData;
		}

		public override void Decode(ByteStream stream)
		{
			m_achievementData = (LogicAchievementData)ByteStreamHelper.ReadDataReference(stream, DataType.ACHIEVEMENT);
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			ByteStreamHelper.WriteDataReference(encoder, m_achievementData);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CLAIM_ACHIEVEMENT_REWARD;

		public override void Destruct()
		{
			base.Destruct();
			m_achievementData = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null && m_achievementData != null)
			{
				if (playerAvatar.IsAchievementCompleted(m_achievementData) && !playerAvatar.IsAchievementRewardClaimed(m_achievementData))
				{
					playerAvatar.XpGainHelper(m_achievementData.GetExpReward());

					if (m_achievementData.GetDiamondReward() > 0)
					{
						int diamondReward = m_achievementData.GetDiamondReward();

						playerAvatar.SetDiamonds(playerAvatar.GetDiamonds() + diamondReward);
						playerAvatar.SetFreeDiamonds(playerAvatar.GetFreeDiamonds() + diamondReward);
						playerAvatar.GetChangeListener().FreeDiamondsAdded(diamondReward, 4);
					}

					playerAvatar.SetAchievementRewardClaimed(m_achievementData, true);
					playerAvatar.GetChangeListener().CommodityCountChanged(1, m_achievementData, 1);

					return 0;
				}
			}

			return -1;
		}
	}
}