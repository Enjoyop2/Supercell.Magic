using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Helper
{
	public class ChecksumHelper
	{
		private int m_checksum;
		private LogicArrayList<LogicJSONNode> m_nodes;

		public ChecksumHelper(LogicJSONObject root)
		{
			if (root != null)
			{
				m_nodes = new LogicArrayList<LogicJSONNode>(16);
				m_nodes.Add(root);
			}
		}

		public void StartObject(string name)
		{
			if (m_nodes != null)
			{
				LogicJSONObject jsonObject = new LogicJSONObject();
				LogicJSONNode prevNode = m_nodes[m_nodes.Size() - 1];

				if (prevNode.GetJSONNodeType() == LogicJSONNodeType.OBJECT)
				{
					((LogicJSONObject)prevNode).Put(name, jsonObject);
				}
				else if (prevNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
				{
					((LogicJSONArray)prevNode).Add(jsonObject);
					jsonObject.Put("name", new LogicJSONString(name));
				}

				m_nodes.Add(jsonObject);
			}
		}

		public void EndObject()
		{
			if (m_nodes != null)
			{
				LogicJSONNode prevNode = m_nodes[m_nodes.Size() - 1];

				Debugger.DoAssert(prevNode.GetJSONNodeType() == LogicJSONNodeType.OBJECT, "ChecksumHelper::endObject() called but top is not an object");
				Debugger.DoAssert(m_nodes.Size() > 1, "ChecksumHelper::endObject() - size is too small");

				m_nodes.Remove(m_nodes.Size() - 1);
			}
		}

		public void StartArray(string name)
		{
			if (m_nodes != null)
			{
				LogicJSONNode prevNode = m_nodes[m_nodes.Size() - 1];

				if (prevNode.GetJSONNodeType() == LogicJSONNodeType.OBJECT)
				{
					LogicJSONArray array = new LogicJSONArray();
					((LogicJSONObject)prevNode).Put(name, array);
					m_nodes.Add(array);
				}
				else if (prevNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
				{
					Debugger.DoAssert(((LogicJSONArray)prevNode).Size() != 0, "ChecksumHelper::startArray can't handle the truth (array inside array)");
				}
			}
		}

		public void EndArray()
		{
			if (m_nodes != null)
			{
				LogicJSONNode prevNode = m_nodes[m_nodes.Size() - 1];

				Debugger.DoAssert(prevNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY, "ChecksumHelper::endArray() called but top is not an array");
				Debugger.DoAssert(m_nodes.Size() > 1, "ChecksumHelper::endArray() - size is too small");

				m_nodes.Remove(m_nodes.Size() - 1);
			}
		}

		public void WriteValue(string name, int value)
		{
			m_checksum += value;

			if (m_nodes != null)
			{
				LogicJSONNode prevNode = m_nodes[m_nodes.Size() - 1];

				if (prevNode.GetJSONNodeType() == LogicJSONNodeType.OBJECT)
				{
					((LogicJSONObject)prevNode).Put(name, new LogicJSONNumber(value));
				}
				else if (prevNode.GetJSONNodeType() == LogicJSONNodeType.ARRAY)
				{
					((LogicJSONArray)prevNode).Add(new LogicJSONString(string.Format("{0} {1}", name, value)));
				}
			}
		}

		public int GetChecksum()
			=> m_checksum;

		public void Destruct()
		{
			if (m_nodes != null)
			{
				m_nodes.Destruct();
				m_nodes = null;
			}
		}
	}
}