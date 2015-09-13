using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class StatementWithEmbeddedStatement : Statement
	{
		private Statement embeddedStatement;

		public Statement EmbeddedStatement
		{
			get
			{
				return this.embeddedStatement;
			}
			set
			{
				this.embeddedStatement = Statement.CheckNull(value);
				if (value != null)
				{
					value.Parent = this;
				}
			}
		}
	}
}
