using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public interface ISpecial
	{
		Location StartPosition
		{
			get;
		}

		Location EndPosition
		{
			get;
		}

		object AcceptVisitor(ISpecialVisitor visitor, object data);
	}
}
