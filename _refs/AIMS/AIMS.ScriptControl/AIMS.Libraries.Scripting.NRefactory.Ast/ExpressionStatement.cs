using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ExpressionStatement : Statement
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

		public ExpressionStatement(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitExpressionStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ExpressionStatement Expression={0}]", this.Expression);
		}
	}
}
