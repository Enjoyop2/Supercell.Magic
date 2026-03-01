using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicDiamondsAddedCommand : LogicServerCommand
	{
		private bool m_freeDiamonds;
		private bool m_bundlePackage;

		private int m_source;
		private int m_diamondsCount;
		private int m_billingPackageId;

		private string m_transactionId;

		public void SetData(bool free, int diamondCount, int billingPackage, bool bundlePackage, int source, string transactionId)
		{
			m_freeDiamonds = free;
			m_diamondsCount = diamondCount;
			m_billingPackageId = billingPackage;
			m_bundlePackage = bundlePackage;
			m_source = source;
			m_transactionId = transactionId;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_transactionId = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_freeDiamonds = stream.ReadBoolean();
			m_diamondsCount = stream.ReadInt();
			m_billingPackageId = stream.ReadInt();
			m_bundlePackage = stream.ReadBoolean();
			m_source = stream.ReadInt();
			m_transactionId = stream.ReadString(900000);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteBoolean(m_freeDiamonds);
			encoder.WriteInt(m_diamondsCount);
			encoder.WriteInt(m_billingPackageId);
			encoder.WriteBoolean(m_bundlePackage);
			encoder.WriteInt(m_source);
			encoder.WriteString(m_transactionId);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (m_source == 1)
				{
					// listener.
				}

				playerAvatar.SetDiamonds(playerAvatar.GetDiamonds() + m_diamondsCount);
				playerAvatar.GetChangeListener().FreeDiamondsAdded(m_diamondsCount, 0);

				if (m_freeDiamonds)
				{
					int freeDiamonds = playerAvatar.GetFreeDiamonds();

					if (m_diamondsCount < 0)
					{
						if (freeDiamonds - m_diamondsCount >= 0 && playerAvatar.GetDiamonds() != freeDiamonds)
						{
							playerAvatar.SetFreeDiamonds(freeDiamonds + m_diamondsCount);
						}
					}
					else
					{
						playerAvatar.SetFreeDiamonds(freeDiamonds + m_diamondsCount);
					}
				}
				else
				{
					if (m_billingPackageId > 0)
					{
						LogicBillingPackageData billingPackageData = (LogicBillingPackageData)LogicDataTables.GetDataById(m_billingPackageId, DataType.BILLING_PACKAGE);

						if (billingPackageData != null)
						{
							if (billingPackageData.IsRED() && !m_bundlePackage)
							{
								int redPackageState = playerAvatar.GetRedPackageState();
								int newRedPackageState = redPackageState | 0x10;

								if ((redPackageState & 3) != 3)
								{
									newRedPackageState = (int)(newRedPackageState & 0xFFFFFFFC);
								}

								playerAvatar.SetRedPackageState(newRedPackageState);
							}
						}
					}

					level.GetGameListener().DiamondsBought();
					playerAvatar.AddCumulativePurchasedDiamonds(m_diamondsCount);
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DIAMONDS_ADDED;
	}
}