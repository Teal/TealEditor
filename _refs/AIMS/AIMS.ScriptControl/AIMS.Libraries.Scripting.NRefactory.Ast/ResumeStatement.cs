using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ResumeStatement : Statement
	{
		private string labelName;

		private bool isResumeNext;

		public string LabelName
		{
			get
			{
				return this.labelName;
			}
			set
			{
				this.labelName = (value ?? "");
			}
		}

		public bool IsResumeNext
		{
			get
			{
				return this.isResumeNext;
			}
			set
			{
				this.isResumeNext = value;
			}
		}

		public ResumeStatement(bool isResumeNext)
		{
			this.IsResumeNext = isResumeNext;
			this.labelName = "";
		}

		public ResumeStatement(string labelName)
		{
			this.LabelName = labelName;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitResumeStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ResumeStatement LabelName={0} IsResumeNext={1}]", this.LabelName, this.IsResumeNext);
		}
	}
}
