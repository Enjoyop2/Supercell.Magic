using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using System.Threading.Tasks;

using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IO;
using Couchbase.N1QL;
using Couchbase.Views;

using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Core;
using Supercell.Magic.Servers.Core.Database.Document;
using Supercell.Magic.Servers.Core.Settings;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Database
{
	public class CouchbaseDatabase
	{
		public const string QUERY_HIGHER_ID = "SELECT MAX(" + CouchbaseDocument.JSON_ATTRIBUTE_ID_LOW + ") FROM `%BUCKET%` WHERE meta().id LIKE '%KEY_PREFIX%%' AND " + CouchbaseDocument.JSON_ATTRIBUTE_ID_HIGH + " == %ID_HIGH%";

		private readonly string m_bucketName;
		private readonly string m_keyPrefix;

		private readonly ICluster m_cluster;
		private readonly IBucket m_bucket;

		private int m_seedCounter;

		public CouchbaseDatabase(string type, string keyPrefix)
		{
			if (EnvironmentSettings.Couchbase.TryGetBucketData(type, out EnvironmentSettings.CouchbaseServerEntry serverEntry, out EnvironmentSettings.CouchbaseBucketEntry bucketEntry))
			{
				m_cluster = new Couchbase.Cluster(new ClientConfiguration
				{
					Servers = new List<Uri>(serverEntry.Hosts)
				});
				m_cluster.Authenticate(serverEntry.Username, serverEntry.Password);
				m_bucket = m_cluster.OpenBucket(bucketEntry.Name);
				m_bucketName = bucketEntry.Name;
				m_keyPrefix = keyPrefix + "-";
			}
			else
			{
				Logging.Error("CouchbaseDatabase::ctor: no database available for type " + type);
			}
		}

		public void CreateIndex(string indexName, params string[] args)
		{
			ExecuteCommand<dynamic>(string.Format("CREATE INDEX {0} ON `%BUCKET%` ({1})", indexName, string.Join(",", args))).Wait();
		}

		public void CreateIndexWithFilter(string indexName, string filter, params string[] args)
		{
			ExecuteCommand<dynamic>(string.Format("CREATE INDEX {0} ON `%BUCKET%` ({1}) WHERE {2}", indexName, string.Join(",", args), filter)).Wait();
		}

		public int[] GetCounterHigherIDs()
		{
			int size = EnvironmentSettings.HigherIdCounterSize;
			int[] highIds = new int[size];

			for (int i = 0; i < size; i++)
			{
				IOperationResult<string> result = m_bucket.Get<string>("counter_" + i);

				if (result.Status != ResponseStatus.Success && result.Status != ResponseStatus.KeyNotFound)
					throw new Exception("Db in loading state!");
				if (result.Value != null)
					highIds[i] = (int)ulong.Parse(result.Value);
			}

			return highIds;
		}

		public int[] GetDocumentHigherIDs()
		{
			CreateIndexWithFilter(m_keyPrefix.Replace("-", string.Empty) + "HigherIdIndex", "meta().id LIKE '%KEY_PREFIX%%'", "meta().id", "id_hi", "id_lo");

			int size = EnvironmentSettings.HigherIdCounterSize;
			int[] highIds = new int[size];

			for (int i = 0; i < size; i++)
			{
				IQueryResult<JToken> result = ExecuteCommand<JToken>(CouchbaseDatabase.QUERY_HIGHER_ID.Replace("%ID_HIGH%", i.ToString())).Result;

				if (result.Status != QueryStatus.Success)
					throw new Exception("Db in loading state!");

				JToken value = result.Rows[0]["$1"];

				if (value != null && value.Type == JTokenType.Integer)
					highIds[i] = (int)value;
			}

			return highIds;
		}

		public void Destruct()
		{
			m_bucket.Dispose();
			m_cluster.Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string BuildKeyName(long id)
			=> m_keyPrefix + id;

		public Task<IQueryResult<T>> ExecuteCommand<T>(string query)
			=> m_bucket.QueryAsync<T>(query.Replace("%BUCKET%", m_bucketName).Replace("%KEY_PREFIX%", m_keyPrefix));

		public Task<IViewResult<T>> ExecuteCommand<T>(IViewQuery query)
			=> m_bucket.QueryAsync<T>(query);

		public Task<IOperationResult<string>> Get(long key)
			=> m_bucket.GetAsync<string>(BuildKeyName(key));

		public Task<IDocumentResult<string>[]> Get(IEnumerable<string> ids)
			=> m_bucket.GetDocumentsAsync<string>(ids);

		public Task<IDocumentResult<string>[]> Get(LogicArrayList<LogicLong> ids)
		{
			string[] keys = new string[ids.Size()];
			for (int i = 0; i < keys.Length; i++)
				keys[i] = BuildKeyName(ids[i]);
			return m_bucket.GetDocumentsAsync<string>(keys);
		}

		public Task<IOperationResult<string>> Insert(long key, string json)
			=> m_bucket.InsertAsync(BuildKeyName(key), json);

		public Task<IOperationResult<string>> InsertOrUpdate(long key, string json)
			=> m_bucket.UpsertAsync(BuildKeyName(key), json);

		public Task<IOperationResult<string>> Update(long key, string json)
			=> m_bucket.ReplaceAsync(BuildKeyName(key), json);

		public Task<IOperationResult<string>> Update(long key, string json, ulong cas)
			=> m_bucket.ReplaceAsync(BuildKeyName(key), json, cas);

		public Task<IOperationResult> Remove(long key)
			=> m_bucket.RemoveAsync(BuildKeyName(key));

		public Task<IOperationResult<ulong>> IncrementSeed()
		{
			int seedCounter = m_seedCounter++ % EnvironmentSettings.HigherIdCounterSize;
			return m_bucket.IncrementAsync("counter_" + seedCounter, 1, (ulong)seedCounter << 32 | 1u);
		}
	}
}