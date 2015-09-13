using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class SearchClassReturnType : ProxyReturnType
	{
		private class ReferenceComparer : IEqualityComparer<SearchClassReturnType>
		{
			public bool Equals(SearchClassReturnType x, SearchClassReturnType y)
			{
				return x == y;
			}

			public int GetHashCode(SearchClassReturnType obj)
			{
				return obj.GetHashCode();
			}
		}

		private IClass declaringClass;

		private IProjectContent pc;

		private int caretLine;

		private int caretColumn;

		private string name;

		private string shortName;

		private int typeParameterCount;

		private static Dictionary<SearchClassReturnType, IReturnType> cache = new Dictionary<SearchClassReturnType, IReturnType>(new SearchClassReturnType.ReferenceComparer());

		private bool isSearching;

		public override int TypeParameterCount
		{
			get
			{
				return this.typeParameterCount;
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				IReturnType result;
				if (this.isSearching)
				{
					result = null;
				}
				else
				{
					lock (SearchClassReturnType.cache)
					{
						IReturnType type;
						if (SearchClassReturnType.cache.TryGetValue(this, out type))
						{
							result = type;
							return result;
						}
					}
					try
					{
						this.isSearching = true;
						IReturnType type = this.pc.SearchType(new SearchTypeRequest(this.name, this.typeParameterCount, this.declaringClass, this.caretLine, this.caretColumn)).Result;
						lock (SearchClassReturnType.cache)
						{
							SearchClassReturnType.cache[this] = type;
						}
						result = type;
					}
					finally
					{
						this.isSearching = false;
					}
				}
				return result;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				string tmp = base.FullyQualifiedName;
				string result;
				if (tmp == "?")
				{
					result = this.name;
				}
				else
				{
					result = tmp;
				}
				return result;
			}
		}

		public override string Name
		{
			get
			{
				return this.shortName;
			}
		}

		public override string DotNetName
		{
			get
			{
				string tmp = base.DotNetName;
				string result;
				if (tmp == "?")
				{
					result = this.name;
				}
				else
				{
					result = tmp;
				}
				return result;
			}
		}

		public override bool IsDefaultReturnType
		{
			get
			{
				return true;
			}
		}

		public SearchClassReturnType(IProjectContent projectContent, IClass declaringClass, int caretLine, int caretColumn, string name, int typeParameterCount)
		{
			if (declaringClass == null)
			{
				throw new ArgumentNullException("declaringClass");
			}
			this.declaringClass = declaringClass;
			this.pc = projectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.typeParameterCount = typeParameterCount;
			this.name = name;
			int pos = name.LastIndexOf('.');
			if (pos < 0)
			{
				this.shortName = name;
			}
			else
			{
				this.shortName = name.Substring(pos + 1);
			}
		}

		public override bool Equals(object o)
		{
			IReturnType rt2 = o as IReturnType;
			return rt2 != null && rt2.IsDefaultReturnType && DefaultReturnType.Equals(this, rt2);
		}

		public override int GetHashCode()
		{
			return this.declaringClass.GetHashCode() ^ this.name.GetHashCode() ^ this.typeParameterCount << 16 + this.caretLine << 8 + this.caretColumn;
		}

		internal static void ClearCache()
		{
			lock (SearchClassReturnType.cache)
			{
				SearchClassReturnType.cache.Clear();
			}
		}

		public override string ToString()
		{
			return string.Format("[SearchClassReturnType: {0}]", this.name);
		}
	}
}
