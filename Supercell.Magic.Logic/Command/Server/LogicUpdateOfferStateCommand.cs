using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Offer;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicUpdateOfferStateCommand : LogicServerCommand
	{
		private int m_offerState;
		private int m_offerId;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_offerId = stream.ReadInt();
			m_offerState = stream.ReadInt();
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteInt(m_offerId);
			encoder.WriteInt(m_offerState);
		}

		public override int Execute(LogicLevel level)
		{
			LogicOffer offer = level.GetOfferManager().GetOfferById(m_offerId);

			if (offer != null)
			{
				offer.SetState(m_offerState);
				return 0;
			}

			Debugger.Warning(string.Format("Offer not found when updating offer state for id: {0} to state: {1}", m_offerId, m_offerState));
			return -2;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.UPDATE_OFFER_STATE;
	}
}