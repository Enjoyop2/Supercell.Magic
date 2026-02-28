using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicTransactionsRevokedCommand : LogicServerCommand
	{
		private int m_diamondCount;

		public void SetAmount(int diamondCount)
		{
			m_diamondCount = diamondCount;
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			m_diamondCount = stream.ReadInt();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);
			encoder.WriteInt(m_diamondCount);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetDiamonds(playerAvatar.GetDiamonds() - m_diamondCount);

				if (playerAvatar.GetFreeDiamonds() > playerAvatar.GetDiamonds())
				{
					playerAvatar.SetFreeDiamonds(playerAvatar.GetDiamonds());
				}

				playerAvatar.AddCumulativePurchasedDiamonds(-m_diamondCount);

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TRANSACTIONS_REVOKED;
	}
}