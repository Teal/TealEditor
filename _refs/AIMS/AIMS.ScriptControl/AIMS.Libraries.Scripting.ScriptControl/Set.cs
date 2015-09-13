using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public sealed class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private SortedDictionary<T, object> dict;

		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public Set()
		{
			this.dict = new SortedDictionary<T, object>();
		}

		public Set(IEnumerable<T> list) : this()
		{
			this.AddRange(list);
		}

		public Set(params T[] list) : this()
		{
			this.AddRange(list);
		}

		public Set(IComparer<T> comparer)
		{
			this.dict = new SortedDictionary<T, object>(comparer);
		}

		public Set(IEnumerable<T> list, IComparer<T> comparer) : this(comparer)
		{
			this.AddRange(list);
		}

		public void Add(T element)
		{
			this.dict[element] = null;
		}

		public void AddRange(IEnumerable<T> elements)
		{
			foreach (T element in elements)
			{
				this.Add(element);
			}
		}

		public bool Contains(T element)
		{
			return this.dict.ContainsKey(element);
		}

		public bool Remove(T element)
		{
			return this.dict.Remove(element);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.dict.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			this.dict.Clear();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.dict.Keys.CopyTo(array, arrayIndex);
		}

		public T[] ToArray()
		{
			T[] arr = new T[this.dict.Count];
			this.dict.Keys.CopyTo(arr, 0);
			return arr;
		}

		public ReadOnlyCollectionWrapper<T> AsReadOnly()
		{
			return new ReadOnlyCollectionWrapper<T>(this.dict.Keys);
		}
	}
}
