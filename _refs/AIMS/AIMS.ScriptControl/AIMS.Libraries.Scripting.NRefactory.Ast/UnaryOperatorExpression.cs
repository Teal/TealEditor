using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class UnaryOperatorExpression : Expression
	{
		private UnaryOperatorType op;

		private Expression expression;

		public UnaryOperatorType Op
		{
			get
			{
				return this.op;
			}
			set
			{
				this.op = value;
			}
		}

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

		public UnaryOperatorExpression(UnaryOperatorType op)
		{
			this.Op = op;
			this.expression = Expression.Null;
		}

		public UnaryOperatorExpression(Expression expression, UnaryOperatorType op)
		{
			this.Expression = expression;
			this.Op = op;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitUnaryOperatorExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[UnaryOperatorExpression Op={0} Expression={1}]", this.Op, this.Expression);
		}
	}
}
