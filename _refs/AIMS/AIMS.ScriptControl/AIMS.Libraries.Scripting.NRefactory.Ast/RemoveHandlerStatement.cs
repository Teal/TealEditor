using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class RemoveHandlerStatement : Statement
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

		public RemoveHandlerStatement(Expression eventExpression, Expression handlerExpression)
		{
			this.EventExpression = eventExpression;
			this.HandlerExpression = handlerExpression;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitRemoveHandlerStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[RemoveHandlerStatement EventExpression={0} HandlerExpression={1}]", this.EventExpression, this.HandlerExpression);
		}
	}
}
