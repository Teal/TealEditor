using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IResolver
	{
		ResolveResult Resolve(ExpressionResult expressionResult, int caretLineNumber, int caretColumn, string fileName, string fileContent);

		ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context);
	}
}
