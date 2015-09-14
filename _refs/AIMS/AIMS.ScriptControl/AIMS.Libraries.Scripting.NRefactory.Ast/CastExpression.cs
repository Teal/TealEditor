using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class CastExpression : Expression
	{
		private TypeReference castTo;

		private Expression expression;

		private CastType castType;

		public TypeReference CastTo
		{
			get
			{
				return this.castTo;
			}
			set
			{
				this.castTo = (value ?? TypeReference.Null);
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

		public CastType CastType
		{
			get
			{
				return this.castType;
			}
			set
			{
				this.castType = value;
			}
		}

		public CastExpression(TypeReference castTo)
		{
			this.CastTo = castTo;
			this.expression = Expression.Null;
		}

		public CastExpression(TypeReference castTo, Expression expression, CastType castType)
		{
			this.CastTo = castTo;
			this.Expression = expression;
			this.CastType = castType;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCastExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[CastExpression CastTo={0} Expression={1} CastType={2}]", this.CastTo, this.Expression, this.CastType);
		}
	}
}
