using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class BaseReferenceExpression : Expression
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitBaseReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return "[BaseReferenceExpression]";
		}
	}
}
