using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class OnErrorStatement : StatementWithEmbeddedStatement
	{
		public OnErrorStatement(Statement embeddedStatement)
		{
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitOnErrorStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[OnErrorStatement EmbeddedStatement={0}]", base.EmbeddedStatement);
		}
	}
}
