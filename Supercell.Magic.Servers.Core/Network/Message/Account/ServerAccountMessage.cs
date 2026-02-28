using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public abstract class ServerAccountMessage : ServerMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}

		public sealed override void EncodeHeader(ByteStream stream)
		{
			base.EncodeHeader(stream);
			stream.WriteLong(AccountId);
		}

		public sealed override void DecodeHeader(ByteStream stream)
		{
			base.DecodeHeader(stream);
			AccountId = stream.ReadLong();
		}

		public sealed override ServerMessageCategory GetMessageCategory()
			=> ServerMessageCategory.ACCOUNT;
	}
}