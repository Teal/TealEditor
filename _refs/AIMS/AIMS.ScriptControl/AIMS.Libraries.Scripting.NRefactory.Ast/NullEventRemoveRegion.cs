using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullEventRemoveRegion : EventRemoveRegion
	{
		private static NullEventRemoveRegion instance = new NullEventRemoveRegion();

		public static NullEventRemoveRegion Instance
		{
			get
			{
				return NullEventRemoveRegion.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullEventRemoveRegion() : base(null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullEventRemoveRegion]";
		}
	}
}
