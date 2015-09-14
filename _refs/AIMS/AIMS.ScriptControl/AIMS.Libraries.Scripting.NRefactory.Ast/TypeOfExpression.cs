using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TypeOfExpression : Expression
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

		public TypeOfExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTypeOfExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TypeOfExpression TypeReference={0}]", this.TypeReference);
		}
	}
}
