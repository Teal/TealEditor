using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullConstructorInitializer : ConstructorInitializer
	{
		private static NullConstructorInitializer instance = new NullConstructorInitializer();

		public static NullConstructorInitializer Instance
		{
			get
			{
				return NullConstructorInitializer.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullConstructorInitializer()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullConstructorInitializer]";
		}
	}
}
