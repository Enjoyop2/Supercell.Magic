using System.Text;

using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.Json
{
	public class LogicJSONArray : LogicJSONNode
	{
		private readonly LogicArrayList<LogicJSONNode> m_items;

		public LogicJSONArray()
		{
			m_items = new LogicArrayList<LogicJSONNode>(20);
		}

		public LogicJSONArray(int capacity)
		{
			m_items = new LogicArrayList<LogicJSONNode>(capacity);
		}

		public LogicJSONNode Get(int idx)
			=> m_items[idx];

		public void Add(LogicJSONNode item)
		{
			m_items.Add(item);
		}

		public LogicJSONArray GetJSONArray(int index)
		{
			LogicJSONNode node = m_items[index];

			if (node.GetJSONNodeType() != LogicJSONNodeType.ARRAY)
			{
				Debugger.Warning(string.Format("LogicJSONObject::getJSONArray wrong type {0}, index {1}", node.GetJSONNodeType(), index));
				return null;
			}

			return (LogicJSONArray)node;
		}

		public LogicJSONBoolean GetJSONBoolean(int index)
		{
			LogicJSONNode node = m_items[index];

			if (node.GetJSONNodeType() != LogicJSONNodeType.BOOLEAN)
			{
				Debugger.Warning(string.Format("LogicJSONObject::getJSONBoolean wrong type {0}, index {1}", node.GetJSONNodeType(), index));
				return null;
			}

			return (LogicJSONBoolean)node;
		}

		public LogicJSONNumber GetJSONNumber(int index)
		{
			LogicJSONNode node = m_items[index];

			if (node.GetJSONNodeType() != LogicJSONNodeType.NUMBER)
			{
				Debugger.Warning(string.Format("LogicJSONObject::getJSONNumber wrong type {0}, index {1}", node.GetJSONNodeType(), index));
				return null;
			}

			return (LogicJSONNumber)node;
		}

		public LogicJSONObject GetJSONObject(int index)
		{
			LogicJSONNode node = m_items[index];

			if (node.GetJSONNodeType() != LogicJSONNodeType.OBJECT)
			{
				Debugger.Warning("LogicJSONObject::getJSONObject wrong type " + node.GetJSONNodeType() + ", index " + index);
				return null;
			}

			return (LogicJSONObject)node;
		}

		public LogicJSONString GetJSONString(int index)
		{
			LogicJSONNode node = m_items[index];

			if (node.GetJSONNodeType() != LogicJSONNodeType.STRING)
			{
				Debugger.Warning(string.Format("LogicJSONObject::getJSONString wrong type {0}, index {1}", node.GetJSONNodeType(), index));
				return null;
			}

			return (LogicJSONString)node;
		}

		public int Size()
			=> m_items.Size();

		public override LogicJSONNodeType GetJSONNodeType()
			=> LogicJSONNodeType.ARRAY;

		public override void WriteToString(StringBuilder builder)
		{
			builder.Append('[');

			for (int i = 0; i < m_items.Size(); i++)
			{
				if (i > 0)
				{
					builder.Append(',');
				}

				m_items[i].WriteToString(builder);
			}

			builder.Append(']');
		}
	}
}