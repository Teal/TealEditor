using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class YieldStatement : Statement
	{
		private Statement statement;

		public Statement Statement
		{
			get
			{
				return this.statement;
			}
			set
			{
				this.statement = (value ?? Statement.Null);
				if (!this.statement.IsNull)
				{
					this.statement.Parent = this;
				}
			}
		}

		public bool IsYieldReturn
		{
			get
			{
				return this.statement is ReturnStatement;
			}
		}

		public bool IsYieldBreak
		{
			get
			{
				return this.statement is BreakStatement;
			}
		}

		public YieldStatement(Statement statement)
		{
			this.Statement = statement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitYieldStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[YieldStatement Statement={0}]", this.Statement);
		}
	}
}
