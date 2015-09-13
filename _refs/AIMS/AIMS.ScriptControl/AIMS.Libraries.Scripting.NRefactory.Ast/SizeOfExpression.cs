using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class SizeOfExpression : Expression
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

		public SizeOfExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitSizeOfExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[SizeOfExpression TypeReference={0}]", this.TypeReference);
		}
	}
}
