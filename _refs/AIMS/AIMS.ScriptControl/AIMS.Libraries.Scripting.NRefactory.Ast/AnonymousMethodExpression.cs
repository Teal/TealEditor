using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class AnonymousMethodExpression : Expression
	{
		private List<ParameterDeclarationExpression> parameters;

		private BlockStatement body;

		private bool hasParameterList;

		public List<ParameterDeclarationExpression> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new List<ParameterDeclarationExpression>());
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

		public bool HasParameterList
		{
			get
			{
				return this.hasParameterList;
			}
			set
			{
				this.hasParameterList = value;
			}
		}

		public AnonymousMethodExpression()
		{
			this.parameters = new List<ParameterDeclarationExpression>();
			this.body = BlockStatement.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitAnonymousMethodExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[AnonymousMethodExpression Parameters={0} Body={1} HasParameterList={2}]", AbstractNode.GetCollectionString(this.Parameters), this.Body, this.HasParameterList);
		}
	}
}
