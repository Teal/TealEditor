using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ConstructorDeclaration : ParametrizedNode
	{
		private ConstructorInitializer constructorInitializer;

		private BlockStatement body;

		public ConstructorInitializer ConstructorInitializer
		{
			get
			{
				return this.constructorInitializer;
			}
			set
			{
				this.constructorInitializer = (value ?? ConstructorInitializer.Null);
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

		public ConstructorDeclaration(string name, Modifiers modifier, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
			this.constructorInitializer = ConstructorInitializer.Null;
			this.body = BlockStatement.Null;
		}

		public ConstructorDeclaration(string name, Modifiers modifier, List<ParameterDeclarationExpression> parameters, ConstructorInitializer constructorInitializer, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
			this.ConstructorInitializer = constructorInitializer;
			this.body = BlockStatement.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitConstructorDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ConstructorDeclaration ConstructorInitializer={0} Body={1} Name={2} Parameters={3} Attributes={4} Modifier={5}]", new object[]
			{
				this.ConstructorInitializer,
				this.Body,
				base.Name,
				AbstractNode.GetCollectionString(base.Parameters),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
