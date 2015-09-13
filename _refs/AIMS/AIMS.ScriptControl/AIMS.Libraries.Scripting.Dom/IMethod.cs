using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IMethod : IMethodOrProperty, IMember, IDecoration, IComparable, ICloneable
	{
		IList<ITypeParameter> TypeParameters
		{
			get;
		}

		bool IsConstructor
		{
			get;
		}
	}
}
