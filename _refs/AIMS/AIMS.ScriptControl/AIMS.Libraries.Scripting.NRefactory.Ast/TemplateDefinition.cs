using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TemplateDefinition : AttributedNode
	{
		private string name;

		private List<TypeReference> bases;

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

		public List<TypeReference> Bases
		{
			get
			{
				return this.bases;
			}
			set
			{
				this.bases = (value ?? new List<TypeReference>());
			}
		}

		public TemplateDefinition(string name, List<AttributeSection> attributes) : base(attributes)
		{
			this.Name = name;
			this.bases = new List<TypeReference>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTemplateDefinition(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TemplateDefinition Name={0} Bases={1} Attributes={2} Modifier={3}]", new object[]
			{
				this.Name,
				AbstractNode.GetCollectionString(this.Bases),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
