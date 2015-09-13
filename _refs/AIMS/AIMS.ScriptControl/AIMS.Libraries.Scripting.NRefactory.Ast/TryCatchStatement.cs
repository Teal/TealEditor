using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TryCatchStatement : Statement
	{
		private Statement statementBlock;

		private List<CatchClause> catchClauses;

		private Statement finallyBlock;

		public Statement StatementBlock
		{
			get
			{
				return this.statementBlock;
			}
			set
			{
				this.statementBlock = (value ?? Statement.Null);
				if (!this.statementBlock.IsNull)
				{
					this.statementBlock.Parent = this;
				}
			}
		}

		public List<CatchClause> CatchClauses
		{
			get
			{
				return this.catchClauses;
			}
			set
			{
				this.catchClauses = (value ?? new List<CatchClause>());
			}
		}

		public Statement FinallyBlock
		{
			get
			{
				return this.finallyBlock;
			}
			set
			{
				this.finallyBlock = (value ?? Statement.Null);
				if (!this.finallyBlock.IsNull)
				{
					this.finallyBlock.Parent = this;
				}
			}
		}

		public TryCatchStatement(Statement statementBlock, List<CatchClause> catchClauses, Statement finallyBlock)
		{
			this.StatementBlock = statementBlock;
			this.CatchClauses = catchClauses;
			this.FinallyBlock = finallyBlock;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTryCatchStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TryCatchStatement StatementBlock={0} CatchClauses={1} FinallyBlock={2}]", this.StatementBlock, AbstractNode.GetCollectionString(this.CatchClauses), this.FinallyBlock);
		}
	}
}
