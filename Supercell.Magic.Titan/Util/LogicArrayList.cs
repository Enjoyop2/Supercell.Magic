using System;

namespace Supercell.Magic.Titan.Util
{
	public class LogicArrayList<T>
	{
		private T[] m_items;
		private int m_size;

		public LogicArrayList()
		{
			m_items = new T[0];
		}

		public LogicArrayList(int initialCapacity)
		{
			m_items = new T[initialCapacity];
		}

		public T this[int index]
		{
			get
			{
				return m_items[index];
			}
			set
			{
				m_items[index] = value;
			}
		}

		public void Add(T item)
		{
			int size = m_items.Length;

			if (size == m_size)
			{
				EnsureCapacity(size != 0 ? size * 2 : 5);
			}

			m_items[m_size++] = item;
		}

		public void Add(int index, T item)
		{
			int size = m_items.Length;

			if (size == m_size)
			{
				EnsureCapacity(size != 0 ? size * 2 : 5);
			}

			if (m_size > index)
			{
				Array.Copy(m_items, index, m_items, index + 1, m_size - index);
			}

			m_items[index] = item;
			m_size += 1;
		}

		public void AddAll(LogicArrayList<T> array)
		{
			EnsureCapacity(m_size + array.m_size);

			for (int i = 0, cnt = array.m_size; i < cnt; i++)
			{
				m_items[m_size++] = array[i];
			}
		}

		public int IndexOf(T item)
			=> Array.IndexOf(m_items, item, 0, m_size);

		public void Remove(int index)
		{
			if ((uint)index < m_size)
			{
				m_size -= 1;

				if (index != m_size)
				{
					Array.Copy(m_items, index + 1, m_items, index, m_size - index);
				}
			}
		}

		public void EnsureCapacity(int count)
		{
			int size = m_items.Length;

			if (size < count)
			{
				Array.Resize(ref m_items, count);
			}
		}

		public int Size()
			=> m_size;

		public void Clear()
		{
			m_size = 0;
		}

		public void Destruct()
		{
			m_items = null;
			m_size = 0;
		}
	}
}