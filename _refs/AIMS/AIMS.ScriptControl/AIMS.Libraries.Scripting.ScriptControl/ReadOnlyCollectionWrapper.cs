using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public sealed class ReadOnlyCollectionWrapper<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly ICollection<T> c;

		public int Count
		{
			get
			{
				return this.c.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public ReadOnlyCollectionWrapper(ICollection<T> c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			this.c = c;
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			return this.c.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.c.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.c.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.c.GetEnumerator();
		}
	}
}
