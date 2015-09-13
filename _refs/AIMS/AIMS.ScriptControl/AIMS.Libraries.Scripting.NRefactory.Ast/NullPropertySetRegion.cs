using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullPropertySetRegion : PropertySetRegion
	{
		private static NullPropertySetRegion instance = new NullPropertySetRegion();

		public static NullPropertySetRegion Instance
		{
			get
			{
				return NullPropertySetRegion.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullPropertySetRegion() : base(null, null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullPropertySetRegion]";
		}
	}
}
