using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ForeachStatement : StatementWithEmbeddedStatement
	{
		private TypeReference typeReference;

		private string variableName;

		private Expression expression;

		private Expression nextExpression;

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

		public Expression NextExpression
		{
			get
			{
				return this.nextExpression;
			}
			set
			{
				this.nextExpression = (value ?? Expression.Null);
				if (!this.nextExpression.IsNull)
				{
					this.nextExpression.Parent = this;
				}
			}
		}

		public ForeachStatement(TypeReference typeReference, string variableName, Expression expression, Statement embeddedStatement)
		{
			this.TypeReference = typeReference;
			this.VariableName = variableName;
			this.Expression = expression;
			base.EmbeddedStatement = embeddedStatement;
			this.nextExpression = Expression.Null;
		}

		public ForeachStatement(TypeReference typeReference, string variableName, Expression expression, Statement embeddedStatement, Expression nextExpression)
		{
			this.TypeReference = typeReference;
			this.VariableName = variableName;
			this.Expression = expression;
			base.EmbeddedStatement = embeddedStatement;
			this.NextExpression = nextExpression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitForeachStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ForeachStatement TypeReference={0} VariableName={1} Expression={2} NextExpression={3} EmbeddedStatement={4}]", new object[]
			{
				this.TypeReference,
				this.VariableName,
				this.Expression,
				this.NextExpression,
				base.EmbeddedStatement
			});
		}
	}
}
