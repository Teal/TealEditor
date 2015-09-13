using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TypeDeclaration : AttributedNode
	{
		private string name;

		private ClassType type;

		private List<TypeReference> baseTypes;

		private List<TemplateDefinition> templates;

		private Location bodyStartLocation;

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

		public ClassType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public List<TypeReference> BaseTypes
		{
			get
			{
				return this.baseTypes;
			}
			set
			{
				this.baseTypes = (value ?? new List<TypeReference>());
			}
		}

		public List<TemplateDefinition> Templates
		{
			get
			{
				return this.templates;
			}
			set
			{
				this.templates = (value ?? new List<TemplateDefinition>());
			}
		}

		public Location BodyStartLocation
		{
			get
			{
				return this.bodyStartLocation;
			}
			set
			{
				this.bodyStartLocation = value;
			}
		}

		public TypeDeclaration(Modifiers modifier, List<AttributeSection> attributes) : base(attributes)
		{
			base.Modifier = modifier;
			this.name = "";
			this.baseTypes = new List<TypeReference>();
			this.templates = new List<TemplateDefinition>();
			this.bodyStartLocation = Location.Empty;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTypeDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TypeDeclaration Name={0} Type={1} BaseTypes={2} Templates={3} BodyStartLocation={4} Attributes={5} Modifier={6}]", new object[]
			{
				this.Name,
				this.Type,
				AbstractNode.GetCollectionString(this.BaseTypes),
				AbstractNode.GetCollectionString(this.Templates),
				this.BodyStartLocation,
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
