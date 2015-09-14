using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ForStatement : StatementWithEmbeddedStatement
	{
		private List<Statement> initializers;

		private Expression condition;

		private List<Statement> iterator;

		public List<Statement> Initializers
		{
			get
			{
				return this.initializers;
			}
			set
			{
				this.initializers = (value ?? new List<Statement>());
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

		public List<Statement> Iterator
		{
			get
			{
				return this.iterator;
			}
			set
			{
				this.iterator = (value ?? new List<Statement>());
			}
		}

		public ForStatement(List<Statement> initializers, Expression condition, List<Statement> iterator, Statement embeddedStatement)
		{
			this.Initializers = initializers;
			this.Condition = condition;
			this.Iterator = iterator;
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitForStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ForStatement Initializers={0} Condition={1} Iterator={2} EmbeddedStatement={3}]", new object[]
			{
				AbstractNode.GetCollectionString(this.Initializers),
				this.Condition,
				AbstractNode.GetCollectionString(this.Iterator),
				base.EmbeddedStatement
			});
		}
	}
}
