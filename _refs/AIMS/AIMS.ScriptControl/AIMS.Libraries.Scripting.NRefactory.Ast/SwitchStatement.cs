using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class SwitchStatement : Statement
	{
		private Expression switchExpression;

		private List<SwitchSection> switchSections;

		public Expression SwitchExpression
		{
			get
			{
				return this.switchExpression;
			}
			set
			{
				this.switchExpression = (value ?? Expression.Null);
				if (!this.switchExpression.IsNull)
				{
					this.switchExpression.Parent = this;
				}
			}
		}

		public List<SwitchSection> SwitchSections
		{
			get
			{
				return this.switchSections;
			}
			set
			{
				this.switchSections = (value ?? new List<SwitchSection>());
			}
		}

		public SwitchStatement(Expression switchExpression, List<SwitchSection> switchSections)
		{
			this.SwitchExpression = switchExpression;
			this.SwitchSections = switchSections;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitSwitchStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[SwitchStatement SwitchExpression={0} SwitchSections={1}]", this.SwitchExpression, AbstractNode.GetCollectionString(this.SwitchSections));
		}
	}
}
