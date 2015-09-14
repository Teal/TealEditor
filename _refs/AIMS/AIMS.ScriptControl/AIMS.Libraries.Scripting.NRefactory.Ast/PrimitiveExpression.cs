using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class PrimitiveExpression : Expression
	{
		private object val;

		private string stringValue;

		public object Value
		{
			get
			{
				return this.val;
			}
			set
			{
				this.val = value;
			}
		}

		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
			set
			{
				this.stringValue = ((value == null) ? string.Empty : value);
			}
		}

		public PrimitiveExpression(object val, string stringValue)
		{
			this.Value = val;
			this.StringValue = stringValue;
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitPrimitiveExpression(this, data);
		}

		public override string ToString()
		{
			return string.Format("[PrimitiveExpression: Value={1}, ValueType={2}, StringValue={0}]", this.stringValue, this.Value, (this.Value == null) ? "null" : this.Value.GetType().FullName);
		}
	}
}
