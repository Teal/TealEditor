using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TypeReferenceExpression : Expression
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

		public TypeReferenceExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}

		public TypeReferenceExpression(string typeName) : this(new TypeReference(typeName))
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTypeReferenceExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[TypeReferenceExpression TypeReference={0}]", this.TypeReference);
		}
	}
}
