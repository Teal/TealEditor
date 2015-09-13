using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class AbstractNode : INode
	{
		private INode parent;

		private List<INode> children = new List<INode>();

		private Location startLocation;

		private Location endLocation;

		public INode Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public Location StartLocation
		{
			get
			{
				return this.startLocation;
			}
			set
			{
				this.startLocation = value;
			}
		}

		public Location EndLocation
		{
			get
			{
				return this.endLocation;
			}
			set
			{
				this.endLocation = value;
			}
		}

		public List<INode> Children
		{
			get
			{
				return this.children;
			}
			set
			{
				Debug.Assert(value != null);
				this.children = value;
			}
		}

		public virtual void AddChild(INode childNode)
		{
			Debug.Assert(childNode != null);
			this.children.Add(childNode);
		}

		public abstract object AcceptVisitor(IAstVisitor visitor, object data);

		public virtual object AcceptChildren(IAstVisitor visitor, object data)
		{
			foreach (INode child in this.children)
			{
				Debug.Assert(child != null);
				child.AcceptVisitor(visitor, data);
			}
			return data;
		}

		public static string GetCollectionString(ICollection collection)
		{
			StringBuilder output = new StringBuilder();
			output.Append('{');
			string result;
			if (collection != null)
			{
				IEnumerator en = collection.GetEnumerator();
				bool isFirst = true;
				while (en.MoveNext())
				{
					if (!isFirst)
					{
						output.Append(", ");
					}
					else
					{
						isFirst = false;
					}
					output.Append((en.Current == null) ? "<null>" : en.Current.ToString());
				}
				output.Append('}');
				result = output.ToString();
			}
			else
			{
				result = "null";
			}
			return result;
		}
	}
}
