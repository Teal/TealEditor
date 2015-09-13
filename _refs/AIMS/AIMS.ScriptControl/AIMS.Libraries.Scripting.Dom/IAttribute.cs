using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IAttribute : IComparable
	{
		AttributeTarget AttributeTarget
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}
