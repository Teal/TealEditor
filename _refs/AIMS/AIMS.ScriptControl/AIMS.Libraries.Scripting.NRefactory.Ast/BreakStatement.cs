using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class BreakStatement : Statement
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitBreakStatement(this, data);
		}

		public override string ToString()
		{
			return "[BreakStatement]";
		}
	}
}
