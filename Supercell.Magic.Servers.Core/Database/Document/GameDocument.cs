using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Server;
using Supercell.Magic.Logic.Home;
using Supercell.Magic.Logic.Message.Avatar.Stream;
using Supercell.Magic.Servers.Core.Util;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Database.Document
{
	public class GameDocument : CouchbaseDocument
	{
		private const string JSON_ATTRIBUTE_HOME = "home";
		private const string JSON_ATTRIBUTE_SAVE_TIME = "saveTime";
		private const string JSON_ATTRIBUTE_MAINTENANCE_TIME = "maintenanceTime";
		private const string JSON_ATTRIBUTE_RECENTLY_MATCHED_ENEMIES = "recentlyEnemies";
		private const string JSON_ATTRIBUTE_ALLIANCE_BOOKMARKS_LIST = "allianceBookmarks";
		private const string JSON_ATTRIBUTE_AVATAR_STREAM_LIST = "avatarStreams";

		public LogicClientAvatar LogicClientAvatar
		{
			get; set;
		}
		public LogicClientHome LogicClientHome
		{
			get; set;
		}
		public LogicArrayList<LogicServerCommand> ServerCommands
		{
			get; set;
		}
		public LogicArrayList<RecentlyEnemy> RecentlyMatchedEnemies
		{
			get; set;
		}
		public LogicArrayList<LogicLong> AllianceBookmarksList
		{
			get; set;
		}
		public LogicArrayList<LogicLong> AvatarStreamList
		{
			get; set;
		}

		public int SaveTime
		{
			get; set;
		}
		public int MaintenanceTime
		{
			get; set;
		}

		public GameDocument()
		{
			LogicClientHome = new LogicClientHome();
			LogicClientAvatar = LogicClientAvatar.GetDefaultAvatar();
			ServerCommands = new LogicArrayList<LogicServerCommand>();
			RecentlyMatchedEnemies = new LogicArrayList<RecentlyEnemy>();
			AllianceBookmarksList = new LogicArrayList<LogicLong>();
			AvatarStreamList = new LogicArrayList<LogicLong>();
			SaveTime = -1;
			MaintenanceTime = -1;
		}

		public GameDocument(LogicLong id) : base(id)
		{
			LogicClientHome = new LogicClientHome();
			LogicClientAvatar = LogicClientAvatar.GetDefaultAvatar();
			ServerCommands = new LogicArrayList<LogicServerCommand>();
			RecentlyMatchedEnemies = new LogicArrayList<RecentlyEnemy>();
			AllianceBookmarksList = new LogicArrayList<LogicLong>();
			AvatarStreamList = new LogicArrayList<LogicLong>();
			SaveTime = -1;
			MaintenanceTime = -1;
			SetLogicId(id);
		}

		private void SetLogicId(LogicLong id)
		{
			LogicClientAvatar.SetId(id);
			LogicClientAvatar.SetCurrentHomeId(id);
			LogicClientHome.SetHomeId(id);
		}

		public int GetRemainingShieldTimeSeconds()
			=> LogicMath.Max(LogicClientHome.GetShieldDurationSeconds() + SaveTime - TimeUtil.GetTimestamp(), 0);

		public int GetRemainingGuardTimeSeconds()
			=> LogicMath.Max(LogicClientHome.GetShieldDurationSeconds() + LogicClientHome.GetGuardDurationSeconds() + SaveTime - TimeUtil.GetTimestamp(), 0);

		protected override void Encode(ByteStream stream)
		{
			LogicClientAvatar.Encode(stream);
			LogicClientHome.Encode(stream);

			stream.WriteVInt(ServerCommands.Size());

			for (int i = 0; i < ServerCommands.Size(); i++)
			{
				LogicCommandManager.EncodeCommand(stream, ServerCommands[i]);
			}

			stream.WriteVInt(RecentlyMatchedEnemies.Size());

			for (int i = 0; i < RecentlyMatchedEnemies.Size(); i++)
			{
				stream.WriteLong(RecentlyMatchedEnemies[i].AvatarId);
				stream.WriteVInt(RecentlyMatchedEnemies[i].Timestamp);
			}

			stream.WriteVInt(AllianceBookmarksList.Size());

			for (int i = 0; i < AllianceBookmarksList.Size(); i++)
			{
				stream.WriteLong(AllianceBookmarksList[i]);
			}

			stream.WriteVInt(AvatarStreamList.Size());

			for (int i = 0; i < AvatarStreamList.Size(); i++)
			{
				stream.WriteLong(AvatarStreamList[i]);
			}

			stream.WriteVInt(SaveTime);
			stream.WriteVInt(MaintenanceTime);
		}

		protected override void Decode(ByteStream stream)
		{
			LogicClientAvatar.Decode(stream);
			LogicClientHome.Decode(stream);
			ServerCommands.Clear();
			RecentlyMatchedEnemies.Clear();
			AllianceBookmarksList.Clear();
			AvatarStreamList.Clear();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				ServerCommands.Add((LogicServerCommand)LogicCommandManager.DecodeCommand(stream));
			}

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				RecentlyMatchedEnemies.Add(new RecentlyEnemy(stream.ReadLong(), stream.ReadVInt()));
			}

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				AllianceBookmarksList.Add(stream.ReadLong());
			}

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				AvatarStreamList.Add(stream.ReadLong());
			}

			SaveTime = stream.ReadVInt();
			MaintenanceTime = stream.ReadVInt();
		}

		protected override void Save(LogicJSONObject jsonObject)
		{
			LogicClientAvatar.Save(jsonObject);

			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_HOME, LogicClientHome.Save());
			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_SAVE_TIME, new LogicJSONNumber(SaveTime));
			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_MAINTENANCE_TIME, new LogicJSONNumber(MaintenanceTime));

			LogicJSONArray recentlyMatchedEnemyArray = new LogicJSONArray(RecentlyMatchedEnemies.Size());

			for (int i = 0; i < RecentlyMatchedEnemies.Size(); i++)
			{
				RecentlyEnemy entry = RecentlyMatchedEnemies[i];
				LogicJSONArray value = new LogicJSONArray(3);

				value.Add(new LogicJSONNumber(entry.AvatarId.GetHigherInt()));
				value.Add(new LogicJSONNumber(entry.AvatarId.GetLowerInt()));
				value.Add(new LogicJSONNumber(entry.Timestamp));

				recentlyMatchedEnemyArray.Add(value);
			}

			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_RECENTLY_MATCHED_ENEMIES, recentlyMatchedEnemyArray);

			LogicJSONArray allianceBookmarksArray = new LogicJSONArray(AllianceBookmarksList.Size());

			for (int i = 0; i < AllianceBookmarksList.Size(); i++)
			{
				LogicLong id = AllianceBookmarksList[i];
				LogicJSONArray ids = new LogicJSONArray(2);

				ids.Add(new LogicJSONNumber(id.GetHigherInt()));
				ids.Add(new LogicJSONNumber(id.GetLowerInt()));

				allianceBookmarksArray.Add(ids);
			}

			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_ALLIANCE_BOOKMARKS_LIST, allianceBookmarksArray);

			LogicJSONArray avatarStreamsArray = new LogicJSONArray(AvatarStreamList.Size());

			for (int i = 0; i < AvatarStreamList.Size(); i++)
			{
				LogicLong id = AvatarStreamList[i];
				LogicJSONArray ids = new LogicJSONArray(2);

				ids.Add(new LogicJSONNumber(id.GetHigherInt()));
				ids.Add(new LogicJSONNumber(id.GetLowerInt()));

				avatarStreamsArray.Add(ids);
			}

			jsonObject.Put(GameDocument.JSON_ATTRIBUTE_AVATAR_STREAM_LIST, avatarStreamsArray);
		}

		protected override void Load(LogicJSONObject jsonObject)
		{
			LogicClientAvatar.Load(jsonObject);
			LogicClientHome.Load(jsonObject.GetJSONObject(GameDocument.JSON_ATTRIBUTE_HOME));

			SaveTime = jsonObject.GetJSONNumber(GameDocument.JSON_ATTRIBUTE_SAVE_TIME).GetIntValue();
			MaintenanceTime = jsonObject.GetJSONNumber(GameDocument.JSON_ATTRIBUTE_MAINTENANCE_TIME).GetIntValue();

			LogicJSONArray recentlyMatchedEnemyArray = jsonObject.GetJSONArray(GameDocument.JSON_ATTRIBUTE_RECENTLY_MATCHED_ENEMIES);

			if (recentlyMatchedEnemyArray != null)
			{
				for (int i = 0; i < recentlyMatchedEnemyArray.Size(); i++)
				{
					LogicJSONArray value = recentlyMatchedEnemyArray.GetJSONArray(i);

					RecentlyMatchedEnemies.Add(new RecentlyEnemy(new LogicLong(value.GetJSONNumber(0).GetIntValue(), value.GetJSONNumber(1).GetIntValue()), value.GetJSONNumber(2).GetIntValue()));
				}
			}

			LogicJSONArray allianceBookmarksArray = jsonObject.GetJSONArray(GameDocument.JSON_ATTRIBUTE_ALLIANCE_BOOKMARKS_LIST);

			if (allianceBookmarksArray != null)
			{
				AllianceBookmarksList.EnsureCapacity(allianceBookmarksArray.Size());

				for (int i = 0; i < allianceBookmarksArray.Size(); i++)
				{
					LogicJSONArray value = allianceBookmarksArray.GetJSONArray(i);
					AllianceBookmarksList.Add(new LogicLong(value.GetJSONNumber(0).GetIntValue(), value.GetJSONNumber(1).GetIntValue()));
				}
			}

			LogicJSONArray avatarStreamArray = jsonObject.GetJSONArray(GameDocument.JSON_ATTRIBUTE_AVATAR_STREAM_LIST);

			if (avatarStreamArray != null)
			{
				AvatarStreamList.EnsureCapacity(avatarStreamArray.Size());

				for (int i = 0; i < avatarStreamArray.Size(); i++)
				{
					LogicJSONArray value = avatarStreamArray.GetJSONArray(i);
					AvatarStreamList.Add(new LogicLong(value.GetJSONNumber(0).GetIntValue(), value.GetJSONNumber(1).GetIntValue()));
				}
			}

			SetLogicId(Id);
		}

		public struct RecentlyEnemy
		{
			public readonly LogicLong AvatarId;
			public readonly int Timestamp;

			public RecentlyEnemy(LogicLong id, int timestamp)
			{
				AvatarId = id;
				Timestamp = timestamp;
			}
		}
	}
}