using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EndStatement : Statement
	{
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEndStatement(this, data);
		}

		public override string ToString()
		{
			return "[EndStatement]";
		}
	}
}
