using System.Text;

using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONObject : LogicJSONNode
	{
		private LogicArrayList<string> m_keys;
		private LogicArrayList<LogicJSONNode> m_items;

		public LogicJSONObject()
		{
			m_keys = new LogicArrayList<string>(20);
			m_items = new LogicArrayList<LogicJSONNode>(20);
		}

		public LogicJSONObject(int capacity)
		{
			m_keys = new LogicArrayList<string>(capacity);
			m_items = new LogicArrayList<LogicJSONNode>(capacity);
		}

		public void Destruct()
		{
			if (m_keys != null)
			{
				m_keys.Destruct();
				m_keys = null;
			}

			if (m_items != null)
			{
				m_items.Destruct();
				m_items = null;
			}
		}

		public LogicJSONNode Get(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			return m_items[itemIndex];
		}

		public LogicJSONArray GetJSONArray(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			LogicJSONNode node = m_items[itemIndex];

			if (node.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
			{
				return (LogicJSONArray)node;
			}

			Debugger.Warning(string.Format("LogicJSONObject::getJSONArray type is {0}, key {1}", node.GetJSONNodeType(), key));

			return null;
		}

		public LogicJSONBoolean GetJSONBoolean(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			LogicJSONNode node = m_items[itemIndex];

			if (node.GetJSONNodeType() == LogicJSONNodeType.BOOLEAN)
			{
				return (LogicJSONBoolean)node;
			}

			Debugger.Warning(string.Format("LogicJSONObject::getJSONBoolean type is {0}, key {1}", node.GetJSONNodeType(), key));

			return null;
		}

		public LogicJSONNumber GetJSONNumber(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			LogicJSONNode node = m_items[itemIndex];

			if (node.GetJSONNodeType() == LogicJSONNodeType.NUMBER)
			{
				return (LogicJSONNumber)node;
			}

			Debugger.Warning(string.Format("LogicJSONObject::getJSONNumber type is {0}, key {1}", node.GetJSONNodeType(), key));

			return null;
		}

		public LogicJSONObject GetJSONObject(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			LogicJSONNode node = m_items[itemIndex];

			if (node.GetJSONNodeType() == LogicJSONNodeType.OBJECT)
			{
				return (LogicJSONObject)node;
			}

			Debugger.Warning(string.Format("LogicJSONObject::getJSONObject type is {0}, key {1}", node.GetJSONNodeType(), key));

			return null;
		}

		public LogicJSONString GetJSONString(string key)
		{
			int itemIndex = m_keys.IndexOf(key);

			if (itemIndex == -1)
			{
				return null;
			}

			LogicJSONNode node = m_items[itemIndex];

			if (node.GetJSONNodeType() == LogicJSONNodeType.STRING)
			{
				return (LogicJSONString)node;
			}

			Debugger.Warning(string.Format("LogicJSONObject::getJSONString type is {0}, key {1}", node.GetJSONNodeType(), key));

			return null;
		}

		public void Put(string key, LogicJSONNode item)
		{
			int keyIndex = m_keys.IndexOf(key);

			if (keyIndex != -1)
			{
				Debugger.Error(string.Format("LogicJSONObject::put already contains key {0}", key));
			}
			else
			{
				int itemIndex = m_items.IndexOf(item);

				if (itemIndex != -1)
				{
					Debugger.Error(string.Format("LogicJSONObject::put already contains the given JSONNode pointer. Key {0}", key));
				}
				else
				{
					m_items.Add(item);
					m_keys.Add(key);
				}
			}
		}

		public void Remove(string key)
		{
			int keyIndex = m_keys.IndexOf(key);

			if (keyIndex != -1)
			{
				m_keys.Remove(keyIndex);
				m_items.Remove(keyIndex);
			}
		}

		public int GetObjectCount()
			=> m_items.Size();

		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.OBJECT;

		public override void WriteToString(StringBuilder builder)
		{
			builder.Append('{');

			for (int i = 0; i < m_items.Size(); i++)
			{
				if (i > 0)
				{
					builder.Append(',');
				}

				LogicJSONParser.WriteString(m_keys[i], builder);
				builder.Append(':');
				m_items[i].WriteToString(builder);
			}

			builder.Append('}');
		}
	}
}