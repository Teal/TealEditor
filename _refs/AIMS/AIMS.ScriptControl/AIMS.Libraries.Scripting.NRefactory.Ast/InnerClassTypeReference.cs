using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class InnerClassTypeReference : TypeReference
	{
		private TypeReference baseType;

		public TypeReference BaseType
		{
			get
			{
				return this.baseType;
			}
			set
			{
				this.baseType = value;
			}
		}

		public override TypeReference Clone()
		{
			InnerClassTypeReference c = new InnerClassTypeReference(this.baseType.Clone(), base.Type, base.GenericTypes);
			TypeReference.CopyFields(this, c);
			return c;
		}

		public InnerClassTypeReference(TypeReference outerClass, string innerType, List<TypeReference> innerGenericTypes) : base(innerType, innerGenericTypes)
		{
			this.baseType = outerClass;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitInnerClassTypeReference(this, data);
		}

		public TypeReference CombineToNormalTypeReference()
		{
			TypeReference tr = (this.baseType is InnerClassTypeReference) ? ((InnerClassTypeReference)this.baseType).CombineToNormalTypeReference() : this.baseType.Clone();
			TypeReference.CopyFields(this, tr);
			TypeReference expr_35 = tr;
			expr_35.Type = expr_35.Type + "." + base.Type;
			return tr;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[InnerClassTypeReference: (",
				this.baseType.ToString(),
				").",
				base.ToString(),
				"]"
			});
		}
	}
}
