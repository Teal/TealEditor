using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EmptyStatement : Statement
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEmptyStatement(this, data);
		}

		public override string ToString()
		{
			return "[EmptyStatement]";
		}
	}
}
