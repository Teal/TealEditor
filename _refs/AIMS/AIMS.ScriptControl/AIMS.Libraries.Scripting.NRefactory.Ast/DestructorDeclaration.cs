using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DestructorDeclaration : AttributedNode
	{
		private string name;

		private BlockStatement body;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = (value ?? "");
			}
		}

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = (value ?? BlockStatement.Null);
				if (!this.body.IsNull)
				{
					this.body.Parent = this;
				}
			}
		}

		public DestructorDeclaration(string name, Modifiers modifier, List<AttributeSection> attributes) : base(attributes)
		{
			this.Name = name;
			base.Modifier = modifier;
			this.body = BlockStatement.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDestructorDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DestructorDeclaration Name={0} Body={1} Attributes={2} Modifier={3}]", new object[]
			{
				this.Name,
				this.Body,
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
