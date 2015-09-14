using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DefaultValueExpression : Expression
	{
		private TypeReference typeReference;

		public TypeReference TypeReference
		{
			get
			{
				return this.typeReference;
			}
			set
			{
				this.typeReference = (value ?? TypeReference.Null);
			}
		}

		public DefaultValueExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDefaultValueExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DefaultValueExpression TypeReference={0}]", this.TypeReference);
		}
	}
}
