using Supercell.Magic.Servers.Core.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Database.Document
{
	public class AccountDocument : CouchbaseDocument
	{
		private const string JSON_ATTRIBUTE_PASS_TOKEN = "passToken";
		private const string JSON_ATTRIBUTE_STATE = "state";
		private const string JSON_ATTRIBUTE_STATE_ARG = "stateArg";
		private const string JSON_ATTRIBUTE_RANK = "rankingType";
		private const string JSON_ATTRIBUTE_COUNTRY = "country";
		private const string JSON_ATTRIBUTE_FACEBOOK_ID = "facebookId";
		private const string JSON_ATTRIBUTE_GAMECENTER_ID = "gamecenterId";
		private const string JSON_ATTRIBUTE_CREATE_TIME = "createTime";
		private const string JSON_ATTRIBUTE_LAST_SESSION_TIME = "lastSessionTime";
		private const string JSON_ATTRIBUTE_SESSION_COUNT = "sessionCount";
		private const string JSON_ATTRIBUTE_PLAY_TIME_SECS = "playTimeSecs";

		public const string CHARS = "abcdefghijklmnopqrstuvwxyz0123456789";
		public const int PASS_TOKEN_LENGTH = 40;

		public string PassToken
		{
			get; private set;
		}
		public string Country
		{
			get; set;
		}
		public string FacebookId
		{
			get; set;
		}
		public string GamecenterId
		{
			get; set;
		}
		public int CreateTime
		{
			get; private set;
		}
		public int LastSessionTime
		{
			get; set;
		}
		public int SessionCount
		{
			get; set;
		}
		public int PlayTimeSeconds
		{
			get; set;
		}

		public AccountState State
		{
			get; set;
		}
		public AccountRankingType Rank
		{
			get; set;
		}

		public string StateArg
		{
			get; set;
		}

		public AccountDocument()
		{
		}

		public AccountDocument(LogicLong id) : base(id)
		{
		}

		public void Init()
		{
			char[] chars = new char[AccountDocument.PASS_TOKEN_LENGTH];

			for (int i = 0; i < AccountDocument.PASS_TOKEN_LENGTH; i++)
			{
				chars[i] = AccountDocument.CHARS[ServerCore.Random.Rand(AccountDocument.CHARS.Length)];
			}

			PassToken = new string(chars);
			State = AccountState.NORMAL;
			Rank = AccountRankingType.NORMAL;
			CreateTime = TimeUtil.GetTimestamp();
		}

		protected override void Save(LogicJSONObject jsonObject)
		{
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_PASS_TOKEN, new LogicJSONString(PassToken));
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_STATE, new LogicJSONNumber((int)State));

			if (State != AccountState.NORMAL && StateArg != null)
				jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_STATE_ARG, new LogicJSONString(StateArg));

			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_RANK, new LogicJSONNumber((int)Rank));
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_COUNTRY, new LogicJSONString(Country));

			if (FacebookId != null)
				jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_FACEBOOK_ID, new LogicJSONString(FacebookId));
			if (GamecenterId != null)
				jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_GAMECENTER_ID, new LogicJSONString(GamecenterId));

			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_CREATE_TIME, new LogicJSONNumber(CreateTime));
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_LAST_SESSION_TIME, new LogicJSONNumber(LastSessionTime));
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_SESSION_COUNT, new LogicJSONNumber(SessionCount));
			jsonObject.Put(AccountDocument.JSON_ATTRIBUTE_PLAY_TIME_SECS, new LogicJSONNumber(PlayTimeSeconds));
		}

		protected override void Load(LogicJSONObject jsonObject)
		{
			PassToken = jsonObject.GetJSONString(AccountDocument.JSON_ATTRIBUTE_PASS_TOKEN).GetStringValue();
			State = (AccountState)jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_STATE).GetIntValue();

			if (State != AccountState.NORMAL)
				StateArg = jsonObject.GetJSONString(AccountDocument.JSON_ATTRIBUTE_STATE_ARG)?.GetStringValue();

			Rank = (AccountRankingType)jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_RANK).GetIntValue();
			Country = jsonObject.GetJSONString(AccountDocument.JSON_ATTRIBUTE_COUNTRY).GetStringValue();
			FacebookId = jsonObject.GetJSONString(AccountDocument.JSON_ATTRIBUTE_FACEBOOK_ID)?.GetStringValue();
			GamecenterId = jsonObject.GetJSONString(AccountDocument.JSON_ATTRIBUTE_GAMECENTER_ID)?.GetStringValue();
			CreateTime = jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_CREATE_TIME).GetIntValue();
			LastSessionTime = jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_LAST_SESSION_TIME).GetIntValue();
			SessionCount = jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_SESSION_COUNT).GetIntValue();
			PlayTimeSeconds = jsonObject.GetJSONNumber(AccountDocument.JSON_ATTRIBUTE_PLAY_TIME_SECS).GetIntValue();
		}

		protected override void Encode(ByteStream stream)
		{
			stream.WriteString(PassToken);
			stream.WriteVInt((int)State);
			stream.WriteVInt((int)Rank);
			stream.WriteString(Country);
			stream.WriteString(FacebookId);
			stream.WriteString(GamecenterId);
			stream.WriteVInt(CreateTime);
			stream.WriteVInt(LastSessionTime);
			stream.WriteVInt(SessionCount);
			stream.WriteVInt(PlayTimeSeconds);
		}

		protected override void Decode(ByteStream stream)
		{
			PassToken = stream.ReadString(900000);
			State = (AccountState)stream.ReadVInt();
			Rank = (AccountRankingType)stream.ReadVInt();
			Country = stream.ReadString(900000);
			FacebookId = stream.ReadString(900000);
			GamecenterId = stream.ReadString(900000);
			CreateTime = stream.ReadVInt();
			LastSessionTime = stream.ReadVInt();
			SessionCount = stream.ReadVInt();
			PlayTimeSeconds = stream.ReadVInt();
		}
	}

	public enum AccountState
	{
		NORMAL,
		LOCKED,
		BANNED
	}

	public enum AccountRankingType
	{
		NORMAL,
		PREMIUM,
		ADMIN
	}
}