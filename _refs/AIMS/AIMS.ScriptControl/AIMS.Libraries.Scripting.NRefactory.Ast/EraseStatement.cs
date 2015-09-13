using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EraseStatement : Statement
	{
		private List<Expression> expressions;

		public List<Expression> Expressions
		{
			get
			{
				return this.expressions;
			}
			set
			{
				this.expressions = (value ?? new List<Expression>());
			}
		}

		public EraseStatement()
		{
			this.expressions = new List<Expression>();
		}

		public EraseStatement(List<Expression> expressions)
		{
			this.Expressions = expressions;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEraseStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[EraseStatement Expressions={0}]", AbstractNode.GetCollectionString(this.Expressions));
		}
	}
}
