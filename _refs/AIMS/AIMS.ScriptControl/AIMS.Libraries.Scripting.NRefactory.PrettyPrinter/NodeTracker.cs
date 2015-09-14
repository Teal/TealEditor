using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Runtime.CompilerServices;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public class NodeTracker
	{
		private IAstVisitor callVisitor;

		public event InformNode NodeVisiting
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.NodeVisiting = (InformNode)Delegate.Combine(this.NodeVisiting, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.NodeVisiting = (InformNode)Delegate.Remove(this.NodeVisiting, value);
			}
		}

		public event InformNode NodeChildrenVisited
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.NodeChildrenVisited = (InformNode)Delegate.Combine(this.NodeChildrenVisited, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.NodeChildrenVisited = (InformNode)Delegate.Remove(this.NodeChildrenVisited, value);
			}
		}

		public event InformNode NodeVisited
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.NodeVisited = (InformNode)Delegate.Combine(this.NodeVisited, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.NodeVisited = (InformNode)Delegate.Remove(this.NodeVisited, value);
			}
		}

		public IAstVisitor CallVisitor
		{
			get
			{
				return this.callVisitor;
			}
		}

		public NodeTracker(IAstVisitor callVisitor)
		{
			this.callVisitor = callVisitor;
		}

		public void BeginNode(INode node)
		{
			if (this.NodeVisiting != null)
			{
				this.NodeVisiting(node);
			}
		}

		public void EndNode(INode node)
		{
			if (this.NodeVisited != null)
			{
				this.NodeVisited(node);
			}
		}

		public object TrackedVisit(INode node, object data)
		{
			this.BeginNode(node);
			object ret = node.AcceptVisitor(this.callVisitor, data);
			this.EndNode(node);
			return ret;
		}

		public object TrackedVisitChildren(INode node, object data)
		{
			foreach (INode child in node.Children)
			{
				this.TrackedVisit(child, data);
			}
			if (this.NodeChildrenVisited != null)
			{
				this.NodeChildrenVisited(node);
			}
			return data;
		}
	}
}
