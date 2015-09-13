using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ContinueStatement : Statement
	{
		private ContinueType continueType;

		public ContinueType ContinueType
		{
			get
			{
				return this.continueType;
			}
			set
			{
				this.continueType = value;
			}
		}

		public ContinueStatement()
		{
		}

		public ContinueStatement(ContinueType continueType)
		{
			this.ContinueType = continueType;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitContinueStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ContinueStatement ContinueType={0}]", this.ContinueType);
		}
	}
}
