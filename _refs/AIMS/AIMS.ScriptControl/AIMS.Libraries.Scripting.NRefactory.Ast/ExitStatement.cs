using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ExitStatement : Statement
	{
		private ExitType exitType;

		public ExitType ExitType
		{
			get
			{
				return this.exitType;
			}
			set
			{
				this.exitType = value;
			}
		}

		public ExitStatement(ExitType exitType)
		{
			this.ExitType = exitType;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitExitStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ExitStatement ExitType={0}]", this.ExitType);
		}
	}
}
