using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class PointerReferenceExpression : Expression
	{
		private Expression targetObject;

		private string identifier;

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

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = (value ?? "");
			}
		}

		public PointerReferenceExpression(Expression targetObject, string identifier)
		{
			this.TargetObject = targetObject;
			this.Identifier = identifier;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPointerReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[PointerReferenceExpression TargetObject={0} Identifier={1}]", this.TargetObject, this.Identifier);
		}
	}
}
