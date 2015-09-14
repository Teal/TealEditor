using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class SwitchSection : BlockStatement
	{
		private List<CaseLabel> switchLabels;

		public List<CaseLabel> SwitchLabels
		{
			get
			{
				return this.switchLabels;
			}
			set
			{
				this.switchLabels = (value ?? new List<CaseLabel>());
			}
		}

		public SwitchSection()
		{
			this.switchLabels = new List<CaseLabel>();
		}

		public SwitchSection(List<CaseLabel> switchLabels)
		{
			this.SwitchLabels = switchLabels;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitSwitchSection(this, data);
		}

		public override string ToString()
		{
			return string.Format("[SwitchSection SwitchLabels={0}]", AbstractNode.GetCollectionString(this.SwitchLabels));
		}
	}
}
