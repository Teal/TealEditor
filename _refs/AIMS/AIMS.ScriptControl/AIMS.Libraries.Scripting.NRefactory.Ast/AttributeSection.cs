using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class AttributeSection : AbstractNode
	{
		private string attributeTarget;

		private List<Attribute> attributes;

		public string AttributeTarget
		{
			get
			{
				return this.attributeTarget;
			}
			set
			{
				this.attributeTarget = (value ?? "");
			}
		}

		public List<Attribute> Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = (value ?? new List<Attribute>());
			}
		}

		public AttributeSection(string attributeTarget, List<Attribute> attributes)
		{
			this.AttributeTarget = attributeTarget;
			this.Attributes = attributes;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitAttributeSection(this, data);
		}

		public override string ToString()
		{
			return string.Format("[AttributeSection AttributeTarget={0} Attributes={1}]", this.AttributeTarget, AbstractNode.GetCollectionString(this.Attributes));
		}
	}
}
