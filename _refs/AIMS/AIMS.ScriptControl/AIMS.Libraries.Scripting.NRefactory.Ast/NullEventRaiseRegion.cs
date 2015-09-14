using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullEventRaiseRegion : EventRaiseRegion
	{
		private static NullEventRaiseRegion instance = new NullEventRaiseRegion();

		public static NullEventRaiseRegion Instance
		{
			get
			{
				return NullEventRaiseRegion.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullEventRaiseRegion() : base(null)
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullEventRaiseRegion]";
		}
	}
}
