using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ErrorStatement : Statement
	{
		private Expression expression;

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = (value ?? Expression.Null);
				if (!this.expression.IsNull)
				{
					this.expression.Parent = this;
				}
			}
		}

		public ErrorStatement(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitErrorStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ErrorStatement Expression={0}]", this.Expression);
		}
	}
}
