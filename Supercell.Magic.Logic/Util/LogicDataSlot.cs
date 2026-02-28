using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Util
{
	public class LogicDataSlot
	{
		private LogicData m_data;
		private int m_count;

		public LogicDataSlot(LogicData data, int count)
		{
			m_data = data;
			m_count = count;
		}

		public LogicDataSlot Clone()
			=> new LogicDataSlot(m_data, m_count);

		public void Destruct()
		{
			m_data = null;
		}

		public void Decode(ByteStream stream)
		{
			m_data = ByteStreamHelper.ReadDataReference(stream);
			m_count = stream.ReadInt();
		}

		public void Encode(ChecksumEncoder encoder)
		{
			ByteStreamHelper.WriteDataReference(encoder, m_data);
			encoder.WriteInt(m_count);
		}

		public LogicData GetData()
			=> m_data;

		public int GetCount()
			=> m_count;

		public void GetChecksum(ChecksumHelper checksumHelper)
		{
			checksumHelper.StartObject("LogicDataSlot");

			if (m_data != null)
			{
				checksumHelper.WriteValue("globalID", m_data.GetGlobalID());
			}

			checksumHelper.WriteValue("m_count", m_count);
			checksumHelper.EndObject();
		}

		public void SetCount(int count)
		{
			m_count = count;
		}

		public void ReadFromJSON(LogicJSONObject jsonObject)
		{
			LogicJSONNumber id = jsonObject.GetJSONNumber("id");

			if (id != null && id.GetIntValue() != 0)
			{
				m_data = LogicDataTables.GetDataById(id.GetIntValue());
			}

			m_count = LogicJSONHelper.GetInt(jsonObject, "cnt");
		}

		public void WriteToJSON(LogicJSONObject jsonObject)
		{
			jsonObject.Put("id", new LogicJSONNumber(m_data != null ? m_data.GetGlobalID() : 0));
			jsonObject.Put("cnt", new LogicJSONNumber(m_count));
		}
	}
}