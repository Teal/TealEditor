using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ForNextStatement : StatementWithEmbeddedStatement
	{
		private Expression start;

		private Expression end;

		private Expression step;

		private List<Expression> nextExpressions;

		private TypeReference typeReference;

		private string variableName;

		public Expression Start
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = (value ?? Expression.Null);
				if (!this.start.IsNull)
				{
					this.start.Parent = this;
				}
			}
		}

		public Expression End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end = (value ?? Expression.Null);
				if (!this.end.IsNull)
				{
					this.end.Parent = this;
				}
			}
		}

		public Expression Step
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = (value ?? Expression.Null);
				if (!this.step.IsNull)
				{
					this.step.Parent = this;
				}
			}
		}

		public List<Expression> NextExpressions
		{
			get
			{
				return this.nextExpressions;
			}
			set
			{
				this.nextExpressions = (value ?? new List<Expression>());
			}
		}

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

		public ForNextStatement(TypeReference typeReference, string variableName, Expression start, Expression end, Expression step, Statement embeddedStatement, List<Expression> nextExpressions)
		{
			this.TypeReference = typeReference;
			this.VariableName = variableName;
			this.Start = start;
			this.End = end;
			this.Step = step;
			base.EmbeddedStatement = embeddedStatement;
			this.NextExpressions = nextExpressions;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitForNextStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ForNextStatement Start={0} End={1} Step={2} NextExpressions={3} TypeReference={4} VariableName={5} EmbeddedStatement={6}]", new object[]
			{
				this.Start,
				this.End,
				this.Step,
				AbstractNode.GetCollectionString(this.NextExpressions),
				this.TypeReference,
				this.VariableName,
				base.EmbeddedStatement
			});
		}
	}
}
