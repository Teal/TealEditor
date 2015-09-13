using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TypeOfIsExpression : Expression
	{
		private Expression expression;

		private TypeReference typeReference;

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

		public TypeOfIsExpression(Expression expression, TypeReference typeReference)
		{
			this.Expression = expression;
			this.TypeReference = typeReference;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTypeOfIsExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TypeOfIsExpression Expression={0} TypeReference={1}]", this.Expression, this.TypeReference);
		}
	}
}
