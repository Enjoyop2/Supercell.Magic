using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public abstract class GameState
	{
		public LogicClientAvatar PlayerAvatar
		{
			get; set;
		}
		public LogicClientHome Home
		{
			get; set;
		}
		public LogicLong HomeId
		{
			get; set;
		}
		public byte[] HomeData
		{
			get; set;
		}

		public int SaveTime { get; set; } = -1;


		public virtual void Encode(ByteStream stream)
		{
			PlayerAvatar.Encode(stream);
			stream.WriteLong(HomeId);
			stream.WriteBytes(HomeData, HomeData.Length);
			stream.WriteVInt(SaveTime);
		}

		public virtual void Decode(ByteStream stream)
		{
			PlayerAvatar = new LogicClientAvatar();
			PlayerAvatar.Decode(stream);
			HomeId = stream.ReadLong();
			HomeData = stream.ReadBytes(stream.ReadBytesLength(), 900000);
			SaveTime = stream.ReadVInt();
		}

		public virtual SimulationServiceNodeType GetSimulationServiceNodeType()
			=> SimulationServiceNodeType.HOME;

		public abstract GameStateType GetGameStateType();
	}

	public enum GameStateType
	{
		HOME,
		NPC_ATTACK,
		NPC_DUEL,
		MATCHED_ATTACK,
		CHALLENGE_ATTACK,
		FAKE_ATTACK,
		VISIT
	}

	public enum SimulationServiceNodeType
	{
		HOME = 10,
		BATTLE = 27
	}
}