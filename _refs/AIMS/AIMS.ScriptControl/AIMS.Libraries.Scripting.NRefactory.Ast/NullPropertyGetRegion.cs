using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullPropertyGetRegion : PropertyGetRegion
	{
		private static NullPropertyGetRegion instance = new NullPropertyGetRegion();

		public static NullPropertyGetRegion Instance
		{
			get
			{
				return NullPropertyGetRegion.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullPropertyGetRegion() : base(null, null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullPropertyGetRegion]";
		}
	}
}
