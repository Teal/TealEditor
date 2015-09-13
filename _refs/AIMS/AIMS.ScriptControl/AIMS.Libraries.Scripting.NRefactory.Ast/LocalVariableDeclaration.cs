using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class LocalVariableDeclaration : Statement
	{
		private TypeReference typeReference;

		private Modifiers modifier;

		private List<VariableDeclaration> variables;

		public TypeReference TypeReference
		{
			get
			{
				return this.typeReference;
			}
			set
			{
				this.typeReference = TypeReference.CheckNull(value);
			}
		}

		public Modifiers Modifier
		{
			get
			{
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		public List<VariableDeclaration> Variables
		{
			get
			{
				return this.variables;
			}
		}

		public TypeReference GetTypeForVariable(int variableIndex)
		{
			TypeReference result;
			if (!this.typeReference.IsNull)
			{
				result = this.typeReference;
			}
			else
			{
				for (int i = variableIndex; i < this.Variables.Count; i++)
				{
					if (!this.Variables[i].TypeReference.IsNull)
					{
						result = this.Variables[i].TypeReference;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public LocalVariableDeclaration(VariableDeclaration declaration) : this(TypeReference.Null)
		{
			this.Variables.Add(declaration);
		}

		public LocalVariableDeclaration(TypeReference typeReference)
		{
			this.modifier = Modifiers.None;
			this.variables = new List<VariableDeclaration>(1);
			base..ctor();
			this.TypeReference = typeReference;
		}

		public LocalVariableDeclaration(TypeReference typeReference, Modifiers modifier)
		{
			this.modifier = Modifiers.None;
			this.variables = new List<VariableDeclaration>(1);
			base..ctor();
			this.TypeReference = typeReference;
			this.modifier = modifier;
		}

		public LocalVariableDeclaration(Modifiers modifier)
		{
			this.modifier = Modifiers.None;
			this.variables = new List<VariableDeclaration>(1);
			base..ctor();
			this.typeReference = TypeReference.Null;
			this.modifier = modifier;
		}

		public VariableDeclaration GetVariableDeclaration(string variableName)
		{
			VariableDeclaration result;
			foreach (VariableDeclaration variableDeclaration in this.variables)
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

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitLocalVariableDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[LocalVariableDeclaration: Type={0}, Modifier ={1} Variables={2}]", this.typeReference, this.modifier, AbstractNode.GetCollectionString(this.variables));
		}
	}
}
