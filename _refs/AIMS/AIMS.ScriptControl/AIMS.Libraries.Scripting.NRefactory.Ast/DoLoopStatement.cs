using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class DoLoopStatement : StatementWithEmbeddedStatement
	{
		private Expression condition;

		private ConditionType conditionType;

		private ConditionPosition conditionPosition;

		public Expression Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = (value ?? Expression.Null);
				if (!this.condition.IsNull)
				{
					this.condition.Parent = this;
				}
			}
		}

		public ConditionType ConditionType
		{
			get
			{
				return this.conditionType;
			}
			set
			{
				this.conditionType = value;
			}
		}

		public ConditionPosition ConditionPosition
		{
			get
			{
				return this.conditionPosition;
			}
			set
			{
				this.conditionPosition = value;
			}
		}

		public DoLoopStatement(Expression condition, Statement embeddedStatement, ConditionType conditionType, ConditionPosition conditionPosition)
		{
			this.Condition = condition;
			base.EmbeddedStatement = embeddedStatement;
			this.ConditionType = conditionType;
			this.ConditionPosition = conditionPosition;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitDoLoopStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[DoLoopStatement Condition={0} ConditionType={1} ConditionPosition={2} EmbeddedStatement={3}]", new object[]
			{
				this.Condition,
				this.ConditionType,
				this.ConditionPosition,
				base.EmbeddedStatement
			});
		}
	}
}
