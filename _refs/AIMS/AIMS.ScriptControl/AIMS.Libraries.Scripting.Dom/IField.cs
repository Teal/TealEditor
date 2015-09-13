using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IField : IMember, IDecoration, IComparable, ICloneable
	{
		bool IsLocalVariable
		{
			get;
		}

		bool IsParameter
		{
			get;
		}
	}
}
