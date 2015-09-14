using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class UsingStatement : StatementWithEmbeddedStatement
	{
		private Statement resourceAcquisition;

		public Statement ResourceAcquisition
		{
			get
			{
				return this.resourceAcquisition;
			}
			set
			{
				this.resourceAcquisition = (value ?? Statement.Null);
				if (!this.resourceAcquisition.IsNull)
				{
					this.resourceAcquisition.Parent = this;
				}
			}
		}

		public UsingStatement(Statement resourceAcquisition, Statement embeddedStatement)
		{
			this.ResourceAcquisition = resourceAcquisition;
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitUsingStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[UsingStatement ResourceAcquisition={0} EmbeddedStatement={1}]", this.ResourceAcquisition, base.EmbeddedStatement);
		}
	}
}
