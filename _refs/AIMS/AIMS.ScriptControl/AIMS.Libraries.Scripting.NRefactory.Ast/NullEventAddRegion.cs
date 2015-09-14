using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullEventAddRegion : EventAddRegion
	{
		private static NullEventAddRegion instance = new NullEventAddRegion();

		public static NullEventAddRegion Instance
		{
			get
			{
				return NullEventAddRegion.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullEventAddRegion() : base(null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullEventAddRegion]";
		}
	}
}
