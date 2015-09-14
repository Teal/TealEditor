using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IExpressionFinder
	{
		ExpressionResult FindExpression(string text, int offset);

		ExpressionResult FindFullExpression(string text, int offset);

		string RemoveLastPart(string expression);
	}
}
