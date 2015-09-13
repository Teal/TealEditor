using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
	public sealed class SpecialNodesInserter : IDisposable
	{
		private IEnumerator<ISpecial> enumerator;

		private SpecialOutputVisitor visitor;

		private bool available;

		private AttributedNode currentAttributedNode;

		public SpecialNodesInserter(IEnumerable<ISpecial> specials, SpecialOutputVisitor visitor)
		{
			if (specials == null)
			{
				throw new ArgumentNullException("specials");
			}
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			this.enumerator = specials.GetEnumerator();
			this.visitor = visitor;
			this.available = this.enumerator.MoveNext();
		}

		private void WriteCurrent()
		{
			this.enumerator.Current.AcceptVisitor(this.visitor, null);
			this.available = this.enumerator.MoveNext();
		}

		public void AcceptNodeStart(INode node)
		{
			if (node is AttributedNode)
			{
				this.currentAttributedNode = (node as AttributedNode);
				if (this.currentAttributedNode.Attributes.Count == 0)
				{
					this.AcceptPoint(node.StartLocation);
					this.currentAttributedNode = null;
				}
			}
			else
			{
				this.AcceptPoint(node.StartLocation);
			}
		}

		public void AcceptNodeEnd(INode node)
		{
			this.visitor.ForceWriteInPreviousLine = true;
			this.AcceptPoint(node.EndLocation);
			this.visitor.ForceWriteInPreviousLine = false;
			if (this.currentAttributedNode != null)
			{
				if (node == this.currentAttributedNode.Attributes[this.currentAttributedNode.Attributes.Count - 1])
				{
					this.AcceptPoint(this.currentAttributedNode.StartLocation);
					this.currentAttributedNode = null;
				}
			}
		}

		public void AcceptPoint(Location loc)
		{
			while (this.available && this.enumerator.Current.StartPosition <= loc)
			{
				this.WriteCurrent();
			}
		}

		public void Finish()
		{
			while (this.available)
			{
				this.WriteCurrent();
			}
		}

		void IDisposable.Dispose()
		{
			this.Finish();
		}

		public static SpecialNodesInserter Install(IEnumerable<ISpecial> specials, IOutputAstVisitor outputVisitor)
		{
			SpecialNodesInserter sni = new SpecialNodesInserter(specials, new SpecialOutputVisitor(outputVisitor.OutputFormatter));
			outputVisitor.NodeTracker.NodeVisiting += new InformNode(sni.AcceptNodeStart);
			outputVisitor.NodeTracker.NodeVisited += new InformNode(sni.AcceptNodeEnd);
			return sni;
		}
	}
}
