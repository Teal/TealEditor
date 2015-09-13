using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class GotoStatement : Statement
	{
		private string label;

		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = (value ?? "");
			}
		}

		public GotoStatement(string label)
		{
			this.Label = label;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitGotoStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[GotoStatement Label={0}]", this.Label);
		}
	}
}
