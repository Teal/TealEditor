using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IProperty : IMethodOrProperty, IMember, IDecoration, IComparable, ICloneable
	{
		DomRegion GetterRegion
		{
			get;
		}

		DomRegion SetterRegion
		{
			get;
		}

		bool CanGet
		{
			get;
		}

		bool CanSet
		{
			get;
		}

		bool IsIndexer
		{
			get;
		}
	}
}
