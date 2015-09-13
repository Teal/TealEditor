using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public static class DiffUtility
	{
		public static int GetAddedItems(IList original, IList changed, IList result)
		{
			return DiffUtility.GetAddedItems(original, changed, result, Comparer.Default);
		}

		public static int GetAddedItems(IList original, IList changed, IList result, IComparer comparer)
		{
			int count = 0;
			if (changed != null && result != null)
			{
				if (original == null)
				{
					foreach (object item in changed)
					{
						result.Add(item);
					}
					count = changed.Count;
				}
				else
				{
					foreach (object item in changed)
					{
						if (!DiffUtility.Contains(original, item, comparer))
						{
							result.Add(item);
							count++;
						}
					}
				}
			}
			return count;
		}

		public static int GetRemovedItems(IList original, IList changed, IList result)
		{
			return DiffUtility.GetRemovedItems(original, changed, result, Comparer.Default);
		}

		public static int GetRemovedItems(IList original, IList changed, IList result, IComparer comparer)
		{
			return DiffUtility.GetAddedItems(changed, original, result, comparer);
		}

		private static bool Contains(IList list, object value, IComparer comparer)
		{
			bool result;
			foreach (object item in list)
			{
				if (0 == comparer.Compare(item, value))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static int Compare(IList a, IList b)
		{
			return DiffUtility.Compare(a, b, Comparer.Default);
		}

		public static int Compare<T>(IList<T> a, IList<T> b)
		{
			return DiffUtility.Compare<T>(a, b, Comparer.Default);
		}

		public static int Compare<T>(IList<T> a, IList<T> b, IComparer comparer)
		{
			int result;
			if (a == null || b == null)
			{
				result = 1;
			}
			else if (a.Count != b.Count)
			{
				result = Math.Sign(a.Count - b.Count);
			}
			else
			{
				int limit = (a.Count < b.Count) ? a.Count : b.Count;
				for (int i = 0; i < limit; i++)
				{
					if (a[i] is IComparable && b[i] is IComparable)
					{
						int cmp = comparer.Compare(a[i], b[i]);
						if (cmp != 0)
						{
							result = cmp;
							return result;
						}
					}
				}
				result = a.Count - b.Count;
			}
			return result;
		}

		public static int Compare(IList a, IList b, IComparer comparer)
		{
			int result;
			if (a == null || b == null)
			{
				result = 1;
			}
			else if (a.Count != b.Count)
			{
				result = Math.Sign(a.Count - b.Count);
			}
			else
			{
				int limit = (a.Count < b.Count) ? a.Count : b.Count;
				for (int i = 0; i < limit; i++)
				{
					if (a[i] is IComparable && b[i] is IComparable)
					{
						int cmp = comparer.Compare(a[i], b[i]);
						if (cmp != 0)
						{
							result = cmp;
							return result;
						}
					}
				}
				result = a.Count - b.Count;
			}
			return result;
		}

		public static int Compare(SortedList a, SortedList b)
		{
			return DiffUtility.Compare(a, b, Comparer.Default);
		}

		public static int Compare(SortedList a, SortedList b, IComparer comparer)
		{
			int result;
			if (a == null || b == null)
			{
				result = 1;
			}
			else
			{
				int limit = (a.Count < b.Count) ? a.Count : b.Count;
				for (int i = 0; i < limit; i++)
				{
					int cmp;
					if (0 != (cmp = comparer.Compare(a.GetByIndex(i), b.GetByIndex(i))))
					{
						result = cmp;
						return result;
					}
				}
				result = a.Count - b.Count;
			}
			return result;
		}
	}
}
