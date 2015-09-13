using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EventAddRegion : EventAddRemoveRegion
	{
		public static EventAddRegion Null
		{
			get
			{
				return NullEventAddRegion.Instance;
			}
		}

		public EventAddRegion(List<AttributeSection> attributes) : base(attributes)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEventAddRegion(this, data);
		}

		public override string ToString()
		{
			return string.Format("[EventAddRegion Block={0} Parameters={1} Attributes={2} Modifier={3}]", new object[]
			{
				base.Block,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
