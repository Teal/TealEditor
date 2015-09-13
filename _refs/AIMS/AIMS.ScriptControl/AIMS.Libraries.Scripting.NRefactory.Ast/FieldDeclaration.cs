using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class FieldDeclaration : AttributedNode
	{
		private TypeReference typeReference;

		private List<VariableDeclaration> fields;

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

		public List<VariableDeclaration> Fields
		{
			get
			{
				return this.fields;
			}
			set
			{
				this.fields = (value ?? new List<VariableDeclaration>());
			}
		}

		public FieldDeclaration(List<AttributeSection> attributes) : base(attributes)
		{
			this.typeReference = TypeReference.Null;
			this.fields = new List<VariableDeclaration>();
		}

		public FieldDeclaration(List<AttributeSection> attributes, TypeReference typeReference, Modifiers modifier) : base(attributes)
		{
			this.TypeReference = typeReference;
			base.Modifier = modifier;
			this.fields = new List<VariableDeclaration>();
		}

		public VariableDeclaration GetVariableDeclaration(string variableName)
		{
			VariableDeclaration result;
			foreach (VariableDeclaration variableDeclaration in this.Fields)
			{
				if (variableDeclaration.Name == variableName)
				{
					result = variableDeclaration;
					return result;
				}
			}
			result = null;
			return result;
		}

		public TypeReference GetTypeForField(int fieldIndex)
		{
			TypeReference result;
			if (!this.typeReference.IsNull)
			{
				result = this.typeReference;
			}
			else
			{
				result = this.Fields[fieldIndex].TypeReference;
			}
			return result;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitFieldDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[FieldDeclaration TypeReference={0} Fields={1} Attributes={2} Modifier={3}]", new object[]
			{
				this.TypeReference,
				AbstractNode.GetCollectionString(this.Fields),
				AbstractNode.GetCollectionString(base.Attributes),
				base.Modifier
			});
		}
	}
}
