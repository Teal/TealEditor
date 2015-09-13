using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class AttributedNode : AbstractNode
	{
		private List<AttributeSection> attributes;

		private Modifiers modifier;

		public List<AttributeSection> Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = (value ?? new List<AttributeSection>());
			}
		}

		public Modifiers Modifier
		{
			get
			{
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		protected AttributedNode(List<AttributeSection> attributes)
		{
			this.Attributes = attributes;
		}

		protected AttributedNode(Modifiers modifier, List<AttributeSection> attributes)
		{
			this.Modifier = modifier;
			this.Attributes = attributes;
		}
	}
}
