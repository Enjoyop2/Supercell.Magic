using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class RankingEntry
	{
		private const string JSON_ATTRIBUTE_ID = "id";
		private const string JSON_ATTRIBUTE_NAME = "name";
		private const string JSON_ATTRIBUTE_ORDER = "o";
		private const string JSON_ATTRIBUTE_PREVIOUS_ORDER = "prevO";
		private const string JSON_ATTRIBUTE_SCORE = "scr";

		private LogicLong m_id;

		private string m_name;

		private int m_order;
		private int m_previousOrder;
		private int m_score;

		public virtual void Encode(ByteStream stream)
		{
			stream.WriteLong(m_id);
			stream.WriteString(m_name);
			stream.WriteInt(m_order);
			stream.WriteInt(m_score);
			stream.WriteInt(m_previousOrder);
		}

		public virtual void Decode(ByteStream stream)
		{
			m_id = stream.ReadLong();
			m_name = stream.ReadString(900000);
			m_order = stream.ReadInt();
			m_score = stream.ReadInt();
			m_previousOrder = stream.ReadInt();
		}

		public LogicLong GetId()
			=> m_id;

		public void SetId(LogicLong value)
		{
			m_id = value;
		}

		public string GetName()
			=> m_name;

		public void SetName(string name)
		{
			m_name = name;
		}

		public int GetOrder()
			=> m_order;

		public void SetOrder(int order)
		{
			m_order = order;
		}

		public int GetScore()
			=> m_score;

		public void SetScore(int value)
		{
			m_score = value;
		}

		public int GetPreviousOrder()
			=> m_previousOrder;

		public void SetPreviousOrder(int order)
		{
			m_previousOrder = order;
		}

		public virtual LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();
			LogicJSONArray idArray = new LogicJSONArray(2);

			idArray.Add(new LogicJSONNumber(m_id.GetHigherInt()));
			idArray.Add(new LogicJSONNumber(m_id.GetLowerInt()));

			jsonObject.Put(RankingEntry.JSON_ATTRIBUTE_ID, idArray);
			jsonObject.Put(RankingEntry.JSON_ATTRIBUTE_NAME, new LogicJSONString(m_name));
			jsonObject.Put(RankingEntry.JSON_ATTRIBUTE_ORDER, new LogicJSONNumber(m_order));
			jsonObject.Put(RankingEntry.JSON_ATTRIBUTE_PREVIOUS_ORDER, new LogicJSONNumber(m_previousOrder));
			jsonObject.Put(RankingEntry.JSON_ATTRIBUTE_SCORE, new LogicJSONNumber(m_score));

			return jsonObject;
		}

		public virtual void Load(LogicJSONObject jsonObject)
		{
			LogicJSONArray idArray = jsonObject.GetJSONArray(RankingEntry.JSON_ATTRIBUTE_ID);

			m_id = new LogicLong(idArray.GetJSONNumber(0).GetIntValue(), idArray.GetJSONNumber(1).GetIntValue());
			m_name = jsonObject.GetJSONString(RankingEntry.JSON_ATTRIBUTE_NAME).GetStringValue();
			m_order = jsonObject.GetJSONNumber(RankingEntry.JSON_ATTRIBUTE_ORDER).GetIntValue();
			m_previousOrder = jsonObject.GetJSONNumber(RankingEntry.JSON_ATTRIBUTE_PREVIOUS_ORDER).GetIntValue();
			m_score = jsonObject.GetJSONNumber(RankingEntry.JSON_ATTRIBUTE_SCORE).GetIntValue();
		}
	}
}