using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullStatement : Statement
	{
		private static NullStatement nullStatement = new NullStatement();

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		public static NullStatement Instance
		{
			get
			{
				return NullStatement.nullStatement;
			}
		}

		private NullStatement()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return data;
		}

		public override string ToString()
		{
			return string.Format("[NullStatement]", new object[0]);
		}
	}
}
