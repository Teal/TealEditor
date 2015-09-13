using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ConditionalExpression : Expression
	{
		private Expression condition;

		private Expression trueExpression;

		private Expression falseExpression;

		public Expression Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = (value ?? Expression.Null);
				if (!this.condition.IsNull)
				{
					this.condition.Parent = this;
				}
			}
		}

		public Expression TrueExpression
		{
			get
			{
				return this.trueExpression;
			}
			set
			{
				this.trueExpression = (value ?? Expression.Null);
				if (!this.trueExpression.IsNull)
				{
					this.trueExpression.Parent = this;
				}
			}
		}

		public Expression FalseExpression
		{
			get
			{
				return this.falseExpression;
			}
			set
			{
				this.falseExpression = (value ?? Expression.Null);
				if (!this.falseExpression.IsNull)
				{
					this.falseExpression.Parent = this;
				}
			}
		}

		public ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		{
			this.Condition = condition;
			this.TrueExpression = trueExpression;
			this.FalseExpression = falseExpression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitConditionalExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ConditionalExpression Condition={0} TrueExpression={1} FalseExpression={2}]", this.Condition, this.TrueExpression, this.FalseExpression);
		}
	}
}
