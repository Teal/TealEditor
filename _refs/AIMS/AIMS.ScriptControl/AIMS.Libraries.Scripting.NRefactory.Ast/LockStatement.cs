using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class LockStatement : StatementWithEmbeddedStatement
	{
		private Expression lockExpression;

		public Expression LockExpression
		{
			get
			{
				return this.lockExpression;
			}
			set
			{
				this.lockExpression = (value ?? Expression.Null);
				if (!this.lockExpression.IsNull)
				{
					this.lockExpression.Parent = this;
				}
			}
		}

		public LockStatement(Expression lockExpression, Statement embeddedStatement)
		{
			this.LockExpression = lockExpression;
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitLockStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[LockStatement LockExpression={0} EmbeddedStatement={1}]", this.LockExpression, base.EmbeddedStatement);
		}
	}
}
