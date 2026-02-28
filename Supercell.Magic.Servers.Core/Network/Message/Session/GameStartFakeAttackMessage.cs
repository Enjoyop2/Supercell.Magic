using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameStartFakeAttackMessage : ServerSessionMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public LogicData ArgData
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (AccountId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(AccountId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			ByteStreamHelper.WriteDataReference(stream, ArgData);
		}

		public override void Decode(ByteStream stream)
		{
			if (stream.ReadBoolean())
			{
				AccountId = stream.ReadLong();
			}

			ArgData = ByteStreamHelper.ReadDataReference(stream);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_START_FAKE_ATTACK;
	}
}