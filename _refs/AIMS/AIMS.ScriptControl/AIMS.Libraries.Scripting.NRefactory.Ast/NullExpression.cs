using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullExpression : Expression
	{
		private static NullExpression nullExpression = new NullExpression();

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		public static NullExpression Instance
		{
			get
			{
				return NullExpression.nullExpression;
			}
		}

		private NullExpression()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return null;
		}

		public override string ToString()
		{
			return string.Format("[NullExpression]", new object[0]);
		}
	}
}
