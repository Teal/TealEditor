using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class CaseLabel : AbstractNode
	{
		private Expression label;

		private BinaryOperatorType binaryOperatorType;

		private Expression toExpression;

		public Expression Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = (value ?? Expression.Null);
				if (!this.label.IsNull)
				{
					this.label.Parent = this;
				}
			}
		}

		public BinaryOperatorType BinaryOperatorType
		{
			get
			{
				return this.binaryOperatorType;
			}
			set
			{
				this.binaryOperatorType = value;
			}
		}

		public Expression ToExpression
		{
			get
			{
				return this.toExpression;
			}
			set
			{
				this.toExpression = (value ?? Expression.Null);
				if (!this.toExpression.IsNull)
				{
					this.toExpression.Parent = this;
				}
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.label.IsNull;
			}
		}

		public CaseLabel()
		{
			this.label = Expression.Null;
			this.toExpression = Expression.Null;
		}

		public CaseLabel(Expression label)
		{
			this.Label = label;
			this.toExpression = Expression.Null;
		}

		public CaseLabel(Expression label, Expression toExpression)
		{
			this.Label = label;
			this.ToExpression = toExpression;
		}

		public CaseLabel(BinaryOperatorType binaryOperatorType, Expression label)
		{
			this.BinaryOperatorType = binaryOperatorType;
			this.Label = label;
			this.toExpression = Expression.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCaseLabel(this, data);
		}

		public override string ToString()
		{
			return string.Format("[CaseLabel Label={0} BinaryOperatorType={1} ToExpression={2}]", this.Label, this.BinaryOperatorType, this.ToExpression);
		}
	}
}
