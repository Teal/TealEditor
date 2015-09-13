using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class UnsafeStatement : Statement
	{
		private Statement block;

		public Statement Block
		{
			get
			{
				return this.block;
			}
			set
			{
				this.block = (value ?? Statement.Null);
				if (!this.block.IsNull)
				{
					this.block.Parent = this;
				}
			}
		}

		public UnsafeStatement(Statement block)
		{
			this.Block = block;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitUnsafeStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[UnsafeStatement Block={0}]", this.Block);
		}
	}
}
