using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IUsing
	{
		DomRegion Region
		{
			get;
		}

		List<string> Usings
		{
			get;
		}

		bool HasAliases
		{
			get;
		}

		SortedList<string, IReturnType> Aliases
		{
			get;
		}

		void AddAlias(string alias, IReturnType type);

		IEnumerable<IReturnType> SearchType(string partialTypeName, int typeParameterCount);

		string SearchNamespace(string partialNamespaceName);
	}
}
