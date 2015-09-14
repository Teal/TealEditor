using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class LabelStatement : Statement
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

		public LabelStatement(string label)
		{
			this.Label = label;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitLabelStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[LabelStatement Label={0}]", this.Label);
		}
	}
}
