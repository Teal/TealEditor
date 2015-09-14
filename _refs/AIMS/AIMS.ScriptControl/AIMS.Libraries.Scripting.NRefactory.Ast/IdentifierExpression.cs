using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class IdentifierExpression : Expression
	{
		private string identifier;

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = (value ?? "");
			}
		}

		public IdentifierExpression(string identifier)
		{
			this.Identifier = identifier;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitIdentifierExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[IdentifierExpression Identifier={0}]", this.Identifier);
		}
	}
}
