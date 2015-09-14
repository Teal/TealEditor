using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EventRemoveRegion : EventAddRemoveRegion
	{
		public static EventRemoveRegion Null
		{
			get
			{
				return NullEventRemoveRegion.Instance;
			}
		}

		public EventRemoveRegion(List<AttributeSection> attributes) : base(attributes)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEventRemoveRegion(this, data);
		}

		public override string ToString()
		{
			return string.Format("[EventRemoveRegion Block={0} Parameters={1} Attributes={2} Modifier={3}]", new object[]
			{
				base.Block,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
