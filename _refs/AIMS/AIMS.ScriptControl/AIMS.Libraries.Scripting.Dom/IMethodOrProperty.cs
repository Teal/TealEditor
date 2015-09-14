using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IMethodOrProperty : IMember, IDecoration, IComparable, ICloneable
	{
		IList<IParameter> Parameters
		{
			get;
		}

		bool IsExtensionMethod
		{
			get;
		}
	}
}
