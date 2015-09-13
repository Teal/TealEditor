using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class FixedStatement : StatementWithEmbeddedStatement
	{
		private TypeReference typeReference;

		private List<VariableDeclaration> pointerDeclarators;

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

		public List<VariableDeclaration> PointerDeclarators
		{
			get
			{
				return this.pointerDeclarators;
			}
			set
			{
				this.pointerDeclarators = (value ?? new List<VariableDeclaration>());
			}
		}

		public FixedStatement(TypeReference typeReference, List<VariableDeclaration> pointerDeclarators, Statement embeddedStatement)
		{
			this.TypeReference = typeReference;
			this.PointerDeclarators = pointerDeclarators;
			base.EmbeddedStatement = embeddedStatement;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitFixedStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[FixedStatement TypeReference={0} PointerDeclarators={1} EmbeddedStatement={2}]", this.TypeReference, AbstractNode.GetCollectionString(this.PointerDeclarators), base.EmbeddedStatement);
		}
	}
}
