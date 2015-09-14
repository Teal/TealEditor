using System;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class DomAssemblyName
	{
		private readonly string fullAssemblyName;

		public string FullName
		{
			get
			{
				return this.fullAssemblyName;
			}
		}

		public DomAssemblyName(string fullAssemblyName)
		{
			this.fullAssemblyName = fullAssemblyName;
		}

		internal static DomAssemblyName[] Convert(AssemblyName[] names)
		{
			DomAssemblyName[] result;
			if (names == null)
			{
				result = null;
			}
			else
			{
				DomAssemblyName[] i = new DomAssemblyName[names.Length];
				for (int j = 0; j < names.Length; j++)
				{
					i[j] = new DomAssemblyName(names[j].FullName);
				}
				result = i;
			}
			return result;
		}
	}
}
