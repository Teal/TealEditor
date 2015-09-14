using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class FieldReferenceExpression : Expression
	{
		private Expression targetObject;

		private string fieldName;

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

		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
			set
			{
				this.fieldName = (value ?? "");
			}
		}

		public FieldReferenceExpression(Expression targetObject, string fieldName)
		{
			this.TargetObject = targetObject;
			this.FieldName = fieldName;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitFieldReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[FieldReferenceExpression TargetObject={0} FieldName={1}]", this.TargetObject, this.FieldName);
		}
	}
}
