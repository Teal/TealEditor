using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ObjectCreateExpression : Expression
	{
		private TypeReference createType;

		private List<Expression> parameters;

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

		public List<Expression> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new List<Expression>());
			}
		}

		public ObjectCreateExpression(TypeReference createType, List<Expression> parameters)
		{
			this.CreateType = createType;
			this.Parameters = parameters;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitObjectCreateExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ObjectCreateExpression CreateType={0} Parameters={1}]", this.CreateType, AbstractNode.GetCollectionString(this.Parameters));
		}
	}
}
