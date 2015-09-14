using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class NamedArgumentExpression : Expression
	{
		private string name;

		private Expression expression;

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

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = (value ?? Expression.Null);
				if (!this.expression.IsNull)
				{
					this.expression.Parent = this;
				}
			}
		}

		public NamedArgumentExpression(string name, Expression expression)
		{
			this.Name = name;
			this.Expression = expression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitNamedArgumentExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[NamedArgumentExpression Name={0} Expression={1}]", this.Name, this.Expression);
		}
	}
}
