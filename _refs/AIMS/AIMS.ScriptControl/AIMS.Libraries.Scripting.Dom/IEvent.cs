using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IEvent : IMember, IDecoration, IComparable, ICloneable
	{
		IMethod AddMethod
		{
			get;
		}

		IMethod RemoveMethod
		{
			get;
		}

		IMethod RaiseMethod
		{
			get;
		}
	}
}
