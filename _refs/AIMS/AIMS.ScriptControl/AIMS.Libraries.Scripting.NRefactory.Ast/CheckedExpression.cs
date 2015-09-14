using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class CheckedExpression : Expression
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

		public CheckedExpression(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCheckedExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[CheckedExpression Expression={0}]", this.Expression);
		}
	}
}
