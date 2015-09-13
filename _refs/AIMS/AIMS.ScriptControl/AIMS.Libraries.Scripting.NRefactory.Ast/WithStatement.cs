using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class WithStatement : Statement
	{
		private Expression expression;

		private BlockStatement body;

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

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = (value ?? BlockStatement.Null);
				if (!this.body.IsNull)
				{
					this.body.Parent = this;
				}
			}
		}

		public WithStatement(Expression expression)
		{
			this.Expression = expression;
			this.body = BlockStatement.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitWithStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[WithStatement Expression={0} Body={1}]", this.Expression, this.Body);
		}
	}
}
