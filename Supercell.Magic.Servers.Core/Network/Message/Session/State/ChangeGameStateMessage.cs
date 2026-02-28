using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class ChangeGameStateMessage : ServerSessionMessage
	{
		// -- HOME STATE --
		public int LayoutId
		{
			get; set;
		}
		public int MapId
		{
			get; set;
		}

		// -- NPC ATTACK/DUEL STATE --
		public GameStateType StateType
		{
			get; set;
		}
		public LogicNpcData NpcData
		{
			get; set;
		}

		// -- VISIT --
		public LogicLong HomeId
		{
			get; set;
		}
		public int VisitType
		{
			get; set;
		}

		// -- CHALLENGE --
		public LogicLong ChallengeHomeId
		{
			get; set;
		}
		public LogicLong ChallengeStreamId
		{
			get; set;
		}
		public LogicLong ChallengeAllianceId
		{
			get; set;
		}
		public byte[] ChallengeHomeJSON
		{
			get; set;
		}
		public int ChallengeMapId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt((int)StateType);

			switch (StateType)
			{
				case GameStateType.HOME:
					stream.WriteVInt(LayoutId);
					stream.WriteVInt(MapId);
					break;
				case GameStateType.NPC_ATTACK:
				case GameStateType.NPC_DUEL:
					ByteStreamHelper.WriteDataReference(stream, NpcData);
					break;
				case GameStateType.VISIT:
					stream.WriteLong(HomeId);
					stream.WriteVInt(VisitType);
					break;
				case GameStateType.CHALLENGE_ATTACK:
					stream.WriteLong(ChallengeHomeId);
					stream.WriteLong(ChallengeStreamId);
					stream.WriteLong(ChallengeAllianceId);
					stream.WriteBytes(ChallengeHomeJSON, ChallengeHomeJSON.Length);
					stream.WriteVInt(ChallengeMapId);
					break;
			}
		}

		public override void Decode(ByteStream stream)
		{
			StateType = (GameStateType)stream.ReadVInt();

			switch (StateType)
			{
				case GameStateType.HOME:
					LayoutId = stream.ReadVInt();
					MapId = stream.ReadVInt();
					break;
				case GameStateType.NPC_ATTACK:
				case GameStateType.NPC_DUEL:
					NpcData = (LogicNpcData)ByteStreamHelper.ReadDataReference(stream, LogicDataType.NPC);
					break;
				case GameStateType.VISIT:
					HomeId = stream.ReadLong();
					VisitType = stream.ReadVInt();
					break;
				case GameStateType.CHALLENGE_ATTACK:
					ChallengeHomeId = stream.ReadLong();
					ChallengeStreamId = stream.ReadLong();
					ChallengeAllianceId = stream.ReadLong();
					ChallengeHomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
					ChallengeMapId = stream.ReadVInt();
					break;
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.CHANGE_GAME_STATE;
	}
}