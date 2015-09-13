using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class PropertyGetRegion : PropertyGetSetRegion
	{
		public static PropertyGetRegion Null
		{
			get
			{
				return NullPropertyGetRegion.Instance;
			}
		}

		public PropertyGetRegion(BlockStatement block, List<AttributeSection> attributes) : base(block, attributes)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPropertyGetRegion(this, data);
		}

		public override string ToString()
		{
			return string.Format("[PropertyGetRegion Block={0} Attributes={1} Modifier={2}]", base.Block, AbstractNode.GetCollectionString(base.Attributes), base.Modifier);
		}
	}
}
