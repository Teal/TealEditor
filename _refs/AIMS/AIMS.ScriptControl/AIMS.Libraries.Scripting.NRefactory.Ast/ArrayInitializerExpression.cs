using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ArrayInitializerExpression : Expression
	{
		private List<Expression> createExpressions;

		public List<Expression> CreateExpressions
		{
			get
			{
				return this.createExpressions;
			}
			set
			{
				this.createExpressions = (value ?? new List<Expression>());
			}
		}

		public new static ArrayInitializerExpression Null
		{
			get
			{
				return NullArrayInitializerExpression.Instance;
			}
		}

		public ArrayInitializerExpression()
		{
			this.createExpressions = new List<Expression>();
		}

		public ArrayInitializerExpression(List<Expression> createExpressions)
		{
			this.CreateExpressions = createExpressions;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitArrayInitializerExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ArrayInitializerExpression CreateExpressions={0}]", AbstractNode.GetCollectionString(this.CreateExpressions));
		}
	}
}
