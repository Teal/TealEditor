using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class BlockStatement : Statement
	{
		public new static NullBlockStatement Null
		{
			get
			{
				return NullBlockStatement.Instance;
			}
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitBlockStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[BlockStatement: Children={0}]", AbstractNode.GetCollectionString(base.Children));
		}
	}
}
