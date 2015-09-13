using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class MethodDeclaration : ParametrizedNode
	{
		private TypeReference typeReference;

		private BlockStatement body;

		private List<string> handlesClause;

		private List<InterfaceImplementation> interfaceImplementations;

		private List<TemplateDefinition> templates;

		public TypeReference TypeReference
		{
			get
			{
				return this.typeReference;
			}
			set
			{
				this.typeReference = (value ?? TypeReference.Null);
			}
		}

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = (value ?? BlockStatement.Null);
				if (!this.body.IsNull)
				{
					this.body.Parent = this;
				}
			}
		}

		public List<string> HandlesClause
		{
			get
			{
				return this.handlesClause;
			}
			set
			{
				this.handlesClause = (value ?? new List<string>());
			}
		}

		public List<InterfaceImplementation> InterfaceImplementations
		{
			get
			{
				return this.interfaceImplementations;
			}
			set
			{
				this.interfaceImplementations = (value ?? new List<InterfaceImplementation>());
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

		public MethodDeclaration(string name, Modifiers modifier, TypeReference typeReference, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = typeReference;
			this.body = BlockStatement.Null;
			this.handlesClause = new List<string>();
			this.interfaceImplementations = new List<InterfaceImplementation>();
			this.templates = new List<TemplateDefinition>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitMethodDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[MethodDeclaration TypeReference={0} Body={1} HandlesClause={2} InterfaceImplementations={3} Templates={4} Name={5} Parameters={6} Attributes={7} Modifier={8}]", new object[]
			{
				this.TypeReference,
				this.Body,
				AbstractNode.GetCollectionString(this.HandlesClause),
				AbstractNode.GetCollectionString(this.InterfaceImplementations),
				AbstractNode.GetCollectionString(this.Templates),
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
