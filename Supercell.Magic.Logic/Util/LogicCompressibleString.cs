using System;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Util
{
	public class LogicCompressibleString
	{
		private string m_stringValue;
		private byte[] m_compressedData;
		private int m_compressedLength;

		public void Destruct()
		{
			m_stringValue = null;
			m_compressedData = null;
			m_compressedLength = 0;
		}

		public void Decode(ByteStream stream)
		{
			if (stream.ReadBoolean())
			{
				m_compressedLength = stream.ReadBytesLength();
				m_compressedData = stream.ReadBytes(m_compressedLength, 900000);
			}
			else
			{
				m_stringValue = stream.ReadString(900000);
			}
		}

		public void Encode(ChecksumEncoder encoder)
		{
			if (m_compressedData != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteBytes(m_compressedData, m_compressedData.Length);
			}
			else
			{
				encoder.WriteBoolean(false);
				encoder.WriteString(m_stringValue);
			}
		}

		public string Get()
			=> m_stringValue;

		public void Set(string value)
		{
			Set(value, null);
		}

		public void Set(byte[] compressedBytes)
		{
			Set(null, compressedBytes);
		}

		public void Set(string value, byte[] compressedBytes)
		{
			m_stringValue = value;
			m_compressedData = null;
			m_compressedLength = compressedBytes?.Length ?? 0;

			if (m_compressedLength > 0)
			{
				m_compressedData = new byte[m_compressedLength];
				Buffer.BlockCopy(compressedBytes, 0, m_compressedData, 0, m_compressedLength);
			}
		}

		public int GetCompressedLength()
			=> m_compressedLength;

		public bool IsCompressed()
			=> m_stringValue == null && m_compressedLength != 0;

		public byte[] GetCompressed()
			=> m_compressedData;

		public byte[] RemoveCompressed()
		{
			byte[] tmp = m_compressedData;
			m_compressedData = null;
			m_compressedLength = 0;
			return tmp;
		}

		public LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			if (m_stringValue != null)
			{
				jsonObject.Put("s", new LogicJSONString(m_stringValue));
			}

			if (m_compressedData != null)
			{
				jsonObject.Put("c", new LogicJSONString(Convert.ToBase64String(m_compressedData, 0, m_compressedLength)));
			}

			return jsonObject;
		}

		public void Load(LogicJSONObject jsonObject)
		{
			LogicJSONString sString = jsonObject.GetJSONString("s");

			if (sString != null)
			{
				m_stringValue = sString.GetStringValue();
			}

			LogicJSONString cString = jsonObject.GetJSONString("c");

			if (cString != null)
			{
				m_compressedData = Convert.FromBase64String(cString.GetStringValue());
				m_compressedLength = m_compressedData.Length;
			}
		}
	}
}