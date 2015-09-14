using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ElseIfSection : StatementWithEmbeddedStatement
	{
		private Expression condition;

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

		public ElseIfSection(Expression condition, Statement embeddedStatement)
		{
			this.Condition = condition;
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitElseIfSection(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ElseIfSection Condition={0} EmbeddedStatement={1}]", this.Condition, base.EmbeddedStatement);
		}
	}
}
