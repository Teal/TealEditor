using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NullBlockStatement : BlockStatement
	{
		private static NullBlockStatement nullBlockStatement = new NullBlockStatement();

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		public static NullBlockStatement Instance
		{
			get
			{
				return NullBlockStatement.nullBlockStatement;
			}
		}

		private NullBlockStatement()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return data;
		}

		public override object AcceptChildren(IAstVisitor visitor, object data)
		{
			return data;
		}

		public override void AddChild(INode childNode)
		{
			throw new InvalidOperationException();
		}

		public override string ToString()
		{
			return string.Format("[NullBlockStatement]", new object[0]);
		}
	}
}
