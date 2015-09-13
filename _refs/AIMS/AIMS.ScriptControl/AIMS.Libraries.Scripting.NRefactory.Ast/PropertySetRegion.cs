using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class PropertySetRegion : PropertyGetSetRegion
	{
		private List<ParameterDeclarationExpression> parameters;

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

		public static PropertySetRegion Null
		{
			get
			{
				return NullPropertySetRegion.Instance;
			}
		}

		public PropertySetRegion(BlockStatement block, List<AttributeSection> attributes) : base(block, attributes)
		{
			this.parameters = new List<ParameterDeclarationExpression>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPropertySetRegion(this, data);
		}

		public override string ToString()
		{
			return string.Format("[PropertySetRegion Parameters={0} Block={1} Attributes={2} Modifier={3}]", new object[]
			{
				AbstractNode.GetCollectionString(this.Parameters),
				base.Block,
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
