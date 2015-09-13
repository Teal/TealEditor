using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class InvocationExpression : Expression
	{
		private Expression targetObject;

		private List<Expression> arguments;

		private List<TypeReference> typeArguments;

		public Expression TargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				this.targetObject = (value ?? Expression.Null);
				if (!this.targetObject.IsNull)
				{
					this.targetObject.Parent = this;
				}
			}
		}

		public List<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = (value ?? new List<Expression>());
			}
		}

		public List<TypeReference> TypeArguments
		{
			get
			{
				return this.typeArguments;
			}
			set
			{
				this.typeArguments = (value ?? new List<TypeReference>());
			}
		}

		public InvocationExpression(Expression targetObject)
		{
			this.TargetObject = targetObject;
			this.arguments = new List<Expression>();
			this.typeArguments = new List<TypeReference>();
		}

		public InvocationExpression(Expression targetObject, List<Expression> arguments)
		{
			this.TargetObject = targetObject;
			this.Arguments = arguments;
			this.typeArguments = new List<TypeReference>();
		}

		public InvocationExpression(Expression targetObject, List<Expression> arguments, List<TypeReference> typeArguments)
		{
			this.TargetObject = targetObject;
			this.Arguments = arguments;
			this.TypeArguments = typeArguments;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitInvocationExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[InvocationExpression TargetObject={0} Arguments={1} TypeArguments={2}]", this.TargetObject, AbstractNode.GetCollectionString(this.Arguments), AbstractNode.GetCollectionString(this.TypeArguments));
		}
	}
}
