using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Offer;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicContinueOfferCommand : LogicServerCommand
	{
		private int m_offerId;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			m_offerId = stream.ReadVInt();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);
			encoder.WriteVInt(m_offerId);
		}

		public override int Execute(LogicLevel level)
		{
			if (level.GetState() == 1)
			{
				LogicOffer offer = level.GetOfferManager().GetOfferById(m_offerId);

				if (offer.GetState() == 1)
				{
					offer.SetState(2);
					return 0;
				}

				return -2;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CONTINUE_OFFER;
	}
}