using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public struct ExpressionResult
	{
		public string Expression;

		public ExpressionContext Context;

		public object Tag;

		public ExpressionResult(string expression)
		{
			this = new ExpressionResult(expression, ExpressionContext.Default, null);
		}

		public ExpressionResult(string expression, ExpressionContext context)
		{
			this = new ExpressionResult(expression, context, null);
		}

		public ExpressionResult(string expression, object tag)
		{
			this = new ExpressionResult(expression, ExpressionContext.Default, tag);
		}

		public ExpressionResult(string expression, ExpressionContext context, object tag)
		{
			this.Expression = expression;
			this.Context = context;
			this.Tag = tag;
		}

		public override string ToString()
		{
			string result;
			if (this.Context == ExpressionContext.Default)
			{
				result = "<" + this.Expression + ">";
			}
			else
			{
				result = string.Concat(new string[]
				{
					"<",
					this.Expression,
					"> (",
					this.Context.ToString(),
					")"
				});
			}
			return result;
		}
	}
}
