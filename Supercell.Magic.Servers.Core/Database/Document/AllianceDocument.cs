using System;
using System.Collections.Generic;

using Supercell.Magic.Logic.Message.Alliance;
using Supercell.Magic.Logic.Message.Alliance.Stream;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Database.Document
{
	public class AllianceDocument : CouchbaseDocument
	{
		private const string JSON_ATTRIBUTE_DESCRIPTION = "description";
		private const string JSON_ATTRIBUTE_MEMBERS = "members";
		private const string JSON_ATTRIBUTE_KICKED_MEMBER_TIMES = "kickedMembers";
		private const string JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_ID = "id";
		private const string JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_TIME = "t";
		private const string JSON_ATTRIBUTE_STREAM_ENTRY_LIST = "streams";
		private const string JSON_ATTRIBUTE_STREAM_ENTRY_TYPE = "type";
		private const string JSON_ATTRIBUTE_STREAM_ENTRY_ID = "id";
		private const string JSON_ATTRIBUTE_STREAM_ENTRY_SENDER_ID = "sId";

		public string Description
		{
			get; set;
		}
		public AllianceHeaderEntry Header
		{
			get;
		}
		public Dictionary<long, AllianceMemberEntry> Members
		{
			get;
		}
		public Dictionary<long, DateTime> KickedMembersTimes
		{
			get;
		}
		public LogicArrayList<LogicLong> StreamEntryList
		{
			get;
		}

		public AllianceDocument()
		{
			Description = string.Empty;
			Header = new AllianceHeaderEntry();
			Members = new Dictionary<long, AllianceMemberEntry>();
			KickedMembersTimes = new Dictionary<long, DateTime>();
			StreamEntryList = new LogicArrayList<LogicLong>();
		}

		public AllianceDocument(LogicLong id) : base(id)
		{
			Description = string.Empty;
			Header = new AllianceHeaderEntry();
			Header.SetAllianceId(id);
			Members = new Dictionary<long, AllianceMemberEntry>();
			KickedMembersTimes = new Dictionary<long, DateTime>();
			StreamEntryList = new LogicArrayList<LogicLong>();
		}

		public bool IsFull()
			=> Header.GetNumberOfMembers() >= 50;

		protected sealed override void Encode(ByteStream stream)
		{
			throw new NotSupportedException();
		}

		protected sealed override void Decode(ByteStream stream)
		{
			throw new NotSupportedException();
		}

		protected sealed override void Save(LogicJSONObject jsonObject)
		{
			Header.Save(jsonObject);

			jsonObject.Put(AllianceDocument.JSON_ATTRIBUTE_DESCRIPTION, new LogicJSONString(Description));

			LogicJSONArray memberArray = new LogicJSONArray(Members.Count);

			foreach (AllianceMemberEntry entry in Members.Values)
			{
				memberArray.Add(entry.Save());
			}

			jsonObject.Put(AllianceDocument.JSON_ATTRIBUTE_MEMBERS, memberArray);

			LogicJSONArray kickedMemberTimeArray = new LogicJSONArray(KickedMembersTimes.Count);

			foreach (KeyValuePair<long, DateTime> entry in KickedMembersTimes)
			{
				LogicJSONObject entryObject = new LogicJSONObject();
				LogicJSONArray idArray = new LogicJSONArray(2);

				idArray.Add(new LogicJSONNumber((int)(entry.Key >> 32)));
				idArray.Add(new LogicJSONNumber((int)(entry.Key)));

				entryObject.Put(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_ID, idArray);
				entryObject.Put(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_TIME, new LogicJSONString(entry.Value.ToString("O")));

				kickedMemberTimeArray.Add(entryObject);
			}

			jsonObject.Put(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES, kickedMemberTimeArray);

			LogicJSONArray streamArray = new LogicJSONArray(StreamEntryList.Size());

			for (int i = 0; i < StreamEntryList.Size(); i++)
			{
				LogicLong id = StreamEntryList[i];
				LogicJSONArray idArray = new LogicJSONArray(2);

				idArray.Add(new LogicJSONNumber(id.GetHigherInt()));
				idArray.Add(new LogicJSONNumber(id.GetLowerInt()));

				streamArray.Add(idArray);
			}

			jsonObject.Put(AllianceDocument.JSON_ATTRIBUTE_STREAM_ENTRY_LIST, streamArray);
		}

		protected sealed override void Load(LogicJSONObject jsonObject)
		{
			Header.Load(jsonObject);
			Header.SetAllianceId(Id);
			Description = jsonObject.GetJSONString(AllianceDocument.JSON_ATTRIBUTE_DESCRIPTION).GetStringValue();

			LogicJSONArray memberArray = jsonObject.GetJSONArray(AllianceDocument.JSON_ATTRIBUTE_MEMBERS);

			for (int i = 0; i < memberArray.Size(); i++)
			{
				AllianceMemberEntry allianceMemberEntry = new AllianceMemberEntry();
				allianceMemberEntry.Load(memberArray.GetJSONObject(i));
				Members.Add(allianceMemberEntry.GetAvatarId(), allianceMemberEntry);
			}

			LogicJSONArray kickedMemberTimeArray = jsonObject.GetJSONArray(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES);

			for (int i = 0; i < kickedMemberTimeArray.Size(); i++)
			{
				LogicJSONObject obj = kickedMemberTimeArray.GetJSONObject(i);
				LogicJSONArray avatarIdArray = obj.GetJSONArray(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_ID);
				LogicLong avatarId = new LogicLong(avatarIdArray.GetJSONNumber(0).GetIntValue(), avatarIdArray.GetJSONNumber(1).GetIntValue());
				DateTime kickTime = DateTime.Parse(obj.GetJSONString(AllianceDocument.JSON_ATTRIBUTE_KICKED_MEMBER_TIMES_TIME).GetStringValue());

				KickedMembersTimes.Add(avatarId, kickTime);
			}

			LogicJSONArray streamArray = jsonObject.GetJSONArray(AllianceDocument.JSON_ATTRIBUTE_STREAM_ENTRY_LIST);

			for (int i = 0; i < streamArray.Size(); i++)
			{
				LogicJSONArray avatarIdArray = streamArray.GetJSONArray(i);
				LogicLong id = new LogicLong(avatarIdArray.GetJSONNumber(0).GetIntValue(), avatarIdArray.GetJSONNumber(1).GetIntValue());

				StreamEntryList.Add(id);
			}
		}
	}
}