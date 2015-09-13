using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ThisReferenceExpression : Expression
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitThisReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return "[ThisReferenceExpression]";
		}
	}
}
