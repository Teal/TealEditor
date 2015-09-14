using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class BinaryOperatorExpression : Expression
	{
		private Expression left;

		private BinaryOperatorType op;

		private Expression right;

		public Expression Left
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = (value ?? Expression.Null);
				if (!this.left.IsNull)
				{
					this.left.Parent = this;
				}
			}
		}

		public BinaryOperatorType Op
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

		public Expression Right
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = (value ?? Expression.Null);
				if (!this.right.IsNull)
				{
					this.right.Parent = this;
				}
			}
		}

		public BinaryOperatorExpression(Expression left, BinaryOperatorType op, Expression right)
		{
			this.Left = left;
			this.Op = op;
			this.Right = right;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitBinaryOperatorExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[BinaryOperatorExpression Left={0} Op={1} Right={2}]", this.Left, this.Op, this.Right);
		}
	}
}
