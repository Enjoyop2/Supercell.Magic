using Supercell.Magic.Servers.Core.Database.Document;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Core
{
	public class ScoringSyncMessage : ServerCoreMessage
	{
		public SeasonDocument CurrentSeasonDocument
		{
			get; set;
		}
		public SeasonDocument LastSeasonDocument
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			CouchbaseDocument.Encode(stream, CurrentSeasonDocument);

			if (LastSeasonDocument != null)
			{
				stream.WriteBoolean(true);
				CouchbaseDocument.Encode(stream, LastSeasonDocument);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			CurrentSeasonDocument = CouchbaseDocument.Decode<SeasonDocument>(stream);

			if (stream.ReadBoolean())
			{
				LastSeasonDocument = CouchbaseDocument.Decode<SeasonDocument>(stream);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SCORING_SYNC;
	}
}