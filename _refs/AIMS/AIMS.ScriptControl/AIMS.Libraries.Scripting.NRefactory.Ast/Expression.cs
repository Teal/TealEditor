using System;
using System.Globalization;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public abstract class Expression : AbstractNode, INullable
	{
		public static NullExpression Null
		{
			get
			{
				return NullExpression.Instance;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public static Expression CheckNull(Expression expression)
		{
			return (expression == null) ? NullExpression.Instance : expression;
		}

		public static Expression AddInteger(Expression expr, int value)
		{
			PrimitiveExpression pe = expr as PrimitiveExpression;
			Expression result;
			if (pe != null && pe.Value is int)
			{
				int newVal = (int)pe.Value + value;
				result = new PrimitiveExpression(newVal, newVal.ToString(NumberFormatInfo.InvariantInfo));
			}
			else
			{
				BinaryOperatorExpression boe = expr as BinaryOperatorExpression;
				if (boe != null && boe.Op == BinaryOperatorType.Add)
				{
					boe.Right = Expression.AddInteger(boe.Right, value);
					if (boe.Right is PrimitiveExpression && ((PrimitiveExpression)boe.Right).Value is int)
					{
						int newVal = (int)((PrimitiveExpression)boe.Right).Value;
						if (newVal == 0)
						{
							result = boe.Left;
							return result;
						}
						if (newVal < 0)
						{
							((PrimitiveExpression)boe.Right).Value = -newVal;
							boe.Op = BinaryOperatorType.Subtract;
						}
					}
					result = boe;
				}
				else
				{
					if (boe != null && boe.Op == BinaryOperatorType.Subtract)
					{
						pe = (boe.Right as PrimitiveExpression);
						if (pe != null && pe.Value is int)
						{
							int newVal = (int)pe.Value - value;
							if (newVal == 0)
							{
								result = boe.Left;
								return result;
							}
							if (newVal < 0)
							{
								newVal = -newVal;
								boe.Op = BinaryOperatorType.Add;
							}
							boe.Right = new PrimitiveExpression(newVal, newVal.ToString(NumberFormatInfo.InvariantInfo));
							result = boe;
							return result;
						}
					}
					BinaryOperatorType opType = BinaryOperatorType.Add;
					if (value < 0)
					{
						value = -value;
						opType = BinaryOperatorType.Subtract;
					}
					result = new BinaryOperatorExpression(expr, opType, new PrimitiveExpression(value, value.ToString(NumberFormatInfo.InvariantInfo)));
				}
			}
			return result;
		}
	}
}
