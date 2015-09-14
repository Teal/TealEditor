using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class StackAllocExpression : Expression
	{
		private TypeReference typeReference;

		private Expression expression;

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

		public StackAllocExpression(TypeReference typeReference, Expression expression)
		{
			this.TypeReference = typeReference;
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitStackAllocExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[StackAllocExpression TypeReference={0} Expression={1}]", this.TypeReference, this.Expression);
		}
	}
}
