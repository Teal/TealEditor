using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class EventRaiseRegion : EventAddRemoveRegion
	{
		public static EventRaiseRegion Null
		{
			get
			{
				return NullEventRaiseRegion.Instance;
			}
		}

		public EventRaiseRegion(List<AttributeSection> attributes) : base(attributes)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitEventRaiseRegion(this, data);
		}

		public override string ToString()
		{
			return string.Format("[EventRaiseRegion Block={0} Parameters={1} Attributes={2} Modifier={3}]", new object[]
			{
				base.Block,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
