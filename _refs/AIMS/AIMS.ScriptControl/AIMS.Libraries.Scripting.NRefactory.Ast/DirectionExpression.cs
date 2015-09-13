using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DirectionExpression : Expression
	{
		private FieldDirection fieldDirection;

		private Expression expression;

		public FieldDirection FieldDirection
		{
			get
			{
				return this.fieldDirection;
			}
			set
			{
				this.fieldDirection = value;
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

		public DirectionExpression(FieldDirection fieldDirection, Expression expression)
		{
			this.FieldDirection = fieldDirection;
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDirectionExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DirectionExpression FieldDirection={0} Expression={1}]", this.FieldDirection, this.Expression);
		}
	}
}
