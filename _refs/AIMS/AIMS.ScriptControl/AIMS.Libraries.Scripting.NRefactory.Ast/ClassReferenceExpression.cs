using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ClassReferenceExpression : Expression
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitClassReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return "[ClassReferenceExpression]";
		}
	}
}
