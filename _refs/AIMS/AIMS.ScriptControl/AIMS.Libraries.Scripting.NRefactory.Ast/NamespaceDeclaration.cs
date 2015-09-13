using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NamespaceDeclaration : AbstractNode
	{
		private string name;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = (value ?? "");
			}
		}

		public NamespaceDeclaration(string name)
		{
			this.Name = name;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitNamespaceDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[NamespaceDeclaration Name={0}]", this.Name);
		}
	}
}
