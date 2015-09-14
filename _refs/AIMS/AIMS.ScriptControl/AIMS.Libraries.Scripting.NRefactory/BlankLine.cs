using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public class BlankLine : AbstractSpecial
	{
		public BlankLine(Location point) : base(point)
		{
		}

		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
