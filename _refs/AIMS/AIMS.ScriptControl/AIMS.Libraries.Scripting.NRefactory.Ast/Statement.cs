using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class Statement : AbstractNode, INullable
	{
		public static NullStatement Null
		{
			get
			{
				return NullStatement.Instance;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public static Statement CheckNull(Statement statement)
		{
			return statement ?? NullStatement.Instance;
		}
	}
}
