using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class CompilationUnit : AbstractNode
	{
		private Stack blockStack = new Stack();

		public INode CurrentBock
		{
			get
			{
				return (this.blockStack.Count > 0) ? ((INode)this.blockStack.Peek()) : null;
			}
		}

		public CompilationUnit()
		{
			this.blockStack.Push(this);
		}

		public void BlockStart(INode block)
		{
			this.blockStack.Push(block);
		}

		public void BlockEnd()
		{
			this.blockStack.Pop();
		}

		public override void AddChild(INode childNode)
		{
			if (childNode != null)
			{
				INode parent = (INode)this.blockStack.Peek();
				parent.Children.Add(childNode);
				childNode.Parent = parent;
			}
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitCompilationUnit(this, data);
		}

		public override string ToString()
		{
			return string.Format("[CompilationUnit]", new object[0]);
		}
	}
}
