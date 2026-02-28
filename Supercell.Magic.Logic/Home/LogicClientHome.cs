using Supercell.Magic.Logic.Home.Change;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Home
{
	public class LogicClientHome
	{
		private LogicLong m_homeId;
		private LogicHomeChangeListener m_listener;

		private int m_shieldDurationSeconds;
		private int m_guardDurationSeconds;
		private int m_personalBreakSeconds;

		private LogicCompressibleString m_compressibleHomeJson;
		private LogicCompressibleString m_compressibleGlobalJson;
		private LogicCompressibleString m_compressibleCalendarJson;

		public LogicClientHome()
		{
			m_compressibleHomeJson = new LogicCompressibleString();
			m_compressibleGlobalJson = new LogicCompressibleString();
			m_compressibleCalendarJson = new LogicCompressibleString();

			Init();
		}

		public void Destruct()
		{
			if (m_compressibleGlobalJson != null)
			{
				m_compressibleGlobalJson.Destruct();
				m_compressibleGlobalJson = null;
			}

			if (m_compressibleCalendarJson != null)
			{
				m_compressibleCalendarJson.Destruct();
				m_compressibleCalendarJson = null;
			}

			if (m_compressibleHomeJson != null)
			{
				m_compressibleHomeJson.Destruct();
				m_compressibleHomeJson = null;
			}

			if (m_listener != null)
			{
				m_listener.Destruct();
				m_listener = null;
			}

			m_homeId = null;
		}

		public void Init()
		{
			m_homeId = new LogicLong();
			m_listener = new LogicHomeChangeListener();
		}

		public virtual void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_homeId);

			encoder.WriteInt(m_shieldDurationSeconds);
			encoder.WriteInt(m_guardDurationSeconds);
			encoder.WriteInt(m_personalBreakSeconds);

			m_compressibleHomeJson.Encode(encoder);
			m_compressibleCalendarJson.Encode(encoder);
			m_compressibleGlobalJson.Encode(encoder);
		}

		public void Decode(ByteStream stream)
		{
			m_homeId = stream.ReadLong();
			m_shieldDurationSeconds = stream.ReadInt();
			m_guardDurationSeconds = stream.ReadInt();
			m_personalBreakSeconds = stream.ReadInt();

			m_compressibleHomeJson.Decode(stream);
			m_compressibleCalendarJson.Decode(stream);
			m_compressibleGlobalJson.Decode(stream);
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public int GetShieldDurationSeconds()
			=> m_shieldDurationSeconds;

		public int GetGuardDurationSeconds()
			=> m_guardDurationSeconds;

		public int GetPersonalBreakSeconds()
			=> m_personalBreakSeconds;

		public LogicCompressibleString GetCompressibleCalendarJSON()
			=> m_compressibleCalendarJson;

		public LogicCompressibleString GetCompressibleGlobalJSON()
			=> m_compressibleGlobalJson;

		public LogicCompressibleString GetCompressibleHomeJSON()
			=> m_compressibleHomeJson;

		public string GetHomeJSON()
			=> m_compressibleHomeJson.Get();

		public void SetHomeJSON(string json)
		{
			m_compressibleHomeJson.Set(json);
		}

		public string GetCalendarJSON()
			=> m_compressibleCalendarJson.Get();

		public void SetCalendarJSON(string json)
		{
			m_compressibleCalendarJson.Set(json);
		}

		public string GetGlobalJSON()
			=> m_compressibleGlobalJson.Get();

		public void SetGlobalJSON(string json)
		{
			m_compressibleGlobalJson.Set(json);
		}

		public void SetShieldDurationSeconds(int secs)
		{
			m_shieldDurationSeconds = secs;
		}

		public void SetGuardDurationSeconds(int secs)
		{
			m_guardDurationSeconds = secs;
		}

		public void SetPersonalBreakSeconds(int secs)
		{
			m_personalBreakSeconds = secs;
		}

		public LogicHomeChangeListener GetChangeListener()
			=> m_listener;

		public void SetChangeListener(LogicHomeChangeListener listener)
		{
			m_listener = listener;
		}

		public LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("homeJSON", m_compressibleHomeJson.Save());
			jsonObject.Put("shield_t", new LogicJSONNumber(m_shieldDurationSeconds));
			jsonObject.Put("guard_t", new LogicJSONNumber(m_guardDurationSeconds));
			jsonObject.Put("personal_break_t", new LogicJSONNumber(m_personalBreakSeconds));

			return jsonObject;
		}

		public void Load(LogicJSONObject jsonObject)
		{
			m_compressibleHomeJson.Load(jsonObject.GetJSONObject("homeJSON"));

			m_shieldDurationSeconds = jsonObject.GetJSONNumber("shield_t").GetIntValue();
			m_guardDurationSeconds = jsonObject.GetJSONNumber("guard_t").GetIntValue();
			m_personalBreakSeconds = jsonObject.GetJSONNumber("personal_break_t").GetIntValue();
		}
	}
}