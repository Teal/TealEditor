using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface ITypeParameter
	{
		string Name
		{
			get;
		}

		int Index
		{
			get;
		}

		IList<IAttribute> Attributes
		{
			get;
		}

		IMethod Method
		{
			get;
		}

		IClass Class
		{
			get;
		}

		IList<IReturnType> Constraints
		{
			get;
		}

		bool HasConstructableConstraint
		{
			get;
		}

		bool HasReferenceTypeConstraint
		{
			get;
		}

		bool HasValueTypeConstraint
		{
			get;
		}
	}
}
