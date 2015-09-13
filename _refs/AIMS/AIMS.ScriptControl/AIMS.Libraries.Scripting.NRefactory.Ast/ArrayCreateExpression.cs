using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ArrayCreateExpression : Expression
	{
		private TypeReference createType;

		private List<Expression> arguments;

		private ArrayInitializerExpression arrayInitializer;

		public TypeReference CreateType
		{
			get
			{
				return this.createType;
			}
			set
			{
				this.createType = (value ?? TypeReference.Null);
			}
		}

		public List<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = (value ?? new List<Expression>());
			}
		}

		public ArrayInitializerExpression ArrayInitializer
		{
			get
			{
				return this.arrayInitializer;
			}
			set
			{
				this.arrayInitializer = (value ?? ArrayInitializerExpression.Null);
				if (!this.arrayInitializer.IsNull)
				{
					this.arrayInitializer.Parent = this;
				}
			}
		}

		public ArrayCreateExpression(TypeReference createType)
		{
			this.CreateType = createType;
			this.arguments = new List<Expression>();
			this.arrayInitializer = ArrayInitializerExpression.Null;
		}

		public ArrayCreateExpression(TypeReference createType, List<Expression> arguments)
		{
			this.CreateType = createType;
			this.Arguments = arguments;
			this.arrayInitializer = ArrayInitializerExpression.Null;
		}

		public ArrayCreateExpression(TypeReference createType, ArrayInitializerExpression arrayInitializer)
		{
			this.CreateType = createType;
			this.ArrayInitializer = arrayInitializer;
			this.arguments = new List<Expression>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitArrayCreateExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ArrayCreateExpression CreateType={0} Arguments={1} ArrayInitializer={2}]", this.CreateType, AbstractNode.GetCollectionString(this.Arguments), this.ArrayInitializer);
		}
	}
}
