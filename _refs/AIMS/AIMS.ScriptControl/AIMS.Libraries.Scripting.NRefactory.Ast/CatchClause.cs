using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class CatchClause : AbstractNode
	{
		private TypeReference typeReference;

		private string variableName;

		private Statement statementBlock;

		private Expression condition;

		public TypeReference TypeReference
		{
			get
			{
				return this.typeReference;
			}
			set
			{
				this.typeReference = (value ?? TypeReference.Null);
			}
		}

		public string VariableName
		{
			get
			{
				return this.variableName;
			}
			set
			{
				this.variableName = (value ?? "");
			}
		}

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

		public CatchClause(TypeReference typeReference, string variableName, Statement statementBlock)
		{
			this.TypeReference = typeReference;
			this.VariableName = variableName;
			this.StatementBlock = statementBlock;
			this.condition = Expression.Null;
		}

		public CatchClause(TypeReference typeReference, string variableName, Statement statementBlock, Expression condition)
		{
			this.TypeReference = typeReference;
			this.VariableName = variableName;
			this.StatementBlock = statementBlock;
			this.Condition = condition;
		}

		public CatchClause(Statement statementBlock)
		{
			this.StatementBlock = statementBlock;
			this.typeReference = TypeReference.Null;
			this.variableName = "";
			this.condition = Expression.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCatchClause(this, data);
		}

		public override string ToString()
		{
			return string.Format("[CatchClause TypeReference={0} VariableName={1} StatementBlock={2} Condition={3}]", new object[]
			{
				this.TypeReference,
				this.VariableName,
				this.StatementBlock,
				this.Condition
			});
		}
	}
}
