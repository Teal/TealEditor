using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ParenthesizedExpression : Expression
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

		public ParenthesizedExpression(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitParenthesizedExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ParenthesizedExpression Expression={0}]", this.Expression);
		}
	}
}
