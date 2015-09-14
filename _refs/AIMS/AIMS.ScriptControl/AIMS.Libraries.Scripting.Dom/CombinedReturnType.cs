using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class CombinedReturnType : AbstractReturnType
	{
		private IList<IReturnType> baseTypes;

		private string fullName;

		private string name;

		private string @namespace;

		private string dotnetName;

		[CompilerGenerated]
		private static Converter<IReturnType, List<IMethod>> <>9__CachedAnonymousMethodDelegate1;

		[CompilerGenerated]
		private static Converter<IReturnType, List<IProperty>> <>9__CachedAnonymousMethodDelegate3;

		[CompilerGenerated]
		private static Converter<IReturnType, List<IField>> <>9__CachedAnonymousMethodDelegate5;

		[CompilerGenerated]
		private static Converter<IReturnType, List<IEvent>> <>9__CachedAnonymousMethodDelegate7;

		public IList<IReturnType> BaseTypes
		{
			get
			{
				return this.baseTypes;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.fullName;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.@namespace;
			}
		}

		public override string DotNetName
		{
			get
			{
				return this.dotnetName;
			}
		}

		public override bool IsDefaultReturnType
		{
			get
			{
				return false;
			}
		}

		public override int TypeParameterCount
		{
			get
			{
				return 0;
			}
		}

		public override bool Equals(object obj)
		{
			CombinedReturnType combined = obj as CombinedReturnType;
			bool result;
			if (combined == null)
			{
				result = false;
			}
			else if (this.baseTypes.Count != combined.baseTypes.Count)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.baseTypes.Count; i++)
				{
					if (!this.baseTypes[i].Equals(combined.baseTypes[i]))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		public override int GetHashCode()
		{
			int res = 0;
			foreach (IReturnType rt in this.baseTypes)
			{
				res *= 27;
				res += rt.GetHashCode();
			}
			return res;
		}

		public CombinedReturnType(IList<IReturnType> baseTypes, string fullName, string name, string @namespace, string dotnetName)
		{
			this.baseTypes = baseTypes;
			this.fullName = fullName;
			this.name = name;
			this.@namespace = @namespace;
			this.dotnetName = dotnetName;
		}

		private List<T> Combine<T>(Converter<IReturnType, List<T>> conv) where T : IMember
		{
			int count = this.baseTypes.Count;
			List<T> result;
			if (count == 0)
			{
				result = null;
			}
			else
			{
				List<T> list = null;
				foreach (IReturnType baseType in this.baseTypes)
				{
					List<T> newList = conv(baseType);
					if (newList != null)
					{
						if (list == null)
						{
							list = newList;
						}
						else
						{
							foreach (T element in newList)
							{
								bool found = false;
								foreach (T t in list)
								{
									if (t.CompareTo(element) == 0)
									{
										found = true;
										break;
									}
								}
								if (!found)
								{
									list.Add(element);
								}
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public override List<IMethod> GetMethods()
		{
			return this.Combine<IMethod>((IReturnType type) => type.GetMethods());
		}

		public override List<IProperty> GetProperties()
		{
			return this.Combine<IProperty>((IReturnType type) => type.GetProperties());
		}

		public override List<IField> GetFields()
		{
			return this.Combine<IField>((IReturnType type) => type.GetFields());
		}

		public override List<IEvent> GetEvents()
		{
			return this.Combine<IEvent>((IReturnType type) => type.GetEvents());
		}

		public override IClass GetUnderlyingClass()
		{
			return null;
		}
	}
}
