using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class VariableDeclaration : AbstractNode
	{
		private string name;

		private Expression initializer;

		private TypeReference typeReference;

		private Expression fixedArrayInitialization;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = (value ?? "");
			}
		}

		public Expression Initializer
		{
			get
			{
				return this.initializer;
			}
			set
			{
				this.initializer = (value ?? Expression.Null);
				if (!this.initializer.IsNull)
				{
					this.initializer.Parent = this;
				}
			}
		}

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

		public Expression FixedArrayInitialization
		{
			get
			{
				return this.fixedArrayInitialization;
			}
			set
			{
				this.fixedArrayInitialization = (value ?? Expression.Null);
				if (!this.fixedArrayInitialization.IsNull)
				{
					this.fixedArrayInitialization.Parent = this;
				}
			}
		}

		public VariableDeclaration(string name)
		{
			this.Name = name;
			this.initializer = Expression.Null;
			this.typeReference = TypeReference.Null;
			this.fixedArrayInitialization = Expression.Null;
		}

		public VariableDeclaration(string name, Expression initializer)
		{
			this.Name = name;
			this.Initializer = initializer;
			this.typeReference = TypeReference.Null;
			this.fixedArrayInitialization = Expression.Null;
		}

		public VariableDeclaration(string name, Expression initializer, TypeReference typeReference)
		{
			this.Name = name;
			this.Initializer = initializer;
			this.TypeReference = typeReference;
			this.fixedArrayInitialization = Expression.Null;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitVariableDeclaration(this, data);
		}

		public override string ToString()
		{
			return string.Format("[VariableDeclaration Name={0} Initializer={1} TypeReference={2} FixedArrayInitialization={3}]", new object[]
			{
				this.Name,
				this.Initializer,
				this.TypeReference,
				this.FixedArrayInitialization
			});
		}
	}
}
