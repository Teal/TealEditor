using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class RaiseEventStatement : Statement
	{
		private string eventName;

		private List<Expression> arguments;

		public string EventName
		{
			get
			{
				return this.eventName;
			}
			set
			{
				this.eventName = (value ?? "");
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

		public RaiseEventStatement(string eventName, List<Expression> arguments)
		{
			this.EventName = eventName;
			this.Arguments = arguments;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitRaiseEventStatement(this, data);
		}

		public override string ToString()
		{
			return string.Format("[RaiseEventStatement EventName={0} Arguments={1}]", this.EventName, AbstractNode.GetCollectionString(this.Arguments));
		}
	}
}
