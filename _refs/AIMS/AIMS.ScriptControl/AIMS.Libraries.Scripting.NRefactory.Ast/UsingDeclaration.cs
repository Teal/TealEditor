using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class UsingDeclaration : AbstractNode
	{
		private List<Using> usings;

		public List<Using> Usings
		{
			get
			{
				return this.usings;
			}
			set
			{
				this.usings = (value ?? new List<Using>());
			}
		}

		public UsingDeclaration(List<Using> usings)
		{
			this.Usings = usings;
		}

		public UsingDeclaration(string @namespace, TypeReference alias)
		{
			this.usings = new List<Using>(1);
			this.usings.Add(new Using(@namespace, alias));
		}

		public UsingDeclaration(string @namespace) : this(@namespace, null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitUsingDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[UsingDeclaration Usings={0}]", AbstractNode.GetCollectionString(this.Usings));
		}
	}
}
