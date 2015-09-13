using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class GotoCaseStatement : Statement
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

		public bool IsDefaultCase
		{
			get
			{
				return this.expression.IsNull;
			}
		}

		public GotoCaseStatement(Expression expression)
		{
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitGotoCaseStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[GotoCaseStatement Expression={0}]", this.Expression);
		}
	}
}
