using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DelegateDeclaration : AttributedNode
	{
		private string name;

		private TypeReference returnType;

		private List<ParameterDeclarationExpression> parameters;

		private List<TemplateDefinition> templates;

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

		public TypeReference ReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = (value ?? TypeReference.Null);
			}
		}

		public List<ParameterDeclarationExpression> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new List<ParameterDeclarationExpression>());
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

		public DelegateDeclaration(Modifiers modifier, List<AttributeSection> attributes) : base(attributes)
		{
			base.Modifier = modifier;
			this.name = "?";
			this.returnType = TypeReference.Null;
			this.parameters = new List<ParameterDeclarationExpression>();
			this.templates = new List<TemplateDefinition>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDelegateDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DelegateDeclaration Name={0} ReturnType={1} Parameters={2} Templates={3} Attributes={4} Modifier={5}]", new object[]
			{
				this.Name,
				this.ReturnType,
				AbstractNode.GetCollectionString(this.Parameters),
				AbstractNode.GetCollectionString(this.Templates),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
