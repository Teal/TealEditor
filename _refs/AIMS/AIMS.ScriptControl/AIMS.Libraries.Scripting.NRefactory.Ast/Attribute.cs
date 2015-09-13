using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class Attribute : AbstractNode
	{
		private string name;

		private List<Expression> positionalArguments;

		private List<NamedArgumentExpression> namedArguments;

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

		public List<Expression> PositionalArguments
		{
			get
			{
				return this.positionalArguments;
			}
			set
			{
				this.positionalArguments = (value ?? new List<Expression>());
			}
		}

		public List<NamedArgumentExpression> NamedArguments
		{
			get
			{
				return this.namedArguments;
			}
			set
			{
				this.namedArguments = (value ?? new List<NamedArgumentExpression>());
			}
		}

		public Attribute(string name, List<Expression> positionalArguments, List<NamedArgumentExpression> namedArguments)
		{
			this.Name = name;
			this.PositionalArguments = positionalArguments;
			this.NamedArguments = namedArguments;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitAttribute(this, data);
		}

		public override string ToString()
		{
			return string.Format("[Attribute Name={0} PositionalArguments={1} NamedArguments={2}]", this.Name, AbstractNode.GetCollectionString(this.PositionalArguments), AbstractNode.GetCollectionString(this.NamedArguments));
		}
	}
}
