using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Offer;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicDeliveringOfferCommand : LogicServerCommand
	{
		private int m_offerId;
		private string m_transactionId;

		private LogicDeliverableBundle m_deliverableBundle;
		private LogicBillingPackageData m_billingPackageData;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_offerId = stream.ReadVInt();
			m_transactionId = stream.ReadString(900000);

			if (m_deliverableBundle != null)
			{
				m_deliverableBundle.Destruct();
				m_deliverableBundle = null;
			}

			m_deliverableBundle = new LogicDeliverableBundle();
			m_deliverableBundle.Decode(stream);
			m_billingPackageData = (LogicBillingPackageData)ByteStreamHelper.ReadDataReference(stream, DataType.BILLING_PACKAGE);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteVInt(m_offerId);
			encoder.WriteString(m_transactionId);

			m_deliverableBundle.Encode(encoder);

			ByteStreamHelper.WriteDataReference(encoder, m_billingPackageData);
		}

		public override int Execute(LogicLevel level)
		{
			if (level.GetGameMode().GetState() == 1)
			{
				LogicAvatar avatar = level.GetHomeOwnerAvatar();

				if (avatar != null)
				{
					if (m_deliverableBundle != null)
					{
						if (m_billingPackageData != null)
						{
							LogicDeliveryHelper.Deliver(level, m_deliverableBundle);
							LogicOffer offer = level.GetOfferManager().GetOfferById(m_offerId);

							if (offer != null)
							{
								offer.SetState(4);
								offer.AddPayCount(1);
							}
							/*else
                            {
                                Debugger.Warning(string.Format("Delivering offerUid:{0}. Offer was no longer found.", this.m_offerId));
                            }*/

							return 0;
						}

						return -4;
					}

					return -3;
				}

				return -2;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DELIVERING_OFFER;

		public void SetDatas(int offerId, string transactionId, LogicDeliverableBundle deliverableBundle, LogicBillingPackageData billingPackageData)
		{
			m_offerId = offerId;
			m_transactionId = transactionId;
			m_deliverableBundle = deliverableBundle;
			m_billingPackageData = billingPackageData;
		}
	}
}