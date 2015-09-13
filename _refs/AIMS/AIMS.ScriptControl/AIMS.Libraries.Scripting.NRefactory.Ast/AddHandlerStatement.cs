using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class AddHandlerStatement : Statement
	{
		private Expression eventExpression;

		private Expression handlerExpression;

		public Expression EventExpression
		{
			get
			{
				return this.eventExpression;
			}
			set
			{
				this.eventExpression = (value ?? Expression.Null);
				if (!this.eventExpression.IsNull)
				{
					this.eventExpression.Parent = this;
				}
			}
		}

		public Expression HandlerExpression
		{
			get
			{
				return this.handlerExpression;
			}
			set
			{
				this.handlerExpression = (value ?? Expression.Null);
				if (!this.handlerExpression.IsNull)
				{
					this.handlerExpression.Parent = this;
				}
			}
		}

		public AddHandlerStatement(Expression eventExpression, Expression handlerExpression)
		{
			this.EventExpression = eventExpression;
			this.HandlerExpression = handlerExpression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitAddHandlerStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[AddHandlerStatement EventExpression={0} HandlerExpression={1}]", this.EventExpression, this.HandlerExpression);
		}
	}
}
