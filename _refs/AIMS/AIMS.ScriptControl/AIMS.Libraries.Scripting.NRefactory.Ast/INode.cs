using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public interface INode
	{
		INode Parent
		{
			get;
			set;
		}

		List<INode> Children
		{
			get;
		}

		Location StartLocation
		{
			get;
			set;
		}

		Location EndLocation
		{
			get;
			set;
		}

		object AcceptChildren(IAstVisitor visitor, object data);

		object AcceptVisitor(IAstVisitor visitor, object data);
	}
}
