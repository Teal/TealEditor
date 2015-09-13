using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullArrayInitializerExpression : ArrayInitializerExpression
	{
		private static NullArrayInitializerExpression instance = new NullArrayInitializerExpression();

		public static NullArrayInitializerExpression Instance
		{
			get
			{
				return NullArrayInitializerExpression.instance;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		private NullArrayInitializerExpression()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return "[NullArrayInitializerExpression]";
		}
	}
}
