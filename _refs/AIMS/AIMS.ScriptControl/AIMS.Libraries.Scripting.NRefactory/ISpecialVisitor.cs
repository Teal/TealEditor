using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public interface ISpecialVisitor
	{
		object Visit(ISpecial special, object data);

		object Visit(BlankLine special, object data);

		object Visit(Comment special, object data);

		object Visit(PreprocessingDirective special, object data);
	}
}
