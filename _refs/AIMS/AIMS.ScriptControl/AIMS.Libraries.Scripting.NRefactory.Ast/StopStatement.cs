using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class StopStatement : Statement
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitStopStatement(this, data);
		}

		public override string ToString()
		{
			return "[StopStatement]";
		}
	}
}
