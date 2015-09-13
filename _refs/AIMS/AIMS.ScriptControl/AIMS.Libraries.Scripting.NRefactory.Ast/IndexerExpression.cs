using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class IndexerExpression : Expression
	{
		private Expression targetObject;

		private List<Expression> indexes;

		public Expression TargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				this.targetObject = (value ?? Expression.Null);
				if (!this.targetObject.IsNull)
				{
					this.targetObject.Parent = this;
				}
			}
		}

		public List<Expression> Indexes
		{
			get
			{
				return this.indexes;
			}
			set
			{
				this.indexes = (value ?? new List<Expression>());
			}
		}

		public IndexerExpression(Expression targetObject, List<Expression> indexes)
		{
			this.TargetObject = targetObject;
			this.Indexes = indexes;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitIndexerExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[IndexerExpression TargetObject={0} Indexes={1}]", this.TargetObject, AbstractNode.GetCollectionString(this.Indexes));
		}
	}
}
