using Supercell.Magic.Servers.Core.Database.Document;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class GameAvatarResponseMessage : ServerResponseMessage
	{
		public GameDocument Document
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				CouchbaseDocument.Encode(stream, Document);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				Document = CouchbaseDocument.Decode<GameDocument>(stream);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_AVATAR_RESPONSE;
	}
}