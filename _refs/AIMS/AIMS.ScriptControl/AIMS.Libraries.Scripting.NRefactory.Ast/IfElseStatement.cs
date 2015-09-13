using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class IfElseStatement : Statement
	{
		private Expression condition;

		private List<Statement> trueStatement;

		private List<Statement> falseStatement;

		private List<ElseIfSection> elseIfSections;

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

		public List<Statement> TrueStatement
		{
			get
			{
				return this.trueStatement;
			}
			set
			{
				this.trueStatement = (value ?? new List<Statement>());
			}
		}

		public List<Statement> FalseStatement
		{
			get
			{
				return this.falseStatement;
			}
			set
			{
				this.falseStatement = (value ?? new List<Statement>());
			}
		}

		public List<ElseIfSection> ElseIfSections
		{
			get
			{
				return this.elseIfSections;
			}
			set
			{
				this.elseIfSections = (value ?? new List<ElseIfSection>());
			}
		}

		public bool HasElseStatements
		{
			get
			{
				return this.falseStatement.Count > 0;
			}
		}

		public bool HasElseIfSections
		{
			get
			{
				return this.elseIfSections.Count > 0;
			}
		}

		public IfElseStatement(Expression condition)
		{
			this.Condition = condition;
			this.trueStatement = new List<Statement>();
			this.falseStatement = new List<Statement>();
			this.elseIfSections = new List<ElseIfSection>();
		}

		public IfElseStatement(Expression condition, Statement trueStatement) : this(condition)
		{
			this.trueStatement.Add(Statement.CheckNull(trueStatement));
		}

		public IfElseStatement(Expression condition, Statement trueStatement, Statement falseStatement) : this(condition)
		{
			this.trueStatement.Add(Statement.CheckNull(trueStatement));
			this.falseStatement.Add(Statement.CheckNull(falseStatement));
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitIfElseStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[IfElseStatement Condition={0} TrueStatement={1} FalseStatement={2} ElseIfSections={3}]", new object[]
			{
				this.Condition,
				AbstractNode.GetCollectionString(this.TrueStatement),
				AbstractNode.GetCollectionString(this.FalseStatement),
				AbstractNode.GetCollectionString(this.ElseIfSections)
			});
		}
	}
}
