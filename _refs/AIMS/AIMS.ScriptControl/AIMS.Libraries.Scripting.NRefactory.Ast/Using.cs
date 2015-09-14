using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class Using : AbstractNode
	{
		private string name;

		private TypeReference alias;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = (string.IsNullOrEmpty(value) ? "?" : value);
			}
		}

		public TypeReference Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = (value ?? TypeReference.Null);
			}
		}

		public bool IsAlias
		{
			get
			{
				return !this.alias.IsNull;
			}
		}

		public Using(string name)
		{
			this.Name = name;
			this.alias = TypeReference.Null;
		}

		public Using(string name, TypeReference alias)
		{
			this.Name = name;
			this.Alias = alias;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitUsing(this, data);
		}

		public override string ToString()
		{
			return string.Format("[Using Name={0} Alias={1}]", this.Name, this.Alias);
		}
	}
}
