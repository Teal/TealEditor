using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class ParametrizedNode : AttributedNode
	{
		private string name;

		private List<ParameterDeclarationExpression> parameters;

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

		protected ParametrizedNode(Modifiers modifier, List<AttributeSection> attributes, string name, List<ParameterDeclarationExpression> parameters) : base(attributes)
		{
			base.Modifier = modifier;
			this.Name = name;
			this.Parameters = parameters;
		}

		protected ParametrizedNode(Modifiers modifier, List<AttributeSection> attributes) : base(attributes)
		{
			base.Modifier = modifier;
			this.name = "";
			this.parameters = new List<ParameterDeclarationExpression>();
		}
	}
}
