using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class ConstructorInitializer : AbstractNode, INullable
	{
		private ConstructorInitializerType constructorInitializerType;

		private List<Expression> arguments;

		public ConstructorInitializerType ConstructorInitializerType
		{
			get
			{
				return this.constructorInitializerType;
			}
			set
			{
				this.constructorInitializerType = value;
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

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public static ConstructorInitializer Null
		{
			get
			{
				return NullConstructorInitializer.Instance;
			}
		}

		public ConstructorInitializer()
		{
			this.arguments = new List<Expression>();
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitConstructorInitializer(this, data);
		}

		public override string ToString()
		{
			return string.Format("[ConstructorInitializer ConstructorInitializerType={0} Arguments={1}]", this.ConstructorInitializerType, AbstractNode.GetCollectionString(this.Arguments));
		}
	}
}
