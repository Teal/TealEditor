using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ReDimStatement : Statement
	{
		private List<InvocationExpression> reDimClauses;

		private bool isPreserve;

		public List<InvocationExpression> ReDimClauses
		{
			get
			{
				return this.reDimClauses;
			}
			set
			{
				this.reDimClauses = (value ?? new List<InvocationExpression>());
			}
		}

		public bool IsPreserve
		{
			get
			{
				return this.isPreserve;
			}
			set
			{
				this.isPreserve = value;
			}
		}

		public ReDimStatement(bool isPreserve)
		{
			this.IsPreserve = isPreserve;
			this.reDimClauses = new List<InvocationExpression>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitReDimStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ReDimStatement ReDimClauses={0} IsPreserve={1}]", AbstractNode.GetCollectionString(this.ReDimClauses), this.IsPreserve);
		}
	}
}
