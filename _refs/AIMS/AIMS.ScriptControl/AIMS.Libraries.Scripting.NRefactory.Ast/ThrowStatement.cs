using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ThrowStatement : Statement
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

		public ThrowStatement(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitThrowStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ThrowStatement Expression={0}]", this.Expression);
		}
	}
}
